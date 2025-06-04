using UnityEngine;
using System;

public class ClockwishController : MonoBehaviour
{
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
        }
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
        barRect.RotateAround(pivotPosition, Vector3.forward, rotateSpeed * Time.deltaTime);
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
