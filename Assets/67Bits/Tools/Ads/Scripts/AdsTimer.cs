using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ads
{
    public class AdsTimer : MonoBehaviour
    {
        [SerializeField] internal int _time = 3;
        [SerializeField] private string _prefix = "ADS IN ";
        [SerializeField] private GameObject _parent;
        [SerializeField] private Slider _sliderTime;
        [SerializeField] private UnityEvent<string> OnUpdateText;
        [SerializeField] private UnityEvent OnEndTime;
        private float _currentTime;

        private bool _active = true;

        public void AciveParent()
        {
            _parent.SetActive(true);
        }

        void OnEnable()
        {
            _active = true;
            _sliderTime.maxValue = _time;
            _sliderTime.value = 0;
            _currentTime = Time.time;
        }


        void Update()
        {
            if (!_active) return;

            float time = (Time.time - _currentTime);
            OnUpdateText?.Invoke(_prefix + ((int)time).ToString("00"));
            _sliderTime.value = time;
            if (time >= _time)
            {
                _sliderTime.value = _time;
                OnUpdateText?.Invoke(_prefix + _time.ToString("00"));
                _active = false;
                OnEndTime?.Invoke();
            }
        }
    }
}
