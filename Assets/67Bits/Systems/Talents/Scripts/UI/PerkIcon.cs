using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Talents
{
    public class PerkIcon : MonoBehaviour
    {
        [SerializeField] private Image iconToChange = null;
        public UpgradeBase upgrade;
        public int upgInt = -1;
        public Button onClick = null;
        public UnityEvent onGain = null;
        public TextMeshProUGUI text;

        public void ChangeIcon(Sprite sprite)
        {
            iconToChange.sprite = sprite;
        }
        public void SetUpgrade()
        {
            onClick.onClick.RemoveAllListeners();
            ChangeIcon(upgrade.upgradeImage);

            onClick.onClick.AddListener(() =>
            {
                //UIManager.Instance.UpdatePerkInfoTab(upgrade);
            });

            switch (upgInt)
            {
                case 0:
                    text.text = $"{Mathf.Round(PlayerUpgrader.Instance.damagePermanentFactor * 100)}%";
                    break;
                case 1:
                    text.text = $"{Mathf.Round(PlayerUpgrader.Instance.criticalHitChancePermanet * 100)}%";
                    break;
                case 2:
                    text.text = $"{Mathf.Round(PlayerUpgrader.Instance.attackSpeedPermanentFactor * 100)}%";
                    break;
            }
        }
    }
}