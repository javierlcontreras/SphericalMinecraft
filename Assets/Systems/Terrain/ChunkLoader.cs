using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLoader : MonoBehaviour {
    public float radiusOfLoad;
    public float radiusOfSave;
    public bool dontRenderChunks = false;
    private Transform currentPosition;
    
    // TODO: temporary, it should not treat any planet specially. Should look at all the tagged GameObjects Planet and treat them equally,
    // choose main planet by proximity and choose other planets to draw also by proximity.
    public PlanetTerrain[] planets;

    private void Awake() {
        currentPosition = GameObject.Find("Player").transform;
        InitChunkList(GetCurrentPlanet());
    }

    List<Vector3Int> chunkList;
    void InitChunkList(PlanetTerrain planet) {
        chunkList = new List<Vector3Int>();
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < planet.GetChunksPerSide(); chunkX++) {
                for (int chunkZ=0; chunkZ < planet.GetChunksPerSide(); chunkZ++) {
                    chunkList.Add(new Vector3Int(side, chunkX, chunkZ));
                }
            }
        }
    }

    public PlanetTerrain GetCurrentPlanet() {
        return planets[0];
    }

    public float GetRadiusOfLoad() {
        return radiusOfLoad;
    }
    public float GetRadiusOfSave() {
        return radiusOfSave;
    }

    public float ChunkPlayerDistance(Vector3 chunkPosition, Vector3 planetPosition) {
        Vector3 radialPosition = (currentPosition.position - planetPosition);
        float height = radialPosition.magnitude;
        return (chunkPosition.normalized*height - radialPosition).magnitude;
    }

    bool coroutineFinished = true;
    public IEnumerator UpdatePlanetMesh(PlanetTerrain planet) {
        if (!dontRenderChunks) {
            for (int index=0; index<chunkList.Count; index++) {
                Vector3Int vec = chunkList[index];
                int side = vec.x;
                int chunkX = vec.y;
                int chunkZ = vec.z;
                float chunkPlayerDist = ComputeChunkPlayerDist(vec, planet);
                if (chunkPlayerDist < radiusOfLoad) {
                    yield return StartCoroutine(planet.GetPlanetChunkLoader().GenerateChunkMesh(side, chunkX, chunkZ));
                } 
                else if (chunkPlayerDist < radiusOfSave) {
                    planet.GetPlanetChunkLoader().HideChunkMesh(side, chunkX, chunkZ);
                }
                else {
                    planet.GetPlanetChunkLoader().DestroyChunkMesh(side, chunkX, chunkZ);
                }
            }
        }
        coroutineFinished = true;
        yield return null;
    }
    public float ComputeChunkPlayerDist(Vector3Int vec, PlanetTerrain planet) {
        int side = vec.x;
        int chunkX = vec.y;
        int chunkZ = vec.z;
        Vector3 chunkPosition = planet.BaseVectorAtCenter(side, chunkX, chunkZ);
        return ChunkPlayerDistance(chunkPosition, planet.GetPlanetPosition());
    }

    public void RegenerateChunkMesh(PlanetTerrain planet, int side, int chunkX, int chunkZ) {
        planet.GetPlanetChunkLoader().RegenerateChunkMesh(side, chunkX, chunkZ);
    }

    private void Start() {
    }
        
    private void Update() {
        if (coroutineFinished) {
            Comp comp = new Comp(this, planets[0]);
            chunkList.Sort(comp);
            coroutineFinished = false;
            StartCoroutine(UpdatePlanetMesh(planets[0]));
        }
    }

    class Comp : IComparer<Vector3Int>
    {
        PlanetTerrain planet;
        ChunkLoader chunkLoader;
        public Comp(ChunkLoader _chunkLoader, PlanetTerrain _planet) {planet = _planet; chunkLoader = _chunkLoader;}
        public int Compare(Vector3Int x, Vector3Int y)
        {
            float a = chunkLoader.ComputeChunkPlayerDist(x, planet);
            float b = chunkLoader.ComputeChunkPlayerDist(y, planet);
            return a.CompareTo(b);
            
        }
    }
}

