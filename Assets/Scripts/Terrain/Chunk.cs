using UnityEngine;

public class Chunk {
    private int chunkSize, chunkHeight;
    public int sideCoord {get;}
    public int xCoord {get;}
    public int zCoord {get;}
    
    public Block[,,] blocks;
    
    public Chunk(int _chunkSize, int _chunkHeight, int _sideCoord, int _xCoord, int _zCoord) {
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        zCoord = _zCoord;

        blocks = new Block[chunkSize, chunkHeight, chunkSize];
    }

    public float DistanceToChunk(Chunk nextChunk) {
        int sideNext = nextChunk.sideCoord; 
        int nextChunkX = nextChunk.xCoord; 
        int nextChunkZ = nextChunk.zCoord;

        int x1 = xCoord * chunkSize + chunkSize/2;
        int z1 = zCoord * chunkSize + chunkSize/2;
        int x2 = nextChunkX * chunkSize + chunkSize/2;
        int z2 = nextChunkZ * chunkSize + chunkSize/2;
        Vector3 base1 = TerrainManager.instance.BaseVectorAtCenter(sideCoord, xCoord, zCoord);
        Vector3 base2 = TerrainManager.instance.BaseVectorAtCenter(sideNext, nextChunkX, nextChunkZ);

        return (base1 - base2).magnitude;
    }

    public void CreateChunkData() {
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {
                for (int y = 0; y<chunkHeight; y++) {
                    float height = TerrainHeightFromNoise(x, z);
                    BlockType type = FillDirtUpToHeight(y, height);

                    Vector3 inChunkPosition = new Vector3(x,y,z);
                    blocks[x, y, z] = new Block(inChunkPosition, type, this);
                }
            }
        }
    }

    public BlockType FillDirtUpToHeight(int y, float height) {
        if (y <= height) {
            return BlockTypeEnum.GetBlockTypeByName("dirt");
        }
        else {
            return BlockTypeEnum.GetBlockTypeByName("air");
        }
    }

    public float TerrainHeightFromNoise(int x, int z) {
        Vector3 samplingDirection = TerrainManager.instance.BaseVector(sideCoord, xCoord, zCoord, x, z);
        
        float terrainHeight = PerlinNoise.get3DPerlinNoise(samplingDirection, 1);
        terrainHeight += 0.5f*PerlinNoise.get3DPerlinNoise(samplingDirection, 2);
        terrainHeight += 0.25f*PerlinNoise.get3DPerlinNoise(samplingDirection, 4);
        terrainHeight /= 1.75f;
        terrainHeight *= (TerrainManager.instance.ChunkHeight - 2)/3f;
        terrainHeight += 1f; 

        return terrainHeight;
    }
}