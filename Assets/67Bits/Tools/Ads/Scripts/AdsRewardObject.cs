using UnityEngine;
using UnityEngine.Events;

namespace Ads
{
    public class AdsRewardObject : MonoBehaviour
    {
        [SerializeField] private UnityEvent _playerInteracted;

        public virtual void Interact()
        {
            GenerateRewards();
            _playerInteracted?.Invoke();
        }

        internal virtual void GenerateRewards()
        {
            RewardGenerator.Instance?.Generate();
        }
    }
}
