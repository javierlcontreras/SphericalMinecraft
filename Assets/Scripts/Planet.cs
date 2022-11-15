using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Planet {
    int chunksPerSide;
    int chunkSize;
    Chunk[,,] chunks;
    
    public Planet(int _chunksPerSide, int _chunkSize) {
        chunksPerSide = _chunksPerSide;
        chunkSize = _chunkSize;
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
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
}