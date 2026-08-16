using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Domain/Reward")]
public class Reward : ScriptableObject
{
    [SerializeField]
    private string _name;

    [SerializeField]
    private Sprite _icon;

    [SerializeField]
    private RewardType _type;

    public string Name => _name;

    public Sprite Icon => _icon;

    public RewardType Type => _type;
}