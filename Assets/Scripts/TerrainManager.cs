using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{
    public int chunkSize = 16;
    public int chunkHeight = 32;
    public int chunksPerSide = 32;
    public float planetRadius = 100;
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
        //planet.DebugChunk(0, 0, 0, 17);

        //GameObject world = new GameObject("World", typeof(MeshFilter), typeof(MeshRenderer));
        //world.GetComponent<MeshFilter>().mesh = mesh;
        //world.GetComponent<MeshRenderer>().material = surfaceMaterial;
    }

    private void Start() {
        GeneratePlanet();
    }
}
