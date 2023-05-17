using System.IO;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(PlayerDataPersistence)), RequireComponent(typeof(PlanetDataPersistence))]
public class SaveSystemManager : MonoBehaviour {
    private static string DATA_FOLDER_NAME = "GameData";
    public static string DATA_FOLDER_PATH = Path.Combine(Application.dataPath, SaveSystemManager.DATA_FOLDER_NAME);
    
    public static DirectoryInfo GetWorldFolderDirectory()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(DATA_FOLDER_PATH);
        if (!directoryInfo.Exists)
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }
    public static bool IsFolderAValidWorld(string folderName)
    {
        // TODO: unimplemented
        return true;
    }
    
    private string worldName;
    private string userId;
    private PlayerDataPersistence playerDataPersistence;
    private PlanetDataPersistence planetDataPersistence;

    public void NewGame() {
        //planetDataPersistence.NewPlanet(worldName, "Earth");
        //planetDataPersistence.NewPlanet(worldName, "Moon");
        //playerDataPersistence.NewPlayer(worldName, userId);
    }

    public void LoadGame() {
        //planetDataPersistence.LoadPlanet(worldName, "Earth");
        // planetDataPersistence.LoadPlanet(worldName, "Moon");
        // playerDataPersistence.LoadPlayer(worldName, userId);
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
        PlayerPrefs.SetString("mode", "host");
        PlayerPrefs.SetString("seed", "Javier");
    }



    public void Start() {
        planetDataPersistence = gameObject.GetComponent<PlanetDataPersistence>();
        playerDataPersistence = gameObject.GetComponent<PlayerDataPersistence>();
        
        if (Application.isEditor) 
            DEBUG_START();
        
        string mode = PlayerPrefs.GetString("mode");
        if (mode.Equals("host"))
        {
            worldName = PlayerPrefs.GetString("worldName");
            userId = PlayerPrefs.GetString("userId");
            bool newWorld = (PlayerPrefs.GetInt("newWorld") == 1);
            if (newWorld) {
                string seed = PlayerPrefs.GetString("seed");
                Random.InitState(seed.GetHashCode());
                NewGame();
            }
            else {
                LoadGame();
            }

            Debug.Log("Started world as host");
            NetworkManager.Singleton.StartHost();
        }
        else if (mode.Equals("client")) {
            NetworkManager.Singleton.StartClient();
        }
    }
}