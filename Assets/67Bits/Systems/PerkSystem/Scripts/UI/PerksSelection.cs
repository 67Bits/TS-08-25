using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ObjectPooling;
namespace SSBPerks
{
    public class PerksSelection : MonoBehaviour
    {
        [SerializeField] private PerkButtonType[] _buttons;
        [SerializeField] private Transform _buttonsParent;
        [SerializeField] private int _perksToShow = 3;
        private bool _deletedPrefabButtons;
        private List<GameObject> _instantiatedButtons = new List<GameObject>();

        private void OnEnable()
        {
            if (!_deletedPrefabButtons)
            {
                DeletePrefabButtons();
            }
            RollPlayerPerks();
        }

        public void RollPlayerPerks()
        {
            ClearButtons();
            TemporaryPerk[] availableTStickers = PerksManager.Instance.GetAvailableTemporaryPerks(_perksToShow);

            if (availableTStickers.Length == 0)
            {
                DisableScreen();
                return;
            }

            int maxPerks = Mathf.Min(availableTStickers.Length, _perksToShow);

            for (int i = 0; i < maxPerks; i++)
            {
                var perkButtonType = _buttons.Where(x => x.Rarity == availableTStickers[i].Rarity).FirstOrDefault();
                if (perkButtonType != null)
                {
                    var buttonInstance = ObjectPool.InstantiateFromPool(perkButtonType.ButtonPrefab, disabledOnly: true);
                    buttonInstance.transform.SetParent(_buttonsParent);
                    var perkButton = buttonInstance.GetComponent<PerkButton>();
                    perkButton.SetButton(availableTStickers[i]);
                    buttonInstance.gameObject.SetActive(true);
                    _instantiatedButtons.Add(buttonInstance);
                }
            }
        }

        private void DeletePrefabButtons()
        {
            var buttons = _buttonsParent.childCount;

            for (var i = 0; i < buttons; i++)
            {
                Destroy(_buttonsParent.GetChild(i).gameObject);
            }

            _deletedPrefabButtons = true;
        }

        private void ClearButtons()
        {
            foreach (var button in _instantiatedButtons)
            {
                button.gameObject.SetActive(false);
            }

            _instantiatedButtons.Clear();
        }

        private void DisableScreen()
        {
            gameObject.SetActive(false);
        }
    }

    [System.Serializable]
    public class PerkButtonType
    {
        [SerializeField] private PerkButton _buttonPrefab;
        public PerkRarity Rarity => _buttonPrefab.Rarity;
        public GameObject ButtonPrefab => _buttonPrefab.gameObject;
        public PerkButton PerkButton => _buttonPrefab;
    }

}