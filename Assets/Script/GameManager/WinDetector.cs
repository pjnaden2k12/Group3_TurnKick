using UnityEngine;

public class WinConditionChecker : MonoBehaviour
{
    public GameObject clockwiseShort; // Reference đến đối tượng clockwiseShort
    public GameObject clockwiseLong;  // Reference đến đối tượng clockwiseLong
    public string wintargetShortTag = "wintargetShort"; // Tag của mục tiêu chiến thắng cho clockwiseShort
    public string wintargetLongTag = "wintargetLong"; // Tag của mục tiêu chiến thắng cho clockwiseLong

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checking trigger for: " + other.gameObject.name);  // Kiểm tra xem có va chạm xảy ra không

        if (clockwiseLong == null)
        {
            // Kiểm tra clockwiseShort và wintargetShort
            if (clockwiseShort != null && other.CompareTag(wintargetShortTag))
            {
                Debug.Log("You win with clockwiseShort!");  // Thông báo khi chiến thắng
                Win(); // Điều kiện chiến thắng
            }
        }
        else
        {
            // Kiểm tra đồng thời clockwiseShort và wintargetShort, clockwiseLong và wintargetLong
            bool clockwiseShortWin = (clockwiseShort != null && other.CompareTag(wintargetShortTag));
            bool clockwiseLongWin = (clockwiseLong != null && other.CompareTag(wintargetLongTag));

            if (clockwiseShortWin && clockwiseLongWin)
            {
                Debug.Log("You win with both clockwiseShort and clockwiseLong!"); // Thông báo chiến thắng
                Win();
            }
        }
    }


    void Win()
    {
        Debug.Log("You win!"); // Thông báo chiến thắng
        // Thực hiện hành động chiến thắng tại đây (ví dụ: chuyển cảnh, hiển thị UI chiến thắng)
    }
}
