using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeWall : MonoBehaviour
{
    //============================�̺�Ʈ
    [Header("ȸ���� Ȯ��")]
    [SerializeField] protected bool isRotate;

    [Header("�ð�, �ݽð� ����")]
    [SerializeField] protected bool isReverse;

    [Header("����ġ ����Ǽ� ȸ��")]
    [SerializeField] protected bool isSwitch;


    //============================����
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

    //ȸ�� �� �ۿ��ؾ� �� �͵�
    private void Hinge_Rotate()
    {
        isRotate = isRotate ? false : true;

        Interaction_Rotate(isRotate); //ȸ�� Ŭ���� ��� ��

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


    //��ü�� �ھ� �Ӱ�, �Ͼ��
    internal void Interaction_Rotate(bool isrotate)
    {
        coreRender.color = isrotate ? Color.red : Color.white;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
        {
            Debug.Log("�ι� ����?");
            Hinge_Rotate();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Bullet"))
            Hinge_Rotate();
    }

}