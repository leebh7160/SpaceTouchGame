using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData
{
    public float bullet_Speed;
    public BULLET_KIND bullet_kind;
}


public class ItemList
{
    public ItemList() { }
    public BulletData bulletdata = new BulletData();

    protected virtual int getBoostValue() { return 0; }
    protected virtual int getShootValue() { return 0; }
    protected virtual BulletData getNormalBulletValue() { return bulletdata; }
    protected virtual BulletData getBounceBulletValue() { return bulletdata; }

    internal int getItemStatus(string name)
    {
        switch(name)
        {
            case "boost":
                return getBoostValue();
            case "shoot":
                return getShootValue();
            default:
                return 0;
        }
    }

    internal BulletData getBulletItemStatus(string name)
    {
        switch (name)
        {
            case "normalBullet":
                return getNormalBulletValue();
            case "bounceBullet":
                return getBounceBulletValue();
            default:
                return null;
        }
    }
}
