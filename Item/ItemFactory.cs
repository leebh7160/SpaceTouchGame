using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    public ItemList GetItemValue(string itemname)
    {
        switch (itemname)
        {
            case "Item":
                return new BoostItem(1);
            default:
                return null;
        }

    }
}
