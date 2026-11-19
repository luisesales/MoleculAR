using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SponsorsController : MonoBehaviour
{
    [SerializeField] private Image logoImage;
    [SerializeField] private CanvasGroup logoGroup;
    [SerializeField] private List<Sprite> sponsors;
    [SerializeField] private string nextScene = "Menu";

    [Header("Tempos (s)")]
    [SerializeField] private float fadeIn = 0.5f;
    [SerializeField] private float hold = 1.5f;
    [SerializeField] private float fadeOut = 0.5f;
    [SerializeField] private float pauseBetween = 0.2f;

    [Header("Zoom leve")]
    [SerializeField] private float startScale = 1f;
    [SerializeField] private float endScale = 1.08f;

    private void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        logoGroup.alpha = 0f;
        float total = fadeIn + hold + fadeOut;
        Sequence main = DOTween.Sequence();

        foreach (Sprite sprite in sponsors)
        {
            Sprite current = sprite; 
            Sequence one = DOTween.Sequence();

            one.AppendCallback(() => logoImage.sprite = current);
            one.Append(logoGroup.DOFade(1f, fadeIn));
            one.AppendInterval(hold);
            one.Append(logoGroup.DOFade(0f, fadeOut));            
            one.Insert(0f, logoImage.rectTransform
                .DOScale(endScale, total)
                .From(Vector3.one * startScale)
                .SetEase(Ease.Linear));

            main.Append(one);
            main.AppendInterval(pauseBetween);
        }

        main.OnComplete(() => SceneManager.LoadScene(nextScene));
    }
}
