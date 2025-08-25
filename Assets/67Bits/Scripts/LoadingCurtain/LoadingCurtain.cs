using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCurtain : Singleton<LoadingCurtain>
{
    /// <summary>
    /// Reflects the current state of loading activity within your application.
    /// </summary>
    [SerializeField] private static int activeLoaders = 0;

    [Header("Loading Curtain settings")]
    [SerializeField] private GameObject Canvas;
    [SerializeField] private bool StartEnabledInBuild = true;


    void Awake()
    {
        base.Awake();

#if !UNITY_EDITOR
        this.gameObject.SetActive(StartEnabledInBuild);
#endif
    }

    public void SetActive(bool value)
    {
        Canvas.SetActive(value);
    }

    public void RegisterLoader()
    {
        activeLoaders++;
        SetActive(HasActiveLoaders());
    }

    public void UnregisterLoader()
    {
        activeLoaders--;
        SetActive(HasActiveLoaders());
    }

    public bool HasActiveLoaders() => activeLoaders > 0;
}
