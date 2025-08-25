using UnityEngine;

namespace Inventoy.Settings
{
    [CreateAssetMenu(menuName = "Services/Inventory/Settings", fileName = "InventorySettings")]
    public class InventorySettings : ScriptableObject
    {
        [Header("Configurations")]
        public bool AutoInitialize = false;

        [Header("References")]
        [SerializeField] private ItemCatalog catalog;
        public ItemCatalog Catalog
        {
            get
            {
                return catalog;
            }
        }
    }
}