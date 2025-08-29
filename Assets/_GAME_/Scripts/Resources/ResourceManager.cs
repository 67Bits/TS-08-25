using UnityEngine;
using UnityEngine.Events;

namespace TS
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private Transform resourceEndPoint;
        public UnityEvent OnConsumeResource;

        public Transform GetResourceEndPoint() => resourceEndPoint;
    }
}
