using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public Biome[] biomes;
    public NoiseGeneratorSettings humidityMap;
    public NoiseGeneratorSettings temperatureMap;

    public Biome GetBiome(Vector3 samplingDirection) {
        float humidity = humidityMap.GetNoiseAt(samplingDirection);
        float temperature = temperatureMap.GetNoiseAt(samplingDirection);
        if (humidity < 0.6 && temperature > 0.4) {
            return biomes[1];
        }
        return biomes[0];
    }
}
