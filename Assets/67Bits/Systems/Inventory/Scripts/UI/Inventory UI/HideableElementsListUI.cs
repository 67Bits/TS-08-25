using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HideableElement;
using UnityEngine.Events;

public class HideableElementsListUI : MonoBehaviour
{
    [Header("ResourceListUI")]

    [Tooltip("Button that show / hide elements on the list.")]
    [SerializeField] protected Button _toggleVisibilityButton;
    [SerializeField] protected Transform _elementsParent;
    [SerializeField] protected bool _startsWithAllElementsVisible = false;

    [Tooltip("list with all elements, lincluding not resource ones.")]
    [SerializeField] protected List<HideableUIElement> _hideableElements = new();
    protected List<GameObject> _allElements = new();

    private bool _showingHiddenElements = false;
    private bool _isElementsListDirty = false;

    public UnityEvent OnShowElementsToggledUE;
    public UnityEvent OnHideElementToggledsUE;

    protected virtual void Awake()
    {
        _showingHiddenElements = _startsWithAllElementsVisible;
    }

    #region Update List
    protected void AddHideableElement(HideableUIElement newElement)
    {
        if (!_hideableElements.Contains(newElement))
            _hideableElements.Add(newElement);

        newElement.OnVisibilityChange += UpdateElementsVisibility;

        if (!_allElements.Contains(newElement.gameObject))
        {
            _allElements.Add(newElement.gameObject);
            _isElementsListDirty = true;
        }
    }

    private void CleanDirtyList()
    {
        if (_isElementsListDirty)
        {
            ShuffleListPerVisibility();
            UpdateElementsVisibility();
            _isElementsListDirty = false;
        }
    }
    #endregion

    #region Update visibility
    protected void RebuildLayout()
        => LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_elementsParent);

    public void ShowElements()
    {
        foreach (var element in _hideableElements)
            element.Show();

        RebuildLayout();
        OnShowElementsToggledUE?.Invoke();
    }

    public void HideElements()
    {
        foreach (var element in _hideableElements)
            element.Hide();

        RebuildLayout();
        OnHideElementToggledsUE?.Invoke();
    }

    protected void UpdateElementsVisibility()
    {
        if (_showingHiddenElements)
            ShowElements();
        else
            HideElements();

        _isElementsListDirty = true;
    }

    public void ToggleElementsVisibility()
    {
        _showingHiddenElements = !_showingHiddenElements;

        UpdateElementsVisibility();
    }
    #endregion

    #region Reorganize list
    /// <summary>
    /// Reorder all hideable childrens at the end of the list in specified order,
    /// then the toggle button at the end.
    /// </summary>
    /// <param name="visibilityOrder">
    /// List of reorganized visibilities in specified order.
    /// </param>
    public void ShuffleListPerVisibility(List<Visibility> visibilityOrder = null)
    {
        visibilityOrder ??= new() {
            Visibility.AlwaysVisible,
            Visibility.TemporarilyVisible,
            Visibility.Hideable
        };

        List<List<HideableUIElement>> elementPerVisibility = new();

        // Separating items in distinct lists list:
        foreach (var filterVisibility in visibilityOrder)
        {
            var list = new List<HideableUIElement>();
            elementPerVisibility.Add(list);

            for (int i = 0; i < _hideableElements.Count; i++)
            {
                if (_hideableElements[i].Visibility == filterVisibility)
                    list.Add(_hideableElements[i]);

                _allElements.Remove(_hideableElements[i].gameObject);
            }
        }

        // Re-adding items in visibilityOrder list order
        for (int vi = 0; vi < visibilityOrder.Count; vi++)
        {
            var list = elementPerVisibility[vi];
            for (int i = 0; i < list.Count; i++)
                _allElements.Add(list[i].gameObject);
        }

        // Passing toggle button to the end
        if (_allElements.Contains(_toggleVisibilityButton.gameObject))
            _allElements.Remove(_toggleVisibilityButton.gameObject);

        _allElements.Add(_toggleVisibilityButton.gameObject);

        // Making transforms follow the final list order.
        for (int i = 0; i < _allElements.Count; i++)
        {
            if (_allElements[i].transform.parent != _elementsParent)
                _allElements[i].transform.parent = _elementsParent;

            _allElements[i].transform.SetSiblingIndex(i);
        }

        Debug.Log("Order Shuffled", this);
    }
    #endregion

    protected void LateUpdate()
    {
        CleanDirtyList();
    }
}