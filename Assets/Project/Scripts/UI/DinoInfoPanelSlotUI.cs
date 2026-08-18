using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DinoInfoPanelSlotUI : MonoBehaviour
{
    [SerializeField]
    private StatType _statType;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _valueText;

    public StatType StatType => _statType;

    public void Set(DinoStatInfo info, int value)
    {
        if (info == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (_icon != null)
        {
            bool hasIcon = info.Icon != null;
            _icon.gameObject.SetActive(hasIcon);
            if (hasIcon)
                _icon.sprite = info.Icon;
        }

        if (_nameText != null)
            _nameText.text = info.DisplayName;

        if (_valueText != null)
            _valueText.text = info.FormatValue(value);
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }
}