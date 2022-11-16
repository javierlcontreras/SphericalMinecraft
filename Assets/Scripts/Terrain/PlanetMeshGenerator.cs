using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetMeshGenerator {
    private Planet planet;
    private ChunkAdjacencyCalculator chunkAdjCalculator;
    private float planetRadius;

    private int chunksPerSide;
    private int chunkSize;
    private int chunkHeight;
    private float blockLength;

    private Vector3[] sideNormalList;
    private Vector3[] sideTangentList;
    private Vector3[,,] baseVectors;

    public PlanetMeshGenerator(Planet _planet, float _planetRadius) {
        planet = _planet;
        planetRadius = _planetRadius;
        
        sideNormalList = TerrainManager.instance.sideNormalList;
        sideTangentList = TerrainManager.instance.sideTangentList;
        baseVectors = TerrainManager.instance.baseVectors;
        
        chunkSize = planet.GetChunkSize();
        chunksPerSide = planet.GetChunksPerSide();
        chunkHeight = planet.GetChunkHeight();
        blockLength = planetRadius*2.0f/chunksPerSide/chunkSize;

        chunkAdjCalculator = new ChunkAdjacencyCalculator(planet, sideNormalList, sideTangentList);
    }

    private List<BlockSide> GenerateListOfQuads(int sideCoord, int xCoord, int yCoord, Chunk chunk) {
        Vector3 sideNormal = sideNormalList[sideCoord];
        Vector3 sideXaxis = sideTangentList[sideCoord];

        Quaternion chunkToGlobal = Quaternion.LookRotation(sideXaxis, sideNormal); 
        Quaternion globalToChunk = Quaternion.Inverse(chunkToGlobal); 

        List<BlockSide> quads = new List<BlockSide>();

        for (int i=0; i<chunkSize; i++) {
            for (int j=0; j<chunkSize; j++) {
                for (int h=0; h<chunkHeight; h++) {
                    if (chunk.blocks[i,j,h].type.GetName() != "air") continue;
                    for (int nextTo=0; nextTo < 6; nextTo++) {
                        Vector3 pointingTo = sideNormalList[nextTo];
                        Vector3 orientedTo = sideTangentList[nextTo];
                        
                        Vector3 pos = new Vector3(i, h, j);
                        Vector3 nextPos = pos + pointingTo;
                        Vector3 pointingToGlobal = chunkToGlobal*pointingTo;

                        Block nextBlock = chunkAdjCalculator.BlockNextToMe(sideCoord, xCoord, yCoord, i, j, h, pointingTo, pointingToGlobal);
                        if (nextBlock.type.GetName() == "air") continue;
                        
                        // in chunk coordinate system
                        Vector3 normal = -pointingTo;
                        Vector3 A = orientedTo;
                        Vector3 B = Vector3.Cross(normal, A);
                        Vector3 mid = (pos + nextPos)/2f;
                        // to global
                        Vector3[] vertices = new Vector3[4] {
                            blockIndexToPointInChunkCoords(mid - A/2f - B/2f, xCoord, yCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid - A/2f + B/2f, xCoord, yCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid + A/2f + B/2f, xCoord, yCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid + A/2f - B/2f, xCoord, yCoord, sideCoord)
                        };
                        Vector3 AGlobal = chunkToGlobal * A;
                        Vector3 BGlobal = chunkToGlobal * B;
                        Vector3 normalGlobal = chunkToGlobal * normal;

                        BlockSide side = new BlockSide(vertices, false);
                        quads.Add(side);
                    }
                }
            }    
        }

        return quads;
    }

    private Vector3 blockIndexToPointInChunkCoords(Vector3 pos, int chunkI, int chunkJ, int sideCoord) {
        int i = (int)(pos.x + 0.5);
        int j = (int)(pos.z + 0.5);
        int h = (int)(pos.y + 0.5);

        return baseVectors[sideCoord, chunkI*chunkSize + i, chunkJ*chunkSize + j] * (planetRadius + (h-1)*blockLength);
    }

    private void spawnDebugBall(Vector3 vertexPosition, float size) {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = vertexPosition;
        ball.transform.localScale *= size;           
    }

    public Mesh GenerateChunk(int sideCoord, int xCoord, int yCoord) {
        Chunk chunk = planet.chunks[sideCoord, xCoord, yCoord];
        
        List<BlockSide> quads = GenerateListOfQuads(sideCoord, xCoord, yCoord, chunk);
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        int vertexCount = 0;
        foreach (BlockSide quad in quads) {
            foreach (BlockSideTriangle triang in quad.GetTriangles()) {
                foreach (Vector3 vertex in triang.vertices) {
                    vertices.Add(vertex);    
                }
                foreach (Vector3 normal in triang.normals) {
                    normals.Add(normal);    
                }
                triangles.Add(vertexCount);
                triangles.Add(vertexCount+1);
                triangles.Add(vertexCount+2);
                vertexCount += 3;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();

        return mesh;
    }
} 