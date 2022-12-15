using System.Text;
using UnityEngine;

public class ChunkDataGenerator {
    Chunk chunk;

    public ChunkDataGenerator(Chunk _chunk) {
        chunk = _chunk;
    }

    public void CreateChunkData() {
        PlanetTerrain planet = chunk.GetPlanet();
        int maxNumSides = planet.NumBlocksAtHeightPerChunk(planet.GetHeight()-1); 
        for (int y = 0; y<planet.GetHeight(); y++) {
            int numSides = planet.NumBlocksAtHeightPerChunk(y);
            for (int x = 0; x < numSides; x++) {
                for (int z = 0; z < numSides; z++) { 
                    float height = 0.5f * planet.GetHeight()/2;//TerrainHeightFromNoise(x,y,z);
                    
                    BlockType type = FillDirtUpToHeight(y, height);
                    Vector3Int inChunkIndex = new Vector3Int(x, y, z);
                    if (type.GetName() != "air") chunk.blocks[x, y, z] = new Block(inChunkIndex, type, chunk);
                }
            }
        }
    }
 
    public BlockType FillDirtUpToHeight(int y, float height) {
        PlanetTerrain planet = chunk.GetPlanet();
        if (y <= planet.GetMinHeight()) {
            return BlockTypeEnum.GetBlockTypeByName("bedrock");
        }
        string[] terrainLayerTypes = planet.terrainLayersTypes;
        float[] terrainLayerHeights = planet.terrainLayersHeights;

        for (int i=0; i<terrainLayerTypes.Length; i++) {
            if (y < terrainLayerHeights[i] * height) {
                return BlockTypeEnum.GetBlockTypeByName(terrainLayerTypes[i]);
            }
        }
        return BlockTypeEnum.GetBlockTypeByName("air");
    }

    public float TerrainHeightFromNoise(int x, int y, int z) {
        Vector3 samplingDirection = chunk.GetPlanet().BaseVector(chunk.GetSideCoord(), chunk.GetXCoord(), chunk.GetZCoord(), x, y, z);
        
        float terrainHeight = PerlinNoise.get3DPerlinNoise(samplingDirection, 1);
        terrainHeight += 0.7f*PerlinNoise.get3DPerlinNoise(samplingDirection, 2);
        terrainHeight += 0.45f*PerlinNoise.get3DPerlinNoise(samplingDirection, 4);
        terrainHeight /= 1.75f;
        terrainHeight *= (chunk.GetPlanet().GetHeight() - 2)/2f;
        terrainHeight += 1f; 

        return terrainHeight;
    }
}