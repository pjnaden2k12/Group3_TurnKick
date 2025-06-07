using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class Pivot : MonoBehaviour, IPointerClickHandler
{
    [Header("Sprite States")]
    public Sprite clearSprite;
    public Sprite noClearSprite;
    public Sprite normalSprite;
    public Sprite spriteX;

    [Header("Chức năng PivotX")]
    public bool isPivotX = false;
    [HideInInspector] public bool pivotXEnabled = true;

    [Header("Âm thanh")]
    public AudioClip clickSound;
    private AudioSource audioSource;

    private Image pivotImage;
    [HideInInspector] public RectTransform pivotRect;
    private ClockwiseController controller;

    private const float detectionRadius = 160f;

    void Start()
    {
        pivotImage = GetComponent<Image>();
        pivotRect = GetComponent<RectTransform>();
        controller = FindFirstObjectByType<ClockwiseController>();

        if (CompareTag("Untagged") || tag != "Pivot")
            gameObject.tag = "Pivot";

        pivotImage.sprite = normalSprite;

        // Tạo AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!pivotXEnabled && isPivotX)
        {
            pivotImage.sprite = spriteX;
            return;
        }

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
            if (pivotImage.sprite != clearSprite)
                ChangeSpriteWithBounce(clearSprite);
        }
        else if (isNear)
        {
            if (pivotImage.sprite != noClearSprite)
                ChangeSpriteWithBounce(noClearSprite);
        }
        else
        {
            if (pivotImage.sprite != normalSprite)
                ChangeSpriteWithBounce(normalSprite);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || controller.isRotating) return;

        // 🔊 Phát âm thanh click
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        pivotRect.DOKill(); // Dừng tween cũ
        pivotRect.DOScale(0.9f, 0.05f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            pivotRect.DOScale(1.1f, 0.1f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                pivotRect.DOScale(1f, 0.08f);
            });
        });

        ToggleAllPivotXStates();

        if (pivotXEnabled)
        {
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

    void ToggleAllPivotXStates()
    {
        Pivot[] allPivots = FindObjectsByType<Pivot>(FindObjectsSortMode.None);

        foreach (Pivot p in allPivots)
        {
            if (!p.isPivotX) continue;

            if (controller != null &&
                (RectTransformUtility.RectangleContainsScreenPoint(p.pivotRect, controller.dotA.position) ||
                 RectTransformUtility.RectangleContainsScreenPoint(p.pivotRect, controller.dotB.position)))
            {
                continue;
            }

            p.pivotXEnabled = !p.pivotXEnabled;
            p.tag = p.pivotXEnabled ? "Pivot" : "Untagged";
            if (p.pivotXEnabled)
                p.ChangeSpriteWithBounce(p.normalSprite);
            else
                p.ChangeSpriteWithBounce(p.spriteX);
        }
    }

    private void ChangeSpriteWithBounce(Sprite newSprite)
    {
        pivotImage.DOKill();

        pivotRect.DOScale(0.7f, 0.05f).SetEase(Ease.InBack).OnComplete(() =>
        {
            pivotImage.sprite = newSprite;
            pivotRect.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                pivotRect.DOScale(1f, 0.08f);
            });
        });
    }
}
