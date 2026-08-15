using UnityEngine;

[CreateAssetMenu(fileName = "Dino", menuName = "Domain/Dino")]
public class Dino : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    public string Name => _name;

    public Sprite Sprite => _sprite;
}
