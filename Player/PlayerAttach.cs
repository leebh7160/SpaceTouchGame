using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

class PlayerPhyscis
{
    private Rigidbody2D playerRigid2D;
    public Rigidbody2D PlayerRigid2D
    {
        get { return playerRigid2D; }
        set { playerRigid2D = value; }
    }
    
    private GameObject playerObj;
    public GameObject PlayerObj
    {
        get { return playerObj; }
        set { playerObj = value; }
    }

    #region private rigid
    private void Rigid_Simulate(bool check) //Rigid On Off
    {
        playerRigid2D.simulated = check;
    }

    private void Rigid_VelocityControl(float speed)//Velocity����
    {
        playerRigid2D.velocity = playerObj.transform.up * speed;
    }
    #endregion private rigid

    #region internal ���� ����
    internal void Physics_AttachWall()//�� ���� �� ���� ����
    {
        Rigid_VelocityControl(0);
        Rigid_Simulate(false);
    }

    internal void Physics_Simulate(bool check)//rigid ���� ����
    {
        Rigid_Simulate(check);
    }

    internal void Physics_Velocity(float speed)
    {
        Rigid_VelocityControl(speed);
    }
    #endregion internal ���� ����

}


public class PlayerAttach
{
    private PlayerPhyscis playerPhysics;

    private Transform touchWall;
    public Transform TouchWall
    {
        get { return touchWall; }
    }

    public PlayerAttach(GameObject _player)
    {
        playerPhysics = new PlayerPhyscis();
        playerPhysics.PlayerObj = _player;
        playerPhysics.PlayerRigid2D = _player.GetComponent<Rigidbody2D>();
    }

    #region �� �ϰ� ����
    internal void Attach_Wall(Transform touchedWall)//���ϰ� ���� ��
    {
        playerPhysics.Physics_AttachWall();
        touchWall = touchedWall;
        //touchWall.GetComponent<Joint2D>().connectedBody = playerPhysics.PlayerRigid2D;
        AttachPlayerRotate(touchedWall);
    }

    internal void Attach_StaticWall(Transform touchedWall)//���� ���ϰ� ���� ��
    {
        touchWall = touchedWall;
        playerPhysics.Physics_AttachWall();
        AttachPlayerRotate(touchWall);
    }

    internal void Attach_BrokenWall(Transform touchedWall)//�μ����� ���ϰ� ���� ��
    {
        touchWall = touchedWall;
        playerPhysics.Physics_AttachWall();
        AttachPlayerRotate(touchWall);
    }

    internal void Attach_BombItem()
    {
        playerPhysics.Physics_AttachWall();
    }

    private void AttachPlayerRotate(Transform block)
    {
        Vector2 thisPos = playerPhysics.PlayerObj.transform.localPosition;
        Vector3 tempPos = new Vector3();

        if (thisPos.x > block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)//x�� +��� ���̺��� �۰� -��� ���̺��� ũ�� y�� 1���� �۰� -1���� ũ�� 
            tempPos = new Vector3(0, 0, 270);
        if (thisPos.x < -block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)
            tempPos = new Vector3(0, 0, 90);

        if (thisPos.y > block.localScale.y)
            tempPos = new Vector3(0, 0, 0);
        if (thisPos.y < -block.localScale.y)
            tempPos = new Vector3(0, 0, 180);

        playerPhysics.PlayerObj.transform.localRotation = Quaternion.Euler(tempPos);
    }
    #endregion �� �ϰ� ����

    #region �� �ϰ� �и�
    internal void Detach_Wall()//���ư��� ���ϰ� �и��ϸ� �θ�� ���� ����
    {
        if (touchWall == null) return;

        playerPhysics.PlayerObj.transform.parent = null;
        touchWall.GetComponent<Joint2D>().connectedBody = null;
    }

    internal void Detach_StaticWall()//���� ���ϰ� �и� ��
    {
        if (touchWall == null) return;

        playerPhysics.PlayerObj.transform.parent = null;
    }
    #endregion �� �ϰ� �и�

    #region ����
    internal void Physics_Velocity(float speed)
    {
        playerPhysics.Physics_Velocity(speed);
    }

    internal void Physics_Simulate(bool check)
    {
        playerPhysics.Physics_Simulate(check);
    }
    #endregion ����

    #region ����
    internal void Setting_Physics()//�÷��̾� ����(��������)
    {
        if(touchWall != null && touchWall.GetComponent<Joint2D>() != null)
            touchWall.GetComponent<Joint2D>().connectedBody = null;

        touchWall = null;

        playerPhysics.Physics_Simulate(false);
        playerPhysics.Physics_Velocity(0);
    }
    #endregion ����
}
