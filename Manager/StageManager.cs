using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
class StageDataList
{
    public GameObject Stage;              //���۵� ���������� �θ�
    public string StageName;
    public Vector2 StageStartPos;         //�÷��̾��� �������� ���� ��ġ
    [Header("-X�� ������,+X�� ������")]
    public Vector2 CamLimitX;            //ī�޶� �̵� ���� ���� X
    [Header("-Y�� ������,+Y�� ������")]
    public Vector2 CamLimitY;            //ī�޶� �̵� ���� ���� Y
    public int StageBoost;                //�÷��̾� ���� �ν���
    public int StageShoot;                //�÷��̾� ���� ���

    [Header("������ ���� �� ������ ����")]
    public List<int> StageEnemyNum;            //�� ������������ ���� ������ ����

    internal List<GameObject> StageEnemyObject;//���� ������Ʈ
    internal List<Vector2> StageObjectPosition; //�������� ��ü�� ��ġ
    internal List<float> StageObjectRotate;   //�������� ȸ�� ��ü�� ���� ȸ�� ����

}

public class StageManager : MonoBehaviour
{
    //===========================�̱���
    private static StageManager instance;
    public static StageManager Instance
    {
        get { return instance; }
    }

    //===========================����

    [SerializeField] private GameObject EnemyDeadBodyContainer;

    //===========================�������� ����Ʈ

    //https://unityindepth.tistory.com/56
    [SerializeField]  private List<StageDataList> stageDataLists = new List<StageDataList>();
    private int stageCount = 0;

    //===========================����Ƽ ����
    [SerializeField] private bool SaveData = false;


    private void Start()
    {
        instance = this;

        Setting_Stage();
    }

    #region �������� ���� ������
    private StageDataList Stage_ListSelect(int stage) //�������� ������ ��ȯ
    {
        StageDataList returnList = new StageDataList();

        returnList = stageDataLists[stage];
        return returnList;
    }

    private StageDataList Stage_SelectStage(int select) // ���� �������� ������ ��ȯ
    {
        Stage_DeActive();
        Stage_Refrash();
        if (stageCount < 0) return null;

        StageDataList returnList = new StageDataList();

        returnList = Stage_ListSelect(select);
        returnList.Stage.SetActive(true);
        stageCount = select;

        return returnList;
    }

    private StageDataList Stage_NextStage() //30�������� ���� ���� �������� ������ ��ȯ
    {
        if (stageCount < 0) return null;
        StageDataList returnList = new StageDataList();

        returnList = Stage_ListSelect(stageCount);
        returnList.Stage.SetActive(false);
        stageCount++;
        if (stageCount >= 41) stageCount = 41;
        returnList = Stage_ListSelect(stageCount);
        returnList.Stage.SetActive(true);
        return returnList;
    }
    internal void Stage_DeActive() //�������� ��ü ��Ȱ��
    {
        StartCoroutine(SettingStageDeActive());
    }

    #region ========================Ʈ���� ����
    internal void Stage_TriggerStart()
    {
        if (stageCount < 0) return;

        StageDataList returnList = new StageDataList();

        returnList = Stage_ListSelect(stageCount);
        returnList.Stage.SetActive(false);
    }

    internal void Stage_TriggerEnd()
    {
        if (stageCount < 0) return;

        StageDataList returnList = new StageDataList();
        stageCount++;
        returnList = Stage_ListSelect(stageCount);
        returnList.Stage.SetActive(true);
    }
    #endregion ========================Ʈ���� ����



    #endregion �������� ���� ������

    #region �������� ����
    internal void Stage_Refrash()//�ٸ� ������� �ʱ�ȭ
    {
        Setting_StageRefrash();
    }

    internal void Stage_RefrashBtn() //�ʱ�ȭ UI ��ư���� �ʱ�ȭ
    {
        Setting_StageRefrash();
    }

    private void Setting_Stage()//������Ʈ ���� �� ���� ����
    {
        int counttemp = 0;
        while (counttemp < stageDataLists.Count)
        {
            stageDataLists[counttemp].StageObjectPosition =
                StagePositionSetting(stageDataLists[counttemp].Stage);
            stageDataLists[counttemp].StageObjectRotate =
                StageRefrashSetting(stageDataLists[counttemp].Stage);
            counttemp++;
        }
    }

    private void Setting_StageRefrash()//�������� ����� �� ������ ��ȯ
    {
        StartCoroutine(StageRefraseCorutine());
    }
    #endregion �������� ����

    #region �������� �θ���
    internal StageDataList Stage_Select(int num) //���� �������� ������ ��ȯ
    {
        stageCount = num;
        StageDataList returnList = new StageDataList();
        returnList = Stage_SelectStage(num);
        return returnList;
    }

