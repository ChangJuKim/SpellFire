using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class SpellFactory
{
    
    // Dummy values for testing
    private static readonly string[] VISUAL_PREFAB_ADDRESSES = new string[2] { "Fireball", "Cube" };

    private static Dictionary<int, Guid> hashToGuid = new Dictionary<int, Guid>();

    private static int CreateDummyHash(int index)
    {
        return index;
    }

    public static async Task InitializeSpellsAsync()
    {
        await ParseJSON("Temp stuff");
        // Other things to do when initializing
    }

    // For now, creates dummy values
    // Does 3 things:
    // 1. Gets the visual prefabs
    // 2. Parses JSON to SpellData
    // 3. Attaches the above two and SpellBehavior to a new GameObject and puts that in the SpellRegistry
    public static async Task ParseJSON(string json)
    {
        await CreateDummySpellsAsync();
    }

    // Creates two spells
    private static async Task CreateDummySpellsAsync()
    {
        if (VISUAL_PREFAB_ADDRESSES == null || VISUAL_PREFAB_ADDRESSES.Length == 0)
        {
            Debug.Log("No visual prefabs");
        }

        for (int i = 0; i < VISUAL_PREFAB_ADDRESSES.Length; i++)
        {
            // 1. Pre-set data
            GameObject visualPrefab = null;
            SpellData spellData = null;

            // 2. Visual prefabs
            GameObject prefab = await LoadPrefab(VISUAL_PREFAB_ADDRESSES[i]);
            if (prefab != null)
            {
                visualPrefab = prefab;
            }
            else
            {
                Debug.LogError($"Unable to get visual prefab {VISUAL_PREFAB_ADDRESSES[i]}");
            }

            // 3. Parse JSON to SpellData
            spellData = ScriptableObject.CreateInstance<SpellData>();

            // 4. Attach to new GameObject and register
            GameObject newSpell = new GameObject($"Spell_{spellData.name ?? $"Unnamed_{i}"}");
            GameObject.DontDestroyOnLoad(newSpell);
            
            GameObject visual = GameObject.Instantiate(visualPrefab, newSpell.transform);

            SpellBehavior spellBehavior = newSpell.AddComponent<SpellBehavior>();
            spellBehavior.spellData = spellData;

            Guid spellGuid;
            if (NetworkServer.active)
            {
                spellGuid = Guid.NewGuid();
                hashToGuid.Add(i, spellGuid);
            }
            else
            {
                if (!hashToGuid.TryGetValue(i, out spellGuid))
                {
                    Debug.LogError($"Client hashToGuid has no hash of {i}");
                    continue;
                }
            }
            SpellRegistry.Add(spellGuid, KeyCode.Q, newSpell);
        }

        // 5. Send hash-to-guid mapping from server to clients
        if (NetworkServer.active)
        {
            RPCSendHashToGuid(hashToGuid);
        }
    }

    [ClientRpc]
    private static void RPCSendHashToGuid(Dictionary<int, Guid> otherHashToGuid)
    {
        hashToGuid = otherHashToGuid;
    }

    private static async Task<GameObject> LoadPrefab(string visualPrefabAddress)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(visualPrefabAddress);
        GameObject prefab = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Failed to load prefab at address: {visualPrefabAddress}");
            return null;
        }
    }
}
