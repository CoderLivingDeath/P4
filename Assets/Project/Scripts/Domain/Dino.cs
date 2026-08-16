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

    public string Name => _name;

    public Sprite Sprite => _sprite;

    public DinoType Type => _type;
}
