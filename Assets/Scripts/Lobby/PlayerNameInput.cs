using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; }

    private const string PLAYER_PREFS_NAME_KEY = "PlayerName";

    private void Start()
    {
        SetUpInputField();
    }

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PLAYER_PREFS_NAME_KEY))
        {
            return;
        }

        string defaultName = PlayerPrefs.GetString(PLAYER_PREFS_NAME_KEY);

        nameInputField.text = defaultName;

        SetPlayerName();
    }

    public void SetPlayerName()
    {
        continueButton.interactable = !string.IsNullOrEmpty(nameInputField.text);
    }

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;

        PlayerPrefs.SetString(PLAYER_PREFS_NAME_KEY, nameInputField.text);
    }
}
