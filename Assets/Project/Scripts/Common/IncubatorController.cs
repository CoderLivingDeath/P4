using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using LitMotion;

public class IncubatorController : MonoBehaviour
{
    [SerializeField]
    private int _requiredHits = 3;

    [SerializeField]
    private float _shrinkDuration = 1.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _sweetSpotCenter = 0.5f;

    [SerializeField]
    [Range(0f, 1f)]
    private float _sweetSpotWidth = 0.2f;

    [SerializeField]
    private float _circleMaxSize = 200f;

    [SerializeField]
    private float _circleMinSize = 20f;

    [SerializeField]
    private Color _circleColor = Color.white;

    [SerializeField]
    private Color _gradientStart = Color.white;

    [SerializeField]
    private Color _gradientEnd = new Color(0.5f, 0.5f, 0.5f, 1f);

    [SerializeField]
    private Color _ringColor = Color.white;

    [SerializeField]
    private Color _successColor = Color.green;

    [SerializeField]
    private Color _failColor = Color.red;

    [SerializeField]
    private int _textureSize = 128;

    [SerializeField]
    private UnityEvent _onQteSuccess;

    [SerializeField]
    private UnityEvent _onQteFail;

    public bool CanIncubate = true;

    private int _currentHit;
    private bool _qteActive;
    private GameObject _container;
    private Image _circleImage;
    private Button _circleButton;
    private Image _ringImage;
    private MotionHandle _shrinkHandle;
    private float _progress;

    public void StartQTE()
    {
        if (_qteActive || !CanIncubate)
            return;

        _currentHit = 0;
        _qteActive = true;
        CreateContainer();
        SpawnCircle();
    }

    public void CancelQTE()
    {
        _qteActive = false;
        if (_shrinkHandle.IsActive())
            _shrinkHandle.Cancel();

        Cleanup();
    }

    private void OnDisable()
    {
        CancelQTE();
    }

    private void CreateContainer()
    {
        _container = new GameObject("QTE_Container", typeof(RectTransform));
        _container.transform.SetParent(transform, false);
        RectTransform rt = _container.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void SpawnCircle()
    {
        CleanupCircle();

        float sweetSpotSize = Mathf.Lerp(_circleMaxSize, _circleMinSize, _sweetSpotCenter);

        GameObject circle = new GameObject("QTECircle", typeof(Image), typeof(Button));
        circle.transform.SetParent(_container.transform, false);
        _circleImage = circle.GetComponent<Image>();
        _circleImage.sprite = GenerateCircleSprite();
        _circleImage.color = _gradientStart;
        _circleButton = circle.GetComponent<Button>();
        _circleButton.onClick.AddListener(OnCircleClick);
        _circleImage.rectTransform.sizeDelta = new Vector2(_circleMaxSize, _circleMaxSize);

        GameObject ring = new GameObject("SweetSpotRing", typeof(Image));
        ring.transform.SetParent(_container.transform, false);
        _ringImage = ring.GetComponent<Image>();
        _ringImage.sprite = GenerateRingSprite();
        _ringImage.color = _ringColor;
        _ringImage.raycastTarget = false;
        _ringImage.rectTransform.sizeDelta = new Vector2(sweetSpotSize, sweetSpotSize);

        _progress = 0f;
        _shrinkHandle = LMotion.Create(0f, 1f, _shrinkDuration)
            .WithOnComplete(OnShrinkComplete)
            .Bind(OnShrinkUpdate);
    }

    private void OnShrinkUpdate(float value)
    {
        _progress = value;
        float size = Mathf.Lerp(_circleMaxSize, _circleMinSize, value);
        _circleImage.rectTransform.sizeDelta = new Vector2(size, size);
        _circleImage.color = Color.Lerp(_gradientStart, _gradientEnd, value);
    }

    private void OnCircleClick()
    {
        if (!_qteActive)
            return;

        float lower = _sweetSpotCenter - _sweetSpotWidth * 0.5f;
        float upper = _sweetSpotCenter + _sweetSpotWidth * 0.5f;

        if (_progress >= lower && _progress <= upper)
        {
            _currentHit++;
            if (_currentHit >= _requiredHits)
            {
                _qteActive = false;
                if (_shrinkHandle.IsActive())
                    _shrinkHandle.Cancel();

                _circleImage.color = _successColor;
                CleanupDelayed(0.3f);
                _onQteSuccess?.Invoke();
            }
            else
            {
                if (_shrinkHandle.IsActive())
                    _shrinkHandle.Cancel();

                _circleImage.color = _successColor;
                Invoke(nameof(SpawnCircle), 0.3f);
            }
        }
        else
        {
            _qteActive = false;
            if (_shrinkHandle.IsActive())
                _shrinkHandle.Cancel();

            _circleImage.color = _failColor;
            CleanupDelayed(0.3f);
            _onQteFail?.Invoke();
        }
    }

    private void OnShrinkComplete()
    {
        if (!_qteActive)
            return;

        _qteActive = false;
        _circleImage.color = _failColor;
        CleanupDelayed(0.3f);
        _onQteFail?.Invoke();
    }

    private void CleanupDelayed(float delay)
    {
        Invoke(nameof(Cleanup), delay);
    }

    private void CleanupCircle()
    {
        if (_shrinkHandle.IsActive())
            _shrinkHandle.Cancel();

        if (_ringImage != null)
            Destroy(_ringImage.gameObject);

        if (_circleImage != null)
            Destroy(_circleImage.gameObject);
    }

    private void Cleanup()
    {
        CleanupCircle();
        if (_container != null)
            Destroy(_container);
    }

    private Sprite GenerateCircleSprite()
    {
        Texture2D tex = new Texture2D(_textureSize, _textureSize, TextureFormat.RGBA32, false);
        float radius = _textureSize * 0.5f;
        Vector2 center = new Vector2(radius, radius);
        float r2 = radius * radius;
        for (int y = 0; y < _textureSize; y++)
        {
            for (int x = 0; x < _textureSize; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;
                tex.SetPixel(x, y, dx * dx + dy * dy <= r2 ? Color.white : Color.clear);
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, _textureSize, _textureSize), Vector2.one * 0.5f);
    }

    private Sprite GenerateRingSprite()
    {
        Texture2D tex = new Texture2D(_textureSize, _textureSize, TextureFormat.RGBA32, false);
        float radius = _textureSize * 0.5f;
        float innerRadius = radius * 0.8f;
        float r2 = radius * radius;
        float ir2 = innerRadius * innerRadius;
        Vector2 center = new Vector2(radius, radius);
        for (int y = 0; y < _textureSize; y++)
        {
            for (int x = 0; x < _textureSize; x++)
            {
                float dx = x - center.x;
                float dy = y - center.y;
                float d2 = dx * dx + dy * dy;
                tex.SetPixel(x, y, d2 <= r2 && d2 >= ir2 ? Color.white : Color.clear);
            }
        }
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, _textureSize, _textureSize), Vector2.one * 0.5f);
    }
}