using UnityEngine;
using UnityEngine.UI;

public class PaddockBehaviour : MonoBehaviour
{
    [SerializeField]
    private PaddockModel _model;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private PaddockUIController _uiController;

    [SerializeField]
    private BoxCollider2D _area;

    [SerializeField]
    private GameObject _dinoPrefab;

    [SerializeField]
    private Transform _spawnRoot;

    private DinoSetuper[] _setups;
    private GameObject[] _dinoRoots;

    public PaddockModel Model
    {
        get
        {
            if (_model == null)
                _model = ScriptableObject.CreateInstance<PaddockModel>();
            return _model;
        }
    }

    private void Awake()
    {
        if (_dinoPrefab == null || _spawnRoot == null)
            return;

        int count = 4;
        _setups = new DinoSetuper[count];
        _dinoRoots = new GameObject[count];

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(_dinoPrefab, _spawnRoot);
            go.name = "Dino_" + i;
            go.SetActive(false);
            _dinoRoots[i] = go;
            _setups[i] = go.GetComponentInChildren<DinoSetuper>();
        }
    }

    private void OnEnable()
    {
        Model.SlotChanged += OnSlotChanged;

        if (_button != null)
            _button.onClick.AddListener(OnClick);

        Refresh();
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.SlotChanged -= OnSlotChanged;

        if (_button != null)
            _button.onClick.RemoveListener(OnClick);
    }

    private void OnSlotChanged(int index)
    {
        if (_model == null || _setups == null || index >= _setups.Length)
            return;

        Dino dino = _model.GetSlot(index);
        DinoSetuper setuper = _setups[index];

        if (setuper == null)
            return;

        if (dino != null)
        {
            Bounds bounds = _area.bounds;
            Vector3 pos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0f);
            _dinoRoots[index].transform.position = pos;
            _dinoRoots[index].SetActive(true);
            setuper.Setup(dino, _area);
        }
        else
        {
            setuper.Clear();
            _dinoRoots[index].SetActive(false);
        }
    }

    private void Refresh()
    {
        if (_model == null || _setups == null)
            return;

        for (int i = 0; i < _setups.Length && i < _model.Count; i++)
            OnSlotChanged(i);
    }

    private void OnClick()
    {
        if (_uiController != null)
        {
            _uiController.SetModel(Model);
            _uiController.Show();
        }
    }

    public void SetModel(PaddockModel model)
    {
        if (_model != null)
            _model.SlotChanged -= OnSlotChanged;

        _model = model;

        if (_model != null)
            _model.SlotChanged += OnSlotChanged;

        Refresh();
    }
}