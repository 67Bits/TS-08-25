using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.DialogueSystem
{
    [CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public Sentence[] Sentences;
        [FoldoutGroup("Events")] public UnityEvent OnStart;
        [FoldoutGroup("Events")] public UnityEvent OnFinish;
        [Button(ButtonSizes.Gigantic)]
        public void StartDialogue()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Dialogue can only be started in play mode.");
                return;
            }
            DialogueUI.Instance.StartDialogue(this);
        }
    }
    [System.Serializable]
    public class Sentence
    {
        [HorizontalGroup("actor",.2f), LabelWidth(75)]public bool UseActor;
        [HorizontalGroup("actor"),ShowIf("@UseActor"), Required, LabelWidth(35)] public DialogueActor Actor;
        [HorizontalGroup("static", .7f), ShowIf("@UseActor"), Required] public string StaticAnimation = "Idle";
        [HorizontalGroup("static"), ShowIf("@UseActor"), LabelText("Transition")] public float StaticTransitionDuration;
        [HorizontalGroup("talk", .7f), ShowIf("@UseActor"), Required] public string TalkingAnimation = "Talk";
        [HorizontalGroup("talk"), ShowIf("@UseActor"), LabelText("Transition")] public float TalkTransitionDuration = .25f;
        [HorizontalGroup("actor"), ShowIf("@!UseActor")] public string Name;

        [TextArea(3, 5)] public string Text;
        [FoldoutGroup("Events")] public UnityEvent OnStart;
        [FoldoutGroup("Events")] public UnityEvent OnFinish;
    }
}
