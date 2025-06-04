using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pivot : MonoBehaviour, IPointerClickHandler
{
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite normalSprite;

    protected Image pivotImage;
    protected RectTransform pivotRect;
    protected ClockwishController controller;

    private const float detectionRadius = 160f;

    protected virtual void Start()
    {
        pivotImage = GetComponent<Image>();
        pivotRect = GetComponent<RectTransform>();
        controller = FindFirstObjectByType<ClockwishController>();

        pivotImage.sprite = normalSprite;
    }

    protected virtual void Update()
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

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || controller.isRotating) return;

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
