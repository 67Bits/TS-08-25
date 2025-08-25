using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SSB.Quests
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance{
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<QuestManager>(FindObjectsInactive.Include);
                return _instance;
            }
        }
        private static QuestManager _instance;
        [field: SerializeField] public List<Quest> AllQuests { get; private set; } = new List<Quest>();

        [field: Header("Main Quest")]
        [field: SerializeField] public QuestUI MainQuestsUI { get; private set; }
        [field: SerializeField] public SideQuestUI SideQuestsUI { get; private set; }
        [field: FoldoutGroup("QuestDeliver"), SerializeField] public Transform QuestDeliverCanvas { get; private set; }
        [field: FoldoutGroup("QuestDeliver"), SerializeField] public Transform SingleQuestDeliverTransform { get; private set; }
        [field: FoldoutGroup("QuestDeliver"), SerializeField] public QuestUI SingleQuestDeliverUI { get; private set; }
        [field: FoldoutGroup("QuestDeliver"), SerializeField] public Transform MultipleQuestDeliverTransform { get; private set; }
        [field: FoldoutGroup("QuestDeliver"), SerializeField] public QuestUI MultipleQuestDeliverUI { get; private set; }
        public List<Quest> MainQuests = new();
        public List<Quest> SideQuests = new();

        [field: Header("Quests In Progress")]
        [field: SerializeField, ReadOnly] public List<Quest> ActiveQuests { get; set; } = new List<Quest>();
        [field: SerializeField, ReadOnly] public Quest CurrentQuest { get; private set; }

        private List<Objective> Objectives = new();

        // Camera
        [SerializeField] private bool _autoLoad;
        [SerializeField] private bool _autoInitialize;
        [SerializeField] private bool _autoStartQuests;
        [SerializeField] private bool _canSkipMainQuests;
        [SerializeField] private float _cameraFocusDuration = 2;

        private Coroutine _currentCameraRoutine;
        public enum CameraStates
        {
            Follow,
            StartFocus,
            EndFocus
        }
        public CameraStates CameraState = CameraStates.Follow;

        public bool ShowingObjective { get; set; }

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            if (_autoLoad) LoadAllQuests();
        }
        private void Start()
        {
            if (_autoInitialize)
                Initialize();
        }

        #region Configs
        public void Initialize()
        {
            ResetObjectives();
            for (int i = 0; i < AllQuests.Count; i++)
            {
                Quest quest = AllQuests[i];
                quest.ResetData();
                quest.Load();
            }
            MainQuests.RemoveAll((x) => x.Status == Quest_Status.Complete);
            for (int i = 0; i < MainQuests.Count; i++)
            {
                Quest quest = MainQuests[i];
                quest.OnComplete.AddListener(OnMainQuestComplete);
                quest.OnActive.AddListener(ChangeQuestUI);
            }
            if (MainQuests.Count > 0)
            {
                ChangeCurrentQuest(MainQuests[0], 1);
            }
            for (int i = 0; i < ActiveQuests.Count; i++)
            {
                Quest quest = ActiveQuests[i];
                if (!MainQuests.Contains(quest) && (quest.QuestType != Quest_Type.MultipleQuests || quest.ShowInUI))
                    SideQuestsUI.AddNewQuest(quest);
            }
            SideQuestsUI?.Initialize();
        }
        [Button, HorizontalGroup("Reset")]
        public void ResetObjectives()
        {
            if (Objectives.Count == 0)
                Objectives = Resources.LoadAll<Objective>("").ToList();
            for (int i = 0; i < Objectives.Count; i++)
            {
                Objective objective = Objectives[i];
                objective.LinkedQuests.Clear();
            }
        }
        [Button, HorizontalGroup("Reset")]
        public void LoadAllQuests()
        {
            AllQuests = Resources.LoadAll<Quest>("").ToList();
            MainQuests = Resources.LoadAll<Quest>("Quests/Main").ToList();
            SideQuests = AllQuests.Where((quest) => !MainQuests.Contains(quest)).ToList();
        }
        #endregion
        public async void ChangeCurrentQuest(Quest newQuest, float delay = 0)
        {
            CurrentQuest = newQuest;
            await FocusOnMainQuestObject(delay);
            CurrentQuest.Active();
            if (CurrentQuest.Status == Quest_Status.Complete)
                await MainQuestsUI.SetQuest(CurrentQuest, delay);
        }
        public void ChangeQuestUI()
        {
            foreach (var quest in MainQuests)
            {
                if (quest.Status == Quest_Status.Active)
                {
                    ChangeCurrentQuest(quest, 2);
                    break;
                }
            }
        }
        private void OnMainQuestComplete()
        {
            var index = 0;
            if (_canSkipMainQuests)
            {
                for (int i = MainQuests.Count - 1; i >= 0; i--)
                {
                    var quest = MainQuests[i];
                    if (quest.Status == Quest_Status.Complete)
                        index = i;
                }
                for (int i = 0; i < index; i++)
                {
                    MainQuests[i].Active();
                    MainQuests[i].Fail();
                    MainQuests.RemoveAt(i);
                }
            }
            if (CurrentQuest.Status != Quest_Status.Complete && CurrentQuest.Status != Quest_Status.Fail) return;
            MainQuests.Remove(CurrentQuest);
            if (MainQuests.Count > 0 && _autoStartQuests)
            {
                ChangeCurrentQuest(MainQuests[0], index > 0 ? 0 : 4);
            }
        }
        #region Quest and Camera
        public IEnumerator ChangeCameraObjective(GameObject target, float delaySeconds = 0f)
        {
            CameraState = CameraStates.StartFocus;
            yield return new WaitForSeconds(delaySeconds);
            if (target)
                ChangeCameraFocus(target.transform);
            yield return new WaitForSeconds(_cameraFocusDuration);
            ChangeCameraFocus();
            yield return new WaitForEndOfFrame();
            CameraState = CameraStates.EndFocus;
            ShowingObjective = false;
        }
        /// <summary>
        /// Change the current cinemachine follow target and look target, as default is reset the target to gamemanager's player transform
        /// </summary>
        /// <param name="focus"></param>
        private void ChangeCameraFocus(Transform focus = null)
        {
            GameReferences.CinemachineCamera.Follow = focus ?? GameReferences.PlayerTransform;
            GameReferences.CinemachineCamera.LookAt = focus ?? GameReferences.PlayerTransform;
        }
        #endregion
        #region Quest Objects;
        public async Awaitable FocusOnMainQuestObject(float? delay = null)
        {
            await MainQuestsUI.SetQuest(CurrentQuest, delay);
            if (CurrentQuest.FocusOnObjective && CurrentQuest.StartFocus && !ShowingObjective)
            {
                var target = CurrentQuest.GetCloserObject(GameReferences.PlayerTransform.position);
                if (target)
                {
                    CurrentQuest.OnFocus.Invoke();
                    ShowObjectivePath(target);
                    target.Focus();
                    ShowingObjective = true;
                }
            }
        }
        public void ShowObjectivePath(QuestObject questObject, bool force = false)
        {
            CheckTrackableQuest(questObject);
            FocusCamera(questObject);
        }
        public void CheckTrackableQuest(QuestObject questObject)
        {
            TrackPath.Instance?.SetPath(questObject);
        }
        public void FocusCamera(QuestObject questObject, bool force = false)
        {
            if (_currentCameraRoutine != null && force) StopCoroutine(_currentCameraRoutine);
            _currentCameraRoutine = StartCoroutine(ChangeCameraObjective(questObject.gameObject));
        }
        #endregion
        private void OnDisable()
        {
            ResetObjectives();
            for (int i = 0; i < MainQuests.Count; i++)
            {
                Quest quest = MainQuests[i];
                quest.OnComplete.RemoveListener(OnMainQuestComplete);
                quest.OnActive.RemoveListener(ChangeQuestUI);
            }
            for (int i = 0; i < AllQuests.Count; i++)
            {
                Quest quest = AllQuests[i];
#if UNITY_EDITOR
                quest.ResetData(true);
#else
                quest.ResetData();
#endif
            }
        }

#if UNITY_EDITOR
        [MenuItem("Tools/QuestSystem/Reset All"), Button, HorizontalGroup("Reset")]
        public static void ResetAllQuestDatas()
        {
            var all = Resources.LoadAll<Quest>("").ToArray();
            for (int i = 0; i < all.Length; i++)
            {
                Quest quest = all[i];
                quest.ResetData(true);
                UnityEditor.EditorUtility.SetDirty(quest);
            }
        }
#endif
    }
}