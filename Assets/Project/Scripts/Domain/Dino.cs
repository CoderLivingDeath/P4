using UnityEngine;

[CreateAssetMenu(fileName = "Dino", menuName = "Domain/Dino")]
public class Dino : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private DinoType _type;

    [SerializeField]
    private int _weightMin = 100;

    [SerializeField]
    private int _weightMax = 1000;

    public string Name => _name;

    public Sprite Sprite => _sprite;

    public DinoType Type => _type;

    public int WeightMin => _weightMin;

    public int WeightMax => _weightMax;
}
