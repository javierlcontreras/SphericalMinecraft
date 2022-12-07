using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ControllerSettings))]
public class PointingTo : MonoBehaviour {
    ControllerSettings settings;

    public RaycastHit GlobalPointingPoint(){
        RaycastHit hit;
        Debug.DrawRay(settings.CameraTransform.position, settings.CameraTransform.forward*settings.reach);
        Physics.Raycast(settings.CameraTransform.position, settings.CameraTransform.forward, out hit, settings.reach, settings.groundedMask);
        return hit;
    }

    public Block BlockToBreak(bool place = false, float blockSkin = 0.2f) {
        RaycastHit hit = GlobalPointingPoint();
        if (hit.colliderInstanceID == 0) return null;
        string chunkName = hit.collider.transform.gameObject.name;
        string[] coord = chunkName.Split("(")[1].Split(")")[0].Split(",");
        int sideCoord = int.Parse(coord[0]);
        int xCoord = int.Parse(coord[1]);
        int zCoord = int.Parse(coord[2]);
        //Debug.Log("Just hit: " + chunkName);
        Chunk chunk = TerrainManager.instance.GetCurrentPlanet().chunks[sideCoord, xCoord, zCoord];
        if (place == false) {
            return BlockToClosestTo(hit.point - blockSkin*hit.normal, chunk);
        }
        else {
            return BlockToClosestTo(hit.point + blockSkin*hit.normal, chunk);
        }
    }

    public Block BlockToClosestTo(Vector3 point, Chunk chunk) {
        int sideCoord = chunk.GetSideCoord();
        Vector3 sideXaxis = TerrainManager.sideXaxisList[sideCoord]; 
        Vector3 sideYaxis = TerrainManager.sideYaxisList[sideCoord]; 
        Vector3 sideZaxis = TerrainManager.sideZaxisList[sideCoord]; 
        Planet planet = TerrainManager.instance.GetCurrentPlanet();
        int chunkSize = planet.GetChunkSize();
        int chunksPerSide = planet.GetChunksPerSide();
        
        float height = point.magnitude;
        int hBlock = (int)(height - TerrainManager.instance.GetCoreRadius());
        int realChunkSize = Mathf.Max(1, planet.NumBlocksAtHeightPerChunk(hBlock));
        int mult = chunkSize / realChunkSize;
        Vector3 dir = point.normalized;
        float normInf = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)); 
        Vector3 cubeDir = dir / normInf;

        int side = chunk.GetSideCoord();
        float xPointOnPlane = 0.5f*(Vector3.Dot(sideXaxis, cubeDir) + 1);
        float zPointOnPlane = 0.5f*(Vector3.Dot(sideZaxis, cubeDir) + 1);
        
        int xGlobal = (int) (xPointOnPlane * chunkSize*chunksPerSide);
        int zGlobal = (int) (zPointOnPlane * chunkSize*chunksPerSide);

        int xBlock = xGlobal % chunkSize;
        int zBlock = zGlobal % chunkSize;
        xBlock /= mult; 
        zBlock /= mult; 

        return chunk.blocks[xBlock, hBlock, zBlock];
    }

    GameObject pointingTo;
    void Start() {
        settings = GetComponent<ControllerSettings>();
        pointingTo = new GameObject("Pointing to", typeof(MeshFilter), typeof(MeshRenderer));
    }

    
    void Update() {
        Block blockPointed = BlockToBreak();
        Mesh mesh = null;
        if (blockPointed != null) {
            mesh = blockPointed.ComputeOutline();
        }
        pointingTo.GetComponent<MeshFilter>().mesh = mesh;
        pointingTo.GetComponent<MeshRenderer>().material = TerrainManager.instance.wireframeMaterial;

    }

    float timeDelayActions = 0;
    float thresholdTime = 0.1f;
    void FixedUpdate() {
        timeDelayActions += Time.fixedDeltaTime;
        bool success = false;
        bool wantToBreak = Input.GetButton("Fire1");
        bool wantToPlace = Input.GetButton("Fire2");
        Chunk chunkPointed = null;
        if (wantToBreak && timeDelayActions > thresholdTime) {
            Block blockPointed = BlockToBreak();
            if (blockPointed != null && blockPointed.GetBlockType().GetName() == "air") {
                Debug.Log("BUG");
            }
            else if (blockPointed != null && blockPointed.GetInChunkPosition().y >= 1) { 
                chunkPointed = blockPointed.GetChunk();
                blockPointed.SetBlockType(BlockTypeEnum.GetBlockTypeByName("air"));
                success = true;
            }
        }
        else if (wantToPlace && timeDelayActions > thresholdTime) {
            Block blockPointed = BlockToBreak(true);
            if (blockPointed != null && blockPointed.GetBlockType().GetName() != "air") {
                Debug.Log("BUG");
            }
            else if (blockPointed != null) { 
                chunkPointed = blockPointed.GetChunk();
                blockPointed.SetBlockType(BlockTypeEnum.GetBlockTypeByName("sand"));
                success = true;
            }
        }
        if (success) {
            Planet planet = TerrainManager.instance.GetCurrentPlanet();
            planet.DestroyChunkMesh(chunkPointed.GetSideCoord(), chunkPointed.GetXCoord(), chunkPointed.GetZCoord());
            planet.GenerateChunkMesh(chunkPointed.GetSideCoord(), chunkPointed.GetXCoord(), chunkPointed.GetZCoord());
            timeDelayActions = 0;
        }
    }
}