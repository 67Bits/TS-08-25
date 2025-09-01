using System;
using TMPro;
using UnityEngine;
using SSBHealthSystem;

namespace TS
{
    [RequireComponent(typeof(Health))]
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private bool isPLayer = false;
        [SerializeField] private GameObject _damageTextPrefab;
        [SerializeField] private GameObject _healingTextPrefab;
        [SerializeField] private Transform _spawnPoint;

        private Health _health;


        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.onTakeDamageValue += OnTakeDamage;
            //_health.ConsumeCoal.AddListener(OnTakeDamage);
            //_health.onTakeDamageValue.AddListener(OnReceiveHealing);
        }

        private void OnDisable()
        {
            _health.onTakeDamageValue += OnTakeDamage;
            //_health.ConsumeCoal.RemoveListener(OnTakeDamage);
            //_health.onTakeDamageValue.RemoveListener(OnReceiveHealing);
        }

        private void OnTakeDamage(float damage)
        {
            if (_damageTextPrefab == null)
            {
                return;
            }

            try
            {
                Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;
                TextMeshProUGUI text = Instantiate(_damageTextPrefab, position, _damageTextPrefab.transform.rotation, _spawnPoint).GetComponentInChildren<TextMeshProUGUI>();
                if (isPLayer) text.text = "-" + damage.ToString("F0");
                else text.text = damage.ToString("F0");

            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void OnReceiveHealing(int damage)
        {
            if (_healingTextPrefab == null)
            {
                return;
            }

            Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;
            TextMeshProUGUI text = Instantiate(_healingTextPrefab, position, _healingTextPrefab.transform.rotation, _spawnPoint).GetComponentInChildren<TextMeshProUGUI>();

            text.text = "+" + damage.ToString("F0");
        }
    }
}
