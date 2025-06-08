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

    [Header("Audio")]
    public AudioSource audioSource;       // AudioSource dùng chung cho hiệu ứng nút
    public AudioClip clickClip;           // Âm thanh khi click nút
    public AudioClip clearClip;           // Âm thanh khi hiện ảnh Clear
    public AudioClip failClip;            // Âm thanh khi hiện ảnh Fail

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
            nextLevelButton.onClick.AddListener(PlayClickSound);
        }
        resetAfterFailButton.onClick.AddListener(OnResetAfterFail);
        resetAfterFailButton.onClick.AddListener(PlayClickSound);
        resetAfterFailButton.gameObject.SetActive(false);

        LoadLevel(currentLevelIndex);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickClip != null && audioSource.enabled && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    void OnResetAfterFail()
    {
        Transform failImg = canvasTransform.Find("FailImage");
        if (failImg != null) Destroy(failImg.gameObject);

        resetAfterFailButton.gameObject.SetActive(false);
        LoadLevel(currentLevelIndex);
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

        if (PlayerPrefs.GetInt($"Level_{levelIndex}_Clear", 0) == 1)
        {
            ShowClearAtPosition();
            ShowNextLevelButton();
        }

        LevelData level = levels[levelIndex];

        if (levelText != null)
            levelText.text = (levelIndex + 1).ToString();

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
                bellTween.SetTarget(bellRect);
                activeTweens.Add(bellTween);
            }
        }
        
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
            pivotTween.SetTarget(pivot);
            activeTweens.Add(pivotTween);
        }

        CreateBar(winTargetShortPrefab, level.winTargetShort, new Vector2(0, -80f), true, 1);

        // Spawn clockwiseShort với offset mặc định
        Vector2 shortBarOffset = new Vector2(0, -80f);
        RectTransform clockwiseShortBar = CreateBar(clockwiseShortPrefab, level.clockwiseShort, shortBarOffset, true, 999);

        if (currentLevelIndex >= 9)
        {
            // Spawn clockwiseLong với offset riêng, ví dụ (0, 80)
            Vector2 longBarOffset = new Vector2(0, 80f);
            RectTransform clockwiseLongBar = CreateBar(clockwiseLongPrefab, level.clockwiseLong, longBarOffset, true, 5);

            CreateBar(winTargetLongPrefab, level.winTargetLong, new Vector2(0, -80f), true, 0);
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

        foreach (var p in allPivots)
        {
            if (p != null)
            {
                p.transform.DOKill();
                var image = p.GetComponent<UnityEngine.UI.Image>();
                if (image != null) image.DOKill();
                Destroy(p.gameObject);
            }
        }

        foreach (var b in allBars)
        {
            if (b != null)
            {
                b.transform.DOKill();
                Destroy(b.gameObject);
            }
        }

        foreach (var bell in allBells)
        {
            if (bell != null)
            {
                bell.transform.DOKill();
                var image = bell.GetComponent<UnityEngine.UI.Image>();
                if (image != null) image.DOKill();
                Destroy(bell.gameObject);
            }
        }

        allPivots.Clear();
        allBars.Clear();
        allBells.Clear();

        foreach (Transform child in canvasTransform)
        {
            if (child.name.Contains(clearImagePrefab.name))
            {
                child.DOKill();
                Destroy(child.gameObject);
            }
        }

        Transform failImg = canvasTransform.Find("FailImage");
        if (failImg != null)
        {
            failImg.DOKill();
            Destroy(failImg.gameObject);
        }

        nextLevelButton.gameObject.SetActive(false);
        resetAfterFailButton.gameObject.SetActive(false);
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

        if (gameTimer != null)
        {
            gameTimer.StopTimer();
            gameTimer.gameObject.SetActive(false);
        }

        ClearLevel();
        ShowLevelCompleteText();
        ShowClearEffect();

        if (resetAfterFailButton != null)
            resetAfterFailButton.gameObject.SetActive(false);

        Transform failImg = canvasTransform.Find("FailImage");
        if (failImg != null)
            Destroy(failImg.gameObject);
    }

    void ShowLevelCompleteText()
    {
        if (levelCompleteText == null) return;

        levelCompleteText.transform.DOKill();

        levelCompleteText.gameObject.SetActive(true);

        levelCompleteText.transform.localScale = new Vector3(0f, 1f, 1f);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1f)
           .Append(levelCompleteText.transform.DOScaleX(1f, 0.6f).SetEase(Ease.OutBack));

        levelCompleteText.transform.DOScaleY(0.9f, 0.4f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetDelay(1f + 0.6f);
    }

    void ShowClearEffect()
    {
        if (clearImagePrefab == null || clearTarget == null)
        {
            Debug.LogError("Clear effect setup sai!");
            return;
        }
        if (audioSource != null && clearClip != null && audioSource.enabled && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(clearClip);
        }
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

        seq.SetTarget(clearImg);
        activeTweens.Add(seq);
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
        Tween t = nextLevelButton.transform.DOScale(1.1f, 0.5f)
            .SetEase(Ease.OutBack)
            .SetLoops(-1, LoopType.Yoyo);
        t.SetTarget(nextLevelButton.transform);
        activeTweens.Add(t);
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
        ClearLevel();
        ShowFailEffect();

        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(false);
    }

    void ShowFailEffect()
    {
        if (failImagePrefab == null) return;
        if (audioSource != null && failClip != null && audioSource.enabled && audioSource.gameObject.activeInHierarchy)
        {
            audioSource.PlayOneShot(failClip);
        }
        RectTransform failImg = Instantiate(failImagePrefab, canvasTransform);
        failImg.gameObject.name = "FailImage";
        failImg.localScale = Vector3.zero;
        failImg.anchoredPosition = Vector2.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(failImg.DOScale(1.3f, 0.4f).SetEase(Ease.OutBack))
           .Append(failImg.DOScale(1f, 0.3f).SetEase(Ease.InOutSine));

        seq.SetTarget(failImg);
        activeTweens.Add(seq);

        resetAfterFailButton.transform.localScale = Vector3.zero;
        resetAfterFailButton.gameObject.SetActive(true);

        Tween resetTween = resetAfterFailButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        resetTween.SetTarget(resetAfterFailButton.transform);
        activeTweens.Add(resetTween);
    }

    RectTransform CreateBar(RectTransform prefab, ClockwiseData data, Vector2 offset, bool isInteractive, int siblingIndex, RectTransform parent = null)
    {
        if (data == null || data.pivotIndex < 0 || data.pivotIndex >= allPivots.Count)
            return null;

        RectTransform pivot = allPivots[data.pivotIndex];
        Vector2 pivotPos = pivot.anchoredPosition;
        Vector2 rotatedOffset = RotateOffset(offset, data.rotation);
        Vector2 finalPos = pivotPos + rotatedOffset;

        RectTransform bar = Instantiate(prefab, canvasTransform);
        bar.name = prefab.name;
        bar.localScale = Vector3.zero;
        bar.anchoredPosition = finalPos;
        bar.localRotation = Quaternion.Euler(0, 0, data.rotation);

        if (parent != null)
        {
            bar.SetParent(parent, worldPositionStays: true);
        }

        allBars.Add(bar);

        Tween barTween = bar.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack).SetDelay(0.5f);
        barTween.SetTarget(bar);
        activeTweens.Add(barTween);

        if (isInteractive)
        {
            // Thêm logic tương tác nếu cần
        }

        bar.SetSiblingIndex(siblingIndex);

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
}
