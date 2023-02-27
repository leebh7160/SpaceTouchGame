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


    //데이터 저장 생각해보기 Json이던 자멜이던 뭐던
    
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

    internal void StageListSelect(int stagenum) //UI로 선택
    {
        stageNumber = stagenum;

        GameManager.Instance.PlayerNextStage();

        GameManager.Instance.MainStageUI();
    }

    public void PlayerNextStage() //플레이어가 다음 스테이지 버튼을 눌렀을 때
    {
        GameManager.Instance.PlayerNextStage();

        GameManager.Instance.NextStageUI(false);
    }

    #region ////////////////////////////////////////스테이지 데이터 지정
    private void StageSetting()//진짜 다른 방식으로 저장할 것 생각해봐봐 프로토타입만 해보고
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
    #endregion ////////////////////////////////////////스테이지 데이터 지정


    internal void StageNextLoad()//스테이지 로드
    {
        if (stageNumber >= 0)
            stageList[stageNumber].Stage.SetActive(false);

        stageNumber++;

        if (stageNumber >= stageList.Count) { Debug.Log("모든 스테이지 완료"); stageNumber = 0; }
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

    private IEnumerator StageRefrash()//재귀 생각해보기
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


    #region ////////////////////////////////////////스테이지 관리
    internal StageDataList GetNextStageData()
    {
        return stageList[stageNumber];
    }
    #endregion ////////////////////////////////////////스테이지 관리


}
