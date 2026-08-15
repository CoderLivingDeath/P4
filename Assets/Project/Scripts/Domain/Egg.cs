using UnityEngine;

[CreateAssetMenu(fileName = "Egg", menuName = "Domain/Egg")]
public class Egg : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    public string Name => _name;

    public Sprite Sprite => _sprite;
}
