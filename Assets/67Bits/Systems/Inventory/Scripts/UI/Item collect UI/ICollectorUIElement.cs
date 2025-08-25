using Inventory;
using UnityEngine;

public interface ICollectorUIElement
{
    /// <summary>
    /// Where in UI the item icon should move to.
    /// </summary>
    public TrackedPosition CollectingUIPosition();
    public void HoldQuantityForAnimation(int quantity, float waitTime);
    public void OnCollect();
    void OnCollectAnimationStart();
}