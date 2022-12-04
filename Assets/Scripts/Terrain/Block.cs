using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class Block {
    public BlockType type;

    public Vector3 inChunkPosition;
    public Vector3 globalPosition;
    public Chunk chunk;

    public BlockSide[] sides;

    public Block(Vector3 _inChunkPosition, BlockType _type, Chunk _chunk) {
        inChunkPosition = _inChunkPosition;
        type = _type;
        chunk = _chunk;

        globalPosition = blockIndexToPointInChunkCoordsBilinear(inChunkPosition);
        sides = computeSides();
    }

    private BlockSide[] computeSides() {
        Quaternion chunkToGlobal = chunk.ChunkToGlobal();
        Quaternion globalToChunk = chunk.GlobalToChunk(); 

        BlockSide[] sideList = new BlockSide[6];
        for (int nextTo=0; nextTo < 6; nextTo++) {
            Vector3 pointingTo = TerrainManager.instance.sideYaxisList[nextTo];
            Vector3 orientedToX = TerrainManager.instance.sideXaxisList[nextTo];
            Vector3 orientedToZ = TerrainManager.instance.sideZaxisList[nextTo];

            string faceDrawn = TerrainManager.instance.sideNameList[nextTo];
            
            Vector3 pos = inChunkPosition;
            Vector3 nextPos = pos + pointingTo;
            Vector3 pointingToGlobal = chunkToGlobal*pointingTo;

            // in chunk coordinate system
            Vector3 normal = pointingTo;
            Vector3 A = orientedToX;
            Vector3 B = orientedToZ;
            Vector3 mid = (pos + nextPos)/2f;
            // to global
            Vector3[] vertices = new Vector3[4] {
                blockIndexToPointInChunkCoords(mid - A/2f - B/2f),
                blockIndexToPointInChunkCoords(mid - A/2f + B/2f),
                blockIndexToPointInChunkCoords(mid + A/2f + B/2f),
                blockIndexToPointInChunkCoords(mid + A/2f - B/2f)
            };
            Vector3 AGlobal = chunkToGlobal * A;
            Vector3 BGlobal = chunkToGlobal * B;
            Vector3 normalGlobal = chunkToGlobal * normal;

            string sideName = TerrainManager.instance.sideNameList[chunk.sideCoord];
            BlockSide side = new BlockSide(vertices, type.GetAtlasCoord(faceDrawn), sideName, faceDrawn);
            sideList[nextTo] = side;
        }
        return sideList;
    }

    private Vector3 blockIndexToPointInChunkCoords(Vector3 pos) {
        int chunkI = chunk.xCoord; 
        int chunkJ = chunk.zCoord;
        int sideCoord = chunk.sideCoord;

        int i = (int)(pos.x + 0.5f);
        int h = (int)(pos.y + 0.5f);
        int j = (int)(pos.z + 0.5f);

        Planet planet = chunk.planet;
        float planetRadius = planet.GetPlanetRadius();
        float blockSize = planet.GetBlockSize();
        return planet.BaseVector(sideCoord, chunkI, chunkJ, i, j) * (planetRadius + blockSize*(h-1));//(blockHeightToHeight[h-1]);
    }

    private Vector3 blockIndexToPointInChunkCoordsBilinear(Vector3 pos) {
        int chunkI = chunk.xCoord; 
        int chunkJ = chunk.zCoord;
        int sideCoord = chunk.sideCoord;
        
        float i = (pos.x + 0.5f);
        float h = (pos.y + 0.5f);
        float j = (pos.z + 0.5f);
        
        Planet planet = chunk.planet;
        float planetRadius = planet.GetPlanetRadius();
        float blockSize = planet.GetBlockSize();
        return planet.BaseVector(sideCoord, chunkI, chunkJ, i, j) * (planetRadius + blockSize * (h - 1));//(blockHeightToHeight[h-1]);
    }

    public Mesh ComputeOutline(float overlineRatio = 1.1f) {
        List<BlockSide> listSides = new List<BlockSide>();
        BlockSide[] sideCopy = computeSides();
        foreach (BlockSide side in sideCopy) {
            Vector3[] vertices = side.GetVertices();
            Vector3[] newVertices = new Vector3[4];
            for (int i=0; i<4; i++) {
                newVertices[i] = globalPosition + (vertices[i] - globalPosition)*overlineRatio; 
            }
            side.SetVertices(newVertices);
            listSides.Add(side);
        }
        Mesh mesh = chunk.planet.GetPlanetMeshGenerator().MeshFromQuads(listSides);
        //mesh.uv = null;
        
        return mesh;
    }
}