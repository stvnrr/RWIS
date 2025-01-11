using UnityEngine;

public class FallingBlockSpawner : MonoBehaviour
{
    public GameObject[] foodPrefabs; // Array of food prefabs
    public float spawnInterval; // Time interval between spawns
    private float fallSpeed; // Initial falling speed
    public float InitfallSpeed;
    public float InitspawnInterval; // Time interval between spawns

    private Vector2 screenBounds;
    private bool isSpawning = false; // Tracks whether spawning is active
    public float levelUpDelay; // Delay time after level up (in seconds)
    private bool waitingForNextSpawn = false; // Flag to check if we are waiting for the delay

    void Start()
    {
        screenBounds = GetScreenBounds();
    }

    Vector2 GetScreenBounds()
    {
        Camera cam = Camera.main;

        // Orthographic size is half the screen height in world units
        float screenHeight = cam.orthographicSize * 2;
        float screenWidth = screenHeight * cam.aspect; // Width depends on aspect ratio

        return new Vector2(screenWidth / 2, screenHeight / 2);
    }

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnInterval = InitspawnInterval;
            fallSpeed = InitfallSpeed;
            InvokeRepeating(nameof(SpawnBlock), 0f, spawnInterval);
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnBlock));
    }

    void SpawnBlock()
    {
        if (!isSpawning) return;

        // Randomly select a food prefab
        GameObject selectedPrefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];

        // Get the width of the block sprite
        float blockWidth = selectedPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        // Adjust bounds to account for block size
        float adjustedXBound = screenBounds.x - blockWidth / 2;

        // Create a random X position within adjusted bounds
        float randomX = Random.Range(-adjustedXBound, adjustedXBound);

        // Spawn the block at the top of the screen (screenBounds.y)
        Vector3 spawnPosition = new Vector3(randomX, screenBounds.y, 0);

        // Instantiate the block
        GameObject spawnedBlock = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

        // Set the falling speed for the block
        FoodBehavior fallingBlockScript = spawnedBlock.GetComponent<FoodBehavior>();
        if (fallingBlockScript != null)
        {
            fallingBlockScript.SetFallSpeed(fallSpeed); // Ensure fallSpeed is dynamically updated
        }
    }
    private System.Collections.IEnumerator WaitForNextSpawn()
    {
        waitingForNextSpawn = true;
        yield return new WaitForSeconds(levelUpDelay); // Wait for the delay before continuing
        waitingForNextSpawn = false;
    }
    public void AdjustSpeeds(float spawnDecrease, float fallIncrease)
    {
        levelUpDelay = Mathf.Clamp(levelUpDelay * 1.2f, 1f, 10f); // Gradually increase the delay (up to a max value)

        spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnDecrease); // Prevent spawn interval from becoming too small
        fallSpeed += fallIncrease;
        StartCoroutine(WaitForNextSpawn());

        Debug.Log($"Adjusted Speeds - Spawn Interval: {spawnInterval}, Fall Speed: {fallSpeed}");
    }
    

}
