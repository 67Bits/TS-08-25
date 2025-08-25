using UnityEditor;
using UnityEngine;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.TestTools;

namespace SSB.Editor
{
    [Overlay(typeof(SceneView), "67 Bits")]
    public class SSBToolbarOverlay : ToolbarOverlay
    {
        public SSBToolbarOverlay() : base(CustomPlayButton.ID, AnotherButton.ID) { }
    }
    [EditorToolbarElement(ID)]
    public class CustomPlayButton : EditorToolbarButton
    {
        public const string ID = "PlayFromInitializationButton";
        public CustomPlayButton()
        {
            text = EditorSceneManager.playModeStartScene ? "Reset Play Mode Scene" : "Play From Initialization";
            clicked += OnClick;
        }
        void OnClick()
        {
            if (EditorSceneManager.playModeStartScene)
            {
                EditorApplication.EnterPlaymode();
                EditorSceneManager.playModeStartScene = null;
                text = "Reset Play Mode Scene";
                Debug.Log($"<color=yellow>Loading game from first build scene</color>");
            }
            else
            {
                EditorApplication.EnterPlaymode();
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneUtility.GetScenePathByBuildIndex(0));
                text = "Play From Initialization";
                Debug.Log($"<color=yellow>Loading game from this scene</color>");
            }
        }
    }
    [EditorToolbarElement(ID)]
    public class AnotherButton : EditorToolbarButton
    {
        public const string ID = "CustomButton";
        public AnotherButton() { text = "CustomButton"; clicked += OnClick; }
        void OnClick() { }
    }
}
