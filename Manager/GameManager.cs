using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

enum PlayBehavior
{
    boost, shoot, itemboost, itemshoot, Hit
}

#region �ػ� ����
public class ResolutionClass //�ػ� ����
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
    //==========================�̱���
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    //==========================����
    private GameUI gameUI;
    private ResolutionClass resolutionClass;
    private Player player;
    private PlayerStatus playerStatus;
    private int playerHP = 3;
    //==========================����^^

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


    #region �ػ�
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
    #endregion �ػ�

    #region ����
    private void PlayerSetting()
    {
        player                  = GameObject.Find("Player").GetComponent<Player>();
        gameUI                  = GameObject.Find("GamePlayUI").GetComponent<GameUI>();
        playerStatus            = new PlayerStatus();
        playerStatus.Heartpoint = playerHP;
    }

    #endregion ����

    #region �÷��̾� ����

    internal GameObject Get_Enemy_PlayerPos() //�Ѿ˿� �¾��� �� ������ ������ �÷��̾��� ������
    {
        if (player != null)
            return player.gameObject;
        return null;
    }

    internal bool Player_MovedUI()//�÷��̾ �̵� ��
    {
        if (playerStatus.Boost <= 0) return false;

        playerStatus.Boost--;

        Player_Behavior(PlayBehavior.boost);

        return true;
    }

    internal void Player_Moved_Pause(bool moved)//�Ͻ����� ����
    {
        gameUI.PauseUI_Interactable(moved);
    }

    internal bool Player_ShootUI()//�÷��̾ ��� ��
    {
        if (playerStatus.Shoot <= 0) return false;


        playerStatus.Shoot--;

        Player_Behavior(PlayBehavior.shoot);
        return true;
    }

    internal void Player_GameEnd()//�÷��̾� ���� ����
    {
        if (playerStatus.Boost <= 0)
            player.Setting_GameEnd();
    }
   

    internal void Player_GetItem(string itemname, int value)//�÷��̾ ������ ȹ�� ��
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

    private void Player_Behavior(PlayBehavior behavior)//�÷��̾ �ൿ �� �������ͽ� Ȯ��
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

    private void Player_StageSetting(Vector2 startpos, int boost, int shoot, int stagenum) //�÷��̾� �������� ����
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

    private void Player_StageRefrash(Vector2 refrashpos, int boost, int shoot) //�÷��̾� ��������
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

    internal void Player_Pause()          //�Ͻ�����
    {
        player.Setting_Pause(false);
        player.Setting_Game_Start(false);
        Debug.Log("Pause : ");
    }

    internal void Player_Move_Slowdown(float time)
    {
        Time.timeScale = time;
    }

    internal void Player_OutOfAria()                //�÷��̾� ȭ�� ����
    {
        //gameUI.PlayerUI_Replay();
    }

    internal void Player_GameOver()                 //�÷��̾� ���� ����
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


    internal void Player_QuestEngage() //����Ʈ ������ ���� ��
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

    #endregion �÷��̾� ����

    #region UI ����
    internal void UI_MainStage() //���� ���� �������� UI ����
    { }

    internal void UI_SelectStage(int stagenum) //���� ���� �������� UI ����
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

    private void UI_QuestCheck(int stageNum)        //����Ʈ�� Ȯ��
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

    //========================================================UIȭ��ǥ
    internal void UI_Indicator_Show(bool show, Vector2 goalpos)
    {
        gameUI.PlayerUI_Indicator(show, goalpos);
    }
    //========================================================UIȭ��ǥ^^

    internal void UI_TriggerNextStage()//Ư�� �������� ���� ��
    {
        gameUI.ClearUI_TriggerNextStage();
        gameUI.PlayerUI_Trigger(false); //UI ����
    }

    internal void UI_ReStage()//����� �������� ��ư ���� �� �������� �����
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

    internal void UI_OptionSetting(float soundValue, KeyCode boostKey, KeyCode shootKey) //�ɼ� ���� �� �۵� Ű ���� �� ���� ����
    {
        player.Setting_ControlKey(boostKey, shootKey);
        player.Setting_SoundValue(soundValue);
        SoundManager.Instant.PlayVolume(soundValue);
    }

    internal void UI_StageRefrash()//�������� ��ȣ �ʱ�ȭ
    {

    }

    internal void UI_StageDeactive()//�������� ������
    {
        StageManager.Instance.Stage_DeActive();
    }

    internal void UI_TimeCheck()
    {

    }
    #endregion UI ����

    #region �������� ����

    internal bool Player_HP_Check()//�÷��̾��� HP�� 0������ ��
    {
        if (playerStatus.Heartpoint <= 0)
            return false;
        return true;
    }

    //�� ���� �̺�Ʈ�� �����ؾ� �� �� ����.
    //�÷��̾ ���������� Ŭ���� ���� ��
    internal bool Player_StageGoal()//�÷��̾ Goal�� ���� ��
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
        else//�й� ����
        {
            return false;
        }*/
    }

    internal void Player_NextStage()//�÷��̾� ���� �������� �Ѿ UI
    {
        //ȭ���� ������ ���, ���ߴ� ���, ���ٰ� Ű�� ���, ���� �ٸ� �̹����� �Ѿ�� ���

    }

    internal bool Player_TriggerGoal()  //30�������� ����
    {
        if (playerStatus.Boost == 0)
        {
            player.Setting_Pause(false);
            gameUI.PlayerUI_Trigger(true); //UI ����
            StageManager.Instance.Stage_TriggerStart(); //30 �������� ����

            Camera_ChaseDeactive();
            Camera_TriggerMove();
            triggerAni.gameObject.SetActive(true);
            triggerAni.SetTrigger("TriggerAni");

            return true;
        }
        else//�й� ����
        {
            return false;
        }
    }

    internal bool Player_FlowGoal() //30�������� ���� �ε巴�� ���� ���������� �Ѿ
    {
        if (playerStatus.Boost == 0)
        {
            Camera_ChaseDeactive();
            UI_TriggerNextStage();
            return true;
        }
        else//�й� ����
        {
            return false;
        }
    }

    private bool Player_QuestCheck()//�÷��̾ �������� ����Ʈ Ŭ���� ��
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

    internal void Setting_SpecielStage()  //�̺�Ʈ ���������� �Ѿ�� ��
    {
        Camera_Shake();
        if (quest_StageCheck < 35) return;
        Player_FlowGoal();
    }

    //===============================���� ����
    internal void Player_PlayEnd()
    {
        if (playerStatus.Boost == 0)
        {
            player.Setting_Pause(false);
            gameUI.PlayerUI_Trigger(true); //UI ����
            StageManager.Instance.Stage_TriggerStart(); //�������� ����

            Camera_ChaseDeactive();
            playEndAni.gameObject.SetActive(true);
            playEndAni.SetTrigger("playEndAni");
        }
    }

    internal void Player_EndScene()
    {
        gameUI.ClearUI_GameEnd();
        gameUI.PlayerUI_Trigger(false); //UI ����
    }

    //===============================���� ����^^

    #endregion �������� ����

    #region ī�޶� ����
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

    #endregion ī�޶� ����
}
