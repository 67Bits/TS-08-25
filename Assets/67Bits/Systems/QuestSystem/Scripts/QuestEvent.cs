using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests
{
    public class QuestEvent : MonoBehaviour
    {
        [System.Serializable]
        private class QuestEvents
        {
            [SerializeField] public Quest Quest;

            [SerializeField, ReadOnly] public int _eventsCount;
            [FoldoutGroup("Events")] public UnityEvent IsDisable;
            [FoldoutGroup("Events")] public UnityEvent OnUpdateProgression;
            [FoldoutGroup("Events/Avaliable")] public UnityEvent OnAvaliable;
            [FoldoutGroup("Events/Avaliable")] public UnityEvent IsAvaliable;
            [FoldoutGroup("Events/Active")] public UnityEvent OnActive;
            [FoldoutGroup("Events/Active")] public UnityEvent IsActive;
            [FoldoutGroup("Events/Fail")] public UnityEvent OnFail;
            [FoldoutGroup("Events/Fail")] public UnityEvent IsFailed;
            [FoldoutGroup("Events/Complete")] public UnityEvent OnComplete;
            [FoldoutGroup("Events/Complete")] public UnityEvent IsComplete;


            [FoldoutGroup("Events/Reward")] public UnityEvent IsRewardReady;
            [FoldoutGroup("Events/Reward")] public UnityEvent OnClaimReward;

            [FoldoutGroup("Events/UI Events")] public UnityEvent OnFocus;

            private bool subscribled;
            public void SubscribleEvents()
            {
                if (subscribled) return;
                subscribled= true;
                Quest.OnAvaliable.AddListener(OnAvaliable.Invoke);
                Quest.IsAvaliable.AddListener(IsAvaliable.Invoke);

                Quest.OnActive.AddListener(OnActive.Invoke);
                Quest.IsActive.AddListener(IsActive.Invoke);

                Quest.OnFail.AddListener(OnFail.Invoke);
                Quest.IsFailed.AddListener(IsFailed.Invoke);

                Quest.OnComplete.AddListener(OnComplete.Invoke);
                Quest.IsComplete.AddListener(IsComplete.Invoke);

                Quest.OnUpdateProgression.AddListener(OnUpdateProgression.Invoke);
                Quest.IsDisable.AddListener(IsDisable.Invoke);

                Quest.IsRewardReady.AddListener(IsRewardReady.Invoke);
                Quest.OnClaimReward.AddListener(OnClaimReward.Invoke);

                Quest.OnFocus.AddListener(OnFocus.Invoke);
            }
            public void UnsubscribleEvents()
            {
                subscribled = false;
                Quest.OnAvaliable.RemoveListener(OnAvaliable.Invoke);
                Quest.IsAvaliable.RemoveListener(IsAvaliable.Invoke);

                Quest.OnActive.RemoveListener(OnActive.Invoke);
                Quest.IsActive.RemoveListener(IsActive.Invoke);

                Quest.OnComplete.RemoveListener(OnComplete.Invoke);
                Quest.IsComplete.RemoveListener(IsComplete.Invoke);

                Quest.OnUpdateProgression.RemoveListener(OnUpdateProgression.Invoke);
                Quest.IsDisable.RemoveListener(IsDisable.Invoke);

                Quest.IsRewardReady.RemoveListener(IsRewardReady.Invoke);
                Quest.OnClaimReward.RemoveListener(OnClaimReward.Invoke);

                Quest.OnFocus.RemoveListener(OnFocus.Invoke);
            }
        }
        [SerializeField] private QuestEvents[] _events;
        private void Awake() => OnEnable();
        private void OnEnable()
        {
            for (int i = 0; i < _events.Length; i++)
            {
                var qEvent = _events[i];
                qEvent.SubscribleEvents();
                switch (qEvent.Quest.Status)
                {
                    case Quest_Status.Inative:
                        qEvent.IsDisable?.Invoke();
                        break;
                    case Quest_Status.Avaliable:
                        qEvent.IsAvaliable?.Invoke();
                        break;
                    case Quest_Status.Active:
                        qEvent.IsActive?.Invoke();
                        break;
                    case Quest_Status.Fail:
                        qEvent.IsFailed?.Invoke();
                        break;
                    case Quest_Status.Complete:
                        qEvent.IsComplete?.Invoke();
                        break;
                }
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < _events.Length; i++)
            {
                var qEvent = _events[i];
                qEvent.UnsubscribleEvents();
            }
        }
        private void OnValidate()
        {
            for (int i = 0; i < _events.Length; i++)
            {
                QuestEvents qEvent = _events[i];
                if (qEvent == null) continue;
                qEvent._eventsCount = 0;
                qEvent._eventsCount += qEvent.IsDisable.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.IsActive.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.IsComplete.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnActive.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnComplete.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnUpdateProgression.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.IsAvaliable.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnAvaliable.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.IsFailed.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnFail.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnFocus.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.IsRewardReady.GetPersistentEventCount();
                qEvent._eventsCount += qEvent.OnClaimReward.GetPersistentEventCount();
            }
        }
    }
}
