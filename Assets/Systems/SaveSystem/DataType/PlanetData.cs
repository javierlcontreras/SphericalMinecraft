using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
class PlanetData {
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public int chunksPerSide;
    public int chunkSize;
    public int chunkHeight;
    public int chunkMinHeight;

    public SerializableDictionary<Vector3Int, string>[] chunksEncoding;
    public Vector3Int[] chunksPositions; 

    public PlanetData(GameObject planet) {
        PlanetTerrain planetTerrain = planet.GetComponent<PlanetTerrain>();
        CelestialBody planetBody = planet.GetComponent<CelestialBody>();
        
        position = planetBody.GetPosition();
        rotation = planetBody.GetRotation();
        velocity = planetBody.GetVelocity();

        chunksPerSide = planetTerrain.GetChunksPerSide();
        chunkSize = planetTerrain.GetChunkSize();
        chunkHeight = planetTerrain.GetChunkHeight();
        chunkMinHeight = planetTerrain.GetChunkMinHeight();

        SerializeChunks(planetTerrain.GetAllChunks());
    }

    public void SerializeChunks(Chunk[,,] chunks) {
        int totalChunks = 6*chunksPerSide*chunksPerSide; 
        chunksEncoding = new SerializableDictionary<Vector3Int, string>[totalChunks];
        chunksPositions = new Vector3Int[totalChunks];

        for (int sideCoord=0; sideCoord<6; sideCoord++) {
            for (int chunkX=0; chunkX<chunksPerSide; chunkX++) {
                for (int chunkZ=0; chunkZ<chunksPerSide; chunkZ++) {
                    int index = sideCoord*chunksPerSide*chunksPerSide + chunkX*chunksPerSide + chunkZ;
                    chunksEncoding[index] = SerializeChunk(chunks[sideCoord, chunkX, chunkZ]);
                    chunksPositions[index] = new Vector3Int(sideCoord, chunkX, chunkZ);
                }
            }
        }
    }

    public SerializableDictionary<Vector3Int, string> SerializeChunk(Chunk chunk) {
        if (chunk == null) return null;
        SerializableDictionary<Vector3Int, string> chunkData = new SerializableDictionary<Vector3Int, string>();
        foreach (KeyValuePair<Vector3Int, Block> pair in chunk.blocks) {
            chunkData.Add(pair.Key, pair.Value.GetBlockType().GetName());
        }
        return chunkData;
    }

    public void SetChunks(PlanetTerrain planetTerrain) {
        int chunksPerSide = planetTerrain.GetChunksPerSide();
        Chunk[,,] chunks = new Chunk[6, chunksPerSide, chunksPerSide];
        for (int sideCoord=0; sideCoord<6; sideCoord++) {
            for (int chunkX=0; chunkX<chunksPerSide; chunkX++) {
                for (int chunkZ=0; chunkZ<chunksPerSide; chunkZ++) {
                    int index = sideCoord*chunksPerSide*chunksPerSide + chunkX*chunksPerSide + chunkZ;
                    Vector3Int chunkPosition = new Vector3Int(sideCoord, chunkX, chunkZ);
                    chunks[sideCoord, chunkX, chunkZ] = GetChunk(chunksEncoding[index], chunkPosition, planetTerrain);
                }
            }
        }
        planetTerrain.SetAllChunks(chunks);
    }
    
    public Chunk GetChunk(SerializableDictionary<Vector3Int, string> chunkData, Vector3Int chunkPosition, PlanetTerrain planetTerrain) {
        if (chunkData == null) return null;
        Chunk chunk = new Chunk(chunkPosition, planetTerrain);
        foreach (KeyValuePair<Vector3Int, string> pair in chunkData) {
            BlockType type = BlockTypeEnum.GetByName(pair.Value);
            chunk.blocks.Add(pair.Key, new Block(pair.Key, type, chunk));
        }
        return chunk;
    }
}
