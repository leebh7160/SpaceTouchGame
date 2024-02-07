using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;


public class Player : MonoBehaviour
{
    //====================참조
    private PlayerAttach playerAttach;
    private PlayerSound playerSound;
    private ItemFactory itemFactory;

    private Explodable _explodable;

    //====================플레이어 조작
    private Transform moveLaser;
    private Transform bulletObject;
    private AudioSource audioSource;
    private int shootTime;
    private List<Bullet> Bullet;
    private int bulltcount = 0;
    [SerializeField] private float bulletDelay = 0.3f;
    private float bulletwaitTime = 1;

    //====================레이저
    private SpriteRenderer detactAria;
    private Stuff_Aria stuff_aria;
    private GameObject laserAria;
    private PlayerLaser playerLaser;

    [Header("사격 위치")]
    [SerializeField] Transform shootLocation;


    //===================변수
    [SerializeField] private AudioClip boostClip;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip bombClip;

    [SerializeField] private bool isMove        = false;
    [SerializeField] private float movespeed    = 40f;
    private bool isWall                         = true;
    private bool isShoot                        = true;
    private bool isGameStart                    = false;
    
    private string wallTag;
    private KeyCode boostKey;
    private KeyCode shootKey;

    private Vector2 limitX = new Vector2(500, -500);
    private Vector2 limitY = new Vector2(500, -500);

    //===================조작 변수
    private Vector2 playerRotate;

    //====================캐릭터 이펙트
    private Animator animator;
    private GameObject playerObj;

    //===================카메라 이벤트 함수
    private bool isOutOfCamera = false;

    void Start()
    {
        Setting_init();
    }

    void Update()
    {
        if (isGameStart == false)
            return;
        /*카메라 멀리 보기 코드 다른 곳으로 뺄 예정
         * if (Input.GetMouseButton(1))
        {
            isOutOfCamera = true;
            CameraManager.Instance.Player_CameraMousePos(this.transform.position);
        }
        else
        {
            isOutOfCamera = false;
            CameraManager.Instance.Player_CameraPos(this.transform.position);
            CameraManager.Instance.Set_Player_LocalPos(this.transform.localPosition);
        }        
        카메라 멀리 보기 코드*/

        isOutOfCamera = false;
        CameraManager.Instance.Player_CameraPos(this.transform.position);
        CameraManager.Instance.Set_Player_LocalPos(this.transform.localPosition);

        playerRotate = Control_LootAtMouse();

        if (isMove == true)
        {
            Control_Move();
        }
        Control_Move_Pause(isWall);

        if (bulletwaitTime < bulletDelay)
        {
            bulletwaitTime += Time.deltaTime;
            isShoot = false;
        }
        else
            isShoot = true;

        if (isShoot == true)
            Control_Shoot();
    }

    private void FixedUpdate()
    {
        //물리 사용은 Fixed로 하라고 한다
        if (isMove == true) //레이저 고정 update
        {
            if (Control_LaserColor())
            {
                Control_Aria_Laser_Active(isMove);
                Control_Aria_Laser_Detect(isMove);
            }
            else
                Control_Aria_Laser_Active(isMove);
        }
        else if (isMove == false)
        {
            Control_LaserColor();
        }
    }

    #region 플레이어 세팅
    private void Setting_init()
    {
        playerAttach    = new PlayerAttach(this.transform.gameObject);
        audioSource     = this.gameObject.transform.Find("PlayerAudio").GetComponent<AudioSource>();
        playerSound     = new PlayerSound(audioSource,boostClip, shootClip, bombClip);
        itemFactory     = new ItemFactory();

        playerRotate    = new Vector2();

        moveLaser       = this.transform.Find("PlayerRotate");
        detactAria      = moveLaser.Find("DetactAria").GetComponent<SpriteRenderer>();
        laserAria       = moveLaser.Find("LaserAria").gameObject;
        playerLaser     = new PlayerLaser();
        playerLaser.Shoot_Setting(laserAria.transform);
        stuff_aria      = moveLaser.Find("WallAria").GetComponent<Stuff_Aria>();
        bulletObject    = GameObject.Find("PlayerShoot").transform.Find("Bullet").transform;
        animator        = this.transform.Find("PlayerFail").GetComponent<Animator>();
        
        Bullet          = new List<Bullet>();
        Setting_Bullet();

        shootTime = 10;
    }

    private void Setting_Bullet()
    {
        for (int i = 0; i < 10; i++)
        {
            Bullet.Add(bulletObject.GetChild(i).GetComponent<Bullet>());
            Bullet[i].gameObject.SetActive(false);
        }
    }

