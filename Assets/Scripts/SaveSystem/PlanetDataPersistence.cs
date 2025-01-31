using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveSystemManager))]
public class PlanetDataPersistence : MonoBehaviour {
    
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject moonPrefab;

    [SerializeField] private GameObject celestialBodiesParent;
    
    public GameObject NewPlanet(string worldName, string planetName) {
        GameObject planet;
        if (planetName == "Earth") planet = Instantiate(planetPrefab, celestialBodiesParent.transform);
        else planet = Instantiate(moonPrefab, celestialBodiesParent.transform);
        planet.name = planetName;
    
        planet.GetComponent<PlanetTerrain>().InitTerrainSizes();
        return planet;
    }

    public void SavePlanet(string worldName, GameObject planet) {
        FileManager fileManager = new FileManager(worldName, "Planets");
        PlanetData planetData = new PlanetData(planet);
        string planetDataJson = JsonUtility.ToJson(planetData);
        fileManager.WriteFile(planetDataJson, planet.name);
    }

    public GameObject LoadPlanet(string worldName, string planetName) {
        FileManager fileManager = new FileManager(worldName, "Planets");

        string json = fileManager.ReadFile(planetName);
        PlanetData data = JsonUtility.FromJson<PlanetData>(json);
        GameObject planet;
        if (planetName == "Earth") planet = Instantiate(planetPrefab, data.position, data.rotation, celestialBodiesParent.transform);
        else planet = Instantiate(moonPrefab, data.position, data.rotation, celestialBodiesParent.transform);
        planet.name = planetName;
        planet.GetComponent<CelestialBody>().SetVelocity(data.velocity);
        
        PlanetTerrain planetTerrain = planet.GetComponent<PlanetTerrain>();
        planetTerrain.SetChunkSize(data.chunkSize);
        planetTerrain.SetChunksPerSide(data.chunksPerSide);
        planetTerrain.SetChunkHeight(data.chunkHeight);
        planetTerrain.SetChunkMinHeight(data.chunkMinHeight);
        data.SetChunks(planetTerrain);
        planetTerrain.TerrainSizePrecomputations();
        
        return planet;
    } 
}