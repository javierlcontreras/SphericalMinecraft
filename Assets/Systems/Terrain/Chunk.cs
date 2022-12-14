using System.Text;
using UnityEngine;

public class Chunk {
    private int chunkSize, chunkHeight;
    private int sideCoord;
    private int xCoord;
    private int zCoord;
    public int GetSideCoord() { return sideCoord; }
    public int GetXCoord() { return xCoord; }
    public int GetZCoord() { return zCoord; }
    
    public Block[,,] blocks;
    private PlanetTerrain planet;
    public PlanetTerrain GetPlanet() { return planet; }
    
    private ChunkDataGenerator chunkDataGenerator;
    public ChunkDataGenerator GetChunkDataGenerator() {
        return chunkDataGenerator;
    }

    public Chunk(int _sideCoord, int _xCoord, int _zCoord, PlanetTerrain _planet) {
        sideCoord = _sideCoord;
        xCoord = _xCoord;
        zCoord = _zCoord;

        planet = _planet;
        chunkSize = planet.GetChunkSize();
        chunkHeight = planet.GetHeight();
        blocks = new Block[chunkSize, chunkHeight, chunkSize];
        chunkDataGenerator = new ChunkDataGenerator(this);
    }

    public float DistanceToChunk(Chunk nextChunk) {
        int sideNext = nextChunk.sideCoord; 
        int nextChunkX = nextChunk.xCoord; 
        int nextChunkZ = nextChunk.zCoord;

        Vector3 base1 = planet.BaseVectorAtCenter(sideCoord, xCoord, zCoord);
        Vector3 base2 = planet.BaseVectorAtCenter(sideNext, nextChunkX, nextChunkZ);

        return (base1 - base2).magnitude;
    }
/*
    public void DebugChunkDataAtHeight(int y) {
        var builder = new StringBuilder();
        int numSides = planet.NumBlocksAtHeightPerChunk(y);
        Debug.Log("Real chunkSize " + numSides); 
        for (int x = 0; x < numSides; x++) {
            for (int z = 0; z < numSides; z++) { 
                BlockType type = blocks[x, y, z].GetBlockType();
                builder.Append(type.GetName()[0]);
            }
            builder.Append("\n");
        }
        Debug.Log(builder.ToString());
    }*/

    public Quaternion ChunkToGlobal() {
        Vector3 sideNormal = TerrainGenerationConstants.sideYaxisList[sideCoord];
        Vector3 sideXaxis = TerrainGenerationConstants.sideXaxisList[sideCoord];
        Vector3 sideZaxis = TerrainGenerationConstants.sideZaxisList[sideCoord];

        return Quaternion.LookRotation(sideZaxis, sideNormal); 
    }

    public Quaternion GlobalToChunk() {
        return Quaternion.Inverse(ChunkToGlobal());
    }
}