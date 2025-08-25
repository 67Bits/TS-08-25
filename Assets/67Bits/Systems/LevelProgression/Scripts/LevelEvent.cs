using System.Collections.Generic;
using UnityEngine;

namespace SSBLevelProgression
{
    [CreateAssetMenu(fileName = "LevelEvent", menuName = "67Bits/LevelEvent")]
    public class LevelEvent : ScriptableObject
    {
        [SerializeField] private GameObject _iconPrefab;
        private List<LevelEventWatcher> _eventWatchers = new List<LevelEventWatcher>();

        public GameObject IconPrefab => _iconPrefab;

        public void AddWatcher(LevelEventWatcher watcher)
        {
            _eventWatchers.Add(watcher);
        }

        public void RemoveWatcher(LevelEventWatcher watcher)
        {
            _eventWatchers.Remove(watcher);
        }

        public void AchieveEvent()
        {
            for (int i = 0; i < _eventWatchers.Count; i++)
            {
                _eventWatchers[i].OnEventAchieved();
            }
        }
    }
}