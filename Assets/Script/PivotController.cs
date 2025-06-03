using UnityEngine;
using UnityEngine.UI;

public class PivotSpriteController : MonoBehaviour
{
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite normalSprite;

    public RectTransform dot1RectTransform;
    public RectTransform dot2RectTransform;

    private Image pivotImage;
    private RectTransform pivotRectTransform;

    void Start()
    {
        pivotImage = GetComponent<Image>();
        pivotRectTransform = GetComponent<RectTransform>();
        pivotImage.sprite = normalSprite;
    }

    void Update()
    {
        Vector2 pivotScreenPos = RectTransformUtility.WorldToScreenPoint(null, pivotRectTransform.position);
        Vector2 dot1ScreenPos = RectTransformUtility.WorldToScreenPoint(null, dot1RectTransform.position);
        Vector2 dot2ScreenPos = RectTransformUtility.WorldToScreenPoint(null, dot2RectTransform.position);

        bool isTouchingDot1 = RectTransformUtility.RectangleContainsScreenPoint(dot1RectTransform, pivotScreenPos);
        bool isTouchingDot2 = RectTransformUtility.RectangleContainsScreenPoint(dot2RectTransform, pivotScreenPos);

        // Tính khoảng cách
        float distanceToDot1 = Vector2.Distance(pivotScreenPos, dot1ScreenPos);
        float distanceToDot2 = Vector2.Distance(pivotScreenPos, dot2ScreenPos);

        Debug.Log($"Distance to Dot1: {distanceToDot1}");
        Debug.Log($"Distance to Dot2: {distanceToDot2}");

        // Kiểm tra chạm
        if (isTouchingDot1 || isTouchingDot2)
        {
            Debug.Log("Pivot đang chạm Dot!");
            pivotImage.sprite = clearSprite;
        }
        // Nếu không chạm nhưng còn trong bán kính 160
        else if (distanceToDot1 <= 160f || distanceToDot2 <= 160f)
        {
            Debug.Log("Pivot không chạm Dot, nhưng trong bán kính 160!");
            pivotImage.sprite = noClearSprite;
        }
        // Ngoài bán kính
        else
        {
            Debug.Log("Pivot ngoài bán kính Dot!");
            pivotImage.sprite = normalSprite;
        }
    }
}
