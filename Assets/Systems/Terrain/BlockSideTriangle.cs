using UnityEngine;

public class BlockSideTriangle {
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;
    public Color[] colors;

    public BlockSideTriangle(Vector3 a, Vector3 b, Vector3 c, 
                            Vector2 uva, Vector2 uvb, Vector2 uvc, 
                            Vector3 normal,
                            Color colora, Color colorb, Color colorc) {
        vertices = new Vector3[3] {
            a,
            b,
            c
        };
        normals = new Vector3[3] {
            normal, 
            normal, 
            normal 
        };
        uvs = new Vector2[3] {
            uva,
            uvb,
            uvc 
        };
        colors = new Color[3]
        {
            colora,
            colorb,
            colorc
        };
    }
}