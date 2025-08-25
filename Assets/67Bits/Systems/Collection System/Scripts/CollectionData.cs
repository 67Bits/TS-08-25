using UnityEngine;
using Sirenix.OdinInspector;

namespace SSB.CollectionUI
{
    [CreateAssetMenu(fileName = "New Collection Data", menuName = "67Bits/ColletionSystem/Colletion Data")]
    public class CollectionData : ScriptableObject
    {
        public string Name;
        [PreviewField(height: 100, Alignment = ObjectFieldAlignment.Left)] public Sprite Icon;
    }
}
