using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    //아이템 코드와 종류를 받으면
    //코드에 맞게 아이템을 넣고
    //랜덤 값도 반환이 가능하다
    public ItemList GetItemValue(string itemname)
    {
        switch (itemname)
        {
            case "boost":
                return new BoostItem(1);
            case "shoot":
                return new ShootItem(1);
            case "normalBullet":
                return new BItem_Normal(1);
            case "bounceBullet":
                return new BItem_Bounce(1);
            default:
                return null;
        }
    }
}
