using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;
    public float moveSpeed;
    public float rotateAmount;
    float rot;
    int score;
    bool win = false;
    public GameObject winText;
    public GameObject Spawner;
    public GameObject newGameButton; // Reference to the New Game button
    private bool isMoving = false;
    private void Awake()
    {  rb = GetComponent<Rigidbody2D>();}
    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (mousePos.x < 0)
                {
                    rot = rotateAmount;
                }
                else
                {
                    rot = -rotateAmount;
                }
                transform.Rotate(0, 0, rot);
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (isMoving)
        {
            rb.velocity = transform.up * moveSpeed;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Food")
        {
            Destroy(collision.gameObject);
            score++;
            if (score >= Spawner.GetComponent<RandomObjectSpawner>().get_numFood())
            {
                print("Level Completed");
                win = true;
                winText.SetActive(true);
                newGameButton.SetActive(true);
            }
        }
        else if (collision.gameObject.tag == "Danger")
        {
            if (!win)
             {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            }
            Destroy(collision.gameObject);
        }
    }
    public void StartMoving()
    {
        // Enable movement by setting isMoving to true
        isMoving = true;
    }
}
