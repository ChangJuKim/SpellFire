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

        // Can't I just delete this private variable and make a local variable in OnColorChanged?
        private Material playerMaterialClone;
        
        [SyncVar(hook = nameof(OnNameChanged))]
        public string playerName;

        [SyncVar(hook = nameof(OnColorChanged))]
        public Color playerColor = Color.white;

        // Movement and aim
        [SerializeField] private NavMeshAgent _agent = null;

        // Spells
        public Guid[] spells = new Guid[7]; // QWERASD

        void OnNameChanged(string _Old, string _New)
        {
            playerNameText.text = playerName;
        }

        void OnColorChanged(Color _Old, Color _New)
        {
            playerNameText.color = _New;
            playerMaterialClone = new Material(GetComponent<Renderer>().material);
            playerMaterialClone.color = _New;
            GetComponent<Renderer>().material = playerMaterialClone;
        }

        public override void OnStartLocalPlayer()
        {
            floatingInfo.transform.localPosition = new Vector3(0, -0.2f, 0.6f);
            floatingInfo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            string name = "Player" + UnityEngine.Random.Range(100, 999);
            Color color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            CmdSetupPlayer(name, color);

            // Spells
            List<Guid> QSpells = SpellRegistry.GetSpellsByKeyCode(KeyCode.Q);
            if (QSpells == null || !QSpells.Any())
            {
                Debug.LogError($"Unable to get any spells in slot Q");
                return;
            }
            spells[0] = QSpells[0];
            Debug.Log($"Spell Q is now Spell with Guid {spells[0]}");
            Debug.Log($"Loaded in Spell '{SpellRegistry.Get(spells[0])}'.");
            
        }

        [Command]
        public void CmdSetupPlayer(string _name, Color _col)
        {
            // player info sent to server, then server updates sync vars which handles it on all clients
            playerName = _name;
            playerColor = _col;
        }

        private void Awake()
        {
            // allow all players to run this
            // sceneScript = GameObject.Find("SceneReference").GetComponent<SceneReference>().sceneScript;

        }

        void Update()
        {
            if (!hasAuthority)
            {
                // make non-local players run this
                floatingInfo.transform.LookAt(Camera.main.transform);
                return;
            }

            /* //Keyboard movement
            float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
            float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4f;

            transform.Rotate(0, moveX, 0);
            transform.Translate(0, 0, moveZ);
            */

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

            if (Input.GetKey(KeyCode.Q))
            {
                CastSpell(0);
            }
        }

        void CastSpell(int index)
        {
            CastSpell();
        }

        // Selects the spell and determines the origin and destination
        void CastSpell()
        {
            // Spell
            if (spells[0] == null)
            {
                Debug.Log("Spell doesn't exist: setting spell to temp spell");
            }

            // Destination
            Vector3 destination = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                destination = hit.point;
            }

            CmdCastSpell(spells[0], transform.position, destination);
        }

        [Command]
        void CmdCastSpell(Guid spellGuid, Vector3 origin, Vector3 destination)
        {
            GameObject spell = SpellRegistry.Get(spellGuid);

            // Creation
            GameObject clone = Instantiate(spell, origin, Quaternion.identity);
            clone.AddComponent<NetworkIdentity>();
            NetworkServer.Spawn(clone);
            
            // Rotation
            clone.transform.LookAt(destination);
            clone.transform.rotation = Quaternion.Euler(0f, clone.transform.rotation.eulerAngles.y, 0f);
        }
    }
}