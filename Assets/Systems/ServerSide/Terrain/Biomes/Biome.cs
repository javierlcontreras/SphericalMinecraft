using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    public string biomeName;
    public bool trees;
    public string[] terrainLayersType;    
    public float[] terrainLayersHeight;    

    public string[] GetTerrainLayersType() {
        return terrainLayersType;
    }
    public float[] GetTerrainLayersHeight() {
        return terrainLayersHeight;
    }
    public string GetBiomeName() {
        return biomeName;
    }
}
