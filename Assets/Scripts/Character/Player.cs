using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{

    [SerializeField] private FirstPersonController _firstPersonController;
    [SerializeField] private ChunkLoader _chunkLoader;
    [SerializeField] private ControllerSettings _controllerSettings;
    [SerializeField] private PlayerOnSpawnLoadFromSave _playerOnSpawnLoadFromSave;
    [SerializeField] private EscScreen _escScreen;
    [SerializeField] private PointingTo _pointingTo;
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("Player Spawned!");
        //gameObject.transform.parent = GameObject.Find("Players").transform;
        
        string playerName = PlayerPrefs.GetString("playerName");
        _playerOnSpawnLoadFromSave.InformServerOfNameServerRpc(playerName);
        
        _chunkLoader.Init();
        _controllerSettings.Init(_chunkLoader);
        _firstPersonController.Init(_controllerSettings);
        _pointingTo.Init(_chunkLoader, _controllerSettings);
    }

    public void FixedUpdate()
    {
        if (!IsOwner) return;
        _firstPersonController.MyFixedUpdate();
        _pointingTo.MyFixedUpdate();
    }

    public void Update()
    {
        if (!IsOwner) return;
        _chunkLoader.MyUpdate();
        _escScreen.MyUpdate();
    }
}
