using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ObjectPooling;
using UnityEngine.UI;

namespace SSBLevelProgression
{
    public class LevelProgressionBar : MonoBehaviour
    {
        [SerializeField] private Transform _startPos;
        [SerializeField] private Transform _endPos;
        [SerializeField] private Transform _iconsParent;
        [SerializeField] private LevelProgression _levelProgression;

        [Header("Slider Configs")]
        [SerializeField] private float _startFill = 1;
        [SerializeField] private float _fillLerpDuration;
        [SerializeField] private AnimationCurve _fillAnimationCurve;
        [SerializeField] private Slider _slider;

        private List<LevelIconInstance> _levelEventsIcons = new List<LevelIconInstance>();
        private bool _initiated;

        private void OnEnable()
        {
            if (_initiated)
                ResetBar();
        }

        private void Start()
        {
            ResetBar();
            DeletePrefabIcons();
            CreateLevelEventsIcons();
            AssignToProgressChanges();
            _initiated = true;
        }
        private void OnDisable()
        {
            ResetIcons();
        }

        private void AssignToProgressChanges()
        {
            _levelProgression.OnProgressionChange.AddListener(SetSliderValues);
        }

        private void SetSliderValues()
        {
            if (!gameObject.activeSelf)
                return;

            float targetFill = GetTargetFill();
            SetFillAmount(targetFill);
        }

        private float GetTargetFill()
        {
            float targetFill = (float)_levelProgression.CurrentProgress / (float)_levelProgression.MaxPoints;
            return targetFill;
        }
        private async void SetFillAmount(float targetFill)
        {
            float timer = 0;
            float lerpValue = 0;
            float startFill = _slider.value;

            while (lerpValue < 1)
            {
                timer += Time.deltaTime;
                lerpValue = timer / _fillLerpDuration;
                _slider.value = Mathf.Lerp(startFill, targetFill, _fillAnimationCurve.Evaluate(lerpValue));

                await Awaitable.NextFrameAsync();
            }
            _slider.value = targetFill;
        }

        private void CreateLevelEventsIcons()
        {
            var levelEvents = _levelProgression.LevelEvents;
            if (levelEvents == null)
                return;

            for (int i = 0; i < levelEvents.Count; i++)
            {
                var levelEvent = levelEvents[i];
                if (!levelEvent.DrawEvent)
                    continue;

                var eventIconPrefab = levelEvent.IconPrefab;
                var iconInstance = ObjectPool.InstantiateFromPool(eventIconPrefab);

                var levelIconInstance = new LevelIconInstance(iconInstance, eventIconPrefab);
                _levelEventsIcons.Add(levelIconInstance);

                iconInstance.transform.SetParent(_iconsParent);

                float percentage = (float)levelEvent.PointsToAppear / (float)_levelProgression.MaxPoints;
                iconInstance.transform.position = Vector3.Lerp(_startPos.position, _endPos.position, percentage);
            }
        }

        private void ResetIcons()
        {
            foreach (var levelEvent in _levelEventsIcons)
            {
                levelEvent.Instance.SetActive(false);
                ObjectPool.Pools[levelEvent.IconPrefab].Return(levelEvent.Instance);
            }
        }

        private void DeletePrefabIcons()
        {
            var buttons = _iconsParent.childCount;

            for (var i = 0; i < buttons; i++)
            {
                Destroy(_iconsParent.GetChild(i).gameObject);
            }
        }

        private void ResetBar()
        {
            _slider.value = _startFill;
        }
    }

    public class LevelIconInstance
    {
        public LevelIconInstance(GameObject instance, GameObject iconPrefab)
        {
            Instance = instance;
            IconPrefab = iconPrefab;
        }

        public GameObject Instance { get; private set; }
        public GameObject IconPrefab { get; private set; }
    }
}