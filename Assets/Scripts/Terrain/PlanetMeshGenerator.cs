using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetMeshGenerator {
    private Planet planet;
    private ChunkAdjacencyCalculator chunkAdjCalculator;
    
    private int chunksPerSide;
    private int chunkSize;
    private int chunkHeight;
    
    private Vector3[] sideYaxisList;
    private Vector3[] sideXaxisList;
    private Vector3[] sideZaxisList;
    private string[] sideNameList;

    public PlanetMeshGenerator(Planet _planet) {
        planet = _planet;
        
        sideYaxisList = TerrainManager.sideYaxisList;
        sideXaxisList = TerrainManager.sideXaxisList;
        sideZaxisList = TerrainManager.sideZaxisList;
        sideNameList = TerrainManager.sideNameList;
        
        chunkSize = planet.GetChunkSize();
        chunksPerSide = planet.GetChunksPerSide();
        chunkHeight = planet.GetHeight();
        
        chunkAdjCalculator = new ChunkAdjacencyCalculator(planet);
    }

    private List<BlockSide> GenerateListOfQuads(Chunk chunk) {
        int sideCoord = chunk.GetSideCoord(); 
        int xCoord = chunk.GetXCoord();
        int zCoord = chunk.GetZCoord();
        Vector3 sideNormal = sideYaxisList[sideCoord];
        Vector3 sideXaxis = sideXaxisList[sideCoord];
        Vector3 sideZaxis = sideZaxisList[sideCoord];

        //Debug.Log(sideNormal + " " + sideXaxis + " " + sideZaxis);

        Quaternion chunkToGlobal = chunk.ChunkToGlobal();
        Quaternion globalToChunk = chunk.GlobalToChunk(); 

        List<BlockSide> quads = new List<BlockSide>();

        Debug.Log("Chunk Height " + chunkHeight);
        for (int h=0; h<chunkHeight; h++) {
            int realChunkSize = planet.NumBlocksAtHeightPerChunk(h);
            //Debug.Log("realChunkSize " + realChunkSize);
            for (int i=0; i<realChunkSize; i++) {
                for (int j=0; j<realChunkSize; j++) {

                    Block block  = chunk.blocks[i,h,j];
                    string blockTypeName = block.GetBlockType().GetName();
                    //Debug.Log("Block type check: " + blockTypeName);
                    if (blockTypeName == "air" || blockTypeName == "invalid") continue;
                    for (int nextTo=0; nextTo < 6; nextTo++) {
                        Vector3 pointingTo = sideYaxisList[nextTo];
                        Vector3 orientedToX = sideXaxisList[nextTo];
                        Vector3 orientedToZ = sideZaxisList[nextTo];
                        string faceDrawn = sideNameList[nextTo];
                        
                        Vector3 pos = new Vector3(i, h, j);
                        Vector3 nextPos = pos + pointingTo;
                        Vector3 pointingToGlobal = chunkToGlobal*pointingTo;

                        BlockAdjacency blocksAdjacent = chunkAdjCalculator.BlockNextToMe(chunk, i, h, j, pointingTo, pointingToGlobal);
                        if (blocksAdjacent != null && !blocksAdjacent.IsAnyAir()) continue;
                        
                        quads.Add(block.sides[nextTo]);
                    }
                }
            }    
        }
        return quads;
    }
/*
    private void spawnDebugBall(Vector3 vertexPosition, float size) {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = vertexPosition;
        ball.transform.localScale *= size;           
    }
*/
    public Mesh GenerateChunkMesh(int sideCoord, int xCoord, int yCoord) {
        Chunk chunk = planet.chunks[sideCoord, xCoord, yCoord];
        
        List<BlockSide> quads = GenerateListOfQuads(chunk);
        
        return MeshFromQuads(quads);
    }

    public Mesh MeshFromQuads(List<BlockSide> quads, bool meshWireMode = false) {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
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
                foreach (Vector2 uv in triang.uvs) {
                    uvs.Add(uv);    
                }
                triangles.Add(vertexCount);
                triangles.Add(vertexCount+1);
                triangles.Add(vertexCount+2);
                vertexCount += 3;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.normals = normals.ToArray();
        
        if (meshWireMode) {
            mesh.SetIndices(triangles, MeshTopology.Lines, 0);
        }
        else {
            mesh.triangles = triangles.ToArray();
        }
        return mesh;
    }
} 