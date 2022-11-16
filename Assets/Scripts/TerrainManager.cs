using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{
    public float EPS = 0.05f;
    
    public int chunkSize = 16;
    public int chunkHeight = 32;
    public int chunksPerSide = 4;
    public float planetRadius = 10; // base radius
    public Material textureMaterial;

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

    public void GeneratePlanet()
    {
        PlanetDataGenerator planetDataGenerator = new PlanetDataGenerator(chunkSize, chunkHeight, chunksPerSide);
        Planet planet = planetDataGenerator.Generate();

        PlanetMeshGenerator planetMeshGenerator = new PlanetMeshGenerator(planet, planetRadius);
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkY=0; chunkY < chunksPerSide; chunkY++) {

                    GenerateChunk(planet, planetMeshGenerator, side, chunkX, chunkY);
                
                }
            }
        }
    }

    public void GenerateChunk(Planet planet, PlanetMeshGenerator planetMeshGenerator, int sideCoord, int xCoord, int yCoord) {
        Mesh mesh = planetMeshGenerator.GenerateChunk(sideCoord, xCoord, yCoord);
        
        GameObject world = new GameObject("Chunk", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = textureMaterial;
    }

    private void Start() {
        GeneratePlanet();
    }
}
