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
                    Vector3 samplingDirection = planet.BaseVector(chunk.GetSideCoord(), chunk.GetXCoord(), chunk.GetZCoord(), x, y, z);
                    float height = TerrainHeightFromNoise(samplingDirection); //
                    BlockType type = FillDirtUpToHeight(samplingDirection, y, height);
                    Vector3Int inChunkIndex = new Vector3Int(x, y, z);
                    if (type.GetName() != "air") chunk.SetBlock(x, y, z, new Block(inChunkIndex, type, chunk));
                }
            }
        }
    }
 
    public BlockType FillDirtUpToHeight(Vector3 samplingDirection, int y, float height) {
        PlanetTerrain planet = chunk.GetPlanet();
        if (y <= planet.GetMinHeight()) {
            return BlockTypeEnum.GetBlockTypeByName("bedrock");
        }
        bool cave = planet.GetCaveAt(samplingDirection, 1f*y);
        if (cave) return BlockTypeEnum.GetBlockTypeByName("air");

        Biome biome = planet.GetBiomeAt(samplingDirection);
        string[] terrainLayerTypes = biome.GetTerrainLayersType();
        float[] terrainLayerHeights = biome.GetTerrainLayersHeight();
        for (int i=0; i<terrainLayerTypes.Length; i++) {
            if (y < terrainLayerHeights[i] * height) {
                return BlockTypeEnum.GetBlockTypeByName(terrainLayerTypes[i]);
            }
        }
        if (biome.GetBiomeName() == "forest" && planet.GetTreeAt(samplingDirection)) {
            float treeHeight = terrainLayerHeights[terrainLayerHeights.Length-1]*height + 5;
            if (y < treeHeight-1) return BlockTypeEnum.GetBlockTypeByName("wood");
            else if (y < treeHeight) return BlockTypeEnum.GetBlockTypeByName("leaves");
        }
        return BlockTypeEnum.GetBlockTypeByName("air");
    }

    public float TerrainHeightFromNoise(Vector3 samplingDirection) {
        PlanetTerrain planet = chunk.GetPlanet();
        float terrainHeight = planet.GetHeightMapAt(samplingDirection);
        terrainHeight *= (planet.GetHeight() - 2);
        return terrainHeight;
    }
}