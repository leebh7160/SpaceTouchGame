using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerLaser
{
    //�� �� �ȵǴ°ɱ�
    //�ϴ� ������Ų��. ���Ӻ��� �� �÷��� �� �� �ְ� �� ��
    //�ƿ� ���̸� �� �ɸ��� �����ɱ�.
    //�� �ڵ忡 ���� �߸��� �ൿ�� �ִٴ� ���ε�
    //������ �װ��� �� ����ġ�ڴ�.
    private float layDistance = 100;
    private Transform laserAria;
    RaycastHit2D hit;
    //private Ray2D ray2D;

    internal void Shoot_Setting(Transform _laserAria)
    {
        laserAria = _laserAria;
    }

    internal void Shoot_Laser_Active(bool active)
    {
        laserAria.gameObject.SetActive(active);
    }

    internal Vector3 Shoot_Laser()
    {
        hit = Physics2D.Raycast(laserAria.position, laserAria.up, layDistance, LayerMask.GetMask("Wall"));

        if (hit.distance <= 0)
            return new Vector3(1, layDistance);
        else
            return new Vector3(1, hit.distance);

    }
}
