using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class PlanetTerrain : MonoBehaviour {
    public int chunksPerSide = 1;
    public int chunkSize = 16;
    public int maxHeight = 32;

    public Material surfaceTexturesMaterial;
    public Material GetSurfaceTexturesMaterial() {
        return surfaceTexturesMaterial;
    }
    public Chunk[,,] chunks;
    
    private int chunkHeight;
    private int minChunkHeight;
    private int[] blocksAtHeight;

    private PlanetChunkLoader planetChunkLoader;
    public PlanetChunkLoader GetPlanetChunkLoader() {
        return planetChunkLoader;
    }
    private PlanetDataGenerator planetDataGenerator;
    public PlanetDataGenerator GetPlanetDataGenerator() {
        return planetDataGenerator;
    }
    private PlanetMeshGenerator planetMeshGenerator;
    public PlanetMeshGenerator GetPlanetMeshGenerator() {
        return planetMeshGenerator;
    }
    public NoiseGeneratorSettings heightMap;
    public float GetHeightMapAt(Vector3 samplingDirection) {
        return heightMap.GetNoiseAt(samplingDirection);
    }
    public BiomeManager biomeManager;
    public Biome GetBiomeAt(Vector3 samplingDirection) {
        return biomeManager.GetBiome(samplingDirection);
    }
    public bool GetTreeAt(Vector3 samplingDirection) {
        return biomeManager.GetTree(samplingDirection);
    }
    public CaveManager caveManager;
    public bool GetCaveAt(Vector3 samplingDirection, float height) {
        if (caveManager == null) return false;
        return caveManager.GetCave(samplingDirection*height);
    }

    public void Awake() {
        if (!isPowerOf2(chunkSize) || !isPowerOf2(chunksPerSide)) {
            Debug.LogWarning("chunkSize and chunksPerSide MUST be powers of 2");
        }

        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        
        chunkHeight = PrecomputeHeight(4*GetChunkSize()*GetChunksPerSide())+1;
        minChunkHeight = PrecomputeHeight(2*GetChunksPerSide())+1;
        blocksAtHeight = new int[chunkHeight+10];
        for (int h=0; h<chunkHeight+10; h++) {
            blocksAtHeight[h] = PrecomputeNumBlocksAtHeight(h);
            //Debug.Log("Blocks at height " + h + ": " + blocksAtHeight[h]);
        }
        planetChunkLoader = new PlanetChunkLoader(this);
        planetDataGenerator = new PlanetDataGenerator(this);
        planetMeshGenerator = new PlanetMeshGenerator(this);
    }

    public int GetHeight() {
        return chunkHeight;
    }
    public int GetMinHeight() {
        return minChunkHeight;
    }
    public Vector3 GetPlanetPosition() {
        return transform.position;
    }
    public Quaternion GetPlanetRotation() {
        return transform.rotation;
    }
    public Vector3 BaseVector(int side, int chunkX, int chunkZ, int cornerX, int cornerY, int cornerZ) {
        Vector3 normal = TerrainGenerationConstants.sideYaxisList[side];
        Vector3 xAxis = TerrainGenerationConstants.sideXaxisList[side];
        Vector3 zAxis = TerrainGenerationConstants.sideZaxisList[side];

        int numBlocks = GetChunksPerSide()*GetChunkSize();
        
        int mult = GetChunkSize() / Mathf.Max(1, NumBlocksAtHeightPerChunk(cornerY));

        float x = chunkX*GetChunkSize() + cornerX*mult - numBlocks/2f;
        float z = chunkZ*GetChunkSize() + cornerZ*mult - numBlocks/2f;
        
        float blockSize = 2f/numBlocks;
        Vector3 radius = normal*1 + x*xAxis*blockSize + z*zAxis*blockSize;
        radius = radius.normalized;

        return radius;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        int center = GetChunkSize()/2;
        return BaseVector(side, chunkX, chunkZ, center, GetHeight()-1, center);
    }
    
    public float HeightAtBottomOfLayer(int h) {
        return h + TerrainGenerationConstants.GetCoreRadius();
    }    

    public int PrecomputeHeight(int polygonSides) {  
        if (polygonSides <= 2) return 0;      
        int maxHeight = GetMaxHeight();
        int resultHeight = -1;
        for (int h = 0; h < maxHeight; h++) {
            float radius = HeightAtBottomOfLayer(h);
            float sideLength  = 2.0f * radius * Mathf.Sin(Mathf.PI / polygonSides);
            if (1 <= sideLength && sideLength < 2) {
                resultHeight = h;
            }
            else if (resultHeight != -1) {
                break;
            }
        }
        if (resultHeight == -1) {
            throw new System.Exception("Planet Generation, the needed height " + resultHeight + " is larger that the maxHeight restriction " + maxHeight);
        }
        return resultHeight;
    }
    public int NumBlocksAtHeight(int y) {
        if (y < 0) return blocksAtHeight[0];
        return blocksAtHeight[y];
    }
    public int NumBlocksAtHeightPerChunk(int y) {
        return NumBlocksAtHeight(y) / GetChunksPerSide() / 4;
    }

    public int PrecomputeNumBlocksAtHeight(int y) { 
        float radius = HeightAtBottomOfLayer(y);

        // 2^n where 4*2^n-poligon that when inscribed in circle of radius r, has side lengths 1 <= s < 2}
        int numSides = -1;
        int power = 4;
        for (int exponent=2; ; exponent++) {
            float sideLength = 2f * radius * Mathf.Sin(Mathf.PI / power);
            if (sideLength < 2) {
                numSides = power;
                break;
            }
            power *= 2;
        }
        return numSides;
    }

    public void SetChunk(int sideCoord, int xCoord, int yCoord, Chunk chunk) {
        chunks[sideCoord, xCoord, yCoord] = chunk;
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
    public string GetPlanetName() {
        return transform.name;
    }
    public int GetMaxHeight() {
        return maxHeight;
    }

    public void DestroyChunkMesh(GameObject mesh) {
        Destroy(mesh);
    }
}