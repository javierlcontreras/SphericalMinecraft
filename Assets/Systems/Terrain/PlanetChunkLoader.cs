using UnityEngine;

public class PlanetChunkLoader {
    private PlanetTerrain planet;
    private GameObject[,,] currentChunksLoaded;

    public PlanetChunkLoader(PlanetTerrain _planet) {
        planet = _planet;
        int chunksPerSide = planet.GetChunksPerSide();
        currentChunksLoaded = new GameObject[6,chunksPerSide,chunksPerSide];
    }

    public void RegenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        DestroyChunkMesh(sideCoord, xCoord, zCoord);
        GenerateChunkMesh(sideCoord, xCoord, zCoord);
    }

    public void HideChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh == null) return;
        oldMesh.SetActive(false);
    }

    public void DestroyChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh == null) return;
        planet.DestroyChunkMesh(oldMesh);
    }

    public void GenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh != null) {
            oldMesh.SetActive(true);
            return;
        }
        //Debug.Log("Creating new chunk mesh!");

        Chunk chunk = planet.chunks[sideCoord, xCoord, zCoord];
        if (chunk == null) { // this is here to delay terrain generation until it is really necessary
            planet.GetPlanetDataGenerator().GenerateChunk(sideCoord, xCoord, zCoord);
            //Debug.Log("Creating chunk data!");
        }

        string chunkName = "(" + sideCoord + "," + xCoord + "," + zCoord + ")";
        Mesh mesh = planet.GetPlanetMeshGenerator().GenerateChunkMesh(sideCoord, xCoord, zCoord);
        GameObject chunkMesh = new GameObject(planet.GetPlanetName() + ": "+ chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        chunkMesh.layer = TerrainLayer;
        chunkMesh.GetComponent<MeshFilter>().mesh = mesh;
        chunkMesh.GetComponent<MeshRenderer>().material = planet.GetSurfaceTexturesMaterial();
        chunkMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
        chunkMesh.transform.position = planet.GetPlanetPosition();
        chunkMesh.transform.SetParent(planet.transform);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = chunkMesh;
    }
}