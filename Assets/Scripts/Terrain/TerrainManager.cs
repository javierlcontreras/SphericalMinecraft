using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{
    private const int chunksPerSide = 1;
    public int ChunksPerSide => chunksPerSide;
    private const int chunkSize = 8;
    public int ChunkSize => chunkSize;
    private const int chunkHeight = 16;
    public int ChunkHeight => chunkHeight;
    private const float blockLength = 1f;
    public float BlockLength => blockLength;
    private const int textureBlockSize = 16;
    public int TextureBlockSize => textureBlockSize;
    private const int textureAtlasSize = 128;
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
        Vector3 radius = normal*planetRadius + x*xAxis*blockLength + z*zAxis*blockLength;
        return radius.normalized;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        int center = chunkSize/2;
        return BaseVector(side, chunkX, chunkZ, center, center);
    }

    private Planet planet;
    private GameObject[,,] currentChunksLoaded;

    private PlanetDataGenerator planetDataGenerator;
    public PlanetMeshGenerator planetMeshGenerator;

    public static TerrainManager instance = null; //{ get; private set; }
    private void Awake() 
    { 
        planetRadius =  chunksPerSide*chunkSize/2f;
        currentChunksLoaded = new GameObject[6,chunkSize,chunkSize];

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
                        GameObject mesh = currentChunksLoaded[side, chunkX, chunkZ];
                        if (mesh == null) {
                            currentChunksLoaded[side, chunkX, chunkZ] = GenerateChunk(planet, planetMeshGenerator, side, chunkX, chunkZ);
                        }
                    } 
                    else {
                        GameObject mesh = currentChunksLoaded[side, chunkX, chunkZ];
                        if (mesh != null) {
                            Destroy(mesh);
                        }
                    }
                }
            }
        }
    }

    public GameObject GenerateChunk(Planet planet, PlanetMeshGenerator planetMeshGenerator, int sideCoord, int xCoord, int yCoord) {
        Mesh mesh = planetMeshGenerator.GenerateChunkMesh(sideCoord, xCoord, yCoord);
        GameObject world = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        world.layer = TerrainLayer;
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = textureMaterial;
        world.GetComponent<MeshCollider>().sharedMesh = mesh;
        //world.GetComponent<MeshCollider>().attachedRigidbody
        // = world.GetComponent<Rigidbody>();
        //world.GetComponent<Rigidbody>().useGravity = false;

        return world;
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
