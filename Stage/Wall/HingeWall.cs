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


    public void Start()
    {
        joint = this.gameObject.GetComponent<HingeJoint2D>();
        joint.useMotor = true;
        joint2d     = this.gameObject.GetComponent<HingeJoint2D>().motor;
        coreRender  = this.gameObject.transform.Find("Core").GetComponent<SpriteRenderer>();
    }

    //ȸ�� �� �ۿ��ؾ� �� �͵�
    private void Hinge_Rotate()
    {
        isRotate = isRotate ? false : true;

        Interaction_Rotate(isRotate); //ȸ�� Ŭ���� ��� ��

        if (isRotate == true)
        {
            if (isReverse == true)
                joint2d.motorSpeed = -50f;
            else
                joint2d.motorSpeed = 50f;
        }
        else
            joint2d.motorSpeed = 0f;

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