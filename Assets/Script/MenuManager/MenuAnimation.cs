using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MenuAnimation : MonoBehaviour
{
    public RectTransform logo;
    public RectTransform playButton;
    public RectTransform helpButton;

    void Start()
    {
        AnimateMenu();
    }

    void AnimateMenu()
    {
        // ====== Logo: chạy từ trên xuống ======
        logo.localScale = Vector3.one; // không scale
        Vector3 originalPos = logo.localPosition;
        logo.localPosition = originalPos + new Vector3(0, 800f, 0); // đặt lên trên

        Sequence seq = DOTween.Sequence();
        seq.Append(logo.DOLocalMove(originalPos, 1.3f).SetEase(Ease.OutBack));

        // ====== Play Button: phóng to ======
        playButton.localScale = Vector3.zero;
        seq.Append(playButton.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        // ====== Help Button: phóng to ======
        helpButton.localScale = Vector3.zero;
        seq.Append(helpButton.DOScale(1f, 0.5f).SetEase(Ease.OutBack));
    }
}