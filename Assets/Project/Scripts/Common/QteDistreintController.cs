using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;

public class QteDistreintController : MonoBehaviour
{
    public Dino CurrentDion;

    public event Action<Dino> SequenceCompleted;

    [Header("Elements")]
    [SerializeField]
    private List<Image> _elements;

    [SerializeField]
    private RectTransform _karetka;

    [Header("Movement")]
    [SerializeField]
    private float _moveSpeed = 200f;

    [Header("Colors")]
    [SerializeField]
    private Color _defaultColor = Color.white;

    [SerializeField]
    private Color _correctColor = Color.green;

    [SerializeField]
    private Color _wrongColor = Color.red;

    [Header("Input")]
    [SerializeField]
    private InputActionReference _pressAction;

    [Header("Sequence")]
    [SerializeField]
    private QteSequence _sequence;

    [SerializeField]
    private int _randomSequenceLength = 4;

    [Header("Events")]
    [SerializeField]
    private UnityEvent _onSequenceComplete;

    [SerializeField]
    private UnityEvent _onSequenceFail;

    private int _currentStepIndex;
    private int _direction = 1;
    private bool _isPlaying;
    private bool _inputBlocked;
    private float _currentX;
    private float _minX;
    private float _maxX;

    private void Awake()
    {
        if (_elements == null || _elements.Count == 0 || _karetka == null)
            return;

        _minX = _elements[0].rectTransform.position.x;
        _maxX = _elements[_elements.Count - 1].rectTransform.position.x;
        _currentX = _minX;

        Vector3 pos = _karetka.position;
        pos.x = _currentX;
        _karetka.position = pos;
    }

    private void OnEnable()
    {
        if (_pressAction != null)
        {
            _pressAction.action.Enable();
            _pressAction.action.performed += OnPressPerformed;
        }
    }

    private void OnDisable()
    {
        if (_pressAction != null)
            _pressAction.action.performed -= OnPressPerformed;
    }

    private void Update()
    {
        if (!_isPlaying || _inputBlocked)
            return;

        _currentX += _moveSpeed * _direction * Time.deltaTime;

        if (_currentX >= _maxX)
        {
            _currentX = _maxX;
            _direction = -1;
        }
        else if (_currentX <= _minX)
        {
            _currentX = _minX;
            _direction = 1;
        }

        Vector3 pos = _karetka.position;
        pos.x = _currentX;
        _karetka.position = pos;
    }

    [ContextMenu("Start QTE")]
    public void StartQTE()
    {
        if (_elements == null || _elements.Count == 0 || _karetka == null)
            return;

        _currentStepIndex = 0;
        _inputBlocked = false;
        _direction = _currentX >= _maxX ? -1 : (_currentX <= _minX ? 1 : _direction);
        RestoreDefaultColors();

        if (_sequence == null || _sequence.Length == 0)
        {
            if (_randomSequenceLength > 0)
            {
                var temp = ScriptableObject.CreateInstance<QteSequence>();
                temp.GenerateRandom(_randomSequenceLength, _elements.Count);
                _sequence = temp;
            }
            else
            {
                return;
            }
        }

        _isPlaying = true;
    }

    public void StartQTE(Dino dino)
    {
        if (dino == null)
            return;

        CurrentDion = dino;
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
        StartQTE();
    }

    public void StopQTE()
    {
        _isPlaying = false;
        _inputBlocked = false;
        CurrentDion = null;
        RestoreDefaultColors();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    private void OnPressPerformed(InputAction.CallbackContext context)
    {
        if (!_isPlaying || _inputBlocked)
            return;

        TryHit();
    }

    private void TryHit()
    {
        int nearest = FindNearestElement();
        if (nearest < 0)
            return;

        // Ignore if already hit (green)
        if (nearest >= 0 && nearest < _elements.Count && _elements[nearest].color == _correctColor)
            return;

        int expected = _sequence.GetElement(_currentStepIndex);

        if (nearest == expected)
        {
            StartCoroutine(FlashElement(nearest, _correctColor, true));
        }
        else
        {
            StartCoroutine(FlashElement(nearest, _wrongColor, false));
        }
    }

    private int FindNearestElement()
    {
        float karetkaX = _karetka.position.x;
        int nearest = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < _elements.Count; i++)
        {
            float dist = Mathf.Abs(_elements[i].rectTransform.position.x - karetkaX);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = i;
            }
        }

        return nearest;
    }

    private IEnumerator FlashElement(int index, Color flashColor, bool correct)
    {
        _inputBlocked = true;

        if (index >= 0 && index < _elements.Count)
            _elements[index].color = flashColor;

        yield return new WaitForSeconds(0.5f);

        if (correct)
        {
            // Keep green — stays until fail/reset
            _currentStepIndex++;

            if (_currentStepIndex >= _sequence.Length)
                CompleteQTE();
        }
        else
        {
            yield return new WaitForSeconds(0.3f);

            RestoreDefaultColors();
            _currentStepIndex = 0;
            _onSequenceFail?.Invoke();
        }

        _inputBlocked = false;
    }

    private void CompleteQTE()
    {
        Dino completedDino = CurrentDion;
        _isPlaying = false;
        _inputBlocked = false;
        CurrentDion = null;

        // Hide first so an exception in a listener cannot leave the QTE on screen.
        transform.localScale = Vector3.zero;

        try
        {
            SequenceCompleted?.Invoke(completedDino);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }

        try
        {
            _onSequenceComplete?.Invoke();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception, this);
        }

        gameObject.SetActive(false);
    }

    private void RestoreDefaultColors()
    {
        foreach (var el in _elements)
        {
            if (el != null)
                el.color = _defaultColor;
        }
    }
}
