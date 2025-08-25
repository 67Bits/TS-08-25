using UnityEngine;

namespace SSBHealthSystem
{
    public class HealthBleed : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private float _bleedSecsInterval = 1;
        private float _bleedDuration;
        private float _damage;
        private bool _repeat;

        private void Start()
        {
            _health.OnStartBleed += StartBleed;
        }

        public void StartBleed(float bleedTimeInSeconds, float damagePerBleed)
        {
            _bleedDuration = bleedTimeInSeconds;
            _damage = damagePerBleed;

            StartBleed();
        }

        private void StartBleed()
        {
            _repeat = true;

            WaitToBleed();
            WaitToStop();
        }

        private async void WaitToStop()
        {
            await Awaitable.WaitForSecondsAsync(_bleedDuration);
            _repeat = false;
        }

        private async void WaitToBleed()
        {
            await Awaitable.WaitForSecondsAsync(_bleedSecsInterval);
            Debug.Log("bleeding " + _damage);
            _health.TakeDamage(_damage);
            if (_repeat)
            {
                WaitToBleed();
            }
        }
    }
}