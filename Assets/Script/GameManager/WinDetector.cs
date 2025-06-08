using UnityEngine;

public class PlayerTriggerWin : MonoBehaviour
{
    private bool touchedShort = false;
    private bool touchedLong = false;

    // Thay vì GameManager, ta lấy LevelManager trực tiếp
    public LevelManager levelManager;

    private void Awake()
    {
        if (levelManager == null)
        {
            levelManager = GameObject.FindFirstObjectByType<LevelManager>();
            if (levelManager == null)
            {
                Debug.LogWarning("Không tìm thấy LevelManager trong scene!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WintargetShort"))
        {
            touchedShort = true;
            CancelInvoke(nameof(ResetShort));
        }

        if (other.CompareTag("WintargetLong"))
        {
            touchedLong = true;
            CancelInvoke(nameof(ResetLong));
        }

        // Kiểm tra Bell
        // Kiểm tra tất cả Bell
        GameObject[] allBells = GameObject.FindGameObjectsWithTag("Bell");
        foreach (GameObject bellObj in allBells)
        {
            BellStateController bell = bellObj.GetComponent<BellStateController>();
            if (bell == null || !bell.HasTouched)
            {
                // Nếu có 1 cái chưa chạm, thì không được tính là thắng
                return;
            }
        }


        bool hasLongTarget = GameObject.Find("WinTargetLong(Clone)") != null;

        if (hasLongTarget)
        {
            if (touchedShort && touchedLong)
            {
                Win();
            }
        }
        else
        {
            if (touchedShort)
            {
                Win();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("WintargetShort"))
        {
            Invoke(nameof(ResetShort), 0.15f);
        }

        if (other.CompareTag("WintargetLong"))
        {
            Invoke(nameof(ResetLong), 0.15f);
        }
    }

    void ResetShort() => touchedShort = false;
    void ResetLong() => touchedLong = false;

    void Win()
    {
        if (levelManager != null)
        {
            levelManager.CompleteLevel();
            Debug.Log("🎉 Level marked as cleared directly from PlayerTriggerWin!");
        }
        else
        {
            Debug.LogWarning("LevelManager chưa được gán trong PlayerTriggerWin!");
        }
        enabled = false;
    }
}
