using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

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

            HandleInput();
        }

        private void HandleInput()
        {
            // Click to move
            if (Input.GetMouseButtonDown(1))
            {
                _agent.SetDestination(GetMouseLocation());
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if (spells[0] != Guid.Empty)
                {
                    // Cast spell Q
                    CommandCastSpell(spells[0], GetMouseLocation());
                }
            }
        }

        private Vector3 GetMouseLocation()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point; // Return the point where the ray hits the ground
            }
            return Vector3.zero; // Default if no hit
        }

        [Command]
        private void CommandCastSpell(Guid spellId, Vector3 targetPosition)
        {
            if (!ValidateSpell(spellId)) { return; }

            GameObject spellPrefab = SpellRegistry.Get(spellId);

            // Creation
            GameObject clone = Instantiate(spellPrefab, transform.position, Quaternion.identity);
            clone.AddComponent<NetworkIdentity>();
            NetworkServer.Spawn(clone);

            // Rotation
            clone.transform.LookAt(targetPosition);
            clone.transform.rotation = Quaternion.Euler(0f, clone.transform.rotation.eulerAngles.y, 0f);

            RpcPlaySpellEffects(spellId, clone.transform.position, clone.transform.rotation);
        }

        private bool ValidateSpell(Guid spellId)
        {
            return true;
        }

        [ClientRpc]
        private void RpcPlaySpellEffects(Guid spellId, Vector3 position, Quaternion rotation)
        {
            // Here you would play the spell effects, e.g. particle systems, sounds, etc.
            Debug.Log($"Playing spell {spellId} effects at {position} with rotation {rotation}");
        }
    }
}