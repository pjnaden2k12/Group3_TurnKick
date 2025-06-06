using UnityEngine;
using System;
using DG.Tweening;

public class ClockwiseController : MonoBehaviour
{
    public GameObject afterimagePrefab;
    public float afterimageInterval = 0.01f;
    public float afterimageLifetime = 0.1f;
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

    private float previousDotADistance = float.MaxValue;
    private float previousDotBDistance = float.MaxValue;

    public event Action OnRotationStarted;

    void Update()
    {
        if (isRotating)
        {
            pivotPosition = currentPivot.position;
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

        CanvasGroup cg = ghost.AddComponent<CanvasGroup>();
        cg.alpha = 1f;

        ghost.transform.SetAsFirstSibling();

        DOTween.To(() => cg.alpha, x => cg.alpha = x, 0f, afterimageLifetime)
            .OnComplete(() => Destroy(ghost));
    }

    public void StartRotate(RectTransform pivotDot)
    {
        if (isRotating) return;

        currentPivot = pivotDot;
        isRotating = true;

        dotAStartPivot = GetPivotUnderDot(dotA);
        dotBStartPivot = GetPivotUnderDot(dotB);

        dotAHasLeftStartPivot = false;
        dotBHasLeftStartPivot = false;

        previousDotADistance = float.MaxValue;
        previousDotBDistance = float.MaxValue;

        OnRotationStarted?.Invoke();
    }

    void RotateBar()
    {
        barRect.RotateAround(pivotPosition, Vector3.forward, -rotateSpeed * Time.deltaTime);
    }

    void CheckCollision()
    {
        GameObject[] pivots = GameObject.FindGameObjectsWithTag("Pivot");

        foreach (GameObject pivot in pivots)
        {
            RectTransform pivotRect = pivot.GetComponent<RectTransform>();

            if (currentPivot == dotA)
            {
                float currentDist = Vector3.Distance(dotB.position, pivotRect.position);

                if (!dotBHasLeftStartPivot)
                {
                    if (!IsDotInPivot(dotB, dotBStartPivot))
                    {
                        dotBHasLeftStartPivot = true;
                    }
                }

                if (dotBHasLeftStartPivot)
                {
                    // Dừng khi dotB đã đi qua tâm pivot
                    if (previousDotBDistance < 1f && currentDist > previousDotBDistance)
                    {
                        StopRotation();
                        return;
                    }
                    previousDotBDistance = currentDist;
                }
            }
            else if (currentPivot == dotB)
            {
                float currentDist = Vector3.Distance(dotA.position, pivotRect.position);

                if (!dotAHasLeftStartPivot)
                {
                    if (!IsDotInPivot(dotA, dotAStartPivot))
                    {
                        dotAHasLeftStartPivot = true;
                    }
                }

                if (dotAHasLeftStartPivot)
                {
                    if (previousDotADistance < 1f && currentDist > previousDotADistance)
                    {
                        StopRotation();
                        return;
                    }
                    previousDotADistance = currentDist;
                }
            }
        }
    }

    public void StopRotation()
    {
        isRotating = false;
        previousDotADistance = float.MaxValue;
        previousDotBDistance = float.MaxValue;

        AlignBarToPivot();
    }
    void AlignBarToPivot()
    {
        if (currentPivot == dotA)
        {
            Vector3 offset = currentPivot.position - dotA.position;
            barRect.position += offset;
        }
        else if (currentPivot == dotB)
        {
            Vector3 offset = currentPivot.position - dotB.position;
            barRect.position += offset;
        }
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
