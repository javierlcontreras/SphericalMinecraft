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
    private BlockSide[] sides;
    public Block(int _xCoord, int _yCoord, int _zCoord, BlockType _type, Chunk _chunk) {
        xCoord = _xCoord;
        yCoord = _yCoord;
        zCoord = _zCoord;
        type = _type;
        chunk = _chunk;

        inChunkPosition = new Vector3(zCoord, yCoord, zCoord);
        vertexPositions = computeVertexPositions();
        globalPosition = averageOf(vertexPositions);
        sides = new BlockSide[6];
    }

    public BlockSide GetSide(int index, bool memo = true) {
        if (memo && sides[index] != null) return sides[index];
        Quaternion chunkToGlobal = chunk.ChunkToGlobal();
        Quaternion globalToChunk = chunk.GlobalToChunk(); 

        int option=index*4;
        string faceDrawn = TerrainGenerationConstants.sideNameList[option/4];
        int vertex1 = TerrainGenerationConstants.sideOptions[option];
        int vertex2 = TerrainGenerationConstants.sideOptions[option+1];
        int vertex3 = TerrainGenerationConstants.sideOptions[option+2];
        int vertex4 = TerrainGenerationConstants.sideOptions[option+3];
        Vector3[] vertices = new Vector3[] {
            vertexPositions[vertex1],    
            vertexPositions[vertex2],    
            vertexPositions[vertex3],    
            vertexPositions[vertex4]    
        };

        string sideName = TerrainGenerationConstants.sideNameList[chunk.GetSideCoord()];
        BlockSide blockSide = new BlockSide(vertices, type.GetAtlasCoord(faceDrawn), sideName, faceDrawn);
        if (memo) {
            sides[index] = blockSide;
        }
        return blockSide;
    }

    public BlockType GetBlockType() {
        return type;
    }
    public void SetBlockType(BlockType _type) {
        type = _type;
        sides = new BlockSide[6];
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
            int dx = TerrainGenerationConstants.vertexOptions[option];
            int dy = TerrainGenerationConstants.vertexOptions[option+1];
            int dz = TerrainGenerationConstants.vertexOptions[option+2];
            int vertexX = xCoord + dx;
            int vertexY = yCoord + (dy - 1);
            int vertexZ = zCoord + dz;
            

            int blocks = chunk.GetPlanet().NumBlocksAtHeight(yCoord);
            int blocksNextTo = chunk.GetPlanet().NumBlocksAtHeight(vertexY);
            if (blocksNextTo < blocks && dy == 0) {
                PlanetTerrain planet = chunk.GetPlanet();
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
        PlanetTerrain planet = chunk.GetPlanet();
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

    public Mesh ComputeOutline(float overlineRatio = 1.1f) {
        List<BlockSide> listSides = new List<BlockSide>();
        for (int sideIndex=0; sideIndex<6; sideIndex++) {
            BlockSide side = GetSide(sideIndex, false);
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