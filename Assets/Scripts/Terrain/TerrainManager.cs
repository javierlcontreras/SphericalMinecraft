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

    public Vector3[,,] baseVectors;
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
    
    private Planet planet;
    private GameObject[,,] currentChunksLoaded;

    private PlanetDataGenerator planetDataGenerator;
    public PlanetMeshGenerator planetMeshGenerator;

    public static TerrainManager instance = null; //{ get; private set; }
    private void Awake() 
    { 
        planetRadius =  chunksPerSide*chunkSize/2f;
        baseVectors = ComputeBaseVectors();
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

    public Vector3[,,] ComputeBaseVectors() {
        int numBlocks = chunkSize*chunksPerSide;
        Vector3[,,] baseVectors = new Vector3[6,numBlocks+1, numBlocks+1];
        for (int side=0; side<6; side++) {
            Vector3 normal = sideYaxisList[side];
            Vector3 xAxis = sideXaxisList[side];
            Vector3 zAxis = sideZaxisList[side];

            for (int i=0; i<=numBlocks; i++) {
                for (int j=0; j<=numBlocks; j++) {
                    float x = i - numBlocks/2f;
                    float z = j - numBlocks/2f;
                    Vector3 radius = normal*planetRadius + x*xAxis*blockLength + z*zAxis*blockLength;
                    baseVectors[side, i, j] = Vector3.Normalize(radius);
                    /*if (side == 0 && i == 0 && j == 3) {
                        Debug.Log(normal + " " + xAxis + " " + zAxis);
                        Debug.Log(i + " " + j + " " + x + " " + z + " " + numBlocks);
                        Debug.Log(baseVectors[side, i, j]);
                    }*/
                    //spawnDebugBall(baseVectors[side, i, j], 0.2f);
                }    
            }
        }
        return baseVectors;
    }


    public bool closeEnough(int side, int centerX, int centerY) {
        //Debug.Log(baseVectors.GetLength(1));
        //Debug.Log(centerX);
        float height = currentPosition.position.magnitude;
        return (baseVectors[side, centerX, centerY]*height - currentPosition.position).magnitude < radiusOfLoad;
    }

    public void GeneratePlanet()
    {
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkY=0; chunkY < chunksPerSide; chunkY++) {
                    int centerX = chunkX * chunkSize + chunkSize/2;
                    int centerY = chunkY * chunkSize + chunkSize/2;
                    
                    if (closeEnough(side, centerX, centerY)) {
                        GameObject mesh = currentChunksLoaded[side, chunkX, chunkY];
                        if (mesh == null) {
                            currentChunksLoaded[side, chunkX, chunkY] = GenerateChunk(planet, planetMeshGenerator, side, chunkX, chunkY);
                        }
                    } 
                    else {
                        GameObject mesh = currentChunksLoaded[side, chunkX, chunkY];
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
