using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
class EnemyData 
{
    private List<GameObject> enemylist;
    private List<int> enemynumlist;

    public EnemyData()
    {
        enemylist       = new List<GameObject>();
        enemynumlist    = new List<int>();
    }

    //====================================적군 오브젝트
    internal List<GameObject> Get_EnemyData()
    {
        return enemylist;
    }

    internal void Set_EnemyData(List <GameObject> _enemylist) //적군 리스트 데이터 세팅
    {
        enemylist.AddRange(_enemylist);
    }

    internal void Reset_EnemyData() //적군 리스트 데이터 클리어
    {
        if (enemylist != null)
            enemylist.Clear();
    }

    //====================================적군 숫자
    internal List<int> Get_EnemyNumData() //적군 개인 데이터 반환
    {
        return enemynumlist;
    }

    internal void Set_EnemyNumData(List<int> _enemynumlist) //적군 개인 데이터 세팅
    {
        enemynumlist.AddRange(_enemynumlist);
    }

    internal void Reset_EnemyNumData() //적군 개인 데이터 클리어
    {
        if (enemynumlist != null)
            enemynumlist.Clear();
    }

}

//=====================================================================
public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    public static EnemyManager Instance { get { return instance; } }

    //======================참조

    private EnemyData enemydata;

    //======================변수

    private bool isstart = false;
    private int stage = 0;

    void Start()
    {
        instance = this;
        enemydata = new EnemyData();
    }

    #region 적군 생성하는 곳

    private void Enemy_Create()//적군 생성
    {
        //해당 라운드까지의 적군 수
        int enemycount = Enemy_Create_Count();
        //지금까지 죽었던 적군 수를 라운드 수까지 셈
        int enemydeadcount = enemycount - enemydata.Get_EnemyNumData()[stage];  

        for(int i = enemydeadcount; i < enemycount; i++)
        {
            enemydata.Get_EnemyData()[i].GetComponent<EnemyNormal>().Enemy_Create();
        }
    }

    private int Enemy_Create_Count()//해당 라운드까지의 적군 수
    {
        int enemycount = 0;

        for (int i = 0; i <= stage; i++)
        {
            enemycount += enemydata.Get_EnemyNumData()[i];
        }

        return enemycount;
    }

    #endregion

    #region 시간에 따라 실행, 적군 체크하는 부분

    //시작하면 시간을 돌리고 적군을 수 만큼 소환한다
    //소환하고 나서 적이 죽는 것을 지속적으로 확인한다.
    //스테이지 시작할 때마다 실행되는 부분
    internal void Enemy_Play_Game()
    {
        if(Enemy_Play_EndCheck() == true)
            return;

        int enemycount = Enemy_Create_Count();

        if (stage == 0 && isstart == false)
        {
            isstart = true;
            Enemy_Create();
        }
        else
            StartCoroutine(Enemy_Play_Check(enemycount, enemydata.Get_EnemyData()));
    }

    private bool Enemy_Play_EndCheck()//스테이지 적은 다 사라짐 확인(임시로 변경)
    {
        if (stage >= enemydata.Get_EnemyNumData().Count)
        {
            //다음 스테이지로 변경()
            
            GameManager.Instance.Player_StageGoal();
            //GameManager.Instance.Player_GameEnd();
            return true;
        }
        return false;
    }

    //적군이 죽는 것을 지속적으로 확인하는 곳
    private IEnumerator Enemy_Play_Check(int enemynum, List<GameObject> enemylist)
    {
        int num = enemynum;
        int check = 0;
        List<GameObject> list = enemylist;

        while (true)
        {
            if (check >= num)
                break;

            if (list[check].activeInHierarchy == true)
            {
                check = 0;
                break;
            }
            else
                check++;
        }

        if (check >= num)
        {
            stage++;
            if (Enemy_Play_EndCheck() == true)
                yield return null;
            else
                Enemy_Create();
        }

        yield return null;
    }

    #endregion

    #region 적군 데이터 받는 부분
    internal void Set_EnemyData(List<GameObject> enemylist) //데이터를 StageManager로부터 받음
    {
        enemydata.Set_EnemyData(enemylist);
    }

    internal void Set_Enemy_NumData(List<int> enemynumlist) //적군 숫자를 StageManager로부터 받음
    {
        enemydata.Set_EnemyNumData(enemynumlist);
    }

    internal void Set_Enemy_ClearData()//적군 초기화
    {
        enemydata.Reset_EnemyData();
        enemydata.Reset_EnemyNumData();
    }

    internal void Set_Enemy_Init()
    {
        isstart = false;
        stage   = 0;
    }
    #endregion


}
