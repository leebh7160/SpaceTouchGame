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

    //====================================���� ������Ʈ
    internal List<GameObject> Get_EnemyData()
    {
        return enemylist;
    }

    internal void Set_EnemyData(List <GameObject> _enemylist) //���� ����Ʈ ������ ����
    {
        enemylist.AddRange(_enemylist);
    }

    internal void Reset_EnemyData() //���� ����Ʈ ������ Ŭ����
    {
        if (enemylist != null)
            enemylist.Clear();
    }

    //====================================���� ����
    internal List<int> Get_EnemyNumData() //���� ���� ������ ��ȯ
    {
        return enemynumlist;
    }

    internal void Set_EnemyNumData(List<int> _enemynumlist) //���� ���� ������ ����
    {
        enemynumlist.AddRange(_enemynumlist);
    }

    internal void Reset_EnemyNumData() //���� ���� ������ Ŭ����
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

    //======================����

    private EnemyData enemydata;

    //======================����

    private bool isstart = false;
    private int stage = 0;

    void Start()
    {
        instance = this;
        enemydata = new EnemyData();
    }

    #region ���� �����ϴ� ��

    private void Enemy_Create()//���� ����
    {
        //�ش� ��������� ���� ��
        int enemycount = Enemy_Create_Count();
        //���ݱ��� �׾��� ���� ���� ���� ������ ��
        int enemydeadcount = enemycount - enemydata.Get_EnemyNumData()[stage];  

        for(int i = enemydeadcount; i < enemycount; i++)
        {
            enemydata.Get_EnemyData()[i].GetComponent<EnemyNormal>().Enemy_Create();
        }
    }

    private int Enemy_Create_Count()//�ش� ��������� ���� ��
    {
        int enemycount = 0;

        for (int i = 0; i <= stage; i++)
        {
            enemycount += enemydata.Get_EnemyNumData()[i];
        }

        return enemycount;
    }

    #endregion

    #region �ð��� ���� ����, ���� üũ�ϴ� �κ�

    //�����ϸ� �ð��� ������ ������ �� ��ŭ ��ȯ�Ѵ�
    //��ȯ�ϰ� ���� ���� �״� ���� ���������� Ȯ���Ѵ�.
    //�������� ������ ������ ����Ǵ� �κ�
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

    private bool Enemy_Play_EndCheck()//�������� ���� �� ����� Ȯ��(�ӽ÷� ����)
    {
        if (stage >= enemydata.Get_EnemyNumData().Count)
        {
            //���� ���������� ����()
            
            GameManager.Instance.Player_StageGoal();
            //GameManager.Instance.Player_GameEnd();
            return true;
        }
        return false;
    }

    //������ �״� ���� ���������� Ȯ���ϴ� ��
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

    #region ���� ������ �޴� �κ�
    internal void Set_EnemyData(List<GameObject> enemylist) //�����͸� StageManager�κ��� ����
    {
        enemydata.Set_EnemyData(enemylist);
    }

    internal void Set_Enemy_NumData(List<int> enemynumlist) //���� ���ڸ� StageManager�κ��� ����
    {
        enemydata.Set_EnemyNumData(enemynumlist);
    }

    internal void Set_Enemy_ClearData()//���� �ʱ�ȭ
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
