using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SSBRoadmap
{
    public class RoadMapFillBar : MonoBehaviour
    {
        [SerializeField] private RoadMapScreen _roadMapScreen;
        [SerializeField] private Slider _slider;
        [SerializeField, Range(0.01f, 0.1f)] private float _fillSpeed;

        private RoadMapStateDot[] _dots;
        //private Vector2 _minPos;
        //private Vector2 _maxPos;
        [SerializeField] private RectTransform _minPos;
        [SerializeField] private RectTransform _maxPos;

        private string _savedFillKey = "roadmapfill";

        private float Fill
        {
            get => PlayerPrefs.GetFloat(_savedFillKey);
            set => PlayerPrefs.SetFloat(_savedFillKey, value);
        }


        private void Awake()
        {
            _slider.value = Fill;
            _roadMapScreen.OnUpdateProgress += AssignFill;
        }

        //private void GetMaxMinPositions()
        //{
        //    var rectTr = _slider.transform as RectTransform;

        //    Vector2 center = rectTr.position;
        //    Vector2 size = new Vector2(Mathf.Abs(rectTr.sizeDelta.x), Mathf.Abs(rectTr.sizeDelta.y));
        //    _minPos = center - size;
        //    _maxPos = center + size;
        //}

        private void AssignFill()
        {
            StartCoroutine(FillSlider());
        }

        private IEnumerator FillSlider()
        {
            yield return new WaitForEndOfFrame();
            float lastBoughtYPos = _roadMapScreen.LastBought.YPos;
            float targetFill = Mathf.InverseLerp(_minPos.position.y, _maxPos.position.y, lastBoughtYPos);

            while (_slider.value != targetFill)
            {
                _slider.value = Mathf.MoveTowards(_slider.value, targetFill, _fillSpeed);
                yield return new WaitForEndOfFrame();
            }

            Fill = _slider.value;
        }
    }
}