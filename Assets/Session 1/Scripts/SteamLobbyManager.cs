using System;
using Core.Attributes;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;

namespace Session_1.Scripts
{
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager Instance;
        private const string HostAddressKey = "HostAddress";
        
        private NetworkManager _networkManager;
        
        
        // Callbacks
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> LobbyJoinRequested;
        protected Callback<LobbyEnter_t> LobbyEntered;

        public ulong lobbyId;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (!SteamManager.Initialized)
            {
                SceneManager.LoadScene(0);
                return;
            }
            
            _networkManager = GetComponent<NetworkManager>();
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            LobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);   
        }

        [Button("Create Lobby")]
        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _networkManager.maxConnections);
        }

        public void JoinLobby(ulong lobbyId)
        {
            SteamMatchmaking.JoinLobby(new CSteamID(lobbyId));
        }

        #region Callbacks

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby!! get better!");
                return;
            }

            lobbyId = callback.m_ulSteamIDLobby;
            Debug.Log($"Lobby Created OK: {lobbyId}");
            
            // Network Manager (Mirror shit)
            _networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), HostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), "Name", $"{SteamFriends.GetPersonaName()}'s Lobby");
        }
        
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            lobbyId = callback.m_ulSteamIDLobby;
            if (NetworkServer.active) return;

            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyId), HostAddressKey);
            _networkManager.networkAddress = hostAddress;
            _networkManager.StartClient();
        }

        private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        #endregion
    }
}