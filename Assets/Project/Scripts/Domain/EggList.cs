using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EggList", menuName = "Domain/EggList")]
public class EggList : ScriptableObject
{
    [SerializeField]
    private List<Egg> _eggs = new List<Egg>();

    public IReadOnlyList<Egg> Eggs => _eggs;
}
