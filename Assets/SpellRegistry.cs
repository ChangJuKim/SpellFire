using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpellRegistry : MonoBehaviour
{

    public static SpellRegistry Instance { get; private set; }

    private static Dictionary<Guid, GameObject> _spellMap = new Dictionary<Guid, GameObject>();

    private void Awake()
    {
        if (Instance == null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public static void Add(Guid guid, GameObject spellPrefab)
    {
        if (_spellMap.ContainsKey(guid))
        {
            Debug.LogWarning($"Spell with ID {guid} already exists. Overwriting.");
        }

        _spellMap[guid] = spellPrefab;
    }

    public static GameObject Get(Guid guid)
    {
        if (_spellMap.TryGetValue(guid, out GameObject spell))
        {
            return spell;
        }

        Debug.LogError($"Spell with ID {guid} not found");
        return null;
    }

    public static async Task LoadAndAddSpell(Guid guid, string address)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
        GameObject spellPrefab = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Add(guid, spellPrefab);
        }
        else
        {
             Debug.LogError($"Failed to load spell prefab from address: {address}");
        }
    }
}