    private void Setting_OtherBullet(BulletData bulletdata)
    {
        for(int i = 0; i < 10; i++)
        {
            Bullet[i].Bullet_Setting(bulletdata.bullet_kind, bulletdata.bullet_Speed);
        }
    }

    internal void Setting_NextStage(Vector2 stagepos)//다음 스테이지 넘어갈 시
    {
        //Setting_Game_Start(true);
        isMove = true;
        isWall = true;
        this.transform.parent = null;
        this.transform.position = stagepos;

        moveLaser.gameObject.SetActive(true);
        Ani_Refrash();

        playerAttach.Setting_Physics();
    }

    internal void Setting_Refrash(Vector2 refrashpos)//스테이지 재시작 할 시
    {
        isMove = true;
        isWall = true;
        isShoot = true;
        _explodable = null;
        wallTag = null;
        this.transform.parent = null;
        this.transform.position = refrashpos;

        moveLaser.gameObject.SetActive(true);
        Control_Aria_Active(true);

        Ani_Refrash();

        playerAttach.Setting_Physics();
        Control_ShootRefrash();
    }

    internal void Setting_StageAria(Vector2 _limitX, Vector2 _limitY)
    {
        limitX = _limitX;
        limitY = _limitY;
    }

    private void Setting_FailGoal()//목표 실패 및 죽었을 때
    {
        Setting_Game_Start(false);
        isMove = false;
        isWall = false;
        moveLaser.gameObject.SetActive(false);
        playerAttach.Physics_Simulate(false);
        Ani_GoalFail();
    }

    internal void Setting_GameEnd()
    {
        isMove = false;
        isWall = false;
    }

    internal void Setting_Pause(bool pause)//일시정지
    {
        isMove = pause;
    }

    internal void Setting_Game_Start(bool check)//게임 시작 확인용도
    {
        isGameStart = check;
    }


    internal void Setting_Trigger(bool check)//트리거 진행 시 캐릭터 안보이게 하기
    {
        moveLaser.gameObject.SetActive(check);
        animator.gameObject.SetActive(check);
    }

    private void Setting_SpecielStage()
    {
        GameManager.Instance.Setting_SpecielStage();
    }

    internal void Setting_ControlKey(KeyCode boost, KeyCode Shoot)          //플레이어 키 변경
    {
        boostKey = boost;
        shootKey = Shoot;
    }

    internal void Setting_SoundValue(float sound)                       //플레이어 사운드 변경
    {
        playerSound.Player_SoundValue(sound);
    }
    #endregion 플레이어 세팅

    #region 플레이어 조작
    private void Control_Move()//플레이어 이동 //슬로우모션 추가함
    {
        if (EventSystem.current.IsPointerOverGameObject() == true)
            return;

        if (Input.GetKey(boostKey) && isWall == true && isMove == true)
        {
            Control_Move_SlowDown(0.2f);
            if (wallTag != null) //플레이어 탈출 시
            {
                if (wallTag == "HingeWall")
                    playerAttach.Detach_Wall();
                else if (wallTag == "StaticWall")
                    playerAttach.Detach_StaticWall();
            }
        }

        if(Input.GetKeyUp(boostKey))
            Control_Move_SlowDown(1f);

        if (Input.GetKeyUp(boostKey) && isWall == true && isMove == true)
        {
            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            if (Control_UseBoost() == false) //부스터 사용 할 수 없을 시
                return;

            this.transform.parent = null;
            this.transform.up = playerRotate; //바꿀 수 있다면 바꿀 것
            isMove = false;
            isWall = false;

            Control_Aria_Active(isMove);
            Control_Aria_Laser_Active(isMove);

            playerAttach.Physics_Velocity(movespeed);//속도 조작

            if (wallTag != null) //플레이어 탈출 시
            {
                if (wallTag == "HingeWall")
                    playerAttach.Detach_Wall();
                else if (wallTag == "StaticWall")
                    playerAttach.Detach_StaticWall();
                else if (wallTag == "BrokenWall")
                {
                    playerAttach.Detach_StaticWall();
                    _explodable.explode();
                    _explodable = null;
                    ExplosionForce ef = GameManager.Instance.GetComponent<ExplosionForce>();
                    ef.doExplosion(transform.position);
                }
            }

            playerAttach.Physics_Simulate(true);//플레이어 물리 작동

            Sound_UseBoost();
        }
    }

    private void Control_Shoot()
    {

        if (Input.GetKey(shootKey) && !Input.GetKey(boostKey) && isShoot == true && isWall == true)
        {
            bulletwaitTime = 0;

            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            if (shootTime == 0)
                return;

            Control_TryShoot();
        }
    }

