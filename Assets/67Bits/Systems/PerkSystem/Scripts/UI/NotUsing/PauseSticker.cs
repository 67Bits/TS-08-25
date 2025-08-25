using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSBPerks
{
    public class PauseSticker : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _stickerName;
        [SerializeField] Image _stickerIcon;

        public void SetSticker(TemporaryPerk sticker)
        {
            _stickerName.text = sticker.Name;
            _stickerIcon.sprite = sticker.Icon;
        }
    }
}