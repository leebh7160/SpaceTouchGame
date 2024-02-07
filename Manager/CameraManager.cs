using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance
    { get { return instance; } }

    //=====================Position
    private Vector2 playerPos;
    private Vector2 playerLocalPos;
    private Vector2 LimitPosX;
    private Vector2 LimitPosY;

    //=====================변수
    private float moveTime;
    private bool chaseCheck = false;

    //=====================카메라 흔들림
    [SerializeField] float shakeTime = 0.05f;
    [SerializeField] float shakeAmount = 0.3f;
    [SerializeField] float cameraSize = 60f;

    private void Awake()
    {
        instance = this;
    }

    #region  카메라 위치 제한
    internal void Stage_CameraLimit(Vector2 limitX, Vector2 limitY)//스테이지 카메라 제한
    {
        LimitPosX = limitX;
        LimitPosY = limitY;
    }
    #endregion 카메라 위치 제한

    #region 플레이어 관련
    internal void Player_CameraPos(Vector2 pos)//실시간 플레이어 위치
    {
        playerPos = pos;
    }

    internal void Player_CameraMousePos(Vector2 pos)//카메라 위치 마우스 따라 이동
    {
        Vector2 mousepos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector2 limitpos = mousepos - playerPos;

        if (limitpos.x      > 20)
            limitpos.x      = 20;
        else if (limitpos.x < -20)
            limitpos.x      = -20;

        if (limitpos.y      > 20)
            limitpos.y      = 20;
        else if (limitpos.y < -20)
            limitpos.y      = -20;

        playerPos = limitpos + pos;
    }

    internal void Player_CameraActive()//카메라 실행
    {
        Camera.main.orthographicSize = cameraSize;
        chaseCheck = true;
        StartCoroutine(CameraChase());
    }

    internal void Player_CameraDeactive()//카메라 종료
    {
        chaseCheck = false;
    }

    internal void Player_CameraReset()
    {
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);
    }


    internal void Player_CameraTrigger()
    {
        Camera.main.transform.localPosition = new Vector3(-80f, 110, -10);
        Camera.main.orthographicSize = 80;
    }

    internal void Player_CameraGameEnd()
    {
        Camera.main.transform.localPosition = new Vector2(-80f, 120);
        Camera.main.orthographicSize = 40;
    }
    #endregion 플레이어 관련

    #region UI플레이어 위치
    internal void Set_Player_LocalPos(Vector2 pos)
    {
        playerLocalPos = pos;
    }
    internal Vector2 Get_player_pos()
    {
        return playerLocalPos;
    }
    #endregion UI플레이어 위치
    #region 코루틴 실행
    private IEnumerator CameraChase()//카메라 이동 코루틴
    {
        while (chaseCheck == true)
        {
            if (shakeTime > 0)
            {
                CameraShaking();
                shakeTime -= Time.deltaTime;
            }
            else
            {
                Vector3 playerMoveControll = playerPos;
                playerMoveControll.z = -10;

                //LimitPosX.x = +X || LimitPosX.y = -X
                if (playerMoveControll.x < LimitPosX.x)
                    playerMoveControll.x = LimitPosX.x;
                else if (playerMoveControll.x > LimitPosX.y)
                    playerMoveControll.x = LimitPosX.y;

                //LimitPosY.x = +Y || LimitPosY.y = -Y
                if (playerMoveControll.y < LimitPosY.x)
                    playerMoveControll.y = LimitPosY.x;
                else if (playerMoveControll.y > LimitPosY.y)
                    playerMoveControll.y = LimitPosY.y;

                Camera.main.transform.position = playerMoveControll;
            }
            yield return null;
        }
    }

    private void CameraShaking()//카메라 벽에 받으면 흔들림
    {
        Camera.main.transform.position = Random.insideUnitSphere * shakeAmount + Camera.main.transform.position;
    }

    private void CameraFall() //카메라 부드럽게
    {
        moveTime += Time.deltaTime / 1.5f;
        Camera.main.orthographicSize = Mathf.Lerp(5f, 10f, moveTime);
    }

    #endregion 코루틴 실행

    internal void CameraShake()
    {
        shakeTime += 0.1f;
    }
}
