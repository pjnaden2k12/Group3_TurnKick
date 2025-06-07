using UnityEngine;
using TMPro;

public class LevelPanelUI : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    private void OnEnable()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = $"Best Score\n: {highScore}";
    }
}
