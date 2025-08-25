using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSBPerks
{
    public class StickerChosen : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _titleTMP;
        [SerializeField] TextMeshProUGUI _descripTMP;
        [SerializeField] Image _icon;

        private void OnEnable()
        {
            SetSticker();
        }

        void SetSticker()
        {
            TemporaryPerk sticker = PerksManager.Instance.EquippedTemporaryPerks[^1];
            _titleTMP.text = sticker.Name;
            _descripTMP.text = sticker.Description;
            _icon.sprite = sticker.Icon;
        }
    }
}