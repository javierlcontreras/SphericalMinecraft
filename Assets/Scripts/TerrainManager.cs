using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetGenerator;

[ExecuteInEditMode]
public class TerrainManager : MonoBehaviour
{
    public int meshSize;
    public float planetRadius;
    public Material surfaceMaterial;

    public static TerrainManager instance = null; //{ get; private set; }

    public void GeneratePlanet()
    {
        PlanetGenerator planetGenerator = new PlanetGenerator(meshSize, planetRadius);
        Mesh mesh = planetGenerator.Generate();

        GameObject world = new GameObject("World", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = surfaceMaterial;
    }

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
}
