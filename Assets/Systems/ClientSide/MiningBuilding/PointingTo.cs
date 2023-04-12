using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(ControllerSettings))]
public class PointingTo : MonoBehaviour {
    ControllerSettings settings;
    public Material wireframeMaterial;

    float timeDelayActions = 0;
    float thresholdTime = 0.1f;
    GameObject wireframeGameObject;
    
    private MineManager mineManager;
    private BuildManager buildManager;
    
	private ChunkLoader chunkLoader;
	
    void Start() {
		chunkLoader = gameObject.GetComponent<ChunkLoader>();
        settings = GetComponent<ControllerSettings>();
        wireframeGameObject = new GameObject("Pointing Wireframe", typeof(MeshFilter), typeof(MeshRenderer));

        mineManager = new MineManager();
        Transform character = transform;
        Inventory inventory = gameObject.GetComponent<Inventory>();
        buildManager = new BuildManager(character, inventory);
    }
    
    public RaycastHit RaycastFromCamera(){
        RaycastHit hit;
        //Debug.DrawRay(settings.CameraTransform.position, settings.CameraTransform.forward*settings.reach);
        Physics.Raycast(settings.CameraTransform.position, settings.CameraTransform.forward, out hit, settings.reach, settings.groundedMask);
        return hit;
    }
    
    public Vector3Int ChunkHit(RaycastHit hit) {
        string chunkName = hit.collider.transform.gameObject.name;
        string[] coord = chunkName.Split("(")[1].Split(")")[0].Split(",");
        int sideCoord = int.Parse(coord[0]);
        int xCoord = int.Parse(coord[1]);
        int zCoord = int.Parse(coord[2]);

        return new Vector3Int(sideCoord, xCoord, zCoord);
    }
    
    public BlockCoordinateInformation BlockToMineOrBuild(RaycastHit hit, Vector3Int chunkCoord, bool build = false, float blockSkin = 0.2f) {
        if (build == false) {
            return BlockClosestTo(hit.point - blockSkin*hit.normal, chunkCoord);
        }
        else {
            return BlockClosestTo(hit.point + blockSkin*hit.normal, chunkCoord);
        }
    }

    // IMPORTANT! Chunk is the chunk that was interesect. not necessarily the chunk where the block im placing is. May be
    // in adjacent
    public BlockCoordinateInformation BlockClosestTo(Vector3 point, Vector3Int chunkCoord) {
        Vector3 pointFromPlanetRef = chunkLoader.GetCurrentPlanet().transform.InverseTransformPoint(point);

        int sideCoord = chunkCoord.x;
        Vector3Int sideXaxis = TerrainGenerationConstants.sideXaxisList[sideCoord]; 
        Vector3Int sideYaxis = TerrainGenerationConstants.sideYaxisList[sideCoord]; 
        Vector3Int sideZaxis = TerrainGenerationConstants.sideZaxisList[sideCoord]; 
        PlanetTerrain planet = chunkLoader.GetCurrentPlanetTerrain();
        int chunksPerSide = planet.GetChunksPerSide();
        
        float height = pointFromPlanetRef.magnitude;
        int hBlock = (int)((height - TerrainGenerationConstants.GetCoreRadius()) / TerrainGenerationConstants.GetBlockHeight());
        if (hBlock >= planet.GetChunkHeight() || hBlock < planet.GetChunkMinHeight())
        {
            return null;
        }

        int realChunkSize = Mathf.Max(1, planet.NumBlocksAtHeightPerChunk(hBlock));
        Vector3 dir = pointFromPlanetRef.normalized;
        float normInf = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)); 
        Vector3 cubeDir = dir / normInf;
        

        int newSideCoord = 0;
        if (cubeDir.y == 1) newSideCoord = 0;
        else if (cubeDir.y == -1) newSideCoord = 1;
        else if (cubeDir.x == 1) newSideCoord = 2;
        else if (cubeDir.x == -1) newSideCoord = 3;
        else if (cubeDir.z == 1) newSideCoord = 4;
        else if (cubeDir.z == -1) newSideCoord = 5;
        
        float xPointOnPlane = 0.5f*(Vector3.Dot(TerrainGenerationConstants.sideXaxisList[newSideCoord], cubeDir) + 1);
        float zPointOnPlane = 0.5f*(Vector3.Dot(TerrainGenerationConstants.sideZaxisList[newSideCoord], cubeDir) + 1);
        
        int xGlobal = (int) (xPointOnPlane * (realChunkSize * chunksPerSide));
        int xChunk = xGlobal / realChunkSize;
        int xBlock = xGlobal % realChunkSize;
        int zGlobal = (int) (zPointOnPlane * (realChunkSize * chunksPerSide));
        int zChunk = zGlobal / realChunkSize;
        int zBlock = zGlobal % realChunkSize;
        
        return new BlockCoordinateInformation(
            new Vector3Int(xBlock, hBlock, zBlock), 
            new Vector3Int(newSideCoord, xChunk, zChunk),
            planet
            );
    }

    public BlockCoordinateInformation BlockPointingTo(bool build = false) {
        RaycastHit hit = RaycastFromCamera();
        if (hit.colliderInstanceID == 0) return null;
        Vector3Int chunkHitCoord = ChunkHit(hit);
        return BlockToMineOrBuild(hit, chunkHitCoord, build);
    }

    void WireFrame(BlockCoordinateInformation blockPointedCoords)
    {
        if (blockPointedCoords == null)
        {
            wireframeGameObject.SetActive(false);
            return;
        }
        Block blockPointed = blockPointedCoords.GetBlockIfExists();
        if (blockPointed == null)
        {
            wireframeGameObject.SetActive(false);
            return;
        }
        Mesh mesh = blockPointed.ComputeOutline();
        
        wireframeGameObject.GetComponent<MeshFilter>().mesh = mesh;
        wireframeGameObject.GetComponent<MeshRenderer>().material = wireframeMaterial;
        GameObject planet = chunkLoader.GetCurrentPlanet();
        wireframeGameObject.transform.position = planet.transform.position;
        wireframeGameObject.transform.rotation = planet.transform.rotation;
        wireframeGameObject.transform.SetParent(planet.transform);
        wireframeGameObject.SetActive(true);
    }
    
    void FixedUpdate() {
        BlockCoordinateInformation blockPointedCoords = BlockPointingTo();
        WireFrame(blockPointedCoords);
        timeDelayActions += Time.fixedDeltaTime;
        
        bool wantToBreak = Input.GetButton("Fire1");
        bool wantToBuild = Input.GetButton("Fire2");

        bool successfulAction = false;
        if (wantToBreak && timeDelayActions > thresholdTime) {
            successfulAction = mineManager.Mine(blockPointedCoords);
        }
        else if (wantToBuild && timeDelayActions > thresholdTime)
        {
            blockPointedCoords = BlockPointingTo(true);
            successfulAction = buildManager.Build(blockPointedCoords);
        }

        if (successfulAction)
        {
            timeDelayActions = 0;
            chunkLoader.ReloadChunksAfterMineBuildChange(blockPointedCoords);
            blockPointedCoords.RecomputeAmbientOcclusionsOfAllNeighbors();
        }
    }
}