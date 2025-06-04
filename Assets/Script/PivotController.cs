using UnityEngine;
using UnityEngine.UI;

public class PivotSpriteController : MonoBehaviour
{
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite normalSprite;

    private Image pivotImage;
    private RectTransform pivotRectTransform;

    private const float detectionRadius = 160f;

    void Start()
    {
        pivotImage = GetComponent<Image>();
        pivotRectTransform = GetComponent<RectTransform>();
        pivotImage.sprite = normalSprite;
    }

    void Update()
    {
        Vector2 pivotScreenPos = RectTransformUtility.WorldToScreenPoint(null, pivotRectTransform.position);

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
}