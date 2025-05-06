using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTrack : MonoBehaviour
{
    public float lerpSpeed = 5.0f;  // Speed of the lerping movement
    public float maxOffset = 50.0f; // Maximum distance the background can move from its starting position

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Canvas canvas;

    void Start()
    {
        // Get the RectTransform component
        rectTransform = GetComponent<RectTransform>();

        // Store the initial anchored position of the background
        startPosition = rectTransform.anchoredPosition;

        // Find the parent canvas to get the correct scale and size for screen space
        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // Convert mouse position to canvas space (anchored position)
        Vector2 mouseCanvasPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out mouseCanvasPosition);

        // Calculate the target position based on the mouse movement
        Vector2 targetPosition = new Vector2(
            Mathf.Clamp(startPosition.x + (mouseCanvasPosition.x - startPosition.x) * 0.1f, startPosition.x - maxOffset, startPosition.x + maxOffset),
            Mathf.Clamp(startPosition.y + (mouseCanvasPosition.y - startPosition.y) * 0.1f, startPosition.y - maxOffset, startPosition.y + maxOffset)
        );

        // Lerp the background anchored position towards the target position
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, lerpSpeed * Time.deltaTime);
    }
}
