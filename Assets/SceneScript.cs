using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QuickStart
{
    public class SceneScript : NetworkBehaviour
    {
        public TMP_Text canvasStatusText;
        public PlayerScript playerScript;

        public SceneReference sceneReference;

        // todo: change sceneNames to dynamically get the names of the scenes of maps (and not the menus) somehow
        private string[] sceneNames = {"Map1", "Map2"};

        [SyncVar(hook = nameof(OnStatusTextChanged))]
        public string statusText;

        void OnStatusTextChanged(string _Old, string _New)
        {
            //called from sync var hook, to update info on screen for all players
            canvasStatusText.text = statusText;
        }

        public void ButtonSendMessage()
        {
            if (playerScript != null)
                playerScript.CmdSendPlayerMessage();
        }

        public void ButtonChangeScene()
        {
            if (isServer)
            {
                Scene scene = SceneManager.GetActiveScene();
                int index = Array.FindIndex(sceneNames, elem => elem == scene.name);
                if (index == -1)
                    NetworkManager.singleton.ServerChangeScene(sceneNames[0]);
                else
                    NetworkManager.singleton.ServerChangeScene(sceneNames[(index + 1) % sceneNames.Length]);
            }
            else
                Debug.Log("You are not Host.");
        }
    }
}
