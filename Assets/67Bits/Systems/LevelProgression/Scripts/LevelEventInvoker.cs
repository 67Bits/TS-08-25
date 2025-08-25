using UnityEngine;
using UnityEngine.Events;

namespace SSBLevelProgression
{
    [System.Serializable]
    public class LevelEventInvoker
    {
        [SerializeField] private LevelEvent _eventData;
        [SerializeField] private int _pointsToAppear;
        [SerializeField] private UnityEvent _onAchieve;
        [SerializeField] private bool _drawEvent = true;
        private bool _reached;

        public GameObject IconPrefab => _eventData.IconPrefab;
        public bool DrawEvent => _drawEvent;

        public int PointsToAppear => _pointsToAppear;
        public bool Reached
        {
            get => _reached;
            set
            {
                _reached = value;
                if (_reached)
                {
                    _eventData.AchieveEvent();
                }
            }
        }
    }

}