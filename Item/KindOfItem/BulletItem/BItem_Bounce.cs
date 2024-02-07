using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BItem_Bounce : ItemList
{
    private BulletData itemStatus = new BulletData();

    public BItem_Bounce(int value)
    {
        SetItemStatus(value);
    }

    private void SetItemStatus(int value)
    {
        switch (value)
        {
            case 1:
                itemStatus.bullet_Speed = 30;
                itemStatus.bullet_kind = BULLET_KIND.Bounce;
                break;
        }
    }

    protected override BulletData getBounceBulletValue()
    {
        return itemStatus;
    }
}
