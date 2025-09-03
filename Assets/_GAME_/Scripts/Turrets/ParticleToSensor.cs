using UnityEngine;
using UnityEngine.Events;

namespace Turret
{
    public class ParticleToSensor : MonoBehaviour
    {
        [SerializeField] private TurretSensor _sensor;
        [SerializeField] private ParticleSystem _particleSystem;
        private Transform _target;
        [SerializeField] private UnityEvent onParticlePlay;
        [SerializeField] private UnityEvent onParticleStop;

        void Update()
        {
            if (_sensor == null || _particleSystem == null) return;

            if (_sensor.CurrentTarget == null)
            {
                if (_particleSystem.isPlaying)
                {
                    _particleSystem.Stop();
                    onParticleStop?.Invoke();
                }
            }
            else
            {
                if (!_particleSystem.isPlaying)
                {
                    _particleSystem.Play();
                    onParticlePlay?.Invoke();
                }
            }
        }
    }
}
