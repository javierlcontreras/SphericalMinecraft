using UnityEngine;

[RequireComponent(typeof(PlayerDataPersistence)), RequireComponent(typeof(PlanetDataPersistence))]
public class SaveSystemManager : MonoBehaviour {
    public static string DATA_FOLDER_NAME = "GameData";
   
    private string worldName;
    private string userId;
    private PlayerDataPersistence playerDataPersistence;
    private PlanetDataPersistence planetDataPersistence;

    public void NewGame() {
        planetDataPersistence.NewPlanet(worldName, "Earth");
        planetDataPersistence.NewPlanet(worldName, "Moon");

        playerDataPersistence.NewPlayer(worldName, userId);
    }

    public void LoadGame() {
        planetDataPersistence.LoadPlanet(worldName, "Earth");
        planetDataPersistence.LoadPlanet(worldName, "Moon");

        playerDataPersistence.LoadPlayer(worldName, userId);
    }
    
    public void SaveGame() {
        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet"); 
        foreach (GameObject planet in planets) {
            planetDataPersistence.SavePlanet(worldName, planet);
        } 

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player"); 
        foreach (GameObject player in players) {
            playerDataPersistence.SavePlayer(worldName, player);
        } 
    }

    private void DEBUG_START() {
        Debug.LogWarning("DEBUGGING TRICK. Must delete for prod. You are overwritting worldName and seed in PlayerPrefs for DEBUG.");
        PlayerPrefs.SetInt("newWorld", 1);
        PlayerPrefs.SetString("userId", "javierlcontreras");
        PlayerPrefs.SetString("worldName", "lala<3javier");
        PlayerPrefs.SetString("seed", "Javier");   
    }


    public void Awake() {
        DEBUG_START();
        
        worldName = PlayerPrefs.GetString("worldName");
        userId = PlayerPrefs.GetString("userId");
        
        planetDataPersistence = gameObject.GetComponent<PlanetDataPersistence>();
        playerDataPersistence = gameObject.GetComponent<PlayerDataPersistence>();
        
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