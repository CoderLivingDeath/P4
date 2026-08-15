using UnityEngine;

[CreateAssetMenu(fileName = "HungerSettings", menuName = "Domain/HungerSettings")]
public class HungerSettings : ScriptableObject
{
    [SerializeField]
    private float _hungerDurationSeconds = 25f;

    [SerializeField]
    private float _postHungerDurationSeconds = 20f;

    public float HungerDurationSeconds => _hungerDurationSeconds;

    public float PostHungerDurationSeconds => _postHungerDurationSeconds;
}
