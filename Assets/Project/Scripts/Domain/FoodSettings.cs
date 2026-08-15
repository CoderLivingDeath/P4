using UnityEngine;

[CreateAssetMenu(fileName = "FoodSettings", menuName = "Domain/FoodSettings")]
public class FoodSettings : ScriptableObject
{
    [SerializeField]
    private int _startFood = 50;

    [SerializeField]
    private float _drainIntervalSeconds = 5f;

    [SerializeField]
    private int _drainAmount = 1;

    [SerializeField]
    private float _foodPerPersonPerTick = 0.5f;

    public int StartFood => _startFood;

    public float DrainIntervalSeconds => _drainIntervalSeconds;

    public int DrainAmount => _drainAmount;

    public float FoodPerPersonPerTick => _foodPerPersonPerTick;
}
