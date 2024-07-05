using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using System.Collections.Generic;
using StatusOptions = Unity.Services.Matchmaker.Models.MultiplayAssignment.StatusOptions;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine.Events;

#if UNITY_EDITOR
using ParrelSync;
#endif

// Look here for more help (https://www.youtube.com/watch?v=FjOZrSPL_-Y) remove after though

public class MatchmakerClient : MonoBehaviour
{
    private string _ticketId;
    InitializationOptions _initializationOptions;

    [SerializeField] UnityEvent ClientInitalized;

    private void OnEnable()
    {
        ServerStartup.ClientInstance += SignIn;
    }

    private void OnDisable()
    {
        ServerStartup.ClientInstance -= SignIn;
        //AuthenticationService.Instance.SignOut(false);
    }

    private async void SignIn()
    {
        await ClientSignIn("SnakePlayer");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async Task ClientSignIn(string serviceProfileName = null)
    {
        if(serviceProfileName != null)
        {
            Debug.Log("Using Unity Multiplay");

            #if UNITY_EDITOR
            serviceProfileName = $"{serviceProfileName}{GetCloneNumberSuffix()}";
            #endif
            var initOptions = new InitializationOptions();
            initOptions.SetProfile(serviceProfileName);
            await UnityServices.InitializeAsync(initOptions);
        }
        else
        {
            Debug.Log("Using Networking");

            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                _initializationOptions = new InitializationOptions();
            }

            await UnityServices.InitializeAsync(_initializationOptions);
        }

        Debug.Log($"Signed In Anonymously as {serviceProfileName}({PlayerID()}");

        // Calls event once client is initalized
        ClientInitalized?.Invoke();
    }

    private static string PlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }


    #if UNITY_EDITOR
    // Used for the parrelel sync package for unity, to mainly test multi client
    private string GetCloneNumberSuffix()
    {
        string projectPath = ClonesManager.GetCurrentProjectPath();
        int lastUnderscore = projectPath.LastIndexOf('_');
        string projectCloneSuffix = projectPath.Substring(lastUnderscore + 1);

        if (projectCloneSuffix.Length != 1)
            projectCloneSuffix = "";
        return projectCloneSuffix;

    }
    #endif

    public void StartClient()
    {
        CreateAMatchmakingTicket();
    }

    private async void CreateAMatchmakingTicket()
    {
        var options = new CreateTicketOptions("SnakeMode");

        var players = new List<Player>
        {
            new Player(
                PlayerID(),
                new MatchmakingPlayerData
                {
                    Skill = 100
                }
            )

        };

        var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
        _ticketId = ticketResponse.Id;
        Debug.Log($"Ticket ID: {_ticketId}");
        PollTicketStatus();
    }

    private async void PollTicketStatus()
    {
        MultiplayAssignment multiplayAssignment = null;
        bool gotAssignment = false;

        do
        {
            Debug.Log("WAITING");
            //Rate limit delay
            await Task.Delay(TimeSpan.FromSeconds(1f));

            // Poll ticket
            var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(_ticketId);

            if (ticketStatus == null) continue;
            if(ticketStatus.Type == typeof(MultiplayAssignment))
            {
                multiplayAssignment = ticketStatus.Value as MultiplayAssignment;
            }

            switch (multiplayAssignment?.Status)
            {
                case StatusOptions.Found:
                    gotAssignment = true;
                    TicketAssigned(multiplayAssignment);
                    break;
                case StatusOptions.InProgress:
                    break;
                case StatusOptions.Failed:
                    Debug.LogError($"Failed to get ticket status. Error: {multiplayAssignment.Message}");
                    break;
                case StatusOptions.Timeout:
                    gotAssignment = true;
                    Debug.LogError($"Failed to get ticket status. Ticket timed out.");
                    break;
                default:
                    throw new InvalidOperationException();
            }

        } while (!gotAssignment);
    }

    private void TicketAssigned(MultiplayAssignment assignment)
    {
        Debug.Log($"Ticket Assigned: {assignment.Ip}:{assignment.Port}");

        if(assignment.Port == null)
        {
            Debug.LogError($"Port not assigned on ticket. Ticket Error, Ticket timed out.");
            return;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(assignment.Ip, (ushort)assignment.Port);
        NetworkManager.Singleton.StartClient();
    }

    [Serializable]
    public class MatchmakingPlayerData
    {
        public int Skill;
    }
}
