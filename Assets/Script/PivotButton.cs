using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Gắn vào mỗi pivot (Button hoặc Image có tag = "Pivot")
/// </summary>
public class PivotButton : MonoBehaviour, IPointerClickHandler
{
    private ClockwishController controller;
    private RectTransform pivotRect;

    void Start()
    {
        controller = FindFirstObjectByType<ClockwishController>();
        pivotRect = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller.isRotating) return;

        // Nếu DotA đang nằm trong pivot này
        if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, controller.dotA.position))
        {
            controller.StartRotate(controller.dotA);
        }
        // Nếu DotB đang nằm trong pivot này
        else if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, controller.dotB.position))
        {
            controller.StartRotate(controller.dotB);
        }
    }
}
