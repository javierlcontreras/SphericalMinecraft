using System.Collections;
using System.Collections.Generic;
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

    public Vector3Int BlockIndexToBreak(RaycastHit hit, Chunk chunk, bool place = false, float blockSkin = 0.2f) {
        if (place == false) {
            return BlockToClosestTo(hit.point - blockSkin*hit.normal, chunk);
        }
        else {
            return BlockToClosestTo(hit.point + blockSkin*hit.normal, chunk);
        }
    }

    public Vector3Int BlockToClosestTo(Vector3 point, Chunk chunk) {
        Vector3 pointFromPlanetRef = chunkLoader.GetCurrentPlanet().transform.InverseTransformPoint(point);

        int sideCoord = chunk.GetSideCoord();
        Vector3 sideXaxis = TerrainGenerationConstants.sideXaxisList[sideCoord]; 
        Vector3 sideYaxis = TerrainGenerationConstants.sideYaxisList[sideCoord]; 
        Vector3 sideZaxis = TerrainGenerationConstants.sideZaxisList[sideCoord]; 
        PlanetTerrain planet = chunkLoader.GetCurrentPlanetTerrain();
        int chunkSize = planet.GetChunkSize();
        int chunksPerSide = planet.GetChunksPerSide();
        
        float height = pointFromPlanetRef.magnitude;
        int hBlock = (int)(height - TerrainGenerationConstants.GetCoreRadius());
        int realChunkSize = Mathf.Max(1, planet.NumBlocksAtHeightPerChunk(hBlock));
        int mult = chunkSize / realChunkSize;
        Vector3 dir = pointFromPlanetRef.normalized;
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

        return new Vector3Int(xBlock, hBlock, zBlock);
    }

    public Block BlockToBreak(bool place = false) {
        RaycastHit hit = RaycastFromCamera();
        if (hit.colliderInstanceID == 0) return null;
        Chunk chunk = ChunkHit(hit);
        Vector3Int blockIndex = BlockIndexToBreak(hit, chunk, place);
        if (place && chunk.GetBlock(blockIndex.x, blockIndex.y, blockIndex.z) == null) {
            BlockType type = BlockTypeEnum.GetBlockTypeByName("invalid"); 
            chunk.SetBlock(blockIndex.x, blockIndex.y, blockIndex.z, new Block(blockIndex, type, chunk));
        }
        return chunk.GetBlock(blockIndex.x, blockIndex.y, blockIndex.z);
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
        }
        else if (blockPointed != null && blockPointed.GetBlockType().GetName() != "bedrock") { 
            Chunk chunkPointed = blockPointed.GetChunk();
            Vector3Int blockIndex = blockPointed.GetInChunkIndex();
            int x = blockIndex.x;
            int y = blockIndex.y;
            int z = blockIndex.z;
            chunkPointed.SetBlock(x,y,z,null);
            SuccessfulChange(chunkPointed);
        }
    }
    public void Place() {
        Block blockPointed = BlockToBreak(true);
        string blockTypeName = blockPointed.GetBlockType().GetName();
        if (blockTypeName != "air" && blockTypeName != "invalid") {
            Debug.LogWarning("BUG Want to place something in a non-air");
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
        SuccessfulChange(chunkPointed);
    
    }
    void SuccessfulChange(Chunk chunkPointed) {
        chunkLoader.RegenerateChunkMesh(chunkLoader.GetCurrentPlanetTerrain(), 
                                        chunkPointed.GetSideCoord(), 
                                        chunkPointed.GetXCoord(), 
                                        chunkPointed.GetZCoord());
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