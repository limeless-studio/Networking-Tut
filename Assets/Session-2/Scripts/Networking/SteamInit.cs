using System;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Session_2.Scripts.Networking
{
    public class SteamInit : MonoBehaviour
    {
        private void Start()
        {
            if (SteamAPI.Init())
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                Debug.LogError("Please Load steam first!");
            }
        }
    }
}