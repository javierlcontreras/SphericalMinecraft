using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
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

    public int meshGridNum;
    public float planetRadius;
    // Start is called before the first frame update
    void Start()
    {
        float meshGridSize = planetRadius*2.0f/(meshGridNum-1);

        Mesh mesh = new Mesh();
        int numVertices = meshGridNum*meshGridNum*numSides;
        int numQuads = (meshGridNum-1)*(meshGridNum-1)*numSides;
        Vector3[] vertices = new Vector3[numVertices];
        Vector3[] normals = new Vector3[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
        int[] triangles = new int[6*numQuads];

        for (int side = 0; side < 6; side++) {
            Vector3 sideNormal = sideNormalList[side];
            Vector3 sideXaxis = sideTangentList[side];
            Vector3 sideYaxis = Vector3.Cross(sideNormal, sideXaxis);

            for (int i=0; i<meshGridNum; i++) {
                float x = (meshGridNum-1)/2.0f - i;
                for (int j=0; j<meshGridNum; j++) {
                    float y = (meshGridNum-1)/2.0f - j;
                    Vector3 vertexPosition = sideNormal*planetRadius + (sideXaxis*x + sideYaxis*y)*meshGridSize;
                    
                    float vertexRadius = planetRadius;
                    vertexPosition = Vector3.Normalize(vertexPosition)*vertexRadius;
                    
                    GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    ball.transform.position = vertexPosition;
                    ball.transform.localScale *= 0.1f;
                    
                    int currentVertex   = vertexFromSphereCoordinates(side, i, j);        
                    vertices[currentVertex] = vertexPosition;
                    normals[currentVertex] = Vector3.Normalize(vertexPosition);

                    if (i < meshGridNum - 1 && j < meshGridNum - 1) {
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

        GameObject world = new GameObject("World", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int vertexFromSphereCoordinates(int side, int x, int y) {
        return side*meshGridNum*meshGridNum + x*meshGridNum + y;
    }
    int quadFromCoordinates(int side, int x, int y) {
        return side*(meshGridNum-1)*(meshGridNum-1) + x*(meshGridNum - 1) + y;
    }
}
