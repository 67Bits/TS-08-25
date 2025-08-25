using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSB.CollectionUI
{
    public class CollectionButton : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        public virtual void Initialize(CollectionData data)
        {
            _icon.sprite = data.Icon;
            _nameText.text = data.Name;
        }
    }
}
