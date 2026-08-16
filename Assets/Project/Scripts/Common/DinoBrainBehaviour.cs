using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class DinoBrainBehaviour : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D _area;

    public void SetArea(BoxCollider2D area) => _area = area;

    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private Vector2 _delayRange = new Vector2(1f, 3f);

    [SerializeField]
    private float _rockAngle = 10f;

    [SerializeField]
    private float _rockFrequency = 4f;

    private MotionHandle _moveHandle;

    private MotionHandle _rockHandle;

    private float _nextMoveTime;
    private bool _isTamed;

    public void StopMoving()
    {
        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        StopRocking();
        _isTamed = true;
    }

    public void Release()
    {
        _isTamed = false;
        _nextMoveTime = Time.time + Random.Range(_delayRange.x, _delayRange.y);
    }

    private void OnEnable()
    {
        _nextMoveTime = Time.time + Random.Range(_delayRange.x, _delayRange.y);
    }

    private void Update()
    {
        if (_isTamed)
            return;

        if (_moveHandle.IsActive())
            return;

        if (Time.time < _nextMoveTime)
            return;

        MoveRandomly();
    }

    private void MoveRandomly()
    {
        if (_area == null)
            return;

        Bounds bounds = _area.bounds;
        Vector3 to = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            transform.position.z);

        Vector3 from = transform.position;
        float distance = Vector3.Distance(from, to);
        float duration = distance / Mathf.Max(_speed, 0.01f);

        _moveHandle = LMotion.Create(from, to, duration)
            .WithEase(Ease.InOutSine)
            .WithOnComplete(OnMoveComplete)
            .BindToPosition(transform);

        StartRocking();
    }

    private void OnMoveComplete()
    {
        StopRocking();
        _nextMoveTime = Time.time + Random.Range(_delayRange.x, _delayRange.y);
    }

    private void StartRocking()
    {
        if (_rockHandle.IsActive())
            _rockHandle.Cancel();

        _rockHandle = LMotion.Create(-_rockAngle, _rockAngle, 1f / _rockFrequency)
            .WithLoops(-1, LoopType.Yoyo)
            .WithEase(Ease.InOutSine)
            .BindToEulerAnglesZ(transform);
    }

    private void StopRocking()
    {
        if (_rockHandle.IsActive())
            _rockHandle.Cancel();

        Vector3 euler = transform.eulerAngles;
        euler.z = 0f;
        transform.eulerAngles = euler;
    }

    private void OnDisable()
    {
        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        if (_rockHandle.IsActive())
            _rockHandle.Cancel();
    }
}
