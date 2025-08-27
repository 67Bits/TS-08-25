using UnityEngine;

namespace TS
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private Transform resourceEndPoint;

        public Transform GetResourceEndPoint() => resourceEndPoint;
    }
}
