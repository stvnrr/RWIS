using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace
using System.Collections;  // For IEnumerator and coroutines
using System.Linq;  // To handle sorting the scores
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject startPanel; // Reference to the start screen panel
    public FallingBlockSpawner spawner; // Reference to the FallingBlock
    public GameObject bestScoreText; // Reference to the Best Score Text object
    public GameObject scoresButton;  // Reference to the "Scores" button
    public GameObject settingsButton;    // Reference to the "Back" button (to go to the menu)
    public GameObject backButton;    // Reference to the "Back" button (to go to the menu)
    public GameObject backButtonSettings;    // Reference to the "Back" button (to go to the menu)
    public GameObject MusicButton;    // Reference to the "Back" button (to go to the menu)
    public GameObject SoundButton;    // Reference to the "Back" button (to go to the menu)

    public GameObject GyroModeButton;    // Reference to the "Back" button (to go to the menu)
    public GameObject TouchModeButton;    // Reference to the "Back" button (to go to the menu)
    public GameObject checkImageGyro; // The first image (default)
    public GameObject checkImageTouch; // The first image (default)
    public GameObject checkImageSound; // The first image (default)
    public GameObject checkImageMusic; // The first image (default)

    public int ModeNumber = 1;    // Reference to the "Back" button (to go to the menu)
    public int MusicNumber = 1;
    public int SoundNumber = 1;
    public GameObject giveUpButton; // Reference to the Give Up button
    public GameObject riceObject; // Reference to the rice object
    public GameObject orderDisplay; // UI Image to display the current order

    //public Sprite[] foodSprites; // List of food sprites
    public TextMeshProUGUI pointsText; // TMP UI for displaying points
    public TextMeshProUGUI levelText; // TMP UI for displaying points
    public MusicControllerUI musicController; // Reference to the MusicControllerUI script

    public GameObject crossImage;  // Reference to the cross image prefab (UI)
    public GameObject vImage;
    public GameObject BackgroundInit;
    public GameObject BackgroundGame;
    public GameObject BackgroundMiss;
    public GameObject RulesImage;

    public TaggedSprite[] foodSprites; // Array of sprites with tags
    private TaggedSprite currentOrder; // The current order's sprite and tag
    public SpriteRenderer[] lifeSprites; // Array of SpriteRenderers for lives
    private int lives; // Number of remaining lives

    //private Sprite currentOrder; // The current order's food sprite
    private int points = 0; // Player points
    public int level = 1; // Current level
    public int correctOrders = 0; // Counter for correct orders
    public int pointsPerLevel = 5; // Number of correct orders to level up
    public float spawnSpeedIncrease = 0.2f; // Decrease spawn interval per level
    public float fallSpeedIncrease = 0.5f; // Increase falling speed per level
    public GameObject levelUpText; // Reference to the LevelUpText
    public GameObject GameOverText; // Reference to the LevelUpText

    public TextMeshProUGUI levelNumberText; // Reference to the "Level: X" text
    public float textDisplayDuration = 1f; // Duration for displaying the "Level Up!" text
    private Color originalColor = Color.black; // Original color of the text
    public GameObject ScoreImage; // Reference to the LevelUpText
    public GameObject LevelImage; // Reference to the LevelUpText

    private const string BestScoresKey = "BestScores";

    public TextMeshProUGUI bestScoresText;  // UI to display the best scores

    void Start()
    {
        // Prevent the screen from going to sleep
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        bestScoreText.SetActive(false);
        backButton.SetActive(false);
        
        RulesImage.SetActive(false);
        GyroModeButton.SetActive(false);
        TouchModeButton.SetActive(false);
    }
    public void StartGame()
    {
        // Hide the start panel
        scoresButton.SetActive(false);
        settingsButton.SetActive(false);

        startPanel.SetActive(false);
        BackgroundInit.SetActive(false);
        BackgroundGame.SetActive(true);
        if (musicController != null && MusicNumber==1)
        {
            musicController.PlayMusic(); // Start music when the game starts
        }
        riceObject.SetActive(true);
        giveUpButton.SetActive(true);
        ScoreImage.SetActive(true);
        LevelImage.SetActive(true);

        orderDisplay.SetActive(true);
        UpdatePointsDisplay();

        pointsText.gameObject.SetActive(true);
        levelText.gameObject.SetActive(true);

        lives = lifeSprites.Length; // Set lives to match the number of life sprites
        foreach (SpriteRenderer life in lifeSprites)
        {

            life.gameObject.SetActive(true);
        }
        GenerateOrder();
        spawner.StartSpawning();

        // Start the game (enable food drop, etc.)
        Debug.Log("Game Started!");
    }

    public TaggedSprite getCurrentOrder()
    {
        return currentOrder;
    }
    public void GiveUpButton() 
    {
        scoresButton.SetActive(true);
        RestartGame();
    }

    public void RestartGame()
    {
        spawner.StopSpawning();
        DestroyAllFood();

        riceObject.SetActive(false);
        giveUpButton.SetActive(false);
        ScoreImage.SetActive(false);
        LevelImage.SetActive(false);
        foreach (SpriteRenderer life in lifeSprites)
        {

            life.gameObject.SetActive(false);
        }
        orderDisplay.SetActive(false);
        pointsText.gameObject.SetActive(false);
        levelText.gameObject.SetActive(false);
        BackgroundMiss.SetActive(false);

        BackgroundGame.SetActive(false);
        settingsButton.SetActive(true);

        BackgroundInit.SetActive(true);

        startPanel.SetActive(true);
        points = 0;
        level = 1;
        correctOrders = 0;
        UpdatePointsDisplay();
        if (musicController != null)
        {
            musicController.StopMusic(); // Stop music when the game restarts
        }
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
        orderDisplay.GetComponent<SpriteRenderer>().sprite = currentOrder.sprite;
        orderDisplay.SetActive(true);
    }
    public void CheckFood(GameObject food,int add_points)
    {
        SpriteRenderer foodRenderer = food.GetComponent<SpriteRenderer>();

        if (add_points == -50) 
        {
            StartCoroutine(ChangeTextColor(Color.red));
            LoseLife();
            if (lives == 0)
            {
                GameOverText.SetActive(true);

                StartCoroutine(GameOverTextCoroutine());
            }
            crossImage.SetActive(true);
            BackgroundMiss.SetActive(true);

            BackgroundGame.SetActive(false);
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
            StartCoroutine(ChangeTextColor(Color.red));
            LoseLife();
            if (lives == 0)
            {
                GameOverText.SetActive(true);

                StartCoroutine(GameOverTextCoroutine());
            }
            crossImage.SetActive(true);
            BackgroundMiss.SetActive(true);

            BackgroundGame.SetActive(false);
            UpdatePointsDisplay();

            StartCoroutine(DestroyWrongFood(food));

            if (food == null)
            {
                crossImage.SetActive(false);
            }



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
        if (BackgroundMiss.activeSelf)
        {
            BackgroundGame.SetActive(true);
            BackgroundMiss.SetActive(false);
        }
        
        crossImage.SetActive(false);

        // Destroy the food and feedback image after shrinking
        if (food != null) 
        {
            Destroy(food);
        }
        if (lives == 0)
        {
            GameOver();

        }
    }
    private IEnumerator DestroyWrongFood(GameObject food)
    {
        Vector3 originalFoodScale = food.transform.localScale/2;

        float shrinkDuration = 0.5f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration && food!=null)
        {
            food.transform.localScale = Vector3.Lerp(originalFoodScale, Vector3.zero, elapsedTime / shrinkDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (BackgroundMiss.activeSelf) 
        {
            BackgroundGame.SetActive(true);
            BackgroundMiss.SetActive(false);
        }
        

        crossImage.SetActive(false);

        // Destroy the food and feedback image after shrinking
        if (food != null)
        {
            Destroy(food);
        }
        if (lives == 0)
        {
            GameOver();
        }
    }

    private void UpdatePointsDisplay()
    {
        pointsText.text = points.ToString();
        levelText.text = level.ToString();


    }
    public void ShowLevelUpText()
    {
        levelUpText.SetActive(true);

        StartCoroutine(LevelUpTextCoroutine());
    }
    private IEnumerator LevelUpTextCoroutine()
    {
        Vector3 originalLevelUpScale = levelUpText.transform.localScale;

        float shrinkDuration = 0.5f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration)
        {
            levelUpText.transform.localScale = Vector3.Lerp(originalLevelUpScale, Vector3.zero, elapsedTime / shrinkDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        levelUpText.SetActive(false);
        levelUpText.transform.localScale = originalLevelUpScale;

    }
    private IEnumerator GameOverTextCoroutine()
    {
        Vector3 originalGameOverScale = GameOverText.transform.localScale;

        float shrinkDuration = 1f;  // Duration for shrinking
        float elapsedTime = 0f;

        // Shrink the food and feedback image at the same time
        while (elapsedTime < shrinkDuration)
        {
            GameOverText.transform.localScale = Vector3.Lerp(originalGameOverScale, Vector3.zero, elapsedTime / shrinkDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameOverText.SetActive(false);
        GameOverText.transform.localScale = originalGameOverScale;

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
    private void LoseLife()
    {
        if (lives > 0)
        {
            lives--; // Decrement lives
            lifeSprites[lives].gameObject.SetActive(false); // Deactivate the corresponding life GameObject

            // Check if game over

        }
    }
    private void GameOver()
    {
        
        EndGame();  // Save the score and display the best scores

        // Show game over screen or take other actions
        RestartGame(); // You can redirect to restart or a game-over screen
    }
    public void SaveBestScore(int score)
    {
        // Load existing scores from PlayerPrefs (if any)
        var bestScores = LoadBestScoresFromPrefs();

        // Add the new score to the list
        bestScores.Add(score);

        // Sort the list in descending order
        bestScores.Sort((a, b) => b.CompareTo(a));

        // Keep only the top 3 scores
        if (bestScores.Count > 3)
        {
            bestScores = bestScores.Take(3).ToList();
        }

        // Save the updated list back to PlayerPrefs
        SaveBestScoresToPrefs(bestScores);

        // Update the displayed best scores
        DisplayBestScores();
    }
    private void SaveBestScoresToPrefs(List<int> bestScores)
    {
        string scoresString = string.Join(",", bestScores);
        PlayerPrefs.SetString(BestScoresKey, scoresString);
        PlayerPrefs.Save();
    }

    private List<int> LoadBestScoresFromPrefs()
    {
        string scoresString = PlayerPrefs.GetString(BestScoresKey, "");

        if (string.IsNullOrEmpty(scoresString))
        {
            return new List<int>();  // No scores saved yet
        }

        // Convert the string back to a list of integers
        return scoresString.Split(',').Select(int.Parse).ToList();
    }

    public void DisplayBestScores()
    {
        List<int> bestScores = LoadBestScoresFromPrefs();

        // Display the best scores as a string
        bestScoresText.text = "";
        foreach (int score in bestScores)
        {
            bestScoresText.text += score.ToString() + " points\n";
        }
    }
    public void EndGame()
    {
        // Call SaveBestScore after the game ends with the current score
        SaveBestScore(points);  // Assuming `points` is your score variable

        // Optionally, you could show the best scores here as well
        DisplayBestScores();
        StartCoroutine(DisplayBestScoreCoroutine());

    }
    private IEnumerator DisplayBestScoreCoroutine()
    {
        // Activate the best score text
        bestScoreText.SetActive(true);

        // Wait for 3 seconds
        yield return new WaitForSeconds(1);

        // After 3 seconds, deactivate best score text and activate the Scores button
        bestScoreText.SetActive(false);
        scoresButton.SetActive(true);
    }
    public void ShowScores()
    {
        startPanel.SetActive(false);

        // Show the current score when the "Scores" button is pressed
        DisplayBestScores();
        backButton.SetActive(true);

        bestScoreText.SetActive(true);

    }
    public void Settings()
    {
        startPanel.SetActive(false);
        scoresButton.SetActive(false);
        RulesImage.SetActive(true);
        BackgroundInit.SetActive(false);
        // Show the current score when the "Scores" button is pressed
        GyroModeButton.SetActive(true);
        TouchModeButton.SetActive(true);
        MusicButton.SetActive(true);
        SoundButton.SetActive(true);
        backButtonSettings.SetActive(true);
        checkImages();
    }
    public void BackToMenuFromSettings()
    {
        GyroModeButton.SetActive(false);
        TouchModeButton.SetActive(false);
        MusicButton.SetActive(false);
        SoundButton.SetActive(false);
        backButtonSettings.SetActive(false);
        uncheckImages();
        BackgroundInit.SetActive(true);
        RulesImage.SetActive(false);
        scoresButton.SetActive(true);
        startPanel.SetActive(true);


        // Here you would load the initial menu scene
        // SceneManager.LoadScene("MenuSceneName");
    }


    public void Sound()
    {
        if (SoundNumber == 1)
        {
            checkImageSound.SetActive(false);
            SoundNumber = 0;
        }
        else 
        {
            checkImageSound.SetActive(true);
            SoundNumber = 1;
        }

    }
    public void Music()
    {
        if (MusicNumber == 1)
        {
            checkImageMusic.SetActive(false);
            MusicNumber = 0;
        }
        else
        {
            checkImageMusic.SetActive(true);
            MusicNumber = 1;
        }



    }
    public void GyroMode()
    {
        if (ModeNumber != 0)
        {
            checkImageGyro.SetActive(true);
            checkImageTouch.SetActive(false);

            ModeNumber = 0;

        }

    }
    public void TouchMode()
    {
        if (ModeNumber != 1)
        {
            checkImageGyro.SetActive(false);
            checkImageTouch.SetActive(true);

            ModeNumber = 1;
        }
    }
    public void checkImages()
    {
        if (ModeNumber == 0)
        {
            checkImageGyro.SetActive(true);
        }
        else
        {
            checkImageTouch.SetActive(true);
        }
        if (SoundNumber == 1)
        {
            checkImageSound.SetActive(true);
        }
        if (MusicNumber == 1)
        {
            checkImageMusic.SetActive(true);
        }
    }
    public void uncheckImages()
    {
        if (ModeNumber == 0)
        {
            checkImageGyro.SetActive(false);
        }
        else
        {
            checkImageTouch.SetActive(false);
        }
        if (SoundNumber == 1)
        {
            checkImageSound.SetActive(false);
        }
        if (MusicNumber == 1)
        {
            checkImageMusic.SetActive(false);
        }
    }
    public void BackToMenu()
    {
        bestScoreText.SetActive(false);
        scoresButton.SetActive(true);
        backButton.SetActive(false);
        backButton.SetActive(false);
        startPanel.SetActive(true);

        // Here you would load the initial menu scene
        // SceneManager.LoadScene("MenuSceneName");
    }
    
    private void ResetBestScores()
    {
        // Reset best scores to default (an empty list or some default values)
        PlayerPrefs.SetString(BestScoresKey, "");  // Empty string means no scores
        PlayerPrefs.Save();  // Save the changes
    }
}