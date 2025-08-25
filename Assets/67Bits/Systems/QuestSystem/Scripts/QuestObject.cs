using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests
{
    public class QuestObject : MonoBehaviour
    {
        [SerializeField] private Quest[] _deliverTarget;
        [Space(5), SerializeField] private bool _progressOnDisable;
        [SerializeField] private ObjectiveProgression[] _objectives;
        [SerializeField] private QuestProgression[] _quests;
        public UnityEvent _onFocus;

        [ReadOnly] public bool IsDelivery;
        [ReadOnly] public QuestDeliver ThisQuestDeliver;
        [SerializeField, ReadOnly] private bool _linked;
        private void Awake() => LinkToObjectives();
        public void Focus() => _onFocus?.Invoke();
        public void UpdateProgression()
        {
            for (int i = 0; i < _objectives.Length; i++)
            {
                var objectiveProgress = _objectives[i]; ;
                objectiveProgress.Objective.UpdateProgression(objectiveProgress.Value);
            }
            for (int i = 0; i < _quests.Length; i++)
            {
                var questProgress = _quests[i]; ;
                questProgress.Quest.UpdateProgression(questProgress.Value);
            }
        }
        public void LinkToObjectives()
        {
            if (_linked) return;
            _linked = true;
            if (!IsDelivery && _deliverTarget.Length > 0) IsDelivery = true;
            for (int i = 0; i < _deliverTarget.Length; i++)
            {
                Quest quest = _deliverTarget[i];
                quest.UpdateAllDeliveries(this);
            }
            for (int i = 0; i < _objectives.Length; i++)
            {
                var objectiveProgress = _objectives[i]; ;
                if (!objectiveProgress.Objective.LinkedObjects.Contains(this))
                    objectiveProgress.Objective.LinkedObjects.Add(this);
            }
            for (int i = 0; i < _quests.Length; i++)
            {
                Quest _quest = _quests[i].Quest;
                if (!_quest.LinkedObjects.Contains(this))
                    _quest.LinkedObjects.Add(this);
            }
        }
        public void LinkToObjective(Objective objective)
        {
            if (!objective.LinkedObjects.Contains(this))
                objective.LinkedObjects.Add(this);
        }
        public void RemoveFromObjectives()
        {
            _linked = false;
            for (int i = 0; i < _objectives.Length; i++)
            {
                var objectiveProgress = _objectives[i]; ;
                objectiveProgress.Objective.LinkedObjects.Remove(this);
            }
            for (int i = 0; i < _quests.Length; i++)
            {
                Quest quest = _quests[i].Quest;
                quest.LinkedObjects.Remove(this);
            }
        }
        public void RemovefromObjective(Objective objective) => objective.LinkedObjects.Remove(this);
        private void OnEnable() => LinkToObjectives();
        private void OnDisable()
        {
            RemoveFromObjectives();
            try
            {
                if (_progressOnDisable)
                    UpdateProgression();
            }
            catch (Exception ex)
            { Debug.Log(gameObject.name + "- Coldn't update progression because: " + ex); }
        }
        [Serializable]
        private class QuestProgression
        {
            [HorizontalGroup] public Quest Quest;
            [HorizontalGroup] public int Value;
        }
        [Serializable]
        private class ObjectiveProgression
        {
            [HorizontalGroup] public Objective Objective;
            [HorizontalGroup] public int Value;
        }
    }
}