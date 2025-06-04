using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Level & Prefabs")]
    public LevelData[] levels; // Tập hợp tất cả level (gán trong Inspector)
    public RectTransform pivotPrefab;
    public RectTransform pivotXPrefab; // ✅ Pivot đặc biệt
    public RectTransform barPrefab;
    public RectTransform targetBarPrefab;

    [Header("UI")]
    public Transform canvasTransform;

    private List<RectTransform> allPivots = new List<RectTransform>();
    private List<RectTransform> allBars = new List<RectTransform>();

    private int currentLevelIndex = 0;

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("❌ Level index ngoài phạm vi!");
            return;
        }

        ClearLevel();
        LevelData level = levels[levelIndex];

        // Tạo pivot thường
        foreach (var p in level.pivots)
        {
            RectTransform pivot = Instantiate(pivotPrefab, canvasTransform);
            pivot.anchoredPosition = new Vector2(p.x, p.y);
            allPivots.Add(pivot);
        }

        // ✅ Tạo PivotX đặc biệt
        if (level.pivotXs != null)
        {
            foreach (var p in level.pivotXs)
            {
                RectTransform pivotX = Instantiate(pivotXPrefab, canvasTransform);
                pivotX.anchoredPosition = new Vector2(p.x, p.y);
                allPivots.Add(pivotX); // Có thể dùng để gắn Bar nếu cần
            }
        }

        // Tạo thanh mẫu & xoay
        CreateBar(targetBarPrefab, level.targetBar);
        CreateBar(barPrefab, level.bar);

        Debug.Log($"✅ Đã load Level {levelIndex + 1}");
    }

    void ClearLevel()
    {
        foreach (var pivot in allPivots)
            Destroy(pivot.gameObject);
        allPivots.Clear();

        foreach (var bar in allBars)
            Destroy(bar.gameObject);
        allBars.Clear();
    }

    void CreateBar(RectTransform barPrefab, BarData data)
    {
        if (data.pivotIndex < 0 || data.pivotIndex >= allPivots.Count)
        {
            Debug.LogWarning("⚠️ pivotIndex không hợp lệ!");
            return;
        }

        RectTransform pivot = allPivots[data.pivotIndex];
        RectTransform bar = Instantiate(barPrefab, canvasTransform);

        Vector2 pivotPos = pivot.anchoredPosition;

        // DotA được đặt tại (0, -80) trong local space của thanh
        Vector2 dotAOffset = new Vector2(0, -80f);

        // Xoay offset theo góc thanh
        Vector2 rotatedOffset = RotateOffset(dotAOffset, data.rotation);

        // Đặt thanh sao cho DotA nằm chính xác trên pivot
        bar.anchoredPosition = pivotPos + rotatedOffset;
        bar.localRotation = Quaternion.Euler(0, 0, data.rotation);

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

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }
}
