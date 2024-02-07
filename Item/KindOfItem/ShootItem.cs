using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootItem : ItemList
{
    private int itemStatus = 0;

    public ShootItem(int value)
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

    protected override int getShootValue()
    {
        return itemStatus;
    }

}
