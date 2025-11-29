using UnityEngine;
using TMPro; // Required for TextMeshPro UI components

public class GameTimer : MonoBehaviour
{
    // Assign the TextMeshPro UI component in the Inspector
    public TextMeshProUGUI timerText;
    
    private float startTime;
    private float currentTime;
    
    void Start()
    {
        if (timerText == null)
        {
            Debug.LogError("TimerText reference not set on GameTimer script!");
            enabled = false;
            return;
        }

        // Record the time when the game starts
        startTime = Time.time;
    }

    void Update()
    {
        // 1. Calculate the elapsed time since the game started
        currentTime = Time.time - startTime;

        // 2. Format the time (e.g., 65 seconds becomes 1:05)
        // We only care about seconds for now, rounding to the nearest integer.
        int seconds = Mathf.FloorToInt(currentTime);
        
        // 3. Update the UI text
        timerText.text = "Time: " + seconds.ToString();
    }
}