    private void Control_TryShoot()
    {
        bulltcount++;

        if (bulltcount >= 10) bulltcount = 0;

        if (Bullet[bulltcount].gameObject.activeInHierarchy == false)
        {
            if (Bullet[bulltcount].Bullet_IsMove() == true)
                return;

            if (Control_UseShoot() == false) return;

            Bullet[bulltcount].Bullet_Deactive();
            Bullet[bulltcount].gameObject.transform.position = shootLocation.position;
            Bullet[bulltcount].gameObject.transform.up = playerRotate;
            Bullet[bulltcount].Bullet_Velocity(playerRotate);
            Bullet[bulltcount].gameObject.SetActive(true);
            Bullet[bulltcount].Bullet_Active();
            shootTime--;
            Sound_UseShoot();
        }
    }

    internal void Control_ShootCountUP()
    {
        if (shootTime >= 0 && shootTime < 10)
            shootTime++;
    }

    private void Control_ShootRefrash()
    {
        for(int i = 0; i < Bullet.Count; i++)
        {
            Bullet[i].Bullet_Refrash();
        }
    }

    private Vector2 Control_LootAtMouse()//플레이어 마우스 이동
    {
        Vector2 t_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float mousePosX = (t_mousePos.x - moveLaser.position.x);
        float mousePosY = (t_mousePos.y - moveLaser.position.y);

        Vector2 t_direction = new Vector2((mousePosX),
                                          (mousePosY));

        moveLaser.transform.up = t_direction;
        return t_direction;
    }

    private bool Control_LaserColor()//레이저가 벽에 닿았을 시
    {
        bool detactcheck = stuff_aria.DetactCheck();

        if (isWall == false)
            return true;

        if (detactcheck == true)
        {
            isMove = false;
            detactAria.color = Color.black;
            return false;
        }
        else
        {
            isMove = true;
            detactAria.color = Color.white;
            return true;
        }
    }

    private bool Control_UseBoost()//이동 시 UI로 값 반환
    {
        return GameManager.Instance.Player_MovedUI();
    }
    private bool Control_UseShoot()//사격 시 UI로 값 반환
    {
        return GameManager.Instance.Player_ShootUI();
    }

    private void Control_Move_Pause(bool moved)//플레이어 이동 시 Pause 비활성화
    {
        GameManager.Instance.Player_Moved_Pause(moved);
    }

    private void Control_Aria_Active(bool active)//플레이어 이동 시 범위 활성&비활성
    {
        detactAria.gameObject.SetActive(active);
    }
    
    //레이저 우선 보류...
    //왜 잘 안되는걸까
    //내 코드에 무엇이 잘못되어있는것일까
    //잔 에러가 많다.
    private void Control_Aria_Laser_Detect(bool active)
    {
        if (active == false)
            return;

        //레이케스트 쏘는 위치, 방향, 결과값 등등
        //거리에 따른 레이저 스케일 변화
        laserAria.transform.localScale = playerLaser.Shoot_Laser();
    }

    private void Control_Aria_Laser_Active(bool active)
    {
        playerLaser.Shoot_Laser_Active(active);
    }

    private void Control_Move_SlowDown(float time) //시간 느려지게하기
    {
        GameManager.Instance.Player_Move_Slowdown(time);
    }
    #endregion 플레이어 조작

    #region 플레이어 사운드
    private void Sound_UseBoost()//부스터 사용 시
    {
        playerSound.Player_UseBoost();
    }
    private void Sound_UseShoot()//탄약 사용 시
    {
        playerSound.Player_UseShoot();
    }
    #endregion 플레이어 사운드

    #region 플레이어 애니메이션
    private void Ani_GoalFail()
    {
        animator.SetBool("Player_Fail", true);
    }

    private void Ani_BoostOut()
    {
        animator.SetBool("Player_BoostOut", true);
    }

    private void Ani_Refrash()
    {
        animator.StartPlayback();
        animator.StopPlayback();
        animator.SetBool("Player_Fail", false);
    }
    #endregion 플레이어 애니메이션

    #region 플레이어 물리
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Control_Aria_Active(true);
        isMove = true;
        isWall = true;
        wallTag = collision.gameObject.tag;
        if(wallTag != "Untagged")
            this.transform.parent = collision.transform;


        //GameManager.Instance.Camera_Shake();

