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

    public event Action OnRotationStarted;

    void Start()
    {
        rotateAudioSource = gameObject.AddComponent<AudioSource>();
        rotateAudioSource.clip = rotateSoundClip;
        rotateAudioSource.loop = true;
        rotateAudioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!isRotating) return;

        pivotPosition = currentPivot.position;
        RotateBar();

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

        currentPivot = pivotDot;
        isRotating = true;

        OnRotationStarted?.Invoke();
    }

    void RotateBar()
    {
        barRect.RotateAround(pivotPosition, Vector3.forward, -rotateSpeed * Time.deltaTime);
    }

    public void StopRotation()
    {
        if (rotateAudioSource.isPlaying)
            rotateAudioSource.Stop();

        isRotating = false;
        AlignBarToPivot();
    }

    void AlignBarToPivot()
    {
        Vector3 offset = currentPivot.position - (currentPivot == dotA ? dotA.position : dotB.position);
        barRect.position += offset;
    }
}
