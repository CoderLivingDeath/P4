using UnityEngine;
using UnityEngine.EventSystems;
using LitMotion;
using LitMotion.Extensions;

public class HoverBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [field: Header("Hover")]
    [field: SerializeField]
    private float Scale { get; set; } = 1.1f;

    [field: SerializeField]
    private float Duration { get; set; } = 0.2f;

    [field: SerializeField]
    private Ease Ease { get; set; } = Ease.OutQuad;

    private Vector3 _originalScale;
    private MotionHandle _handle;

    public bool CanHover => this.enabled;

    private void Awake()
    {
        var rect = GetComponent<RectTransform>();
        _originalScale = rect.localScale;
    }

    public void HoverStart()
    {
        if (_handle.IsActive())
            _handle.Cancel();

        var rect = GetComponent<RectTransform>();
        _handle = LMotion.Create(_originalScale, _originalScale * Scale, Duration)
            .WithEase(Ease)
            .BindToLocalScale(rect);
    }

    public void HoverEnd()
    {
        if (_handle.IsActive())
            _handle.Cancel();

        var rect = GetComponent<RectTransform>();
        _handle = LMotion.Create(rect.localScale, _originalScale, Duration)
            .WithEase(Ease)
            .BindToLocalScale(rect);
    }

    private void OnDestroy()
    {
        if (_handle.IsActive())
            _handle.Cancel();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!CanHover)
            return;

        HoverStart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!CanHover)
            return;

        HoverEnd();
    }
}