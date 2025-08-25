using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Runs coroutines for other objects.
/// </summary>
// Used by non monobehaviors, or disabled objects.
public class CoroutineRunner : Singleton<CoroutineRunner>
{
    private static CoroutineRunnerSettings _CRunnerSettings;
    public static CoroutineRunnerSettings CRunnerSettings
    {
        get
        {
            if (_CRunnerSettings == null)
            {
                CoroutineRunnerSettings[] cRunnerSettings = Resources.LoadAll<CoroutineRunnerSettings>("");
                if (cRunnerSettings == null || cRunnerSettings.Length < 1)
                    return null;
                _CRunnerSettings = cRunnerSettings[0];
            }
            return _CRunnerSettings;
        }
    }

    #region Initialization
    /// <summary>
    /// Runs on start up, before the first scene, creates the firt instance of SaveSystem and/or Load the gamene
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnApplicationInicialization()
    {
        if (CRunnerSettings.AutoInitialize)
            InitializeInstance();
    }

    public static void InitializeInstance()
    {
        DontDestroyOnLoad(CoroutineRunner.Instance);
    }
    #endregion

    public Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return (this as MonoBehaviour).StartCoroutine(coroutine);
    }

    public void StopCoroutine(Coroutine coroutine)
    {
        (this as MonoBehaviour).StopCoroutine(coroutine);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
