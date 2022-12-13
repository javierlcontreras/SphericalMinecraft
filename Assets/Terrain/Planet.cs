using UnityEngine;

public class Planet {
    public Chunk[,,] chunks;
    
    private PlanetGeneratorSettings settings;
    private GameObject planetGameObject;
    
    private Vector3 planetPosition;
    private int chunkHeight;
    private int minChunkHeight;
    private int[] blocksAtHeight;

    private PlanetDataGenerator planetDataGenerator;
    private PlanetMeshGenerator planetMeshGenerator;
    public PlanetMeshGenerator GetPlanetMeshGenerator() {
        return planetMeshGenerator;
    }
    
    private GameObject[,,] currentChunksLoaded;

    public Planet(PlanetGeneratorSettings _settings) {
        settings = _settings;

        planetPosition = settings.GetInitialPosition();
        planetGameObject = new GameObject(settings.GetName());
        planetGameObject.transform.position = planetPosition;
        
        int chunksPerSide = settings.GetChunksPerSide();
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        currentChunksLoaded = new GameObject[6,chunksPerSide,chunksPerSide];

        chunkHeight = PrecomputeHeight(4*GetChunkSize()*GetChunksPerSide())+1;
        minChunkHeight = PrecomputeHeight(4*GetChunksPerSide());
        blocksAtHeight = new int[chunkHeight+10];
        for (int h=0; h<chunkHeight+10; h++) {
            blocksAtHeight[h] = PrecomputeNumBlocksAtHeight(h);
            //Debug.Log("Blocks at height " + h + ": " + blocksAtHeight[h]);
        }
        planetDataGenerator = new PlanetDataGenerator(this);
        planetMeshGenerator = new PlanetMeshGenerator(this);
    }

    public int GetHeight() {
        return chunkHeight;
    }
    public int GetMinHeight() {
        return minChunkHeight;
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
        return planetPosition;
    }

    public Vector3 BaseVector(int side, int chunkX, int chunkZ, int cornerX, int cornerY, int cornerZ) {
        Vector3 normal = TerrainManager.sideYaxisList[side];
        Vector3 xAxis = TerrainManager.sideXaxisList[side];
        Vector3 zAxis = TerrainManager.sideZaxisList[side];

        int numBlocks = GetChunksPerSide()*GetChunkSize();
        
        int mult = GetChunkSize() / Mathf.Max(1, NumBlocksAtHeightPerChunk(cornerY));
        /*Debug.Log(cornerX + " " + cornerY + " " + cornerZ);
        Debug.Log(GetChunkSize() + " " + NumBlocksAtHeightPerChunk(cornerY) + " " + mult); 
        Debug.Log("------------");*/

        float x = chunkX*GetChunkSize() + cornerX*mult - numBlocks/2f;
        float z = chunkZ*GetChunkSize() + cornerZ*mult - numBlocks/2f;
        
        float blockSize = 2f/numBlocks;
        Vector3 radius = normal*1 + x*xAxis*blockSize + z*zAxis*blockSize;
        radius = radius.normalized;

/*        if(radius.x == 0) Debug.DrawRay(Vector3.zero, radius, Color.red, 400);
        if(radius.y == 0) Debug.DrawRay(Vector3.zero, radius, Color.blue, 400);
        if(radius.z == 0) Debug.DrawRay(Vector3.zero, radius, Color.green, 400);
        //Debug.DrawRay(Vector3.zero, radius, Color.white, 400);
*/
        return radius;
    }

    public Vector3 BaseVectorAtCenter(int side, int chunkX, int chunkZ) {
        int center = GetChunkSize()/2;
        return BaseVector(side, chunkX, chunkZ, center, GetHeight()-1, center);
    }
    
    public float HeightAtBottomOfLayer(int h) {
        return h + TerrainManager.instance.GetCoreRadius();
    }    

    public int PrecomputeHeight(int polygonSides) {        
        int maxHeight = settings.GetMaxHeight();
        int resultHeight = -1;
        for (int h = 0; ; h++) {
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
            Debug.Log("BUG: the needed height: " + resultHeight + " is larger that the maxHeight:" + maxHeight);
            Debug.Break();
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

    public void UpdatePlanetMesh()
    {
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < GetChunksPerSide(); chunkX++) {
                for (int chunkZ=0; chunkZ < GetChunksPerSide(); chunkZ++) {
                    Vector3 chunkPosition = BaseVectorAtCenter(side, chunkX, chunkZ);
                    if (TerrainManager.instance.ChunkCloseEnoughToLoad(chunkPosition, planetPosition)) {
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
        oldMesh.SetActive(false);
        //TerrainManager.instance.DestroyChunk(oldMesh);
        //currentChunksLoaded[sideCoord, xCoord, zCoord] = null;
    }

    public void GenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh != null) {
            oldMesh.SetActive(true);
            return;
        }
        Debug.Log("Creating new chunk mesh!");

        Chunk chunk = chunks[sideCoord, xCoord, zCoord];
        if (chunk == null) {
            planetDataGenerator.GenerateChunk(sideCoord, xCoord, zCoord);
            Debug.Log("Creating chunk data!");
        }

        Mesh mesh = planetMeshGenerator.GenerateChunkMesh(sideCoord, xCoord, zCoord);
        string chunkName = "(" + sideCoord + "," + xCoord + "," + zCoord + ")";
        GameObject chunkMesh = new GameObject(GetName() + ": "+ chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        chunkMesh.layer = TerrainLayer;
        chunkMesh.GetComponent<MeshFilter>().mesh = mesh;
        chunkMesh.GetComponent<MeshRenderer>().material = TerrainManager.instance.textureMaterial;
        chunkMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
        chunkMesh.transform.position = planetPosition;
        chunkMesh.transform.SetParent(planetGameObject.transform);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = chunkMesh;
    }

    /*public void GeneratePlanetData() {
        planetDataGenerator.Generate();
    }*/
}