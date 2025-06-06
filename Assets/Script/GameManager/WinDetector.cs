using UnityEngine;

public class PlayerTriggerWin : MonoBehaviour
{
    private bool touchedShort = false;
    private bool touchedLong = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("WintargetShort"))
        {
            touchedShort = true;
            CancelInvoke(nameof(ResetShort)); // tránh reset sớm
        }

        if (other.CompareTag("WintargetLong"))
        {
            touchedLong = true;
            CancelInvoke(nameof(ResetLong));
        }

        // Kiểm tra Bell
        GameObject bellObj = GameObject.FindWithTag("Bell");
        if (bellObj != null)
        {
            BellStateController bell = bellObj.GetComponent<BellStateController>();
            if (bell == null || !bell.HasTouched) return; // Không chạm -> không win
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
            Invoke(nameof(ResetShort), 0.15f); // đợi 0.2s rồi mới reset
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
        Debug.Log("Chiến thắng!");
        enabled = false;
    }
}
