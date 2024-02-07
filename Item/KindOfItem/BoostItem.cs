using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoostItem : ItemList
{
    private int itemStatus = 0;

    public BoostItem(int value)
    {
        SetItemStatus(1);
    }

    private void SetItemStatus(int value)
    {
        switch (value)
        {
            case 1:
                itemStatus = 1;
                break;
        }
    }

    protected override int getBoostValue()
    {
        return itemStatus;
    }


}
