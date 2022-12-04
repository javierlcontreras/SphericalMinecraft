
using System.Text;
using UnityEngine;

public class Planet {
    public Chunk[,,] chunks;
    
    private int chunksPerSide;
    private int chunkSize;
    private int chunkHeight;
    
    private PlanetDataGenerator planetDataGenerator;
    private PlanetMeshGenerator planetMeshGenerator;
    public PlanetMeshGenerator GetPlanetMeshGenerator() {
        return planetMeshGenerator;
    }
    
    private GameObject[,,] currentChunksLoaded;

    public Planet(int _chunksPerSide, int _chunkSize, int _chunkHeight) {
        chunksPerSide = _chunksPerSide;
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        
        currentChunksLoaded = new GameObject[6,chunksPerSide,chunksPerSide];

        planetDataGenerator = new PlanetDataGenerator(chunkSize, chunkHeight, chunksPerSide);
        planetMeshGenerator = new PlanetMeshGenerator(this);

    }
    
    public float GetPlanetRadius() {
        return chunksPerSide*chunkSize/2f;   
    }

    public float GetBlockSize() {
        return 1f;
    }

    public int GetChunksPerSide() {
        return chunksPerSide;
    }
    
    public int GetChunkSize() {
        return chunkSize;
    }
    
    public int GetChunkHeight() {
        return chunkHeight;
    }

    public Vector3 BaseVector(int side, int chunkX, int chunkZ, float blockX, float blockZ) {
        Vector3 normal = TerrainManager.instance.sideYaxisList[side];
        Vector3 xAxis = TerrainManager.instance.sideXaxisList[side];
        Vector3 zAxis = TerrainManager.instance.sideZaxisList[side];
        int numBlocks = chunksPerSide*chunkSize;
        float x = chunkX*chunkSize + blockX - numBlocks/2f;
        float z = chunkZ*chunkSize + blockZ - numBlocks/2f;
        Vector3 radius = normal*GetPlanetRadius() + x*xAxis*GetBlockSize() + z*zAxis*GetBlockSize();
        return radius.normalized;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        float center = chunkSize/2f;
        return BaseVector(side, chunkX, chunkZ, center, center);
    }

    public void SetChunk(int sideCoord, int xCoord, int yCoord, Chunk chunk) {
        chunks[sideCoord, xCoord, yCoord] = chunk;
    }

    public void DebugChunk(int side, int chunkX, int chunkY, int height) {        
        var builder = new StringBuilder();
        for (int i = 0; i < chunkSize; i++) {
            for (int j = 0; j < chunkSize; j++) {
                BlockType type = chunks[side, chunkX, chunkY].blocks[i, j, height].type;
                builder.Append(type.GetName()[0]);
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
    }

    public void UpdatePlanet()
    {
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkZ=0; chunkZ < chunksPerSide; chunkZ++) {
                    Vector3 chunkPosition = BaseVectorAtCenter(side, chunkX, chunkZ);
                    if (TerrainManager.instance.ChunkCloseEnoughToLoad(chunkPosition)) {
                        GenerateChunkMesh(side, chunkX, chunkZ);
                    } 
                    else {
                        DestroyChunkMesh(side, chunkX, chunkZ);
                    }
                }
            }
        }
    }
    public void DestroyChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh == null) return;
        TerrainManager.instance.DestroyChunk(oldMesh);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = null;
    }

    public void GenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh != null) return;

        Mesh mesh = planetMeshGenerator.GenerateChunkMesh(sideCoord, xCoord, zCoord);
        string chunkName = "(" + sideCoord + "," + xCoord + "," + zCoord + ")";
        GameObject world = new GameObject("Chunk " + chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        world.layer = TerrainLayer;
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = TerrainManager.instance.textureMaterial;
        world.GetComponent<MeshCollider>().sharedMesh = mesh;
        
        currentChunksLoaded[sideCoord, xCoord, zCoord] = world;
    }

    public void GeneratePlanet() {
        planetDataGenerator.Generate(this);
    }
}