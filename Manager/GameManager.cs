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

    GameUI gameUI;//���� UI�� �Ϲ�ȭ�� �ʿ䰡 �ִ°�? �̱��� ���� ������?
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
    #region////////////////////////////////�ػ� ����
    private void ResolutionSetting()
    {
        int setWidth = 2560;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);
        //���̸� �����ϰ�, �ػ󵵰� ������ ���̿� �ʺ� ����� ���̷� ���ϸ�
        //1680/960 * 1920 = 3360

        if((float)setWidth / setHeight < (float)deviceWidth / deviceHeight) //������ ���� �ػ󵵸� �����
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
    #endregion////////////////////////////////�ػ� ����

    #region ///////////////////////////////////////UI����
    internal void MainStageUI()
    {
        gameUI.MainStageUI(true);
    }
    internal void NextStageUI(bool switchCheck)
    {
        gameUI.NextStageUI(switchCheck);
    }
    #endregion ///////////////////////////////////////UI����

    #region ///////////////////////////////////////�÷��̾� ����

    internal void PlayerMoved(int movecount)//�÷��̾ �̵� ��
    {
        gameUI.PlayUI_Moved(movecount);
    }

    internal void PlayerGetItem(int movecount)//�÷��̾ ������ ȹ�� ��
    {
        gameUI.PlayUI_Moved(movecount);
        //����Ʈ����� ����
    }

    internal void PlayerGoalIn() //�÷��̾ goal�� �������� ��
    {
        gameUI.NextStageUI(true);

        StopCoroutine(CameraManager.Instance.CameraChase());
    }

    internal void PlayerNextStage() //�÷��̾ ���� �������� ��ư�� ������ ��
    {
        int stageboost = 0;
        float stageCamLimit = 0;
        Vector2 stage_StartPos = new Vector2();
        StageDataList stage_DataList = new StageDataList();

        gameUI.NextStageUI(false);//�������� UI���� (�ε带 �����)

        StageManager.Instance.StageNextLoad();//���� �������� �ε�

        stage_DataList  = StageManager.Instance.GetNextStageData();//�ε� �� �������� ���� ������ �ޱ�
        stageboost      = stage_DataList.StageBoost;
        stage_StartPos  = stage_DataList.StageStartPos;
        stageCamLimit   = stage_DataList.StageCamLimit;


        player.PlayerNextStatSetting(stageboost, stage_StartPos);//�÷��̾ �ʿ��� ������ �ֱ�

        gameUI.PlayUI_Boost(stageboost);//�÷��̾� ������ UI����

        CameraManager.Instance.Stage_CameraLimit(stageCamLimit);//ī�޶� ���� �Ѱ���� �ֱ�
                    
        StartCoroutine(CameraManager.Instance.CameraChase());//ī�޶� ���� �۵�

    }

    internal void PlayerReSetting()
    {
        int stageboost = 0;
        float stageCamLimit = 0;
        Vector2 stage_StartPos = new Vector2();
        StageDataList stage_DataList = new StageDataList();

        stage_DataList = StageManager.Instance.GetNextStageData();//�ε� �� �������� ���� ������ �ޱ�
        stageboost = stage_DataList.StageBoost;
        stage_StartPos = stage_DataList.StageStartPos;
        stageCamLimit = stage_DataList.StageCamLimit;

        player.PlayerRefrashSetting(stageboost, stage_StartPos);//�÷��̾ �ʿ��� ������ �ֱ�
    }
    #endregion ///////////////////////////////////////�÷��̾� ����

}
