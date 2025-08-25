using UnityEngine;

namespace SSBPerks
{
    [CreateAssetMenu(fileName = "Example Perk", menuName = "67Bits/Perk/ExamplePerk")]
    public class ExamplePerk : TemporaryPerk
    {
        public override void Equip(bool equiped = true)
        {
            Debug.Log($"equipping perk of rarity {Rarity}");
        }
    }
}
