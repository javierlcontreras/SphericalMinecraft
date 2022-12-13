using System.Collections;
using System.Collections.Generic;

using UnityEngine;
public class Block {
    private BlockType type;

    private int xCoord, yCoord, zCoord;
    private Vector3 inChunkPosition;
    private Vector3 globalPosition;
    private Chunk chunk;

    public Vector3[] vertexPositions;
    public BlockSide[] sides;

    public Block(int _xCoord, int _yCoord, int _zCoord, BlockType _type, Chunk _chunk) {
        xCoord = _xCoord;
        yCoord = _yCoord;
        zCoord = _zCoord;
        type = _type;
        chunk = _chunk;

        inChunkPosition = new Vector3(zCoord, yCoord, zCoord);
        vertexPositions = computeVertexPositions();
        globalPosition = averageOf(vertexPositions);
        sides = computeSides();
    }

    public BlockType GetBlockType() {
        return type;
    }
    public void SetBlockType(BlockType _type) {
        type = _type;
        sides = computeSides();
    }
    public Vector3 GetInChunkPosition() {
        return inChunkPosition;
    }
    public Chunk GetChunk() {
        return chunk;
    }
    private Vector3[] computeVertexPositions() {
        Vector3[] vertices = new Vector3[8];
       
        for (int option=0; option < 8*3; option += 3) {
            int dx = TerrainManager.vertexOptions[option];
            int dy = TerrainManager.vertexOptions[option+1];
            int dz = TerrainManager.vertexOptions[option+2];
            int vertexX = xCoord + dx;
            int vertexY = yCoord + (dy - 1);
            int vertexZ = zCoord + dz;
            

            int blocks = chunk.GetPlanet().NumBlocksAtHeight(yCoord);
            int blocksNextTo = chunk.GetPlanet().NumBlocksAtHeight(vertexY);
            if (blocksNextTo < blocks && dy == 0) {
                Planet planet = chunk.GetPlanet();
                Vector3 pos = chunkIndexToGlobalPosition(vertexX, yCoord, vertexZ).normalized * (1 + planet.HeightAtBottomOfLayer(vertexY));
                vertices[option/3] = pos;
            }
            else {
                vertices[option/3] = chunkIndexToGlobalPosition(vertexX, vertexY, vertexZ);
            } 

        }
        return vertices;
    }
    private Vector3 chunkIndexToGlobalPosition(int vertexX, int vertexY, int vertexZ) {
        int chunkSide = chunk.GetSideCoord();
        int chunkX = chunk.GetXCoord();
        int chunkZ = chunk.GetZCoord();
        Planet planet = chunk.GetPlanet();
        return planet.BaseVector(chunkSide, chunkX, chunkZ, vertexX, vertexY, vertexZ) * (1 + planet.HeightAtBottomOfLayer(vertexY));
    }

    private Vector3 averageOf(Vector3[] list) {
        Vector3 res = Vector3.zero;
        int n = list.Length;
        for (int i=0; i<n; i++) {
            res += list[i] / (1f*n);
        }
        return res;
    }

    private BlockSide[] computeSides() {
        Quaternion chunkToGlobal = chunk.ChunkToGlobal();
        Quaternion globalToChunk = chunk.GlobalToChunk(); 

        BlockSide[] sideList = new BlockSide[6];
        for (int option=0; option < 6*4; option += 4) {
            string faceDrawn = TerrainManager.sideNameList[option/4];
            int vertex1 = TerrainManager.sideOptions[option];
            int vertex2 = TerrainManager.sideOptions[option+1];
            int vertex3 = TerrainManager.sideOptions[option+2];
            int vertex4 = TerrainManager.sideOptions[option+3];
            Vector3[] vertices = new Vector3[] {
                vertexPositions[vertex1],    
                vertexPositions[vertex2],    
                vertexPositions[vertex3],    
                vertexPositions[vertex4]    
            };

            string sideName = TerrainManager.sideNameList[chunk.GetSideCoord()];
            BlockSide side = new BlockSide(vertices, type.GetAtlasCoord(faceDrawn), sideName, faceDrawn);
            sideList[option/4] = side;
        }
        return sideList;
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
        Mesh mesh = chunk.GetPlanet().GetPlanetMeshGenerator().MeshFromQuads(listSides, true);
        //mesh.uv = null;
        
        return mesh;
    }

}