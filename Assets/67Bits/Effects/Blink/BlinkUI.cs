using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace SSB.Blink
{
    public class BlinkUI : MonoBehaviour
    {
        [SerializeField] private GameObject _blinkParentTarget;
        [Tooltip("Value for repeat blink effect"), SerializeField]
        private int _repeatValue = 3;
        [Tooltip("Duration of blink in seconds"), SerializeField]
        private float blinkInterval = .3f;
        [SerializeField] private Color _blinkColor = Color.white;
        [SerializeField] private Material _blinkTextMaterial;

        [ReadOnly, SerializeField] private Image[] _images;
        [ReadOnly, SerializeField] private Slider[] _sliders;
        [ReadOnly, SerializeField] private TextMeshProUGUI[] _textMeshProUGUIs;

        private Dictionary<Image, Color> _imageDic = new Dictionary<Image, Color>();
        private Dictionary<Slider, float> _sliderDic = new Dictionary<Slider, float>();
        private Dictionary<TextMeshProUGUI, Material> _textMeshProDic = new Dictionary<TextMeshProUGUI, Material>();
        public UnityEvent OnBlinkStart;

        private void Start()
        {
            SaveUIMaterials();
        }
        private void SaveUIMaterials()
        {
            if (_imageDic.Count == 0)
            {
                foreach (var img in _images)
                {
                   _imageDic.Add(img, img.color);
                }
            }
            if (_sliderDic.Count == 0)
            {
                foreach (var sld in _sliders)
                {
                    _sliderDic.Add(sld, sld.value);
                }
            }
            if (_textMeshProDic.Count == 0)
            {
                foreach (var tmp in _textMeshProUGUIs)
                {
                    if (tmp.fontMaterial == null || tmp.font == null) continue;
                    _textMeshProDic.Add(tmp, tmp.fontMaterial);
                }
            }
        }
        [ContextMenu("UIBlink")]
        public void UIBlink() => StartCoroutine(Blink());
        private IEnumerator Blink()
        {
            SaveUIMaterials();

            for (int x = 0; x < _repeatValue; x++)
            {
                foreach (var image in _imageDic)
                {
                    image.Key.color = _blinkColor;
                }

                foreach (var slider in _sliderDic)
                {
                    slider.Key.value = 0;
                }

                foreach (var textMeshPro in _textMeshProDic)
                {
                    textMeshPro.Key.fontMaterial = _blinkTextMaterial;
                }

                yield return new WaitForSeconds(blinkInterval / 2);

                foreach (var image in _imageDic)
                {
                    image.Key.color = image.Value;
                }

                foreach (var slider in _sliderDic)
                {
                    slider.Key.value = slider.Value;
                }

                foreach (var textMeshPro in _textMeshProDic)
                {
                    textMeshPro.Key.fontMaterial = textMeshPro.Value;
                }

                yield return new WaitForSeconds(blinkInterval / 2);
            }
        }
        [ContextMenu("FindMaterials")]
        private void FindMaterials()
        {
            _images = _blinkParentTarget.GetComponentsInChildren<Image>(true);
            _sliders = _blinkParentTarget.GetComponentsInChildren<Slider>(true);
            _textMeshProUGUIs = _blinkParentTarget.GetComponentsInChildren<TextMeshProUGUI>(true);
            var textMeshlist = new List<TextMeshProUGUI>(_textMeshProUGUIs);
            for (int i = 0; i < textMeshlist.Count; i++)
            {
                if (_textMeshProUGUIs[i].font == null)
                    textMeshlist.RemoveAt(i);
            }
            _textMeshProUGUIs = textMeshlist.ToArray();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        private void OnValidate()
        {
            if (_blinkParentTarget == null)
            {
                Debug.LogError($"Please add a parent reference on Blink Parent Target");
            }
            else FindMaterials();

            if (_images.Length <= 0 && _sliders.Length <= 0 && _textMeshProUGUIs.Length <= 0)
            {
                FindMaterials();
            }
        }
    }
}