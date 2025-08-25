using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SSB.WaypointIndicator
{
    public class Waypoint : MonoBehaviour
    {
        [field: SerializeField] internal bool ShowWhenInsideCamera { get; private set; } = true;
        [field: SerializeField] internal bool ShowWhenClose { get; private set; } = false;
        [field: SerializeField] internal Sprite Sprite { get; private set; }
        [field: SerializeField] internal Color Color { get; private set; } = Color.white;
        private void OnEnable()
        {
            WaypointManager.Instance.AddWaypoint(this);
        }
        private void OnDisable()
        {
            WaypointManager.Instance?.RemoveWaypoint(this);
        }
    }
}
