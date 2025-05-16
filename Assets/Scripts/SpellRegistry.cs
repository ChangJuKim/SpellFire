using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class SpellRegistry
{
    private static Dictionary<Guid, GameObject> _spellMap = new Dictionary<Guid, GameObject>();
    
    private static Dictionary<KeyCode, List<Guid>> _keyToSpellList = new Dictionary<KeyCode, List<Guid>>();

    public static void Add(Guid guid, KeyCode keyCode, GameObject spellPrefab)
    {
        if (_spellMap.ContainsKey(guid))
        {
            Debug.LogWarning($"Spell with ID {guid} already exists. Overwriting.");
        }

        _spellMap[guid] = spellPrefab;

        if (!_keyToSpellList.ContainsKey(keyCode))
        {
            _keyToSpellList[keyCode] = new List<Guid>();
        }
        _keyToSpellList[keyCode].Add(guid);
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

    public static List<Guid> GetSpellsByKeyCode(KeyCode keyCode)
    {
        if (!_keyToSpellList.ContainsKey(keyCode))
        {
            Debug.LogError($"There are no spells mapped to keyCode {keyCode}");
            return null;
        }
        return _keyToSpellList[keyCode];
    }

    public static async Task LoadAndAddSpell(Guid guid, KeyCode keyCode, string address)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
        GameObject spellPrefab = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Add(guid, keyCode, spellPrefab);
        }
        else
        {
             Debug.LogError($"Failed to load spell prefab from address: {address}");
        }
    }
}
