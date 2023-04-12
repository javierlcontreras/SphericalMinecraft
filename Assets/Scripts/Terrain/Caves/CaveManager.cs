using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveManager : MonoBehaviour
{
    public NoiseGeneratorSettings caveMap;
    public bool GetCave(Vector3 position) {
        float noise = caveMap.GetNoiseAt(position);
        if (noise == 0) return false;
        return true;
    }
}
