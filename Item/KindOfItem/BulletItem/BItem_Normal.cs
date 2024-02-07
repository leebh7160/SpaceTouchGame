using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BItem_Normal : ItemList
{
    private BulletData itemStatus = new BulletData();

    public BItem_Normal(int value)
    {
        SetItemStatus(value);
    }

    private void SetItemStatus(int value)
    {
        switch (value)
        {
            case 1:
                itemStatus.bullet_Speed = 60;
                itemStatus.bullet_kind = BULLET_KIND.Normal;
                break;
        }
    }

    protected override BulletData getNormalBulletValue()
    {
        return itemStatus;
    }
}