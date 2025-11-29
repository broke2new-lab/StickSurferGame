using UnityEngine;
using System.Collections;

public class RockSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject rockPrefab;
    public float initialSpawnInterval = 2.0f; // Time between rock spawns when the game starts
    public float minimumSpawnInterval = 0.5f;  // Fastest spawn rate allowed
    public float difficultyRampRate = 0.02f;   // Amount to decrease the interval per second of gameplay

    public float spawnZPosition = 40f; 
    public float spawnXRange = 35f;    

    [Header("Rock Size Randomization")]
    public float minScale = 0.5f;
    public float maxScale = 1.5f;

    private float currentSpawnInterval;
    private float startTime;

    void Start()
    {
        if (rockPrefab == null)
        {
            Debug.LogError("Rock Prefab not assigned to the Spawner!");
            enabled = false;
            return;
        }

        startTime = Time.time;
        currentSpawnInterval = initialSpawnInterval;

        // Start the continuous spawning coroutine
        StartCoroutine(SpawnRocksRoutine());
    }

    IEnumerator SpawnRocksRoutine()
    {
        while (true)
        {
            SpawnRock();
            
            // 1. Calculate the elapsed time
            float elapsedTime = Time.time - startTime;

            // 2. Calculate the new interval
            float intervalDecrease = elapsedTime * difficultyRampRate;
            float newInterval = initialSpawnInterval - intervalDecrease;

            // 3. Clamp the interval to ensure it doesn't get too fast
            currentSpawnInterval = Mathf.Max(newInterval, minimumSpawnInterval);

            // Wait for the dynamically calculated interval
            yield return new WaitForSeconds(currentSpawnInterval);
        }
    }

    void SpawnRock()
    {
        // 1. Calculate a random X position
        float randomX = Random.Range(-spawnXRange, spawnXRange);
        
        // 2. Determine the spawn position
        Vector3 spawnPosition = new Vector3(randomX, 0, spawnZPosition);
        
        // 3. Instantiate the rock
        GameObject newRock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        // 4. Apply random scale
        float randomScale = Random.Range(minScale, maxScale);
        newRock.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }
}