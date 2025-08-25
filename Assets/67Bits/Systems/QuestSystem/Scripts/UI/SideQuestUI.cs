using Bello.Unity;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests
{
    public class SideQuestUI : MonoBehaviour
    {
        [SerializeField] private Transform _sideQuestUIParent;
        [SerializeField] private QuestUI _sideQuestUIPrefab;

        [SerializeField] private bool _showOnStart = true;
        [SerializeField] private bool _showOnNewQuest = true;
        [SerializeField, ReadOnly] private bool _isUIVisible = true;
        public UnityEvent OnHideUI;
        public UnityEvent OnShowUI;

        List<QuestUI> questUIs = new();

        private void Awake() => _sideQuestUIParent.DestroyAllChildern();
        public void Initialize()
        {
            for (int i = 0; i < QuestManager.Instance.SideQuests.Count; i++)
            {
                Quest quest = QuestManager.Instance.SideQuests[i];
                if (quest.QuestType != Quest_Type.MultipleQuests || quest.ShowInUI)
                {
                    void StartSideQuest() => AddNewQuest(quest);
                    quest.OnActive.RemoveListener(StartSideQuest);
                    quest.OnActive.AddListener(StartSideQuest);
                }
            }
            if (!_isUIVisible && _showOnStart) TogleUI();
        }
        public void AddNewQuest(in Quest newQuest)
        {
            var newUI = Instantiate(_sideQuestUIPrefab, _sideQuestUIParent);
            _ = newUI.SetQuest(newQuest);
            newQuest.OnComplete.AddListener(OnCompleteQuest);
            questUIs.Add(newUI);
            if (_showOnNewQuest && !_isUIVisible)
                TogleUI();
        }
        private void OnCompleteQuest()
        {
            for (int i = 0; i < questUIs.Count; i++)
            {
                QuestUI questUI = questUIs[i];
                if (questUI.targetQuest.Status == Quest_Status.Complete)
                {
                    questUI.targetQuest.OnComplete.RemoveListener(OnCompleteQuest);
                    questUIs.Remove(questUI);
                    DestroyAfter(questUI.gameObject, 3);
                }
            }
        }
        private async void DestroyAfter(GameObject toDestroy, float time)
        {
            await Awaitable.WaitForSecondsAsync(time);
            Destroy(toDestroy);
        }
        public void TogleUI()
        {
            _isUIVisible = !_isUIVisible;
            if (_isUIVisible)
                ShowUI();
            else HideUI();
        }
        private void ShowUI() => OnShowUI?.Invoke();
        private void HideUI() => OnHideUI?.Invoke();
    }
}
