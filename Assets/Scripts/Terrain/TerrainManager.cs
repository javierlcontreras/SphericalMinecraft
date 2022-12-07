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


    public Transform currentPosition;
    public float radiusOfLoad;
    public Planet planet;
    public Planet moon;
    
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

    public bool ChunkCloseEnoughToLoad(Vector3 chunkPosition) {
        return (chunkPosition - currentPosition.position).magnitude < radiusOfLoad;
    }

    private void Start() {
        planet = new Planet("Earth", Vector3.zero, 4, 16, 64);
        planet.GeneratePlanet();
        moon = new Planet("Moon", Vector3.up * 150, 1, 16, 32);
        moon.GeneratePlanet();
    }

    private void Update() {
        planet.UpdatePlanet();
        moon.UpdatePlanet();
    }

    public void DestroyChunk(GameObject mesh) {
        Destroy(mesh);
    }
}
