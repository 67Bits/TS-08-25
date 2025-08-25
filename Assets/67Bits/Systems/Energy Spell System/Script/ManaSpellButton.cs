using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SSB.EnergySpell
{
    public class ManaSpellButton : MonoBehaviour
    {
        [SerializeField] private bool _autoInitialize = true;
        [SerializeField] private Button _button;
        [SerializeField] private float _targetManaValue;
        [SerializeField] private TextMeshProUGUI _valueText;
        [FoldoutGroup("Events"), SerializeField] private UnityEvent _onEnoughMana;
        [FoldoutGroup("Events"), SerializeField] private UnityEvent _onNotEnoughMana;

        private ManaGenerator _generator;
        private void OnEnable() => _button.onClick.AddListener(RemoveMana);
        private void OnDisable() => _button.onClick.RemoveListener(RemoveMana);
        private void Start()
        {
            if (_autoInitialize)
            {
                _generator = ManaGenerator.Instance;
                Initialize(_generator, _targetManaValue);
            }
        }
        public void Initialize(ManaGenerator generator, float cost)
        {
            _generator = generator;
            _targetManaValue = cost;
            _valueText.text = cost.ToString();
            _generator.OnManaUpdate += CompareManaValue;
            CompareManaValue(0);
        }
        protected virtual void CompareManaValue(float value)
        {
            if (value >= _targetManaValue) _onEnoughMana?.Invoke();
            else _onNotEnoughMana?.Invoke();
        }
        public void RemoveMana() => _generator.RemoveMana(_targetManaValue);
    }
}
