using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAction
{
    Idle = 0, TrackMove = 1, Track = 2, 
}


class EnemyStatus
{
    [Header("적군 체력 조정")]
    [SerializeField] internal int enemyHP;
}


public class Enemy : MonoBehaviour
{
    //================참조
    protected Transform bulletObject;
    protected A_Bullet a_Bullet;
    protected NavMeshAgent nmAgent;
    protected GameObject playerPos; //플레이어 포지션 계속 추적
    protected GameObject dead_Container;
    //================변수
    [Header("회전 속도")]
    [SerializeField] protected float rotateDelay = 3;
    protected float rayDistance = 60;
    protected Ray2D ray2d;

    //================정적 머신
    protected bool state_isShoot                = false;
    protected float state_ShootAriaDistance     = 80;
    protected float state_MoveSpeed             = 0.5f; //적군 이동 속도
    protected Vector2 state_TargetRotation;         //타겟의 위치 바라보기
    protected Vector2 state_BulletHitPos        = new Vector2(); //총알을 맞았을 시 받는 고정 위치
    protected EnemyAction state_EnemyAction;


    //================인스펙터

    public void Start()
    {
        Enemy_Init();
    }

    private void Enemy_Init()
    {
        dead_Container          = GameObject.Find("DeadBodyContainer");
        state_TargetRotation    = new Vector2();
        state_BulletHitPos      = new Vector2();
        state_EnemyAction       = EnemyAction.Idle;
        bulletObject            = GameObject.Find("AlienBullet").transform.Find("Bullet").transform;
        Enemy_Init_Bullet();
    }

    private void Enemy_Init_Bullet()
    {
        a_Bullet = bulletObject.GetChild(0).GetComponent<A_Bullet>();
        a_Bullet.gameObject.SetActive(true);
    }

    protected void Enemy_Init_Destination(Vector2 target)//Navigation에 위치 전해줌
    {
        nmAgent.SetDestination(target);
    }

    internal void Enemy_DeadBody_Clear()
    {
        StartCoroutine(BodyContainerClear());
    }

    protected IEnumerator BodyContainerClear()
    {
        for (int i = 0; i < dead_Container.transform.childCount; i++)
            Destroy(dead_Container.transform.GetChild(i).gameObject);
        yield return null;
    }

    #region 정적 머신(State Machine)
    //적군 움직임 참조 : https://unitybeginner.tistory.com/50

    protected IEnumerator StateMachine()
    {
        bool isAriaEnter            = false;//플레이어 들어오면 true
        bool isShootAriaEnter       = false;//플레이어 사격 들어오면 True


        while (true)
        {
            float tempDistance = Vector2.Distance(this.transform.position, playerPos.transform.position);

            //==========범위에 맞는 행동============
            if (state_EnemyAction == EnemyAction.Idle)//플레이어 없으니 대기
            {
                isAriaEnter         = false;
                isShootAriaEnter    = false;
            }
            else if (state_EnemyAction == EnemyAction.TrackMove)                               //총알을 맞았을 시 고정 위치 찾기
            {
                Enemy_Init_Destination(state_BulletHitPos);                                   //Navigation에게 위치를 전해줌

                state_TargetRotation    = Enemy_Act_Rotate(state_BulletHitPos);                        //고정 위치로 회전
                this.transform.up       = state_TargetRotation;
                //this.transform.position = Enemy_Act_Move(state_BulletHitPos);                     //고정 위치로 이동(속도가 너무 빠름)
                this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * 2f);             //고정 위치로 이동
            }

            else if (state_EnemyAction == EnemyAction.Track)                                   //플레이어가 범위 안에 있음을 확인한다
            {

                state_TargetRotation    = Enemy_Act_Rotate(playerPos.transform.position);        //위치로 회전
                isAriaEnter             = Enemy_Act_Tracking(state_TargetRotation);           //벽 뒤에 있는지 판별
                this.transform.up       = state_TargetRotation;

                if (isAriaEnter == true)
                {
                    Enemy_Init_Destination(playerPos.transform.position);                   //Navigation에게 위치를 전해줌
                    //this.transform.position = Enemy_Act_Move(playerPos.transform.position);//고정 위치로 이동(속도가 너무 빠름)
                    this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * 5f);  //고정 위치로 이동

                }

            }

            //==============범위 확인===============
            state_EnemyAction = Enemy_Check_Aria_In(tempDistance);

            isShootAriaEnter = Enemy_Check_ShootAria_In(tempDistance);                  //타겟 사격 범위 확인

            if (isAriaEnter == true && isShootAriaEnter == true)
                state_isShoot = true;
            else
                state_isShoot = false;

            yield return this;
        }
        yield return null;
    }

    protected EnemyAction Enemy_Check_Aria_In(float target)//적군의 행동 범위를 확인
    {
        if (target >= 150)//범위 안에 없음
            return EnemyAction.Idle;
        else
            return EnemyAction.Track;
    }
    private bool Enemy_Check_ShootAria_In(float target)//적군의 사격 범위를 확인
    {
        if(target <= state_ShootAriaDistance)
            return true;
        return false;
    }

    private Vector2 Enemy_Act_Rotate(Vector2 targetpos)//조정된 위치로 회전
    {
        float enemyPosX = targetpos.x - transform.position.x;
        float enemyPosY = targetpos.y - transform.position.y;
        Vector2 t_direction = new Vector2(enemyPosX, enemyPosY);
        Vector2 enemyRotation;

        enemyRotation = Vector2.Lerp(transform.rotation.eulerAngles, t_direction, rotateDelay);

        return enemyRotation;
    }

    private Vector2 Enemy_Act_Move(Vector2 target)//고정된 위치로 이동
    {
        Vector2 temp = Vector2.Lerp(this.transform.position, target, Time.smoothDeltaTime * state_MoveSpeed);

        return temp;
    }

    private bool Enemy_Act_Tracking(Vector2 target)//조정된 위치의 Ray로 벽 확인
    {
        string temp_name;

        ray2d = new Ray2D(this.transform.position, target);
        RaycastHit2D raycast = Physics2D.Raycast(ray2d.origin, ray2d.direction, rayDistance, LayerMask.GetMask("Player", "Wall"));

        Debug.DrawRay(ray2d.origin, target * raycast.distance, Color.red, 0.8f);

        if (raycast.collider != null)
        {
            if (raycast.collider.name == "WallAria" || raycast.collider.name == "PlayerItemChecker") //벽 뒤에 없음
            {
                return true;
            }
            return false;
        }
        return false;
    }

    #endregion
}
