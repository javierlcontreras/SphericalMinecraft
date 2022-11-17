using UnityEngine;

public class Chunk {
    int chunkSize, chunkHeight;
    public int sideCoord {get;}
    public int xCoord {get;}
    public int yCoord {get;}
    
    public Block[,,] blocks;
    
    public Chunk(int _chunkSize, int _chunkHeight, int _sideCoord, int _xCoord, int _yCoord) {
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        yCoord = _yCoord;

        blocks = new Block[chunkSize, chunkSize, chunkHeight];
    }

    public float DistanceToChunk(Chunk nextChunk) {
        int sideNext = nextChunk.sideCoord; 
        int nextChunkX = nextChunk.xCoord; 
        int nextChunkY = nextChunk.yCoord;

        int x1 = xCoord * chunkSize + chunkSize/2;
        int y1 = yCoord * chunkSize + chunkSize/2;
        int x2 = nextChunkX * chunkSize + chunkSize/2;
        int y2 = nextChunkY * chunkSize + chunkSize/2;
        Vector3 base1 = TerrainManager.instance.baseVectors[sideCoord, x1, y1];
        Vector3 base2 = TerrainManager.instance.baseVectors[sideNext, x2, y2];

        return (base1 - base2).magnitude;
    }

    public void Flatten(int height) {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int h=0; h<chunkHeight; h++) {
                    BlockType type;
                    if (h < height) {
                        type = BlockTypeEnum.GetBlockTypeByName("dirt");
                    }
                    else {
                        type = BlockTypeEnum.GetBlockTypeByName("air");
                    }
                    blocks[x, y, h] = new Block(x, y, h, type);
                }
            }    
        }
    }
    public void DebugPattern() {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int h=0; h<chunkHeight; h++) {
                    BlockType type;
                    if (h <= Random.Range(1, 3)) {
                        type = BlockTypeEnum.GetBlockTypeByName("dirt");
                    }
                    else {
                        type = BlockTypeEnum.GetBlockTypeByName("air");
                    }
                    blocks[x, y, h] = new Block(x, y, h, type);
                }
            }    
        }
    }

    public void FromNoise() {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                Vector3 samplingDirection = TerrainManager.instance.baseVectors[sideCoord, xCoord*chunkSize + x, yCoord*chunkSize + y];
                
                float terrainHeight = PerlinNoise.get3DPerlinNoise(samplingDirection, 1);
                terrainHeight += 0.5f*PerlinNoise.get3DPerlinNoise(samplingDirection, 2);
                terrainHeight += 0.25f*PerlinNoise.get3DPerlinNoise(samplingDirection, 4);
                terrainHeight /= 1.75f;
                terrainHeight *= (TerrainManager.instance.ChunkHeight - 2)/3f;
                terrainHeight += 1f; 
                //Debug.Log(terrainHeight);

                for (int h=0; h<chunkHeight; h++) {
                    BlockType type;
                    if (h <= terrainHeight) {
                        type = BlockTypeEnum.GetBlockTypeByName("dirt");
                    }
                    else {
                        type = BlockTypeEnum.GetBlockTypeByName("air");
                    }
                    blocks[x, y, h] = new Block(x, y, h, type);
                }
            }    
        }        
    }
}