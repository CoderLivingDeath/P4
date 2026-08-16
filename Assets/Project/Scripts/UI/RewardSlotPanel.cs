using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardSlotPanel : MonoBehaviour
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private Image _iconBg;

    [SerializeField]
    private TextMeshProUGUI _amountText;

    [SerializeField]
    private Color _lockedColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    public void Set(Sprite icon, string amount, bool locked)
    {
        if (_icon != null)
        {
            _icon.gameObject.SetActive(icon != null);
            _icon.sprite = icon;
        }

        if (_iconBg != null)
            _iconBg.color = locked ? _lockedColor : Color.white;

        if (_amountText != null)
            _amountText.text = amount;
    }

    public void Clear()
    {
        if (_icon != null)
        {
            _icon.gameObject.SetActive(false);
            _icon.sprite = null;
            _icon.color = Color.white;
        }

        if (_iconBg != null)
            _iconBg.color = Color.white;

        if (_amountText != null)
            _amountText.text = string.Empty;
    }
}