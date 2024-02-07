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
    [Header("���� ü�� ����")]
    [SerializeField] internal int enemyHP;
}


public class Enemy : MonoBehaviour
{
    //================����
    protected Transform bulletObject;
    protected A_Bullet a_Bullet;
    protected NavMeshAgent nmAgent;
    protected GameObject playerPos; //�÷��̾� ������ ��� ����
    protected GameObject dead_Container;
    //================����
    [Header("ȸ�� �ӵ�")]
    [SerializeField] protected float rotateDelay = 3;
    protected float rayDistance = 60;
    protected Ray2D ray2d;

    //================���� �ӽ�
    protected bool state_isShoot                = false;
    protected float state_ShootAriaDistance     = 80;
    protected float state_MoveSpeed             = 0.5f; //���� �̵� �ӵ�
    protected Vector2 state_TargetRotation;         //Ÿ���� ��ġ �ٶ󺸱�
    protected Vector2 state_BulletHitPos        = new Vector2(); //�Ѿ��� �¾��� �� �޴� ���� ��ġ
    protected EnemyAction state_EnemyAction;


    //================�ν�����

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

    protected void Enemy_Init_Destination(Vector2 target)//Navigation�� ��ġ ������
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

    #region ���� �ӽ�(State Machine)
    //���� ������ ���� : https://unitybeginner.tistory.com/50

    protected IEnumerator StateMachine()
    {
        bool isAriaEnter            = false;//�÷��̾� ������ true
        bool isShootAriaEnter       = false;//�÷��̾� ��� ������ True


        while (true)
        {
            float tempDistance = Vector2.Distance(this.transform.position, playerPos.transform.position);

            //==========������ �´� �ൿ============
            if (state_EnemyAction == EnemyAction.Idle)//�÷��̾� ������ ���
            {
                isAriaEnter         = false;
                isShootAriaEnter    = false;
            }
            else if (state_EnemyAction == EnemyAction.TrackMove)                               //�Ѿ��� �¾��� �� ���� ��ġ ã��
            {
                Enemy_Init_Destination(state_BulletHitPos);                                   //Navigation���� ��ġ�� ������

                state_TargetRotation    = Enemy_Act_Rotate(state_BulletHitPos);                        //���� ��ġ�� ȸ��
                this.transform.up       = state_TargetRotation;
                //this.transform.position = Enemy_Act_Move(state_BulletHitPos);                     //���� ��ġ�� �̵�(�ӵ��� �ʹ� ����)
                this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * 2f);             //���� ��ġ�� �̵�
            }

            else if (state_EnemyAction == EnemyAction.Track)                                   //�÷��̾ ���� �ȿ� ������ Ȯ���Ѵ�
            {

                state_TargetRotation    = Enemy_Act_Rotate(playerPos.transform.position);        //��ġ�� ȸ��
                isAriaEnter             = Enemy_Act_Tracking(state_TargetRotation);           //�� �ڿ� �ִ��� �Ǻ�
                this.transform.up       = state_TargetRotation;

                if (isAriaEnter == true)
                {
                    Enemy_Init_Destination(playerPos.transform.position);                   //Navigation���� ��ġ�� ������
                    //this.transform.position = Enemy_Act_Move(playerPos.transform.position);//���� ��ġ�� �̵�(�ӵ��� �ʹ� ����)
                    this.transform.Translate(Vector3.forward * Time.smoothDeltaTime * 5f);  //���� ��ġ�� �̵�

                }

            }

            //==============���� Ȯ��===============
            state_EnemyAction = Enemy_Check_Aria_In(tempDistance);

            isShootAriaEnter = Enemy_Check_ShootAria_In(tempDistance);                  //Ÿ�� ��� ���� Ȯ��

            if (isAriaEnter == true && isShootAriaEnter == true)
                state_isShoot = true;
            else
                state_isShoot = false;

            yield return this;
        }
        yield return null;
    }

    protected EnemyAction Enemy_Check_Aria_In(float target)//������ �ൿ ������ Ȯ��
    {
        if (target >= 150)//���� �ȿ� ����
            return EnemyAction.Idle;
        else
            return EnemyAction.Track;
    }
    private bool Enemy_Check_ShootAria_In(float target)//������ ��� ������ Ȯ��
    {
        if(target <= state_ShootAriaDistance)
            return true;
        return false;
    }

    private Vector2 Enemy_Act_Rotate(Vector2 targetpos)//������ ��ġ�� ȸ��
    {
        float enemyPosX = targetpos.x - transform.position.x;
        float enemyPosY = targetpos.y - transform.position.y;
        Vector2 t_direction = new Vector2(enemyPosX, enemyPosY);
        Vector2 enemyRotation;

        enemyRotation = Vector2.Lerp(transform.rotation.eulerAngles, t_direction, rotateDelay);

        return enemyRotation;
    }

    private Vector2 Enemy_Act_Move(Vector2 target)//������ ��ġ�� �̵�
    {
        Vector2 temp = Vector2.Lerp(this.transform.position, target, Time.smoothDeltaTime * state_MoveSpeed);

        return temp;
    }

    private bool Enemy_Act_Tracking(Vector2 target)//������ ��ġ�� Ray�� �� Ȯ��
    {
        string temp_name;

        ray2d = new Ray2D(this.transform.position, target);
        RaycastHit2D raycast = Physics2D.Raycast(ray2d.origin, ray2d.direction, rayDistance, LayerMask.GetMask("Player", "Wall"));

        Debug.DrawRay(ray2d.origin, target * raycast.distance, Color.red, 0.8f);

        if (raycast.collider != null)
        {
            if (raycast.collider.name == "WallAria" || raycast.collider.name == "PlayerItemChecker") //�� �ڿ� ����
            {
                return true;
            }
            return false;
        }
        return false;
    }

    #endregion
}
