using UnityEngine;
using UnityEngine.UI;

public class DebugReset : MonoBehaviour
{
    public Button resetButton;

    void Start()
    {
        resetButton.onClick.AddListener(ResetPlayer);
    }

    void ResetPlayer()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Đã reset toàn bộ PlayerPrefs!");
    }
}
