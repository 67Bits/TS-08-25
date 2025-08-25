using System;
using System.Collections;
using UnityEngine;







public class Move3DObjectToUIElement : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform uiElement;
    public float moveSpeed = 5f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private IEnumerator Start()
    {
        StartMovement();

        while (isMoving)
        {
            MoveObject();
            CheckIfFinished();
            yield return null;
        }
    }

    void StartMovement()
    {
        // 1. Find the 3D direction
        Vector3 uiPosition = uiElement.position;
        Vector3 uiWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(uiPosition.x, uiPosition.y, mainCamera.nearClipPlane));

        Vector3 objectScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 direction = uiPosition - objectScreenPos;
        direction.z = mainCamera.nearClipPlane;

        targetPosition = mainCamera.ScreenToWorldPoint(objectScreenPos + direction);

        isMoving = true;
    }

    void MoveObject()
    {
        // 2. Move the element
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void CheckIfFinished()
    {
        // 3. Check when finished move
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            isMoving = false;
            Debug.Log("Movement finished");
        }

        // Check if object is behind UI element in screen space
        Vector3 objectScreenPos = mainCamera.WorldToScreenPoint(transform.position);
        Vector3 uiScreenPos = uiElement.position;

        if (objectScreenPos.z > mainCamera.nearClipPlane &&
            objectScreenPos.x >= uiScreenPos.x && objectScreenPos.x <= uiScreenPos.x + uiElement.rect.width &&
            objectScreenPos.y >= uiScreenPos.y && objectScreenPos.y <= uiScreenPos.y + uiElement.rect.height)
        {
            isMoving = false;
            Debug.Log("Object is behind UI element");
        }
    }
}