using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WrapAround : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 screenBounds;

    void Start()
    {
        mainCamera = Camera.main;
        // Calculate screen bounds based on the camera size
        screenBounds = new Vector2(
            mainCamera.orthographicSize * Screen.width / Screen.height,
            mainCamera.orthographicSize
        );
    }

    void Update()
    {
        Vector3 position = transform.position;

        // Check if the player is outside the horizontal bounds
        if (position.x > screenBounds.x)
        {
            position.x = -screenBounds.x;
        }
        else if (position.x < -screenBounds.x)
        {
            position.x = screenBounds.x;
        }

        // Check if the player is outside the vertical bounds
        if (position.y > screenBounds.y)
        {
            position.y = -screenBounds.y;
        }
        else if (position.y < -screenBounds.y)
        {
            position.y = screenBounds.y;
        }

        // Apply the new position
        transform.position = position;
    }
}

