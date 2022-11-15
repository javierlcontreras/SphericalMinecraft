using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlanetMeshGenerator {
    private int meshSize;
    private float meshLength;
    private float planetRadius;

    private const int numSides = 6;
    private Vector3[] sideNormalList = new Vector3[]{
        new Vector3(1,0,0),
        new Vector3(-1,0,0),
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1)
    };
    private Vector3[] sideTangentList = new Vector3[]{
        new Vector3(0,1,0),
        new Vector3(0,-1,0),
        new Vector3(0,0,1),
        new Vector3(0,0,-1),
        new Vector3(1,0,0),
        new Vector3(-1,0,0)
    };

    public PlanetMeshGenerator(int _chunkSize, int _chunksPerSide, float _planetRadius) {
        meshSize = _chunksPerSide * _chunkSize;
        planetRadius = _planetRadius;
        meshLength = planetRadius*2.0f/(meshSize-1);
    }

    public Mesh GenerateChunk(int sideCoord, int xCoord, int yCoord) {
        return null;
    }

    public Mesh GenerateFullMesh() {
        Mesh mesh = new Mesh();
        int numVertices = meshSize*meshSize*numSides;
        int numQuads = (meshSize-1)*(meshSize-1)*numSides;
        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[6*numQuads];

        for (int side = 0; side < 6; side++) {
            Vector3 sideNormal = sideNormalList[side];
            Vector3 sideXaxis = sideTangentList[side];
            Vector3 sideYaxis = Vector3.Cross(sideNormal, sideXaxis);

            for (int i=0; i<meshSize; i++) {
                float x = (meshSize-1)/2.0f - i;
                for (int j=0; j<meshSize; j++) {
                    float y = (meshSize-1)/2.0f - j;

                    Vector3 vertexPosition = sideNormal*planetRadius + (sideXaxis*x + sideYaxis*y)*meshLength;
                    Vector3 normalizedVertexPosition = Vector3.Normalize(vertexPosition);

                    float randomNumber = NoiseGeneration.Smooth3DNoise(2*normalizedVertexPosition);
                    //Debug.Log(randomNumber);
                    float vertexRadius = planetRadius + randomNumber/2;
                    vertexPosition = normalizedVertexPosition*vertexRadius;
                    
                    //spawnDebugBall(vertexPosition);

                    int currentVertex   = vertexFromSphereCoordinates(side, i, j);        
                    vertices[currentVertex] = vertexPosition;
                    normals[currentVertex] = normalizedVertexPosition;
                    //uvs[currentVertex] = NoiseGeneration.Smooth3DNoise()

                    if (i < meshSize - 1 && j < meshSize - 1) {
                        int currentQuad = quadFromCoordinates(side, i, j);
        
                        triangles[6*currentQuad]   = vertexFromSphereCoordinates(side, i, j);
                        triangles[6*currentQuad+1] = vertexFromSphereCoordinates(side, i+1, j+1);
                        triangles[6*currentQuad+2] = vertexFromSphereCoordinates(side, i, j+1);

                        triangles[6*currentQuad+3] = vertexFromSphereCoordinates(side, i, j);
                        triangles[6*currentQuad+4] = vertexFromSphereCoordinates(side, i+1, j);
                        triangles[6*currentQuad+5] = vertexFromSphereCoordinates(side, i+1, j+1);
                    }
                }
            }
        }
        mesh.vertices = vertices;        
        mesh.triangles = triangles;        
        mesh.normals = normals;   
        return mesh;     
    }

    private void spawnDebugBall(Vector3 vertexPosition) {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.transform.position = vertexPosition;
        ball.transform.localScale *= 0.2f;           
    }
    private int vertexFromSphereCoordinates(int side, int x, int y) {
        return side*meshSize*meshSize + x*meshSize + y;
    }
    private int quadFromCoordinates(int side, int x, int y) {
        return side*(meshSize-1)*(meshSize-1) + x*(meshSize - 1) + y;
    }
} 