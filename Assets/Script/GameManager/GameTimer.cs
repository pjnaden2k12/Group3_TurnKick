using System;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float countdownTime = 60f;
    private float timeRemaining;
    private bool isRunning = false;

    public event Action OnTimeUp;

    public TextMeshProUGUI timerText; // UI hiển thị thời gian (nếu có)

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            OnTimeUp?.Invoke();
        }
        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(timeRemaining);
            timerText.text = seconds.ToString() + "s";
        }
    }

    public void StartTimer()
    {
        timeRemaining = countdownTime;
        isRunning = true;
        UpdateTimerUI();
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTimeRemaining() => timeRemaining;
}
