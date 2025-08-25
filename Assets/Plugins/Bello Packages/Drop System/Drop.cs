using UnityEngine;

namespace DropSystem
{
    [CreateAssetMenu(fileName = "New Drop", menuName = "Bello/DropSystem/Drop")]
    public class Drop : ScriptableObject, IDroppable
    {
        [field: SerializeField] public GameObject DropPrefab { get; set; }
        [field: SerializeField, Range(0, 100)] public float Chance { get; set; }

        public void DropItem(Vector3 position)
        {
            var newDrop = Instantiate(DropPrefab);
            newDrop.transform.position = position;
        }
    }
    public interface IDroppable
    {
        public GameObject DropPrefab { get; set; }
        public float Chance { get; set; }
        public void DropItem(Vector3 position);
    }
}