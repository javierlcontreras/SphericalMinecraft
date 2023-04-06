using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        GenerateChunkMeshNow(sideCoord, xCoord, zCoord);
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

    public IEnumerator GenerateChunkMesh(int sideCoord, int xCoord, int zCoord) {
        GameObject oldMesh = currentChunksLoaded[sideCoord, xCoord, zCoord];
        if (oldMesh != null) {
            oldMesh.SetActive(true);
        }
        else {
            if (planet.chunks == null)
            {
                Debug.LogWarning("Planet is null at PlanetChunkLoader");
            }

            for (int dx = -1; dx <= 1; dx += 1)
            {
                for (int dz = -1; dz <= 1; dz += 1)
                {
                    if (dx * dz != 0) continue;

                    Vector3Int nextChunkCoord;
                    if (dx == 0 && dz == 0)
                    {
                        nextChunkCoord = new Vector3Int(sideCoord, xCoord, zCoord);
                    }
                    else
                    {
                        nextChunkCoord = planet.GetPlanetMeshGenerator().GetChunkAdjacencyCalculator().ChunkNextToMe(sideCoord, xCoord, zCoord, dx, dz);
                    }
                    Chunk nextChunk = planet.chunks[nextChunkCoord.x, nextChunkCoord.y, nextChunkCoord.z];
                    if (nextChunk == null) { // this is here to delay terrain generation until it is really necessary
                        planet.GetPlanetDataGenerator().GenerateChunk(nextChunkCoord);
                        yield return null;
                    }
                }
            }
            GenerateChunkMeshNow(sideCoord, xCoord, zCoord);
        }
        yield return null;
    }
    public void GenerateChunkMeshNow(int sideCoord, int xCoord, int zCoord) {
        Chunk chunk = planet.chunks[sideCoord, xCoord, zCoord];
        string chunkName = "(" + sideCoord + "," + xCoord + "," + zCoord + ")";
        Mesh mesh = planet.GetPlanetMeshGenerator().GenerateChunkMesh(sideCoord, xCoord, zCoord);
        GameObject chunkMesh = new GameObject(planet.GetPlanetName() + ": "+ chunkName, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        int TerrainLayer = LayerMask.NameToLayer("Terrain");
        chunkMesh.layer = TerrainLayer;
        chunkMesh.GetComponent<MeshFilter>().mesh = mesh;
        chunkMesh.GetComponent<MeshRenderer>().material = planet.GetSurfaceTexturesMaterial();
        chunkMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
        CelestialBody planetBody = planet.gameObject.GetComponent<CelestialBody>();
        chunkMesh.transform.position = planetBody.GetPosition();
        chunkMesh.transform.rotation = planetBody.GetRotation();
        chunkMesh.transform.SetParent(planetBody.transform);
        currentChunksLoaded[sideCoord, xCoord, zCoord] = chunkMesh;
    }
}