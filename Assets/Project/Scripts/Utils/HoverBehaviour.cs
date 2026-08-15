using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class HoverBehaviour : MonoBehaviour
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
        _originalScale = transform.localScale;
    }

    public void HoverStart()
    {
        if (_handle.IsActive())
            _handle.Cancel();

        _handle = LMotion.Create(_originalScale, _originalScale * Scale, Duration)
            .WithEase(Ease)
            .BindToLocalScale(transform);
    }

    public void HoverEnd()
    {
        if (_handle.IsActive())
            _handle.Cancel();

        _handle = LMotion.Create(transform.localScale, _originalScale, Duration)
            .WithEase(Ease)
            .BindToLocalScale(transform);
    }

    private void OnDestroy()
    {
        if (_handle.IsActive())
            _handle.Cancel();
    }

    private void OnMouseEnter()
    {
        if (!CanHover)
            return;

        HoverStart();
    }

    private void OnMouseExit()
    {
        if (!CanHover)
            return;

        HoverEnd();
    }
}
