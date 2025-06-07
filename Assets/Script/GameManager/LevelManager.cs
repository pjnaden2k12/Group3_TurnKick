using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    [Header("Data & Prefabs")]
    public LevelData[] levels;

    public RectTransform pivotPrefab;
    public RectTransform clockwiseShortPrefab;
    public RectTransform clockwiseLongPrefab;
    public RectTransform winTargetShortPrefab;
    public RectTransform winTargetLongPrefab;
    public RectTransform bellPrefab;

    [Header("UI & Prefabs")]
    public RectTransform clearTarget;          // Vị trí cố định duy nhất cho ảnh Clear
    public RectTransform clearImagePrefab;     // Prefab ảnh Clear (ẩn sẵn)
    public Button nextLevelButton;              // Nút Next Level
    public Transform canvasTransform;
    public TextMeshProUGUI levelCompleteText;
    [Header("Fail UI")]
    public RectTransform failImagePrefab;
    public Button resetAfterFailButton;

    public TextMeshProUGUI levelText;
    public GameTimer gameTimer;

    private List<RectTransform> allPivots = new();
    private List<RectTransform> allBars = new();
    private List<RectTransform> allBells = new();
    private List<Tween> activeTweens = new();

    private int currentLevelIndex = 0;
    public int CurrentLevelIndex => currentLevelIndex;

    void Start()
    {
        if (gameTimer != null)
            gameTimer.OnTimeUp += OnTimeOut;

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
            levelCompleteText.gameObject.SetActive(false);
            nextLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }
        resetAfterFailButton.onClick.AddListener(OnResetAfterFail);
        resetAfterFailButton.gameObject.SetActive(false);

        LoadLevel(currentLevelIndex);
    }
    void OnResetAfterFail()
    {
        // Xóa fail image nếu có
        Transform failImg = canvasTransform.Find("FailImage");
        if (failImg != null) Destroy(failImg.gameObject);

        resetAfterFailButton.gameObject.SetActive(false);
        LoadLevel(currentLevelIndex); // Reset màn chơi
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levels.Length)
        {
            Debug.LogError("Level index out of range!");
            return;
        }

        currentLevelIndex = levelIndex;

        ClearLevel();

        nextLevelButton.gameObject.SetActive(false);
        levelCompleteText.gameObject.SetActive(false);

        // Nếu level đã hoàn thành, hiện ảnh Clear sẵn ở vị trí cố định, bật nút Next Level
        if (PlayerPrefs.GetInt($"Level_{levelIndex}_Clear", 0) == 1)
        {
            ShowClearAtPosition();
            ShowNextLevelButton();
        }

        LevelData level = levels[levelIndex];

        if (levelText != null)
            levelText.text = (levelIndex + 1).ToString();

        // Tạo pivot, bar, bell... theo data level (giữ nguyên logic của bạn)
        foreach (var p in level.pivots)
        {
            var pivot = Instantiate(pivotPrefab, canvasTransform);
            pivot.anchoredPosition = new Vector2(p.x, p.y);
            pivot.localScale = Vector3.zero;

            Pivot pivotScript = pivot.GetComponent<Pivot>();
            if (pivotScript != null)
            {
                pivotScript.isPivotX = p.isPivotX;
                pivotScript.pivotXEnabled = true;
            }

            allPivots.Add(pivot);

            float delay = 0.3f * allPivots.Count;
            Tween pivotTween = pivot.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack).SetDelay(delay);
            activeTweens.Add(pivotTween);
        }

        RectTransform clockwiseShortBar = CreateBar(clockwiseShortPrefab, level.clockwiseShort, true, 999);
        CreateBar(winTargetShortPrefab, level.winTargetShort, true, 1);

        if (currentLevelIndex >= 10)
        {
            RectTransform clockwiseLongBar = CreateBar(clockwiseLongPrefab, level.clockwiseLong, true, 5, clockwiseShortBar, true);
            CreateBar(winTargetLongPrefab, level.winTargetLong, true, 0);
        }

        if (level.bells != null)
        {
            foreach (var bell in level.bells)
            {
                var bellRect = Instantiate(bellPrefab, canvasTransform);
                bellRect.anchoredPosition = new Vector2(bell.x, bell.y);
                bellRect.localScale = Vector3.zero;
                allBells.Add(bellRect);

                float delay = 0.2f * allBells.Count;
                Tween bellTween = bellRect.DOScale(Vector3.one, 1.2f).SetEase(Ease.OutBack).SetDelay(delay);
                activeTweens.Add(bellTween);
            }
        }

        if (gameTimer != null)
        {
            if ((currentLevelIndex + 1) % 5 == 0)
            {
                gameTimer.gameObject.SetActive(true);
                gameTimer.countdownTime = level.timeLimit;
                gameTimer.StartTimer();
            }
            else
            {
                gameTimer.gameObject.SetActive(false);
            }
        }
    }

    public void ClearLevel()
    {
        foreach (Tween t in activeTweens)
            if (t.IsActive()) t.Kill();
        activeTweens.Clear();

        foreach (var p in allPivots) Destroy(p.gameObject);
        foreach (var b in allBars) Destroy(b.gameObject);
        foreach (var bell in allBells) Destroy(bell.gameObject);

        allPivots.Clear();
        allBars.Clear();
        allBells.Clear();

        // Xóa hết ảnh Clear cũ trên Canvas (nếu có)
        foreach (Transform child in canvasTransform)
        {
            if (child.name.Contains(clearImagePrefab.name))
            {
                Destroy(child.gameObject);
            }
        }
        nextLevelButton.gameObject.SetActive(false);
    }

    public void CompleteLevel()
    {
        PlayerPrefs.SetInt($"Level_{currentLevelIndex}_Clear", 1);
        PlayerPrefs.Save();

        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (currentLevelIndex + 1 > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
            PlayerPrefs.Save();
        }

        // Dừng bộ đếm thời gian và ẩn UI timer khi hoàn thành level
        if (gameTimer != null)
        {
            gameTimer.StopTimer();  // Dừng timer
            gameTimer.gameObject.SetActive(false);  // Ẩn UI timer
        }

        ClearLevel();
        ShowLevelCompleteText();
        ShowClearEffect();

        // Ẩn nút reset sau fail nếu đang hiển thị
        if (resetAfterFailButton != null)
            resetAfterFailButton.gameObject.SetActive(false);

        // Ẩn ảnh fail nếu có
        Transform failImg = canvasTransform.Find("FailImage");
        if (failImg != null)
            Destroy(failImg.gameObject);
    }

    void ShowLevelCompleteText()
    {
        if (levelCompleteText == null) return;

        levelCompleteText.gameObject.SetActive(true);

        // Kill tween đang chạy trên transform này (nếu có)
        levelCompleteText.transform.DOKill();

        // Reset scale ban đầu
        levelCompleteText.transform.localScale = new Vector3(0f, 1f, 1f);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f)
           .Append(levelCompleteText.transform.DOScaleX(1f, 0.6f).SetEase(Ease.OutBack));

        // Tween lặp vô hạn tách riêng
        levelCompleteText.transform.DOScaleY(0.9f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(1f + 0.6f); // delay bằng tổng thời gian delay + tween trước đó

    }

    void ShowClearEffect()
    {
        if (clearImagePrefab == null || clearTarget == null)
        {
            Debug.LogError("Clear effect setup sai!");
            return;
        }

        // Tạo bản sao ảnh Clear trên Canvas, bắt đầu từ scale 0 và vị trí trung tâm canvas
        RectTransform clearImg = Instantiate(clearImagePrefab, canvasTransform);
        clearImg.gameObject.SetActive(true);
        clearImg.localScale = Vector3.zero;
        clearImg.anchoredPosition = Vector2.zero;

        Vector2 targetPos = clearTarget.anchoredPosition;

        Sequence seq = DOTween.Sequence();
        seq.Append(clearImg.DOScale(1.5f, 0.4f).SetEase(Ease.OutBack))
           .AppendInterval(0.3f)
           .Append(clearImg.DOScale(0.4f, 0.6f).SetEase(Ease.InBack))
           .Join(clearImg.DOAnchorPos(targetPos, 0.6f).SetEase(Ease.InQuad))
           .AppendCallback(() =>
           {
               clearImg.localScale = Vector3.one;
               clearImg.anchoredPosition = targetPos;
           })
           .AppendCallback(() =>
           {
               ShowNextLevelButton();
           });
    }

    void ShowClearAtPosition()
    {
        if (clearImagePrefab == null || clearTarget == null) return;

        RectTransform clearImg = Instantiate(clearImagePrefab, clearTarget);
        clearImg.gameObject.SetActive(true);
        clearImg.localScale = Vector3.one;
        clearImg.anchoredPosition = Vector2.zero;
        clearImg.localRotation = Quaternion.identity;
    }

    void ShowNextLevelButton()
    {
        if (nextLevelButton == null) return;

        nextLevelButton.gameObject.SetActive(true);
        nextLevelButton.transform.localScale = Vector3.one;

        nextLevelButton.transform.DOKill();
        nextLevelButton.transform.DOScale(1.1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void OnNextLevelButtonClicked()
    {
        nextLevelButton.gameObject.SetActive(false);
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
            currentLevelIndex = 0;
        LoadLevel(currentLevelIndex);
    }

    void OnTimeOut()
    {
        Debug.Log("⏰ Hết giờ! Bạn thua.");

        // Hiện hiệu ứng fail
        ShowFailEffect();

        // Ẩn những thứ liên quan đến level hiện tại (nếu cần)
        ClearLevel();

        // Ẩn nút next level nếu đang hiện
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);
    }

    void ShowFailEffect()
    {
        if (failImagePrefab == null) return;

        RectTransform failImg = Instantiate(failImagePrefab, canvasTransform);
        failImg.gameObject.name = "FailImage"; // để xóa dễ
        failImg.localScale = Vector3.zero;      // bắt đầu scale 0
        failImg.anchoredPosition = Vector2.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(failImg.DOScale(1.3f, 0.4f).SetEase(Ease.OutBack))   // phóng to
           .Append(failImg.DOScale(1f, 0.3f).SetEase(Ease.InOutSine)); // thu nhỏ về scale 1 (bình thường)

        // Hiện nút reset sau 1.1 giây (0.4 + 0.3 + 0.4 delay)
        resetAfterFailButton.transform.localScale = Vector3.zero;
        resetAfterFailButton.gameObject.SetActive(true);
        resetAfterFailButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetDelay(1.1f);
    }

    RectTransform CreateBar(RectTransform prefab, ClockwiseData data, bool isInteractive, int siblingIndex, RectTransform parent = null, bool keepWorldPosition = false)
    {
        if (data == null || data.pivotIndex < 0 || data.pivotIndex >= allPivots.Count)
            return null;

        RectTransform pivot = allPivots[data.pivotIndex];
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
            bar.rotation = Quaternion.Euler(0, 0, data.rotation);
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

        CanvasGroup cg = bar.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        float delay = 0.3f * allBars.Count;
        Sequence seq = DOTween.Sequence();
        seq.Append(bar.DOAnchorPos(bar.anchoredPosition, 0.8f).SetEase(Ease.OutBack).SetDelay(delay));
        seq.Join(cg.DOFade(1f, 0.8f).SetEase(Ease.Linear));
        activeTweens.Add(seq);

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
        return new Vector2(
            offset.x * Mathf.Cos(rad) - offset.y * Mathf.Sin(rad),
            offset.x * Mathf.Sin(rad) + offset.y * Mathf.Cos(rad)
        );
    }
}
