using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LocationButton : MonoBehaviour
{
    [SerializeField]
    private HuntZone _zone;

    [SerializeField]
    private Button _button;

    [SerializeField]
    private TextMeshProUGUI _labelText;

    [SerializeField]
    private TextMeshProUGUI _requirementText;

    [SerializeField]
    private string _lockedFormat = "Required: {0}";

    [Inject]
    private ResourcesManager _resourcesManager;

    [Inject]
    private LocationSettings _settings;

    [Inject]
    private HuntWorldUI _huntWorldUI;

    private LocationEntry _entry;

    private void Start()
    {
        if (_button == null)
            _button = GetComponent<Button>();
        if (_requirementText == null)
            _requirementText = GetComponentInChildren<TextMeshProUGUI>();

        _button.onClick.AddListener(OnClicked);
        _entry = _settings != null ? _settings.GetEntry(_zone) : null;

        if (_resourcesManager == null)
            return;

        _resourcesManager.PeopleChanged += OnPeopleChanged;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(OnClicked);

        if (_resourcesManager == null)
            return;

        _resourcesManager.PeopleChanged -= OnPeopleChanged;
    }

    private void OnClicked()
    {
        string name = _labelText != null ? _labelText.text : _zone.ToString();
        _huntWorldUI.Open(_zone, name);
    }

    private void OnPeopleChanged(int people)
    {
        Refresh();
    }

    private void Refresh()
    {
        bool unlocked = _entry == null || _entry.UnlockedByDefault || _resourcesManager.People >= _entry.PeopleThreshold;

        if (_button != null)
            _button.interactable = unlocked;

        if (_requirementText != null)
        {
            _requirementText.text = unlocked
                ? string.Empty
                : string.Format(_lockedFormat, _entry.PeopleThreshold);
        }
    }
}
