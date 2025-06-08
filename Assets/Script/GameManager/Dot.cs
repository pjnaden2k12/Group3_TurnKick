using UnityEngine;

public class DotStopCollider : MonoBehaviour
{
    public ClockwiseController controller;

    private void Start()
    {
        if (controller == null)
        {
            controller = FindFirstObjectByType<ClockwiseController>();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Stop") && controller != null && controller.isRotating)
        {
            Debug.Log("DotStop chạm Stop => Dừng quay");
            controller.StopRotation();
        }
    }
}
