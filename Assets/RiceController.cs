using UnityEngine;

public class RiceController : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of horizontal movement
    public float tiltSpeed = 2f; // Speed of movement based on phone tilt
    public float touchThreshold = 0.5f; // Threshold for determining left or right side (0.5 means center of screen)
    public GameObject crossImage;  // Reference to the cross image prefab (UI)
    public GameObject vImage;

    void Start()
    {
        // Set initial position of rice at the bottom of the screen
        //SetInitialPosition();
    }

    void Update()
    {
        // Move rice with tilt if using accelerometer
        MoveWithTilt();

        // Move rice with touch input
        MoveWithTouch();
        MoveWithKeyboard();
        // Restrict movement within screen bounds
        RestrictMovement();
        ImagePos();
    }

    void SetInitialPosition()
    {
        // Calculate the screen bounds
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Camera.main.aspect;

        // Set the y position to the bottom of the screen, and the x to a starting position
        transform.position = new Vector3(0, -screenHeight / 2 + GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);
    }

    void MoveWithTilt()
    {
        // Use the phone's accelerometer to move the rice
        float horizontal = Input.acceleration.x; // Get tilt input (range from -1 to 1)

        // Move the rice horizontally based on the tilt, adjusting speed with tiltSpeed
        transform.Translate(Vector3.right * horizontal * moveSpeed * tiltSpeed * Time.deltaTime);
    }
    void MoveWithKeyboard()
    {
        // Get horizontal input from the keyboard arrows (Left/Right)
        float horizontalInput = Input.GetAxis("Horizontal");

        // Move the rice left or right based on the input
        if (horizontalInput != 0)
        {
            transform.Translate(Vector3.right * horizontalInput * moveSpeed * Time.deltaTime);
        }
    }

    void MoveWithTouch()
{
    // Check if there is at least one touch
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0); // Get the first touch

        // Check the touch phase to handle continuous movement
        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            // Determine the horizontal position of the touch
            if (touch.position.x < Screen.width * 0.5f)
            {
                // Move left if touch is on the left side
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Move right if touch is on the right side
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
        }
    }
}


    void RestrictMovement()
    {
        // Get the screen bounds
        float screenWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2;

        // Clamp the rice's x position to within screen bounds
        float clampedX = Mathf.Clamp(transform.position.x, -screenWidth + halfWidth, screenWidth - halfWidth);

        // Set the position back with the clamped X, keeping the Y the same
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    // Function to show feedback image above the rice object
    public void ImagePos()
    {
        // Calculate the top-center position of the rice
        Vector3 riceTopCenter = new Vector3(transform.position.x, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y / 2, transform.position.z);

        // Instantiate the feedback image at the top-center position of the rice
        crossImage.transform.position = riceTopCenter;
        vImage.transform.position = riceTopCenter;

        // Parent the feedback image to the rice so it moves with the rice

    }
}


