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
    
    public string[] sideNameList = new string[] {
        "up",
        "down",
        "right",
        "left",
        "forward",
        "back"
    };
    public Vector3[] sideYaxisList = new Vector3[]{
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };
    public Vector3[] sideXaxisList = new Vector3[]{
        Vector3.forward,
        Vector3.back,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };
    public Vector3[] sideZaxisList = new Vector3[]{
        Vector3.left,
        Vector3.left,
        Vector3.back,
        Vector3.back,
        Vector3.down,
        Vector3.down
    };
    public int[] vertexOptions = new int[8*3] {
        0,0,0, // left-bot-back
        1,0,0, // right-bot-back
        0,1,0, // left-top-back
        1,1,0, // right-top-back
        0,0,1, // left-bot-forw
        1,0,1, // right-bot-forw
        0,1,1, // left-top-forw
        1,1,1  // right-top-forw
    };
    // TODO: possibly rearange order in each
    public int[] sideOptions = new int[6*4] {
        2,3,6,7, // top
        0,1,4,5, // bot

        1,3,5,7, // right
        0,2,4,6, // left
        
        4,5,6,7, // forward
        0,1,2,3, // back
    };

    public float GetCoreRadius() {
        return 0.5f*0.5f*Mathf.Sqrt(2.0f);
    }

    public Transform currentPosition;
    public float radiusOfLoad;
    public Planet planet;
    
    public Planet GetCurrentPlanet() {
        return planet;
    }

    public static TerrainManager instance = null; //{ get; private set; }
    private void Awake() { 
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
        PlanetGeneratorSettings planetSettings = new PlanetGeneratorSettings("Earth", Vector3.zero, 1, 16);
        planet = new Planet(planetSettings);
        //planet.GeneratePlanetData();
        
        for (int h=0; h<10; h++) {
            Debug.Log(h + ": " + planet.NumBlocksAtHeight(h));
        }
        Debug.Log(planet.GetHeight() + ": " + planet.NumBlocksAtHeight(planet.GetHeight()));
        Debug.Log((planet.GetHeight()+1) + ": " + planet.NumBlocksAtHeight(planet.GetHeight()+1));
    }

    private void Update() {
        //planet.UpdatePlanetMesh();
    }

    public void DestroyChunk(GameObject mesh) {
        Destroy(mesh);
    }
}
