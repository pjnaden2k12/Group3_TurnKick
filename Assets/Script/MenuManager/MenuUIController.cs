using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;

public class MenuUIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject levelPanel;
    public GameObject helpPanel;
    public GameObject gamePlayPanel;

    [Header("Menu UI")]
    public RectTransform logo;
    public Button playButton;
    public Button helpButton;

    [Header("Back Buttons")]
    public Button backFromHelpButton;
    public Button backFromLevelButton;
    public Button btnBackToLevels;
    public Button btnRestartLevel;

    [Header("Level Panel Items")]
    public RectTransform[] levelButtons;

    [Header("Help Panel Items")]
    public RectTransform[] helpItems;

    [Header("Manager")]
    public LevelManager levelManager;
    public TextMeshProUGUI levelText;
    public Image gamePlayPanelImage;

    public static MenuUIController Instance;
    private int currentLevelIndex = 0;

    private List<Tween> buttonTweens = new List<Tween>();
    private Tween logoTween;
    private Tween panelTween;
    private Tween gamePlayPanelTween;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        helpButton.onClick.AddListener(OnHelpClicked);
        backFromHelpButton.onClick.AddListener(OnBackFromHelp);
        backFromLevelButton.onClick.AddListener(OnBackFromLevel);
        btnBackToLevels.onClick.AddListener(OnBackToLevelsClicked);
        btnRestartLevel.onClick.AddListener(OnRestartLevelClicked);

        levelPanel.SetActive(false);
        helpPanel.SetActive(false);
        gamePlayPanel.SetActive(false);

        AnimateMenu();
    }

    void AnimateMenu()
    {
        RectTransform menu = menuPanel.GetComponent<RectTransform>();
        CanvasGroup cg = menuPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = menuPanel.AddComponent<CanvasGroup>();

        menu.localScale = Vector3.one;
        cg.alpha = 1;

        Vector3 originalPos = logo.localPosition;
        logo.localPosition = originalPos + new Vector3(0, 800f, 0);
        logoTween?.Kill();
        logoTween = logo.DOLocalMove(originalPos, 1f).SetEase(Ease.OutBack);

        playButton.transform.localScale = Vector3.zero;
        helpButton.transform.localScale = Vector3.zero;

        playButton.transform.DOScale(1f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.4f);
        helpButton.transform.DOScale(1f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.6f);
    }

    void AnimatePanel(RectTransform panel, RectTransform[] buttons)
    {
        if (panelTween != null && panelTween.IsActive()) panelTween.Kill();

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

        panel.localScale = Vector3.one * 0.7f;
        cg.alpha = 0;

        panelTween = DOTween.Sequence()
            .Append(panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack))
            .Join(cg.DOFade(1f, 0.4f));

        KillButtonTweens();

        float delay = 0.05f;
        foreach (RectTransform btn in buttons)
        {
            btn.localScale = Vector3.zero;
            Tween t = btn.DOScale(1f, 0.3f).SetEase(Ease.OutBack).SetDelay(delay);
            buttonTweens.Add(t);
            delay += 0.05f;
        }
    }

    void AnimateClosePanel(RectTransform panel, System.Action onComplete)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

        KillButtonTweens();
        if (panelTween != null && panelTween.IsActive()) panelTween.Kill();

        DOTween.Sequence()
            .Append(panel.DOScale(0.8f, 0.5f).SetEase(Ease.InBack))
            .Join(cg.DOFade(0f, 0.5f))
            .OnComplete(() => onComplete?.Invoke());
    }

    void KillButtonTweens()
    {
        foreach (var tween in buttonTweens)
        {
            if (tween.IsActive()) tween.Kill();
        }
        buttonTweens.Clear();
    }

    void OnPlayClicked()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
        UpdateLevelButtons();
        AnimatePanel(levelPanel.GetComponent<RectTransform>(), levelButtons);
    }

    void OnHelpClicked()
    {
        helpPanel.SetActive(true);
        AnimatePanel(helpPanel.GetComponent<RectTransform>(), helpItems);
    }

    void OnBackFromHelp()
    {
        AnimateClosePanel(helpPanel.GetComponent<RectTransform>(), () =>
        {
            helpPanel.SetActive(false);
        });
    }

    void OnBackFromLevel()
    {
        AnimateClosePanel(levelPanel.GetComponent<RectTransform>(), () =>
        {
            levelPanel.SetActive(false);
            ShowMenuPanel();
        });
    }

    void ShowMenuPanel()
    {
        menuPanel.SetActive(true);
        AnimateMenu();
    }

    public void LoadLevelAndStart(int levelIndex)
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (levelIndex > unlocked)
        {
            Debug.Log("🔒 Level chưa được mở!");
            return;
        }

        currentLevelIndex = levelIndex;

        KillButtonTweens();
        if (panelTween != null && panelTween.IsActive()) panelTween.Kill();

        gamePlayPanel.SetActive(true);
        AnimateOpenGamePlayPanel();

        levelPanel.SetActive(false);
        levelText.text = $"{levelIndex + 1}";
        levelManager.LoadLevel(levelIndex);
    }

    void OnBackToLevelsClicked()
    {
        DestroyAllAfterimages();
        KillButtonTweens();
        if (panelTween != null && panelTween.IsActive()) panelTween.Kill();
        if (gamePlayPanelTween != null && gamePlayPanelTween.IsActive()) gamePlayPanelTween.Kill();

        gamePlayPanel.SetActive(false);
        levelPanel.SetActive(true);
        UpdateLevelButtons();
        AnimatePanel(levelPanel.GetComponent<RectTransform>(), levelButtons);
    }

    void OnRestartLevelClicked()
    {
        // Lấy current level từ levelManager
        currentLevelIndex = levelManager.CurrentLevelIndex;
        Debug.Log($"Restarting level {currentLevelIndex + 1}");

        DOTween.KillAll();
        ClearSpawnedPrefabs();
        levelManager.LoadLevel(currentLevelIndex);
    }



    void ClearSpawnedPrefabs()
    {
        levelManager.ClearLevel();
    }

    public void AnimateOpenGamePlayPanel()
    {
        if (gamePlayPanelTween != null && gamePlayPanelTween.IsActive()) gamePlayPanelTween.Kill();

        CanvasGroup cg = gamePlayPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = gamePlayPanel.AddComponent<CanvasGroup>();

        gamePlayPanel.transform.localScale = Vector3.one * 0.7f;
        cg.alpha = 0;
        gamePlayPanelImage.color = new Color(1, 1, 1, 0);

        gamePlayPanelTween = DOTween.Sequence()
            .Append(gamePlayPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack))
            .Join(cg.DOFade(1f, 0.5f))
            .Join(gamePlayPanelImage.DOFade(1f, 0.5f));
    }

    void DestroyAllAfterimages()
    {
        GameObject[] afterimages = GameObject.FindGameObjectsWithTag("Afterimage");
        foreach (GameObject obj in afterimages)
        {
            Destroy(obj);
        }
    }

    public void UpdateLevelButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            Button btn = levelButtons[i].GetComponent<Button>();
            TextMeshProUGUI txt = levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (i <= unlockedLevel)
            {
                levelButtons[i].gameObject.SetActive(true);
                btn.interactable = true;
                if (txt != null) txt.text = $"{i + 1}";
            }
            
            else
            {
                levelButtons[i].gameObject.SetActive(false); // ẩn hoàn toàn
            }
        }
    }

}
