using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapList", menuName = "Domain/MapList")]
public class MapList : ScriptableObject
{
    [SerializeField]
    private List<Map> _maps = new List<Map>();

    public IReadOnlyList<Map> Maps => _maps;
}