        switch (wallTag)
        {
            case "HingeWall":
                playerAttach.Attach_Wall(collision.transform);
                GameManager.Instance.Player_GameEnd();//게임 엔딩 조건
                break;
            case "StaticWall":
                Setting_SpecielStage();
                playerAttach.Attach_StaticWall(collision.transform);
                GameManager.Instance.Player_GameEnd();//게임 엔딩 조건
                break;
            case "GoalWall":
                playerAttach.Attach_StaticWall(collision.transform);
                if (GameManager.Instance.Player_StageGoal() == false)//게임 엔딩 조건
                    Setting_FailGoal();
                break;
            case "BrokenWall":
                this.transform.parent = collision.transform.parent;
                playerAttach.Attach_BrokenWall(collision.transform);
                _explodable = collision.transform.GetComponent<Explodable>();
                GameManager.Instance.Player_GameEnd();
                break;
            case "FlowWall":
                playerAttach.Attach_StaticWall(collision.transform);
                if (GameManager.Instance.Player_FlowGoal() == false)//게임 엔딩 조건
                    Setting_FailGoal();
                break;
            case "Trigger":
                playerAttach.Attach_Wall(collision.transform);
                break;
            case "End":
                GameManager.Instance.Player_PlayEnd();//게임 트리거 진행
                playerAttach.Attach_Wall(collision.transform);
                break;
            case "Enemy"://시스템 바꿀 때 건드릴 곳임
                playerAttach.Attach_StaticWall(collision.transform);
                Control_Move_SlowDown(1f);
                if (GameManager.Instance.Player_StageGoal() == false)//게임 패배 조건
                    Setting_FailGoal();
                break;
        }
    }

    internal void TriggerEnter2D(string tagname)
    {
        switch (tagname)
        {
            case "boost":
                GetItemData("boost", 1);
                break;
            case "shoot":
                GetItemData("shoot", 1);
                break;
            case "Bomb":
                GameManager.Instance.Player_Hit();
                Control_Move_SlowDown(1f);
                if (GameManager.Instance.Player_HP_Check() == false)//게임 패배 조건
                {
                    Setting_Game_Start(false);
                    playerAttach.Attach_BombItem();
                    Setting_FailGoal();
                }
                break;
            case "QuestItem":
                GameManager.Instance.Player_QuestEngage();
                break;
            case "Trigger":
                GameManager.Instance.Player_TriggerGoal();//게임 트리거 진행
                Setting_Trigger(false);
                break;
            case "normalBullet":
                GetItemData("normalBullet", 2);
                break;
            case "bounceBullet":
                GetItemData("bounceBullet", 2);
                break;
            case "EnemyBullet":
                GameManager.Instance.Player_Hit();
                Control_Move_SlowDown(1f);
                if (GameManager.Instance.Player_HP_Check() == false)//게임 패배 조건
                {
                    Setting_Game_Start(false);
                    playerAttach.Attach_BombItem();
                    Setting_FailGoal();
                }
                break;
        }
    }
    
    private void OnBecameInvisible()
    {
        if (isOutOfCamera == true) return; //카메라 멀리 보기 코드임

        Control_Aria_Active(false);
        isMove = true;
        /*Debug.Log("죽었을 때 이동 문제" + this.gameObject.transform.position.x);
        Debug.Log("게임 시작 문제" + isGameStart) ;
        Debug.Log("범위 나감 문제" +  Player_Out_StageAria());*/
        if (this.gameObject.transform.position.x != -1000 && isGameStart == true && Player_Out_StageAria() == true)
            GameManager.Instance.Player_GameOver();
    }

    private bool Player_Out_StageAria()
    {
        //LimitPosX.x = +X || LimitPosX.y = -X
        //LimitPosY.x = +Y || LimitPosY.y = -Y

        if (this.transform.position.x < limitX.x)
            return true;
        else if (this.transform.position.x > limitX.y)
            return true;

        if (this.transform.position.y < limitY.x)
            return true;
        else if (this.transform.position.y > limitY.y)
            return true;

        return false;
    }
    #endregion 플레이어 물리

    #region 아이템 효과 받음
    private void GetItemData(string itemname, int itemvalue)//플레이어가 아이템을 먹음
    {
        int index = 0;
        BulletData bulletdata = new BulletData();
        ItemList itemfindfactory;

        itemfindfactory = itemFactory.GetItemValue(itemname);

        if (itemvalue == 1)
        {
            index = itemfindfactory.getItemStatus(itemname);
            Item_EffectApply(itemname, index);
        }
        else if (itemvalue == 2)
        {
            bulletdata = itemfindfactory.getBulletItemStatus(itemname);
            Item_BulletEffectApply(bulletdata);
        }
    }

    private void Item_EffectApply(string itemname, int value)//아이템 효과 반환
    {
        GameManager.Instance.Player_GetItem(itemname, value);
    }

    private void Item_BulletEffectApply(BulletData bulletdata)
    {
        Setting_OtherBullet(bulletdata);
    }
    #endregion 아이템 효과 받음
}