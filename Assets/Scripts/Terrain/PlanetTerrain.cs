using UnityEngine;

[RequireComponent(typeof(CelestialBody))]
public class PlanetTerrain : MonoBehaviour
{
    private float cubeToSphereInterpolator = 0; // 1: cube, 0: sphere 
    public int chunksPerSide = 1;
    public int chunkSize = 16;
    public int maxHeight = 32;
    public int minHeight = 4;

    public Material surfaceTexturesMaterial;
    public Material GetSurfaceTexturesMaterial() {
        return surfaceTexturesMaterial;
    }
    private Chunk[,,] chunks;
    
    private int chunkHeight;
    private int chunkMinHeight;
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
        InitTerrainSizes();
    }

    public void InitTerrainSizes() {
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        
        chunkHeight = PrecomputeHeight(4*GetChunkSize()*GetChunksPerSide())+1;
        chunkMinHeight = Mathf.Max(minHeight, PrecomputeHeight(2*GetChunksPerSide())+1);
        TerrainSizePrecomputations();
        InitSubsystems();
    }

    public void InitSubsystems() {
        planetChunkLoader = new PlanetChunkLoader(this);
        planetDataGenerator = new PlanetDataGenerator(this);
        planetMeshGenerator = new PlanetMeshGenerator(this);
    }

    public void TerrainSizePrecomputations() {
        if (!isPowerOf2(chunkSize) || !isPowerOf2(chunksPerSide)) {
            Debug.LogWarning("chunkSize and chunksPerSide MUST be powers of 2");
        }
        
        blocksAtHeight = new int[chunkHeight+10];
        for (int h=0; h<chunkHeight+10; h++) {
            blocksAtHeight[h] = PrecomputeNumBlocksAtHeight(h);
        }
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
        Vector3 radiusNormalized = radius.normalized; //* Mathf.Pow(3, 1f/2f);
        
        return cubeToSphereInterpolator*radius + (1 - cubeToSphereInterpolator)*radiusNormalized;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        int center = GetChunkSize()/2;
        return BaseVector(side, chunkX, chunkZ, center, GetChunkHeight()-1, center);
    }
    
    public float HeightAtBottomOfLayer(int h) {
        return TerrainGenerationConstants.GetBlockHeight()*h + TerrainGenerationConstants.GetCoreRadius();
    }    

    public int PrecomputeHeight(int polygonSides) {  
        if (polygonSides <= 2) return 0;      
        int maxHeight = GetMaxHeight();
        int resultHeight = -1;
        for (int h = 0; h < maxHeight; h++) {
            float radius = HeightAtBottomOfLayer(h);
            float sideLength  = 2.0f * radius * Mathf.Sin(Mathf.PI / polygonSides);
            float blockSize = TerrainGenerationConstants.GetBlockSize();
//            Debug.Log(blockSize + " " + sideLength);
            if (sideLength < 2*blockSize) {
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
            if (sideLength < 2*TerrainGenerationConstants.GetBlockSize()) {
                numSides = power;
                break;
            }
            power *= 2;
        }
        return numSides;
    }

    public void SetChunk(Vector3Int chunkCoord, Chunk chunk) {
        int sideCoord = chunkCoord.x; 
        int xCoord = chunkCoord.y; 
        int zCoord = chunkCoord.z;
        chunks[sideCoord, xCoord, zCoord] = chunk;
    }

    private bool isPowerOf2(int n) {
        if (n == 1) return true;
        if (n%2 != 0) return false;
        return isPowerOf2(n/2);
    }
    public int GetChunkSize() {
        return chunkSize;
    }
    public void SetChunkSize(int _chunkSize) {
        chunkSize = _chunkSize;
        InitSubsystems();
    }
    public int GetChunksPerSide() {
        return chunksPerSide;
    }
    public void SetChunksPerSide(int _chunksPerSide) {
        chunksPerSide = _chunksPerSide;
        InitSubsystems();
    }
    public int GetChunkHeight() {
        return chunkHeight;
    }
    public void SetChunkHeight(int _chunkHeight) {
        chunkHeight = _chunkHeight;
        InitSubsystems();
    }
    public int GetChunkMinHeight() {
        return chunkMinHeight;
    }
    public void SetChunkMinHeight(int _chunkMinHeight) {
        chunkMinHeight = _chunkMinHeight;
        InitSubsystems();
    }
    public string GetPlanetName() {
        return transform.name;
    }
    public int GetMaxHeight() {
        return maxHeight;
    }

    public Chunk GetChunk(Vector3Int chunkCoord)
    {
        return chunks[chunkCoord.x, chunkCoord.y, chunkCoord.z];
    }

    public Chunk GetChunk(int chunkSide, int chunkX, int chunkZ)
    {
        return chunks[chunkSide, chunkX, chunkZ];
    }

    public Chunk[,,] GetAllChunks()
    {
        return chunks;
    }

    public void SetAllChunks(Chunk[,,] _chunks)
    {
        chunks = _chunks;
    }
    public void DestroyChunkMesh(GameObject mesh) {
        Destroy(mesh);
    }
    
    /*
    // Only to show off
    private float interpolationValue = 0;
    private float interpolationVelocity = 1f;
    public void Update()
    {
        if (Input.GetKey("f"))
        {
            Debug.Log("Trying to move " + interpolationValue);
            interpolationValue += Time.deltaTime * interpolationVelocity;
            cubeToSphereInterpolator = 0.5f + 0.5f*Mathf.Cos(interpolationValue);
            planetDataGenerator.RegenerateAllChunks();
            planetChunkLoader.RegenerateAllChunks();
        }

    }*/
}