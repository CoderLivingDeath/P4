using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocationDino
{
    [SerializeField]
    private Dino _dino;

    [SerializeField]
    private bool _availableFromStart;

    public Dino Dino => _dino;

    public bool AvailableFromStart => _availableFromStart;
}

[System.Serializable]
public class LocationReward
{
    [SerializeField]
    private HuntZone _zone;

    [SerializeField]
    private Reward _food;

    [SerializeField]
    private int _foodQuantity;

    [SerializeField]
    private bool _foodUseRange;

    [SerializeField]
    private int _foodMinQuantity = 1;

    [SerializeField]
    private int _foodMaxQuantity = 1;

    [SerializeField]
    private int _missionTimeSeconds;

    [SerializeField]
    private List<LocationDino> _dinos = new List<LocationDino>();

    public HuntZone Zone => _zone;

    public Reward Food => _food;

    public int FoodQuantity => _foodQuantity;

    public bool FoodUseRange => _foodUseRange;

    public int FoodMinQuantity => _foodMinQuantity;

    public int FoodMaxQuantity => _foodMaxQuantity;

    public int MissionTimeSeconds => _missionTimeSeconds;

    public IReadOnlyList<LocationDino> Dinos => _dinos;
}