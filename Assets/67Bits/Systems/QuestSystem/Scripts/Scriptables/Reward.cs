using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests
{
    [CreateAssetMenu(fileName = "Default Reward", menuName = "67Bits/Quest/Reward")]
    public class Reward : ScriptableObject
    {
        [field: SerializeField, PreviewField(45)] public Sprite Icon { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [SerializeField] private UnityEvent<float> OnGetReward;
        public virtual void GetReward(float ammount) => OnGetReward?.Invoke(ammount);
    }
}
