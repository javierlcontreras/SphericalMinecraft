using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetDataGenerator {
    int chunkSize;
    int chunkHeight;
    int chunksPerSide;

    public PlanetDataGenerator(int _chunkSize, int _chunkHeight, int _chunksPerSide) {
        chunkSize = _chunkSize;
        chunkHeight = _chunkHeight;
        chunksPerSide = _chunksPerSide;
    }

    public Planet Generate() {
        Planet planet = new Planet(chunksPerSide, chunkSize);

        for (int side = 0; side < 6; side++) {
            for (int chunkX = 0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkY = 0; chunkY < chunksPerSide; chunkY++) {
                    
                    Chunk chunk = GenerateChunk(side, chunkX, chunkY);
                    planet.SetChunk(side, chunkX, chunkY, chunk);

                }
            }
        }
        return planet;
    }


    public Chunk GenerateChunk(int sideCoord, int xCoord, int yCoord) {
        Chunk chunk = new Chunk(chunkSize, chunkHeight, sideCoord, xCoord, yCoord);
        chunk.Flatten(chunkHeight/2);
        return chunk;
    }
} 