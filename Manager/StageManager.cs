using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

class StageDataList
{
    internal GameObject Stage;
    internal Vector2 StageStartPos;
    internal int StageBoost;
    internal int StageCamLimit;

    internal List<float> StageObjectRotate;
}

public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    public static StageManager Instance
    {
        get { return instance; }
    }


    //������ ���� �����غ��� Json�̴� �ڸ��̴� ����
    
    [SerializeField] private List<GameObject> stageObjectList;
    [SerializeField] private List<Vector2> StageStartList;
    [SerializeField] private List<int> StageBoost;
    [SerializeField] private List<int> StageCamLimit;

    [SerializeField] private List<StageDataList> stageList;

    private int stageNumber = 1;

    void Start()
    {
        instance = this;
        stageList = new List<StageDataList>();
        StageSetting();
    }

    internal void StageListSelect(int stagenum) //UI�� ����
    {
        stageNumber = stagenum;

        GameManager.Instance.PlayerNextStage();

        GameManager.Instance.MainStageUI();
    }

    public void PlayerNextStage() //�÷��̾ ���� �������� ��ư�� ������ ��
    {
        GameManager.Instance.PlayerNextStage();

        GameManager.Instance.NextStageUI(false);
    }

    #region ////////////////////////////////////////�������� ������ ����
    private void StageSetting()//��¥ �ٸ� ������� ������ �� �����غ��� ������Ÿ�Ը� �غ���
    {
        int counttemp = 0;
        while(counttemp < stageObjectList.Count)
        {
            StageDataList tempDataList = new StageDataList();
            tempDataList.Stage = stageObjectList[counttemp];
            tempDataList.StageStartPos = StageStartList[counttemp];
            tempDataList.StageBoost = StageBoost[counttemp];
            tempDataList.StageCamLimit = StageCamLimit[counttemp];


            tempDataList.StageObjectRotate = StageRefrashSetting(tempDataList.Stage);

            counttemp++;
            stageList.Add(tempDataList);
        }
    }

    private List<float> StageRefrashSetting(GameObject stage)
    {
        List<float> returnList = new List<float>();
        int stageCount = stage.transform.childCount;

        for(int i = 0; i < stageCount; i++)
        {
            returnList.Add(stage.transform.GetChild(i).rotation.eulerAngles.z);
        }
        return returnList;
    }
    #endregion ////////////////////////////////////////�������� ������ ����


    internal void StageNextLoad()//�������� �ε�
    {
        if (stageNumber >= 0)
            stageList[stageNumber].Stage.SetActive(false);

        stageNumber++;

        if (stageNumber >= stageList.Count) { Debug.Log("��� �������� �Ϸ�"); stageNumber = 0; }
        StartCoroutine(StageRefrash());
        stageList[stageNumber].Stage.SetActive(true);
    }

    internal void StageAllClear()
    {
        stageList[stageNumber].Stage.SetActive(false);
        stageNumber = 0;
    }

    internal void StageRefrashBtn()
    {
        GameManager.Instance.PlayerReSetting();
        StartCoroutine(StageRefrash());
    }

    private IEnumerator StageRefrash()//��� �����غ���
    {
        int stageCount = stageList[stageNumber].Stage.transform.childCount;

        for(int i = 0; i < stageCount; i++)
        {
            Transform tempObj = stageList[stageNumber].Stage.transform.GetChild(i);
            tempObj.gameObject.SetActive(true);
            tempObj.eulerAngles = new Vector3(0, 0, stageList[stageNumber].StageObjectRotate[i]);

            if (tempObj.GetComponent<Rigidbody2D>() != null)
            {
                tempObj.GetComponent<Rigidbody2D>().freezeRotation = true;
                yield return new WaitForSeconds(0.1f);
                tempObj.GetComponent<Rigidbody2D>().freezeRotation = false;
            }
        }

        yield return null;
    }


    #region ////////////////////////////////////////�������� ����
    internal StageDataList GetNextStageData()
    {
        return stageList[stageNumber];
    }
    #endregion ////////////////////////////////////////�������� ����


}
