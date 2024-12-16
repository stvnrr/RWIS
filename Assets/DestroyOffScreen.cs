using UnityEngine;

public class DestroyOffScreen : MonoBehaviour
{
    void Update()
    {
        // Check if the object is below the bottom of the screen
        if (transform.position.y < -Camera.main.orthographicSize)
        {
            Destroy(gameObject);
        }
    }
}

