using UnityEngine;

public class SaveSystemManager : MonoBehaviour {
    public static string DATA_FOLDER_NAME = "GameData";
    private string worldName;
    
    public void NewGame() {
    }

    public void LoadGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerDataPersitence playerDataPersitence = new PlayerDataPersitence(worldName);
        foreach (GameObject player in players) {
            GameObject empty = playerDataPersitence.LoadPlayer(player.name);
            // TODO Make the prefab instantiation here
            player.transform.position = empty.transform.position;
            player.transform.rotation = empty.transform.rotation;
            Destroy(empty);
        }

        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        PlanetDataPersitence planetDataPersitence = new PlanetDataPersitence(worldName);
        foreach (GameObject planet in planets) {
            GameObject empty = planetDataPersitence.LoadPlanet(planet.name);
            // TODO Make the prefab instantiation here
            planet.transform.position = empty.transform.position;
            planet.transform.rotation = empty.transform.rotation;
            planet.GetComponent<CelestialBody>().SetVelocity(empty.GetComponent<CelestialBody>().GetVelocity());
            Destroy(empty);
        }
    }
    
    public void SaveGame() {
        PlayerDataPersitence playerDataPersitence = new PlayerDataPersitence(worldName);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
        foreach (GameObject player in players) {
            playerDataPersitence.SavePlayer(player);
        } 
        PlanetDataPersitence planetDataPersistence = new PlanetDataPersitence(worldName); 
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet"); 
        foreach (GameObject planet in planets) {
            planetDataPersistence.SavePlanet(planet);
        } 
    }

    private void DEBUG_START() {
        Debug.LogWarning("DEBUGGING TRICK. Must delete for prod. You are overwritting worldName and seed in PlayerPrefs for DEBUG.");
        PlayerPrefs.SetInt("newWorld", 0);
        PlayerPrefs.SetString("worldName", "New World!");
        PlayerPrefs.SetString("seed", "Javier");   
    }


    public void Awake() {
        //DEBUG_START();
        worldName = PlayerPrefs.GetString("worldName");
        bool newWorld = (PlayerPrefs.GetInt("newWorld") == 1);
        if (newWorld) {
            string seed = PlayerPrefs.GetString("seed");
            Random.InitState(seed.GetHashCode());
            NewGame();
        }
        else {
            LoadGame();
        }
    }

}