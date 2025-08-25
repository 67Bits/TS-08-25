using UnityEngine;

namespace SSB.DialogueSystem
{
    public class DialogueActor : MonoBehaviour
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
    }
}
