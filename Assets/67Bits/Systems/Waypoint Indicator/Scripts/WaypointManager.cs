using Bello.Unity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SSB.WaypointIndicator
{
    public class WaypointManager : MonoBehaviour
    {
        public static WaypointManager Instance
        {
            get
            {
                if (!_instance) _instance = FindFirstObjectByType<WaypointManager>();
                return _instance;
            }
        }
        private static WaypointManager _instance;

        [SerializeField] private WaypointUI _waypointPrefab;
        [SerializeField] private Dictionary<Waypoint, WaypointUI> _waypoints = new();
        [SerializeField] private Vector2 _uIDistanceFromEdge;
        [SerializeField] private float _uIYOffset;
        [SerializeField] private float _uIMoveSpeed = 2.5f;
        public Transform Target;
        [SerializeField, ShowIf("@Target")] private float _closerDistanceBeforeDisapear = 2.5f;

        private Camera mainCameraRef;
        private void Awake()
        {
            _instance = this;
            mainCameraRef = Camera.main;
        }
        internal void AddWaypoint(Waypoint waypoint)
        {
            if (!_waypoints.ContainsKey(waypoint))
            {
                var newWaypointUI = Instantiate(_waypointPrefab, transform);
                newWaypointUI.Initialize(waypoint);
                _waypoints.Add(waypoint, newWaypointUI);
            }
        }
        internal void RemoveWaypoint(Waypoint waypoint)
        {
            if (_waypoints.TryGetValue(waypoint, out var waypointUI))
            {
                Destroy(waypointUI.gameObject);
                _waypoints.Remove(waypoint);
            }
        }
        private void Update()
        {
            UpdateWaypoints();
        }
        private void UpdateWaypoints()
        {
            for (int i = 0; i < _waypoints.Count; i++)
            {
                var waypoint = _waypoints.ElementAt(i);
                var waypointUI = waypoint.Value;
                var waypointTransform = waypoint.Key.transform;

                // Update the position and rotation of the UI element
                var newPosition = mainCameraRef.WorldToViewportPoint(waypointTransform.position);
                var screenPoint = mainCameraRef.ViewportToScreenPoint(newPosition);

                var behindCamera = newPosition.z < 0;
                if (!mainCameraRef.pixelRect.Contains(screenPoint) || behindCamera) // Is outside Camera
                {
                    waypointUI.RotateIndicator.up = behindCamera ? Vector2.down : screenPoint - waypointUI.transform.position;

                    newPosition.x =
                        Mathf.Clamp(behindCamera ? -newPosition.x : newPosition.x, 0 + _uIDistanceFromEdge.x, 1f - _uIDistanceFromEdge.x);
                    newPosition.y =
                        Mathf.Clamp(behindCamera ? 0 : newPosition.y, 0 + _uIDistanceFromEdge.y, 1f - _uIDistanceFromEdge.y);

                    waypointUI.gameObject.SetActive(true);
                }
                else
                {
                    if (!waypoint.Key.ShowWhenInsideCamera || Target &&
                        !waypoint.Key.ShowWhenClose &&
                        Vector3.Distance(waypoint.Key.transform.position, Target.position) <= _closerDistanceBeforeDisapear)
                    {
                        waypointUI.gameObject.SetActive(false);
                        continue;
                    } else waypointUI.gameObject.SetActive(true);
                    newPosition.y += _uIYOffset;
                    waypointUI.RotateIndicator.up = Vector2.down;
                }
                screenPoint = mainCameraRef.ViewportToScreenPoint(newPosition);
                waypointUI.transform.position = behindCamera ?
                    Vector3.Lerp(waypointUI.transform.position, screenPoint, Time.deltaTime * _uIMoveSpeed) :
                    screenPoint;
            }
        }
    }
}
