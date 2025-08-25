using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace SSB.Quests
{
    [RequireComponent(typeof(SplineContainer), typeof(SplineInstantiate))]
    public class TrackPath : MonoBehaviour
    {
        public static TrackPath Instance;
        [Header("Spline Settings")]
        [SerializeField] private float _updateInterval = 0f;
        [SerializeField] private float _yOffset = 0.01f;
        [field: ReadOnly, SerializeField] public QuestObject CurrentQuestObject { get; set; }
        [field: ReadOnly, SerializeField] public List<QuestObject> QuestObjects { get; set; } = new();
        private SplineContainer _spline;
        private SplineInstantiate _splineInstantiate;
        private Coroutine _trackPathCoroutine;

        private void Awake()
        {
            Instance = this;
            _spline = GetComponent<SplineContainer>();
            _splineInstantiate = GetComponent<SplineInstantiate>();
        }
        private void Start()
        {
            StartCoroutine(TrackPathCoroutine());
        }
        public void SetPath(QuestObject target)
        {
            if (!QuestObjects.Contains(target))
                QuestObjects.Add(target);
            StartTrackPath();
        }
        public void EndPath(QuestObject target)
        {
            if (!QuestObjects.Contains(target)) return;
            QuestObjects.Remove(target);
            if (QuestObjects.Count == 0)
            {
                CurrentQuestObject = null;
                if (_splineInstantiate.isActiveAndEnabled)
                    _splineInstantiate.Clear();
            }
        }
        [ContextMenu("Clear")]
        public void ClearPath()
        {
            _splineInstantiate.Clear();
            _splineInstantiate.enabled = false;
            _spline.Spline.Clear();
            if (_trackPathCoroutine != null) StopCoroutine(_trackPathCoroutine);
            _trackPathCoroutine = null;
        }
        [ContextMenu("Start Tracking")]
        public void StartTrackPath()
        {
            if (CurrentQuestObject == null && _trackPathCoroutine != null)
                ClearPath();
            if (_trackPathCoroutine == null)
                _trackPathCoroutine = StartCoroutine(TrackPathCoroutine());
        }
        private IEnumerator TrackPathCoroutine()
        {
            _splineInstantiate.enabled = true;
            while (QuestObjects.Count > 0)
            {
                var nearestQuestObject = GetClosestTarget();
                if (nearestQuestObject != CurrentQuestObject || CurrentQuestObject == null)
                    CurrentQuestObject = nearestQuestObject;
                DrawPath();
                yield return new WaitForSeconds(_updateInterval);
            }
            ClearPath();
        }
        private QuestObject GetClosestTarget()
        {
            float shortestDistance = Mathf.Infinity;
            QuestObject nearestQuestObject = null;

            for (int i = 0; i < QuestObjects.Count; i++)
            {
                QuestObject questObject = QuestObjects[i];
                if (i == 0 && CurrentQuestObject == null)
                {
                    nearestQuestObject = questObject;
                    continue;
                }
                float distance = Vector3.Distance(GameReferences.PlayerTransform.position, questObject.transform.position);
                if (distance < shortestDistance && HasAnyActiveObjective(questObject))
                {
                    shortestDistance = distance;
                    nearestQuestObject = questObject;
                }
            }
            return nearestQuestObject;
        }
        private bool HasAnyActiveObjective(QuestObject questObject, Quest? targetObjective = null)
        {
            return true;
        }
        private void DrawPath()
        {
            if (CurrentQuestObject == null)
                return;
            _spline.Spline.Clear();
            NavMeshPath newPath = new NavMeshPath();

            if (NavMesh.CalculatePath(GameReferences.PlayerTransform.position, CurrentQuestObject.transform.position, NavMesh.AllAreas, newPath))
            {
                for (int i = 0; i < newPath.corners.Length; i++)
                {
                    BezierKnot bezier = new BezierKnot
                    {
                        Position = newPath.corners[i] + new Vector3(0, _yOffset, 0),
                        Rotation = Quaternion.identity
                    };
                    _spline.Spline.Add(bezier);
                }
            }
        }
    }
}