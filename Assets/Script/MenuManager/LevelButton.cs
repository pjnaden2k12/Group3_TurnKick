using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public int levelIndex; // Gán trong Inspector
    public Button button;

    private void Start()
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(OnLevelClicked);
    }

    void OnLevelClicked()
    {
        MenuUIController.Instance.LoadLevelAndStart(levelIndex);
    }
}