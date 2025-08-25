using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSBHealthSystem
{
    public class SliderBar : MonoBehaviour
    {
        [SerializeField] protected Slider _slider;
        [SerializeField] private float _startFill = 1;
        [Header("Fill Animations")]
        [SerializeField] protected float _fillLerpDuration;
        [SerializeField] private AnimationCurve _fillAnimationCurve;

        [Header("Visibility Settings")]
        [SerializeField] protected bool _autoHide;
        [SerializeField, ShowIf("_autoHide")] private CanvasGroup _canvasGroup;
        [SerializeField, ShowIf("_autoHide")] private MMF_Player _onDisableOnStartFeel;
        [SerializeField, ShowIf("_autoHide")] private float _timeToDisappear;

        [Header("Delay Bar")]
        [SerializeField] protected bool _useDelayBar;
        [SerializeField, ShowIf("_useDelayBar")] protected Slider _delaySlider;
        [SerializeField, ShowIf("_useDelayBar")] protected float _delay;
        [SerializeField, ShowIf("_useDelayBar")] protected float _delayFillLerpDuration;
        [SerializeField, ShowIf("_useDelayBar")] private AnimationCurve _delayFillAnimationCurve;

        [Header("Text")]
        [SerializeField] protected bool _useText;
        [SerializeField, ShowIf("_useText")] protected TextMeshProUGUI _valueTMP;
        [SerializeField, ShowIf("_useText")] protected bool _lerpText;
        protected float _currentValue;

        private bool _initiated;

        protected virtual void Start()
        {
            _currentValue = GetMaxValue();
            SetSliderValues();

            if (_autoHide)
            {
                _onDisableOnStartFeel?.PlayFeedbacks();
            }

            _initiated = true;
        }

        private void OnEnable()
        {
            if (_initiated)
                ResetBar();
        }

        protected virtual void SetSliderValues()
        {
            if (!gameObject.activeSelf)
                return;

            float targetFill = GetTargetFill();
            if (_slider.gameObject.activeInHierarchy)
            {
                SetFillAmount(targetFill);
            }
            else
            {
                _slider.value = targetFill;
            }

            if (!_useText)
                return;

            _valueTMP.gameObject.SetActive(true);
            if (_lerpText)
            {
                SetTextValueWithLerp();
            }
            else
            {
                string text = GetValueText();
                SetValueText(text);
            }
        }

        protected virtual float GetTargetFill() => 0;
        protected virtual string GetValueText() => " ";
        protected virtual float GetCurrentValue() => 0;
        protected virtual float GetMaxValue() => 0;
        protected virtual void SetValueText(string text)
        {
            _valueTMP.text = text;
        }

        private async void SetFillAmount(float targetFill)
        {
            if (_useDelayBar)
            {
                SetDelaySliderFill(targetFill);
            }

            float timer = 0;
            float lerpValue = 0;
            float startFill = _slider.value;

            while (lerpValue < 1)
            {
                timer += Time.deltaTime;
                lerpValue = timer / _fillLerpDuration;
                _slider.value = Mathf.Lerp(startFill, targetFill, _fillAnimationCurve.Evaluate(lerpValue));

                await Awaitable.NextFrameAsync();
            }
            _slider.value = targetFill;
        }

        private async void SetDelaySliderFill(float targetFill)
        {
            await Awaitable.WaitForSecondsAsync(_delay);

            float timer = 0;
            float lerpValue = 0;
            float startFill = _delaySlider.value;

            while (lerpValue < 1)
            {
                timer += Time.deltaTime;
                lerpValue = timer / _delayFillLerpDuration;
                _delaySlider.value = Mathf.Lerp(startFill, targetFill, _delayFillAnimationCurve.Evaluate(lerpValue));

                await Awaitable.NextFrameAsync();
            }

            _delaySlider.value = targetFill;
        }

        private async void SetTextValueWithLerp()
        {
            float timer = 0;
            float lerpValue = 0;
            float targetValue = GetCurrentValue();

            while (lerpValue < 1)
            {
                timer += Time.deltaTime;
                lerpValue = timer / _fillLerpDuration;
                float lerpedFloat = Mathf.Lerp(_currentValue, targetValue, _fillAnimationCurve.Evaluate(lerpValue));
                _valueTMP.text = $"{(int)lerpedFloat}/{GetMaxValue()}";

                await Awaitable.NextFrameAsync();
            }

            _valueTMP.text = $"{(int)targetValue}/{GetMaxValue()}";
            _currentValue = targetValue;
        }

        protected virtual void ResetBar()
        {
            _slider.value = _startFill;
            _currentValue = GetMaxValue();

            if (_useDelayBar)
            {
                _delaySlider.value = _startFill;
            }
            else
            {
                _delaySlider.gameObject.SetActive(false);
            }
        }

        #region Visibility

        protected async void WaitToDisappear()
        {
            await Awaitable.WaitForSecondsAsync(_timeToDisappear);
            Disappear();
        }
        protected virtual void Disappear()
        {
        }
        protected virtual void Appear()
        {
            WaitToDisappear();
        }

        #endregion
    }
}