using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;

public class ServerStartup : SingletonMonobehaviour<ServerStartup>
{
    public static event System.Action ClientInstance;
    private const string InternalServerIP = "0.0.0.0";
    private string _externalServerIP = "0.0.0.0";
    private ushort _serverPort = 7777;

    private string _externalConnectionString => $"{_externalServerIP}:{_serverPort}";

    private IMultiplayService _multiplayService;
    const int _multiplayServiceTimeout = 20000; // this is 20 seconds stored as miliseconds

    InitializationOptions _initializationOptions;

    private string _allocationID;
    private MultiplayEventCallbacks _serverCallbacks;
    private IServerEvents _serverEvents;

    private BackfillTicket _localBackfillTicket;
    CreateBackfillTicketOptions _createBackfillTicketOptions;
    private const int _ticketCheckMs = 1000;
    private MatchmakingResults _matchmakingPayload;

    private bool _backfilling = false;
    [SerializeField] bool StartupAsServer = false;

    public enum StartupType
    {
        NotInitalized,
        Server,
        Client
    }
    [SerializeField] private StartupType startupType = StartupType.NotInitalized;
    public StartupType GetConnectionType() { return startupType; }

    async void Start()
    {
        bool server = false;
        var args = System.Environment.GetCommandLineArgs();

        // Loops for all arguments
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer" || StartupAsServer == true)
            {
                server = true;
                StartupAsServer = false;
            }
            if (args[i] == "-port" && (i + 1 < args.Length))
            {
                _serverPort = (ushort)int.Parse(args[i + 1]);
            }

            if (args[i] == "-ip" && (i + 1 < args.Length))
            {
                _externalServerIP = args[i + 1];
            }
        }

        if (server)
        {
            Debug.Log("STARTING SERVER");
            startupType = StartupType.Server;
            StartServer();
            await StartServerServices();
        }
        else
        {
            Debug.Log("STARTING CLIENT");
            startupType = StartupType.Client;
            ClientInstance?.Invoke();
        }
    }

    private void StartServer()
    {
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(InternalServerIP, _serverPort);
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }

    async Task StartServerServices()
    {
        if(UnityServices.State != ServicesInitializationState.Initialized)
        {
            _initializationOptions = new InitializationOptions();
        }

        await UnityServices.InitializeAsync(_initializationOptions);
        
        try
        {
            _multiplayService = MultiplayService.Instance;
            await _multiplayService.StartServerQueryHandlerAsync((ushort)ConnectionApprovalHandler.MAX_PLAYERS, "n/a", "n/a", "0", "n/a"); // do not leave these parameters empty, might throw bug
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Somthing went wrong trying to setup the SQP Service: \n {ex}");
        }

        try
        {
            _matchmakingPayload = await GetMatchmakerPayload(_multiplayServiceTimeout);

            // If we want backfill, keep this if else, if not comment it out
            if (_matchmakingPayload != null)
            {
                Debug.Log($"Got payload: {_matchmakingPayload}");
                await StartBackfill(_matchmakingPayload);
            }
            else
            {
                Debug.LogWarning($"Getting the Matchmaker Payload timed out, starting with defaults.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Somthing went wrong trying to setup the Allocation and Backfill Services: \n {ex}");
        }
    }

    private async Task<MatchmakingResults> GetMatchmakerPayload(int timeout)
    {
        var matchmakerPayloadTask = SubscribeAndAwaitMatchmakerAllocation();
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    private async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
        if (_multiplayService == null) return null;

        _allocationID = null;
        _serverCallbacks = new MultiplayEventCallbacks();
        _serverCallbacks.Allocate += OnMultiplayAllocation;
        _serverEvents = await _multiplayService.SubscribeToServerEventsAsync(_serverCallbacks);

        _allocationID = await AwaitAllocationID();
        var matchmakerPayload = await GetMatchmakerAllocationPayloadAsync();
        return matchmakerPayload;
    }

    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {
        Debug.Log($"OnAllocation: {allocation.AllocationId}");
        if (String.IsNullOrEmpty(allocation.AllocationId)) return;
        _allocationID = allocation.AllocationId;
    }

    private async Task<string> AwaitAllocationID()
    {
        var config = _multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is: \n" +
            $"-ServerID: {config.ServerId}\n" +
            $"-AllocationID: {config.AllocationId}\n" +
            $"-Port: {config.Port}\n" +
            $"-QueryPort: {config.QueryPort}\n" +
            $"-logs: {config.ServerLogDirectory}\n");


        while (string.IsNullOrEmpty(_allocationID))
        {
            var configID = config.AllocationId;
            if (string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(_allocationID))
            {
                _allocationID = configID;
                break;
            }
            await Task.Delay(100);
        }

        return _allocationID;
    }

    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
        try
        {
            var payloadAllocation = await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();

            var modelAsJSON = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            Debug.Log($"{nameof(GetMatchmakerAllocationPayloadAsync)}: \n {modelAsJSON}");
            return payloadAllocation;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Somthing went wrong trying to get the Matchmaker Payload in GetMatchmakerAllocationPayloadAsync: \n {ex}");
        }
        return null;
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        var backfillProperties = new BackfillTicketProperties(payload.MatchProperties);
        _localBackfillTicket = new BackfillTicket { Id = payload.MatchProperties.BackfillTicketId, Properties = backfillProperties };

        await BeginBackfilling(payload);
    }

    private async Task BeginBackfilling(MatchmakingResults payload)
    {
        // If we dont already have a local backfill ticket id, we make one
        if (string.IsNullOrEmpty(_localBackfillTicket.Id))
        {
            var matchProperties = payload.MatchProperties;

            _createBackfillTicketOptions = new CreateBackfillTicketOptions
            {
                Connection = _externalConnectionString,
                QueueName = payload.QueueName,
                Properties = new BackfillTicketProperties(matchProperties)
            };
            _localBackfillTicket.Id = await MatchmakerService.Instance.CreateBackfillTicketAsync(_createBackfillTicketOptions);
        }

        _backfilling = true;

        // this pragma just removes a warning related to the backfill loop method being async while we arent calling it as such
        #pragma warning disable 4014
        BackfillLoop();
        #pragma warning restore 4014
    }

    private async Task BackfillLoop()
    {
        while (_backfilling && NeedsPlayers())
        {
            _localBackfillTicket = await MatchmakerService.Instance.ApproveBackfillTicketAsync(_localBackfillTicket.Id);
            if (!NeedsPlayers())
            {
                await MatchmakerService.Instance.DeleteBackfillTicketAsync(_localBackfillTicket.Id);
                _localBackfillTicket.Id = null;
                _backfilling = false;
                return;
            }

            await Task.Delay(_ticketCheckMs);
        }

        _backfilling = false;
    }

    private void ClientDisconnected(ulong clientId)
    {
        if(!_backfilling && NetworkManager.Singleton.ConnectedClients.Count > 0 && NeedsPlayers())
        {
            BeginBackfilling(_matchmakingPayload);
        }
    }


    private bool NeedsPlayers()
    {
        return NetworkManager.Singleton.ConnectedClients.Count < ConnectionApprovalHandler.MAX_PLAYERS;
    }

    // This should be called after a client disconnects or after we are done using these calls in the code
    private void Dispose()
    {
        _serverCallbacks.Allocate -= OnMultiplayAllocation;
        _serverEvents?.UnsubscribeAsync();
    }
}
