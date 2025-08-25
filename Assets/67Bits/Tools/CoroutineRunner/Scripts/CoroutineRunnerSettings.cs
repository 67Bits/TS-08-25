using UnityEngine;


[CreateAssetMenu(menuName = "Services/Coroutine Runner/Settings", fileName = "CoroutineRunnerSettings")]
public class CoroutineRunnerSettings : ScriptableObject
{
    [Header("Configurations")]
    public bool AutoInitialize = false;

}