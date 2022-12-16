using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public Biome[] biomes;
    public NoiseGeneratorSettings humidityMap;
    public NoiseGeneratorSettings temperatureMap;
    public NoiseGeneratorSettings treeMap;

    public Biome GetBiome(Vector3 samplingDirection) {
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
        float tree = treeMap.GetNoiseAt(samplingDirection);
        return (tree == 1f);
    }
}
