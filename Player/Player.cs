using UnityEngine;
using UnityEngine.EventSystems;

class PlayerStatus
{
    public float speed;
    public int boost;
}

public class Player : MonoBehaviour
{
    //========================플레이어 스테이터스(따로 클래스로 빼기)
    PlayerStatus playerStatus;
    //========================플레이어 물리(따로 클래스로 빼기)
    PlayerPhysics playerPhysics;
    Transform playerRotateObj;

    //========================플레이어 UI
    [SerializeField] Transform moveLaser;
    //========================아이템
    ItemFactory itemFactory;
    //========================플레이어가 벽에 닿았을 때
    private bool isMove = false;
    private bool isWall = false;
    private Transform TouchedWall;
    //========================물리 계산
    private float currentRadius = 0;
    private Transform rotateObject;

    //========================사운드 관련
    AudioSource moveSound;

    private Vector2 playerRotate;

    private void Start()
    {
        playerPhysics = new PlayerPhysics();
        playerStatus = new PlayerStatus();
        itemFactory = new ItemFactory();
        playerRotate = new Vector2();

        playerRotateObj = this.transform.Find("RotateObject").transform;

        moveSound = this.transform.Find("PlayerAudio").GetComponent<AudioSource>();
        playerPhysics.SetRigid2D(this.gameObject.GetComponent<Rigidbody2D>());
        playerPhysics.SetPlayerObj(this.gameObject);

        playerStatus.speed = 23f;
        playerStatus.boost = 1; //스테이지마다 바뀜
    }

    void Update()
    {
        //카메라에 값 넣어주기
        CameraManager.Instance.Player_Position(this.transform.position);

        if (isMove == true)
        {
            TryMove();
        }
    }

    #region ===========================================플레이어 스테이지 세팅
    internal void PlayerNextStatSetting(int stageboost, Vector2 playerpos) //플레이어 다음 스테이지 이동
    {
        playerStatus.boost = stageboost;
        this.transform.position = playerpos;
        isMove = true;

        playerPhysics.PhysicsVelocityControl(0);
    }

    internal void PlayerRefrashSetting(int stageboost, Vector2 playerpos) //플레이어 스테이지 리프레시
    {
        isMove = true;
        if (TouchedWall != null && TouchedWall.GetComponent<Joint2D>() != null)
            TouchedWall.GetComponent<Joint2D>().connectedBody = null;
        TouchedWall = null;
        this.transform.parent = null;
        this.transform.position = playerpos;

        playerStatus.boost = stageboost;
        PlayerBoostUI(playerStatus.boost);

        playerPhysics.PhysicsSimulateOff();

        playerPhysics.PhysicsVelocityControl(0);


    }
    #endregion ===========================================플레이어 스테이지 세팅

    #region ===========================================플레이어 조작

    private void TryMove()//플레이어 발사
    {
        if (isMove == false)
        {
            return;
        }
        //회전
        playerRotate = LookAtMouse();

        if (Input.GetMouseButtonDown(0) && playerStatus.boost != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            this.transform.parent = null;
            this.transform.up = playerRotate; //바꿀 수 없을까
            isMove = false;
            isWall = false;

            moveLaser.gameObject.SetActive(isMove);

            playerStatus.boost--;

            PlayerBoostUI(playerStatus.boost);//플레이어가 이동 시 UI

            playerPhysics.PhysicsVelocityControl(playerStatus.speed);//발사

            if (TouchedWall != null) //플레이어 벽에서 떼기
            {
                if (TouchedWall.tag == "Wall")
                    DetachWall();
                else if (TouchedWall.tag == "StaticWall")
                    DetachStaticWall();
            }

            playerPhysics.PhysicsSimulateOn();//물리 실행

            PlayerMoveSound();//플레이어 사운드 실행
        }
    }

    private void PlayerBoostUI(int boost)//플레이어가 이동 시 UI
    {
        GameManager.Instance.PlayerMoved(boost);
    }

    private void PlayerMoveSound()
    {
        moveSound.Play();
    }

    private void ControlSpeed() //아이템이나 뭔가 했을 때 속도 조정
    {

    }

    private Vector2 LookAtMouse()//플레이어가 다른 각도로 날아가기 위해 위치 알려줌
    {
        Vector2 t_mousePos = CameraManager.Instance.Camera.ScreenToWorldPoint(Input.mousePosition);

        float mousePosX = (t_mousePos.x - playerRotateObj.position.x);
        float mousePosY = (t_mousePos.y - playerRotateObj.position.y);

        /*Vector2 t_direction = new Vector2((t_mousePos.x - playerRotateObj.position.x),
                                          (t_mousePos.y - playerRotateObj.position.y));*/
        Vector2 t_direction = new Vector2((mousePosX),
                                          (mousePosY));

        /*Debug.Log(t_direction);
        Debug.Log(5 * (Mathf.Cos(90 + currentRadius)));*/
        /*if (t_direction.x < 5 * (Mathf.Cos(90)))
            t_direction.x = 5 * (Mathf.Cos(90));
        if (t_direction.x > -(5 * (Mathf.Cos(90))))
            t_direction.x = -(5 * (Mathf.Cos(90)));
        if (t_direction.y < Mathf.Cos(90))
            t_direction.y = Mathf.Cos(90);*/

        //this.transform.up = t_direction;
        playerRotateObj.transform.up = t_direction;
        return t_direction;
    }

    #endregion ===========================================플레이어 조작
    #region ===========================================플레이어 물리

