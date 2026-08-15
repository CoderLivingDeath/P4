using System;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    [SerializeField]
    private int _food;

    [SerializeField]
    private int _eggs;

    public int Food => _food;

    public int Eggs => _eggs;

    public event Action<int> FoodChanged;

    public event Action<int> EggsChanged;

    private void OnValidate()
    {
        SetFood(_food);
        SetEggs(_eggs);
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

    public void AddEggs(int amount)
    {
        if (amount <= 0)
            return;

        SetEggs(_eggs + amount);
    }

    public bool TryConsumeEggs(int amount)
    {
        if (amount <= 0 || _eggs < amount)
            return false;

        SetEggs(_eggs - amount);
        return true;
    }

    private void SetFood(int value)
    {
        int clamped = Mathf.Max(0, value);
        if (clamped == _food)
            return;

        _food = clamped;
        if (Application.isPlaying)
            FoodChanged?.Invoke(_food);
    }

    private void SetEggs(int value)
    {
        int clamped = Mathf.Max(0, value);
        if (clamped == _eggs)
            return;

        _eggs = clamped;
        if (Application.isPlaying)
            EggsChanged?.Invoke(_eggs);
    }
}
