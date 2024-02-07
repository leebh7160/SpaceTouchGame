using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class EnemyNormal : Enemy
{
    //=======================변수
    EnemyStatus status;
    private Vector2 defalutPos;
    private float shootTime = 0;
    private float shootDelay = 1.3f;

    //=======================참조
    private GameObject enemy_Body;
    private GameObject enemy_Alive_Effect;
    private GameObject dead_Body;
    private GameObject dead_Effect;

    private Transform EnemyShootLoaction;

    private void Awake()
    {
        defalutPos          = this.transform.position;
        enemy_Body          = this.transform.Find("EnemyBody").gameObject;
        enemy_Alive_Effect  = this.transform.Find("EnemyAliveEffect").gameObject;
        dead_Body           = this.transform.Find("EnemyDeadBody").gameObject;
        dead_Effect         = this.transform.Find("EnemyEffect").gameObject;
        EnemyShootLoaction  = this.transform.Find("EnemyShootLocation");

    }

    private void OnEnable()
    {
        this.transform.position = defalutPos;
        state_BulletHitPos  = this.transform.position;
        state_TargetRotation = Vector2.zero;
        dead_Body.SetActive(false);
        dead_Effect.SetActive(false);
        state_EnemyAction = EnemyAction.Idle;
        Enemy_Setting_Status();
        Enemy_Know_PlayerPosition();
    }

    private void Enemy_Setting_Status()
    {
        status = new EnemyStatus();
        //status.enemyHP = 5;
        status.enemyHP = 1;
    }

    private void FixedUpdate()
    {
        if (shootTime < shootDelay)
        {
            shootTime += Time.deltaTime;
        }

        if (state_isShoot == true)
            if (shootTime >= shootDelay)
            {
                shootTime = 0;
                EnemyShoot();
            }
    }

    #region 적군 생성에 관한

    internal void Enemy_Create()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(Enemy_Create_Effect());
    }

    private IEnumerator Enemy_Create_Effect()
    {
        float time = 0;
        float speed = 0;
        bool check = false;

        enemy_Alive_Effect.gameObject.SetActive(true);
        enemy_Body.gameObject.SetActive(false);

        while (speed < 10)
        {
            time = Mathf.Sin(speed) * 10;
            speed += 0.3f;
            enemy_Alive_Effect.transform.localScale = new Vector2(time, time);

            //=====================================================
            if (speed >= 5 && check == false)
            {
                check = true;
                enemy_Alive_Effect.gameObject.SetActive(false);
                enemy_Body.gameObject.SetActive(true);
                StartCoroutine(StateMachine());
            }
            //=====================================================
            yield return new WaitForSeconds(0.1f);
        }
        check = false;
        yield return null;
    }
    #endregion


    #region 적군 사격에 관한
    private void EnemyShoot()
    {
        if (state_TargetRotation == Vector2.zero) return;

        A_Bullet inst_A_Bullet = Instantiate(a_Bullet, bulletObject);

        inst_A_Bullet.gameObject.transform.position = EnemyShootLoaction.position;
        inst_A_Bullet.gameObject.transform.up = state_TargetRotation;
        inst_A_Bullet.A_Bullet_Velocity(state_TargetRotation);
        inst_A_Bullet.gameObject.SetActive(true);
        inst_A_Bullet.A_Bullet_Active();

        shootTime = 0;
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
            Enemy_Damage();
    }

    private void Enemy_Damage()
    {
        if (status.enemyHP > 0)
        {
            state_BulletHitPos = playerPos.transform.position;
            state_EnemyAction = EnemyAction.TrackMove;
            status.enemyHP -= 1;

            if (status.enemyHP <= 0)
                Enemy_Damaged_Effect();
        }
    }

    private void Enemy_Know_PlayerPosition() //플레이어 위치 찾아오기
    {
        playerPos = GameManager.Instance.Get_Enemy_PlayerPos();
        nmAgent = this.gameObject.GetComponent<NavMeshAgent>();
        nmAgent.ResetPath();
    }

    private void Enemy_Damaged_Effect()
    {
        Enemy_Dead_Effect();
        this.gameObject.SetActive(false);
        this.transform.position = defalutPos;
        EnemyManager.Instance.Enemy_Play_Game();
    }

    private void Enemy_Dead_Effect()
    {
        var clone_effect    = Instantiate(dead_Effect, this.transform.position, Quaternion.identity);
        var clone_body      = Instantiate(dead_Body, this.transform.position, this.transform.rotation, dead_Container.transform);

        clone_effect.SetActive(true);
        clone_body.SetActive(true);
    }
    #endregion
}
