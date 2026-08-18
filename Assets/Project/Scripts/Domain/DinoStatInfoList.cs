using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DinoStatInfoList", menuName = "Domain/DinoStatInfoList")]
public class DinoStatInfoList : ScriptableObject
{
    [SerializeField]
    private List<DinoStatInfo> _infos = new List<DinoStatInfo>();

    public DinoStatInfo Get(StatType type)
    {
        foreach (DinoStatInfo info in _infos)
        {
            if (info != null && info.Type == type)
                return info;
        }

        return null;
    }
}