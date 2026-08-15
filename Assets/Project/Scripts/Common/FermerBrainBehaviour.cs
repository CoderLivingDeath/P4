using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public enum FermerState
{
    Idle,
    Wandering,
    MovingToTarget,
}

public class FermerBrainBehaviour : MonoBehaviour
{
    [SerializeField]
    private PolygonCollider2D _area;

    [SerializeField]
    private float _speed = 2f;

    [SerializeField]
    private Vector2 _wanderDelayRange = new Vector2(1f, 3f);

    [SerializeField]
    private float _rockAngle = 10f;

    [SerializeField]
    private float _rockFrequency = 4f;

    [SerializeField]
    private float _boundaryMargin = 0.1f;

    public FermerState State;

    private const float Epsilon = 1e-4f;

    private readonly List<Vector2> _vertices = new List<Vector2>();
    private readonly List<Vector3> _path = new List<Vector3>();

    private MotionHandle _moveHandle;
    private MotionHandle _rockHandle;

    private int _pathIndex;
    private float _nextMoveTime;
    private bool _moving;

    public void Wander()
    {
        State = FermerState.Wandering;
        CancelMovement();
        _nextMoveTime = Time.time;
    }

    public void MoveTo(Vector3 target)
    {
        State = FermerState.MovingToTarget;
        CancelMovement();
        MoveAlongPath(FindPath(transform.position, target));
    }

    public void Stop()
    {
        CancelMovement();
        State = FermerState.Idle;
    }

    private void OnEnable()
    {
        if (State == FermerState.Idle)
            Wander();
    }

    private void Update()
    {
        if (_area == null)
            return;

        if (!BuildWorldVertices())
            return;

        if (!IsInsidePolygon(transform.position))
        {
            Vector2 inside = ClampToPolygon(transform.position);
            transform.position = new Vector3(inside.x, inside.y, transform.position.z);
            CancelMovement();
            return;
        }

        if (_moving && !_moveHandle.IsActive())
        {
            if (_pathIndex < _path.Count)
                StartNextLeg();
            else
                OnPathFinished();
            return;
        }

        if (State == FermerState.Wandering && !_moving && Time.time >= _nextMoveTime)
            MoveAlongPath(FindPath(transform.position, RandomPointInPolygon()));
    }

    private void OnPathFinished()
    {
        _moving = false;
        _pathIndex = 0;
        StopRocking();

        if (State == FermerState.Wandering)
            _nextMoveTime = Time.time + Random.Range(_wanderDelayRange.x, _wanderDelayRange.y);
        else if (State == FermerState.MovingToTarget)
            State = FermerState.Idle;
    }

    private void CancelMovement()
    {
        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        StopRocking();
        _moving = false;
        _pathIndex = 0;
        _path.Clear();
    }

    private void MoveAlongPath(List<Vector3> path)
    {
        if (path == null || path.Count == 0)
        {
            OnPathFinished();
            return;
        }

        _path.Clear();
        _path.AddRange(path);
        _pathIndex = 0;
        _moving = true;
        StartNextLeg();
    }

    private void StartNextLeg()
    {
        if (!_moving)
            return;

        Vector3 from = transform.position;
        while (_pathIndex < _path.Count && Vector3.Distance(from, _path[_pathIndex]) < Epsilon)
            _pathIndex++;

        if (_pathIndex >= _path.Count)
        {
            OnPathFinished();
            return;
        }

        if (_moveHandle.IsActive())
            _moveHandle.Cancel();

        Vector3 to = _path[_pathIndex];
        float distance = Vector3.Distance(from, to);
        float duration = distance / Mathf.Max(_speed, 0.01f);

        _moveHandle = LMotion.Create(from, to, duration)
            .WithEase(Ease.InOutSine)
            .WithOnComplete(OnLegComplete)
            .BindToPosition(transform);

        StartRocking();
    }

