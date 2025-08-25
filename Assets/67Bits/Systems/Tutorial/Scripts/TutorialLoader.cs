using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SSBTutorial
{
    public class TutorialLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void LoadAllTutorials()
        {
            var tutorials = Resources.LoadAll<TutorialStep>("");

            for (int i = 0; i < tutorials.Length; i++)
            {
                TutorialStep tutorial = tutorials[i];
                tutorial.LoadState();
            }
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Tutorial/ResetAll")]
        private static void ResetAll()
        {
            var tutorials = Resources.LoadAll<TutorialStep>("");

            for (int i = 0; i < tutorials.Length; i++)
            {
                TutorialStep tutorial = tutorials[i];
                tutorial.Reset();
                EditorUtility.SetDirty(tutorial);
                AssetDatabase.SaveAssetIfDirty(tutorial);
                AssetDatabase.Refresh();
            }
        }
#endif
    }
}