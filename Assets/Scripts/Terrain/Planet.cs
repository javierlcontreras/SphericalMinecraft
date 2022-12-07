
using System.Text;
using UnityEngine;

public class Planet {
    public Chunk[,,] chunks;
    
    private PlanetGeneratorSettings settings;
    private GameObject planetGameObject;
    
    private Vector3 position;
    private int height;

    private PlanetDataGenerator planetDataGenerator;
    private PlanetMeshGenerator planetMeshGenerator;
    public PlanetMeshGenerator GetPlanetMeshGenerator() {
        return planetMeshGenerator;
    }
    
    private GameObject[,,] currentChunksLoaded;

    public Planet(PlanetGeneratorSettings _settings) {
        settings = _settings;

        position = settings.GetInitialPosition();
        planetGameObject = new GameObject(settings.GetName());
        planetGameObject.transform.position = position;
        
        int chunksPerSide = settings.GetChunksPerSide();
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        currentChunksLoaded = new GameObject[6,chunksPerSide,chunksPerSide];

        planetDataGenerator = new PlanetDataGenerator(this);
        planetMeshGenerator = new PlanetMeshGenerator(this);
        
        height = PrecomputeHeight();
    }

    public int GetHeight() {
        return height;
    }
    public string GetName() {
        return settings.GetName();
    }
    public int GetChunksPerSide() {
        return settings.GetChunksPerSide();
    }        
    public int GetChunkSize() {
        return settings.GetChunkSize();
    }
    public Vector3 GetPosition() {
        return position;
    }

    public Vector3 BaseVector(int side, int chunkX, int chunkZ, float blockX, int blockY, float blockZ) {
        Vector3 normal = TerrainManager.instance.sideYaxisList[side];
        Vector3 xAxis = TerrainManager.instance.sideXaxisList[side];
        Vector3 zAxis = TerrainManager.instance.sideZaxisList[side];
        int numBlocks = GetChunksPerSide()*GetChunkSize();
        int mult = NumBlocksAtHeight(blockY) / GetChunksPerSide() / 4;
        float x = chunkX*GetChunkSize() + blockX*mult - numBlocks/2f;
        float z = chunkZ*GetChunkSize() + blockZ*mult - numBlocks/2f;
        float coreRadius = TerrainManager.instance.GetCoreRadius();

        float blockSize = 1f/(GetChunkSize()*GetChunksPerSide());
        Vector3 radius = normal*1 + x*xAxis*blockSize + z*zAxis*blockSize;

        return radius.normalized;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        float center = GetChunkSize()/2f;
        return BaseVector(side, chunkX, chunkZ, center, GetHeight(), center);
    }
    
    public float HeightAt(int h) {
        return h + TerrainManager.instance.GetCoreRadius();
    }
    
    public int PrecomputeHeight() {
        // maximo h tal que 4*chunkSize*chunkPerSide - poligon en la esfera de radio h tiene lados 2 > i > 1
        int polygonSides = 4*GetChunkSize()*GetChunksPerSide();
        
        int resultHeight = -1;
        for (int height = 0; ; height++) {
            float radius = HeightAt(height);
            float sideLength  = 2.0f * radius * Mathf.Sin(Mathf.PI / polygonSides);
            if (1 <= sideLength && sideLength < 2) {
                resultHeight = height;
            }
            else if (resultHeight != -1) {
                break;
            }
        }
        return resultHeight;
    }

    public int NumBlocksAtHeight(int y) {        
        float radius = HeightAt(y);
        
        // 2^n where 4*2^n-poligon that when inscribed in circle of radius r, has side lengths 1 <= s < 2}
        int numSides = -1;
        int power = 1;
        for (int exponent=0; ; exponent++) {
            float sideLength = 2f * radius * Mathf.Sin(Mathf.PI / power);
            if (1 <= sideLength && sideLength < 2) {
                numSides = power;
                break;
            }
            power *= 2;
        }
        if (numSides < 4) numSides = 4;
        /*Debug.DrawRay(Vector3.zero, Vector3.up, Color.red, 300);
        for (int i=0; i<numSides; i++) {
            float angle = i*2*Mathf.PI/numSides;
            float x = radius*Mathf.Sin(angle);
            float z = radius*Mathf.Cos(angle);
            Debug.DrawRay(x*Vector3.right + z*Vector3.forward, Vector3.up, Color.white, 100);
        }*/
        return numSides;
    }

    public void SetChunk(int sideCoord, int xCoord, int yCoord, Chunk chunk) {
        chunks[sideCoord, xCoord, yCoord] = chunk;
    }
/*
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
*/

    public void UpdatePlanetMesh()
    {
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < GetChunksPerSide(); chunkX++) {
                for (int chunkZ=0; chunkZ < GetChunksPerSide(); chunkZ++) {
                    Vector3 chunkPosition = BaseVectorAtCenter(side, chunkX, chunkZ);
                    if (TerrainManager.instance.ChunkCloseEnoughToLoad(chunkPosition, position)) {
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
        GameObject chunk = new GameObject(GetName() + ": "+ chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        chunk.layer = TerrainLayer;
        chunk.GetComponent<MeshFilter>().mesh = mesh;
        chunk.GetComponent<MeshRenderer>().material = TerrainManager.instance.textureMaterial;
        chunk.GetComponent<MeshCollider>().sharedMesh = mesh;
        chunk.transform.position = position;
        chunk.transform.SetParent(planetGameObject.transform);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = chunk;
    }

    public void GeneratePlanetData() {
        planetDataGenerator.Generate();
    }
}