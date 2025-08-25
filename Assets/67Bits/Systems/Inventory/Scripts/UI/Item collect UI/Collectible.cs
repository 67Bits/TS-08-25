using System.Collections.Generic;
using UnityEngine;
using Inventory;
using Sirenix.OdinInspector;

public class Collectible : MonoBehaviour
{
    [SerializeField] private List<Amount<ItemID>> ItemAmounts = new();

    [Button]
    public void CollectItem()
    {
        if (CollectingUI.Instance == null) 
            return;

        foreach (var amount in ItemAmounts)
        {
            if (amount == null) continue;
            if (amount.Quantity < 1) continue;

            if (InventoryService.Instance.TryAddOnItemCount(amount.Unit, amount.Quantity))
            {
                CollectingUI.Instance?.TryCollectAnimation(amount, GetScreenPosition(this.transform.position));
            }
        }
    }

    public Vector2 GetScreenPosition(Vector3 WorldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(WorldPosition);
        return new Vector2(screenPos.x, screenPos.y);
    }
}
