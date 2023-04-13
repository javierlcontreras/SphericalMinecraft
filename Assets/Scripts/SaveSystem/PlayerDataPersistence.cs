using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SaveSystemManager))]
public class PlayerDataPersistence : MonoBehaviour {
    public GameObject playerPrefab;
    
    public GameObject NewPlayer(string worldName, string playerName) {
        GameObject player = Instantiate(playerPrefab);
        player.name = playerName;
        return player;
    }

    public void SavePlayer(string worldName, GameObject player) {
        FileManager fileManager = new FileManager(worldName, "Players");
        PlayerData playerData = new PlayerData(player);
        string playerDataJson = JsonUtility.ToJson(playerData);
        fileManager.WriteFile(playerDataJson, player.name);
    }

    public GameObject LoadPlayer(string worldName, string playerName) {
        FileManager fileManager = new FileManager(worldName, "Players");
        if (fileManager.DoesFileExist(playerName))
        {
            string json = fileManager.ReadFile(playerName);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            GameObject player = Instantiate(playerPrefab, data.position, data.rotation);
            player.GetComponent<FirstPersonController>().SetIsFlying(data.isFlying);
            player.GetComponent<FirstPersonController>().SetVerticalVelocity(data.verticalVelocity);
            player.name = playerName;
            return player;
        }
        else
        {
            return NewPlayer(worldName, playerName);
        }
    } 
}