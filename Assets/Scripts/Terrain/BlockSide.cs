using UnityEngine;

public class BlockSide {
    Vector3[] vertices;
    //Vector3 uv;
    

    public BlockSide(Vector3[] _vertices, bool spin) {
        vertices = _vertices;
        
        // not used right now
        if (spin) {
            Vector3 tmp0 = vertices[0];
            Vector3 tmp1 = vertices[1];
            vertices[0] = vertices[3];
            vertices[1] = vertices[2];
            vertices[2] = tmp1;
            vertices[3] = tmp0;
        }
    }

    public BlockSideTriangle[] GetTriangles() {
        BlockSideTriangle[] triang = new BlockSideTriangle[2];
        triang[0] = new BlockSideTriangle(vertices[0], vertices[1], vertices[2]);
        triang[1] = new BlockSideTriangle(vertices[0], vertices[2], vertices[3]);
        return triang;
    }
}