using System.Collections.Generic;
using UnityEngine;

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
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index ngoài phạm vi!");
            return;
        }

        ClearLevel();

        LevelData level = levels[levelIndex];
        
        // Tạo pivot
        foreach (var p in level.pivots)
        {
            RectTransform pivot = Instantiate(pivotPrefab, canvasTransform);
            pivot.anchoredPosition = new Vector2(p.x, p.y);
            allPivots.Add(pivot);
        }
        CreateBar(targetBarPrefab, level.targetBar);
        // Tạo bar xoay
        CreateBar(barPrefab, level.bar);

        // Tạo targetBar
        

        Debug.Log($"Đã load Level {levelIndex + 1}");
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
        RectTransform pivot = allPivots[data.pivotIndex];
        RectTransform bar = Instantiate(barPrefab, canvasTransform);

        Vector2 pivotPos = pivot.anchoredPosition;

        // Offset từ tâm thanh đến DotA (vì DotA đặt tại (0, -80) trong local space)
        Vector2 dotAOffset = new Vector2(0, -80f);

        // Xoay offset theo góc xoay của thanh
        Vector2 rotatedOffset = RotateOffset(dotAOffset, data.rotation);

        // Đặt thanh sao cho DotA nằm chính xác tại pivot
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



    // Gọi hàm này để load level kế tiếp
    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }
}
