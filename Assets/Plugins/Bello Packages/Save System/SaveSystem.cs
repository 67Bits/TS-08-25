using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[HelpURL("https://doc.clickup.com/9017157017/p/h/8cqdtct-91597/543035e87450e2a")]
public class SaveSystem : Singleton<SaveSystem>
{
    private static SaveSettings _saveSettings;
    public static SaveSettings SaveSettings
    {
        get
        {
            if (_saveSettings == null)
            {
                SaveSettings[] savesSettings = Resources.LoadAll<SaveSettings>("");
                if (savesSettings == null || savesSettings.Length < 1) return null;
                _saveSettings = savesSettings[0];
            }
            return _saveSettings;
        }
    }

    public static Action OnSave;

    #region Save and Load Game
    public static void SaveGame()
    {
#if UNITY_EDITOR
        if (!SaveSettings.EnableSavingInEditor) return;
#endif
        if (SaveSettings.GoToLastSaveSceneWhenLoad) SaveData.Instance.lastScene = SceneManager.GetActiveScene().name;

        SaveDataFile(SaveData.Instance);
        OnSave?.Invoke();
        Debug.Log("Game Saved");
    }
    public static void LoadGame()
    {
#if UNITY_EDITOR
        if (!SaveSettings.EnableLoadingInEditor) return;
#endif
        if (SaveSettings.LinkToPlayerPrefs && !PlayerPrefs.HasKey("SaveData")) return;

        var save = LoadDataFile(SaveData.Instance) as SaveData;
        if (save == null) return;

        SaveData.Instance = save;

        if (SaveSettings.GoToLastSaveSceneWhenLoad &&
            SaveData.Instance.lastScene != null && SceneManager.GetSceneByName(SaveData.Instance.lastScene) != null)
            SceneManager.LoadScene(SaveData.Instance.lastScene);
    }
    public static void SaveGameJson()
    {
#if UNITY_EDITOR
        if (!SaveSettings.EnableSavingInEditor) return;
#endif
        if (SaveSettings.GoToLastSaveSceneWhenLoad) SaveData.Instance.lastScene = SceneManager.GetActiveScene().name;

        SaveDataFile(SaveData.Instance);
        OnSave?.Invoke();
        Debug.Log("Game Saved");
    }
    public static void LoadGameJson()
    {
#if UNITY_EDITOR
        if (!SaveSettings.EnableLoadingInEditor) return;
#endif
        if (SaveSettings.LinkToPlayerPrefs && !PlayerPrefs.HasKey("SaveData")) return;

        var save = LoadCustomJson(SaveData.Instance);
        if (save == null) return;

        SaveData.Instance = save;

        if (SaveSettings.GoToLastSaveSceneWhenLoad &&
            SaveData.Instance.lastScene != null && SceneManager.GetSceneByName(SaveData.Instance.lastScene) != null)
            SceneManager.LoadScene(SaveData.Instance.lastScene);
    }
    #endregion
    #region Save and Load Custom JSON
    public static void SaveGameCustomJson(object saveData)
    {
#if UNITY_EDITOR
        if (!SaveSettings.EnableSavingInEditor) return;
#endif
        var className = saveData.GetType().Name;
        SaveDataFile(saveData);
        OnSave?.Invoke();
        Debug.Log($"{className} Data Saved");
    }
    public static T LoadCustomJson<T>(T loadData)
    {
        var className = loadData.GetType().Name;
#if UNITY_EDITOR
        if (!SaveSettings.EnableLoadingInEditor) return default;
#endif
        if (SaveSettings.LinkToPlayerPrefs && !PlayerPrefs.HasKey(className)) return default;

        var save = LoadDataFile(loadData);
        if (save == null) return default;

        loadData = (T)save;
        return loadData;
    }
    public static bool SaveDataFile<T>(T saveData)
    {
        try
        {
            var className = saveData.GetType().Name;
            string data = JsonConvert.SerializeObject(saveData);
            PlayerPrefs.SetString(className, data);
            return true;
        }
        catch (Exception ex) { Debug.LogException(ex); }
        return false;
    }
    public static object LoadDataFile<T>(T loadData)
    {
        try
        {
            var className = loadData.GetType().Name;
            string dataJson = PlayerPrefs.GetString(className, "");
            object data = JsonConvert.DeserializeObject<T>(dataJson);
            Debug.Log($"{className} Data Loaded");
            return data;
        }
        catch (Exception ex) { Debug.LogException(ex); return false; }
    }
    public static bool TryLoadDataFile<T>(out T loadData)
    {
        try
        {
            var className = typeof(T).Name;
            string dataJson = PlayerPrefs.GetString(className, "");
            loadData = JsonConvert.DeserializeObject<T>(dataJson);
            return true;
        }
        catch (Exception ex) { Debug.LogException(ex); }
        loadData = default(T);
        return false;
    }

    #endregion
    #region Save and Load Unity JSON
    public static bool SaveDataFile(object saveData)
    {
        var className = saveData.GetType().Name;
        string dataJson = PlayerPrefs.GetString(className, "");
        string data = JsonUtility.ToJson((SaveData)saveData);
        PlayerPrefs.SetString(dataJson, data);
        return true;
    }
    public static object LoadDataFile(object saveData)
    {
        var className = saveData.GetType().Name;
        string dataJson = PlayerPrefs.GetString(className, "");
        object data = JsonUtility.FromJson<SaveData>(dataJson);
        Debug.Log("Game Loaded");
        return data;
    }
    #endregion
    #region Save ID

    public static bool SaveDataById<T>(T saveData, string id)
    {
        try
        {
            string data = JsonConvert.SerializeObject(saveData);
            PlayerPrefs.SetString(id, data);
            Debug.Log($"<color=blue>{id} Data Saved</color>");
            return true;
        }
        catch (Exception ex) { Debug.LogException(ex); }
        return false;
    }

    public static T LoadDataById<T>(string id)
    {
        try
        {
            string dataJson = PlayerPrefs.GetString(id, "");
            var data = JsonConvert.DeserializeObject<T>(dataJson);
            Debug.Log($"<color=blue>{id} Data Loaded</color>");
            return data;
        }
        catch (Exception ex) { Debug.LogException(ex); return default; }
    }
    #endregion

    #region OnApplication
    /// <summary>
    /// Runs on start up, before the first scene, creates the firt instance of SaveSystem and/or Load the gamene
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnApplicationInicialization()
    {
        if (SaveSettings.AutoStart) DontDestroyOnLoad(SaveSystem.Instance);
        if (SaveSettings.LoadAtApplicationStart) SaveSystem.LoadGameJson();
    }
    private void OnApplicationPause(bool pause)
    {
        if (SaveSettings.SaveWhenPause) SaveGame();
    }
    private void OnApplicationQuit()
    {
        if (SaveSettings.SaveWhenQuit) SaveGame();
    }
    #endregion
}

public static class SaveSystemExtensions
{
    public static void SaveBool(this bool @bool, string key)
    {
        PlayerPrefs.SetInt(key, @bool ? 1 : 0);
    }
    public static void SaveBool(string key, bool @bool)
    {
        PlayerPrefs.SetInt(key, @bool ? 1 : 0);
    }
    public static bool LoadBool(this bool @bool, string key)
    {
        var data = PlayerPrefs.GetInt(key);
        return data == 1;
    }
    public static bool LoadBool(string key)
    {
        var data = PlayerPrefs.GetInt(key);
        return data == 1;
    }
}
