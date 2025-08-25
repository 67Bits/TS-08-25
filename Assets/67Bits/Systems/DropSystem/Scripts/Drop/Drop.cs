using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Drop is abstract, since is ment to be 
/// reimplemented for each game inventory system.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Drop : OverlayObject
{
    [SerializeField] CollectionEvent collectionEvent;
    
    [ShowIf("collectionEvent == TimeCollect")]
    [SerializeField] private float _time;

    public virtual bool TryBeCollectedBy(IDropCollector dropCollector)
    {
        if (collectionEvent != CollectionEvent.CollisionCollect)
            return false;

        OnCollected();

        return true;
    }

    public virtual void OnCollected()
    {
        Debug.Log($"Drop '{this.gameObject.name}' being collected");
        // ... Extra collected code.
    }

    protected virtual void Start()
    {
        if (collectionEvent == CollectionEvent.InstantCollect)
            OnCollected();

        else if (collectionEvent == CollectionEvent.TimeCollect)
            Invoke("OnCollected", _time);
    }

    public enum CollectionEvent
    {
        InstantCollect,
        TimeCollect,
        CollisionCollect,
    }

    #region Enums

    public enum State
    {
        Disabled,
        Dropped,
        Collected,
    }

    public enum DropAnimationType
    {
        Fall,
        JumpArc,
        Nothing,
    }

    public enum CollectAnimationType
    {
        Disapear,
        JumpArc,
        GoToCollector,
        GoToHUD,
    }

    #endregion

}