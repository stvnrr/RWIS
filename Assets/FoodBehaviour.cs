using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private bool hasTouchedRice = false;
    private float fallSpeed;
    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private int maxPoints = 100;
    private int goodPoints = 75;

    private int midPoints = 50;
    private int minPoints = 25;
    private int incorrectFoodPenalty = -50;
    public Sprite[] sprites; // Array to hold the different sprites
    private Sprite leftCloseSprite;
    private Sprite leftMidSprite;
    private Sprite leftFarSprite;
    private Sprite rightCloseSprite;
    private Sprite rightMidSprite;
    private Sprite rightFarSprite;

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftCloseSprite = sprites[0];
        leftMidSprite = sprites[1];
        leftFarSprite = sprites[2];
        rightCloseSprite = sprites[3];
        rightMidSprite = sprites[4];
        rightFarSprite = sprites[5];

    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Disable gravity on the food object initially
        rb.gravityScale = 0f;
    }
    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
    }
    void FixedUpdate()
    {
        if (!hasTouchedRice)
        {
            // Manually update the falling speed based on the fallSpeed value
            rb.velocity = new Vector2(rb.velocity.x, -fallSpeed); // Apply downward velocity
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the food touches the rice
        if (collision.gameObject.CompareTag("Rice") && !hasTouchedRice)
        {
            // Get the contact point of the collision
            ContactPoint2D[] contactPoints = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contactPoints);

            foreach (var contact in contactPoints)
            {
                // Check if the contact point is on the top of the rice
                float riceTop = collision.collider.bounds.max.y; // Top edge of the rice
                float foodBottom = GetComponent<Collider2D>().bounds.min.y; // Bottom edge of the food
                float tolerance = Mathf.Max(0.01f, 0.1f * transform.localScale.y); // Scale-dependent tolerance

                if (Mathf.Abs(foodBottom - riceTop) < tolerance) // Tolerance to ensure alignment
                {
                    // Check if the centers align horizontally
                    float foodCenterX = transform.position.x;
                    float riceCenterX = collision.transform.position.x;
                    float riceHalfWidth = collision.collider.bounds.size.x / 2;
                    float maxThreshold = riceHalfWidth;
                    float midThreshold = 0.7f * riceHalfWidth;
                    float minThreshold = 0.4f * riceHalfWidth;
                    float perfectThreshold = 0.1f * riceHalfWidth;

                    int points = 0;
                    SpriteRenderer foodRenderer = GetComponent<SpriteRenderer>();

                    float distance = Mathf.Abs(foodCenterX - riceCenterX);
                    if (distance <= perfectThreshold)
                    {
                        points = maxPoints; // Maximum points

                    }
                    else if (distance <= minThreshold)
                    {
                        points = goodPoints; // Maximum points
                        foodRenderer.sprite = (foodCenterX < riceCenterX) ? leftCloseSprite : rightCloseSprite;

                    }
                    else if (distance <= midThreshold)
                    {
                        points = midPoints; // Medium points
                        foodRenderer.sprite = (foodCenterX < riceCenterX) ? leftMidSprite : rightMidSprite;

                    }
                    else if (distance <= maxThreshold)
                    {
                        points = minPoints; // Minimum points
                        foodRenderer.sprite = (foodCenterX < riceCenterX) ? leftFarSprite : rightFarSprite;

                    }
                    else
                    {
                        points = incorrectFoodPenalty; // Penalty for incorrect food
                    }
                    if (distance <= riceHalfWidth)
                    {
                        hasTouchedRice = true;
                        FindObjectOfType<GameManager>().CheckFood(gameObject, points);

                        // Stop moving after touching rice
                        Rigidbody2D rb = GetComponent<Rigidbody2D>();
                        rb.bodyType = RigidbodyType2D.Static;

                        // Make the food a child of the rice
                        transform.SetParent(collision.transform);

                        Debug.Log("Food attached to the top of the rice!");
                        break;
                    }
                }
            }
        }
        if (!hasTouchedRice) 
        {
        FindObjectOfType<GameManager>().CheckFood(gameObject, incorrectFoodPenalty);

        }
    }
}



