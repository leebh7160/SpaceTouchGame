using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerLaser
{
    //왜 잘 안되는걸까
    //일단 보류시킨다. 게임부터 좀 플레이 될 수 있게 할 것
    //아우 왜이리 잔 걸림이 많은걸까.
    //내 코드에 무언가 잘못된 행동이 있다는 것인데
    //아직도 그것을 잘 못고치겠다.
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