    //중심이 되는 한 객체가 있고, 객체가 중심이 되어 닿는 각도로부터 반대로(그니까 닿았을 위치에 섰을 때 각도를 거꾸로
    //각도가 거꾸로 되었으니까 회전 각은 중심 각을 더한만큼만 이동하고 각도를 제한
    //그러면 각도가 어디로 가던 항상 제한을 줄 수 있다.

    /*private void PlayerWallTouch()
    {
        if (wallTouched == null)
            return;

        float touchedX = 0;
        float touchedY = 0;


        Vector2 centerDot = wallTouched.transform.position;

        float length = Vector2.Distance(centerDot, wallTouchLocation);
        joint -=wallTouched.rotation.z;
        Debug.Log(joint);
        //touchedX = wallTouchLocation.x + Mathf.Cos((wallTouched.rotation.z + Mathf.PI / 11) * 30);
        //touchedY = wallTouchLocation.y + Mathf.Cos((wallTouched.rotation.z + Mathf.PI / 11) * 30);
        //touchedX = Mathf.Cos(Mathf.Deg2Rad * 360 / wallTouched.rotation.z) * length;
        //touchedY = Mathf.Sin(Mathf.Deg2Rad * 360 / wallTouched.rotation.z) * length;
        //touchedX = centerDot.x + Mathf.Cos(Mathf.Deg2Rad * 360 / wallTouched.rotation.z) * length;
        //touchedY = centerDot.y - Mathf.Sin(Mathf.Deg2Rad * 360 / wallTouched.rotation.z) * length;
        touchedX = centerDot.x + (Mathf.Cos(joint * (Mathf.PI / 180)) * length) * -1;
        touchedY = centerDot.y - (Mathf.Sin(joint * (Mathf.PI / 180)) * length) * -1;

        //touchedX = centerDot.x + (length * Mathf.Cos(wallTouched.rotation.z * Mathf.PI / 180)); 
        //touchedY = centerDot.y - (length * Mathf.Sin(wallTouched.rotation.z * Mathf.PI / 180));


        this.transform.position = new Vector3(touchedX, touchedY);
    }*/


    //===========================================벽
    private void OnCollisionEnter2D(Collision2D collision) //벽 터치
    {
        moveLaser.gameObject.SetActive(true);
        switch (collision.gameObject.tag)
        {
            case "Wall":
                AttachWall(collision);
                break;
            case "StaticWall":
                AttachStaticWall(collision);
                break;
            case "Goal":
                GoalWall();
                break;
            case "OutWall":
                AttachOutWall();
                break;
        }
    }

    internal void AttachWall(Collision2D collision) //벽하고 플레이어 접촉
    {

        if (TouchedWall == collision.transform) return;

        isWall = true;
        isMove = true;


        TouchedWall = collision.transform;
        this.transform.parent = TouchedWall;

        playerPhysics.PhysicsAttachWall();
        TouchedWall.GetComponent<Joint2D>().connectedBody = playerPhysics.GetRigid2D();

        AttachPlayerRotate(TouchedWall.GetChild(0));
    }

    private void AttachStaticWall(Collision2D collision) //움직이지 않는 벽하고 플레이어 접촉
    {

        if (TouchedWall == collision.transform) return;

        isWall = true;
        isMove = true;


        TouchedWall = collision.transform;
        this.transform.parent = TouchedWall;

        playerPhysics.PhysicsAttchStaticWall();

        AttachPlayerRotate(TouchedWall.GetChild(0));
    }

    private void AttachPlayerRotate(Transform block)
    {
        Vector2 thisPos = this.transform.localPosition;
        Vector3 tempPos = new Vector3();

        if (thisPos.x > block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)//x가 +블록 길이보다 작고 -블록 길이보다 크게 y가 1보다 작고 -1보다 크고 
            tempPos = new Vector3(0, 0, 270);
        if (thisPos.x < -block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)
            tempPos = new Vector3(0, 0, 90);

        if (thisPos.y > block.localScale.y)
            tempPos = new Vector3(0, 0, 0);
        if (thisPos.y < -block.localScale.y)
            tempPos = new Vector3(0, 0, 180);

        this.transform.localRotation = Quaternion.Euler(tempPos);
    }

    private void AttachOutWall()
    {
        StageManager.Instance.StageRefrashBtn();
    }

    private void GoalWall()
    {
        if (playerStatus.boost == 0)
            GameManager.Instance.PlayerGoalIn();
        else
            StageManager.Instance.StageRefrashBtn();
    }

    private void DetachWall() //벽하고 플레이어 분리
    {
        if (TouchedWall == null) return;

        this.transform.parent = null;
        TouchedWall.GetComponent<Joint2D>().connectedBody = null;
    }

    private void DetachStaticWall()
    {
        if (TouchedWall == null) return;

        this.transform.parent = null;
    }
    #endregion ===========================================플레이어 물리

    #region =====================================아이템
    private void OnTriggerEnter2D(Collider2D collision) //아이템 터치
    {
        switch (collision.gameObject.tag)
        {
            case "Item":
                GetItemData("Item", 1);
                collision.gameObject.SetActive(false);
                break;
        }
    }

    private void GetItemData(string itemname, int itemvalue)//아이템 가져오는 팩토리 패턴
    {
        int index = 0;
        ItemList itemfindfactory;

        itemfindfactory = itemFactory.GetItemValue(itemname);
        index           = itemfindfactory.getItemStatus(itemvalue);

        ItemEffectApply(index);
    }

    private void ItemEffectApply(int effect) //아이템 효과 적용
    {
        playerStatus.boost += effect;
        PlayerBoostUI(playerStatus.boost);
    }
    #endregion =====================================아이템


}
