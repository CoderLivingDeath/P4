using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DinoList", menuName = "Domain/DinoList")]
public class DinoList : ScriptableObject
{
    [SerializeField]
    private List<Dino> _dinos = new List<Dino>();

    public IReadOnlyList<Dino> Dinos => _dinos;
}
