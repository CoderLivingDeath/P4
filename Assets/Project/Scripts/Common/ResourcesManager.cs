using System;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private int _food;

    [SerializeField]
    private int _people;

    public int Food => _food;

    public int People => _people;

    public event Action<int> FoodChanged;

    public event Action<int> PeopleChanged;

    private void OnValidate()
    {
        SetFood(_food);
        SetPeople(_people);
    }

    public void AddFood(int amount)
    {
        if (amount <= 0)
            return;

        SetFood(_food + amount);
    }

    public bool TryConsumeFood(int amount)
    {
        if (amount <= 0 || _food < amount)
            return false;

        SetFood(_food - amount);
        return true;
    }

    public void AddPeople(int amount)
    {
        if (amount <= 0)
            return;

        SetPeople(_people + amount);
    }

    public void SetFood(int value)
    {
        int clamped = Mathf.Max(0, value);
        if (clamped == _food)
            return;

        _food = clamped;
        if (Application.isPlaying)
            FoodChanged?.Invoke(_food);
    }

    public void SetPeople(int value)
    {
        int clamped = Mathf.Max(0, value);
        if (clamped == _people)
            return;

        _people = clamped;
        if (Application.isPlaying)
            PeopleChanged?.Invoke(_people);
    }
}
