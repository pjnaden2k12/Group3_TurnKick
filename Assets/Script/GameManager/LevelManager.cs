using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Data & Prefabs")]
    public LevelData[] levels;

    public RectTransform pivotPrefab;
    public RectTransform clockwiseShortPrefab;
    public RectTransform clockwiseLongPrefab;
    public RectTransform winTargetShortPrefab;
    public RectTransform winTargetLongPrefab;

    public Transform canvasTransform;
    public TextMeshProUGUI levelText;

    private List<RectTransform> allPivots = new List<RectTransform>();
    private List<RectTransform> allBars = new List<RectTransform>();

    private int currentLevelIndex = 0;

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index out of range!");
            return;
        }

        currentLevelIndex = levelIndex;
        ClearLevel();

        LevelData level = levels[levelIndex];

        if (levelText != null)
            levelText.text = (levelIndex + 1).ToString();

        foreach (var p in level.pivots)
        {
            var pivot = Instantiate(pivotPrefab, canvasTransform);
            pivot.anchoredPosition = new Vector2(p.x, p.y);
            pivot.localScale = Vector3.zero;

            allPivots.Add(pivot);

            float delay = 0.3f * allPivots.Count;
            if (pivot != null)  // Kiểm tra null trước khi tween
                pivot.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack).SetDelay(delay);
        }

        RectTransform clockwiseShortBar = CreateBar(clockwiseShortPrefab, level.clockwiseShort, true, 999);
        CreateBar(winTargetShortPrefab, level.winTargetShort, true, 1);

        if (currentLevelIndex >= 3)
        {
            RectTransform clockwiseLongBar = CreateBar(clockwiseLongPrefab, level.clockwiseLong, true, 5, clockwiseShortBar, keepWorldPosition: true);
            CreateBar(winTargetLongPrefab, level.winTargetLong, true, 0);
        }
    }

    public void ClearLevel()
    {
        // Xóa tất cả các pivot đã spawn
        foreach (var p in allPivots)
        {
            DOTween.Kill(p); // Hủy tween của các pivot
            Destroy(p.gameObject); // Xóa đối tượng pivot
        }
        allPivots.Clear();

        // Xóa tất cả các bar đã spawn
        foreach (var b in allBars)
        {
            DOTween.Kill(b); // Hủy tween của các bar
            Destroy(b.gameObject); // Xóa đối tượng bar
        }
        allBars.Clear();
    }

    RectTransform CreateBar(RectTransform prefab, ClockwiseData data, bool isInteractive, int siblingIndex, RectTransform parent = null, bool keepWorldPosition = false)
    {
        if (data == null || data.pivotIndex < 0 || data.pivotIndex >= allPivots.Count)
            return null;

        RectTransform pivot = allPivots[data.pivotIndex];
        if (pivot == null || pivot.gameObject == null) return null;  // Kiểm tra xem pivot có tồn tại không

        Vector2 pivotPos = pivot.anchoredPosition;
        Vector2 baseOffset = new Vector2(0, -80f);
        Vector2 rotatedOffset = RotateOffset(baseOffset, data.rotation);
        Vector2 finalPos = pivotPos + rotatedOffset;

        RectTransform bar;

        if (parent != null)
        {
            GameObject tempGO = new GameObject("Temp", typeof(RectTransform));
            RectTransform temp = tempGO.GetComponent<RectTransform>();
            temp.SetParent(canvasTransform);
            temp.anchorMin = temp.anchorMax = temp.pivot = new Vector2(0.5f, 0.5f);
            temp.sizeDelta = Vector2.zero;
            temp.anchoredPosition = finalPos;
            temp.localRotation = Quaternion.identity;

            bar = Instantiate(prefab, temp);
            bar.anchoredPosition = Vector2.zero;
            bar.rotation = Quaternion.Euler(0, 0, data.rotation); // Giữ đúng hướng gốc
            bar.SetParent(parent, worldPositionStays: true);

            Destroy(tempGO);
        }
        else
        {
            bar = Instantiate(prefab, canvasTransform);
            bar.anchoredPosition = finalPos;
            bar.localRotation = Quaternion.Euler(0, 0, data.rotation);
        }

        bar.localScale = Vector3.one;

        // Thêm CanvasGroup với alpha ban đầu là 0
        CanvasGroup cg = bar.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // Tạo một sequence cho DOTween
        float delay = 0.3f * allBars.Count;
        Sequence seq = DOTween.Sequence();
        if (bar != null && bar.gameObject != null)  // Kiểm tra xem bar có còn tồn tại không
        {
            seq.Append(bar.DOAnchorPos(bar.anchoredPosition, 0.8f).SetEase(Ease.OutBack).SetDelay(delay));
            seq.Join(cg.DOFade(1f, 0.8f).SetEase(Ease.Linear));
        }

        if (!isInteractive)
        {
            foreach (var script in bar.GetComponents<MonoBehaviour>())
                script.enabled = false;
        }

        bar.SetSiblingIndex(siblingIndex);

        allBars.Add(bar);
        return bar;
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
