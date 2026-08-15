using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class HuntController : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private TextMeshProUGUI _counter;

    [SerializeField]
    private DinoList _dinoList;

    [SerializeField]
    private HuntResultOverviewBehaviour _resultBehaviour;

    [SerializeField]
    private float _duration = 10f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _successChance = 0.5f;

    [SerializeField]
    private UnityEvent _onHuntStart;

    [SerializeField]
    private UnityEvent _onHuntStep;

    [SerializeField]
    private UnityEvent _onHuntEnd;

    [SerializeField]
    private UnityEvent _onHuntSuccess;

    [SerializeField]
    private UnityEvent _onHuntFail;

    private float _remaining;

    private int _lastStep;

    private bool _isHunting;

    private void OnEnable()
    {
        if (_startButton != null)
            _startButton.onClick.AddListener(StartHunt);
    }

    private void OnDisable()
    {
        if (_startButton != null)
            _startButton.onClick.RemoveListener(StartHunt);
    }

    private void StartHunt()
    {
        if (_isHunting)
            return;

        _isHunting = true;
        _remaining = _duration;
        _lastStep = Mathf.CeilToInt(_duration);

        UpdateCounter(_lastStep);
        if (_slider != null)
            _slider.SetValueWithoutNotify(1f);

        _onHuntStart?.Invoke();
    }

    private void Update()
    {
        if (!_isHunting)
            return;

        _remaining -= Time.deltaTime;

        if (_slider != null)
            _slider.SetValueWithoutNotify(Mathf.Max(0f, _remaining / _duration));

        int step = Mathf.CeilToInt(Mathf.Max(0f, _remaining));
        if (step != _lastStep)
        {
            _lastStep = step;
            UpdateCounter(step);
            _onHuntStep?.Invoke();
        }

        if (_remaining <= 0f)
        {
            _isHunting = false;
            UpdateCounter(0);
            _onHuntEnd?.Invoke();

            if (Random.value <= _successChance)
            {
                Dino dino = PickRandomDino();
                _onHuntSuccess?.Invoke();
                _resultBehaviour?.OnHuntSuccess(dino);
            }
            else
            {
                _onHuntFail?.Invoke();
                _resultBehaviour?.OnHuntFail();
            }
        }
    }

    private void UpdateCounter(int value)
    {
        if (_counter != null)
            _counter.text = value.ToString();
    }

    private Dino PickRandomDino()
    {
        if (_dinoList == null || _dinoList.Dinos.Count == 0)
            return null;

        return _dinoList.Dinos[Random.Range(0, _dinoList.Dinos.Count)];
    }
}
