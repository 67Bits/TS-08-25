using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSB.EnergySpell
{
    public class ManaGeneratorBarUI : MonoBehaviour
    {
        [SerializeField] private ManaGenerator _manaGenerator;
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _manaCounterText;
        [SerializeField] private float _sliderSpeed;
        [ReadOnly, SerializeField] private float _currentValue;

        private float _targetValue;
        private void OnEnable() => _manaGenerator.OnManaUpdate += VirtualUpdate;
        private void OnDisable() => _manaGenerator.OnManaUpdate -= VirtualUpdate;
        private void VirtualUpdate(float value)
        {
            _targetValue = value;
            _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, _sliderSpeed);
            _slider.value = Mathf.Repeat(_currentValue, _slider.maxValue);
            _manaCounterText.text = ((int)_targetValue).ToString();
        }
    }
}
