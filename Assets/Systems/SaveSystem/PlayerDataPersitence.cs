using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataPersitence {
    private string worldName;
    private FileManager fileManager;
    public PlayerDataPersitence(string _worldName) {
        worldName = _worldName;
        fileManager = new FileManager(worldName, "Players");
    }

    public void NewPlayer() {

    }

    public void SavePlayer(GameObject player) {
        PlayerData playerData = new PlayerData(player);
        string playerDataJson = JsonUtility.ToJson(playerData);
        fileManager.WriteFile(playerDataJson, player.name);
    }

    public GameObject LoadPlayer(string fileName) {
        string json = fileManager.ReadFile(fileName);
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        // TODO make the prefab work here
        GameObject empty = new GameObject("empty");
        empty.transform.position = data.position;
        empty.transform.rotation = data.rotation;
        return empty;
    } 

    [System.Serializable]
    class PlayerData {
        public Vector3 position;
        public Quaternion rotation;
        public PlayerData(GameObject player) {
            position = player.transform.position;
            rotation = player.transform.rotation;
        }
    }
}