using UnityEngine;

namespace DropSystem
{
    public class DropsControll : MonoBehaviour
    {
        [Tooltip("Leave at '0' to drop all, or use a value for the dropper to stop after dropping the ammout of items")]
        [SerializeField] private int _targetItemAmmount;
        [Tooltip("Leave at '0' to drop all, or use a value for the dropper to stop after dropping the ammout of drops")]
        [SerializeField] private int _targetDropAmmount;
        [Tooltip("Max drop distance from origin")]
        [SerializeField] private Vector3 _dropOffset;
        [Tooltip("Max drop distance from origin")]
        [SerializeField] private float _dropRange = 1;
        [SerializeField] private DropItems[] _dropItems;
        [ContextMenu("Drop")]
        public void Drop()
        {
            var dropsDropped = 0;
            var itensDropped = 0;

            if (_targetItemAmmount > 0)
                while (itensDropped < _targetItemAmmount)
                {
                    CreateDrops(ref dropsDropped, ref itensDropped);
                }
            else CreateDrops(ref dropsDropped, ref itensDropped);

        }
        private void CreateDrops(ref int dropsDropped, ref int itensDropped)
        {
            for (int i = 0; i < _dropItems.Length; i++)
            {
                float chance = Random.Range(0, 100);
                DropItems item = _dropItems[i];
                if (!item.UseItemChance || chance < item.ItemChance)
                {
                    itensDropped++;
                    for (int currentAmmount = 0; currentAmmount < item.Ammount; currentAmmount++)
                    {
                        chance = Random.Range(0, 100);
                        if (!item.UseDropChance || chance < item.Drop.Chance)
                        {
                            item.Drop.DropItem(transform.position + _dropOffset + Random.insideUnitSphere * _dropRange);
                            dropsDropped++;
                            if (_targetDropAmmount > 0 && dropsDropped >= _targetDropAmmount) return;
                        }
                    }
                    if (_targetItemAmmount > 0 && itensDropped >= _targetItemAmmount) return;
                }
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + _dropOffset, _dropRange);
        }
    }
    [System.Serializable]
    public class DropItems
    {
        public string Name = "New Drop";      
        [Tooltip("Should this item have a random chance for drop beside the one in the drop")]
        [Header("Item Settings")]
        public bool UseItemChance;
        [Tooltip("The chance in % for this item be dropped")]
        [Range(0, 100)] public float ItemChance = 100;
        [Tooltip("The ammout of times it will use this item and create the drops")]
        [Range(0, 100)] public int Ammount;
        [Header("Drop Settings")]
        [Tooltip("Should this item drop have a random chance for drop beside the one in the item")]
        public bool UseDropChance = true;
        [Tooltip("Wich drop will be used")]
        public Drop Drop;
    }
}