using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class CanvasGroupFader : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _fadeDuration = 0.5f;

    [SerializeField]
    private Ease _ease = Ease.InOutQuad;

    [SerializeField]
    private bool _startHidden;

    private MotionHandle _fadeHandle;

    private void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        if (_startHidden && _canvasGroup != null)
            _canvasGroup.alpha = 0f;
    }

    private void OnDisable()
    {
        if (_fadeHandle.IsActive())
            _fadeHandle.Cancel();
    }

    public void FadeIn()
    {
        FadeTo(1f);
    }

    public void FadeOut()
    {
        FadeTo(0f);
    }

    private void FadeTo(float target)
    {
        if (_canvasGroup == null)
            return;

        if (_fadeHandle.IsActive())
            _fadeHandle.Cancel();

        _fadeHandle = LMotion.Create(_canvasGroup.alpha, target, _fadeDuration)
            .WithEase(_ease)
            .BindToAlpha(_canvasGroup);
    }
}
