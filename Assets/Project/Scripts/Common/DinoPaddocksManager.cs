using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DinoPaddocksManager : MonoBehaviour
{
    public List<PaddockBehaviour> paddockBehaviours;

    public bool TrySetDinoInFreeSlot(Dino dino)
    {
        var freepaddock = GetFreePaddock();
        if (freepaddock = null) return false;
        int slot = freepaddock.GetFreeSlot();
        if (slot == -1) return false;

        freepaddock.Model.SetSlot(slot, dino);

        return true;
    }
    
    public PaddockBehaviour GetFreePaddock()
    {
        foreach (var item in paddockBehaviours)
        {
            foreach (var slot in item.Model.Slots)
            {
                if (slot == null) return item;
            }
        }

        return null;
    }
}