using UnityEngine;

public class BlockSide {
    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector2Int atlasCoord;
    private Vector3 normal; 

    public BlockSide(Vector3[] _vertices, Vector2Int _atlasCoord, string side, string pointing) {
        int rot = NumRotations(side, pointing);
       
        vertices = Rotate(_vertices, rot);
        atlasCoord = _atlasCoord;
        uvs = TerrainGenerationConstants.ComputeUVsFromAtlasCoord(atlasCoord);
        normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;
    }

    public Vector3[] GetVertices() {
        return vertices;
    }
    public void SetVertices(Vector3[] _vertices) {
        vertices = _vertices;
    }
    public Vector2Int GetAtlasCoord() {
        return atlasCoord;
    }

    public BlockSideTriangle[] GetTriangles() {
        BlockSideTriangle[] triang = new BlockSideTriangle[2];
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2], uvs[0], uvs[1], uvs[2], normal);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3], uvs[0], uvs[2], uvs[3], normal);
        return triang;
    }
    public void DebugSide() {
        Debug.Log(vertices[0]);
        Debug.Log(vertices[1]);
        Debug.Log(vertices[2]);
        Debug.Log(vertices[3]);
    }

    private Vector3[] Rotate(Vector3[] A, int n) {
        if (n == 0) return A; 
        if (n == 1) {
            Vector3 a0 = A[0];
            Vector3 a1 = A[1];
            Vector3 a2 = A[2];
            Vector3 a3 = A[3];
            return new Vector3[]{a1, a2, a3, a0};
        }
        else return Rotate(Rotate(A, n-1), 1);
    }

    private int NumRotations(string side, string pointing) {
        if (pointing == "right")           return 3;
        if (pointing == "left")            return 1;
        if (pointing == "forward")         return 0;
        if (pointing == "back")            return 0;
        return 0;
    }
}