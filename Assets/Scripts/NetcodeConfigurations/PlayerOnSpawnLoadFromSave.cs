using UnityEngine;
using Unity.Netcode;

public class PlayerOnSpawnLoadFromSave : NetworkBehaviour
{
    
    [ServerRpc(RequireOwnership = false)]
    public void InformServerOfNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            NetworkManager.ConnectedClients[clientId].PlayerObject.name = playerName;
            InformEveryoneOfNameClientRpc(playerName, clientId);
        }
    }

    [ClientRpc]
    public void InformEveryoneOfNameClientRpc(string playerName, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        NetworkManager.ConnectedClients[clientId].PlayerObject.name = playerName;
    }
    
}
