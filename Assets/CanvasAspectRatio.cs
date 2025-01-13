using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasAspectRatio : MonoBehaviour
{
    public float targetAspect = 1080f / 1920f; // Target aspect ratio (width/height)

    void Start()
    {
        // Calculate the current screen's aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // Calculate the scale height
        float scaleHeight = windowAspect / targetAspect;

        // Adjust the canvas scaler's match mode
        Canvas canvas = GetComponent<Canvas>();
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaleHeight < 1.0f)
        {
            // Letterboxing (match width)
            scaler.matchWidthOrHeight = 0;
        }
        else
        {
            // Pillarboxing (match height)
            scaler.matchWidthOrHeight = 1;
        }
    }
}
