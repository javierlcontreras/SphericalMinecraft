using System.Text;
using UnityEngine;

public class ChunkDataGenerator {
    Chunk chunk;

    public ChunkDataGenerator(Chunk _chunk) {
        chunk = _chunk;
    }

    public void CreateChunkData()
    {
        //CreateCubeTestingData(10,0.8f);
        CreateChunkTerrainData();
        //CreateChunkTestingData();
    }

    public void CreateCubeTestingData(float radius, float thickness)
    {
        PlanetTerrain planet = chunk.GetPlanet();
        int maxNumSides = planet.NumBlocksAtHeightPerChunk(planet.GetChunkHeight()-1); 
        for (int y = 0; y<planet.GetChunkHeight(); y++) {
            int numSides = planet.NumBlocksAtHeightPerChunk(y);
            for (int x = 0; x < numSides; x++) {
                for (int z = 0; z < numSides; z++)
                {
                    Vector3 baseVector = planet.BaseVector(chunk.GetSideCoord(), chunk.GetXCoord(), chunk.GetZCoord(), x, y, z);
                    float height = planet.HeightAtBottomOfLayer(y);
                    Vector3 cubeDirUnit = baseVector / Mathf.Max(Mathf.Abs(baseVector.x), Mathf.Abs(baseVector.y), Mathf.Abs(baseVector.z));
                    Vector3 cubeDir = radius * cubeDirUnit;
                    int corner = 0;
                    if (Mathf.Abs(cubeDirUnit.x) > thickness) corner += 1;
                    if (Mathf.Abs(cubeDirUnit.y) > thickness) corner += 1;
                    if (Mathf.Abs(cubeDirUnit.z) > thickness) corner += 1;
                    
                    BlockType type = BlockTypeManager.Singleton.GetByName("air");
                    float dist = (cubeDir - height * baseVector).magnitude;
                    if (dist < 1 && corner >= 2)
                    {
                        type = BlockTypeManager.Singleton.GetByName("stone");
                    }
                    
                    Vector3Int inChunkIndex = new Vector3Int(x, y, z);
                    if (type.GetName() != "air") chunk.SetBlock(x, y, z, new Block(inChunkIndex, type, chunk), true);
                }
            }
        }   
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
                    BlockType type = BlockTypeManager.Singleton.GetByName("air");
                    if (y < 14) type = BlockTypeManager.Singleton.GetByName("grass");
                    Vector3Int inChunkIndex = new Vector3Int(x, y, z);
                    if (type.GetName() != "air") chunk.SetBlock(x, y, z, new Block(inChunkIndex, type, chunk), true);
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
                    if (type.GetName() != "air") chunk.SetBlock(x, y, z, new Block(inChunkIndex, type, chunk),true);
                }
            }
        }
    }
    
    public BlockType FillDirtUpToHeight(Vector3 samplingDirection, int y, float height) {
        PlanetTerrain planet = chunk.GetPlanet();
        if (y < planet.GetChunkMinHeight())
        {
            return BlockTypeManager.Singleton.GetByName("air");;
        }
        if (y == planet.GetChunkMinHeight()) {
            return BlockTypeManager.Singleton.GetByName("bedrock");
        }
        bool cave = planet.GetCaveAt(samplingDirection, 1f*y);
        if (cave) return BlockTypeManager.Singleton.GetByName("air");

        Biome biome = planet.GetBiomeAt(samplingDirection);
        string[] terrainLayerTypes = biome.GetTerrainLayersType();
        float[] terrainLayerHeights = biome.GetTerrainLayersHeight();
        for (int i=0; i<terrainLayerTypes.Length; i++) {
            if (y < terrainLayerHeights[i] * height) {
                return BlockTypeManager.Singleton.GetByName(terrainLayerTypes[i]);
            }
        }
        /*if (biome.GetBiomeName() == "forest" && planet.GetTreeAt(samplingDirection)) {
            float treeHeight = terrainLayerHeights[terrainLayerHeights.Length-1]*height + 5;
            if (y < treeHeight-1) return BlockTypeManager.Instance.GetByName("wood");
            else if (y < treeHeight) return BlockTypeManager.Instance.GetByName("leaves");
        }*/
        return BlockTypeManager.Singleton.GetByName("air");
    }

    public float TerrainHeightFromNoise(Vector3 samplingDirection) {
        PlanetTerrain planet = chunk.GetPlanet();
        float terrainHeight = planet.GetHeightMapAt(samplingDirection);
        terrainHeight *= (planet.GetChunkHeight() - planet.GetChunkMinHeight());
        terrainHeight += planet.GetChunkMinHeight();
        return terrainHeight;
    }
}