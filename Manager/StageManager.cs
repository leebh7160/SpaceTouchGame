using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[Serializable]
class StageDataList
{
    public GameObject Stage;              //제작된 스테이지의 부모
    public string StageName;
    public Vector2 StageStartPos;         //플레이어의 스테이지 시작 위치
    [Header("-X축 고정값,+X축 고정값")]
    public Vector2 CamLimitX;            //카메라 이동 길이 제한 X
    [Header("-Y축 고정값,+Y축 고정값")]
    public Vector2 CamLimitY;            //카메라 이동 길이 제한 Y
    public int StageBoost;                //플레이어 시작 부스터
    public int StageShoot;                //플레이어 시작 사격

    [Header("각각의 라운드 당 생성할 적군")]
    public List<int> StageEnemyNum;            //한 스테이지에서 나올 적군의 숫자

    internal List<GameObject> StageEnemyObject;//적군 오브젝트
    internal List<Vector2> StageObjectPosition; //스테이지 객체의 위치
    internal List<float> StageObjectRotate;   //스테이지 회전 객체의 시작 회전 각도

}

public class StageManager : MonoBehaviour
{
    //===========================싱글톤
    private static StageManager instance;
    public static StageManager Instance
    {
        get { return instance; }
    }

    //===========================참조

    [SerializeField] private GameObject EnemyDeadBodyContainer;

    //===========================스테이지 리스트

    //https://unityindepth.tistory.com/56
    [SerializeField]  private List<StageDataList> stageDataLists = new List<StageDataList>();
    private int stageCount = 0;

    //===========================유니티 조작
    [SerializeField] private bool SaveData = false;


    private void Start()
    {
        instance = this;

        Setting_Stage();
    }

    #region 스테이지 관련 데이터
    private StageDataList Stage_ListSelect(int stage) //스테이지 데이터 반환
    {
        StageDataList returnList = new StageDataList();

        returnList = stageDataLists[stage];
        return returnList;
    }

    private StageDataList Stage_SelectStage(int select) // 선택 스테이지 데이터 반환
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

    private StageDataList Stage_NextStage() //30스테이지 이후 다음 스테이지 데이터 반환
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
    internal void Stage_DeActive() //스테이지 전체 비활성
    {
        StartCoroutine(SettingStageDeActive());
    }

    #region ========================트리거 실행
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
    #endregion ========================트리거 실행



    #endregion 스테이지 관련 데이터

    #region 스테이지 세팅
    internal void Stage_Refrash()//다른 방식으로 초기화
    {
        Setting_StageRefrash();
    }

    internal void Stage_RefrashBtn() //초기화 UI 버튼으로 초기화
    {
        Setting_StageRefrash();
    }

    private void Setting_Stage()//프로젝트 실행 시 설정 저장
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

    private void Setting_StageRefrash()//스테이지 재시작 시 데이터 반환
    {
        StartCoroutine(StageRefraseCorutine());
    }
    #endregion 스테이지 세팅

    #region 스테이지 부르기
    internal StageDataList Stage_Select(int num) //선택 스테이지 데이터 반환
    {
        stageCount = num;
        StageDataList returnList = new StageDataList();
        returnList = Stage_SelectStage(num);
        return returnList;
    }

    internal StageDataList Stage_Current()  //현재 스테이지 데이터 반환
    {
        StageDataList returnList = new StageDataList();
        returnList = Stage_SelectStage(stageCount);
        return returnList;
    }

    internal void NextStage_Load() //다음 스테이지 로드 30스테이지부터 작동
    {
        Stage_NextStage();
    }
    #endregion 스테이지 부르기

    #region 코루틴

    private List<Vector2> StagePositionSetting(GameObject stage)    //스테이지 위치 세팅
    {
        List<Vector2> returnList    = new List<Vector2>();
        int stageCount              = stage.transform.childCount;

        for(int i = 0; i < stageCount; i++)
        {
            returnList.Add(stage.transform.GetChild(i).transform.position);
        }
        return returnList;
    }

    private List<float> StageRefrashSetting(GameObject stage) //스테이지 회전 세팅
    {
        List<float> returnList = new List<float>();
        int stageCount = stage.transform.childCount;

        for (int i = 0; i < stageCount; i++)
        {
            returnList.Add(stage.transform.GetChild(i).rotation.eulerAngles.z);
        }
        return returnList;
    }

    private IEnumerator StageRefraseCorutine() //스테이지 재시작 코루틴
    {
        if (stageCount >= 41) stageCount = 41;
        int stageChildCount = stageDataLists[stageCount].Stage.transform.childCount;

        StageRefrashEnemyBody();

        for (int i = 0; i < stageChildCount; i++)
        {            
            Transform tempObj = stageDataLists[stageCount].Stage.transform.GetChild(i);

            //벽 파편 없애기
            if (tempObj.name == "FregmentParent")
            {
                StageRefrashChild(tempObj);
            }

            //적군 추가
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

    private void StageRefrashChild(Transform parentobj)//파편 제거 용도
    {
        for(int i = 0; i < parentobj.childCount; i++)
        {
            parentobj.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void StageRefrashEnemyBody()//적군 시체 제거 용도
    {
        if (EnemyDeadBodyContainer == null)
            return;

        for(int i = 0; i < EnemyDeadBodyContainer.transform.childCount; i++)
        {
            Destroy(EnemyDeadBodyContainer.transform.GetChild(i).gameObject);
        }
    }

    private void Setting_Stage_Enemy(int stagenum)//스테이지 실행 시 적군 데이터 전송
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


    private void StageEnemyChildSet(Transform parentobj, int stagecount)//적군 추가 용도
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
    #endregion 코루틴
}
