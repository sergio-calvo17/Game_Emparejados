using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    [SerializeField] private float startTime = 45f;   // Initial timer value in seconds
    [SerializeField] private TMP_Text timerText;      // Reference to the TextMeshPro text in the UI

    private float currentTime;
    private bool isRunning = true;

    void Start()
    {
        currentTime = startTime;
        UpdateTimerDisplay(currentTime);
    }

    void Update()
    {
        if (!isRunning) return;

        // Decrease time based on frame rate
        currentTime -= Time.deltaTime;

        // Clamp to zero when time runs out
        if (currentTime <= 0f)
        {
            currentTime = 0f;
            isRunning = false;

            // Llamar derrota cuando se acaba el tiempo
            GameManager.Instance.MostrarDerrota();
        }

        UpdateTimerDisplay(currentTime);
    }

    // Update the UI text with the remaining seconds
    private void UpdateTimerDisplay(float time)
    {
        int seconds = Mathf.FloorToInt(Mathf.Max(0f, time));

        if (timerText != null)
            timerText.text = seconds.ToString();
    }
}
