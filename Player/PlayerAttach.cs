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

    private void Rigid_VelocityControl(float speed)//Velocity조정
    {
        playerRigid2D.velocity = playerObj.transform.up * speed;
    }
    #endregion private rigid

    #region internal 물리 조정
    internal void Physics_AttachWall()//벽 접촉 시 물리 조정
    {
        Rigid_VelocityControl(0);
        Rigid_Simulate(false);
    }

    internal void Physics_Simulate(bool check)//rigid 세팅 접근
    {
        Rigid_Simulate(check);
    }

    internal void Physics_Velocity(float speed)
    {
        Rigid_VelocityControl(speed);
    }
    #endregion internal 물리 조정

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

    #region 벽 하고 접촉
    internal void Attach_Wall(Transform touchedWall)//벽하고 접촉 시
    {
        playerPhysics.Physics_AttachWall();
        touchWall = touchedWall;
        //touchWall.GetComponent<Joint2D>().connectedBody = playerPhysics.PlayerRigid2D;
        AttachPlayerRotate(touchedWall);
    }

    internal void Attach_StaticWall(Transform touchedWall)//정적 벽하고 접촉 시
    {
        touchWall = touchedWall;
        playerPhysics.Physics_AttachWall();
        AttachPlayerRotate(touchWall);
    }

    internal void Attach_BrokenWall(Transform touchedWall)//부서지는 벽하고 접촉 시
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

        if (thisPos.x > block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)//x가 +블록 길이보다 작고 -블록 길이보다 크게 y가 1보다 작고 -1보다 크고 
            tempPos = new Vector3(0, 0, 270);
        if (thisPos.x < -block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)
            tempPos = new Vector3(0, 0, 90);

        if (thisPos.y > block.localScale.y)
            tempPos = new Vector3(0, 0, 0);
        if (thisPos.y < -block.localScale.y)
            tempPos = new Vector3(0, 0, 180);

        playerPhysics.PlayerObj.transform.localRotation = Quaternion.Euler(tempPos);
    }
    #endregion 벽 하고 접촉

    #region 벽 하고 분리
    internal void Detach_Wall()//돌아가는 벽하고 분리하면 부모와 연결 끊기
    {
        if (touchWall == null) return;

        playerPhysics.PlayerObj.transform.parent = null;
        touchWall.GetComponent<Joint2D>().connectedBody = null;
    }

    internal void Detach_StaticWall()//정적 벽하고 분리 시
    {
        if (touchWall == null) return;

        playerPhysics.PlayerObj.transform.parent = null;
    }
    #endregion 벽 하고 분리

    #region 물리
    internal void Physics_Velocity(float speed)
    {
        playerPhysics.Physics_Velocity(speed);
    }

    internal void Physics_Simulate(bool check)
    {
        playerPhysics.Physics_Simulate(check);
    }
    #endregion 물리

    #region 세팅
    internal void Setting_Physics()//플레이어 세팅(리프레쉬)
    {
        if(touchWall != null && touchWall.GetComponent<Joint2D>() != null)
            touchWall.GetComponent<Joint2D>().connectedBody = null;

        touchWall = null;

        playerPhysics.Physics_Simulate(false);
        playerPhysics.Physics_Velocity(0);
    }
    #endregion 세팅
}
