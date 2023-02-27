using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static CameraManager instance;
    public static CameraManager Instance
    {
        get { return instance; }
    }

    private Camera camera;
    public Camera Camera
    {
        get { return camera; }
    }


    private float shootTime = 0;
    private float stageCamLimit = 30;

    private Vector2 playerPos_Realtime;

    private void Awake()
    {
        instance = this;
        camera = Camera.main;
    }

    //============================================stage camera
    internal void Stage_CameraLimit(float limit)
    {
        stageCamLimit = limit;
    }
    //============================================stage camera^^
    //============================================playerPos camera
    internal void Player_Position(Vector2 player_pos)
    {
        playerPos_Realtime = player_pos;
    }
    //============================================playerPos camera^^



    public IEnumerator CameraChase()//카메라 플레이어 추적 기능
    {
        while (true)
        {
            Vector3 playerMoveControll = playerPos_Realtime;
            playerMoveControll.z = -10;

            if (playerMoveControll.x < 0f)
                playerMoveControll.x = 0f;
            else if (playerMoveControll.x > stageCamLimit)
                playerMoveControll.x = stageCamLimit;

            if (playerMoveControll.y < 0)
                playerMoveControll.y = 0;
            else if (playerMoveControll.y > 20)
                playerMoveControll.y = 20;

            camera.transform.position = playerMoveControll;
            yield return null;
        }
        yield return null;
    }

    private void CameraFall() //어느 위치로 스무스하게 이동
    {
        shootTime += Time.deltaTime / 1.5f;
        camera.orthographicSize = Mathf.Lerp(5f, 10f, shootTime);
    }
}
