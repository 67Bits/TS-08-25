using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class UnityObjectExtension
{

#if UNITY_EDITOR
    /// <summary>
    /// Calls action when the inspector changes, avoiding extra validation tests.
    /// </summary>
    public static void TestForValueChangeInInspector<T>(this T unityObject, ref bool firstOnValidateHasOccurred, Action onValueChangedUsingTheInspector) where T : UnityEngine.Object
    {
        if (!firstOnValidateHasOccurred)
        {
            firstOnValidateHasOccurred = true;
            return;
        }

        if (Thread.CurrentThread.IsBackground)
            // I wish that Awaitable.MainThreadAsync could be used here, but, alas, it's not
            // usable during deserialization, so need to use a callback like they did in the 90s.
            EditorApplication.delayCall += OnSubsequentValidateOnMainThread;
        else
            OnSubsequentValidateOnMainThread();
        
        async void OnSubsequentValidateOnMainThread()
        {
            bool isShownInAnEditor = Resources.FindObjectsOfTypeAll<Editor>().Any(editor => Array.IndexOf(editor.targets, unityObject) != -1);
            if (!isShownInAnEditor)
                return;
            
            while (EditorApplication.isCompiling || EditorApplication.isUpdating)
                await Task.Yield();
            
            if (unityObject)
                onValueChangedUsingTheInspector();
        }
    }

    public static void RenameObject(this UnityEngine.Object unityObject, string name)
        => unityObject.name = name;
#endif
}
