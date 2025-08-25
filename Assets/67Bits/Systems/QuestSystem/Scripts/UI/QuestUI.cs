using Bello.Unity;
using Mono.CSharp;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SSB.Quests
{
    public class QuestUI : MonoBehaviour
    {
        [field: ReadOnly, SerializeField] public Quest targetQuest;
        [SerializeField] float taskInitDelay;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _focusButton;
        #region Progress
        [Header("Progress")]
        [FoldoutGroup("Progress"),SerializeField] private Transform _progressBarParent;
        [FoldoutGroup("Progress"),SerializeField] private Slider _progressBarSlider;
        [FoldoutGroup("Progress"),SerializeField] private Image _progressBarImage;
        [FoldoutGroup("Progress"),SerializeField] private TextMeshProUGUI _progressText;
        [FoldoutGroup("Progress"),SerializeField, Header("Progress Icon")] private bool _useIcon;
        [FoldoutGroup("Progress"),SerializeField, ShowIf("@_useIcon")] private Transform _progressIconParent;
        [FoldoutGroup("Progress"),SerializeField, ShowIf("@_useIcon")] private ProgressIconUI _progressIconPrefab;
        #endregion
        [SerializeField, FoldoutGroup("QuestEvents")] private UnityEvent IsDisabled;
        [SerializeField, FoldoutGroup("QuestEvents")] private UnityEvent IsActive;
        [SerializeField, FoldoutGroup("QuestEvents")] private UnityEvent IsComplete;

        #region Reward
        [SerializeField] private bool hasReward;
        [ShowIf(@"hasReward"), SerializeField, Required] private Transform _rewardParent;
        [ShowIf(@"hasReward"), SerializeField, Required] private RewardUI _rewardPrefab;
        [ShowIf(@"hasReward"), SerializeField, Required] private Button _claimButton;
        [ShowIf(@"hasReward"), SerializeField, FoldoutGroup("RewardEvents")] private UnityEvent IsRewardDisabled;
        [ShowIf(@"hasReward"), SerializeField, FoldoutGroup("RewardEvents")] private UnityEvent IsRewardReady;
        [ShowIf(@"hasReward"), SerializeField, FoldoutGroup("RewardEvents")] private UnityEvent IsRewardClaimed;
        #endregion
        #region Linked Quests
        [SerializeField] private bool showLinkedQuests;
        [ShowIf(@"showLinkedQuests"), SerializeField, Required] private Transform _linkedQuestParent;
        [ShowIf(@"showLinkedQuests"), SerializeField, Required] private QuestUI _linkedQuestUIPrefab;
        #endregion
        #region Feels
        [SerializeField] private bool useFeels;
        [FoldoutGroup("Feels"), SerializeField, ShowIf(@"useFeels")] private MMF_Player _onNewObjetive;
        [FoldoutGroup("Feels"), SerializeField, ShowIf(@"useFeels")] private MMF_Player _onCompleteObjetive;
        #endregion
        [SerializeField] private bool _unlinkOnDisable;
        [field: SerializeField] public Button ActiveQuestButton { get; private set; }
        public QuestDeliver CurrentDeliver { get; set; }

        private void Awake()
        {
            _focusButton?.onClick.AddListener(FocusOnObjective);
            ActiveQuestButton?.onClick.AddListener(ActiveQuest);
        }
        public void FocusOnObjective() => targetQuest?.FocusOnQuestObject(true);
        public void ActiveQuest()
        {
            if (targetQuest)
            {
                targetQuest.Active();
            }
        }
        public async Awaitable SetQuest(Quest quest, float? delay = null)
        {
            var newDelay = delay ?? taskInitDelay;
            if (newDelay > 0)
                await Awaitable.WaitForSecondsAsync(newDelay);
            if (!this) return;
            targetQuest = quest;
            RemoveQuestListeners();
            AddQuestListeners();
            UpdateUI();
            if (hasReward && _claimButton)
            {
                targetQuest.IsRewardReady.RemoveListener(EnableRewardButton);
                targetQuest.IsRewardReady.AddListener(EnableRewardButton);
                _claimButton.interactable = false;
                _claimButton.onClick.RemoveListener(GetReward);
                _claimButton.onClick.AddListener(GetReward);
            }
            if (useFeels)
            {
                targetQuest.OnComplete.RemoveListener(_onCompleteObjetive.PlayFeedbacks);
                targetQuest.OnComplete.AddListener(_onCompleteObjetive.PlayFeedbacks);
                if (targetQuest.Status == Quest_Status.Complete) _onCompleteObjetive?.PlayFeedbacks();
                else _onNewObjetive?.PlayFeedbacks();
            }
            CallQuestEvents();
        }
        private void SetProgressIcon(in Quest quest)
        {
            if (!_progressBarParent || !_progressIconParent) return;
            if (quest.ProgressIcon && _progressIconParent)
            {
                _progressBarParent.gameObject.SetActive(false);
                _progressIconParent.gameObject.SetActive(true);
                _progressIconParent.DestroyAllChildern();
                for (int i = 1; i <= targetQuest.TotalValue; i++)
                {
                    var newIcon = Instantiate(_progressIconPrefab, _progressIconParent);
                    newIcon.transform.localScale = Vector3.one;
                    newIcon.SetIcon(quest.ProgressIcon, i <= quest.CurrentValue);
                }
            }
            else
            {
                _progressBarParent.gameObject.SetActive(true);
                _progressIconParent.gameObject.SetActive(false);
            }
        }
        private void EnableRewardButton() => _claimButton.interactable = true;
        private void GetReward()
        {
            targetQuest.ClaimRewards();
            _claimButton.interactable = false;
            IsRewardClaimed?.Invoke();
            if (targetQuest.Status != Quest_Status.Complete)
            {
                targetQuest.Complete(true);
                if (CurrentDeliver)
                {
                    CurrentDeliver.ShowCurrentQuest();
                    CurrentDeliver = null;
                }
            }
            UpdateUI();
        }
        private void CallQuestEvents()
        {
            switch (targetQuest.Status)
            {
                case Quest_Status.Inative:
                    IsDisabled?.Invoke();
                    break;
                case Quest_Status.Active:
                    IsActive?.Invoke();
                    break;
                case Quest_Status.Complete:
                    IsComplete?.Invoke();
                    break;
            }
            if (hasReward)
            {
                if (_claimButton)
                    _claimButton.interactable = false;
                switch (targetQuest.RewardStatus)
                {
                    case Reward_Status.Waiting:
                        IsRewardDisabled?.Invoke();
                        break;
                    case Reward_Status.Ready:
                        IsRewardReady?.Invoke();
                        if (_claimButton)
                            EnableRewardButton();
                        break;
                    case Reward_Status.Claimed:
                        IsRewardClaimed?.Invoke();
                        break;
                }
            }
        }
        public void UpdateUI()
        {
            if (_icon) _icon.sprite = targetQuest.Icon;

            if (_titleText) _titleText.text = targetQuest.Title;

            if (_descriptionText) _descriptionText.text = targetQuest.Description;

            if (_progressBarImage) _progressBarImage.fillAmount = (float)targetQuest.CurrentValue / targetQuest.TotalValue;
            if (_progressBarSlider) _progressBarSlider.value = (float)targetQuest.CurrentValue / targetQuest.TotalValue;
            SetProgressIcon(targetQuest);

            if (_progressText) _progressText.text = $"{targetQuest.CurrentValue}/{targetQuest.TotalValue}";

            if (hasReward)
            {
                _rewardParent.DestroyAllChildern();
                for (int i = 0; i < targetQuest.Rewards.Count; i++)
                {
                    QuestReward reward = targetQuest.Rewards[i];
                    var newReward = Instantiate(_rewardPrefab, _rewardParent);
                    newReward.transform.localScale = Vector3.one;
                    newReward.SetReward(reward);
                }
            }
            if (showLinkedQuests)
            {
                _linkedQuestParent.DestroyAllChildern();
                for (int i = 0; i < targetQuest.LinkedQuests.Count; i++)
                {
                    var quest = targetQuest.LinkedQuests[i];
                    var newQuestUI = Instantiate(_linkedQuestUIPrefab, _linkedQuestParent);
                    newQuestUI.transform.localScale = Vector3.one;
                    newQuestUI.SetQuest(quest);
                }
            }
            CallQuestEvents();
        }
        private void RemoveQuestListeners()
        {
            if (targetQuest)
            {
                targetQuest.OnUpdateProgression.RemoveListener(UpdateUI);
                targetQuest.IsRewardReady.RemoveListener(UpdateUI);
                targetQuest.OnComplete.RemoveListener(UpdateUI);
                targetQuest.OnActive.RemoveListener(UpdateUI);
            }
        }
        private void AddQuestListeners()
        {
            if (targetQuest)
            {
                targetQuest.OnUpdateProgression.AddListener(UpdateUI);
                targetQuest.IsRewardReady.AddListener(UpdateUI);
                targetQuest.OnComplete.AddListener(UpdateUI);
                targetQuest.OnActive.AddListener(UpdateUI);
            }
        }
        private void OnEnable()
        {
            if (useFeels)
            {
                targetQuest?.OnComplete.RemoveListener(_onCompleteObjetive.PlayFeedbacks);
                targetQuest?.OnComplete.AddListener(_onCompleteObjetive.PlayFeedbacks);
            }
            AddQuestListeners();
        }
        private void OnDisable()
        {
            CurrentDeliver = null;
            if (!_unlinkOnDisable) return;
            if (useFeels)
                targetQuest?.OnComplete.RemoveListener(_onCompleteObjetive.PlayFeedbacks);
            RemoveQuestListeners();
        }
        private void OnDestroy()
        {
            if (useFeels)
                targetQuest?.OnComplete.RemoveListener(_onCompleteObjetive.PlayFeedbacks);
            RemoveQuestListeners();
        }
    }
}
