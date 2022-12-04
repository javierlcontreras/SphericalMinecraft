using UnityEngine;

public class BlockSideTriangle {
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;

    public BlockSideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 uva, Vector2 uvb, Vector2 uvc) {
        vertices = new Vector3[3] {
            a,
            b,
            c
        };
        normals = new Vector3[3] {
            Vector3.Normalize(a), 
            Vector3.Normalize(b), 
            Vector3.Normalize(c) 
        };
        uvs = new Vector2[3] {
            uva,
            uvb,
            uvc 
        };
    }
}