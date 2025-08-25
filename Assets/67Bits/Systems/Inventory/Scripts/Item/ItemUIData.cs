using System;
using UnityEngine;

[Serializable]
public class ItemUIData
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _sprite;

    [Tooltip("Used in UI elements visually related with this item.")]
    [SerializeField] private Color _relatedColor = Color.gray;

    [Tooltip("When is automatically visible in UI lists.")]
    [SerializeField] private bool _isInitiallyVisible;

    public string Name { get => _name;}
    public Sprite Sprite { get => _sprite; }
    public Color RelatedColor { get => _relatedColor; }
    public bool IsInitiallyVisible { get => _isInitiallyVisible; }

#if UNITY_EDITOR
    public void SetSpecialName(string name)
    {
        _name = name;
    }
#endif
}