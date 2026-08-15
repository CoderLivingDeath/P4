using TMPro;
using UnityEngine;

public class ResourcesOverviewBehaviour : MonoBehaviour
{
    [SerializeField]
    private ResourcesManager _resourcesManager;

    [SerializeField]
    private TextMeshProUGUI _foodText;

    [SerializeField]
    private TextMeshProUGUI _eggsText;

    [SerializeField]
    private string _foodFormat = "Food: {0}";

    [SerializeField]
    private string _eggsFormat = "Eggs: {0}";

    private void OnEnable()
    {
        if (_resourcesManager == null)
            return;

        _resourcesManager.FoodChanged += OnFoodChanged;
        _resourcesManager.EggsChanged += OnEggsChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (_resourcesManager == null)
            return;

        _resourcesManager.FoodChanged -= OnFoodChanged;
        _resourcesManager.EggsChanged -= OnEggsChanged;
    }

    private void Refresh()
    {
        OnFoodChanged(_resourcesManager.Food);
        OnEggsChanged(_resourcesManager.Eggs);
    }

    private void OnFoodChanged(int value)
    {
        if (_foodText != null)
            _foodText.text = string.Format(_foodFormat, value);
    }

    private void OnEggsChanged(int value)
    {
        if (_eggsText != null)
            _eggsText.text = string.Format(_eggsFormat, value);
    }
}
