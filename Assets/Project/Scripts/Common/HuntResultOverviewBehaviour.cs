using TMPro;
using UnityEngine;

public class HuntResultOverviewBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _resultText;

    [SerializeField]
    private TextMeshProUGUI _dinoText;

    [SerializeField]
    private string _successText = "Success";

    [SerializeField]
    private string _failText = "Fail";

    public void OnHuntSuccess(Dino dino)
    {
        if (_resultText != null)
            _resultText.text = _successText;

        if (_dinoText != null)
            _dinoText.text = dino != null ? dino.Name : string.Empty;
    }

    public void OnHuntFail()
    {
        if (_resultText != null)
            _resultText.text = _failText;

        if (_dinoText != null)
            _dinoText.text = string.Empty;
    }
}
