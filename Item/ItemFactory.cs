using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFactory
{
    //������ �ڵ�� ������ ������
    //�ڵ忡 �°� �������� �ְ�
    //���� ���� ��ȯ�� �����ϴ�
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
