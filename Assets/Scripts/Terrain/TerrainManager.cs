using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static PlanetDataGenerator;

public class TerrainManager : MonoBehaviour
{

    public Material textureMaterial;
    public Material wireframeMaterial;
    private const int textureBlockSize = 128;
    public int TextureBlockSize => textureBlockSize;
    private const int textureAtlasSize = 2048;
    public int TextureAtlasSize => textureAtlasSize;
    
    public static readonly string[] sideNameList = new string[] {
        "up",
        "down",
        "right",
        "left",
        "forward",
        "back"
    };
    public static readonly Vector3[] sideYaxisList = new Vector3[]{
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    public static readonly Vector3[] sideXaxisList = new Vector3[]{
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    public static readonly Vector3[] sideZaxisList = new Vector3[]{
        Vector3.left,
        Vector3.left,
        Vector3.back,
        Vector3.back,
        Vector3.down,
        Vector3.down
    };
    public static readonly int[] vertexOptions = new int[] {
        0,0,0, // left-bot-back
        1,0,0, // right-bot-back
        0,1,0, // left-top-back
        1,1,0, // right-top-back
        0,0,1, // left-bot-forw
        1,0,1, // right-bot-forw
        0,1,1, // left-top-forw
        1,1,1  // right-top-forw
    };
    public static readonly int[] sideOptions = new int[] {
        6,7,3,2, // top
        5,4,0,1, // bot
        7,5,1,3, // right
        4,6,2,0, // left
        7,6,4,5, // forward
        2,3,1,0 // back
    };

    public float GetCoreRadius() {
        return 0.5f*0.5f*Mathf.Sqrt(2.0f);
    }

    public Transform currentPosition;
    public float radiusOfLoad;
    private Planet planet;
    
    public Planet GetCurrentPlanet() {
        return planet;
    }

    /*public void Reload()
    {
        applicationIsQuitting = false;
    }*/

    // TODO: I think singleton is a bad idea, chango to Scriptable Object
    public static TerrainManager instance = null;
    public void Awake() { 
        if (instance != null && instance != this) { 
            Destroy(this); 
        } 
        else { 
            instance = this;
        } 
    }

    public bool ChunkCloseEnoughToLoad(Vector3 chunkPosition, Vector3 planetPosition) {
        Vector3 radialPosition = (currentPosition.position - planetPosition).normalized;
        return (chunkPosition - radialPosition).magnitude < radiusOfLoad;
    }

    private void Start() {
        
        PlanetGeneratorSettings planetSettings = new PlanetGeneratorSettings("Earth", Vector3.zero, 4, 8, 32);
        planet = new Planet(planetSettings);
        
        planet.GeneratePlanetData();
        
        //planet.chunks[0,0,0].DebugChunkDataAtHeight(3);
        Debug.Log("Planet Height: " + planet.GetHeight());


        /*for (int option=0; option < 8*3; option += 3) {
            int x = vertexOptions[option];
            int y = vertexOptions[option+1];
            int z = vertexOptions[option+2];
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale =  0.1f * Vector3.one;
            sphere.transform.position = new Vector3(x,y,z);
            sphere.name += ": " + (option/3);
        }*/
    }

    private void Update() {
        planet.UpdatePlanetMesh();
    }

    public void DestroyChunk(GameObject mesh) {
        Destroy(mesh);
    }
}
