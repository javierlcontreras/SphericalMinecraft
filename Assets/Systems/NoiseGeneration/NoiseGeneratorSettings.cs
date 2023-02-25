using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGeneratorSettings : MonoBehaviour
{
    public Vector3 offset;
    public float[] frequencies;
    public float[] amplitudes;

    public bool thresholded = false;
    public bool diffusesWithRadius = false;
    public float diffuseCoef;
    public float threshold = 0.5f;
    public int blocksSampled;
    public bool debugInSphere = true;

    public bool ultraFlat = false;
    public float ultraFlatLevel;

    public Texture2D DebugTexture() {
        float debugZoom = 1;
        if (debugInSphere) debugZoom = 1f/blocksSampled;
        Texture2D texture = new Texture2D(blocksSampled, blocksSampled);
        for (int blockX=0; blockX < blocksSampled; blockX++) {
            for (int blockZ=0; blockZ < blocksSampled; blockZ++) {
                Vector3 samplingPoint = (Vector3.forward*blockZ + Vector3.right*blockX)*debugZoom;
                float noise = GetNoiseAt(samplingPoint);
                Color color = new Color(noise, noise, noise);
                texture.SetPixel(blockX, blockZ, color);
            }
        }
        texture.Apply();
        return texture;
    }

    public float GetNoiseAt(Vector3 samplingPoint) {
        if (ultraFlat) return ultraFlatLevel;
        
        int layers = frequencies.Length;
        float noise = 0;
        float maxNoise = 0;
        for (int layer=0; layer<layers; layer++) {
            noise += amplitudes[layer] * PerlinNoise.get3DPerlinNoise(offset + samplingPoint, frequencies[layer]);
            maxNoise += amplitudes[layer];
        }
        noise = noise / maxNoise;
        if (diffusesWithRadius) noise = Mathf.Max(0, noise - samplingPoint.magnitude*diffuseCoef);
        if(thresholded) {
            if (noise > threshold) return 1;
            return 0;
        }
        return noise;
    }
}
