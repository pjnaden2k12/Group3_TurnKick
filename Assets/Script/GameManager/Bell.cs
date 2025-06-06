using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BellStateController : MonoBehaviour
{
    [Header("Bell Sprites")]
    public Image bellImage;
    public Sprite normalSprite;
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite wasClearButFarSprite;

    [Header("Detection Settings")]
    public float detectDistance = 160f;

    private RectTransform bellRect;
    private RectTransform clockwiseShort;
    private RectTransform clockwiseLong;

    private bool hasTouched = false; // NEW

    private BellState currentState = BellState.Normal;
    private CanvasGroup canvasGroup;
    public bool HasTouched => hasTouched; // Cho phép script khác truy cập trạng thái đã chạm


    private enum BellState
    {
        Normal,
        NoClear,
        Clear,
        WasClearButFar
    }

    void Start()
    {
        bellRect = GetComponent<RectTransform>();

        GameObject[] clocks = GameObject.FindGameObjectsWithTag("Clockwise");
        foreach (var obj in clocks)
        {
            if (obj.name.Contains("Short"))
                clockwiseShort = obj.GetComponent<RectTransform>();
            else if (obj.name.Contains("Long"))
                clockwiseLong = obj.GetComponent<RectTransform>();
        }

        canvasGroup = bellImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = bellImage.gameObject.AddComponent<CanvasGroup>();
    }

    void Update()
    {
        float shortDist = clockwiseShort ? Vector2.Distance(bellRect.anchoredPosition, clockwiseShort.anchoredPosition) : float.MaxValue;
        float longDist = clockwiseLong ? Vector2.Distance(bellRect.anchoredPosition, clockwiseLong.anchoredPosition) : float.MaxValue;

        bool isNearShort = shortDist <= detectDistance;
        bool isNearLong = longDist <= detectDistance;
        bool isNear = isNearShort || isNearLong;

        if (!hasTouched)
        {
            // Chưa từng chạm
            if (isNear)
                ChangeState(BellState.NoClear);
            else
                ChangeState(BellState.Normal);
        }
        else
        {
            // Đã từng chạm
            if (isNear)
                ChangeState(BellState.Clear);
            else
                ChangeState(BellState.WasClearButFar);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Clockwise")) return;

        // Toggle trạng thái đã chạm
        hasTouched = !hasTouched;
    }

    void ChangeState(BellState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        Sprite targetSprite = normalSprite;
        switch (newState)
        {
            case BellState.Clear: targetSprite = clearSprite; break;
            case BellState.NoClear: targetSprite = noClearSprite; break;
            case BellState.WasClearButFar:
                targetSprite = wasClearButFarSprite != null ? wasClearButFarSprite : clearSprite;
                break;
            case BellState.Normal: targetSprite = normalSprite; break;
        }

        bellImage.transform.DOScale(0.8f, 0.1f).OnComplete(() =>
        {
            bellImage.sprite = targetSprite;
            bellImage.transform.DOScale(1f, 0.15f).SetEase(Ease.OutBack);
        });
    }
}
