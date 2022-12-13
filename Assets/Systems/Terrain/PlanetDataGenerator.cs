using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetDataGenerator {
    private PlanetTerrain planet;
    private int chunkSize;
    private int height;
    private int chunksPerSide;

    public PlanetDataGenerator(PlanetTerrain _planet) {
        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunksPerSide = planet.GetChunksPerSide();
        height = planet.GetHeight();
    }

    /*public void Generate() {
        for (int side = 0; side < 6; side++) {
            for (int chunkX = 0; chunkX < chunksPerSide; chunkX++) {
                for (int chunkZ = 0; chunkZ < chunksPerSide; chunkZ++) {

                    GenerateChunk(side, chunkX, chunkZ);
                }
            }
        }
        //AddTrees(planet);
    }*/

    public void GenerateChunk(int sideCoord, int xCoord, int zCoord) {
        Chunk chunk = new Chunk(sideCoord, xCoord, zCoord, planet);
        chunk.CreateChunkData();
        planet.SetChunk(sideCoord, xCoord, zCoord, chunk);
    }

/*  
    public void AddTrees(Planet planet) {
        // TODO: surface this
        int numTrees = 3;
        for (int tree = 0; tree < numTrees; tree++) {
            int randSide = Random.Range(0, 6);
            int chunkX = Random.Range(0, chunksPerSide);
            int chunkZ = Random.Range(0, chunksPerSide);
            int x = Random.Range(0, chunkSize);
            int z = Random.Range(0, chunkSize);

            int stem = 3;
            for (int h = 0; h < chunkHeight && stem > 0; h++) {
                if (planet.chunks[randSide, chunkX, chunkZ].blocks[x, h, z].type.GetName() == "air") {
                    BlockType type = BlockTypeEnum.GetBlockTypeByName("wood");
                    if (stem < 3) type = BlockTypeEnum.GetBlockTypeByName("leaves");
                    planet.chunks[randSide, chunkX, chunkZ].blocks[x, h, z].type = type;
                    stem -= 1;
                }
            }
        }
    }
*/
} 