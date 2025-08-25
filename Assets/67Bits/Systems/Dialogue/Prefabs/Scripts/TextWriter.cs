using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GabrielBertasso.UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextWriter : MonoBehaviour
    {
        [SerializeField, TextArea] private string _targetText;
        [SerializeField, Range(0f, 1f)] private float _fillAmount;
        [SerializeField, Min(0f)] private float _charsPerMinute = 280f;
        [SerializeField] private bool _restoreOnDisable;

        [Space]
        public UnityEvent OnEnd;

        private TextMeshProUGUI _text;

        private float _lastFillAmount;
        
        public string TargetText
        {
            get => _targetText;
            set
            {
                _targetText = value;
                UpdateText();
            }
        }
        public float FillAmount
        {
            get => _fillAmount;
            set => SetFillAmount(value);
        }
        
        private float FillAmountPerChar => 1f / TargetText.ToString().Length;
        
        
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                UpdateText();
                return;
            }

            if (_charsPerMinute != 0f)
            {
                float charCount = Time.unscaledDeltaTime / 60f * _charsPerMinute;
                FillAmount += charCount * FillAmountPerChar;
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            
            if (FillAmount >= 0.99f && _lastFillAmount < 0.99f)
            {
                OnEnd?.Invoke();
            }

            _lastFillAmount = FillAmount;
            
            UpdateText();
        }

        private void OnDisable()
        {
            if (_restoreOnDisable)
            {
                Restore();
            }
        }

        public void SetFillAmount(float value)
        {
            _fillAmount = Mathf.Clamp(value, 0f, 1f);
            UpdateText();
        }
        
        public void Restore()
        {
            _fillAmount = 0f;
            _text.text = null;
        }

        private void UpdateText()
        {
            string text = TargetText;
            _text.text = text.Substring(0, Mathf.CeilToInt(text.Length * _fillAmount));
        }
    }
}