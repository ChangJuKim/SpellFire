using UnityEngine;

public class GameManager : MonoBehaviour
{
    private async void Awake()
    {
        await SpellFactory.InitializeSpellsAsync();
    }
}
