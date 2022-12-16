using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chunk {
    private int chunkSize, chunkHeight;
    private int sideCoord;
    private int xCoord;
    private int zCoord;
    public int GetSideCoord() { return sideCoord; }
    public int GetXCoord() { return xCoord; }
    public int GetZCoord() { return zCoord; }
    
    //private Block[,,] blocks;
    private Dictionary<Vector3Int, Block> blocks;
    public Block GetBlock(int x, int y, int z) {
        Vector3Int pos = new Vector3Int(x, y, z);
        if (blocks.ContainsKey(pos)) return blocks[pos];
        return null;
    }
    public void SetBlock(int x, int y, int z, Block block) {
        Vector3Int pos = new Vector3Int(x, y, z);
        if (block == null) {
            blocks.Remove(pos);
        }
        else blocks[pos] = block;
    } 
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
        blocks = new Dictionary<Vector3Int, Block>();
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