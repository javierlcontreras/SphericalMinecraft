using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{
    public float EPS = 0.05f;
    
    private const int chunkSize = 16;
    private const int chunkHeight = 64;
    public int chunksPerSide = 4;
    public float planetRadius = 10; // base radius
    public Material textureMaterial;

    public Vector3 currentPosition;
    public float radiusOfLoad;

    public Vector3[,,] baseVectors;
    public Vector3[] sideNormalList = new Vector3[]{
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    public Vector3[] sideTangentList = new Vector3[]{
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    
    private Planet planet;
    private GameObject[,,] currentChunksLoaded;

    private PlanetDataGenerator planetDataGenerator;
    public PlanetMeshGenerator planetMeshGenerator;

    public static TerrainManager instance = null; //{ get; private set; }
    private void Awake() 
    { 
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
        float blockLength = planetRadius*2.0f/chunksPerSide/chunkSize;
        int numBlocks = chunkSize*chunksPerSide;
        Vector3[,,] baseVectors = new Vector3[6,numBlocks+1, numBlocks+1];
        for (int side=0; side<6; side++) {
            Vector3 normal = sideNormalList[side];
            Vector3 xAxis = sideTangentList[side];
            Vector3 yAxis = Vector3.Cross(normal, xAxis);

            for (int i=0; i<=numBlocks; i++) {
                for (int j=0; j<=numBlocks; j++) {
                    float x = numBlocks/2f - i;
                    float y = numBlocks/2f - j;
                    Vector3 radius = normal*planetRadius + x*xAxis*blockLength + y*yAxis*blockLength;
                    baseVectors[side, i, j] = Vector3.Normalize(radius);
                    //spawnDebugBall(baseVectors[side, i, j], 0.2f);
                }    
            }
        }
        return baseVectors;
    }


    public bool closeEnough(int side, int centerX, int centerY) {
        //Debug.Log(baseVectors.GetLength(1));
        //Debug.Log(centerX);
        return (baseVectors[side, centerX, centerY] - Vector3.Normalize(currentPosition)).magnitude < radiusOfLoad;
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
        Mesh mesh = planetMeshGenerator.GenerateChunk(sideCoord, xCoord, yCoord);
        
        GameObject world = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = textureMaterial;

        return world;
    }

    private void Start() {
        baseVectors = ComputeBaseVectors();
        
        planetDataGenerator = new PlanetDataGenerator(chunkSize, chunkHeight, chunksPerSide);
        planet = planetDataGenerator.Generate();
        
        planetMeshGenerator = new PlanetMeshGenerator(planet, planetRadius);

        currentChunksLoaded = new GameObject[6,chunkSize,chunkSize];
        
    }

    private void Update() {
        if (baseVectors != null) GeneratePlanet();
    }
}
