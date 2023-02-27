using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    GameUI gameUI;//게임 UI를 일반화할 필요가 있는가? 싱글톤 문제 때문에?
    Player player;

    private void Start()
    {
        ResolutionSetting();
        instance = this;
        gameUI = GameObject.Find("GamePlayUI").GetComponent<GameUI>();
        player = GameObject.Find("Player").GetComponent<Player>();

        CameraManager.Instance.Stage_CameraLimit(30);
        StartCoroutine(CameraManager.Instance.CameraChase());
    }
    #region////////////////////////////////해상도 관련
    private void ResolutionSetting()
    {
        int setWidth = 2560;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);
        //넓이를 고정하고, 해상도가 지정된 높이와 너비를 나누어서 넓이로 곱하면
        //1680/960 * 1920 = 3360

        if((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) //비율에 따라 해상도를 낮춘다
        {
            float newWidth = ((float)setWidth / setHeight) / ((float)deviceWidth / deviceHeight);
            Camera.main.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = ((float)deviceWidth / deviceHeight) / ((float)setWidth / setHeight);
            Camera.main.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
    #endregion////////////////////////////////해상도 관련

    #region ///////////////////////////////////////UI관련
    internal void MainStageUI()
    {
        gameUI.MainStageUI(true);
    }
    internal void NextStageUI(bool switchCheck)
    {
        gameUI.NextStageUI(switchCheck);
    }
    #endregion ///////////////////////////////////////UI관련

    #region ///////////////////////////////////////플레이어 관련

    internal void PlayerMoved(int movecount)//플레이어가 이동 시
    {
        gameUI.PlayUI_Moved(movecount);
    }

    internal void PlayerGetItem(int movecount)//플레이어가 아이템 획득 시
    {
        gameUI.PlayUI_Moved(movecount);
        //이펙트라던가 실행
    }

    internal void PlayerGoalIn() //플레이어가 goal에 도착했을 때
    {
        gameUI.NextStageUI(true);

        StopCoroutine(CameraManager.Instance.CameraChase());
    }

    internal void PlayerNextStage() //플레이어가 다음 스테이지 버튼을 눌렀을 때
    {
        int stageboost = 0;
        float stageCamLimit = 0;
        Vector2 stage_StartPos = new Vector2();
        StageDataList stage_DataList = new StageDataList();

        gameUI.NextStageUI(false);//스테이지 UI끄기 (로드를 만들기)

        StageManager.Instance.StageNextLoad();//다음 스테이지 로드

        stage_DataList  = StageManager.Instance.GetNextStageData();//로드 후 스테이지 관리 데이터 받기
        stageboost      = stage_DataList.StageBoost;
        stage_StartPos  = stage_DataList.StageStartPos;
        stageCamLimit   = stage_DataList.StageCamLimit;


        player.PlayerNextStatSetting(stageboost, stage_StartPos);//플레이어가 필요한 데이터 주기

        gameUI.PlayUI_Boost(stageboost);//플레이어 아이템 UI조정

        CameraManager.Instance.Stage_CameraLimit(stageCamLimit);//카메라 추적 한계범위 주기
                    
        StartCoroutine(CameraManager.Instance.CameraChase());//카메라 추적 작동

    }

    internal void PlayerReSetting()
    {
        int stageboost = 0;
        float stageCamLimit = 0;
        Vector2 stage_StartPos = new Vector2();
        StageDataList stage_DataList = new StageDataList();

        stage_DataList = StageManager.Instance.GetNextStageData();//로드 후 스테이지 관리 데이터 받기
        stageboost = stage_DataList.StageBoost;
        stage_StartPos = stage_DataList.StageStartPos;
        stageCamLimit = stage_DataList.StageCamLimit;

        player.PlayerRefrashSetting(stageboost, stage_StartPos);//플레이어가 필요한 데이터 주기
    }
    #endregion ///////////////////////////////////////플레이어 관련

}
