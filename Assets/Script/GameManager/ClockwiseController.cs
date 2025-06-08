using UnityEngine;
using System;
using DG.Tweening;

public class ClockwiseController : MonoBehaviour
{
    public GameObject afterimagePrefab;
    public float afterimageInterval = 0.01f;
    public float afterimageLifetime = 0.1f;

    public RectTransform barRect;
    public RectTransform dotA;
    public RectTransform dotB;
    public float rotateSpeed = 100f;

    [HideInInspector] public bool isRotating = false;
    [HideInInspector] public RectTransform currentPivot;

    private Vector3 pivotPosition;
    private float afterimageTimer = 0f;

    [Header("Âm thanh")]
    public AudioClip rotateSoundClip;
    private AudioSource rotateAudioSource;

    private Vector3 previousDotAPosition;
    private Vector3 previousDotBPosition;
    private Transform originalParentA;
    private Transform originalParentB;



    public event Action OnRotationStarted;

    void Start()
    {
        rotateAudioSource = gameObject.AddComponent<AudioSource>();
        rotateAudioSource.clip = rotateSoundClip;
        rotateAudioSource.loop = true;
        rotateAudioSource.playOnAwake = false;

        previousDotAPosition = dotA.position;
        previousDotBPosition = dotB.position;

        originalParentA = dotA.parent;
        originalParentB = dotB.parent;
    }


    void Update()
    {
        if (!isRotating) return;

        previousDotAPosition = dotA.position;
        previousDotBPosition = dotB.position;

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

    void CreateAfterimage()
    {
        GameObject ghost = Instantiate(afterimagePrefab, barRect.position, barRect.rotation, barRect.parent);
        ghost.transform.localScale = barRect.localScale;

        CanvasGroup cg = ghost.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        ghost.transform.SetAsFirstSibling();

        DOTween.To(() => cg.alpha, x => cg.alpha = x, 0f, afterimageLifetime)
            .SetTarget(ghost)
            .OnComplete(() => Destroy(ghost));
    }

    public void StartRotate(RectTransform pivotDot)
    {
        if (rotateSoundClip != null && !rotateAudioSource.isPlaying)
            rotateAudioSource.Play();

        // Nếu dot đã bị snap vào Pivot -> gỡ ra khỏi Pivot
        if (dotA.parent != originalParentA)
        {
            dotA.SetParent(originalParentA);
        }
        if (dotB.parent != originalParentB)
        {
            dotB.SetParent(originalParentB);
        }

        if (isRotating) return;

        currentPivot = pivotDot;
        isRotating = true;

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

            if (currentPivot == dotA && IsDotInStopWithDirection(dotB, previousDotBPosition, pivotRect))
            {
                StopRotation();  // Dừng quay khi dotB va chạm pivot
                return;
            }

            if (currentPivot == dotB && IsDotInStopWithDirection(dotA, previousDotAPosition, pivotRect))
            {
                StopRotation();  // Dừng quay khi dotA va chạm pivot
                return;
            }
        }
    }


    void StopRotation()
    {
        if (rotateAudioSource.isPlaying)
            rotateAudioSource.Stop();

        isRotating = false;
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

    void SnapDotToPivot(RectTransform dot, RectTransform pivot)
    {
        dot.SetParent(pivot);
        dot.localPosition = Vector3.zero;
    }

    bool IsDotInStopWithDirection(RectTransform dot, Vector3 previousPos, RectTransform stopRect)
    {
        Vector3 currentPos = dot.position;
        Vector3 velocity = currentPos - previousPos;
        Vector3 toStop = stopRect.position - currentPos;

        return toStop.magnitude <= 1f && Vector3.Dot(velocity, toStop) > 0;
    }
}