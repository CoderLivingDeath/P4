using System.Collections.Generic;
using UnityEngine;

public class DinoPaddocksManager : MonoBehaviour
{
    public List<PaddockBehaviour> paddockBehaviours;

    public bool TrySetDinoInFreeSlot(Dino dino)
    {
        if (dino == null)
            return false;

        PaddockBehaviour freePaddock = GetFreePaddock();
        if (freePaddock == null)
            return false;

        int slot = freePaddock.GetFreeSlot();
        if (slot < 0)
            return false;

        // SetSlot raises SlotChanged, which refreshes both the paddock and its UI.
        freePaddock.Model.SetSlot(slot, dino);

        return true;
    }
    
    public PaddockBehaviour GetFreePaddock()
    {
        if (paddockBehaviours == null)
            return null;

        foreach (var item in paddockBehaviours)
        {
            if (item != null && item.GetFreeSlot() >= 0)
                return item;
        }

        return null;
    }
}
