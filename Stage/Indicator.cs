using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    //이미지는 계속 뜨고있고 목표가 화면에 나타났을 때만 이미지를 꺼주면 되는 것 아닐까?
    //카메라의 중심 위치에서부터 위치 판단
    [SerializeField]
    private GameObject Indicator_Object; //화살표
    private RectTransform Indicator_Object_Rect;//화살표 Rect
    private Vector2 Goal_Pos            = new Vector2(); //목표의 pos
    private Vector2 Goal_Vec            = new Vector2(); //목표
    private Vector2 Goal_screenVec      = new Vector2(); //Screen 상의 목표 벡터
    private Vector2 Player_Vec          = new Vector2(); //플레이어 벡터
    private Vector2 Player_screenVec    = new Vector2(); //Screen 상의 Player 벡터

    private bool isIndicator_active = false;//나타남 확인

    private float screenHalfHeight = 0.5f;//화면 높이 절반
    private float screenHalfWidth = 0.5f;//화면 폭 절반

    private float angleRU; //우측상단 대각선 각도
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

    internal void Indicator_goalPos(Vector2 goalpos)//목표 위치 가져오기
    {
        Goal_Pos = goalpos;
        if (Application.isPlaying == false)
            return;
        Goal_screenVec = Camera.main.WorldToScreenPoint(Goal_Pos);
        Goal_Vec = Camera.main.WorldToViewportPoint(Goal_Pos);
    }

    internal void Indicator_Active(bool active)//인디케이터 실행 확인
    {
        isIndicator_active  = active;
        if(Application.isPlaying == true)
            Indicator_Object.gameObject.SetActive(active);
    }

    //참조 : https://inyongs.tistory.com/29
    //닮은비를 사용
    //x : x' = y : y'
    //x'y = xy'
    //y = xy'/x
    private void Indicator_Show()//인디케이터 화살표 지속 확인
    {
        //자기의 위치와 플레이어의 위치를 빼서 거리를 계산
        //그 후 앵글의 위치를 계산
        Goal_screenVec  = Camera.main.WorldToScreenPoint(Goal_Pos);
        Goal_Vec        = Camera.main.WorldToViewportPoint(Goal_Pos);

        Vector2 targetVec = Goal_screenVec - Player_screenVec;
        targetVec = targetVec.normalized;

        float targetAngle = Vector2.Angle(targetVec, Vector2.up); //0~180
                                                                  //Vector의 외적을 판단 즉, 우측인지 좌측인지 판단
        Indicator_Object_Rect.rotation = Quaternion.AngleAxis(-targetAngle, Vector3.forward);


        int sign = Vector3.Cross(targetVec, Vector2.up).z < 0 ? -1 : 1;
        targetAngle *= sign; // -180~180

        //소수고 목표의 위치를 -1, 0, 1로 찾기위해 계산
        float xPrime = Goal_Vec.x - Player_Vec.x;
        float yPrime = Goal_Vec.y - Player_Vec.y;

        //왼 위, 왼 아래, 오른 위, 오른 아래
        float anchorMinX;
        float anchorMinY;
        float anchorMaxX;
        float anchorMaxY;

        if (-angleRU < targetAngle && angleRU >= targetAngle) //UP쪽에 있을 때
        {
            //Y anchor지정
            anchorMinY = 0.94f;
            anchorMaxY = 0.94f;

            //y거리 * 화면의 반 / y거리
            float posX = (Mathf.Abs(xPrime) * screenHalfHeight) / yPrime;

            if (xPrime > 0) //right
            {
                anchorMinX = screenHalfWidth + posX;
                anchorMaxX = screenHalfWidth + posX;

                if (anchorMinX > 0.965f) anchorMinX = 0.965f;
                if (anchorMaxX > 0.965f) anchorMaxX = 0.965f;
                //이미지 넘어가는 것 방지
            }
            else //left
            {
                anchorMinX = screenHalfWidth - posX;
                anchorMaxX = screenHalfWidth - posX;

                if (anchorMinX < 0.035f) anchorMinX = 0.035f;
                if (anchorMaxX < 0.035f) anchorMaxX = 0.035f;
                //이미지 넘어가는 것 방지
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator의 anchor 지정
        }
        else if (angleRU < targetAngle && 180 - angleRU >= targetAngle) //Right쪽에 있을 때
        {
            //Y anchor지정
            anchorMinX = 0.965f;
            anchorMaxX = 0.965f;

            //y거리 * 화면의 반 / y거리
            float posY = (screenHalfWidth * Mathf.Abs(yPrime)) / xPrime;

            if (yPrime > 0) //up
            {
                anchorMinY = screenHalfHeight + posY;
                anchorMaxY = screenHalfHeight + posY;

                if (anchorMinY > 0.94f) anchorMinY = 0.94f;
                if (anchorMaxY > 0.94f) anchorMaxY = 0.94f;
                //이미지 넘어가는 것 방지
            }
            else //down
            {
                anchorMinY = screenHalfHeight - posY;
                anchorMaxY = screenHalfHeight - posY;

                if (anchorMinY < 0.04f) anchorMinY = 0.04f;
                if (anchorMaxY < 0.04f) anchorMaxY = 0.04f;
                //이미지 넘어가는 것 방지
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator의 anchor 지정
        }
        else if ((180 - angleRU < targetAngle && 180 > targetAngle) ||
                (-180 <= targetAngle && angleRU - 180 >= targetAngle)) //Down쪽에 있을 때
        {
            //Y anchor지정
            anchorMinY = 0.04f;
            anchorMaxY = 0.04f;

            //y거리 * 화면의 반 / y거리
            float posX = (Mathf.Abs(xPrime) * screenHalfHeight) / -yPrime;

            if (xPrime > 0) //Right
            {
                anchorMinX = screenHalfWidth + posX;
                anchorMaxX = screenHalfWidth + posX;

                if (anchorMinX > 0.965f) anchorMinX = 0.965f;
                if (anchorMaxX > 0.965f) anchorMaxX = 0.965f;
                //이미지 넘어가는 것 방지
            }
            else //Left
            {
                anchorMinX = screenHalfWidth - posX;
                anchorMaxX = screenHalfWidth - posX;

                if (anchorMinX < 0.035f) anchorMinX = 0.035f;
                if (anchorMaxX < 0.035f) anchorMaxX = 0.035f;
                //이미지 넘어가는 것 방지
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);

            //indicator의 anchor 지정
        }
        else if (angleRU - 180 < targetAngle && -angleRU >= targetAngle) //Left쪽에 있을 때
        {
            //Y anchor지정
            anchorMinX = 0.035f;
            anchorMaxX = 0.035f;

            //y거리 * 화면의 반 / y거리
            float posY = (screenHalfWidth * Mathf.Abs(yPrime)) / -xPrime;

            if (yPrime > 0) //up
            {
                anchorMinY = screenHalfWidth + posY;
                anchorMaxY = screenHalfWidth + posY;

                if (anchorMinY > 0.94f) anchorMinY = 0.94f;
                if (anchorMaxY > 0.94f) anchorMaxY = 0.94f;
                //이미지 넘어가는 것 방지
            }
            else //down
            {
                anchorMinY = screenHalfWidth - posY;
                anchorMaxY = screenHalfWidth - posY;

                if (anchorMinY < 0.04f) anchorMinY = 0.04f;
                if (anchorMaxY < 0.04f) anchorMaxY = 0.04f;
                //이미지 넘어가는 것 방지
            }

            Indicator_Object_Rect.anchorMin = new Vector2(anchorMinX, anchorMinY);
            Indicator_Object_Rect.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            //indicator의 anchor 지정
        }

        Indicator_Object_Rect.anchoredPosition = new Vector2(0, 0);
    }
}
