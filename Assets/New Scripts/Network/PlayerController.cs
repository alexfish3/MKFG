using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Camera _mainCamera;
    private Vector3 mouseInput = Vector3.zero;
    private bool _canCollide = true;
    private PlayerLength _playerLength;

    public static event Action GameOverEvent;

    private readonly ulong[] _TargetClientsArray = new ulong[1];

    private void Initalize()
    {
        _mainCamera = Camera.main;
        _playerLength = GetComponent<PlayerLength>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Initalize();
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        MovePlayerServer();
    }

    private void MovePlayerServer()
    {
        // Movement
        mouseInput.x = Input.mousePosition.x;
        mouseInput.y = Input.mousePosition.y;
        mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(mouseInput);
        mouseWorldCoordinates.z = 0;
        MovePlayerServerRpc(mouseWorldCoordinates);
    }

    [ServerRpc]
    private void MovePlayerServerRpc(Vector3 mouseWorldCoordinates)
    {
        // To move position, use server time, fixed delta time to handle movement. this is to move the player based on the fixed delta time of the server
        // If we are using any time.deltaTime on the server, make sure we use this instead
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, 
            NetworkManager.Singleton.ServerTime.FixedDeltaTime * speed);

        // Rotate
        if (mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDir = mouseWorldCoordinates - transform.position;
            targetDir.z = 0;
            transform.up = targetDir;
        }
    }

    // Client Authoritative Movement
    private void MovePlayerClient()
    {
        // Movement
        mouseInput.x = Input.mousePosition.x;
        mouseInput.y = Input.mousePosition.y;
        mouseInput.z = _mainCamera.nearClipPlane;
        Vector3 mouseWorldCoordinates = _mainCamera.ScreenToWorldPoint(mouseInput);

        mouseWorldCoordinates.z = 0;
        transform.position = Vector3.MoveTowards(transform.position, mouseWorldCoordinates, Time.deltaTime * speed);

        // Rotate
        if (mouseWorldCoordinates != transform.position)
        {
            Vector3 targetDir = mouseWorldCoordinates - transform.position;
            targetDir.z = 0;
            transform.up = targetDir;
        }
    }

    [ServerRpc]
    private void DetermineCollisionWinnerServerRpc(PlayerData player1, PlayerData player2)
    {
        if(player1.lengthOfPlayer > player2.lengthOfPlayer)
        {
            WinInformation(player1.id, player2.id);
        }
        else
        {
            WinInformation(player2.id, player1.id);
        }
    }

    [ServerRpc]
    private void WinInformationServerRpc(ulong winner, ulong loser)
    {
        WinInformation(winner, loser);
    }

    private void WinInformation(ulong winner, ulong loser)
    {
        // sends client rpc to winner
        _TargetClientsArray[0] = winner;
        ClientRpcParams clientRpcParams = new ClientRpcParams()
        {
            Send = new ClientRpcSendParams()
            {
                TargetClientIds = _TargetClientsArray
            }
        };

        AtePlayerClientRpc(clientRpcParams);

        // Sends client rpc to loser
        _TargetClientsArray[0] = loser;
        clientRpcParams.Send.TargetClientIds = _TargetClientsArray;
        GameOverClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void AtePlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("You ate a player");
    }

    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;
        Debug.Log("Game Over");
        GameOverEvent?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }

    [ContextMenu("Test Game Over")]
    private void GameOverTest()
    {
        Debug.Log("Game Over");
        GameOverEvent?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }

    private IEnumerator CollisionCheckCoroutine()
    {
        _canCollide = false;
        yield return new WaitForSeconds(0.5f);
        _canCollide = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player Collision");

        if (!collision.gameObject.CompareTag("Player")) return;
        if(!IsOwner) return;
        if (!_canCollide) return;
        StartCoroutine(CollisionCheckCoroutine());

        // Head on Collision
        if(collision.gameObject.TryGetComponent(out PlayerLength playerLength))
        {
            Debug.Log("Head Collision");
            var player1 = new PlayerData()
            {
                id = OwnerClientId,
                lengthOfPlayer = _playerLength.length.Value
            };

            var player2 = new PlayerData()
            {
                id = playerLength.OwnerClientId,
                lengthOfPlayer = playerLength.length.Value
            };

            DetermineCollisionWinnerServerRpc(player1, player2);
        }
        else if(collision.gameObject.TryGetComponent(out Tail tail))
        {
            Debug.Log("Tail Collision");
            WinInformationServerRpc(tail.networkedOwner.GetComponent<PlayerController>().OwnerClientId, OwnerClientId);
        }

    }

    struct PlayerData : INetworkSerializable
    {
        public ulong id;
        public ushort lengthOfPlayer;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref lengthOfPlayer);
        }
    }
}
