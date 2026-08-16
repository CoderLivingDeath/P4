using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using LitMotion;
using LitMotion.Extensions;

public class CameraController : MonoBehaviour
{
    public event Action<Location> LocationChanged;

    public Location CurrentLocation => _targetLocation;

    [SerializeField]
    private CinemachineCamera _camera;

    [SerializeField]
    private InputActionReference _move;

    [SerializeField]
    private float _duration = 1f;

    [SerializeField]
    private Ease _ease = Ease.InOutCubic;

    private MotionHandle _moveHandle;

    private float _originX;

    private float _screenWidth;

    private Location _targetLocation = Location.Village;

    private void Awake()
    {
        if (_camera == null)
            return;

        _originX = _camera.transform.position.x;

        Camera output = Camera.main;
        float aspect = output != null ? output.aspect : _camera.Lens.Aspect;
        _screenWidth = _camera.Lens.OrthographicSize * 2f * aspect;
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

        Move(value.x < 0f ? Direction.Left : Direction.Right);
    }

    public void Move(Direction direction)
    {
        Location? target = direction == Direction.Left
            ? LeftNeighbor(_targetLocation)
            : RightNeighbor(_targetLocation);
        if (target != null)
            MoveTo(target.Value);
    }

    public void MoveTo(Location target)
    {
        var camera = _camera;
        if (camera == null || target == _targetLocation)
            return;

        _targetLocation = target;
        LocationChanged?.Invoke(_targetLocation);

        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        float from = camera.transform.position.x;
        float to = _originX + _screenWidth * OffsetFor(target);

        _moveHandle = LMotion.Create(from, to, _duration)
            .WithEase(_ease)
            .BindToPositionX(camera.transform);
    }

    private static Location? LeftNeighbor(Location location) => location switch
    {
        Location.Village => Location.Map,
        Location.Farm => Location.Village,
        _ => null,
    };

    private static Location? RightNeighbor(Location location) => location switch
    {
        Location.Map => Location.Village,
        Location.Village => Location.Farm,
        _ => null,
    };

    private static float OffsetFor(Location location) => location switch
    {
        Location.Map => -1f,
        Location.Farm => 1f,
        _ => 0f,
    };
}
