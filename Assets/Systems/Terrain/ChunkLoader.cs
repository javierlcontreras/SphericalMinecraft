using UnityEngine;

public class ChunkLoader : MonoBehaviour {
    public float radiusOfLoad;
    public float radiusOfSave;
    public bool dontRenderChunks = false;
    private Transform currentPosition;
    
    // TODO: temporary, it should not treat any planet specially. Should look at all the tagged GameObjects Planet and treat them equally,
    // choose main planet by proximity and choose other planets to draw also by proximity.
    public PlanetTerrain[] planet;

    private void Awake() {
        currentPosition = GameObject.Find("Player").transform;
    }

    public PlanetTerrain GetCurrentPlanet() {
        return planet[0];
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

    public void UpdatePlanetMesh(PlanetTerrain planet) {
        if (dontRenderChunks) return;
        for (int side=0; side<6; side++) {
            for (int chunkX=0; chunkX < planet.GetChunksPerSide(); chunkX++) {
                for (int chunkZ=0; chunkZ < planet.GetChunksPerSide(); chunkZ++) {
                    Vector3 chunkPosition = planet.BaseVectorAtCenter(side, chunkX, chunkZ);
                    float chunkPlayerDist = ChunkPlayerDistance(chunkPosition, planet.GetPlanetPosition());
                    if (chunkPlayerDist < radiusOfLoad) {
                        planet.GetPlanetChunkLoader().GenerateChunkMesh(side, chunkX, chunkZ);
                    } 
                    else if (chunkPlayerDist < radiusOfSave) {
                        planet.GetPlanetChunkLoader().HideChunkMesh(side, chunkX, chunkZ);
                    }
                    else {
                        planet.GetPlanetChunkLoader().DestroyChunkMesh(side, chunkX, chunkZ);
                    }
                }
            }
        }
    }

    public void RegenerateChunkMesh(PlanetTerrain planet, int side, int chunkX, int chunkZ) {
        planet.GetPlanetChunkLoader().RegenerateChunkMesh(side, chunkX, chunkZ);
    }

    private void Update() {
        UpdatePlanetMesh(planet[0]);
    }
}