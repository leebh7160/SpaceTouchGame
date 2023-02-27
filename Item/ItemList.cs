using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ItemCode
{
    boost = 1,
}


public class ItemList
{
    public ItemList() { }

    protected virtual int getBoostValue() { return 0; }

    internal int getItemStatus(int type)
    {
        switch (type)
        {
            case 1:
                return getBoostValue();
            default:
                break;
        }
        return 0;
    }
}
