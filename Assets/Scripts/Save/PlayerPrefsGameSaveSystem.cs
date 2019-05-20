using UnityEngine;

/// <summary>
/// Saves and load game saves from PlayerPrefs.
/// </summary>
public class PlayerPrefsGameSaveSystem
{
    private const string SaveKey = "SavedGame";
        
    public void Save(GameSaveState saveState)
    {
        var serializedState = JsonUtility.ToJson(saveState);
        Debug.Log(serializedState);
        PlayerPrefs.SetString(SaveKey, serializedState);
    }

    public static GameSaveState Load()
    {
        var serializedState = PlayerPrefs.GetString(SaveKey);
        var saveState = JsonUtility.FromJson<GameSaveState>(serializedState);
        return saveState;
    }
}