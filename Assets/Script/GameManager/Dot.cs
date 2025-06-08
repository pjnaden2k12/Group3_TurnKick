using UnityEngine;

public class DotStopCollider : MonoBehaviour
{
    public ClockwiseController controller;
    public bool isPivot = false;

    private RectTransform thisDotRect;

    private void Start()
    {
        if (controller == null)
            controller = GetComponentInParent<ClockwiseController>();

        thisDotRect = GetComponent<RectTransform>();
    }

    public void SetPivotState(bool isThisDotPivot)
    {
        isPivot = isThisDotPivot;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra va chạm với tag Stop
        if (!controller || !controller.isRotating || !other.CompareTag("Stop"))
            return;

        // Kiểm tra nếu Dot này KHÔNG phải là pivot thực tế (được chọn để xoay)
        if (controller.currentPivot == thisDotRect)
        {
            // Dot này là pivot thực sự => bỏ qua va chạm
            return;
        }

        // Dot này KHÔNG phải pivot => hợp lệ để dừng
        Debug.Log($"❗ Dot {gameObject.name} chạm Stop => Dừng quay");
        controller.StopRotation();
    }
}
