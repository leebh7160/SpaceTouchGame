using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

enum PlayBehavior
{
    boost, shoot, itemboost, itemshoot, Hit
}

#region 해상도 관련
public class ResolutionClass //해상도 관련
{
    private FullScreenMode fullScreenMode;
    private List<Resolution> resolutions_List = new List<Resolution>();

    public ResolutionClass()
    {
        Resolution_Init();
    }

    internal List<Resolution> Resolution_GetData()
    {
        return resolutions_List;
    }

    private void Resolution_Init()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions_List.Add(Screen.resolutions[i]);
        }
    }

    internal void Resolution_Change(int resNum = 0)
    {
        Screen.SetResolution(resolutions_List[resNum].width,
            resolutions_List[resNum].height,
            fullScreenMode);
    }

    internal void Resolution_FullScreen(bool isFull)
    {
        fullScreenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

}
#endregion

internal class PlayerStatus
{
    internal int Boost;
    internal int Shoot;
    internal int Heartpoint;
}

public class GameManager : MonoBehaviour
{
    //==========================싱글톤
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    //==========================참조
    private GameUI gameUI;
    private ResolutionClass resolutionClass;
    private Player player;
    private PlayerStatus playerStatus;
    private int playerHP = 3;
    //==========================참조^^

    private int quest_ItemCheck = 0;
    private int quest_StageCheck = 0;
    //==========================
    //==========================
    [SerializeField]
    private Animator triggerAni;
    [SerializeField]
    private Animator playEndAni;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        instance = this;
        resolutionClass = new ResolutionClass();

