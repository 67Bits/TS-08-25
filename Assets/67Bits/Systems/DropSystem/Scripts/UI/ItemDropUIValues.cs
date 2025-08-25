using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SSBDrops
{
    [RequireComponent(typeof(Image))]
    public class ItemDropUIValues : MonoBehaviour
    {
        [SerializeField] private Image _render;
        [SerializeField] private Sprite _sprite;

        private void OnEnable()
        {
            SetUIValues();
        }

        public void SetUIValues()
        {
            _render.sprite = _sprite;
        }
    }
}