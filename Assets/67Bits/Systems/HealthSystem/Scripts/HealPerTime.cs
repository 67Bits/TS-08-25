using System.Collections;
using UnityEngine;

namespace SSBHealthSystem
{
    public class HealPerTime : MonoBehaviour
    {
        [SerializeField] private float _intervalToHeal;
        [SerializeField] private int _healingAmount;
        private Health _health;

        private void Start()
        {
            _health = GetComponent<Health>();
            WaitToHeal();
        }

        private async void  WaitToHeal()
        {
            await Awaitable.WaitForSecondsAsync(_intervalToHeal);
            if (_health.CurrentHealth < _health.MaxHealth)
            {
                _health.Heal(_healingAmount, true);
            }
            WaitToHeal();
        }
    }
}