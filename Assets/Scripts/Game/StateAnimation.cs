using UnityEngine;
using DG.Tweening;

public class StateAnimation : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private CanvasGroup background;
    [SerializeField] private CanvasGroup rays;
    [SerializeField] private CanvasGroup iconRoot;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float visibleDelay = 1.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Rays Settings")]
    [SerializeField] private float raysRotationSpeed = 40f;

    private Tween raysRotationTween;
    private Sequence mainSequence;

    public void Play()
    {
        gameObject.SetActive(true);

        // Сброс состояний
        background.alpha = 0;
        rays.alpha = 0;
        iconRoot.alpha = 0;

        background.interactable = false;
        background.blocksRaycasts = false;

        // Запуск вращения лучей
        raysRotationTween = rays.transform
            .DORotate(new Vector3(0, 0, -360f),
                360f / raysRotationSpeed,
                RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1);

        mainSequence = DOTween.Sequence();

        // Появление
        mainSequence.Append(background.DOFade(1, fadeInDuration));
        mainSequence.Join(rays.DOFade(1, fadeInDuration));
        mainSequence.Join(iconRoot.DOFade(1, fadeInDuration));

        // Задержка
        mainSequence.AppendInterval(visibleDelay);

        // Исчезновение
        mainSequence.Append(background.DOFade(0, fadeOutDuration));
        mainSequence.Join(rays.DOFade(0, fadeOutDuration));
        mainSequence.Join(iconRoot.DOFade(0, fadeOutDuration));

        mainSequence.OnComplete(() =>
        {
            raysRotationTween?.Kill();
            gameObject.SetActive(false);
        });
    }

    private void OnDisable()
    {
        mainSequence?.Kill();
        raysRotationTween?.Kill();
    }
}
