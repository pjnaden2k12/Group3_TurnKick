using UnityEngine;
using System;
using DG.Tweening;

public class ClockwiseController : MonoBehaviour
{
    public GameObject afterimagePrefab; // Prefab làm tàn ảnh
    public float afterimageInterval = 0.05f;
    public float afterimageLifetime = 0.3f;
    private float afterimageTimer = 0f;

    public RectTransform barRect;   // Thanh
    public RectTransform dotA;
    public RectTransform dotB;
    public float rotateSpeed = 100f;

    [HideInInspector] public bool isRotating = false;
    [HideInInspector] public RectTransform currentPivot;

    private Vector3 pivotPosition;

    private RectTransform dotAStartPivot = null;
    private RectTransform dotBStartPivot = null;

    private bool dotAHasLeftStartPivot = false;
    private bool dotBHasLeftStartPivot = false;

    // Event phát khi bắt đầu xoay
    public event Action OnRotationStarted;

    void Update()
    {
        if (isRotating)
        {
            RotateBar();
            CheckCollision();

            afterimageTimer += Time.deltaTime;
            if (afterimageTimer >= afterimageInterval)
            {
                CreateAfterimage();
                afterimageTimer = 0f;
            }
        }
    }
    void CreateAfterimage()
    {
        GameObject ghost = Instantiate(afterimagePrefab, barRect.position, barRect.rotation, barRect.parent);
        ghost.transform.localScale = barRect.localScale;

        // Làm mờ dần và tự hủy sau một thời gian
        CanvasGroup cg = ghost.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        ghost.transform.SetAsFirstSibling(); // Cho nó nằm dưới thanh gốc

        DG.Tweening.DOTween.To(() => cg.alpha, x => cg.alpha = x, 0f, afterimageLifetime)
            .OnComplete(() => Destroy(ghost));
    }


    public void StartRotate(RectTransform pivotDot)
    {
        if (isRotating) return;

        currentPivot = pivotDot;
        pivotPosition = currentPivot.position;
        isRotating = true;

        dotAStartPivot = GetPivotUnderDot(dotA);
        dotBStartPivot = GetPivotUnderDot(dotB);

        dotAHasLeftStartPivot = false;
        dotBHasLeftStartPivot = false;

        // Phát sự kiện báo bắt đầu xoay
        OnRotationStarted?.Invoke();
    }

    void RotateBar()
    {
        barRect.RotateAround(pivotPosition, Vector3.forward, -rotateSpeed * Time.deltaTime);
    }

    void CheckCollision()
    {
        GameObject[] pivots = GameObject.FindGameObjectsWithTag("Pivot");

        if (currentPivot == dotA)
        {
            if (!dotBHasLeftStartPivot)
            {
                if (!IsDotInPivot(dotB, dotBStartPivot))
                {
                    dotBHasLeftStartPivot = true;
                }
            }

            foreach (GameObject pivot in pivots)
            {
                RectTransform pivotRect = pivot.GetComponent<RectTransform>();

                if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dotB.position))
                {
                    if (dotBHasLeftStartPivot)
                    {
                        StopRotation();
                        break;
                    }
                }
            }
        }
        else if (currentPivot == dotB)
        {
            if (!dotAHasLeftStartPivot)
            {
                if (!IsDotInPivot(dotA, dotAStartPivot))
                {
                    dotAHasLeftStartPivot = true;
                }
            }

            foreach (GameObject pivot in pivots)
            {
                RectTransform pivotRect = pivot.GetComponent<RectTransform>();

                if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dotA.position))
                {
                    if (dotAHasLeftStartPivot)
                    {
                        StopRotation();
                        break;
                    }
                }
            }
        }
    }

    void StopRotation()
    {
        isRotating = false;
    }

    RectTransform GetPivotUnderDot(RectTransform dot)
    {
        GameObject[] pivots = GameObject.FindGameObjectsWithTag("Pivot");

        foreach (GameObject pivot in pivots)
        {
            RectTransform pivotRect = pivot.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dot.position))
            {
                return pivotRect;
            }
        }

        return null;
    }

    bool IsDotInPivot(RectTransform dot, RectTransform pivot)
    {
        if (pivot == null) return false;
        return RectTransformUtility.RectangleContainsScreenPoint(pivot, dot.position);
    }
}
