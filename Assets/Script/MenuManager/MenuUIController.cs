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

    // Tween management
    private List<Tween> buttonTweens = new List<Tween>();
    private Tween logoTween;
    private Tween panelTween;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Button listeners
        playButton.onClick.AddListener(OnPlayClicked);
        helpButton.onClick.AddListener(OnHelpClicked);
        backFromHelpButton.onClick.AddListener(OnBackFromHelp);
        backFromLevelButton.onClick.AddListener(OnBackFromLevel);
        btnBackToLevels.onClick.AddListener(OnBackToLevelsClicked);
        btnRestartLevel.onClick.AddListener(OnRestartLevelClicked);

        // Init
        levelPanel.SetActive(false);
        helpPanel.SetActive(false);
        gamePlayPanel.SetActive(false); // Cho phép bật lúc cần

        AnimateMenu();
    }

    // ===== Animate Menu Panel =====
    void AnimateMenu()
    {
        RectTransform menu = menuPanel.GetComponent<RectTransform>();
        CanvasGroup cg = menuPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = menuPanel.AddComponent<CanvasGroup>();

        menu.localScale = Vector3.one;
        cg.alpha = 1;

        // Logo trượt xuống
        Vector3 originalPos = logo.localPosition;
        logo.localPosition = originalPos + new Vector3(0, 800f, 0);
        logoTween?.Kill();
        logoTween = logo.DOLocalMove(originalPos, 1f).SetEase(Ease.OutBack);

        // Button scale
        playButton.transform.localScale = Vector3.zero;
        helpButton.transform.localScale = Vector3.zero;

        playButton.transform.DOScale(1f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.4f);
        helpButton.transform.DOScale(1f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.6f);
    }

    // ===== Panel Open Logic =====
    void AnimatePanel(RectTransform panel, RectTransform[] buttons)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.gameObject.AddComponent<CanvasGroup>();

        panel.localScale = Vector3.one * 0.7f;
        cg.alpha = 0;

        panelTween?.Kill();
        panelTween = DOTween.Sequence()
            .Append(panel.DOScale(1f, 0.4f).SetEase(Ease.OutBack))
            .Join(cg.DOFade(1f, 0.4f));

        // Clear old tweens
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

    // ===== Button Events =====
    void OnPlayClicked()
    {
        menuPanel.SetActive(false);
        levelPanel.SetActive(true);
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

    // ===== Load Level From LevelButton =====
    public void LoadLevelAndStart(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelManager.levels.Length)
        {
            Debug.LogError($"Level {levelIndex} không hợp lệ!");
            return;
        }

        currentLevelIndex = levelIndex; // ✅ Gán lại để chơi lại đúng level này

        gamePlayPanel.SetActive(true);
        AnimateOpenGamePlayPanel();
        levelPanel.SetActive(false);
        levelText.text = $"{levelIndex + 1}";
        levelManager.LoadLevel(levelIndex);
    }

    // Nút quay về level panel
    void OnBackToLevelsClicked()
    {
        // Tắt gameplay, mở lại level panel
        gamePlayPanel.SetActive(false);
        levelPanel.SetActive(true);
        AnimatePanel(levelPanel.GetComponent<RectTransform>(), levelButtons);
    }

    // Nút chơi lại level hiện tại
    void OnRestartLevelClicked()
    {
        levelManager.LoadLevel(currentLevelIndex);
    }
    public void AnimateOpenGamePlayPanel()
    {
        CanvasGroup cg = gamePlayPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = gamePlayPanel.AddComponent<CanvasGroup>();

        gamePlayPanel.transform.localScale = Vector3.one * 0.7f;
        cg.alpha = 0;
        gamePlayPanelImage.color = new Color(1, 1, 1, 0);  // bắt đầu trong suốt

        Sequence seq = DOTween.Sequence();
        seq.Append(gamePlayPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
        seq.Join(cg.DOFade(1f, 0.5f));
        seq.Join(gamePlayPanelImage.DOFade(1f, 0.5f));  // fade in ảnh panel
    }
}
