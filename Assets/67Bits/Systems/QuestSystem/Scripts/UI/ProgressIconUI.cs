using UnityEngine;
using UnityEngine.UI;

namespace SSB.Quests
{
    public class ProgressIconUI : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Color _enabledColor;
        [SerializeField] private Color _disabledColor;
        public void SetIcon(Sprite icon, bool active)
        {
            _iconImage.sprite = icon;
            _iconImage.color = active ? _enabledColor : _disabledColor;
        }
    }
}
