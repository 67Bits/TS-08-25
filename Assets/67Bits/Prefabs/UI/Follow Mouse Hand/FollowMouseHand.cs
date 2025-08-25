using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseHand : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private Animation _animToPlay;
    private RectTransform _rect;


    private void Start()
    {
        _rect = (RectTransform)transform;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, mousePos, null, out Vector2 pos);
        _rect.anchoredPosition = pos;

        bool playAnim = Input.GetMouseButtonDown(0);
        if (playAnim)
        {
            _animToPlay.Play();
        }
    }

}
