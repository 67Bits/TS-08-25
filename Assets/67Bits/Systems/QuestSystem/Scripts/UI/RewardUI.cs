using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSB.Quests
{
    public class RewardUI : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;
        public void SetReward(QuestReward reward)
        {
            _icon.sprite = reward.Reward.Icon;
            _valueText.text = $"x{reward.Ammount}";
        }
    }
}