    internal StageDataList Stage_Current()  //���� �������� ������ ��ȯ
    {
        StageDataList returnList = new StageDataList();
        returnList = Stage_SelectStage(stageCount);
        return returnList;
    }

    internal void NextStage_Load() //���� �������� �ε� 30������������ �۵�
    {
        Stage_NextStage();
    }
    #endregion �������� �θ���

    #region �ڷ�ƾ

    private List<Vector2> StagePositionSetting(GameObject stage)    //�������� ��ġ ����
    {
        List<Vector2> returnList    = new List<Vector2>();
        int stageCount              = stage.transform.childCount;

        for(int i = 0; i < stageCount; i++)
        {
            returnList.Add(stage.transform.GetChild(i).transform.position);
        }
        return returnList;
    }

    private List<float> StageRefrashSetting(GameObject stage) //�������� ȸ�� ����
    {
        List<float> returnList = new List<float>();
        int stageCount = stage.transform.childCount;

        for (int i = 0; i < stageCount; i++)
        {
            returnList.Add(stage.transform.GetChild(i).rotation.eulerAngles.z);
        }
        return returnList;
    }

    private IEnumerator StageRefraseCorutine() //�������� ����� �ڷ�ƾ
    {
        if (stageCount >= 41) stageCount = 41;
        int stageChildCount = stageDataLists[stageCount].Stage.transform.childCount;

        StageRefrashEnemyBody();

        for (int i = 0; i < stageChildCount; i++)
        {            
            Transform tempObj = stageDataLists[stageCount].Stage.transform.GetChild(i);

            //�� ���� ���ֱ�
            if (tempObj.name == "FregmentParent")
            {
                StageRefrashChild(tempObj);
            }

            //���� �߰�
            if (tempObj.name == "EnemyParent")
            {
                StageEnemyChildSet(tempObj, stageCount);
            }

            tempObj.gameObject.SetActive(true);
            if (tempObj.name.StartsWith("NV") == false)
                tempObj.eulerAngles = new Vector3(0, 0, stageDataLists[stageCount].StageObjectRotate[i]);
            tempObj.position = stageDataLists[stageCount].StageObjectPosition[i];


            if (tempObj.GetComponent<Rigidbody2D>() != null)
            {
                tempObj.GetComponent<Rigidbody2D>().freezeRotation = true;
                yield return new WaitForSeconds(0.01f);
                tempObj.GetComponent<Rigidbody2D>().freezeRotation = false;
            }
        }

        Setting_Stage_Enemy(stageCount);
        Setting_Stage_EnemyStart();

        yield return null;
    }

    private void StageRefrashChild(Transform parentobj)//���� ���� �뵵
    {
        for(int i = 0; i < parentobj.childCount; i++)
        {
            parentobj.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void StageRefrashEnemyBody()//���� ��ü ���� �뵵
    {
        if (EnemyDeadBodyContainer == null)
            return;

        for(int i = 0; i < EnemyDeadBodyContainer.transform.childCount; i++)
        {
            Destroy(EnemyDeadBodyContainer.transform.GetChild(i).gameObject);
        }
    }

    private void Setting_Stage_Enemy(int stagenum)//�������� ���� �� ���� ������ ����
    {
        EnemyManager.Instance.Set_Enemy_ClearData();
        EnemyManager.Instance.Set_EnemyData(stageDataLists[stagenum].StageEnemyObject);
        EnemyManager.Instance.Set_Enemy_NumData(stageDataLists[stagenum].StageEnemyNum);
    }

    private void Setting_Stage_EnemyStart()
    {
        EnemyManager.Instance.Set_Enemy_Init();
        EnemyManager.Instance.Enemy_Play_Game();
    }


    private void StageEnemyChildSet(Transform parentobj, int stagecount)//���� �߰� �뵵
    {
        if (stageDataLists[stagecount].StageEnemyObject != null)
            stageDataLists[stagecount].StageEnemyObject.Clear();
        else
            stageDataLists[stagecount].StageEnemyObject = new List<GameObject>();

        for (int i = 0; i < parentobj.childCount; i++)
        {
            if (parentobj.GetChild(i).gameObject.name[0] == '=')
                continue;

            stageDataLists[stagecount].StageEnemyObject.Add(parentobj.GetChild(i).gameObject);
            parentobj.GetChild(i).gameObject.SetActive(false);
        }
    }

    private IEnumerator SettingStageDeActive()
    {
        for(int i = 0; i < stageDataLists.Count; i++)
        {
            stageDataLists[i].Stage.SetActive(false);
        }
        yield return null;
    }
    #endregion �ڷ�ƾ
}
