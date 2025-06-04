using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class Pivot : MonoBehaviour, IPointerClickHandler
{
    [Header("Sprite States")]
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite normalSprite;

    private Image pivotImage;
    private RectTransform pivotRect;
    private ClockwishController controller;

    private const float detectionRadius = 160f;

    void Start()
    {
        pivotImage = GetComponent<Image>();
        pivotRect = GetComponent<RectTransform>();
        controller = FindFirstObjectByType<ClockwishController>();

        pivotImage.sprite = normalSprite;
    }

    void Update()
    {
        Vector2 pivotScreenPos = RectTransformUtility.WorldToScreenPoint(null, pivotRect.position);
        GameObject[] dots = GameObject.FindGameObjectsWithTag("Dot");

        bool isTouching = false;
        bool isNear = false;

        foreach (GameObject dot in dots)
        {
            RectTransform dotRect = dot.GetComponent<RectTransform>();
            Vector2 dotScreenPos = RectTransformUtility.WorldToScreenPoint(null, dotRect.position);
            float distance = Vector2.Distance(pivotScreenPos, dotScreenPos);

            if (RectTransformUtility.RectangleContainsScreenPoint(dotRect, pivotScreenPos))
            {
                isTouching = true;
                break;
            }
            else if (distance <= detectionRadius)
            {
                isNear = true;
            }
        }

        // Đổi sprite tương ứng
        if (isTouching)
        {
            pivotImage.sprite = clearSprite;
        }
        else if (isNear)
        {
            pivotImage.sprite = noClearSprite;
        }
        else
        {
            pivotImage.sprite = normalSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || controller.isRotating) return;

        // Kiểm tra nếu dot đang nằm trong pivot thì thực hiện xoay
        if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, controller.dotA.position))
        {
            controller.StartRotate(controller.dotA);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, controller.dotB.position))
        {
            controller.StartRotate(controller.dotB);
        }
    }
}
