using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Talents
{
    public class PermanentUpgradeApplier : MonoBehaviour
    {
        public Action<bool> OnNotification;
        [SerializeField] private List<PerkIcon> _perkIcons = new List<PerkIcon>();
        [Tooltip("Predefined sequence of talents to be rewarded")]
        [SerializeField] private List<PermanentUpgradeTier> _appliersTiers = new List<PermanentUpgradeTier>();
        [SerializeField] private PermanentUpgradeTier _whenEndTier = null;
        [SerializeField] private Button _nextRewardButton;
        [SerializeField, ReadOnly] private int _currentTierId = 0;
        private ICurrency _currency;

        private IEnumerator Start()
        {
            yield return null;
            LoadCurrentTierId();
            ShowNotificationWhenNextTierAvailable();
        }
        private void OnEnable()
        {
            LoadCurrentTierVisual();
            //Show Notification if has money for next
        }
        public void ShowNotificationWhenNextTierAvailable()
        {
            int nextLevel = _currentTierId;

            int nextLevelPrice = 0;
            if (nextLevel >= _appliersTiers.Count)
            {
                int levelsAboveList = nextLevel - _appliersTiers.Count - 1;
                nextLevelPrice = _whenEndTier.price + 20 * levelsAboveList;
            }
            else
                nextLevelPrice = _appliersTiers[nextLevel].price;

            bool playerHasMoney = HasCurrency(nextLevelPrice);
            OnNotification?.Invoke(playerHasMoney);
        }
        public void PlayerHasMoneyForNextLevel()
        {
            int nextLevel = _currentTierId;

            int nextLevelPrice = 0;
            if (nextLevel >= _appliersTiers.Count)
            {
                int levelsAboveList = nextLevel - _appliersTiers.Count - 1;
                nextLevelPrice = _whenEndTier.price + 20 * levelsAboveList;
            }
            else
                nextLevelPrice = _appliersTiers[nextLevel].price;

            bool playerHasMoney = HasCurrency(nextLevelPrice);
            _nextRewardButton.interactable = playerHasMoney;
            TextMeshProUGUI text = _nextRewardButton.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            if (text)
            {
                text.text = $"{nextLevelPrice}";
                if (playerHasMoney)
                    text.color = Color.white;
                else
                    text.color = Color.red;
            }
        }
        public Tuple<UpgradeBase, int> GetNextUpgrade()
        {
            Tuple<UpgradeBase, int> upgradeAndCost;

            int nextLevel = _currentTierId;
            int nextLevelPrice = 0;

            if (nextLevel >= _appliersTiers.Count)
            {
                int levelsAboveList = nextLevel - _appliersTiers.Count - 1;
                nextLevelPrice = _whenEndTier.price + 20 * levelsAboveList;
            }
            else
                nextLevelPrice = _appliersTiers[nextLevel].price;

            UpgradeBase upgToApply = null;

            if (_currentTierId >= _appliersTiers.Count)
                upgToApply = _whenEndTier.GetRandomUpgradeTier();
            else
                upgToApply = _appliersTiers[_currentTierId].GetRandomUpgradeTier();
            upgradeAndCost = new Tuple<UpgradeBase, int>(upgToApply, nextLevelPrice);
            return upgradeAndCost;
        }
        public void ApplyUpgrade(UpgradeBase upg)
        {
            _currentTierId++;
            SaveCurrentTierId();

            //upg.ApplyUpgrade(player);

            UnlockUpgrade(upg);

            PlayerHasMoneyForNextLevel();
            ShowNotificationWhenNextTierAvailable();
        }
        public void LoadCurrentTierVisual()
        {
            UpgradeBase upgToApply = null;

            for (int i = 0; i < _currentTierId; i++)
            {
                if (i >= _appliersTiers.Count) return;
                UpgradeBase upg = _appliersTiers[i].GetRandomUpgradeTier();
                UnlockUpgrade(upg);
            }
        }
        public void UnlockUpgrade(UpgradeBase upg)
        {
            foreach (PerkIcon perkIcon in _perkIcons)
            {
                if (upg == perkIcon.upgrade)
                {
                    perkIcon.onGain.Invoke();
                    perkIcon.SetUpgrade();
                }
            }
        }
        [ContextMenu("Find Perk Icons")]
        public void FindPerkIcons()
        {
            var icons = FindObjectsOfType<PerkIcon>(true);
            _perkIcons.Clear();
            _perkIcons = icons.ToList();
        }
        private void SaveCurrentTierId()
        {
            PlayerPrefs.SetInt("CurrentTierId", _currentTierId);
            PlayerPrefs.Save(); // Optional: ensures immediate save
        }
        // Load the current tier ID from PlayerPrefs
        private void LoadCurrentTierId()
        {
            // Get saved tier ID or default to 0 if not found
            _currentTierId = PlayerPrefs.GetInt("CurrentTierId", 0);
        }
        private bool HasCurrency(int nextLevelPrice)
        {
            return _currency?.GetCurrency() >= nextLevelPrice;
        }
        [Serializable]
        public class PermanentUpgradeTier
        {
            public List<UpgradeBase> upgradeOptions;
            public List<UnityEvent> onUnlock;
            public int price = 10;

            public UpgradeBase GetUpgradeTier(int index)
            {
                return upgradeOptions[index];
            }
            public UpgradeBase GetRandomUpgradeTier()
            {
                return upgradeOptions[UnityEngine.Random.Range(0, upgradeOptions.Count)];
            }
        }
        [Serializable]
        public class PerksTiers
        {
            public TierType tier = TierType.Epic;

            public List<UpgradeBase> upgrades = new List<UpgradeBase>();
            public List<PerkIcon> icons = new List<PerkIcon>();
            public List<TextMeshProUGUI> infoText = new List<TextMeshProUGUI>();
            public List<UnityEvent> onUnlock = new List<UnityEvent>();
        }
        public enum TierType
        {
            Normal,
            Rare,
            Epic,
            Legendary
        }
    }
}
