using Unity.Netcode;
using UnityEngine;

public class Food : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!NetworkManager.Singleton.IsServer) return;

        if(collision.TryGetComponent(out PlayerLength playerLength))
        {
            playerLength.AddLengthServer();
        }
        else if(collision.TryGetComponent(out Tail tail))
        {
            if(tail.networkedOwner.TryGetComponent(out PlayerLength playerLengthFromTail))
            {
                playerLengthFromTail.AddLengthServer();
            }
        }

        //NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);

        if (NetworkObject.IsSpawned)
        {
            NetworkObject.Despawn();
        }
    }
}
