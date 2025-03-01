using UnityEngine;


public class RiceController : MonoBehaviour
{

    private Vector3 smoothedAcceleration;

    public float moveSpeed = 5f; // Speed of horizontal movement
    public float tiltSpeed = 0.5f; // Speed of movement based on phone tilt
    public float touchThreshold = 0.5f; // Threshold for determining left or right side (0.5 means center of screen)
    public float tiltThreshold = 0.01f;
    public float smoothFactor = 0.5f; // Adjust this value to get the desired smoothing effect
    public GameObject crossImage;  // Reference to the cross image prefab (UI)
    public GameObject vImage;
    public GameManager gameManager;

    void Start()
    {
        // Set initial position of rice at the bottom of the screen
        //SetInitialPosition();
    }

    void Update()
    {
        
        // Move rice with tilt if using accelerometer

        // Move rice with touch input
        MoveWithKeyboard();
        RestrictMovement();
        ImagePos();

        
        if (gameManager.ModeNumber == 0)
        {

            MoveWithTilt();

        }
        else if (gameManager.ModeNumber == 1)
        {


            MoveWithTouch();

        }
        // Restrict movement within screen bounds
        
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
        Vector3 currentAcceleration = Input.acceleration;

        // Apply low-pass filter
        smoothedAcceleration = Vector3.Lerp(smoothedAcceleration, currentAcceleration, smoothFactor * Time.deltaTime);
            
        // Use the phone's accelerometer to move the rice
        float horizontal = Input.acceleration.x; // Get tilt input (range from -1 to 1)

       if (Mathf.Abs(horizontal) > tiltThreshold)
        {
            // Calculate the movement, scaled by speed and tilt speed
            float movement = (horizontal - Mathf.Sign(horizontal) * tiltThreshold) * moveSpeed * tiltSpeed;
            
            int maxTiltSpeed = 5;
            // Clamp the movement to prevent excessive speed
            movement = Mathf.Clamp(movement, -maxTiltSpeed, maxTiltSpeed);

            // Apply the horizontal movement to the object's position
            transform.Translate(Vector3.right * movement * Time.deltaTime, Space.World);
        }
          
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

private float touchStartTime = 0f; // Tracks the time when the touch starts
public float maxSpeed = 5f; // Maximum speed limit when the touch is held longer


    void MoveWithTouch()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            // Handle touch start
            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time; // Record the time when the touch starts
            }

            // Handle touch move or stationary
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // Calculate the touch duration
                float touchDuration = Time.time - touchStartTime;

                // Calculate the speed based on how long the touch has been held
                float currentSpeed = Mathf.Min(touchDuration * 40f, maxSpeed); // Increase speed over time, with a maximum speed limit

                // Determine the horizontal position of the touch
                if (touch.position.x < Screen.width * 0.5f)
                {
                    // Move left, increasing speed the longer the touch is held
                    MoveLeft(currentSpeed);
                }
                else
                {
                    // Move right, increasing speed the longer the touch is held
                    MoveRight(currentSpeed);
                }
            }
        }
    }

    // Helper methods for movement
    void MoveLeft(float speed)
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void MoveRight(float speed)
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
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


