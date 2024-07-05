using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FoodSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject prefab;

    private WaitForSeconds _waitFor2Seconds = new WaitForSeconds(2f);
    private const int MaxPrefabCount = 50;

    bool spawning = false;
    [SerializeField] bool serverInitalized = false;

    int counter = 0;

    private void Awake()
    {
        //NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
        //serverInitalized = true;
    }

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
        serverInitalized = true;

        if (serverInitalized)
        {
            NetworkManager.Singleton.OnServerStarted += SpawnFoodStart;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
    }

    private void OnDisable()
    {
        if (!NetworkManager.Singleton) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void SpawnFoodStart()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.OnServerStarted -= SpawnFoodStart;
        NetworkObjectPool.Singleton.InitializePool();
        for(int i = 0; i < 30; i++)
        {
            SpawnFood();
        }

        StartCoroutine(SpawnOverTime());
    }

    private IEnumerator SpawnOverTime()
    {
        spawning = true;
        while (spawning && NetworkManager.Singleton.ConnectedClients.Count > 0)
        {
            yield return _waitFor2Seconds;

            if(NetworkObjectPool.Singleton.GetCurrentPrefabCount(prefab) < MaxPrefabCount)
                SpawnFood();
        }

        spawning = false;
    }

    private void SpawnFood()
    {
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(prefab, GetRandomPositionOnMap(), Quaternion.identity);
        obj.Spawn(true);

        if (!obj.IsSpawned) obj.Spawn(true);
    }

    private Vector3 GetRandomPositionOnMap()
    {
        return new Vector3(Random.Range(-9, 9), Random.Range(-5, 5), 0);
    }

    private void OnClientConnect(ulong clientId)
    {
        Debug.Log("On Client Connect" + NetworkManager.Singleton.IsServer);
        if (!NetworkManager.Singleton.IsServer) return;
        if (spawning) return;
        StartCoroutine(SpawnOverTime());
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if(spawning && NetworkManager.Singleton.ConnectedClients.Count == 0)
        {
            spawning = false;
        }
    }
}
