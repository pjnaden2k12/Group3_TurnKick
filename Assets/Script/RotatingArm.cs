using UnityEngine;

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

        // Reset flags
        dotAHasLeftStartPivot = false;
        dotBHasLeftStartPivot = false;
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
            // Kiểm tra nếu DotB đã rời pivot ban đầu
            if (!dotBHasLeftStartPivot)
            {
                if (!IsDotInPivot(dotB, dotBStartPivot))
                {
                    dotBHasLeftStartPivot = true;
                    Debug.Log("DotB đã rời pivot ban đầu!");
                }
            }

            foreach (GameObject pivot in pivots)
            {
                RectTransform pivotRect = pivot.GetComponent<RectTransform>();

                if (RectTransformUtility.RectangleContainsScreenPoint(pivotRect, dotB.position))
                {
                    // Nếu DotB đã rời pivot ban đầu ít nhất 1 lần thì coi như chạm pivot mới
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
                    Debug.Log("DotA đã rời pivot ban đầu!");
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
        Debug.Log("Dot chạm pivot sau khi đã rời pivot ban đầu! Dừng xoay.");
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
