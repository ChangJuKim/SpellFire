using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpellFactory
{
    // Dummy values for testing
    private static readonly string[] VISUAL_PREFAB_ADDRESSES = new string[2] { "FireballVisual", "SpellTwoVisual" };
    public static readonly SpellData[] SPELL_DATAS = new SpellData[2];

    // For now, creates dummy values
    // Does 3 things:
    // 1. Gets the visual prefabs
    // 2. Parses JSON to SpellData
    // 3. Attaches the above two and SpellBehavior to a new GameObject and puts that in the SpellRegistry
    public void ParseJSON(string json)
    {
        CreateDummySpellsAsync();
    }

    // Creates two spells
    private async void CreateDummySpellsAsync()
    {
        Assert.IsTrue(VISUAL_PREFAB_ADDRESSES.Length == SPELL_DATAS.Length);

        // 0. Pre-set data
        GameObject visualPrefab = null;
        SpellData spellData = null;

        for (int i = 0; i < VISUAL_PREFAB_ADDRESSES.Length; i++)
        {
            // 1. Visual prefabs
            GameObject prefab = await LoadPrefab(VISUAL_PREFAB_ADDRESSES[i]);
            if (prefab != null)
            {
                visualPrefab = prefab;
            }
            else
            {
                Debug.LogError($"Unable to get visual prefab {VISUAL_PREFAB_ADDRESSES[i]}");
            }

            // 2. Parse JSON to SpellData
            spellData = SPELL_DATAS[i];

            // 3. Attach to new GameObject and register
            GameObject newSpell = new GameObject($"Spell_{spellData.name ?? $"Unnamed_{i}"}");
            GameObject.DontDestroyOnLoad(newSpell);
            
            GameObject visual = GameObject.Instantiate(visualPrefab, newSpell.transform);

            SpellBehavior spellBehavior = newSpell.AddComponent<SpellBehavior>();
            spellBehavior.spellData = spellData;

            SpellRegistry.Add(Guid.NewGuid(), newSpell);
        }
    }

    public async Task<GameObject> LoadPrefab(string visualPrefab)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(visualPrefab);
        GameObject prefab = await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return prefab;
        }
        else
        {
            Debug.LogError($"Failed to load prefab at address: {visualPrefab}");
            return null;
        }
    }
}
