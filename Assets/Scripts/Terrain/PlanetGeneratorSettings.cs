using UnityEngine;

public class PlanetGeneratorSettings {
    private string name;
    private Vector3 initialPosition;
    private int chunksPerSide;
    private int chunkSize;
    
    public PlanetGeneratorSettings(string _name, Vector3 _initialPosition, int _chunksPerSide, int _chunkSize) {
        name = _name;
        initialPosition = _initialPosition;
        chunkSize = _chunkSize;
        chunksPerSide = _chunksPerSide;

        if (!isPowerOf2(chunkSize) || !isPowerOf2(chunksPerSide)) {
            Debug.Log("chunkSize and chunksPerSide MUST be powers of 2");
        }
    }

    private bool isPowerOf2(int n) {
        if (n == 1) return true;
        if (n%2 != 0) return false;
        return isPowerOf2(n/2);
    }
    public int GetChunkSize() {
        return chunkSize;
    }
    public int GetChunksPerSide() {
        return chunksPerSide;
    }
    public string GetName() {
        return name;
    }
    public Vector3 GetInitialPosition() {
        return initialPosition;
    }
} 