        ResolutionSetting();
        PlayerSetting();
    }


    #region 해상도
    private void ResolutionSetting()
    {

    }

    internal List<Resolution> GetResolution()
    {
        return resolutionClass.Resolution_GetData();
    }

    internal void Resolution_Change(int resolution_num = 0)
    {
        resolutionClass.Resolution_Change(resolution_num);
    }
    #endregion 해상도

    #region 세팅
    private void PlayerSetting()
    {
        player                  = GameObject.Find("Player").GetComponent<Player>();
        gameUI                  = GameObject.Find("GamePlayUI").GetComponent<GameUI>();
        playerStatus            = new PlayerStatus();
        playerStatus.Heartpoint = playerHP;
    }

    #endregion 세팅

    #region 플레이어 관련

    internal GameObject Get_Enemy_PlayerPos() //총알에 맞았을 시 적군이 가져갈 플레이어의 포지션
    {
        if (player != null)
            return player.gameObject;
        return null;
    }

    internal bool Player_MovedUI()//플레이어가 이동 시
    {
        if (playerStatus.Boost <= 0) return false;

        playerStatus.Boost--;

        Player_Behavior(PlayBehavior.boost);

        return true;
    }

    internal void Player_Moved_Pause(bool moved)//일시정지 제한
    {
        gameUI.PauseUI_Interactable(moved);
    }

    internal bool Player_ShootUI()//플레이어가 사격 시
    {
        if (playerStatus.Shoot <= 0) return false;


        playerStatus.Shoot--;

        Player_Behavior(PlayBehavior.shoot);
        return true;
    }

    internal void Player_GameEnd()//플레이어 게임 실패
    {
        if (playerStatus.Boost <= 0)
            player.Setting_GameEnd();
    }
   

    internal void Player_GetItem(string itemname, int value)//플레이어가 아이템 획득 시
    {
        PlayBehavior temp = new PlayBehavior();
        switch (itemname)
        {
            case "boost":
                playerStatus.Boost += value;
                temp = PlayBehavior.boost;
                break;
            case "shoot":
                playerStatus.Shoot += value;
                temp = PlayBehavior.shoot;
                break;
        }
        Player_Behavior(temp);
    }

    private void Player_Behavior(PlayBehavior behavior)//플레이어가 행동 시 스테이터스 확인
    {
        switch(behavior)
        {
            case PlayBehavior.boost:
                gameUI.PlayerUI_Moved(playerStatus.Boost);
                break;
            case PlayBehavior.shoot:
                gameUI.PlayerUI_Shoot(playerStatus.Shoot);
                break;
            case PlayBehavior.itemboost:
                gameUI.PlayerUI_SetBoost(playerStatus.Boost);
                break;
            case PlayBehavior.itemshoot:
                gameUI.PlayerUI_SetShoot(playerStatus.Shoot);
                break;
        }
    }

    internal void Player_Hit()
    {
        playerStatus.Heartpoint -= 1;
        gameUI.PlayerUI_Hp(playerStatus.Heartpoint);

        if(playerStatus.Heartpoint > 0)
            gameUI.PlayerUI_Hit_Alert();
    }

    private void Player_StageSetting(Vector2 startpos, int boost, int shoot, int stagenum) //플레이어 스테이지 세팅
    {
        quest_StageCheck = stagenum;
        player.Setting_NextStage(startpos);
        playerStatus.Boost = boost;
        playerStatus.Shoot = shoot;
        playerStatus.Heartpoint = playerHP;
        gameUI.PlayerUI_Hp(playerStatus.Heartpoint);
    }

    private void Player_StageOut_Setting(Vector2 limitX, Vector2 limitY)
    {
        player.Setting_StageAria(limitX, limitY);
    }

    private void Player_StageRefrash(Vector2 refrashpos, int boost, int shoot) //플레이어 리프래쉬
    {
        quest_ItemCheck = 0;
        UI_QuestCheck(quest_StageCheck);
        player.Setting_Refrash(refrashpos);
        playerStatus.Boost = boost;
        playerStatus.Shoot = shoot;
        playerStatus.Heartpoint = playerHP;
        gameUI.PlayerUI_Hp(playerStatus.Heartpoint);
        Camera_ChaseDeactive();
        Camera_ChaseActive();

    }

    internal void Player_Pause()          //일시정지
    {
        player.Setting_Pause(false);
        player.Setting_Game_Start(false);
        Debug.Log("Pause : ");
    }

    internal void Player_Move_Slowdown(float time)
    {
        Time.timeScale = time;
    }

    internal void Player_OutOfAria()                //플레이어 화면 나감
    {
        //gameUI.PlayerUI_Replay();
    }

    internal void Player_GameOver()                 //플레이어 게임 오버
    {
        player.Setting_Game_Start(false);
        gameUI.PauseUI_GameOver(true);
    }

    internal void Player_OtherScreen()
    {
        player.Setting_NextStage(new Vector2(-1000, 0));
    }

    internal void Player_ShootCountUp()
    {
        player.Control_ShootCountUP();
    }


    internal void Player_QuestEngage() //퀘스트 아이템 먹을 때
    {
        string questText;
        switch (quest_StageCheck)
        {
            case 17:
                quest_ItemCheck++;
                questText = " / 1";
                gameUI.PlayerUI_QuestNumber(quest_ItemCheck, questText);
                break;
            case 24:
                quest_ItemCheck++;
                questText = " / 3";
                gameUI.PlayerUI_QuestNumber(quest_ItemCheck, questText);
                break;
        }
    }

    #endregion 플레이어 관련

    #region UI 관련
    internal void UI_MainStage() //게임 메인 스테이지 UI 조작
    { }

    internal void UI_SelectStage(int stagenum) //게임 다음 스테이지 UI 조작
    {
        if (stagenum >= 41)
            stagenum = 41;
        StageDataList tempList  = new StageDataList();
        tempList                = StageManager.Instance.Stage_Select(stagenum);
        UI_QuestCheck(stagenum);

        Player_StageSetting(tempList.StageStartPos, tempList.StageBoost, tempList.StageShoot, stagenum);
        gameUI.PlayerUI_Moved(tempList.StageBoost);
        gameUI.PlayerUI_Shoot(tempList.StageShoot);

        Player_StageOut_Setting(tempList.CamLimitX, tempList.CamLimitY);
        CameraManager.Instance.Stage_CameraLimit(tempList.CamLimitX, tempList.CamLimitY);
        CameraManager.Instance.Player_CameraActive();
    }

    private void UI_QuestCheck(int stageNum)        //퀘스트맵 확인
    {
        if (stageNum == 17 || stageNum == 24)
        {
            gameUI.PlayerUI_QuestActive(true);
            if (stageNum == 17)
                gameUI.PlayerUI_QuestNumber(0, " / 1");
            else if (stageNum == 24)
                gameUI.PlayerUI_QuestNumber(0, " / 3");
        }
        else
        {
            gameUI.PlayerUI_QuestActive(false);
        }
    }

    //========================================================UI화살표
    internal void UI_Indicator_Show(bool show, Vector2 goalpos)
    {
        gameUI.PlayerUI_Indicator(show, goalpos);
    }
    //========================================================UI화살표^^

    internal void UI_TriggerNextStage()//특수 스테이지 진입 시
    {
        gameUI.ClearUI_TriggerNextStage();
        gameUI.PlayerUI_Trigger(false); //UI 끄기
    }

    internal void UI_ReStage()//재시작 스테이지 버튼 누를 시 스테이지 재시작
    {
        StageDataList tempList  = new StageDataList();
        tempList                = StageManager.Instance.Stage_Current();

        StageManager.Instance.Stage_RefrashBtn();
        Player_StageRefrash(tempList.StageStartPos, tempList.StageBoost, tempList.StageShoot);
        gameUI.PlayerUI_Moved(tempList.StageBoost);
        gameUI.PlayerUI_Shoot(tempList.StageShoot);
        //gameUI.PauseUI_GameOver(false);
        gameUI.ClearUI_Active(false);
    }

    internal void UI_Refrash_Player()
    {
        player.Setting_Game_Start(true);
    }

    internal void UI_OptionSetting(float soundValue, KeyCode boostKey, KeyCode shootKey) //옵션 저장 시 작동 키 변경 및 사운드 조정
    {
        player.Setting_ControlKey(boostKey, shootKey);
        player.Setting_SoundValue(soundValue);
        SoundManager.Instant.PlayVolume(soundValue);
    }

    internal void UI_StageRefrash()//스테이지 번호 초기화
    {

    }

    internal void UI_StageDeactive()//스테이지 가리기
    {
        StageManager.Instance.Stage_DeActive();
    }

    internal void UI_TimeCheck()
    {

    }
    #endregion UI 관련

    #region 스테이지 관련

    internal bool Player_HP_Check()//플레이어의 HP가 0이하일 때
    {
        if (playerStatus.Heartpoint <= 0)
            return false;
        return true;
    }

    //골 말고 이벤트로 변경해야 할 것 같다.
    //플레이어가 스테이지를 클리어 했을 시
    internal bool Player_StageGoal()//플레이어가 Goal에 도착 시
    {
        bool isQuestClear = false;

        isQuestClear = Player_QuestCheck();

        /*if (playerStatus.Boost == 0 && isQuestClear == true)
        {*/
            player.Setting_Pause(false);
            gameUI.PlayerUI_Goal();
            Camera_ChaseDeactive();
            return true;
        /*}
        else//패배 조건
        {
            return false;
        }*/
    }

    internal void Player_NextStage()//플레이어 다음 스테이지 넘어갈 UI
    {
        //화면을 가리는 방식, 멈추는 방식, 껐다가 키는 방식, 완전 다른 이미지로 넘어가는 방식

    }

    internal bool Player_TriggerGoal()  //30스테이지 전용
    {
        if (playerStatus.Boost == 0)
        {
            player.Setting_Pause(false);
            gameUI.PlayerUI_Trigger(true); //UI 끄기
            StageManager.Instance.Stage_TriggerStart(); //30 스테이지 끄기

            Camera_ChaseDeactive();
            Camera_TriggerMove();
            triggerAni.gameObject.SetActive(true);
            triggerAni.SetTrigger("TriggerAni");

            return true;
        }
        else//패배 조건
        {
            return false;
        }
    }

    internal bool Player_FlowGoal() //30스테이지 이후 부드럽게 다음 스테이지로 넘어감
    {
        if (playerStatus.Boost == 0)
        {
            Camera_ChaseDeactive();
            UI_TriggerNextStage();
            return true;
        }
        else//패배 조건
        {
            return false;
        }
    }

    private bool Player_QuestCheck()//플레이어가 스테이지 퀘스트 클리어 시
    {
        switch (quest_StageCheck)
        {
            case 17:
                if (quest_ItemCheck != 1)
                    return false;
                break;
            case 24:
                if (quest_ItemCheck != 3)
                    return false;
                break;
        }
        return true;
    }

    internal void Setting_SpecielStage()  //이벤트 스테이지에 넘어갔을 시
    {
        Camera_Shake();
        if (quest_StageCheck < 35) return;
        Player_FlowGoal();
    }

    //===============================게임 엔딩
    internal void Player_PlayEnd()
    {
        if (playerStatus.Boost == 0)
        {
            player.Setting_Pause(false);
            gameUI.PlayerUI_Trigger(true); //UI 끄기
            StageManager.Instance.Stage_TriggerStart(); //스테이지 끄기

            Camera_ChaseDeactive();
            playEndAni.gameObject.SetActive(true);
            playEndAni.SetTrigger("playEndAni");
        }
    }

    internal void Player_EndScene()
    {
        gameUI.ClearUI_GameEnd();
        gameUI.PlayerUI_Trigger(false); //UI 끄기
    }

    //===============================게임 엔딩^^

    #endregion 스테이지 관련

    #region 카메라 관련
    private void Camera_ChaseActive()
    {
        CameraManager.Instance.Player_CameraActive();
    }

    private void Camera_ChaseDeactive()
    {
        CameraManager.Instance.Player_CameraDeactive();
    }

    private void Camera_TriggerMove()
    {
        CameraManager.Instance.Player_CameraTrigger();
    }

    private void Camera_GameEndMove()
    {
        CameraManager.Instance.Player_CameraGameEnd();
    }

    internal void Camera_Shake()
    {
        CameraManager.Instance.CameraShake();
    }

    internal void Camera_Reset()
    {
        CameraManager.Instance.Player_CameraDeactive();
        CameraManager.Instance.Player_CameraReset();
    }

    #endregion 카메라 관련
}
