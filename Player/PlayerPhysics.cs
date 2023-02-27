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

    //=================================���� ����
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
        Rigid2D.velocity = PlayerObj.transform.up * speed; //�߻�
    }

    //=================================�߻�
    internal void PhysicsVelocityControl(float speed)
    {
        VelocityControl(speed);
    }

    //=================================����
    internal void PhysicsAttachWall() //���ϰ� �÷��̾� ����
    {
        VelocityControl(0);
        RigidSimulateOff();
    }

    internal void PhysicsAttchStaticWall()
    {
        VelocityControl(0);
        RigidSimulateOff();
    }
    //=================================�и�
    internal void PhysicsSimulateOn()
    {
        RigidSimulateOn();
    }

    internal void PhysicsSimulateOff()
    {
        RigidSimulateOff();
    }
}
