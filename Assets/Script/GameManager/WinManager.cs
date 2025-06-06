using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameTimer gameTimer;            // Tham chiếu tới GameTimer
    public TextMeshProUGUI winText;        // UI hiển thị thông báo chiến thắng (nếu cần)

   

    public void WinGame()
    {
        

        Debug.Log("Bạn đã chiến thắng!");

        // Dừng thời gian
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

        

        // Có thể thêm các xử lý khác: dừng game, chuyển scene, v.v.
    }
}
