using UnityEngine;
using UnityEngine.Events;

namespace SSB.Quests.Examples
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private bool _canDestroy = false;
        [SerializeField] private UnityEvent _onCollected;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerController player))
            {
                _onCollected?.Invoke();
                if (_canDestroy)
                    Destroy(this.gameObject);
            }
        }
    }
}