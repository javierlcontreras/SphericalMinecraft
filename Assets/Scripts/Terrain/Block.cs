using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Block {
    [SerializeField] private BlockType type;
    private BlockCoordinateInformation coords;
    private Chunk chunk;
    
    private Vector3?[] vertexPositions;
    private int[] vertexAmbientOcclusionCounts;
    
    private BlockSide[] sides;
    
    private Vector3? blockPositionFromPlanetReference;
    private int[] ambientOcclusionCounts;

    public Block(Vector3Int _inChunkIndex, BlockType _type, Chunk _chunk) {
        coords = new BlockCoordinateInformation(_inChunkIndex, _chunk.GetChunkCoords(), _chunk.GetPlanet());
        type = _type;
        chunk = _chunk;

        vertexPositions = new Vector3?[8];
        sides = new BlockSide[6];
        RecomputeAmbientOcclusions();
    }

    public int VertexAmbientOcclusionCount(int vertexIndex, bool memo = true)
    {
        if (memo && ambientOcclusionCounts[vertexIndex] != -1) return ambientOcclusionCounts[vertexIndex];
        
        BlockAdjacency[] sideBlocks = chunk.GetPlanet().GetPlanetMeshGenerator().GetChunkAdjacencyCalculator()
            .BlocksTouchingAVertex(
                coords, TerrainGenerationConstants.vertexOptions[vertexIndex]);

        int count = 0;
        foreach (BlockAdjacency adj in sideBlocks)
        {
            if (!adj.AreAllAir()) count += 1;
        }
        ambientOcclusionCounts[vertexIndex] = count;
        return count;
    }
    
    public void RecomputeAmbientOcclusions()
    {
        ambientOcclusionCounts = new int[8] { -1, -1, -1, -1, -1, -1, -1,-1 };
    }

    public Vector3 GetVertex(int index, bool memo = true) {
        if (memo && vertexPositions[index] != null) {
            return (Vector3) vertexPositions[index] !;
        }
        int dx = (1 + TerrainGenerationConstants.vertexOptions[index].x)/2;
        int dy = (1 + TerrainGenerationConstants.vertexOptions[index].y)/2;
        int dz = (1 + TerrainGenerationConstants.vertexOptions[index].z)/2;
        
        Vector3Int inChunkIndex = coords.GetBlockCoords();
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

    public BlockSide GetSide(int index, bool justOutline = false) {
        int option=index*4;
        int vertex1 = TerrainGenerationConstants.sideOptions[option];
        int vertex2 = TerrainGenerationConstants.sideOptions[option+1];
        int vertex3 = TerrainGenerationConstants.sideOptions[option+2];
        int vertex4 = TerrainGenerationConstants.sideOptions[option+3];
        int[] ambientOcclusionCount = new int[4] {0,0,0,0};
        if (!justOutline)
        {
            ambientOcclusionCount = new int[4]
            {
                VertexAmbientOcclusionCount(vertex1),
                VertexAmbientOcclusionCount(vertex2),
                VertexAmbientOcclusionCount(vertex3),
                VertexAmbientOcclusionCount(vertex4)
            };
        }

        if (!justOutline && sides[index] != null)
        {
            sides[index].ResetAmbientOcclusionCounts(ambientOcclusionCount);
            return sides[index];
        }

        string faceDrawn = TerrainGenerationConstants.sideNameList[option/4];
        Vector3[] vertices = new Vector3[] {
            GetVertex(vertex1, !justOutline),    
            GetVertex(vertex2, !justOutline),    
            GetVertex(vertex3, !justOutline),
            GetVertex(vertex4, !justOutline)    
        };

        string sideName = TerrainGenerationConstants.sideNameList[chunk.GetSideCoord()];
        BlockSide blockSide = new BlockSide(
            vertices, 
            type.GetAtlasCoord(faceDrawn), 
            sideName, 
            faceDrawn,
            ambientOcclusionCount);
        if (!justOutline) {
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
        return coords.GetBlockCoords();
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
            BlockSide side = GetSide(sideIndex, true);
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