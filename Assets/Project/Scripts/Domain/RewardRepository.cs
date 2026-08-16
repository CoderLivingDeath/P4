using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardRepository", menuName = "Domain/RewardRepository")]
public class RewardRepository : ScriptableObject
{
    [SerializeField]
    private List<LocationReward> _entries = new List<LocationReward>();

    public IReadOnlyList<LocationReward> Entries => _entries;

    public LocationReward GetEntry(HuntZone zone)
    {
        foreach (LocationReward entry in _entries)
        {
            if (entry.Zone == zone)
                return entry;
        }

        return null;
    }
}