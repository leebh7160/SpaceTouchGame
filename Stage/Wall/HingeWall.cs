using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeWall : MonoBehaviour
{
    //============================이벤트
    [Header("회전을 확인")]
    [SerializeField] protected bool isRotate;

    [Header("시계, 반시계 지정")]
    [SerializeField] protected bool isReverse;

    [Header("스위치 연결되서 회전")]
    [SerializeField] protected bool isSwitch;


    //============================참조
    private HingeJoint2D joint;
    protected JointMotor2D joint2d;
    private SpriteRenderer coreRender;
    private Rigidbody2D rigid2d;

    internal void OnEnable()
    {
        Hinge_refrash();
    }

    internal void Hinge_refrash()
    {
        joint                   = this.gameObject.GetComponent<HingeJoint2D>();
        joint.useMotor          = true;
        joint2d                 = this.gameObject.GetComponent<HingeJoint2D>().motor;
        rigid2d                 = this.gameObject.GetComponent<Rigidbody2D>();
        coreRender              = this.gameObject.transform.Find("Core").GetComponent<SpriteRenderer>();

        isRotate                = false;
        rigid2d.freezeRotation  = true;
        joint2d.motorSpeed      = 0f;
        joint.motor             = joint2d;
        Interaction_Rotate(false);
    }

    //회전 시 작용해야 할 것들
    private void Hinge_Rotate()
    {
        isRotate = isRotate ? false : true;

        Interaction_Rotate(isRotate); //회전 클래스 사용 시

        if (isRotate == true)
        {
            rigid2d.freezeRotation = false;
            if (isReverse == true)
                joint2d.motorSpeed = -50f;
            else
                joint2d.motorSpeed = 50f;
        }
        else
        {
            rigid2d.freezeRotation = true;
            joint2d.motorSpeed = 0f;
        }

        joint.motor = joint2d;
    }


    //객체의 코어 붉게, 하얗게
    internal void Interaction_Rotate(bool isrotate)
    {
        coreRender.color = isrotate ? Color.red : Color.white;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
            Debug.Log("두번 터짐?");
            Hinge_Rotate();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
            Hinge_Rotate();
    }

}