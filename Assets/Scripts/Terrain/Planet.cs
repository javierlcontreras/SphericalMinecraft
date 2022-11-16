using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Planet {
    private int chunksPerSide;
    private int chunkSize;
    private int chunkHeight;
    public Chunk[,,] chunks;
    
    public Planet(int _chunksPerSide, int _chunkSize, int _chunkHeight) {
        chunksPerSide = _chunksPerSide;
        chunkHeight = _chunkHeight;
        chunkSize = _chunkSize;
        chunks = new Chunk[6, chunksPerSide, chunksPerSide];
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