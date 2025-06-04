using UnityEngine;

public class WinDetector : MonoBehaviour
{
    public string targetTag = "WinTarget";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log("🎉 WIN! Hai chốt đã chạm nhau!");
            // TODO: Gọi sự kiện win ở đây
        }
    }
}