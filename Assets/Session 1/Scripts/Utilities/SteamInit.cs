using Steamworks;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Session_1.Scripts.Utilities
{
    public class SteamInit : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("initing....");
            // Init
            if (!SteamAPI.Init())
            {
                Debug.LogError("Steam no work!!!");
                return;
            }

            SceneManager.LoadScene(1);
        }
    }
}