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
            // DEBUG planet shoud never be null in prod.
            if (planet.chunks != null) {
                Chunk chunk = planet.chunks[sideCoord, xCoord, zCoord];
                if (chunk == null) { // this is here to delay terrain generation until it is really necessary
                    Vector3Int chunkCoord = new Vector3Int(sideCoord, xCoord, zCoord); 
                    planet.GetPlanetDataGenerator().GenerateChunk(chunkCoord);
                    yield return null;
                    //Debug.Log("Creating chunk data!");
                }

                GenerateChunkMeshNow(sideCoord, xCoord, zCoord);
            }
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