using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "Domain/Map")]
public class Map : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _sprite;

    public string Name => _name;

    public Sprite Sprite => _sprite;
}