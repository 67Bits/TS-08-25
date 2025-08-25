using UnityEngine;
namespace SSBPerks
{
    public abstract class TemporaryPerk : ScriptableObject
    {
        [Header("Deafult Sticker Settings")]
        public string Name = "New Power Up";
        [TextArea(2, 3)]
        public string Description = "New Power Up Description";
        [Tooltip("Icon for the UI")]
        public Sprite Icon;

        [Tooltip("Perk that will not go to equipped perk list")]
        [SerializeField] private bool _consumable;

        [Tooltip("Max times it can be eqquiped"), Range(0, 99)]
        public int MaxEquipped = 1;
        [Tooltip("Power Up Level/Rarity"), Range(0, 5)]
        public int Level;
        [Tooltip("New power up to add to list after eqquiping this one")]
        public TemporaryPerk NextLevel;
        [Tooltip("The chance it will appear in roullete"), Range(0, 100)]
        public int DropChance = 100;
        [SerializeField] private PerkRarity _rarity;
        public PerkRarity Rarity => _rarity;
        public bool Consumable => _consumable;
        /// <summary>
        /// When player select this powe up this will take effect
        /// </summary>
        public abstract void Equip(bool equiped = true);
    }
    public enum PerkRarity
    {
        Common,
        Epic,
        Rare,
        Legendary,
    }
}