using UnityEngine;

[CreateAssetMenu(fileName = "DinoStatInfo", menuName = "Domain/DinoStatInfo")]
public class DinoStatInfo : ScriptableObject
{
    [SerializeField]
    private StatType _type;

    [SerializeField]
    private string _displayName;

    [SerializeField]
    private Sprite _icon;

    [SerializeField]
    private string _valueFormat = "{0}";

    [SerializeField]
    private string[] _valueNames;

    public StatType Type => _type;

    public string DisplayName => _displayName;

    public Sprite Icon => _icon;

    public string FormatValue(int value)
    {
        if (_valueNames != null && _valueNames.Length > 0 && value >= 0 && value < _valueNames.Length)
            return _valueNames[value];

        return string.Format(_valueFormat, value);
    }
}