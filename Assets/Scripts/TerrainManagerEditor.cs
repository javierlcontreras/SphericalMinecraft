using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
     
        if (GUILayout.Button("Generate Planet!"))
        {
            ButtonGeneratePlanet();
        }
    }
    
    void ButtonGeneratePlanet()
    {
        PlanetGenerator planetGenerator = new PlanetGenerator(meshSize, planetRadius);
        Mesh mesh = planetGenerator.Generate();

        GameObject world = new GameObject("World", typeof(MeshFilter), typeof(MeshRenderer));
        world.GetComponent<MeshFilter>().mesh = mesh;
        world.GetComponent<MeshRenderer>().material = surfaceMaterial;
    }
}
