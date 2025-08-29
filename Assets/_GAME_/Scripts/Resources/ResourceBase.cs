using UnityEngine;
using Turret;
using GabrielBertasso;
using UnityEngine.Events;

namespace TS
{
    public class ResourceBase : MonoBehaviour
    {
        [SerializeField] private Bullet resourceBullet;
        [SerializeField] private LayerMask resourceCollectorLayer;

        [SerializeField] UnityEvent OnCollectResource;
        

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & resourceCollectorLayer) != 0)
            {
                Bullet bullet = Instantiate(resourceBullet.gameObject, transform.position, Quaternion.identity).GetComponent<Bullet>();
                ResourceManager resourceManager = other.GetComponent<ResourceManager>();
                OnCollectResource.Invoke();

                if ( resourceManager == null) return;

                UnityEvent OnReach = new UnityEvent();
                OnReach.AddListener(resourceManager.OnConsumeResource.Invoke);

                bullet.SetUpCurvedMovement(transform, resourceManager.GetResourceEndPoint(), OnReach);
                OnCollectResource.Invoke();
            }
        }
    }
}
