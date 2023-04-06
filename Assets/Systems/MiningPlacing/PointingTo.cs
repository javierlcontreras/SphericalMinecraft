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
    GameObject pointingTo;

    private Inventory inventory;
    private Transform character;

	private ChunkLoader chunkLoader;
	
    void Start() {
		chunkLoader = gameObject.GetComponent<ChunkLoader>();
        character = transform;
        inventory = gameObject.GetComponent<Inventory>();
        settings = GetComponent<ControllerSettings>();
        pointingTo = new GameObject("Pointing Wireframe", typeof(MeshFilter), typeof(MeshRenderer));
    }
    
    public RaycastHit RaycastFromCamera(){
        RaycastHit hit;
        //Debug.DrawRay(settings.CameraTransform.position, settings.CameraTransform.forward*settings.reach);
        Physics.Raycast(settings.CameraTransform.position, settings.CameraTransform.forward, out hit, settings.reach, settings.groundedMask);
        return hit;
    }
    
    public Chunk ChunkHit(RaycastHit hit) {
        string chunkName = hit.collider.transform.gameObject.name;
        string[] coord = chunkName.Split("(")[1].Split(")")[0].Split(",");
        int sideCoord = int.Parse(coord[0]);
        int xCoord = int.Parse(coord[1]);
        int zCoord = int.Parse(coord[2]);

        Chunk chunk = chunkLoader.GetCurrentPlanetTerrain().chunks[sideCoord, xCoord, zCoord];
        return chunk;
    }
    
    public Block BlockToBreakOrPlace(RaycastHit hit, Chunk chunk, bool place = false, float blockSkin = 0.2f) {
        if (place == false) {
            return BlockClosestTo(hit.point - blockSkin*hit.normal, chunk);
        }
        else {
            return BlockClosestTo(hit.point + blockSkin*hit.normal, chunk);
        }
    }

    // IMPORTANT! Chunk is the chunk that was interesect. not necessarily the chunk where the block im placing is. May be
    // in adjacent
    public Block BlockClosestTo(Vector3 point, Chunk chunk) {
        Vector3 pointFromPlanetRef = chunkLoader.GetCurrentPlanet().transform.InverseTransformPoint(point);

        int sideCoord = chunk.GetSideCoord();
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
        
        return planet.chunks[newSideCoord, xChunk, zChunk].GetBlock(xBlock, hBlock, zBlock);
    }

    public Vector3Int OutofChunkDifferentialFromBlockCoord(int x, int z, int s)
    {
        int dx = 0, dz = 0;
        if (x == s) dx = 1;
        if (x == -1) dx = -1;
        if (z == s) dz = 1;
        if (z == -1) dz = -1;
        return new Vector3Int(dx, 0, dz);
    }
    
    public Block BlockToBreak(bool place = false) {
        RaycastHit hit = RaycastFromCamera();
        if (hit.colliderInstanceID == 0) return null;
        Chunk chunkHit = ChunkHit(hit);
        return BlockToBreakOrPlace(hit, chunkHit, place);
    }


    void WireFrame() {
        Block blockPointed = BlockToBreak();
        Mesh mesh = null;
        if (blockPointed != null) {
            mesh = blockPointed.ComputeOutline();
        }
        pointingTo.GetComponent<MeshFilter>().mesh = mesh;
        pointingTo.GetComponent<MeshRenderer>().material = wireframeMaterial;
        GameObject planet = chunkLoader.GetCurrentPlanet();
        pointingTo.transform.position = planet.transform.position;
        pointingTo.transform.rotation = planet.transform.rotation;
        pointingTo.transform.SetParent(planet.transform);
    }

    public void Mine() {
        Block blockPointed = BlockToBreak();
        if (blockPointed != null && blockPointed.GetBlockType().GetName() == "air") {
            Debug.LogWarning("BUG Want to break a air");
            return;
        }
        
        if (blockPointed != null && blockPointed.GetBlockType().GetName() != "bedrock") { 
            Chunk chunkPointed = blockPointed.GetChunk();
            Vector3Int blockIndex = blockPointed.GetInChunkIndex();
            int x = blockIndex.x;
            int y = blockIndex.y;
            int z = blockIndex.z;
            chunkPointed.SetBlock(x,y,z,null);
            
            ReloadChunksAfterChange(chunkPointed, blockPointed);
        }
    }
    public void Place() {
        Block blockPointed = BlockToBreak(true);
        if (blockPointed == null)
        {
            return;
        }

        string blockTypeName = blockPointed.GetBlockType().GetName();
        if (blockTypeName != "air" && blockTypeName != "invalid") {
            Debug.LogWarning("BUG Want to place something in a non-air");
            Debug.LogWarning(blockTypeName + " "+ blockPointed.GetInChunkIndex());
            return;
        }
        Chunk chunkPointed = blockPointed.GetChunk();
        if ((blockPointed.GetBlockGlobalPosition() - character.position).magnitude < 1.5f) {
            Vector3Int blockIndex = blockPointed.GetInChunkIndex();
            int x = blockIndex.x;
            int y = blockIndex.y;
            int z = blockIndex.z;
            chunkPointed.SetBlock(x,y,z,null);
            return;
        }

        BlockType type = inventory.GetSlot(inventory.GetSelectedSlotIndex()).GetBlockType();
        blockPointed.SetBlockType(type);

        ReloadChunksAfterChange(chunkPointed, blockPointed);
    }

    public void ReloadChunksAfterChange(Chunk chunkPointed, Block blockPointed) 
    {
        ChunkAdjacencyCalculator chunkAdjacencyCalculator = chunkPointed.GetPlanet().GetPlanetMeshGenerator().GetChunkAdjacencyCalculator();

        Vector3Int chunkCoords = chunkPointed.GetChunkCoords();
        Vector3Int blockIndex = blockPointed.GetInChunkIndex();
        int len = chunkPointed.GetPlanet().NumBlocksAtHeightPerChunk(blockIndex.y);
        SuccessfulChangeInChunk(chunkPointed.GetPlanet(), chunkCoords);
        if (blockIndex.x == 0) SuccessfulChangeInChunk(chunkPointed.GetPlanet(),chunkAdjacencyCalculator.ChunkNextToMe(chunkCoords,-1, 0));
        if (blockIndex.z == 0) SuccessfulChangeInChunk(chunkPointed.GetPlanet(),chunkAdjacencyCalculator.ChunkNextToMe(chunkCoords,0, -1));
        if (blockIndex.x == len-1) SuccessfulChangeInChunk(chunkPointed.GetPlanet(),chunkAdjacencyCalculator.ChunkNextToMe(chunkCoords,1, 0));
        if (blockIndex.z == len-1) SuccessfulChangeInChunk(chunkPointed.GetPlanet(),chunkAdjacencyCalculator.ChunkNextToMe(chunkCoords,0, 1));
    }
    void SuccessfulChangeInChunk(PlanetTerrain planet, Vector3Int chunkCoord)
    {
        chunkLoader.RegenerateChunkMesh(planet,
            chunkCoord.x, chunkCoord.y, chunkCoord.z);
        timeDelayActions = 0;
    }

    void MineAndPlace() {
        bool wantToBreak = Input.GetButton("Fire1");
        bool wantToPlace = Input.GetButton("Fire2");
        if (wantToBreak && timeDelayActions > thresholdTime) {
            Mine();
        }
        else if (wantToPlace && timeDelayActions > thresholdTime) {
            Place();
        }
    }

    void FixedUpdate() {
        WireFrame();
        timeDelayActions += Time.fixedDeltaTime;
        MineAndPlace();
    }
}