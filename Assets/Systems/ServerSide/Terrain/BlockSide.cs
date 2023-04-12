using UnityEngine;

public class BlockSide {

    private Vector3[] vertices;
    private string side, pointing;
    private Vector2Int atlasCoord;
    
    private Vector2[] uvs;
    private Color[] colors;
    private Vector3 normal;

    public BlockSide(Vector3[] _vertices, Vector2Int _atlasCoord, string _side, string _pointing, int[] ambientOcclusionCounts)
    {
        side = _side;
        pointing = _pointing;
        int rot = NumRotations(side, pointing);
       
        vertices = Rotate(_vertices, rot);
        atlasCoord = _atlasCoord;
        uvs = TerrainGenerationConstants.ComputeUVsFromAtlasCoord(atlasCoord);
        normal = Vector3.Cross(vertices[1] - vertices[0], vertices[2] - vertices[0]).normalized;
        ResetAmbientOcclusionCounts(ambientOcclusionCounts);
    }

    public void ResetAmbientOcclusionCounts(int[] ambientOcclusionCounts)
    {
        int rot = NumRotations(side, pointing);
        Color white = new Color(1, 1, 1, 1);
        colors = new Color[4]
        {
            TerrainGenerationConstants.AmbientOcclusionPerCount(ambientOcclusionCounts[0]) * white,
            TerrainGenerationConstants.AmbientOcclusionPerCount(ambientOcclusionCounts[1]) * white,
            TerrainGenerationConstants.AmbientOcclusionPerCount(ambientOcclusionCounts[2]) * white,
            TerrainGenerationConstants.AmbientOcclusionPerCount(ambientOcclusionCounts[3]) * white,
        };
        colors = Rotate(colors, rot);
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
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2], 
                                            uvs[0], uvs[1], uvs[2], 
                                            normal, 
                                            colors[0], colors[1], colors[2]);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3], 
                                            uvs[0], uvs[2], uvs[3], 
                                            normal,
                                            colors[0], colors[2], colors[3]);
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
    private Color[] Rotate(Color[] A, int n) {
        if (n == 0) return A; 
        if (n == 1) {
            Color a0 = A[0];
            Color a1 = A[1];
            Color a2 = A[2];
            Color a3 = A[3];
            return new Color[]{a1, a2, a3, a0};
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