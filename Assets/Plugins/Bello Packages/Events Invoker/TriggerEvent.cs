using Bello.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace Bello.EventsInvoker
{
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private LayerMask _targerLayers;
        public UnityEvent<Collider2D> OnTriggerEnter;
        public UnityEvent<Collider2D> OnTriggerExit;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_targerLayers.Include(collision.gameObject.layer))
                OnTriggerEnter?.Invoke(collision);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_targerLayers.Include(collision.gameObject.layer))
                OnTriggerExit?.Invoke(collision);
        }
    }
}
