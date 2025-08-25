using Mono.CSharp;
using System;
using UnityEngine;

namespace SSB.Analytics
{
    public class AnalyticsManager : MonoBehaviour
    {
        public static AnalyticsManager Instance { get; private set; }
        public Action OnProgresionEvent;
        public Action OnDesignEvent;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            Instance = new GameObject("Analytics Manager").AddComponent<AnalyticsManager>();
            DontDestroyOnLoad(Instance);
        }
        public virtual void ProgressionEvent(ProgressionType type)
        {
            switch (type)
            {
                case ProgressionType.Start:
                    break;
                case ProgressionType.Complete:
                    break;
                case ProgressionType.Fail:
                    break;
            }
            OnProgresionEvent?.Invoke();
        }
        public virtual void DesignEvent(string eventName)
        {
            OnDesignEvent?.Invoke();
        }
        public virtual void DesignEvent(string eventName, string args)
        {
            OnDesignEvent?.Invoke();
        }
    }
    public enum ProgressionType
    {
        Start,
        Complete,
        Fail
    }
}