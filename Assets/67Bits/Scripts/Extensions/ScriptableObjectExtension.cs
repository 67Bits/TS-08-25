using System;
using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

public static class ScriptableObjectExtension
{

#if UNITY_EDITOR
    /// <summary>
    /// Calls action when the inspector changes, avoiding extra validation tests.
    /// </summary>
    public static void TestForValueChangeInInspector(this ScriptableObject scriptableObject, ref bool firstOnValidateHasOccurred, Action onValueChangedUsingTheInspector)
    {
        UnityObjectExtension.TestForValueChangeInInspector(scriptableObject, ref firstOnValidateHasOccurred, onValueChangedUsingTheInspector);
    }

    public static void RenameScriptableObject(this ScriptableObject scriptableObject, string newName)
    {
        if (scriptableObject == null)
        {
            Debug.LogError("ScriptableObject is null.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(scriptableObject);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("ScriptableObject is not an asset in the project.");
            return;
        }

        string newAssetPath = System.IO.Path.GetDirectoryName(assetPath) + "/" + newName + ".asset";

        AssetDatabase.RenameAsset(assetPath, newName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        scriptableObject.RenameObject(newName);

        Debug.Log($"Renamed asset from {assetPath} to {newAssetPath}");
    }
#endif
}