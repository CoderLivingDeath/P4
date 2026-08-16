using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PaddockModel", menuName = "Domain/PaddockModel")]
public class PaddockModel : ScriptableObject
{
    private const int SlotCount = 4;

    [SerializeField]
    private Dino[] _slots = new Dino[SlotCount];

    public event Action<int> SlotChanged;

    public Dino[] Slots => _slots;

    public int Count => _slots.Length;

    public Dino GetSlot(int index) => _slots[index];

    public bool IsEmpty(int index) => _slots[index] == null;

    public void SetSlot(int index, Dino dino)
    {
        _slots[index] = dino;
        SlotChanged?.Invoke(index);
    }

    public void ClearSlot(int index)
    {
        _slots[index] = null;
        SlotChanged?.Invoke(index);
    }
}