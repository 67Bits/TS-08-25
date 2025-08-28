using UnityEngine;

namespace Turret
{
    public class ParticleToSensor : MonoBehaviour
    {
        [SerializeField] private TurretSensor _sensor;
        [SerializeField] private ParticleSystem _particleSystem;
        private Transform _target;

        void Update()
        {
            if (_sensor == null || _particleSystem == null) return;

            if (_sensor.CurrentTarget == null)
            {
                if (_particleSystem.isPlaying)
                    _particleSystem.Stop();
            }
            else
            {
                if (!_particleSystem.isPlaying)
                    _particleSystem.Play();
            }
        }
    }
}
