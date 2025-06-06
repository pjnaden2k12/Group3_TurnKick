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
        }

        if (other.gameObject.name == "WintargetLong")
        {
            touchedLong = true;
        }

        // Kiểm tra Bell
        GameObject bellObj = GameObject.FindWithTag("Bell");
        if (bellObj != null)
        {
            BellStateController bell = bellObj.GetComponent<BellStateController>();
            if (bell == null || !bell.HasTouched) return; // Không chạm -> không win
        }

        // Kiểm tra win
        bool hasLongTarget = GameObject.Find("WintargetLong") != null;

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

    void Win()
    {
        Debug.Log("Chiến thắng!");
        enabled = false;
    }
}
