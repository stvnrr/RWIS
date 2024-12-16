using UnityEngine;

public class FallingBlockSpawner : MonoBehaviour
{
    public GameObject[] foodPrefabs; // Array of food prefabs
    public float spawnInterval = 1f; // Time interval between spawns
    public float fallSpeed = 1f; // Initial falling speed

    private Vector2 screenBounds;
    private bool isSpawning = false; // Tracks whether spawning is active

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
    public void AdjustSpeeds(float spawnDecrease, float fallIncrease)
    {
        spawnInterval = Mathf.Max(0.5f, spawnInterval - spawnDecrease); // Prevent spawn interval from becoming too small
        fallSpeed += fallIncrease;
        Debug.Log($"Adjusted Speeds - Spawn Interval: {spawnInterval}, Fall Speed: {fallSpeed}");
    }

}
