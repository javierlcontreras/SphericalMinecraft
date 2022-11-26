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
        Planet planet = new Planet(chunksPerSide, chunkSize, chunkHeight);

        for (int side = 0; side < 6; side++) {
            for (int chunkX = 0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkY = 0; chunkY < chunksPerSide; chunkY++) {
                    
                    Chunk chunk = GenerateChunk(side, chunkX, chunkY);
                    planet.SetChunk(side, chunkX, chunkY, chunk);
                }
            }
        }
        AddTrees(planet);
        return planet;
    }

    public void AddTrees(Planet planet) {
        // TODO: surface this
        int numTrees = 300;
        for (int tree = 0; tree < numTrees; tree++) {
            int randSide = Random.Range(0, 6);
            int chunkX = Random.Range(0, chunksPerSide);
            int chunkY = Random.Range(0, chunksPerSide);
            int x = Random.Range(0, chunkSize);
            int y = Random.Range(0, chunkSize);

            int stem = 3;
            for (int h = 0; h < chunkHeight && stem > 0; h++) {
                if (planet.chunks[randSide, chunkX, chunkY].blocks[x, y, h].type.GetName() == "air") {
                    BlockType type = BlockTypeEnum.GetBlockTypeByName("wood");
                    if (stem < 3) type = BlockTypeEnum.GetBlockTypeByName("leaves");
                    planet.chunks[randSide, chunkX, chunkY].blocks[x, y, h].type = type;
                    stem -= 1;
                }
            }
        }
    }

    public Chunk GenerateChunk(int sideCoord, int xCoord, int yCoord) {
        Chunk chunk = new Chunk(chunkSize, chunkHeight, sideCoord, xCoord, yCoord);
        //chunk.Flatten(1);
        //chunk.DebugPattern();
        chunk.FromNoise();
        return chunk;
    }
} 