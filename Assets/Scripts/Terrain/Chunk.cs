using UnityEngine;

public class Chunk {
    int chunkSize, chunkHeight;
    int sideCoord, xCoord, yCoord; // relative to the planet

    public Block[,,] blocks;
    
    public Chunk(int _chunkSize, int _chunkHeight, int _sideCoord, int _xCoord, int _yCoord) {
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        yCoord = _yCoord;

        blocks = new Block[chunkSize, chunkSize, chunkHeight];
    }

    public void Flatten(int height) {
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int h=0; h<chunkHeight; h++) {
                    BlockType type;
                    if (h <= height) {
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
                terrainHeight *= (TerrainManager.instance.GetChunkHeight() - 2);
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