using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ReadOnly = Bello;
using UnityEditor;

namespace SSB.Quests
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "67Bits/Quest/Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [VerticalGroup("Title/Right"), Required, GUIColor(1f, 1f, 0f), LabelWidth(50)] public string Title;
        [HorizontalGroup("Title", .2f), Required, PreviewField(75f, ObjectFieldAlignment.Right), HideLabel] public Sprite Icon;
        [field: ReadOnly, SerializeField, VerticalGroup("Title/Right"), LabelWidth(50)] public string Id { get; private set; }
        [field: SerializeField, Space(15)] public Quest_Type QuestType { get; private set; }

        [field: ShowIf("@QuestType == Quest_Type.MultipleQuests"), SerializeField] public List<Quest> LinkedQuests { get; private set; } = new();
        [field: ShowIf("@QuestType == Quest_Type.MultipleQuests"), SerializeField, HorizontalGroup("LinkedQuestSettings")]
        [field: LabelWidth(150)] public bool FollowLinkListOrder { get; private set; } = new();
        [field: ShowIf("@QuestType == Quest_Type.MultipleQuests"), SerializeField, HorizontalGroup("LinkedQuestSettings")]
        [field: LabelWidth(100)] public bool ShowInUI { get; private set; } = new();
        [field: ShowIf("@QuestType == Quest_Type.MultipleQuests"), SerializeField, HorizontalGroup("LinkedQuestSettings")]
        [field: LabelWidth(100)] public bool AutoActive { get; private set; } = true;
        [field:HorizontalGroup("Objective", .65f), ShowIf("@QuestType == Quest_Type.SingleObjective"), SerializeField] public Objective Objective { get; private set; }
        [field:HorizontalGroup("Objective"), PreviewField(25), SerializeField, LabelWidth(150)] public Sprite ProgressIcon { get; private set; }
        [field: HorizontalGroup("Values"), LabelWidth(100), ReadOnly, SerializeField] public int CurrentValue { get; private set; }
        [field: HorizontalGroup("Values"), LabelWidth(100), SerializeField] public int TotalValue { get; private set; }
        [field: SerializeField, ReadOnly, HorizontalGroup("Status", .35f), LabelWidth(50)] public Quest_Status Status { get; private set; }
        [field: SerializeField, ReadOnly, HorizontalGroup("Status"), LabelWidth(100)] public Reward_Status RewardStatus { get; private set; }

        [TextArea(2, 5)] public string Description;

        [Tooltip("Should the quest progression be saved every time or just when the quest is completed")]
        [field: SerializeField, HorizontalGroup("Boll Settings"), LabelWidth(150)] public bool SaveOnAnyProgress { get; private set; } = true;
        [field: SerializeField, LabelWidth(175), HorizontalGroup("Boll Settings")] public bool RequireActivateToComplete { get; private set; }
        [field: SerializeField, HorizontalGroup("Focus", .4f), LabelWidth(115)] public bool FocusOnObjective { get; private set; } = true;
        [field: SerializeField, HorizontalGroup("Focus", .25f), LabelWidth(75), ShowIf("@FocusOnObjective")] public bool StartFocus { get; private set; } = false;
        [field: SerializeField, HorizontalGroup("Focus2"), LabelWidth(100), ShowIf("@FocusOnObjective")] public bool ProgressFocus { get; private set; } = false;
        [field: SerializeField, HorizontalGroup("Focus2"), LabelWidth(100), ShowIf("@FocusOnObjective")] public bool EndFocus { get; private set; } = false;
        [field: SerializeField, LabelWidth(150), HorizontalGroup("Deliver")] public bool HasDeliverTarget { get; private set; }
        [field: SerializeField, LabelWidth(150), HorizontalGroup("Deliver")] public bool AutoFocusOnDeliver { get; private set; }
        [LabelWidth(200), ShowIf("@HasDeliverTarget"), ReadOnly] public QuestObject DeliverTarget;

        [field: Header("Rewards")]
        [field: SerializeField] public bool AutoClaimReward { get; private set; }

        [field: SerializeField] public List<QuestReward> Rewards { get; private set; } = new();

        [SerializeField, ReadOnly] private int _eventsCount;
        [FoldoutGroup("Events")] public UnityEvent IsDisable;
        [FoldoutGroup("Events")] public UnityEvent OnUpdateProgression;
        [FoldoutGroup("Events/Avaliable")] public UnityEvent OnAvaliable;
        [FoldoutGroup("Events/Avaliable")] public UnityEvent IsAvaliable;
        private bool _linked;
        [FoldoutGroup("Events/Active")] public UnityEvent OnActive;
        [FoldoutGroup("Events/Active")] public UnityEvent IsActive;
        [FoldoutGroup("Events/Fail")] public UnityEvent OnFail;
        [FoldoutGroup("Events/Fail")] public UnityEvent IsFailed;
        [FoldoutGroup("Events/Complete")] public UnityEvent OnComplete;
        [FoldoutGroup("Events/Complete")] public UnityEvent IsComplete;


        [FoldoutGroup("Events/Reward")] public UnityEvent IsRewardReady;
        [FoldoutGroup("Events/Reward")] public UnityEvent OnClaimReward;

        [FoldoutGroup("Events/UI Events")] public UnityEvent OnFocus;
        public List<QuestObject> LinkedObjects { get; set; } = new();

        #region Quest Progression
        [Button(ButtonSizes.Large), HorizontalGroup("Change Status")]
        public void Active()
        {
            if (Status == Quest_Status.Complete || Status == Quest_Status.Fail) return;
            IsActive?.Invoke();
            if (!_linked)
            {
                LinkObjective();
                LinkQuests();
                _linked = true;
            }
            if (!QuestManager.Instance.ActiveQuests.Contains(this))
                QuestManager.Instance.ActiveQuests.Add(this);
            if (Status != Quest_Status.Inative && Status != Quest_Status.Avaliable) return;

            Status = Quest_Status.Active;
            DeliverTarget?.ThisQuestDeliver?.UpdateAvaliableQuests();
            ActiveLinkedQuests();
            OnActive?.Invoke();

            if (FocusOnObjective && StartFocus)
                FocusOnQuestObject(true);

            CheckProgression();
            Save();
        }
        [Button(ButtonSizes.Large), HorizontalGroup("Change Status")]
        public void Complete() => Complete(true);
        public void Complete(bool force = false)
        {
            if (!force && CurrentValue < TotalValue) return;
            IsComplete?.Invoke();
            DeliverTarget?.ThisQuestDeliver?.UpdateAvaliableQuests();
            if (RewardStatus != Reward_Status.Claimed) IsRewardReady?.Invoke();
            if (!force)
                if (RequireActivateToComplete && Status != Quest_Status.Active ||
                    Status == Quest_Status.Complete) return;

            if (RewardStatus != Reward_Status.Claimed)
            {
                if (AutoClaimReward) ClaimRewards();
                else
                {
                    RewardStatus = Reward_Status.Ready;
                    if (EndFocus && HasDeliverTarget && AutoFocusOnDeliver)
                        FocusOnQuestObject(true);
                }
            }

            CurrentValue = TotalValue;
            Status = Quest_Status.Complete;
            Save();
            OnComplete?.Invoke();
        }
        [Button(ButtonSizes.Large), HorizontalGroup("Change Status")]
        public void Fail()
        {
            OnFail?.Invoke();
            if (Status != Quest_Status.Active) return;
            Status = Quest_Status.Fail;
            IsFailed?.Invoke();
            Save();
        }
        [Button(ButtonSizes.Large), HorizontalGroup("Change Status")]
        public void Disable()
        {
            if (Status == Quest_Status.Complete) return;
            Status = Quest_Status.Inative;
            IsDisable?.Invoke();
            Save();
        }
        [Button(ButtonSizes.Large), HorizontalGroup("Progression", .25f), PropertySpace(5)]
        public void SetAvaliable()
        {
            IsAvaliable?.Invoke();
            if (Status != Quest_Status.Inative) return;
            Status = Quest_Status.Avaliable;
            DeliverTarget?.ThisQuestDeliver?.UpdateAvaliableQuests();
            OnAvaliable?.Invoke();
            Save();
        }
        [Button(ButtonSizes.Large), HorizontalGroup("Progression"), PropertySpace(5)]
        public void AddProgression() => UpdateProgression(1);
        public void UpdateProgression(int valueToAdd = 1)
        {
            if (QuestType != Quest_Type.DirectProgression && Status != Quest_Status.Active) return;

            if (QuestType == Quest_Type.MultipleQuests)
                CurrentValue = LinkedQuests.Where((x) => x.Status == Quest_Status.Complete).ToArray().Length;
            else CurrentValue += valueToAdd;
            CurrentValue = Mathf.Clamp(CurrentValue, 0, TotalValue);

            OnUpdateProgression?.Invoke();
            if (SaveOnAnyProgress) Save();
            if (CurrentValue >= TotalValue)
            {
                if (RewardStatus != Reward_Status.Claimed)
                {
                    RewardStatus = Reward_Status.Ready;
                    IsRewardReady?.Invoke();
                }
                if ((!RequireActivateToComplete || Status == Quest_Status.Active) && !HasDeliverTarget)
                {
                    Complete(false);
                    Save();
                    TrackPath.Instance?.QuestObjects.Clear();
                }
            }
            if (FocusOnObjective && ProgressFocus && valueToAdd > 0) FocusOnQuestObject(AutoFocusOnDeliver, true);

        }
        public void CheckProgression()
        {
            ActiveLinkedQuests();
            UpdateProgression(0);
        }
        #endregion
        public void LinkObjective()
        {
            if (QuestType != Quest_Type.SingleObjective) return;
            Objective?.LinkedQuests.Add(this);
        }
        public void LinkQuests()
        {
            if (QuestType != Quest_Type.MultipleQuests) return;
            for (int i = 0; i < LinkedQuests.Count; i++)
            {
                var quest = LinkedQuests[i];
                quest.OnComplete.RemoveListener(CheckProgression);
                quest.OnComplete.AddListener(CheckProgression);
            }
        }
        public void UpdateAllDeliveries(QuestObject newDelivery)
        {
            if (!DeliverTarget)
                DeliverTarget = newDelivery;
            if (QuestType != Quest_Type.MultipleQuests) return;
            for (int i = 0; i < LinkedQuests.Count; i++)
            {
                Quest quest = LinkedQuests[i];
                quest.UpdateAllDeliveries(newDelivery);
            }
        }
        public void ActiveLinkedQuests(bool loading = false)
        {
            if (!AutoActive) return;
            if (QuestType == Quest_Type.MultipleQuests)
                for (int i = 0; i < LinkedQuests.Count; i++)
                {
                    Quest quest = LinkedQuests[i];
                    if (loading)
                        quest.Load();
                    if (quest.Status == Quest_Status.Inative ||
                        quest.Status == Quest_Status.Avaliable ||
                        quest.Status == Quest_Status.Active)
                    {
                        quest.Active();
                        if (FollowLinkListOrder) break;
                    }
                }
        }
        private void Save()
        {
            PlayerPrefs.SetString("Quest", "");
            var saveData = new SaveQuestData((int)Status, (int)RewardStatus, CurrentValue);
            SaveQuest.SaveData(saveData, $"{name}: {Id}");
        }
        public void Load()
        {
            var saveData = SaveQuest.LoadData<SaveQuestData>($"{name}: {Id}");
            if (saveData != null)
            {
                Status = (Quest_Status)saveData.Status;
                RewardStatus = (Reward_Status)saveData.RewardStatus;
                CurrentValue = saveData.CurrentValue;
                switch (Status)
                {
                    case Quest_Status.Avaliable:
                        SetAvaliable();
                        DeliverTarget?.ThisQuestDeliver?.UpdateAvaliableQuests();
                        break;
                    case Quest_Status.Active:
                        Active();
                        DeliverTarget?.ThisQuestDeliver?.UpdateAvaliableQuests();
                        ActiveLinkedQuests(true);
                        UpdateProgression(0); // Finishes the quest if the value is already completed
                        break;
                    case Quest_Status.Inative: Disable(); break;
                    case Quest_Status.Complete: Complete(false); break;
                }
            }
            else ResetData();
        }
        public QuestObject GetCloserObject(in Vector3 position)
        {
            if (CurrentValue >= TotalValue)
            {
                if (RewardStatus == Reward_Status.Ready && HasDeliverTarget) return DeliverTarget;
                else return null;
            }
            if (Status == Quest_Status.Avaliable && HasDeliverTarget)
                return DeliverTarget;
            var distance = 0f;
            QuestObject target = null;
            if (QuestType == Quest_Type.MultipleQuests)
            {
                for (int i = 0; i < LinkedQuests.Count; i++)
                {
                    var quest = LinkedQuests[i];
                    if (quest.Status == Quest_Status.Avaliable || quest.Status == Quest_Status.Active || quest.RewardStatus == Reward_Status.Ready)
                    {
                        if (quest.QuestType == Quest_Type.MultipleQuests)
                        {
                            Debug.LogWarning($"The quest {name} is searching on another quest with multiple targets\n" +
                                $"Please check the quest {quest.name}");
                        }
                        return quest.GetCloserObject(position);
                    }
                }
            }
            for (int i = 0; i < LinkedObjects.Count; i++)
            {
                QuestObject questObj = LinkedObjects[i];
                var currentDistance = Vector3.Distance(questObj.transform.position, position);
                if (i == 0 || currentDistance < distance)
                {
                    distance = currentDistance;
                    target = questObj;
                }
            }
            if (QuestType == Quest_Type.SingleObjective && Objective)
            {
                for (int i = 0; i < Objective.LinkedObjects.Count; i++)
                {
                    QuestObject questObj = Objective.LinkedObjects[i];
                    var currentDistance = Vector3.Distance(questObj.transform.position, position);
                    if (i == 0 || currentDistance < distance)
                    {
                        distance = currentDistance;
                        target = questObj;
                    }
                }
            }
            return target?? DeliverTarget;
        }
        public void ClaimRewards()
        {
            try
            {
                if (RewardStatus == Reward_Status.Claimed) return;
                RewardStatus = Reward_Status.Claimed;
                for (int i = 0; i < Rewards.Count; i++)
                {
                    QuestReward item = Rewards[i];
                    item.Reward.GetReward(item.Ammount);
                }
                OnClaimReward?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.Log($"<color=yellow>Fail to claim rewards</color>!");
                Debug.LogException(ex);
            }
        }
        public void FocusOnQuestObject(bool shouldFocusOnDelivery) => FocusOnQuestObject(shouldFocusOnDelivery, false);
        public void FocusOnQuestObject(bool shouldFocusOnDelivery, bool force = false)
        {
            if (Status == Quest_Status.Complete || Status == Quest_Status.Inative ||Status == Quest_Status.Fail) return;
            if (FocusOnObjective && (force || !QuestManager.Instance.ShowingObjective))
            {
                var target = GetCloserObject(GameReferences.PlayerTransform.position);
                if (target && target.IsDelivery && !shouldFocusOnDelivery) return;
                if (target)
                {
                    OnFocus.Invoke();
                    QuestManager.Instance.ShowObjectivePath(target);
                    target.Focus();
                    QuestManager.Instance.ShowingObjective = true;
                }
            }
        }
        [Button, PropertySpace(50)]
        public void ResetData(bool hardReset = false)
        {
            Status = Quest_Status.Inative;
            RewardStatus = Reward_Status.Waiting;
            CurrentValue = 0;
            _linked = false;
            if (hardReset)
            {
                LinkedObjects.Clear();
                DeliverTarget = null;
                IsAvaliable.RemoveAllListeners();
                IsActive.RemoveAllListeners();
                IsComplete.RemoveAllListeners();
                IsDisable.RemoveAllListeners();
                IsRewardReady.RemoveAllListeners();
                OnClaimReward.RemoveAllListeners();
                OnFocus.RemoveAllListeners();
            }
        }
        [ContextMenu("Generate Id"), Button(ButtonSizes.Large), VerticalGroup("Title/Right")] public void GenerateId() => Id = Guid.NewGuid().ToString();
        private void OnValidate()
        {
            if (String.IsNullOrEmpty(Id))
            {
                GenerateId();
                //EditorUtility.SetDirty(this);
            }

            for (int i = 0; i < Rewards.Count; i++)
            {
                QuestReward reward = Rewards[i];
                if (reward.Reward != null && !reward.Icon)
                {
                    reward.Icon = reward.Reward.Icon;
                    //EditorUtility.SetDirty(this);
                }
            }
            if (QuestType == Quest_Type.MultipleQuests && LinkedQuests.Count > 0)
            {
                TotalValue = LinkedQuests.Count;
                //EditorUtility.SetDirty(this);
            }
            _eventsCount = 0;
            _eventsCount += IsDisable.GetPersistentEventCount();
            _eventsCount += IsActive.GetPersistentEventCount();
            _eventsCount += IsComplete.GetPersistentEventCount();
            _eventsCount += OnActive.GetPersistentEventCount();
            _eventsCount += OnComplete.GetPersistentEventCount();
            _eventsCount += OnUpdateProgression.GetPersistentEventCount();
            _eventsCount += IsAvaliable.GetPersistentEventCount();
            _eventsCount += OnAvaliable.GetPersistentEventCount();
            _eventsCount += IsFailed.GetPersistentEventCount();
            _eventsCount += OnFail.GetPersistentEventCount();
            _eventsCount += OnFocus.GetPersistentEventCount();
            _eventsCount += IsRewardReady.GetPersistentEventCount();
            _eventsCount += OnClaimReward.GetPersistentEventCount();
        }
    }
    public enum Quest_Type { SingleObjective, MultipleQuests, DirectProgression }
    public enum Quest_Status { Inative, Avaliable, Active, Fail, Complete }
    public enum Reward_Status { Waiting, Ready, Claimed }
    [System.Serializable]
    public class QuestReward
    {
        [HorizontalGroup("Split", .2f), PreviewField(ObjectFieldAlignment.Left, Height = 45), ReadOnly, ShowIf(@"Reward"), HideLabel]
        public Sprite Icon;
        [VerticalGroup("Split/Right"), LabelWidth(75)] public Reward Reward;
        [VerticalGroup("Split/Right"), LabelWidth(75)] public float Ammount;
    }
}