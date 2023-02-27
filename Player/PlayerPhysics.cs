using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics
{
    Rigidbody2D Rigid2D;
    GameObject PlayerObj;

    internal void SetRigid2D(Rigidbody2D rigid)
    {
        Rigid2D = rigid;
    }

    internal Rigidbody2D GetRigid2D()
    {
        return Rigid2D;
    }
    internal void SetPlayerObj(GameObject obj)
    {
        PlayerObj = obj;
    }

    //=================================물리 조정
    private void RigidSimulateOff()
    {
        Rigid2D.simulated = false;
    }
    private void RigidSimulateOn()
    {
        Rigid2D.simulated = true;
    }
    private void VelocityControl(float speed)
    {
        Rigid2D.velocity = PlayerObj.transform.up * speed; //발사
    }

    //=================================발사
    internal void PhysicsVelocityControl(float speed)
    {
        VelocityControl(speed);
    }

    //=================================접촉
    internal void PhysicsAttachWall() //벽하고 플레이어 접촉
    {
        VelocityControl(0);
        RigidSimulateOff();
    }

    internal void PhysicsAttchStaticWall()
    {
        VelocityControl(0);
        RigidSimulateOff();
    }
    //=================================분리
    internal void PhysicsSimulateOn()
    {
        RigidSimulateOn();
    }

    internal void PhysicsSimulateOff()
    {
        RigidSimulateOff();
    }
}
