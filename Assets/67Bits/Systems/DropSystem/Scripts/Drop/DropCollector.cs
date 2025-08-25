using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Implements IDropCollector for a monobehavior
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DropCollector : OverlayDetector, IDropCollector
{
    [SerializeField] private UnityEvent<Drop> OnDropGotUE;
    public Action<Drop> OnDropGotAction = (drop) => {};

    protected virtual void TryGetDrop(GameObject obj)
    {
        if (!obj.TryGetComponent(out Drop drop))
            return;

        if (!drop.TryBeCollectedBy(this))
            return;

        // On Collected:

        OnDropGotAction?.Invoke(drop);
        OnDropGotUE?.Invoke(drop);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryGetDrop(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryGetDrop(collision.collider.gameObject);
    }
}