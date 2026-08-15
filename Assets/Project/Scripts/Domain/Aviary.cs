using UnityEngine;

[CreateAssetMenu(fileName = "Aviary", menuName = "Domain/Aviary")]
public class Aviary : ScriptableObject
{
    private const int SlotCount = 4;

    [SerializeField]
    private Dino[] _slots = new Dino[SlotCount];

    public int Count => _slots.Length;

    public Dino GetSlot(int index) => _slots[index];

    public bool IsEmpty(int index) => _slots[index] == null;
}
