using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests
{
    public class QuestDeliver : MonoBehaviour
    {
        [SerializeField] private QuestObject _thisQuestObject;
        [SerializeField] private bool _autoSetAvalibleNextQuest;
        [SerializeField] private List<Quest> _linkedQuests;
        [SerializeField, ReadOnly] private List<Quest> _avaliableQuests;
        private Transform _questCanvas;
        private Transform _singleQuestTransform;
        private QuestUI _singleQuestUI;
        private Transform _multipleQuestTransform;
        private QuestUI _multipleQuestUI;

        [SerializeField, FoldoutGroup("Events")] private UnityEvent OnQuestsAvaliable;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent OnNoQuestsAvaliable;
        [SerializeField, FoldoutGroup("Events")] private UnityEvent OnQuestsActive;

        private Quest _lastSelectedQuest;
        private void Awake()
        {
            _questCanvas = QuestManager.Instance.QuestDeliverCanvas;
            _singleQuestTransform = QuestManager.Instance.SingleQuestDeliverTransform;
            _singleQuestUI = QuestManager.Instance.SingleQuestDeliverUI;
            _multipleQuestTransform = QuestManager.Instance.MultipleQuestDeliverTransform;
            _multipleQuestUI = QuestManager.Instance.MultipleQuestDeliverUI;
            _thisQuestObject.IsDelivery = true;
            _thisQuestObject.ThisQuestDeliver = this;
            LinkQuests();
        }
        private void Start()
        {
            UpdateAvaliableQuests();
        }
        public void ShowCurrentQuest()
        {
            _lastSelectedQuest = GetNextAvaliableQuest();
            if (!_lastSelectedQuest)
            {
                _questCanvas.gameObject.SetActive(false);
                Time.timeScale = 1;
                OnNoQuestsAvaliable?.Invoke();
                return;
            }
            switch (_lastSelectedQuest.QuestType)
            {
                case Quest_Type.SingleObjective:
                case Quest_Type.DirectProgression:
                    _multipleQuestTransform.gameObject.SetActive(false);
                    _singleQuestTransform.gameObject.SetActive(true);
                    _singleQuestUI.CurrentDeliver = this;
                    _ = _singleQuestUI.SetQuest(_lastSelectedQuest);
                    break;
                case Quest_Type.MultipleQuests:
                    _lastSelectedQuest.ActiveLinkedQuests();
                    for (int i = 0; i < _lastSelectedQuest.LinkedQuests.Count; i++)
                    {
                        Quest quest = _lastSelectedQuest.LinkedQuests[i];
                        if (_lastSelectedQuest.FollowLinkListOrder && quest.Status == Quest_Status.Active) break;
                        if (quest.Status == Quest_Status.Avaliable)
                        {
                            quest.Active();
                            break;
                        }
                    }
                    _singleQuestTransform.gameObject.SetActive(false);
                    _multipleQuestTransform.gameObject.SetActive(true);
                    _multipleQuestUI.CurrentDeliver = this;
                    _ = _multipleQuestUI.SetQuest(_lastSelectedQuest);
                    break;
            }
            _questCanvas.gameObject.SetActive(true);
            Time.timeScale = 0;
            OnQuestsActive?.Invoke();
        }
        private Quest GetNextAvaliableQuest()
        {
            var quest = GetNextQuest();
            return quest && (quest.Status == Quest_Status.Avaliable || quest.Status == Quest_Status.Active) ?
                quest : null;
        }
        private Quest GetNextQuest()
        {
            for (int i = 0; i < _linkedQuests.Count; i++)
            {
                Quest quest = _linkedQuests[i];
                if (quest.Status == Quest_Status.Complete || quest.Status == Quest_Status.Fail)
                {
                    _linkedQuests.Remove(quest);
                    i--;
                    continue;
                }
                else
                {
                    if (quest.CurrentValue == quest.TotalValue)
                        return quest;
                }
            }
            if (_linkedQuests.Count > 0) return _linkedQuests[0];
            return null;
        }
        public void SetAvaliableNextQuest()
        {
            var newQuest = GetNextQuest();
            newQuest?.SetAvaliable();
            _questCanvas.gameObject.SetActive(newQuest);
        }
        private void LinkQuests()
        {
            for (int i = 0; i < _linkedQuests.Count; i++)
            {
                Quest quest = _linkedQuests[i];
                quest.UpdateAllDeliveries(_thisQuestObject);
                quest.IsAvaliable.AddListener(UpdateAvaliableQuests);
                quest.OnAvaliable.AddListener(UpdateAvaliableQuests);
                quest.OnComplete.AddListener(OnCompleteQuest);
            }
        }
        private void UnlinkQuests()
        {
            for (int i = 0; i < _linkedQuests.Count; i++)
            {
                Quest quest = _linkedQuests[i];
                quest.IsAvaliable.RemoveListener(UpdateAvaliableQuests);
                quest.OnAvaliable.RemoveListener(UpdateAvaliableQuests);
                quest.OnComplete.RemoveListener(OnCompleteQuest);
            }
        }
        public void UpdateAvaliableQuests()
        {
            for (int i = 0; i < _linkedQuests.Count; i++)
            {
                Quest quest = _linkedQuests[i];
                if (quest.Status == Quest_Status.Avaliable && !_avaliableQuests.Contains(quest))
                {
                    _avaliableQuests.Add(quest);
                }
            }
            for (int i = 0; i < _avaliableQuests.Count; i++)
            {
                Quest quest = _avaliableQuests[i];
                if (quest.Status != Quest_Status.Avaliable)
                {
                    _avaliableQuests.Remove(quest);
                    if (quest.Status != Quest_Status.Active && !_linkedQuests.Contains(quest))
                        _linkedQuests.Add(quest);
                }
            }
            if (_avaliableQuests.Count > 0)
                OnQuestsAvaliable?.Invoke();
            else
            {
                var nextQuest = GetNextQuest();
                if (nextQuest && nextQuest.Status == Quest_Status.Active)
                    OnQuestsActive?.Invoke();
                else OnNoQuestsAvaliable?.Invoke();
            }
        }
        private void OnCompleteQuest()
        {
            UpdateAvaliableQuests();
            if (_autoSetAvalibleNextQuest) SetAvaliableNextQuest();
            else _questCanvas.gameObject.SetActive(false);
        }
        private void OnEnable() => LinkQuests();
        private void OnDisable() => UnlinkQuests();
    }
}
