using System.Text;
using UnityEngine;

public class Chunk {
    private int chunkSize, chunkHeight;
    private int sideCoord;
    private int xCoord;
    private int zCoord;
    public int GetSideCoord() { return sideCoord; }
    public int GetXCoord() { return xCoord; }
    public int GetZCoord() { return zCoord; }
    
    public Block[,,] blocks;
    private PlanetTerrain planet;
    public PlanetTerrain GetPlanet() { return planet; }
    
    public Chunk(int _sideCoord, int _xCoord, int _zCoord, PlanetTerrain _planet) {
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        zCoord = _zCoord;

        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetHeight();
        blocks = new Block[chunkSize, chunkHeight, chunkSize];
        Init();
    }

    public float DistanceToChunk(Chunk nextChunk) {
        int sideNext = nextChunk.sideCoord; 
        int nextChunkX = nextChunk.xCoord; 
        int nextChunkZ = nextChunk.zCoord;

        Vector3 base1 = planet.BaseVectorAtCenter(sideCoord, xCoord, zCoord);
        Vector3 base2 = planet.BaseVectorAtCenter(sideNext, nextChunkX, nextChunkZ);

        return (base1 - base2).magnitude;
    }

    public void Init() {
        for (int y = 0; y<chunkHeight; y++) {
            for (int x = 0; x < chunkSize; x++) {
                for (int z = 0; z < chunkSize; z++) { 
                    BlockType type = BlockTypeEnum.GetBlockTypeByName("invalid");
                    Vector3 inChunkPosition = new Vector3(x,y,z);
                    blocks[x, y, z] = new Block(x, y, z, type, this);
                }
            }
        }
    }

    public void DebugChunkDataAtHeight(int y) {
        var builder = new StringBuilder();
        int numSides = planet.NumBlocksAtHeightPerChunk(y);
        Debug.Log("Real chunkSize " + numSides); 
        for (int x = 0; x < numSides; x++) {
            for (int z = 0; z < numSides; z++) { 
                BlockType type = blocks[x, y, z].GetBlockType();
                builder.Append(type.GetName()[0]);
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
    }

    public void CreateChunkData() {
        int maxNumSides = planet.NumBlocksAtHeightPerChunk(planet.GetHeight()-1); 
        for (int y = 0; y<chunkHeight; y++) {
            int numSides = planet.NumBlocksAtHeightPerChunk(y);
            for (int x = 0; x < numSides; x++) {
                for (int z = 0; z < numSides; z++) { 
                    float height = TerrainHeightFromNoise(x,y,z);
                    
                    BlockType type = FillDirtUpToHeight(y, height);
                    blocks[x, y, z].SetBlockType(type);
                }
            }
        }
    }
 
    public BlockType FillDirtUpToHeight(int y, float height) {
        if (y < planet.GetMinHeight()) {
            return BlockTypeEnum.GetBlockTypeByName("bedrock");
        }
        else if (y < 2*height/3) {
            return BlockTypeEnum.GetBlockTypeByName("stone");
        }
        else if (y < height) {
            return BlockTypeEnum.GetBlockTypeByName("dirt");
        }
        else if (y < 1+height) {
            return BlockTypeEnum.GetBlockTypeByName("grass");
        }
        else {
            return BlockTypeEnum.GetBlockTypeByName("air");
        }
    }

    public float TerrainHeightFromNoise(int x, int y, int z) {
        Vector3 samplingDirection = planet.BaseVector(sideCoord, xCoord, zCoord, x, y, z);
        
        float terrainHeight = PerlinNoise.get3DPerlinNoise(samplingDirection, 1);
        terrainHeight += 0.7f*PerlinNoise.get3DPerlinNoise(samplingDirection, 2);
        terrainHeight += 0.45f*PerlinNoise.get3DPerlinNoise(samplingDirection, 4);
        terrainHeight /= 1.75f;
        terrainHeight *= (planet.GetHeight() - 2)/2f;
        terrainHeight += 1f; 

        return terrainHeight;
    }

    public Quaternion ChunkToGlobal() {
        Vector3 sideNormal = TerrainGenerationConstants.sideYaxisList[sideCoord];
        Vector3 sideXaxis = TerrainGenerationConstants.sideXaxisList[sideCoord];
        Vector3 sideZaxis = TerrainGenerationConstants.sideZaxisList[sideCoord];

        return Quaternion.LookRotation(sideZaxis, sideNormal); 
    }

    public Quaternion GlobalToChunk() {
        return Quaternion.Inverse(ChunkToGlobal());
    }
}