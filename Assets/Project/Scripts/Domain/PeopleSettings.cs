using UnityEngine;

[CreateAssetMenu(fileName = "PeopleSettings", menuName = "Domain/PeopleSettings")]
public class PeopleSettings : ScriptableObject
{
    [SerializeField]
    private int _startPeople = 2;

    [SerializeField]
    private float _growthIntervalSeconds = 30f;

    [SerializeField]
    private int _growthAmount = 1;

    public int StartPeople => _startPeople;

    public float GrowthIntervalSeconds => _growthIntervalSeconds;

    public int GrowthAmount => _growthAmount;
}
