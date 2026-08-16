using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DinosHolder : MonoBehaviour
{
    [SerializeField]
    private List<Dino> _dinos = new List<Dino>();

    [SerializeField]
    private GameObject _dinoPrefab;

    [SerializeField]
    private GameObject _dinoWCanvasPrefab;

    [SerializeField]
    private Vector3 _canvasOffset;

    [SerializeField]
    private Vector3 _canvasScale = Vector3.one;

    [SerializeField]
    private BoxCollider2D _area;

    [SerializeField]
    private Transform _spawnRoot;

    [SerializeField]
    private FermerBrainBehaviour _fermer;

    private readonly Dictionary<Dino, GameObject> _spawned = new Dictionary<Dino, GameObject>();

    public IReadOnlyList<Dino> Dinos => _dinos;

    private void Start()
    {
        Refresh();
    }

    public void Add(Dino dino)
    {
        if (dino == null || _dinos.Contains(dino))
            return;

        _dinos.Add(dino);
        Spawn(dino);
    }

    public void Remove(Dino dino)
    {
        if (dino == null || !_dinos.Remove(dino))
            return;

        Despawn(dino);
    }

    public void Release(Dino dino)
    {
        if (dino == null || !_spawned.TryGetValue(dino, out GameObject go) || go == null)
            return;

        DinoBrainBehaviour dinoBrain = go.GetComponentInChildren<DinoBrainBehaviour>();
        if (dinoBrain != null)
            dinoBrain.Release();
    }

    public void Clear()
    {
        foreach (var dino in _dinos.ToList())
            Despawn(dino);

        _dinos.Clear();
    }

    public void Refresh()
    {
        foreach (var dino in _dinos.ToList())
        {
            Despawn(dino);
            Spawn(dino);
        }
    }

    private void Spawn(Dino dino)
    {
        if (_dinoPrefab == null || _spawnRoot == null || _area == null)
            return;

        GameObject go = Instantiate(_dinoPrefab, _spawnRoot);
        go.name = $"Dino_{dino.Name}_{_dinos.Count}";

        Vector3 pos = RandomPointInArea();
        pos.z = 0f;
        go.transform.position = pos;
        _spawned[dino] = go;

        DinoSetuper setuper = go.GetComponentInChildren<DinoSetuper>();
        if (setuper != null)
            setuper.Setup(dino, _area);

        go.SetActive(true);

        if (_dinoWCanvasPrefab != null)
        {
            GameObject canvas = Instantiate(_dinoWCanvasPrefab, go.transform, false);
            canvas.name = "DinoWCanvas";
            canvas.transform.localPosition = _canvasOffset;
            canvas.transform.localScale = _canvasScale;

            Button tameButton = canvas.GetComponentInChildren<Button>();
            if (tameButton != null && _fermer != null)
            {
                DinoBrainBehaviour dinoBrain = go.GetComponentInChildren<DinoBrainBehaviour>();
                Dino captured = dino;
                tameButton.onClick.AddListener(() =>
                {
                    if (dinoBrain != null)
                        dinoBrain.StopMoving();
                    _fermer.TameDino(captured, go.transform.position);
                });
            }
        }
    }

    private void Despawn(Dino dino)
    {
        if (!_spawned.TryGetValue(dino, out GameObject go))
            return;

        _spawned.Remove(dino);

        if (go == null)
            return;

        DinoSetuper setuper = go.GetComponentInChildren<DinoSetuper>();
        if (setuper != null)
            setuper.Clear();

        Button tameButton = go.GetComponentInChildren<Button>();
        if (tameButton != null)
            tameButton.onClick.RemoveAllListeners();

        Destroy(go);
    }

    private Vector3 RandomPointInArea()
    {
        Bounds bounds = _area.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector3(x, y, 0f);
    }
}
