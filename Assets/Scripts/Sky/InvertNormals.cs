using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertNormals : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;

        Vector3[] normals = mesh.normals;
        for (int i=0; i<normals.Length; i++) normals[i] *= -1f;

        mesh.normals = normals;  

        for (int i=0; i<mesh.subMeshCount; i++) {
            int[] tris = mesh.GetTriangles(i);
            for (int j=0; j<tris.Length; j+=3) {
                int temp = tris[j+1];
                tris[j+1] = tris[j+2];
                tris[j+2] = temp;
            }
            mesh.SetTriangles(tris, i);
        } 
    }
}
