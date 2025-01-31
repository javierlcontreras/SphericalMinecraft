using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;

public class ChunkLoader : NetworkBehaviour {
    public float radiusOfLoad;
    public float radiusOfSave;
    public bool dontRenderChunks = false;
    private Transform currentPosition;
    
    private GameObject[] planets;

    public void Init() {
        currentPosition = gameObject.transform;
        
        planets = GameObject.FindGameObjectsWithTag("Planet");
        if (planets.Length == 0)
        {
            Debug.LogWarning("Player is getting loaded before the planets! This should never happen");
        }
        InitChunkList();
    }

    List<Vector4> chunkList;
    void InitChunkList() {
        chunkList = new List<Vector4>();
        for (int planetIndex = 0; planetIndex < planets.Length; planetIndex++){
            GameObject planet = planets[planetIndex];
            PlanetTerrain planetTerrain = planet.GetComponent<PlanetTerrain>();
            for (int side=0; side<6; side++) {
                for (int chunkX=0; chunkX < planetTerrain.GetChunksPerSide(); chunkX++) {
                    for (int chunkZ=0; chunkZ < planetTerrain.GetChunksPerSide(); chunkZ++) {
                        chunkList.Add(new Vector4(side, chunkX, chunkZ, planetIndex));
                    }
                }
            }
        }
    }

    public GameObject GetCurrentPlanet() {
        float minDist = float.PositiveInfinity;
        GameObject planet = null;
        for (int planetIndex=0; planetIndex<planets.Length; planetIndex++) {
            float dist = (planets[planetIndex].transform.position - transform.position).magnitude;
            if (dist < minDist) {
                minDist = dist;
                planet = planets[planetIndex];
            }
        }
        return planet;
    }

    public PlanetTerrain GetCurrentPlanetTerrain() {
        return GetCurrentPlanet().GetComponent<PlanetTerrain>();
    }

    public float GetRadiusOfLoad() {
        return radiusOfLoad;
    }
    public float GetRadiusOfSave() {
        return radiusOfSave;
    }

    bool coroutineFinished = true;
    public IEnumerator UpdatePlanetMeshes() {
        if (!dontRenderChunks) {
            for (int chunkIndex=0; chunkIndex<chunkList.Count; chunkIndex++) {
                Vector4 vec = chunkList[chunkIndex];
                int side = (int)vec.x;
                int chunkX = (int)vec.y;
                int chunkZ = (int)vec.z;
                int planetIndex = (int)vec.w;
                GameObject planet = planets[planetIndex];
                PlanetTerrain planetTerrain = planet.GetComponent<PlanetTerrain>();
                PlanetChunkLoader planetChunkLoader = planetTerrain.GetPlanetChunkLoader();
                float chunkPlayerDist = ComputeChunkPlayerDist(vec, planet);
                if (chunkPlayerDist < radiusOfLoad) {
                    yield return StartCoroutine(planetChunkLoader.GenerateChunkMesh(side, chunkX, chunkZ));
                } 
                else if (chunkPlayerDist < radiusOfSave) {
                    planetChunkLoader.HideChunkMesh(side, chunkX, chunkZ);
                }
                else {
                    planetChunkLoader.DestroyChunkMesh(side, chunkX, chunkZ);
                }
            }
        }
        coroutineFinished = true;
        yield return null;
    }
    
    public float ComputeChunkPlayerDist(Vector3 vec, GameObject planet) {
        int side = (int)vec.x;
        int chunkX = (int)vec.y;
        int chunkZ = (int)vec.z;
        PlanetTerrain planetTerrain = planet.GetComponent<PlanetTerrain>();
        Vector3 chunkPosition = planetTerrain.BaseVectorAtCenter(side, chunkX, chunkZ);
    
        Vector3 positionPlanetReference = planet.transform.InverseTransformPoint(currentPosition.position);
        float height = positionPlanetReference.magnitude;
        return (chunkPosition.normalized*height - positionPlanetReference).magnitude;
    
    }

    public void RegenerateChunkMesh(PlanetTerrain planetTerrain, int side, int chunkX, int chunkZ) {
        planetTerrain.GetPlanetChunkLoader().RegenerateChunkMesh(side, chunkX, chunkZ);
    }
    
    public void ReloadChunksAfterMineBuildChange(BlockCoordinateInformation blockChangedCoords) 
    {

        Vector3Int chunkCoords = blockChangedCoords.GetChunkCoords();
        Vector3Int blockIndex = blockChangedCoords.GetBlockCoords();
        PlanetTerrain planet = blockChangedCoords.GetPlanet();
        SphericalBlockAdjacencyCalculator sphericalBlockAdjacencyCalculator = planet.GetPlanetMeshGenerator().GetChunkAdjacencyCalculator();
        RegenerateChunkMesh(planet, chunkCoords);
        int len = planet.NumBlocksAtHeightPerChunk(blockIndex.y);
        if (blockIndex.x == 0) RegenerateChunkMesh(planet,sphericalBlockAdjacencyCalculator.ChunkNextToMe(chunkCoords,-1, 0));
        if (blockIndex.z == 0) RegenerateChunkMesh(planet,sphericalBlockAdjacencyCalculator.ChunkNextToMe(chunkCoords,0, -1));
        if (blockIndex.x == len-1) RegenerateChunkMesh(planet,sphericalBlockAdjacencyCalculator.ChunkNextToMe(chunkCoords,1, 0));
        if (blockIndex.z == len-1) RegenerateChunkMesh(planet,sphericalBlockAdjacencyCalculator.ChunkNextToMe(chunkCoords,0, 1));
    }
    void RegenerateChunkMesh(PlanetTerrain planet, Vector3Int chunkCoord)
    {
        RegenerateChunkMesh(planet,
            chunkCoord.x, chunkCoord.y, chunkCoord.z);
    }
    
    public void MyUpdate() {
        if (coroutineFinished) {
            Comp comp = new Comp(this, planets);
            chunkList.Sort(comp);
            coroutineFinished = false;
            StartCoroutine(UpdatePlanetMeshes());
        }
    }

    class Comp : IComparer<Vector4>
    {
        GameObject[] planets;
        ChunkLoader chunkLoader;
        public Comp(ChunkLoader _chunkLoader, GameObject[] _planets) {planets = _planets; chunkLoader = _chunkLoader;}
        public int Compare(Vector4 first, Vector4 second)
        {
            int planetIndex1 = (int)first.w;
            int planetIndex2 = (int)second.w;
            float a = chunkLoader.ComputeChunkPlayerDist((Vector3) first, planets[planetIndex1]);
            float b = chunkLoader.ComputeChunkPlayerDist((Vector3) second, planets[planetIndex2]);
            return a.CompareTo(b);   
        }
    }
}

