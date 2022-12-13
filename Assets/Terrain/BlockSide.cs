using UnityEngine;

public class BlockSide {
    private Vector3[] vertices;
    private Vector2[] uvs;
    private Vector3 atlasCoord;

    public BlockSide(Vector3[] _vertices, Vector2 _atlasCoord, string side, string pointing) {
        int rot = NumRotations(side, pointing);
       
        vertices = Rotate(_vertices, rot);
        atlasCoord = _atlasCoord;
        uvs = computeUVsFromAtlas(atlasCoord);
    }

    public Vector3[] GetVertices() {
        return vertices;
    }
    public void SetVertices(Vector3[] _vertices) {
        vertices = _vertices;
    }
    public Vector2 GetAtlasCoord() {
        return atlasCoord;
    }

    public BlockSideTriangle[] GetTriangles() {
        BlockSideTriangle[] triang = new BlockSideTriangle[2];
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2], uvs[0], uvs[1], uvs[2]);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3], uvs[0], uvs[2], uvs[3]);
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


    private Vector2[] computeUVsFromAtlas(Vector2 atlasCoord) {
        float s = 1f*TerrainManager.instance.TextureBlockSize/TerrainManager.instance.TextureAtlasSize;
        float x = atlasCoord.x*s;
        float y = 1 - atlasCoord.y*s;
        return new Vector2[] {
            new Vector2(x, y-s),
            new Vector2(x+s, y-s),
            new Vector2(x+s, y),
            new Vector2(x, y)
        };
    }

    private int NumRotations(string side, string pointing) {
        if (pointing == "right")           return 1;
        if (pointing == "left")            return 3;
        if (pointing == "forward")         return 2;
        if (pointing == "back")            return 2;
        return 0;
    }
}