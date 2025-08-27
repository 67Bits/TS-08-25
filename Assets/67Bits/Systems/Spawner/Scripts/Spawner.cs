using ObjectPooling;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace SSB.Spawner
{
    [HelpURL("https://doc.clickup.com/9017157017/p/h/8cqdtct-96437/e11947ddb2ccf76")]
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private bool _autoInitialize;
        [SerializeField] private bool _requireZeroCounterToFinish = true;
        [SerializeField] private bool _usePool;

        [SerializeField, ReadOnly, HorizontalGroup, LabelWidth(85)]
        private int _currentWave = -1;
        [SerializeField, ReadOnly, HorizontalGroup, LabelWidth(125)] private string _currentWaveName;

        [Header("Position Settings")]
        [SerializeField] private bool _useTransformPoints;
        [SerializeField, ShowIf("@_useTransformPoints")] private Transform[] _transformPoints;
        [SerializeField, ShowIf("@!_useTransformPoints")] private Vector3[] _spawnPoints = new Vector3[1];
        [Space(5)]
        [SerializeField, Tooltip("Shoud objects spawn in the areas beetween the points")] private bool _useAreaBeetweenPoints;
        [SerializeField, Tooltip("Shoud objects spawn in the areas around the points")] private bool _useRadius;
        [SerializeField, ShowIf("_useRadius")] private float _AreaRadius;

        [Space(5), Header("Waves Settings")]
        public Wave[] Waves;

        [field: SerializeField, ReadOnly] public int SpawnerCount { get; private set; }
        [field: SerializeField, ReadOnly] public bool Finished { get; private set; }
        private void Start()
        {
            if (_autoInitialize) BeginWave();
        }
        #region Wave
        [Button]
        public void BeginWave()
        {
            if (_currentWave >= 1)
            {
                BeginWave(_currentWave++);
            }
            _currentWave = 1;
            BeginWave(Waves[0]);
        }
        public void BeginWave(int waveId)
        {
            if (waveId < Waves.Length)
                BeginWave(Waves[waveId]);
        }
        public void BeginWave(string waveName) => BeginWave(Waves.First((wave) => wave.Name == waveName));
        public void BeginAllWaves()
        {
            for (int i = 0; i < Waves.Length; i++)
            {
                Wave wave = Waves[i];
                BeginWave(wave);
            }
        }
        public virtual async void BeginWave(Wave wave)
        {
            if (wave.ElapsedTime != 0 && !wave.Completed && !wave.Stopped) return;
            wave.Started = true;
            wave.Spawner = this;
            _currentWaveName = wave.Name;
            await Awaitable.WaitForSecondsAsync(wave.StartDelay);
            wave.OnStartWave?.Invoke();
            var spawnTime = wave.Loop ? wave.Delay : wave.Duration / wave.Ammount;
            var nextSpawnTime = Time.time + spawnTime;
            var loopCount = 0;
            while (!wave.Stopped &&
                     enabled &&
                     (
                         wave.Loop && (wave.LoopAmmount == 0 || loopCount < wave.LoopAmmount) ||
                         !wave.Loop && wave.ElapsedTime <= wave.Duration
                      ))
            {
                await Awaitable.FixedUpdateAsync();
                if (Time.time + Time.fixedDeltaTime >= nextSpawnTime)
                {
                    nextSpawnTime = Time.time + spawnTime;
                    Spawn(wave);
                    if (wave.Loop) loopCount++;
                }
                wave.ElapsedTime += Time.fixedDeltaTime;
            }
            if (wave.Stopped)
            {
                wave.OnStopWave?.Invoke();
            }
            else
            {
                wave.Completed = true;
                wave.OnFinishWave?.Invoke();
            }
            if (wave.NextWaveOnFinish)
            {
                for (int i = 0; i < Waves.Length; i++)
                {
                    var thisWave = Waves[i];
                    if (wave == thisWave && i + 1 < Waves.Length)
                    {
                        BeginWave(Waves[i + 1]);
                        break;
                    }
                }
            }
            if (!_requireZeroCounterToFinish)
                Finished = Waves.FirstOrDefault((w) => w.Started && !w.Stopped && !w.Completed) == null;
        }
        public void StopWave(Wave wave) => wave.Stopped = true;
        public void StopWave(int waveId) => Waves[waveId].Stopped = true;
        public void StopWave(string waveName) => Waves.First((wave) => wave.Name == waveName).Stopped = true;
        [Button]
        public void StopAllWaves()
        {
            for (int i = 0; i < Waves.Length; i++)
            {
                Wave wave = Waves[i];
                StopWave(wave);
            }
        }
        public void ResetWave(int waveId) => ResetWave(Waves[waveId]);
        public void ResetWave(string waveName) => ResetWave(Waves.First((wave) => wave.Name.Equals(waveName)));
        public void ResetWave(Wave wave)
        {
            wave.Completed = false;
            wave.ElapsedTime = 0;
        }
        [Button] public void NextWave() => BeginWave(_currentWave++);
        public void NextWave(bool stopCurrentWave)
        {
            if (stopCurrentWave && _currentWave < 0) StopWave(_currentWave);
            BeginWave(_currentWave++);
        }
        #endregion
        #region Spawn
        public virtual void Spawn(Wave wave)
        {
            var spawnedCount = wave.SpawCount;
            do
            {
                for (int i = 0; i < wave.Spawns.Length; i++)
                {
                    Spawn spawn = wave.Spawns[i];
                    for (int j = 0; j < spawn.SpawnAmmount; j++)
                    {
                        if (spawn.SpawnChance == 0 || Random.Range(0, 100) < spawn.SpawnChance)
                        {
                            var spawned = _usePool ? spawn.Prefab.InstantiateFromPool() : Instantiate(spawn.Prefab);
                            if (!spawned)
                            {
                                Debug.LogError($"Failed to spawn at {gameObject.name} {wave.Name}");
                                break;
                            }
                            spawned.transform.position = spawn.TargetPoint ? spawn.TargetPoint.position : GetPosition();
                            var spawnObj = spawned.AddComponent<SpawnObject>();
                            spawnObj.Spawner = this;
                            spawnObj.WaveName = wave.Name;
                            spawn.OnSpawnCreated?.Invoke();
                            spawnObj.OnDisabled += spawn.OnSpawnDisabled.Invoke;
                            wave.SpawCount++;
                            wave.SpawCountTotal++;
                            SpawnerCount++;
                        }
                    }

                }
            }
            while (wave.AlwaysSpawn && spawnedCount == wave.SpawCount);
        }
        public virtual void Spawn(string waveName)
        {
            var wave = Waves.First((x) => x.Name == waveName);
            Spawn(wave);
        }
        public virtual void Spawn()
        {
            if (_currentWave >= Waves.Length) return;
            var wave = Waves[_currentWave];
            Spawn(wave);
        }
        public void RemoveSpawn()
        {
            SpawnerCount--;
            if (_requireZeroCounterToFinish)
                Finished = SpawnerCount <= 0 && Waves.FirstOrDefault((w) => w.Started && !w.Stopped && !w.Completed) == null;
        }
        /// <summary>
        /// Change position for a object inside the spawn area
        /// </summary>
        /// <param name="target"></param>
        public void Respawn(Transform target)
        {
            target.gameObject.SetActive(false);
            target.position = GetPosition();
            target.gameObject.SetActive(true);
        }
        #endregion
        #region Positioning
        protected void SettupPoints()
        {
            if (_useTransformPoints)
            {
                _spawnPoints = new Vector3[_transformPoints.Length];
                for (int i = 0; i < _transformPoints.Length; i++)
                {
                    Transform point = _transformPoints[i];
                    _spawnPoints[i] = point.position;
                }
            }
        }
        protected virtual Vector3 GetPosition() =>
            (_useTransformPoints ? Vector3.zero : transform.position)
            + GetPointPosition() + GetAreaPosition();
        protected virtual Vector3 GetPointPosition()
        {
            var pointId = Random.Range(0, _spawnPoints.Length);
            var point = _spawnPoints[pointId];
            if (_useAreaBeetweenPoints && _spawnPoints.Length > 1)
            {
                pointId = ((int)Mathf.PingPong(pointId + 1, _spawnPoints.Length - 1));
                var nextPoint = _spawnPoints[pointId];

                float lerpPosition = Random.Range(0, 1f);

                return Vector3.Lerp(point, nextPoint, lerpPosition);
            }
            else return point;
        }
        protected virtual Vector3 GetAreaPosition()
        {
            if (_useRadius)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f); // random angle in radians
                float x = Mathf.Cos(angle) * _AreaRadius;
                float z = Mathf.Sin(angle) * _AreaRadius;
                return new Vector3(x, 0f, z); // y = 0, circle on XZ plane
            }
            return Vector3.zero;
        }

        #endregion
        protected virtual void OnDrawGizmos()
        {
            SettupPoints();
            Vector3? lastPoint = null;
            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                Vector3 point = _spawnPoints[i];
                Gizmos.color = Color.red;
                Gizmos.DrawSphere((_useTransformPoints ? Vector3.zero : transform.position) + point, .1f);

                if (_useAreaBeetweenPoints)
                {
                    if (lastPoint != null)
                        Debug.DrawLine(
                            (_useTransformPoints ? Vector3.zero : transform.position)
                            + lastPoint.Value, (_useTransformPoints ? Vector3.zero : transform.position) + point, Color.red);
                    lastPoint = point;
                }
                if (_useRadius)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere((_useTransformPoints ? Vector3.zero : transform.position) + point, _AreaRadius);
                }
            }
        }
    }
    [System.Serializable]
    public class Wave
    {
        [HideInInspector] public Spawner Spawner;
        [Required()] public string Name = "Wave";
        [FoldoutGroup("Wave Settings"), Tooltip("Automatically starts next wave on finish")] public bool NextWaveOnFinish;
        [FoldoutGroup("Wave Settings"), Tooltip("Time to wait before start the wave")] public float StartDelay;
        [FoldoutGroup("Wave Settings"), Space(5), Header("Time Settings")]
        [FoldoutGroup("Wave Settings"), Tooltip("Use loops or fixed duration")] public bool Loop;
        [FoldoutGroup("Wave Settings"), ShowIf("Loop"), Tooltip("Leave at 0 to infinite loop")] public int LoopAmmount;
        [FoldoutGroup("Wave Settings"), ShowIf("Loop"), Tooltip("Delay beetween loops")] public float Delay;
        [FoldoutGroup("Wave Settings"), ShowIf("@!Loop"), Tooltip("The duration for spawn all itens")] public float Duration;
        [FoldoutGroup("Wave Settings"), ShowIf("@!Loop"), Tooltip("The ammount to spawn inside the duration")] public int Ammount;

        [Header("Spaw Settings")]
        [FoldoutGroup("Wave Settings"), Tooltip("Will spawn at least 1 instance")] public bool AlwaysSpawn;
        [FoldoutGroup("Wave Settings")]
        public Spawn[] Spawns;

        [FoldoutGroup("Events")]
        public UnityEvent OnStartWave;
        [FoldoutGroup("Events")]
        public UnityEvent OnStopWave;
        [FoldoutGroup("Events")]
        public UnityEvent OnFinishWave;
        [FoldoutGroup("Events"), Range(0, 100),
            Tooltip("Target percentage for invoking event \nExample: 50% means when 1 SpawCount is left of 2 SpawCountTotal")]
        public float ClearPercentage = 100;
        [SerializeField, FoldoutGroup("Events"),
            Tooltip("Should this event be invoked every time a spawn is disabled" +
            " when the clear percentage is greater than the target percentage")]
        private bool _invokeMulitpleTimes;
        private bool _invokedWaveClear;
        [FoldoutGroup("Events"),
            Tooltip("Called when wave is stopped or completed and the spawn count lowered by the target percentage \n" +
            "Example 50% = when 1 SpawCount of 2 SpawCountTotal")]
        public UnityEvent OnWaveClear;

        [HorizontalGroup("Counters"), ReadOnly, Tooltip("Time since Wave started"), LabelWidth(75)] public int SpawCount;
        [HorizontalGroup("Counters"), ReadOnly, Tooltip("Time since Wave started"), LabelWidth(120)] public int SpawCountTotal;
        [HorizontalGroup("Counters"), ReadOnly, Tooltip("Time since Wave started"), LabelWidth(100)] public float ElapsedTime;
        [HorizontalGroup("Status"), ReadOnly, Tooltip("Manually Stopped")] public bool Started;
        [HorizontalGroup("Status"), ReadOnly, Tooltip("Manually Stopped")] public bool Stopped;
        [HorizontalGroup("Status"), ReadOnly, Tooltip("Finish All Spawns")] public bool Completed;
        public void RemoveSpawn()
        {
            SpawCount--;
            if (1f - (float)SpawCount / SpawCountTotal >= ClearPercentage / 100 && (Completed || Stopped))
            {
                if (!_invokedWaveClear || _invokeMulitpleTimes)
                {
                    OnWaveClear?.Invoke();
                    _invokedWaveClear = true;
                }
            }
            Spawner.RemoveSpawn();
        }
    }
    [System.Serializable]
    public class Spawn
    {
        [HideInInspector] public string Name;
        [Tooltip("Object to spawn")] public GameObject Prefab;
        [Tooltip("Object to spawn")] public Transform TargetPoint;
        [HorizontalGroup]
        [Tooltip("Ammount to instantiate per spawn"), LabelWidth(100)] public int SpawnAmmount = 1;
        [HorizontalGroup]
        [Tooltip("The percentage of chance it can be instantiated for each unity " +
            "\n0% will never instantiate the objects " +
            "\n100% will always instantiate all" +
             "\n50% has a great change to instantiate half of the ammount, but the result can differ in each spawn")]
        [Range(1, 100), LabelWidth(100)]
        public int SpawnChance = 100;
        [FoldoutGroup("Spawn Object Events"), Tooltip("Called on intantiation")]
        public UnityEvent OnSpawnCreated;
        [FoldoutGroup("Spawn Object Events"), Tooltip("Called when the instanced object is disabled")]
        public UnityEvent OnSpawnDisabled;
    }
}
