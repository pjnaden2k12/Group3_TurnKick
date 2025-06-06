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

        // Kiểm tra lại mỗi lần va chạm xem đối tượng còn tồn tại không
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
        // Time.timeScale = 0f; // Dừng game nếu cần
        enabled = false; // Tránh gọi Win nhiều lần
    }
}
