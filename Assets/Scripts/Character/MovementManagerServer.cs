using System;
using UnityEngine;
using Unity.Netcode;

public class MovementManagerServer : NetworkBehaviour
{
    public static MovementManagerServer Singleton;

    private void Awake()
    {
        if (MovementManagerServer.Singleton != null)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    [ServerRpc]
    public void MoveMeServerRpc(float dx, float dy, float dz, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
        	NetworkClient client = NetworkManager.ConnectedClients[clientId];
        	client.PlayerObject.transform.Translate(new Vector3(dx, dy, dz));
        }
    }
        
}
