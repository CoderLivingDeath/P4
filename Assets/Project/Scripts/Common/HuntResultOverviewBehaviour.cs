using TMPro;
using UnityEngine;

public class HuntResultOverviewBehaviour : MonoBehaviour
{
    [SerializeField]
    private HuntController _huntController;

    [SerializeField]
    private TextMeshProUGUI _resultText;

    [SerializeField]
    private TextMeshProUGUI _dinoText;

    [SerializeField]
    private string[] _successVariants = new[] { "T-Rex", "Triceratops", "Pterodactyl" };

    [SerializeField]
    private string _successText = "Success";

    [SerializeField]
    private string _failText = "Fail";

    public void OnHuntSuccess()
    {
        if (_resultText != null)
            _resultText.text = _successText;

        if (_dinoText != null)
            _dinoText.text = PickRandomVariant();
    }

    public void OnHuntFail()
    {
        if (_resultText != null)
            _resultText.text = _failText;

        if (_dinoText != null)
            _dinoText.text = string.Empty;
    }

    private string PickRandomVariant()
    {
        if (_successVariants == null || _successVariants.Length == 0)
            return string.Empty;

        return _successVariants[Random.Range(0, _successVariants.Length)];
    }
}
