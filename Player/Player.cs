using UnityEngine;
using UnityEngine.EventSystems;

class PlayerStatus
{
    public float speed;
    public int boost;
}

public class Player : MonoBehaviour
{
    //========================�÷��̾� �������ͽ�(���� Ŭ������ ����)
    PlayerStatus playerStatus;
    //========================�÷��̾� ����(���� Ŭ������ ����)
    PlayerPhysics playerPhysics;
    Transform playerRotateObj;

    //========================�÷��̾� UI
    [SerializeField] Transform moveLaser;
    //========================������
    ItemFactory itemFactory;
    //========================�÷��̾ ���� ����� ��
    private bool isMove = false;
    private bool isWall = false;
    private Transform TouchedWall;
    //========================���� ���
    private float currentRadius = 0;
    private Transform rotateObject;

    //========================���� ����
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
        playerStatus.boost = 1; //������������ �ٲ�
    }

    void Update()
    {
        //ī�޶� �� �־��ֱ�
        CameraManager.Instance.Player_Position(this.transform.position);

        if (isMove == true)
        {
            TryMove();
        }
    }

    #region ===========================================�÷��̾� �������� ����
    internal void PlayerNextStatSetting(int stageboost, Vector2 playerpos) //�÷��̾� ���� �������� �̵�
    {
        playerStatus.boost = stageboost;
        this.transform.position = playerpos;
        isMove = true;

        playerPhysics.PhysicsVelocityControl(0);
    }

    internal void PlayerRefrashSetting(int stageboost, Vector2 playerpos) //�÷��̾� �������� ��������
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
    #endregion ===========================================�÷��̾� �������� ����

    #region ===========================================�÷��̾� ����

    private void TryMove()//�÷��̾� �߻�
    {
        if (isMove == false)
        {
            return;
        }
        //ȸ��
        playerRotate = LookAtMouse();

        if (Input.GetMouseButtonDown(0) && playerStatus.boost != 0)
        {
            if (EventSystem.current.IsPointerOverGameObject() == true)
                return;

            this.transform.parent = null;
            this.transform.up = playerRotate; //�ٲ� �� ������
            isMove = false;
            isWall = false;

            moveLaser.gameObject.SetActive(isMove);

            playerStatus.boost--;

            PlayerBoostUI(playerStatus.boost);//�÷��̾ �̵� �� UI

            playerPhysics.PhysicsVelocityControl(playerStatus.speed);//�߻�

            if (TouchedWall != null) //�÷��̾� ������ ����
            {
                if (TouchedWall.tag == "Wall")
                    DetachWall();
                else if (TouchedWall.tag == "StaticWall")
                    DetachStaticWall();
            }

            playerPhysics.PhysicsSimulateOn();//���� ����

            PlayerMoveSound();//�÷��̾� ���� ����
        }
    }

    private void PlayerBoostUI(int boost)//�÷��̾ �̵� �� UI
    {
        GameManager.Instance.PlayerMoved(boost);
    }

    private void PlayerMoveSound()
    {
        moveSound.Play();
    }

    private void ControlSpeed() //�������̳� ���� ���� �� �ӵ� ����
    {

    }

    private Vector2 LookAtMouse()//�÷��̾ �ٸ� ������ ���ư��� ���� ��ġ �˷���
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

    #endregion ===========================================�÷��̾� ����
    #region ===========================================�÷��̾� ����

    //�߽��� �Ǵ� �� ��ü�� �ְ�, ��ü�� �߽��� �Ǿ� ��� �����κ��� �ݴ��(�״ϱ� ����� ��ġ�� ���� �� ������ �Ųٷ�
    //������ �Ųٷ� �Ǿ����ϱ� ȸ�� ���� �߽� ���� ���Ѹ�ŭ�� �̵��ϰ� ������ ����
    //�׷��� ������ ���� ���� �׻� ������ �� �� �ִ�.

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


    //===========================================��
    private void OnCollisionEnter2D(Collision2D collision) //�� ��ġ
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

    internal void AttachWall(Collision2D collision) //���ϰ� �÷��̾� ����
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

    private void AttachStaticWall(Collision2D collision) //�������� �ʴ� ���ϰ� �÷��̾� ����
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

        if (thisPos.x > block.localScale.x * 0.5f && thisPos.y < block.localScale.y && thisPos.y > -block.localScale.y)//x�� +��� ���̺��� �۰� -��� ���̺��� ũ�� y�� 1���� �۰� -1���� ũ�� 
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

    private void DetachWall() //���ϰ� �÷��̾� �и�
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
    #endregion ===========================================�÷��̾� ����

    #region =====================================������
    private void OnTriggerEnter2D(Collider2D collision) //������ ��ġ
    {
        switch (collision.gameObject.tag)
        {
            case "Item":
                GetItemData("Item", 1);
                collision.gameObject.SetActive(false);
                break;
        }
    }

    private void GetItemData(string itemname, int itemvalue)//������ �������� ���丮 ����
    {
        int index = 0;
        ItemList itemfindfactory;

        itemfindfactory = itemFactory.GetItemValue(itemname);
        index           = itemfindfactory.getItemStatus(itemvalue);

        ItemEffectApply(index);
    }

    private void ItemEffectApply(int effect) //������ ȿ�� ����
    {
        playerStatus.boost += effect;
        PlayerBoostUI(playerStatus.boost);
    }
    #endregion =====================================������


}
