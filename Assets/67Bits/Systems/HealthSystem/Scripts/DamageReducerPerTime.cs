using System.Collections;
using UnityEngine;

namespace SSBHealthSystem
{
    public class DamageReducerPerTime : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private int _percentageToReduce;
        [SerializeField] private float _timeToReduce;
        [SerializeField] private bool _playAtStart;

        private void Start()
        {
            if (_playAtStart)
            {
                StartDamageReduction(_percentageToReduce);
            }
        }

        public void StartDamageReduction(int percentageToReduce)
        {
            _percentageToReduce = percentageToReduce;
            ReduceDamage();
        }

        private async void ReduceDamage()
        {
            _health.ReduceDamagedPercentage += _percentageToReduce;
            await Awaitable.WaitForSecondsAsync(_timeToReduce);
            _health.ReduceDamagedPercentage -= _percentageToReduce;
        }
    }
}
