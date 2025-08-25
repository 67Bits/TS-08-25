using System;
using ObjectPooling;
using UnityEngine;

namespace Ads
{
    public class RewardGenerator : Singleton<RewardGenerator>
    {
        [SerializeField] private GameObject _objectReward;
        public Transform Target;

        internal virtual void Start()
        {
            InterstitialManager.Instance.OnFinishAdEvent.AddListener(CreateObjectReward);
        }

        internal virtual void CreateObjectReward()
        {
            if (_objectReward)
            {
                Instantiate(_objectReward, Target.position, Target.transform.rotation);
            }
        }

        internal virtual void Generate()
        {
            Debug.Log("Gerou recompensas");
        }
    }
}