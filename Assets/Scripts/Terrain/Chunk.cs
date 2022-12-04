using UnityEngine;

public class Chunk {
    private int chunkSize, chunkHeight;
    public int sideCoord {get;}
    public int xCoord {get;}
    public int zCoord {get;}
    
    public Block[,,] blocks;
    public Planet planet;
    
    public Chunk(int _chunkSize, int _chunkHeight, int _sideCoord, int _xCoord, int _zCoord, Planet _planet) {
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        zCoord = _zCoord;

        blocks = new Block[chunkSize, chunkHeight, chunkSize];
        planet = _planet;
    }

    public float DistanceToChunk(Chunk nextChunk) {
        int sideNext = nextChunk.sideCoord; 
        int nextChunkX = nextChunk.xCoord; 
        int nextChunkZ = nextChunk.zCoord;

        int x1 = xCoord * chunkSize + chunkSize/2;
        int z1 = zCoord * chunkSize + chunkSize/2;
        int x2 = nextChunkX * chunkSize + chunkSize/2;
        int z2 = nextChunkZ * chunkSize + chunkSize/2;
        Vector3 base1 = planet.BaseVectorAtCenter(sideCoord, xCoord, zCoord);
        Vector3 base2 = planet.BaseVectorAtCenter(sideNext, nextChunkX, nextChunkZ);

        return (base1 - base2).magnitude;
    }

    public void CreateChunkData() {
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {
                float height = TerrainHeightFromNoise(x, z);
                for (int y = 0; y<chunkHeight; y++) {
                    BlockType type = FillDirtUpToHeight(y, height);

                    Vector3 inChunkPosition = new Vector3(x,y,z);
                    blocks[x, y, z] = new Block(inChunkPosition, type, this);
                }
            }
        }
    }

    public BlockType FillDirtUpToHeight(int y, float height) {
        if (y == 0) {
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

    public float TerrainHeightFromNoise(int x, int z) {
        Vector3 samplingDirection = planet.BaseVector(sideCoord, xCoord, zCoord, x, z);
        
        float terrainHeight = PerlinNoise.get3DPerlinNoise(samplingDirection, 1);
        terrainHeight += 0.7f*PerlinNoise.get3DPerlinNoise(samplingDirection, 2);
        terrainHeight += 0.45f*PerlinNoise.get3DPerlinNoise(samplingDirection, 4);
        terrainHeight /= 1.75f;
        terrainHeight *= (planet.GetChunkHeight() - 2)/3f;
        terrainHeight += 1f; 

        return terrainHeight;
    }

    public Quaternion ChunkToGlobal() {
        Vector3 sideNormal = TerrainManager.instance.sideYaxisList[sideCoord];
        Vector3 sideXaxis = TerrainManager.instance.sideXaxisList[sideCoord];
        Vector3 sideZaxis = TerrainManager.instance.sideZaxisList[sideCoord];

        return Quaternion.LookRotation(sideZaxis, sideNormal); 
    }

    public Quaternion GlobalToChunk() {
        return Quaternion.Inverse(ChunkToGlobal());
    }
}