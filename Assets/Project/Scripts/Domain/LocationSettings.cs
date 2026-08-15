using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationSettings", menuName = "Domain/LocationSettings")]
public class LocationSettings : ScriptableObject
{
    [SerializeField]
    private List<LocationEntry> _entries = new List<LocationEntry>();

    public IReadOnlyList<LocationEntry> Entries => _entries;

    public LocationEntry GetEntry(HuntZone zone)
    {
        foreach (LocationEntry entry in _entries)
        {
            if (entry.Zone == zone)
                return entry;
        }

        return null;
    }
}

[System.Serializable]
public class LocationEntry
{
    [SerializeField]
    private HuntZone _zone;

    [SerializeField]
    private bool _unlockedByDefault;

    [SerializeField]
    private int _peopleThreshold;

    public HuntZone Zone => _zone;

    public bool UnlockedByDefault => _unlockedByDefault;

    public int PeopleThreshold => _peopleThreshold;
}
