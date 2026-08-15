using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class PulseBehaviour : MonoBehaviour
{
    [field: Header("Pulse")]
    [field: SerializeField]
    private float PulseScale { get; set; } = 1.15f;

    [field: SerializeField]
    private float Duration { get; set; } = 0.5f;

    [field: SerializeField]
    private Ease Ease { get; set; } = Ease.InOutSine;

    [field: SerializeField]
    private bool PlayOnAwake { get; set; } = true;

    private Vector3 _originScale;
    private MotionHandle _handle;

    private void Awake()
    {
        _originScale = transform.localScale;
    }

    private void Start()
    {
        if (PlayOnAwake)
            StartPulse();
    }

    public void StartPulse()
    {
        StopPulse();

        _handle = LMotion.Create(_originScale, _originScale * Mathf.Max(0f, PulseScale), Mathf.Max(0f, Duration))
            .WithEase(Ease)
            .WithLoops(2, LoopType.Yoyo)
            .BindToLocalScale(transform);
    }

    public void StopPulse()
    {
        if (_handle.IsActive())
            _handle.Cancel();

        transform.localScale = _originScale;
    }

    private void OnDestroy()
    {
        StopPulse();
    }
}
