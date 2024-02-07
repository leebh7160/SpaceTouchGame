using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    //�̹����� ��� �߰��ְ� ��ǥ�� ȭ�鿡 ��Ÿ���� ���� �̹����� ���ָ� �Ǵ� �� �ƴұ�?
    //ī�޶��� �߽� ��ġ�������� ��ġ �Ǵ�
    [SerializeField]
    private GameObject Indicator_Object; //ȭ��ǥ
    private RectTransform Indicator_Object_Rect;//ȭ��ǥ Rect
    private Vector2 Goal_Pos            = new Vector2(); //��ǥ�� pos
    private Vector2 Goal_Vec            = new Vector2(); //��ǥ
    private Vector2 Goal_screenVec      = new Vector2(); //Screen ���� ��ǥ ����
    private Vector2 Player_Vec          = new Vector2(); //�÷��̾� ����
    private Vector2 Player_screenVec    = new Vector2(); //Screen ���� Player ����

    private bool isIndicator_active = false;//��Ÿ�� Ȯ��

    private float screenHalfHeight = 0.5f;//ȭ�� ���� ����
    private float screenHalfWidth = 0.5f;//ȭ�� �� ����

    private float angleRU; //������� �밢�� ����
    Vector2 vecRU;
    private void Start()
    {
        Player_screenVec = Camera.main.WorldToScreenPoint(CameraManager.Instance.Get_player_pos());
        Player_Vec = Camera.main.WorldToViewportPoint(CameraManager.Instance.Get_player_pos());

        vecRU = new Vector2(Screen.width, Screen.height) - Player_screenVec;
        vecRU = vecRU.normalized;
        angleRU = Vector2.Angle(vecRU, Vector2.up);
    }

    private void Update()
    {
        if(Indicator_Object.activeInHierarchy == true)
            Indicator_Object_Rect = Indicator_Object.GetComponent<RectTransform>();
        if (isIndicator_active == true)
            Indicator_Show();
    }

    internal void Indicator_goalPos(Vector2 goalpos)//��ǥ ��ġ ��������
    {
        Goal_Pos = goalpos;
        if (Application.isPlaying == false)
            return;
        Goal_screenVec = Camera.main.WorldToScreenPoint(Goal_Pos);
        Goal_Vec = Camera.main.WorldToViewportPoint(Goal_Pos);
    }

    internal void Indicator_Active(bool active)//�ε������� ���� Ȯ��
    {
        isIndicator_active  = active;
        if(Application.isPlaying == true)
            Indicator_Object.gameObject.SetActive(active);
    }

    //���� : https://inyongs.tistory.com/29
    //������ ���
    //x : x' = y : y'
    //x'y = xy'
    //y = xy'/x
    private void Indicator_Show()//�ε������� ȭ��ǥ ���� Ȯ��
    {
        //�ڱ��� ��ġ�� �÷��̾��� ��ġ�� ���� �Ÿ��� ���
        //�� �� �ޱ��� ��ġ�� ���
        Goal_screenVec  = Camera.main.WorldToScreenPoint(Goal_Pos);
        Goal_Vec        = Camera.main.WorldToViewportPoint(Goal_Pos);

        Vector2 targetVec = Goal_screenVec - Player_screenVec;
        targetVec = targetVec.normalized;

        float targetAngle = Vector2.Angle(targetVec, Vector2.up); //0~180
                                                                  //Vector�� ������ �Ǵ� ��, �������� �������� �Ǵ�
        Indicator_Object_Rect.rotation = Quaternion.AngleAxis(-targetAngle, Vector3.forward);


        int sign = Vector3.Cross(targetVec, Vector2.up).z < 0 ? -1 : 1;
        targetAngle *= sign; // -180~180

        //�Ҽ��� ��ǥ�� ��ġ�� -1, 0, 1�� ã������ ���
        float xPrime = Goal_Vec.x - Player_Vec.x;
        float yPrime = Goal_Vec.y - Player_Vec.y;

        //�� ��, �� �Ʒ�, ���� ��, ���� �Ʒ�
        float anchorMinX;
        float anchorMinY;
        float anchorMaxX;
        float anchorMaxY;

        if (-angleRU < targetAngle && angleRU >= targetAngle) //UP�ʿ� ���� ��
        {
            //Y anchor����
            anchorMinY = 0.94f;
            anchorMaxY = 0.94f;

            //y�Ÿ� * ȭ���� �� / y�Ÿ�
            float posX = (Mathf.Abs(xPrime) * screenHalfHeight) / yPrime;

            if (xPrime > 0) //right
            {
                anchorMinX = screenHalfWidth + posX;
                anchorMaxX = screenHalfWidth + posX;

                if (anchorMinX > 0.965f) anchorMinX = 0.965f;
                if (anchorMaxX > 0.965f) anchorMaxX = 0.965f;
                //�̹��� �Ѿ�� �� ����
            }
            else //left
            {
                anchorMinX = screenHalfWidth - posX;
                anchorMaxX = screenHalfWidth - posX;

                if (anchorMinX < 0.035f) anchorMinX = 0.035f;
                if (anchorMaxX < 0.035f) anchorMaxX = 0.035f;
                //�̹��� �Ѿ�� �� ����
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator�� anchor ����
        }
        else if (angleRU < targetAngle && 180 - angleRU >= targetAngle) //Right�ʿ� ���� ��
        {
            //Y anchor����
            anchorMinX = 0.965f;
            anchorMaxX = 0.965f;

            //y�Ÿ� * ȭ���� �� / y�Ÿ�
            float posY = (screenHalfWidth * Mathf.Abs(yPrime)) / xPrime;

            if (yPrime > 0) //up
            {
                anchorMinY = screenHalfHeight + posY;
                anchorMaxY = screenHalfHeight + posY;

                if (anchorMinY > 0.94f) anchorMinY = 0.94f;
                if (anchorMaxY > 0.94f) anchorMaxY = 0.94f;
                //�̹��� �Ѿ�� �� ����
            }
            else //down
            {
                anchorMinY = screenHalfHeight - posY;
                anchorMaxY = screenHalfHeight - posY;

                if (anchorMinY < 0.04f) anchorMinY = 0.04f;
                if (anchorMaxY < 0.04f) anchorMaxY = 0.04f;
                //�̹��� �Ѿ�� �� ����
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator�� anchor ����
        }
        else if ((180 - angleRU < targetAngle && 180 > targetAngle) ||
                (-180 <= targetAngle && angleRU - 180 >= targetAngle)) //Down�ʿ� ���� ��
        {
            //Y anchor����
            anchorMinY = 0.04f;
            anchorMaxY = 0.04f;

            //y�Ÿ� * ȭ���� �� / y�Ÿ�
            float posX = (Mathf.Abs(xPrime) * screenHalfHeight) / -yPrime;

            if (xPrime > 0) //Right
            {
                anchorMinX = screenHalfWidth + posX;
                anchorMaxX = screenHalfWidth + posX;

                if (anchorMinX > 0.965f) anchorMinX = 0.965f;
                if (anchorMaxX > 0.965f) anchorMaxX = 0.965f;
                //�̹��� �Ѿ�� �� ����
            }
            else //Left
            {
                anchorMinX = screenHalfWidth - posX;
                anchorMaxX = screenHalfWidth - posX;

                if (anchorMinX < 0.035f) anchorMinX = 0.035f;
                if (anchorMaxX < 0.035f) anchorMaxX = 0.035f;
                //�̹��� �Ѿ�� �� ����
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);

            //indicator�� anchor ����
        }
        else if (angleRU - 180 < targetAngle && -angleRU >= targetAngle) //Left�ʿ� ���� ��
        {
            //Y anchor����
            anchorMinX = 0.035f;
            anchorMaxX = 0.035f;

            //y�Ÿ� * ȭ���� �� / y�Ÿ�
            float posY = (screenHalfWidth * Mathf.Abs(yPrime)) / -xPrime;

            if (yPrime > 0) //up
            {
                anchorMinY = screenHalfWidth + posY;
                anchorMaxY = screenHalfWidth + posY;

                if (anchorMinY > 0.94f) anchorMinY = 0.94f;
                if (anchorMaxY > 0.94f) anchorMaxY = 0.94f;
                //�̹��� �Ѿ�� �� ����
            }
            else //down
            {
                anchorMinY = screenHalfWidth - posY;
                anchorMaxY = screenHalfWidth - posY;

                if (anchorMinY < 0.04f) anchorMinY = 0.04f;
                if (anchorMaxY < 0.04f) anchorMaxY = 0.04f;
                //�̹��� �Ѿ�� �� ����
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator�� anchor ����
        }

        Indicator_Object_Rect.anchoredPosition = new Vector2(0, 0);
    }
}
