using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[0];

        GameObject world = new GameObject("World", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh.

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
