using UnityEngine;

public class BlockSide {
    Vector3[] vertices;
    Vector2[] uvs;
    
    public BlockSide(Vector3[] _vertices, Vector2 atlasCoord, string side, string pointing) {
        int rot = NumRotations(side, pointing);
       
        vertices = Rotate(_vertices, rot);
        uvs = computeUVsFromAtlas(atlasCoord);
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
        float x = (int)atlasCoord.x*s;
        float y = 1 - (int)atlasCoord.y*s;
        return new Vector2[] {
            new Vector2(x, y-s),
            new Vector2(x+s, y-s),
            new Vector2(x+s, y),
            new Vector2(x, y)
        };
    }

    public BlockSideTriangle[] GetTriangles() {
        BlockSideTriangle[] triang = new BlockSideTriangle[2];
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2], uvs[0], uvs[1], uvs[2]);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3], uvs[0], uvs[2], uvs[3]);
        return triang;
    }

    private int NumRotations(string side, string pointing) {
        if (pointing == "right")           return 1;
        if (pointing == "left")            return 3;
        if (pointing == "forward")         return 2;
        if (pointing == "back")            return 2;
        return 0;
    }
}