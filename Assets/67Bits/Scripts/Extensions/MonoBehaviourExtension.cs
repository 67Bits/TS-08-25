using System;
using UnityEngine;

public static class MonoBehaviourExtension
{

#if UNITY_EDITOR

    /// <summary>
    /// Calls action when the inspector changes, avoiding extra validation tests.
    /// </summary>
    public static void TestForValueChangeInInspector(this MonoBehaviour scriptableObject, ref bool firstOnValidateHasOccurred, Action onValueChangedUsingTheInspector)
    {
        UnityObjectExtension.TestForValueChangeInInspector(scriptableObject, ref firstOnValidateHasOccurred, onValueChangedUsingTheInspector);
    }
#endif
}