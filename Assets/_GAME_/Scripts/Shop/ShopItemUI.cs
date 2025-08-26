using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TGR
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] public UnityEvent OnDisable;

        public void SetUpItem(ShopItem item, Action onClick)
        {
            if (iconImage) iconImage.sprite = item.Icon;
            if (nameText) nameText.text = item.Name;
            if (descriptionText) descriptionText.text = item.Description;
            if (priceText) priceText.text = item.Price.ToString();

            if (buyButton)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    item.Apply();
                    onClick?.Invoke();
                });

            }
        }

    }
}
