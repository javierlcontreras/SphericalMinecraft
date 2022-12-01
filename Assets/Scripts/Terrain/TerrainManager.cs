using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{
    private const int chunksPerSide = 2;
    public int ChunksPerSide => chunksPerSide;
    private const int chunkSize = 16;
    public int ChunkSize => chunkSize;
    private const int chunkHeight = 64;
    public int ChunkHeight => chunkHeight;
    private const int textureBlockSize = 128;
    public int TextureBlockSize => textureBlockSize;
    private const int textureAtlasSize = 2048;
    public int TextureAtlasSize => textureAtlasSize;
    private float planetRadius;
    public float PlanetRadius => planetRadius;

    
    public Material textureMaterial;

    public Transform currentPosition;
    public float radiusOfLoad;

    public string[] sideNameList = new string[] {
        "up",
        "down",
        "right",
        "left",
        "forward",
        "back"
    };
    public Vector3[] sideYaxisList = new Vector3[]{
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    public Vector3[] sideXaxisList = new Vector3[]{
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    public Vector3[] sideZaxisList = new Vector3[]{
        Vector3.left,
        Vector3.left,
        Vector3.back,
        Vector3.back,
        Vector3.down,
        Vector3.down
    };
    public Vector3 BaseVector(int side, int chunkX, int chunkZ, int blockX, int blockZ) {
        Vector3 normal = sideYaxisList[side];
        Vector3 xAxis = sideXaxisList[side];
        Vector3 zAxis = sideZaxisList[side];
        int numBlocks = chunksPerSide*chunkSize;
        float x = chunkX*chunkSize + blockX - numBlocks/2f;
        float z = chunkZ*chunkSize + blockZ - numBlocks/2f;
        Vector3 radius = normal*planetRadius + x*xAxis*GetBlockSize() + z*zAxis*GetBlockSize();
        return radius.normalized;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        int center = chunkSize/2;
        return BaseVector(side, chunkX, chunkZ, center, center);
    }
    public float GetBlockSize() {
        return 2f*planetRadius/(chunksPerSide*chunkSize);
    }

    public Block BlockClosestToGlobalPoint(Vector3 point) {
        Vector3 dir = point.normalized;
        float normInf = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)); 
        Vector3 cubeDir = dir / normInf;

        int side = SideFromPoint(point);
        
        float xPointOnPlane = 0.5f*(Vector3.Dot(sideXaxisList[side], cubeDir) + 1);
        float zPointOnPlane = 0.5f*(Vector3.Dot(sideZaxisList[side], cubeDir) + 1);
        
        int xGlobal = (int) (xPointOnPlane * chunkSize*chunksPerSide); 
        int zGlobal = (int) (zPointOnPlane * chunkSize*chunksPerSide);

        int xChunk = xGlobal / chunkSize;
        int zChunk = zGlobal / chunkSize;
        int xBlock = xGlobal % chunkSize;
        int zBlock = zGlobal % chunkSize;

        float height = point.magnitude;
        int hBlock = (int) ((point.magnitude - planetRadius)/GetBlockSize() + 0.5);

        Debug.Log(side + " " + xChunk + " " + zChunk + " " + xBlock + " " + hBlock + " " + zBlock);
        return planet.chunks[side, xChunk, zChunk].blocks[xBlock, hBlock, zBlock];
    }
    public int SideFromPoint(Vector3 point) {
        Vector3 dir = point.normalized;
        float normInf = Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y), Mathf.Abs(dir.z)); 
        Vector3 cubeDir = dir / normInf;

        float minDist = 10000;
        int bestSide = -1;
        for (int side = 0; side < 6; side++) {
            float dot = Vector3.Dot(sideYaxisList[side], cubeDir);
            float distToPlane = Mathf.Abs(dot - 1);
            if ( distToPlane < minDist ) {
                minDist = distToPlane;
                bestSide = side;
            } 
        }
        return bestSide;
    }


    public Planet planet;
    private GameObject[,,] currentChunksLoaded;

    private PlanetDataGenerator planetDataGenerator;
    public PlanetMeshGenerator planetMeshGenerator;

    public static TerrainManager instance = null; //{ get; private set; }
    private void Awake() 
    { 
        planetRadius =  chunksPerSide*chunkSize/2f;
        currentChunksLoaded = new GameObject[6,chunksPerSide,chunksPerSide];

        if (instance != null && instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            instance = this; 
        } 
    }

    public bool ChunkCloseEnoughToLoad(int side, int chunkX, int chunkZ) {
        return (BaseVectorAtCenter(side, chunkX, chunkZ) - currentPosition.position.normalized).magnitude < radiusOfLoad;
    }

    public void GeneratePlanet()
    {
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkZ=0; chunkZ < chunksPerSide; chunkZ++) {
                    if (ChunkCloseEnoughToLoad(side, chunkX, chunkZ)) {
                        GenerateChunkMesh(side, chunkX, chunkZ);
                    } 
                    else {
                        DestroyChunkMesh(side, chunkX, chunkZ);
                    }
                }
            }
        }
    }

    public void DestroyChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh == null) return;
        Destroy(oldMesh);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = null;
    }

    public void GenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh != null) return;

        Mesh mesh = planetMeshGenerator.GenerateChunkMesh(sideCoord, xCoord, zCoord);
        GameObject world = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        world.layer = TerrainLayer;
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = textureMaterial;
        world.GetComponent<MeshCollider>().sharedMesh = mesh;
        //world.GetComponent<MeshCollider>().attachedRigidbody
        // = world.GetComponent<Rigidbody>();
        //world.GetComponent<Rigidbody>().useGravity = false;

        currentChunksLoaded[sideCoord, xCoord, zCoord] = world;
    }

    private void Start() {
        planetDataGenerator = new PlanetDataGenerator(chunkSize, chunkHeight, chunksPerSide);
        planet = planetDataGenerator.Generate();
        
        planetMeshGenerator = new PlanetMeshGenerator(planet, planetRadius);
    }

    private void Update() {

        GeneratePlanet();
    }
}
