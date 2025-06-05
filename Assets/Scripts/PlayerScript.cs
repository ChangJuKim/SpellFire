using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace QuickStart
{
    public class PlayerScript : NetworkBehaviour
    {
        public TMP_Text playerNameText;
        public GameObject floatingInfo;
        
        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        public Color playerColor = Color.white;

        // Movement and aim
        [SerializeField] private NavMeshAgent _agent = null;

        // Spells
        public Guid[] spells = new Guid[7]; // QWERASD

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        public override void OnStartLocalPlayer()
        {
            floatingInfo.transform.localPosition = new Vector3(0, -0.2f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + UnityEngine.Random.Range(100, 999);
            Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;
        }

        void Update()
        {
            if (!isOwned)
            {
                // make non-local players run this
                floatingInfo.transform.LookAt(Camera.main.transform);
                return;
            }

            // Click to move
            // From the camera, shoots a ray to a point. If valid, sets the agent's destination to the collision point
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    _agent.SetDestination(hit.point);
                }
            }
        }
    }
}