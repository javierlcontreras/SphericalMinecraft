using System;
using UnityEngine;
using Unity.Netcode;

public class ServerMovement : NetworkBehaviour
{
    public static ServerMovement Singleton;

    private void Awake()
    {
        if (ServerMovement.Singleton != null)
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
        // TODO: Check if move is valid
        ClientHasMovedClientRpc(dx,dy,dz,clientId);
    }

    [ClientRpc]
    public void ClientHasMovedClientRpc(float dx, float dy, float dz, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
        	NetworkClient client = NetworkManager.ConnectedClients[clientId];
            client.PlayerObject.transform.Translate(new Vector3(dx, dy, dz));
        }
    }
        
}
