using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerMove_tmp;
    

    #region //////////////////////////////////�÷��̾� �������� ����

    internal void PlayUI_Moved(int movecount)
    {
        if (movecount < 0)
        {
            movecount = 0; Debug.Log("�ּ� ���� �ʰ�");
            return;
        }

        playerMove_tmp.text = movecount.ToString();
    }//�÷��̾ �������� �� UI�۵�
    internal void PlayUI_Boost(int boost)
    {
        playerMove_tmp.text = boost.ToString();
    }//�÷��̾��� boost����


    public void StageSelectButton() //�������� ��ư ������ ��
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        int num = int.Parse(buttonName.Substring(8));
        StageManager.Instance.StageListSelect(num - 2);

        MainStageUI(false);
    }

    public void ButtonClick_Main()//ó�� ȭ������ ���ư���
    {
        NextStageUI(false);
        MainStageUI(true);
        StageManager.Instance.StageAllClear();
    }

    public void ButtonClick_NextStage() //���� �������� ������ ��
    {
        StageManager.Instance.PlayerNextStage();
    }

    public void ButtonClick_Refrash() //��������
    {
        StageManager.Instance.StageRefrashBtn();
    }

    //=========================================== Fade �ڷ�ƾ
    internal void NextStageUI(bool switchCheck) //ȭ�� ���̵� ��, UI ����
    {
        StartCoroutine(FadeStage(switchCheck));
    }

    internal void MainStageUI(bool switchCheck)
    {
        StartCoroutine(FadeMain(switchCheck));
    }//�÷��� ���� UI����


    //���̵� ��� ���� �������ϱ�
    private IEnumerator FadeMain(bool switchCheck)
    {
        GameObject main;
        GameObject mainBG;

        main = this.transform.Find("Main").gameObject;
        mainBG = this.transform.Find("PadeGrapic").gameObject;

        if (switchCheck == false)
        {
            main.SetActive(false);
            mainBG.SetActive(false);
        }
        else
        {
            main.SetActive(true);
            mainBG.SetActive(true);
        }
        yield return null;
    }

    private IEnumerator FadeStage(bool switchCheck)
    {
        GameObject stage;

        stage = this.transform.Find("ClearUI").gameObject;

        if (switchCheck == false)
        {
            stage.SetActive(false);
        }
        else
        {
            stage.SetActive(true);
        }
        yield return null;
    }
    #endregion //////////////////////////////////�÷��̾� �������� ����

}
