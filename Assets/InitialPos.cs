using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // Assign the circle object prefab in the Inspector
    public int minObjects = 1; // Minimum number of objects to spawn
    public int maxObjects = 10; // Maximum number of objects to spawn

    private Camera mainCamera;
    private Vector2 screenBounds;
    private int numFood;
    void Start()
    {
        mainCamera = Camera.main;

        // Calculate screen bounds based on the camera size
        screenBounds = new Vector2(
            mainCamera.orthographicSize * Screen.width / Screen.height,
            mainCamera.orthographicSize
        );

        // Generate a random number of objects
        numFood = Random.Range(minObjects, maxObjects + 1);

        for (int i = 0; i < numFood; i++)
        {
            SpawnRandomObject();
        }
    }

    void SpawnRandomObject()
    {
        // Generate random x and y positions within the screen bounds
        float randomX = Random.Range(-screenBounds.x, screenBounds.x);
        float randomY = Random.Range(-screenBounds.y, screenBounds.y);

        // Instantiate the object at the random position
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);
        Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
    }
    public int get_numFood()
    {
        return numFood;
    }
}
