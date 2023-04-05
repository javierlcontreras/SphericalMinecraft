using System.Text;
using UnityEngine;

public class ChunkDataGenerator {
    Chunk chunk;

    public ChunkDataGenerator(Chunk _chunk) {
        chunk = _chunk;
    }

    public void CreateChunkData()
    {
        //CreateChunkTerrainData();
        CreateChunkTestingData();
    }

    public void CreateChunkTestingData()
    {
        PlanetTerrain planet = chunk.GetPlanet();
        int maxNumSides = planet.NumBlocksAtHeightPerChunk(planet.GetChunkHeight()-1); 
        for (int y = 0; y<planet.GetChunkHeight(); y++) {
            int numSides = planet.NumBlocksAtHeightPerChunk(y);
            for (int x = 0; x < numSides; x++) {
                for (int z = 0; z < numSides; z++)
                {
                    BlockType type = BlockTypeEnum.GetByName("air");
                    if (y < 60) type = BlockTypeEnum.GetByIndex((chunk.GetXCoord() + 3*chunk.GetZCoord())%8+1);
                    Vector3Int inChunkIndex = new Vector3Int(x, y, z);
                    if (type.GetName() != "air") chunk.SetBlock(x, y, z, new Block(inChunkIndex, type, chunk));
                }
            }
        }   
    }
    
    public void CreateChunkTerrainData()
    {
        PlanetTerrain planet = chunk.GetPlanet();
        int maxNumSides = planet.NumBlocksAtHeightPerChunk(planet.GetChunkHeight()-1); 
        for (int y = 0; y<planet.GetChunkHeight(); y++) {
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
        if (y <= planet.GetChunkMinHeight()) {
            return BlockTypeEnum.GetByName("bedrock");
        }
        bool cave = planet.GetCaveAt(samplingDirection, 1f*y);
        if (cave) return BlockTypeEnum.GetByName("air");

        Biome biome = planet.GetBiomeAt(samplingDirection);
        string[] terrainLayerTypes = biome.GetTerrainLayersType();
        float[] terrainLayerHeights = biome.GetTerrainLayersHeight();
        for (int i=0; i<terrainLayerTypes.Length; i++) {
            if (y < terrainLayerHeights[i] * height) {
                return BlockTypeEnum.GetByName(terrainLayerTypes[i]);
            }
        }
        if (biome.GetBiomeName() == "forest" && planet.GetTreeAt(samplingDirection)) {
            float treeHeight = terrainLayerHeights[terrainLayerHeights.Length-1]*height + 5;
            if (y < treeHeight-1) return BlockTypeEnum.GetByName("wood");
            else if (y < treeHeight) return BlockTypeEnum.GetByName("leaves");
        }
        return BlockTypeEnum.GetByName("air");
    }

    public float TerrainHeightFromNoise(Vector3 samplingDirection) {
        PlanetTerrain planet = chunk.GetPlanet();
        float terrainHeight = planet.GetHeightMapAt(samplingDirection);
        terrainHeight *= (planet.GetChunkHeight() - 2);
        return terrainHeight;
    }
}