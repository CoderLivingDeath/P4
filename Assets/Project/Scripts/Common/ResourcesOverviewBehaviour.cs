using TMPro;
using UnityEngine;

public class ResourcesOverviewBehaviour : MonoBehaviour
{
    [SerializeField]
    private ResourcesManager _resourcesManager;

    [SerializeField]
    private TextMeshProUGUI _foodText;

    [SerializeField]
    private TextMeshProUGUI _peopleText;

    [SerializeField]
    private string _foodFormat = "Food: {0}";

    [SerializeField]
    private string _peopleFormat = "People: {0}";

    private void OnEnable()
    {
        if (_resourcesManager == null)
            return;

        _resourcesManager.FoodChanged += OnFoodChanged;
        _resourcesManager.PeopleChanged += OnPeopleChanged;

        Refresh();
    }

    private void OnDisable()
    {
        if (_resourcesManager == null)
            return;

        _resourcesManager.FoodChanged -= OnFoodChanged;
        _resourcesManager.PeopleChanged -= OnPeopleChanged;
    }

    private void Refresh()
    {
        OnFoodChanged(_resourcesManager.Food);
        OnPeopleChanged(_resourcesManager.People);
    }

    private void OnFoodChanged(int value)
    {
        if (_foodText != null)
            _foodText.text = string.Format(_foodFormat, value);
    }

    private void OnPeopleChanged(int value)
    {
        if (_peopleText != null)
            _peopleText.text = string.Format(_peopleFormat, value);
    }
}
