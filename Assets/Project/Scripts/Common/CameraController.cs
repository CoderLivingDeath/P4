using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using LitMotion;
using LitMotion.Extensions;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera _camera;

    [SerializeField]
    private InputActionReference _move;

    [SerializeField]
    private float _duration = 1f;

    [SerializeField]
    private Ease _ease = Ease.InOutCubic;

    [SerializeField]
    private int _minStep = int.MinValue;

    [SerializeField]
    private int _maxStep = int.MaxValue;

    private MotionHandle _moveHandle;

    private float _originX;

    private int _targetStep;

    private void Awake()
    {
        if (_camera != null)
            _originX = _camera.transform.position.x;
    }

    private void OnEnable()
    {
        _move.action.Enable();
        _move.action.performed += OnMovePerformed;
    }

    private void OnDisable()
    {
        _move.action.performed -= OnMovePerformed;
        if (_moveHandle.IsActive())
            _moveHandle.Cancel();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if (value.x == 0f)
            return;

        var camera = _camera;
        if (camera == null)
            return;

        int sign = (int)Mathf.Sign(value.x);
        int nextStep = Mathf.Clamp(_targetStep + sign, _minStep, _maxStep);
        if (nextStep == _targetStep)
            return;

        _targetStep = nextStep;

        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        float screenWidth = camera.Lens.OrthographicSize * 2f * camera.Lens.Aspect;
        float from = camera.transform.position.x;
        float to = _originX + _targetStep * screenWidth;

        _moveHandle = LMotion.Create(from, to, _duration)
            .WithEase(_ease)
            .BindToPositionX(camera.transform);
    }
}
