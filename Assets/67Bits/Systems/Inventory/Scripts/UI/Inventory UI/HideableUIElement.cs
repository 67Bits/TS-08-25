using System;
using UnityEngine;
using UnityEngine.Events;

namespace HideableElement
{
    public class HideableUIElement : MonoBehaviour
    {
        //[ReadOnly] [SerializeField] private Visibility _visibility = (Visibility)(-1);
        public Visibility Visibility = (Visibility)(-1);

        public bool IsHideable => Visibility == Visibility.Hideable;
        public bool IsTemporarilyVisible => Visibility == Visibility.TemporarilyVisible;

        public Action OnVisibilityChange = () => { };

        public UnityEvent OnShowElementUE;
        public UnityEvent OnHideElementUE;

        public virtual void SetVisibility(Visibility visibility)
        {
            if (Visibility == visibility)
                return;

            Visibility = visibility;

            if (IsHideable)
                Hide();
            else
                Show();
        }

        public void Show()
        {
            if (this.gameObject.activeSelf)
                return;

            this.gameObject.SetActive(true);
            OnShowElementUE?.Invoke();

            OnVisibilityChange?.Invoke();
        }

        /// <summary> Applies only if hideable. </summary>
        public void Hide()
        {
            if (!IsHideable) return;

            if (!this.gameObject.activeSelf)
                return;

            this.gameObject.SetActive(false);
            OnHideElementUE?.Invoke();

            OnVisibilityChange?.Invoke();
        }

        /// <summary> Applies only if hideable. </summary>
        public void SetAsTemporarilyUnhideable()
        {
            if (IsHideable)
                SetVisibility(Visibility.TemporarilyVisible);
        }

        public void ResetVisibility()
        {
            if (IsTemporarilyVisible)
                SetVisibility(Visibility.Hideable);
        }
    }

    public enum Visibility
    {
        AlwaysVisible,
        TemporarilyVisible,
        Hideable,
    }
}