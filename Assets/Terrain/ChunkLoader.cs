using UnityEngine;

public class ChunkLoader : MonoBehaviour {
    public bool dontRenderChunks = false;
    private Transform currentPosition;
    
    // TODO: temporary, it should not treat any planet specially. Should look at all the tagged GameObjects Planet and treat them equally,
    // choose main planet by proximity and choose other planets to draw also by proximity.
    private PlanetTerrain earth;
    private PlanetTerrain moon;
    private void Awake() {
        currentPosition = GameObject.Find("Player").transform;
        earth = GameObject.Find("Earth").GetComponent<PlanetTerrain>();
        moon = GameObject.Find("Moon").GetComponent<PlanetTerrain>();
    }
    public PlanetTerrain GetCurrentPlanet() {
        return earth;
    }

    public float radiusOfLoad;
    public float GetRadiusOfLoad() {
        return radiusOfLoad;
    }

    public bool ChunkCloseEnoughToLoad(Vector3 chunkPosition, Vector3 planetPosition) {
        Vector3 radialPosition = (currentPosition.position - planetPosition).normalized;
        return (chunkPosition.normalized - radialPosition).magnitude < GetRadiusOfLoad();
    }



    public void UpdatePlanetMesh(PlanetTerrain planet) {
        if (dontRenderChunks) return;
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < planet.GetChunksPerSide(); chunkX++) {
                for (int chunkZ=0; chunkZ < planet.GetChunksPerSide(); chunkZ++) {
                    Vector3 chunkPosition = planet.BaseVectorAtCenter(side, chunkX, chunkZ);
                    if (ChunkCloseEnoughToLoad(chunkPosition, planet.GetPlanetPosition())) {
                        planet.GetPlanetChunkLoader().GenerateChunkMesh(side, chunkX, chunkZ);
                    } 
                    else {
                        planet.GetPlanetChunkLoader().HideChunkMesh(side, chunkX, chunkZ);
                    }
                }
            }
        }
    }

    public void RegenerateChunkMesh(PlanetTerrain planet, int side, int chunkX, int chunkZ) {
        planet.GetPlanetChunkLoader().RegenerateChunkMesh(side, chunkX, chunkZ);
    }

    private void Update() {
        UpdatePlanetMesh(earth);
        UpdatePlanetMesh(moon);
    }
}