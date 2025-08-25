using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace ObjectPooling
{
    public static class ObjectPool
    {
        public static Dictionary<Object, Pool<Object>> Pools = new();
        public static Pool<Object> CreatePool<T>(this T obj, Object poolId, int? starSize = null, int? maxSize = null) where T : Object
        {
            if (Pools.Count == 0) SceneManager.sceneUnloaded += ClearPool;
            if (!Pools.TryGetValue(poolId, out Pool<Object> pool))
            {
                pool = new Pool<Object>(obj, new GameObject(poolId.name.ToString() + "(Pool)").transform, starSize, maxSize);
                Pools.Add(poolId, pool);
            }
            return pool;
        }
        public static T InstantiateFromPool<T>(this T poolType, Object poolId,
            Vector3? position = null, Quaternion? rotation = null,
            bool disabledOnly = true, bool autoEnable = true, bool autoReturn = true) where T : Object
        {
            T instance;
            if (!Pools.TryGetValue(poolId, out Pool<Object> pool))
                pool = CreatePool<T>(poolType as T, poolId);
            instance = pool.Get(disabledOnly) as T;
            if (instance)
            {
                if (autoReturn) pool.Return(instance);

                GameObject instanceGO = null;
                if (instance is MonoBehaviour monoBehaviour)
                    instanceGO = monoBehaviour.gameObject;
                else if (instance is GameObject gameObject)
                    instanceGO = gameObject;

                if (autoEnable && instanceGO) instanceGO.gameObject.SetActive(true);
                instanceGO?.transform.SetPositionAndRotation(position ?? instanceGO.transform.position, rotation ?? instanceGO.transform.rotation);
            }
            return instance;
        }
        public static GameObject InstantiateFromPool(this GameObject poolType,
    Vector3? position = null, Quaternion? rotation = null,
    bool disabledOnly = true, bool autoEnable = true, bool autoReturn = true)
        {
            GameObject instance;
            if (!Pools.TryGetValue(poolType.gameObject, out Pool<Object> pool))
                pool = CreatePool(poolType, poolType);
            instance = pool.Get(disabledOnly) as GameObject;
            if (instance)
            {
                if (autoReturn) pool.Return(instance);
                if (autoEnable) instance.gameObject.SetActive(true);
                instance.transform.SetPositionAndRotation(position ?? instance.transform.position, rotation ?? instance.transform.rotation);
            }
            return instance;
        }
        public static void ClearPool(Scene scene)
        {
            Pools.Clear();
        }
    }
    public class Pool<T> where T : Object
    {
        public T Prefab;
        public Transform PoolParent;
        public int CurrentSize;
        public int MaxSize;
        public int StartSize;
        public List<T> Objects = new();
        private bool isGameObject;
        public Pool(T prefab, Transform poolParent, int? minSize = null, int? maxSize = null, bool spawnSizeAtStart = false)
        {
            Prefab = prefab;
            isGameObject = Prefab is GameObject;
            StartSize = minSize ?? 5;
            MaxSize = maxSize ?? 1000;
            PoolParent = poolParent;
            if (spawnSizeAtStart)
                for (int i = 0; i < StartSize; i++)
                    Spawn(false);
        }
        public T Get(bool firstDisbled)
        {
            if (CurrentSize < StartSize)
                return Spawn();
            if (firstDisbled)
            {
                T instance = null;
                for (int i = 0; i < Objects.Count; i++)
                {
                    T obj = Objects[i];
                    if (!obj)
                    {
                        Objects.RemoveAt(i);
                        i--;
                        continue;
                    }
                    if (isGameObject && obj is GameObject go && !go.activeInHierarchy)
                    {
                        go?.transform.SetParent(PoolParent);
                        instance = obj;
                        break;
                    }
                    if (obj is MonoBehaviour gObj && !gObj.gameObject.activeInHierarchy)
                    {
                        gObj?.transform.SetParent(PoolParent);
                        instance = obj;
                        break;
                    }
                }
                if (!instance && CurrentSize < MaxSize)
                    instance = Spawn();
                else Objects.Remove(instance);
                return instance;
            }
            if (CurrentSize == 0 && CurrentSize < MaxSize)
                Spawn();
            T toReturn = Objects[0];
            Objects.RemoveAt(0);
            if (toReturn is MonoBehaviour returnObject)
            {
                returnObject.transform.SetParent(PoolParent);
                returnObject.gameObject.SetActive(true);
            }
            else if (isGameObject && toReturn is GameObject gc)
            {
                gc.transform.SetParent(PoolParent);
                gc.SetActive(true);
            }
            return toReturn;
        }
        public void Return(T toReturn) => Objects.Add(toReturn);
        private T Spawn(bool setActive = true)
        {
            CurrentSize++;
            var instance = MonoBehaviour.Instantiate(Prefab);
            if (instance is MonoBehaviour returnObject)
            {
                returnObject.gameObject.SetActive(setActive);
                returnObject.transform.parent = PoolParent;
            }
            else if (isGameObject && instance is GameObject gc)
            {
                gc.SetActive(setActive);
                gc.transform.parent = PoolParent;
            }
            return instance;
        }
    }
}
