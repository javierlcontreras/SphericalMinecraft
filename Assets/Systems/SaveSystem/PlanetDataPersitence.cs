using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetDataPersitence {
    private string worldName;
    private FileManager fileManager;
    public PlanetDataPersitence(string _worldName) {
        worldName = _worldName;
        fileManager = new FileManager(worldName, "Planets");
    }

    public void NewPlanet() {

    }

    public void SavePlanet(GameObject planet) {
        PlanetData planetData = new PlanetData(planet);
        string planetDataJson = JsonUtility.ToJson(planetData);
        fileManager.WriteFile(planetDataJson, planet.name);
    }

    public GameObject LoadPlanet(string fileName) {
        string json = fileManager.ReadFile(fileName);
        PlanetData data = JsonUtility.FromJson<PlanetData>(json);
        // TODO make the prefab work here
        GameObject empty = new GameObject("empty", typeof(CelestialBody));
        empty.transform.position = data.position;
        empty.transform.rotation = data.rotation;
        empty.GetComponent<CelestialBody>().SetVelocity(data.velocity);
        return empty;
    } 

    [System.Serializable]
    class PlanetData {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 velocity;
        public PlanetData(GameObject planet) {
            CelestialBody planetBody = planet.GetComponent<CelestialBody>();
            position = planetBody.GetPosition();
            rotation = planetBody.GetRotation();
            velocity = planetBody.GetVelocity();
        }
    }
}