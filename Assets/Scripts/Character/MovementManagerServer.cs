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

    // TODO: tag as [ServerRpc] and remember to add ServerRpc to name
    public void MoveMe(float dx, float dy, float dz, Transform clientTransform) // ServerRpcParams serverRpcParams = default)
    {
        clientTransform.Translate(new Vector3(dx,dy,dz));
        /*
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log("client trying to move");
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
        	NetworkClient client = NetworkManager.ConnectedClients[clientId];
        	client.PlayerObject.transform.Translate(new Vector3(dx, dy, dz));
        }*/
    }
        
}
