using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace SSBPerks
{
    public class PerkButton : MonoBehaviour
    {
        [SerializeField] private PerkRarity _rarity;
        [SerializeField] private bool _showStars;
        [Header("Visuals References")]
        [SerializeField] private Image _upgradeIcon;
        [SerializeField] private TextMeshProUGUI _upgradeNameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _button;
        //[SerializeField] private TemporaryPerk _bufferSticker;
        private TemporaryPerk _currentPerk;

        [Header("Stars Settings")]
        [SerializeField] private GameObject _starsParent;
        [SerializeField] private GameObject[] _starsFill;

        public PerkRarity Rarity => _rarity;

        private void Awake()
        {
            _button.onClick.AddListener(Select);
        }

        private void OnEnable()
        {
            transform.rotation = Quaternion.identity;
        }

        public void SetButton(TemporaryPerk sticker)
        {
            _upgradeIcon.sprite = sticker.Icon;
            _upgradeNameText.text = sticker.Name;
            _currentPerk = sticker;
            _descriptionText.text = sticker.Description;

            if (!_showStars || _showStars && sticker.Level == 0)
                return;

            _starsParent.SetActive(true);
            for (int i = 0; i < _starsFill.Length; i++)
            {
                bool active = sticker.Level > i;
                _starsFill[i].SetActive(active);
            }
        }

        private void Select()
        {
            if (_currentPerk.Consumable)
            {
                PerksManager.Instance.ConsumePerk(_currentPerk);
            }
            else
            {
                PerksManager.Instance.EquipTemporaryPerk(_currentPerk);
            }
        }

    }
}