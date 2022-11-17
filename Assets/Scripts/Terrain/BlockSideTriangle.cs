using UnityEngine;

public class BlockSideTriangle {
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;

    public BlockSideTriangle(Vector3 a, Vector3 b, Vector3 c, Vector2 uva, Vector2 uvb, Vector2 uvc) {
        
        /*Vector3 side1 = b - a;
        Vector3 side2 = c - a;
        Vector3 normal = Vector3.Normalize(Vector3.Cross(side1, side2));

        if ((normal - Vector3.Normalize(a)).magnitude > 0.5) {
            Vector3 tmp = b;
            b = c;
            c = tmp;
            Debug.Log("Spin!");
            Debug.Log(Vector3.Dot(normal, Vector3.Normalize(a)));
            Debug.Log((normal - Vector3.Normalize(a)).magnitude);
        }*/

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