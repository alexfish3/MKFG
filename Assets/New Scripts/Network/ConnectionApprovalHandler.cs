using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : MonoBehaviour
{
    public static int MAX_PLAYERS = 10;

    private void Awake()
    {
        if (!NetworkManager.Singleton) return;
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        Debug.Log("Connection Approval");

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        if(NetworkManager.Singleton.ConnectedClients.Count >= MAX_PLAYERS)
        {
            response.Approved = false;
            response.Reason = "Server is Full";
        }
        response.Pending = false;
    }
}
