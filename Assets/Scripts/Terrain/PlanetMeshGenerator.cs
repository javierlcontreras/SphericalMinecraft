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
    //private float[] blockHeightToHeight;
    private float blockSize;

    private Vector3[] sideYaxisList;
    private Vector3[] sideXaxisList;
    private Vector3[] sideZaxisList;
    private string[] sideNameList;

    public PlanetMeshGenerator(Planet _planet, float _planetRadius) {
        planet = _planet;
        planetRadius = _planetRadius;
        
        sideYaxisList = TerrainManager.instance.sideYaxisList;
        sideXaxisList = TerrainManager.instance.sideXaxisList;
        sideZaxisList = TerrainManager.instance.sideZaxisList;
        sideNameList = TerrainManager.instance.sideNameList;
        
        chunkSize = planet.GetChunkSize();
        chunksPerSide = planet.GetChunksPerSide();
        chunkHeight = planet.GetChunkHeight();

        blockSize = TerrainManager.instance.GetBlockSize();
        /*blockHeightToHeight = new float[chunkHeight+1];
        float lastRadius = planetRadius;
        for (int h=0; h<=chunkHeight; h++) {
            blockHeightToHeight[h] = lastRadius;
            lastRadius += blockSizeAtPreviousHeight;
        }*/
        
        chunkAdjCalculator = new ChunkAdjacencyCalculator(planet, sideXaxisList, sideYaxisList, sideZaxisList, sideNameList);
        //spawnCoreBall();
    }

    private List<BlockSide> GenerateListOfQuads(Chunk chunk) {
        int sideCoord = chunk.sideCoord; 
        int xCoord = chunk.xCoord;
        int zCoord = chunk.zCoord;
        Vector3 sideNormal = sideYaxisList[sideCoord];
        Vector3 sideXaxis = sideXaxisList[sideCoord];
        Vector3 sideZaxis = sideZaxisList[sideCoord];

        //Debug.Log(sideNormal + " " + sideXaxis + " " + sideZaxis);

        Quaternion chunkToGlobal = Quaternion.LookRotation(sideZaxis, sideNormal); 
        Quaternion globalToChunk = Quaternion.Inverse(chunkToGlobal); 

        List<BlockSide> quads = new List<BlockSide>();

        for (int i=0; i<chunkSize; i++) {
            for (int j=0; j<chunkSize; j++) {
                for (int h=0; h<chunkHeight; h++) {
                    Block block  = chunk.blocks[i,h,j];
                    if (block.type.GetName() == "air") continue;
                    for (int nextTo=0; nextTo < 6; nextTo++) {
                        Vector3 pointingTo = sideYaxisList[nextTo];
                        Vector3 orientedToX = sideXaxisList[nextTo];
                        Vector3 orientedToZ = sideZaxisList[nextTo];

                        //int oppositeNextTo = 2*(nextTo/2) + (nextTo+1)%2;
                        string faceDrawn = sideNameList[nextTo];
                        
                        Vector3 pos = new Vector3(i, h, j);
                        Vector3 nextPos = pos + pointingTo;
                        Vector3 pointingToGlobal = chunkToGlobal*pointingTo;

                        Block nextBlock = chunkAdjCalculator.BlockNextToMe(chunk, i, j, h, pointingTo, pointingToGlobal);
                        if (nextBlock.type.GetName() != "air") continue;
                        
                        // in chunk coordinate system
                        Vector3 normal = pointingTo;
                        Vector3 A = orientedToX;
                        Vector3 B = orientedToZ;
                        Vector3 mid = (pos + nextPos)/2f;
                        // to global
                        Vector3[] vertices = new Vector3[4] {
                            blockIndexToPointInChunkCoords(mid - A/2f - B/2f, xCoord, zCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid - A/2f + B/2f, xCoord, zCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid + A/2f + B/2f, xCoord, zCoord, sideCoord),
                            blockIndexToPointInChunkCoords(mid + A/2f - B/2f, xCoord, zCoord, sideCoord)
                        };
                        Vector3 AGlobal = chunkToGlobal * A;
                        Vector3 BGlobal = chunkToGlobal * B;
                        Vector3 normalGlobal = chunkToGlobal * normal;

                        BlockSide side = new BlockSide(vertices, block.type.GetAtlasCoord(faceDrawn), sideNameList[sideCoord], faceDrawn);
                        quads.Add(side);
                    }
                }
            }    
        }

        return quads;
    }

    private Vector3 blockIndexToPointInChunkCoords(Vector3 pos, int chunkI, int chunkJ, int sideCoord) {
        int i = (int)(pos.x + 0.5);
        int h = (int)(pos.y + 0.5);
        int j = (int)(pos.z + 0.5);

        return TerrainManager.instance.BaseVector(sideCoord, chunkI, chunkJ, i, j) * (planetRadius + blockSize*(h-1));//(blockHeightToHeight[h-1]);
    }

    private void spawnDebugBall(Vector3 vertexPosition, float size) {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = vertexPosition;
        ball.transform.localScale *= size;           
    }

    private void spawnCoreBall() {
        //Debug.Log(planetRadius + " " + blockLength);
        spawnDebugBall(Vector3.zero, 2f*planetRadius);
    }

    public Mesh GenerateChunkMesh(int sideCoord, int xCoord, int yCoord) {
        Chunk chunk = planet.chunks[sideCoord, xCoord, yCoord];
        
        List<BlockSide> quads = GenerateListOfQuads(chunk);
        
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
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }
} 