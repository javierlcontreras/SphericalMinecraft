using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block {
    [SerializeField] private BlockType type;
    private Vector3Int inChunkIndex;
    private Chunk chunk;

    private Vector3?[] vertexPositions;
    private BlockSide[] sides;
    private Vector3? blockPositionFromPlanetReference;

    public Block(Vector3Int _inChunkIndex, BlockType _type, Chunk _chunk) {
        inChunkIndex = _inChunkIndex;
        type = _type;
        chunk = _chunk;

        vertexPositions = new Vector3?[8];
        sides = new BlockSide[6];
    }

    public Vector3 GetVertex(int index, bool memo = true) {
        if (memo && vertexPositions[index] != null) {
            return (Vector3) vertexPositions[index] !;
        }
        int option = 3*index;
        int dx = TerrainGenerationConstants.vertexOptions[option];
        int dy = TerrainGenerationConstants.vertexOptions[option+1];
        int dz = TerrainGenerationConstants.vertexOptions[option+2];
        int vertexX = inChunkIndex.x + dx;
        int vertexY = inChunkIndex.y + (dy - 1);
        int vertexZ = inChunkIndex.z + dz;
        
        Vector3 vertex;
        int blocks = chunk.GetPlanet().NumBlocksAtHeight(inChunkIndex.y);
        int blocksNextTo = chunk.GetPlanet().NumBlocksAtHeight(vertexY);
        PlanetTerrain planet = chunk.GetPlanet();
        if (blocksNextTo < blocks && vertexY >= planet.GetChunkMinHeight()) {
            float radius = (TerrainGenerationConstants.GetBlockHeight() + planet.HeightAtBottomOfLayer(vertexY));
            float height = radius;
            if (vertexX%2 != 0 && vertexZ%2 != 0) {
                Vector3 pos0 = chunkIndexToGlobalPosition(vertexX-1, inChunkIndex.y, vertexZ-1).normalized * radius; 
                Vector3 pos1 = chunkIndexToGlobalPosition(vertexX-1, inChunkIndex.y, vertexZ+1).normalized * radius; 
                Vector3 pos2 = chunkIndexToGlobalPosition(vertexX+1, inChunkIndex.y, vertexZ-1).normalized * radius; 
                Vector3 pos3 = chunkIndexToGlobalPosition(vertexX+1, inChunkIndex.y, vertexZ+1).normalized * radius; 
                height = (0.25f*(pos0+pos1+pos2+pos3)).magnitude; 
            }
            else if (vertexX%2 != 0) {
                Vector3 pos0 = chunkIndexToGlobalPosition(vertexX-1, inChunkIndex.y, vertexZ).normalized * radius; 
                Vector3 pos1 = chunkIndexToGlobalPosition(vertexX+1, inChunkIndex.y, vertexZ).normalized * radius; 
                height = (0.5f*(pos0+pos1)).magnitude; 
            }
            else if (vertexZ%2 != 0) {
                Vector3 pos0 = chunkIndexToGlobalPosition(vertexX, inChunkIndex.y, vertexZ-1).normalized * radius; 
                Vector3 pos1 = chunkIndexToGlobalPosition(vertexX, inChunkIndex.y, vertexZ+1).normalized * radius; 
                height = (0.5f*(pos0+pos1)).magnitude; 
            } 
            Vector3 pos = chunkIndexToGlobalPosition(vertexX, inChunkIndex.y, vertexZ).normalized * height;
            vertex = pos;
        }
        else {
            vertex = chunkIndexToGlobalPosition(vertexX, vertexY, vertexZ);
        } 
        if (memo) vertexPositions[index] = vertex;
        return vertex;
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
            GetVertex(vertex1, memo),    
            GetVertex(vertex2, memo),    
            GetVertex(vertex3, memo),
            GetVertex(vertex4, memo)    
        };

        string sideName = TerrainGenerationConstants.sideNameList[chunk.GetSideCoord()];
        BlockSide blockSide = new BlockSide(vertices, type.GetAtlasCoord(faceDrawn), sideName, faceDrawn);
        if (memo) {
            sides[index] = blockSide;
        }
        return blockSide;
    }

    public Vector3 GetBlockPosition(bool memo = true) {
        if (memo && blockPositionFromPlanetReference != null) return (Vector3) blockPositionFromPlanetReference;

        Vector3 res = Vector3.zero;
        for (int i=0; i<8; i++) {
            res += GetVertex(i, memo) / 8f;
        }

        if (memo) blockPositionFromPlanetReference = res;
        return res;
    }

    public Vector3 GetBlockGlobalPosition(bool memo = true) {
        Vector3 position = GetBlockPosition(memo);
        return chunk.GetPlanet().gameObject.transform.TransformPoint(position);
    }

    public float DistanceTo(Vector3 pointInGlobal)
    {
        Vector3 blockCenter = GetBlockGlobalPosition();
        return (blockCenter - pointInGlobal).magnitude;
    }


    public BlockType GetBlockType() {
        return type;
    }
    public void SetBlockType(BlockType _type) {
        type = _type;
        sides = new BlockSide[6];
    }
    public Vector3Int GetInChunkIndex() {
        return inChunkIndex;
    }
    public Chunk GetChunk() {
        return chunk;
    }

    private Vector3 chunkIndexToGlobalPosition(int vertexX, int vertexY, int vertexZ) {
        int chunkSide = chunk.GetSideCoord();
        int chunkX = chunk.GetXCoord();
        int chunkZ = chunk.GetZCoord();
        PlanetTerrain planet = chunk.GetPlanet();
        return planet.BaseVector(chunkSide, chunkX, chunkZ, vertexX, vertexY, vertexZ) * 
                    (TerrainGenerationConstants.GetBlockHeight() + planet.HeightAtBottomOfLayer(vertexY));
    }

    public Mesh ComputeOutline(float overlineRatio = 1.05f) {
        List<BlockSide> listSides = new List<BlockSide>();
        for (int sideIndex=0; sideIndex<6; sideIndex++) {
            BlockSide side = GetSide(sideIndex, false);
            Vector3[] vertices = side.GetVertices();
            Vector3[] newVertices = new Vector3[4];
            for (int i=0; i<4; i++) {
                Vector3 pos = GetBlockPosition();
                newVertices[i] = pos + (vertices[i] - pos)*overlineRatio; 
            }
            side.SetVertices(newVertices);
            listSides.Add(side);
        }
        Mesh mesh = chunk.GetPlanet().GetPlanetMeshGenerator().MeshFromQuads(listSides, true);
        //mesh.uv = null;
        
        return mesh;
    }

}