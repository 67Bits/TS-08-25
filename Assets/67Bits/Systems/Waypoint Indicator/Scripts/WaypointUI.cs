using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SSB.WaypointIndicator
{
    internal class WaypointUI : MonoBehaviour
    {
        [field: SerializeField] internal Transform RotateIndicator { get; private set; }
        [SerializeField] private Image _icon;
        [SerializeField] private bool updateColor = true;
        [field: SerializeField] internal TextMeshProUGUI DistanceText { get; private set; }
        public void Initialize(Waypoint waypoint)
        {
            transform.position = Camera.main.WorldToScreenPoint(waypoint.transform.position);
            _icon.sprite = waypoint.Sprite;
            if (updateColor)
            {
                var images = GetComponentsInChildren<Image>();
                foreach (var image in images)
                    if (image != _icon)
                        image.color = waypoint.Color;
            }
        }
    }
}