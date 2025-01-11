using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using System.Collections;  // For IEnumerator and coroutines

public class GameManager : MonoBehaviour
{
    public GameObject startPanel; // Reference to the start screen panel
    public FallingBlockSpawner spawner; // Reference to the FallingBlock
    public GameObject giveUpButton; // Reference to the Give Up button
    public GameObject riceObject; // Reference to the rice object
    public GameObject orderDisplay; // UI Image to display the current order
    //public Sprite[] foodSprites; // List of food sprites
    public TextMeshProUGUI pointsText; // TMP UI for displaying points
    public GameObject crossImage;  // Reference to the cross image prefab (UI)
    public GameObject vImage;
    public GameObject BackgroundInit;
    public GameObject BackgroundGame;
    public TaggedSprite[] foodSprites; // Array of sprites with tags
    private TaggedSprite currentOrder; // The current order's sprite and tag


    //private Sprite currentOrder; // The current order's food sprite
    private int points = 0; // Player points
    public int level = 1; // Current level
    public int correctOrders = 0; // Counter for correct orders
    public int pointsPerLevel = 5; // Number of correct orders to level up
    public float spawnSpeedIncrease = 0.2f; // Decrease spawn interval per level
    public float fallSpeedIncrease = 0.5f; // Increase falling speed per level
    public TextMeshProUGUI levelUpText; // Reference to the LevelUpText
    public TextMeshProUGUI levelNumberText; // Reference to the "Level: X" text
    public float textDisplayDuration = 1f; // Duration for displaying the "Level Up!" text
    private Color originalColor = Color.black; // Original color of the text

    void Start()
    {
        // Prevent the screen from going to sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    public void StartGame()
    {
        // Hide the start panel
        startPanel.SetActive(false);
        BackgroundInit.SetActive(false);
        BackgroundGame.SetActive(true);

        riceObject.SetActive(true);
        giveUpButton.SetActive(true);
        orderDisplay.SetActive(true);
        UpdatePointsDisplay();

        pointsText.gameObject.SetActive(true);

        spawner.StartSpawning();
        GenerateOrder();

        // Start the game (enable food drop, etc.)
        Debug.Log("Game Started!");
    }

    public void RestartGame()
    {
        spawner.StopSpawning();
        DestroyAllFood();

        riceObject.SetActive(false);
        giveUpButton.SetActive(false);
        orderDisplay.SetActive(false);
        pointsText.gameObject.SetActive(false);
        BackgroundGame.SetActive(false);

        BackgroundInit.SetActive(true);

        startPanel.SetActive(true);
        points = 0;
        level = 1;
        correctOrders = 0;
        UpdatePointsDisplay();
        // Reload the current scene to reset the game
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void DestroyAllFood()
    {
        // Array of tags you want to search for
        string[] tags = new string[] { "Avocat", "Crevette", "Daurade", "Choco", "Saumon", "Oeuf", "Omelette" ,"Thon"};

        // Loop through each tag and find objects with that tag
        foreach (string tag in tags)
        {
            // Find all objects with the current tag
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

            // Loop through each object and destroy it
            foreach (GameObject obj in objectsWithTag)
            {
                Destroy(obj);
            }
        }


        Debug.Log("All food objects destroyed!");
    }
    private void GenerateOrder()
    {
        // Randomly pick a food sprite for the order
        currentOrder = foodSprites[Random.Range(0, foodSprites.Length)];

        // Update the order display UI
        orderDisplay.GetComponent<Image>().sprite = currentOrder.sprite;
        orderDisplay.SetActive(true);
    }
    public void CheckFood(GameObject food,int add_points)
    {
        SpriteRenderer foodRenderer = food.GetComponent<SpriteRenderer>();

        if (add_points == -50) 
        {
            points -= 50;
            StartCoroutine(ChangeTextColor(Color.red));

            crossImage.SetActive(true);
            UpdatePointsDisplay();
            StartCoroutine(DestroyMistakeFood(food));
            if (food == null)
            {
                crossImage.SetActive(false);
            }
            // Destroy the food and feedback image after shrinking
        }
        else if (foodRenderer.tag == currentOrder.tag)
        {
            // Correct food
            points += add_points;
            correctOrders++; // Increment correct orders
            StartCoroutine(ChangeTextColor(Color.green));
            vImage.SetActive(true);
            UpdatePointsDisplay();
            StartCoroutine(DestroyGoodFood(food));

            if (correctOrders >= pointsPerLevel)
            {
                LevelUp();
            }

            GenerateOrder();

            UpdatePointsDisplay();
            Debug.Log("Correct food! Points: " + points);
        }
        else
        {
            // Incorrect food
            points -= 50;
            StartCoroutine(ChangeTextColor(Color.red));

            crossImage.SetActive(true);
            UpdatePointsDisplay();

            StartCoroutine(DestroyWrongFood(food));





            Debug.Log("Incorrect food! Points: " + points);
        }

        // Update points display

        // Destroy the food


        // Generate a new order
    }
    private void LevelUp()
    {
        level++; // Increase level
        correctOrders = 0; // Reset the counter
        ShowLevelUpText();

        spawner.AdjustSpeeds(spawnSpeedIncrease, fallSpeedIncrease); // Adjust speeds
        Debug.Log("Level Up! Current Level: " + level);
    }
    private IEnumerator DestroyGoodFood(GameObject food)
    {
        Vector3 originalFoodScale = food.transform.localScale;

        float shrinkDuration = 0.5f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        vImage.SetActive(false);

        // Destroy the food and feedback image after shrinking
        Destroy(food);
    }
    private IEnumerator DestroyMistakeFood(GameObject food)
    {
        Vector3 originalFoodScale = food.transform.localScale;

        float shrinkDuration = 0.3f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        crossImage.SetActive(false);

        // Destroy the food and feedback image after shrinking
        if (food != null) 
        {
            Destroy(food);
        }
    }
    private IEnumerator DestroyWrongFood(GameObject food)
    {
        Vector3 originalFoodScale = food.transform.localScale/2;

        float shrinkDuration = 0.5f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration)
        {
            food.transform.localScale = Vector3.Lerp(originalFoodScale, Vector3.zero, elapsedTime / shrinkDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        crossImage.SetActive(false);

        // Destroy the food and feedback image after shrinking
        Destroy(food);
    }

    private void UpdatePointsDisplay()
    {
        pointsText.text = "Points: " + points + "\nLevel: " + level;

    }
    public void ShowLevelUpText()
    {
        StartCoroutine(LevelUpTextCoroutine());
    }

    private IEnumerator LevelUpTextCoroutine()
    {
        // Ensure the text is fully visible
        levelUpText.alpha = 1;
        levelNumberText.text = "Level: " + level;
        levelNumberText.gameObject.SetActive(true);

        // Wait for the duration before hiding the texts
        yield return new WaitForSeconds(textDisplayDuration);
        levelNumberText.gameObject.SetActive(false);

        // Fade out the text
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            levelUpText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the alpha is fully zero at the end
        levelUpText.alpha = 0;
    }
    private IEnumerator ChangeTextColor(Color newColor)
    {
        // Change the color
        pointsText.color = newColor;

        // Wait for 1 second
        yield return new WaitForSeconds(0.4f);

        // Revert to the original color
        pointsText.color = originalColor;
    }
}