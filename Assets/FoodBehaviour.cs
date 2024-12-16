using UnityEngine;

public class FoodBehavior : MonoBehaviour
{
    private bool hasTouchedRice = false;
    private float fallSpeed;

    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
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
                    float riceWidth = collision.collider.bounds.size.x / 2;

                    if (Mathf.Abs(foodCenterX - riceCenterX) <= riceWidth)
                    {
                        hasTouchedRice = true;
                        FindObjectOfType<GameManager>().CheckFood(gameObject);

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
    }
}



