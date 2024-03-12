using System;
using Mirror;
using Steamworks;
using UnityEngine;
using Core.Attributes;
using UnityEngine.SceneManagement;

namespace Session_2.Scripts.Networking
{
    /*
     * Lobby Manager:
     *  - Create Lobbies.
     *  - Join Lobbies.
     *  - Query Lobbies.
     */
    
    /*
     * {
     *  "key": "value"
     * }
     */
    
    public class SteamLobbyManager : MonoBehaviour
    {
        public static SteamLobbyManager Instance;

        // Events
        public event Action<LobbyCreated_t> onLobbyCreated;
        public event Action<LobbyEnter_t> onLobbyEntered;
        
        // Callbacks
        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<LobbyEnter_t> LobbyEntered;
        
        // vars
        public ulong lobbyId;
        private const string HostAddress = "HostAdress";
        private NetworkManager _networkManager;
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
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
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }

        [Button("Create Lobby")]
        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
        }

        public void JoinLobby(ulong lobby)
        {
            SteamMatchmaking.JoinLobby(new CSteamID(lobby));
        }
        
        #region Callbacks

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            // Check if is successfull
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create a lobby! check ur internet!!!!");
                return;
            }

            lobbyId = callback.m_ulSteamIDLobby;
            Debug.Log($"Created lobby: {lobbyId}");
            _networkManager.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), HostAddress, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyId), "Name", $"{SteamFriends.GetPersonaName()}'s Lobby");
            onLobbyCreated?.Invoke(callback);
        }

        
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            lobbyId = callback.m_ulSteamIDLobby;

            if (NetworkServer.active) return;

            string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyId), HostAddress);
            _networkManager.networkAddress = hostAddress;
            _networkManager.StartClient();
            onLobbyEntered?.Invoke(callback);
        }
        
        #endregion
    }
}