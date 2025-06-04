using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public LevelData[] levels;             // Mảng các level (gán trong Inspector)
    public RectTransform targetBarPrefab;
    public RectTransform pivotPrefab;
    public RectTransform barPrefab;
    
    public Transform canvasTransform;

    private List<RectTransform> allPivots = new List<RectTransform>();
    private List<RectTransform> allBars = new List<RectTransform>();

    private int currentLevelIndex = 0;

    void Start()
    {
        
    }

    public void LoadLevel(int levelIndex)
    {
        Debug.Log($"LoadLevel called with index: {levelIndex}");

        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index ngoài phạm vi!");
            return;
        }

        Debug.Log($"[LoadLevel] Bắt đầu load level {levelIndex + 1}");

        ClearLevel();

        LevelData level = levels[levelIndex];
    
        Debug.Log($"Level pivots count: {level.pivots.Length}");

        foreach (var p in level.pivots)
        {
            Debug.Log($"Pivot pos: {p.x}, {p.y}");
            RectTransform pivot = Instantiate(pivotPrefab, canvasTransform);
            pivot.anchoredPosition = new Vector2(p.x, p.y);
            pivot.localScale = Vector3.zero; // Start nhỏ

            allPivots.Add(pivot);

            float delay = 0.3f * allPivots.Count;
            pivot.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack).SetDelay(delay).SetLink(pivot.gameObject);
        }

        // Tạo targetBar
        CreateBar(targetBarPrefab, level.targetBar);
        // Tạo bar xoay
        CreateBar(barPrefab, level.bar);

        Debug.Log($"[LoadLevel] Đã load Level {levelIndex + 1}");
    }


    void ClearLevel()
    {
        foreach (var pivot in allPivots)
        {
            DOTween.Kill(pivot.gameObject);  // Kill tween trước khi destroy
            Destroy(pivot.gameObject);
        }
        allPivots.Clear();

        foreach (var bar in allBars)
        {
            DOTween.Kill(bar.gameObject);    // Kill tween trước khi destroy
            Destroy(bar.gameObject);
        }
        allBars.Clear();
    }

    void CreateBar(RectTransform barPrefab, BarData data)
    {
        RectTransform pivot = allPivots[data.pivotIndex];
        RectTransform bar = Instantiate(barPrefab, canvasTransform);

        Vector2 pivotPos = pivot.anchoredPosition;
        Vector2 dotAOffset = new Vector2(0, -80f);
        Vector2 rotatedOffset = RotateOffset(dotAOffset, data.rotation);
    
        Vector2 startPos = pivotPos + rotatedOffset + new Vector2(0, -200);  // bắt đầu ngoài màn hình
        bar.anchoredPosition = startPos;
        bar.localRotation = Quaternion.Euler(0, 0, data.rotation);
        bar.localScale = Vector3.one;

        CanvasGroup cg = bar.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;

        float delay = 0.3f * allBars.Count;

        Sequence seq = DOTween.Sequence();
        seq.Append(bar.DOAnchorPos(pivotPos + rotatedOffset, 0.8f).SetEase(Ease.OutBack).SetDelay(delay));
        seq.Join(cg.DOFade(1f, 0.8f).SetEase(Ease.Linear));

        // Liên kết tween với bar để auto kill tween khi bar bị destroy
        seq.SetLink(bar.gameObject);

        allBars.Add(bar);
    }

    Vector2 RotateOffset(Vector2 offset, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(
            offset.x * cos - offset.y * sin,
            offset.x * sin + offset.y * cos
        );
    }

    // Gọi hàm này để load level kế tiếp
    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }
}
