using MoreMountains.Feedbacks;
using UnityEngine;

namespace SSBRoadmap
{
    public class RoadMapElement : MonoBehaviour
    {
        [Header("States Feels")]
        [SerializeField] private MMF_Player _stateLockedFeel;
        [SerializeField] private MMF_Player _stateNextFeel;
        [SerializeField] private MMF_Player _stateAvailableFeel;
        [SerializeField] private MMF_Player _stateBoughtFeel;

        protected virtual void Start()
        {
            transform.localScale = Vector3.one;
        }

        public virtual void SetLocked(bool loading = false)
        {
            if (loading)
                _stateLockedFeel?.PlayFeedbacks();
        }
        public virtual void SetNext(bool loading = false)
        {
            if (loading)
                _stateNextFeel?.PlayFeedbacks();
        }
        public virtual void SetAvailable(bool loading = false)
        {
            if (loading)
                _stateAvailableFeel?.PlayFeedbacks();
        }
        public virtual void SetBought(bool loading = false)
        {
            if (loading)
                _stateBoughtFeel?.PlayFeedbacks();
        }

    }
}