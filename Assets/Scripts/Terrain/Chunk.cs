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
}