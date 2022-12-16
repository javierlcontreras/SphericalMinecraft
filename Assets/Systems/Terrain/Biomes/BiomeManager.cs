using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public bool singleBiome;
    public Biome[] biomes;
    public NoiseGeneratorSettings humidityMap;
    public NoiseGeneratorSettings temperatureMap;
    public NoiseGeneratorSettings treeMap;

    public Biome GetBiome(Vector3 samplingDirection) {
        if (singleBiome || humidityMap == null || temperatureMap == null) return biomes[0];

        float humidity = humidityMap.GetNoiseAt(samplingDirection);
        float temperature = temperatureMap.GetNoiseAt(samplingDirection);
        if (humidity < 0.5 && temperature > 0.5) {
            return biomes[1]; // desert
        }
        if (humidity > 0.5 && temperature < 0.5) {
            return biomes[2]; // forest
        }
        return biomes[0];
    }

    public bool GetTree(Vector3 samplingDirection) {
        if (treeMap == null) return false;
        float tree = treeMap.GetNoiseAt(samplingDirection);
        return (tree == 1f);
    }
}