    private void OnLegComplete()
    {
        _pathIndex++;
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

    private List<Vector3> FindPath(Vector2 start, Vector2 goal)
    {
        if (!BuildWorldVertices())
            return null;

        start = ClampToPolygon(start);
        goal = ClampToPolygon(goal);

        if ((goal - start).sqrMagnitude < Epsilon * Epsilon)
            return new List<Vector3> { new Vector3(goal.x, goal.y, transform.position.z) };

        int vertexCount = _vertices.Count;
        int nodeCount = vertexCount + 2;
        Vector2[] nodes = new Vector2[nodeCount];
        nodes[0] = start;
        nodes[1] = goal;
        for (int i = 0; i < vertexCount; i++)
            nodes[i + 2] = _vertices[i];

        bool[,] visible = new bool[nodeCount, nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            for (int j = i + 1; j < nodeCount; j++)
                visible[i, j] = visible[j, i] = IsVisible(nodes[i], nodes[j]);
        }

        float[] distance = new float[nodeCount];
        int[] previous = new int[nodeCount];
        bool[] visited = new bool[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            distance[i] = float.PositiveInfinity;
            previous[i] = -1;
        }
        distance[0] = 0f;

        for (int iteration = 0; iteration < nodeCount; iteration++)
        {
            int current = -1;
            float best = float.PositiveInfinity;
            for (int i = 0; i < nodeCount; i++)
            {
                if (!visited[i] && distance[i] < best)
                {
                    best = distance[i];
                    current = i;
                }
            }

            if (current < 0 || current == 1)
                break;

            visited[current] = true;
            for (int i = 0; i < nodeCount; i++)
            {
                if (visited[i] || !visible[current, i])
                    continue;

                float candidate = distance[current] + Vector2.Distance(nodes[current], nodes[i]);
                if (candidate < distance[i])
                {
                    distance[i] = candidate;
                    previous[i] = current;
                }
            }
        }

        if (float.IsPositiveInfinity(distance[1]))
            return null;

        List<Vector3> path = new List<Vector3>();
        float z = transform.position.z;
        for (int i = 1; i != -1; i = previous[i])
            path.Add(new Vector3(nodes[i].x, nodes[i].y, z));
        path.Reverse();
        return path;
    }

    private bool BuildWorldVertices()
    {
        _vertices.Clear();
        if (_area.pathCount == 0)
            return false;

        Vector2[] local = _area.GetPath(0);
        for (int i = 0; i < local.Length; i++)
            _vertices.Add(_area.transform.TransformPoint(local[i]));

        return _vertices.Count >= 3;
    }

    private Vector2 ClampToPolygon(Vector2 point)
    {
        if (IsInsidePolygon(point))
            return point;

        int edgeIndex;
        Vector2 boundary = ClosestPointOnBoundary(point, out edgeIndex);
        if (edgeIndex < 0)
            return boundary;

        return boundary + EdgeInwardNormal(edgeIndex) * _boundaryMargin;
    }

    private Vector2 ClosestPointOnBoundary(Vector2 point, out int edgeIndex)
    {
        Vector2 closest = point;
        float closestDistance = float.MaxValue;
        edgeIndex = -1;
        for (int i = 0; i < _vertices.Count; i++)
        {
            Vector2 a = _vertices[i];
            Vector2 b = _vertices[(i + 1) % _vertices.Count];
            Vector2 projected = ClosestPointOnSegment(point, a, b);
            float distance = (projected - point).sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = projected;
                edgeIndex = i;
            }
        }
        return closest;
    }

    private Vector2 EdgeInwardNormal(int edgeIndex)
    {
        Vector2 a = _vertices[edgeIndex];
        Vector2 b = _vertices[(edgeIndex + 1) % _vertices.Count];
        Vector2 direction = (b - a).normalized;
        Vector2 leftNormal = new Vector2(-direction.y, direction.x);
        return SignedArea() > 0f ? leftNormal : -leftNormal;
    }

    private float SignedArea()
    {
        float area = 0f;
        for (int i = 0; i < _vertices.Count; i++)
        {
            Vector2 a = _vertices[i];
            Vector2 b = _vertices[(i + 1) % _vertices.Count];
            area += a.x * b.y - b.x * a.y;
        }
        return area * 0.5f;
    }

    private bool IsInsidePolygon(Vector2 point)
    {
        bool inside = false;
        for (int i = 0, j = _vertices.Count - 1; i < _vertices.Count; j = i++)
        {
            Vector2 a = _vertices[i];
            Vector2 b = _vertices[j];
            if ((a.y > point.y) != (b.y > point.y) &&
                point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
                inside = !inside;
        }
        return inside;
    }

    private Vector2 RandomPointInPolygon()
    {
        Bounds bounds = _area.bounds;
        for (int i = 0; i < 32; i++)
        {
            Vector2 point = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y));
            if (IsInsidePolygon(point))
                return point;
        }
        return transform.position;
    }

    private bool IsVisible(Vector2 a, Vector2 b)
    {
        for (int i = 0; i < _vertices.Count; i++)
        {
            Vector2 c = _vertices[i];
            Vector2 d = _vertices[(i + 1) % _vertices.Count];
            if (SegmentsProperlyIntersect(a, b, c, d))
                return false;
        }
        return true;
    }

    private bool SegmentsProperlyIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        float o1 = Cross(b - a, c - a);
        float o2 = Cross(b - a, d - a);
        float o3 = Cross(d - c, a - c);
        float o4 = Cross(d - c, b - c);
        bool intersectsFirst = (o1 > Epsilon && o2 < -Epsilon) || (o1 < -Epsilon && o2 > Epsilon);
        bool intersectsSecond = (o3 > Epsilon && o4 < -Epsilon) || (o3 < -Epsilon && o4 > Epsilon);
        return intersectsFirst && intersectsSecond;
    }

    private static float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    private static Vector2 ClosestPointOnSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        Vector2 segment = end - start;
        float lengthSquared = segment.sqrMagnitude;
        if (lengthSquared <= 0f)
            return start;

        float t = Mathf.Clamp01(Vector2.Dot(point - start, segment) / lengthSquared);
        return start + segment * t;
    }
}
