using System;
using Steamworks;
using UnityEngine;

namespace Session_1.Scripts.UI
{
    public class MenuEvents : MonoBehaviour
    {
        public static MenuEvents Instance;

        private void Awake()
        {
            if (Instance != null) Instance = this;
        }

        private void OnEnable()
        {
            SteamLobbyManager.Instance.onLobbyCreated += OnLobbyCreated;
            SteamLobbyManager.Instance.onLobbyEntered += OnLobbyEntered;
        }

        private void OnDisable()
        {
            SteamLobbyManager.Instance.onLobbyCreated -= OnLobbyCreated;
            SteamLobbyManager.Instance.onLobbyEntered -= OnLobbyEntered;
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            MenuManager.Instance.OpenMenu("Loading");
        }
        
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            MenuManager.Instance.OpenMenu("Loading");
        }

        public void CreateLobby()
        {
            MenuManager.Instance.OpenMenu("Loading");
            SteamLobbyManager.Instance.CreateLobby();
        }
    }
}