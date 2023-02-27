using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerMove_tmp;
    

    #region //////////////////////////////////플레이어 스테이지 관련

    internal void PlayUI_Moved(int movecount)
    {
        if (movecount < 0)
        {
            movecount = 0; Debug.Log("최소 숫자 초과");
            return;
        }

        playerMove_tmp.text = movecount.ToString();
    }//플레이어가 움직였을 때 UI작동
    internal void PlayUI_Boost(int boost)
    {
        playerMove_tmp.text = boost.ToString();
    }//플레이어의 boost갱신


    public void StageSelectButton() //스테이지 버튼 눌렀을 때
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;

        int num = int.Parse(buttonName.Substring(8));
        StageManager.Instance.StageListSelect(num - 2);

        MainStageUI(false);
    }

    public void ButtonClick_Main()//처음 화면으로 돌아가기
    {
        NextStageUI(false);
        MainStageUI(true);
        StageManager.Instance.StageAllClear();
    }

    public void ButtonClick_NextStage() //다음 스테이지 눌렀을 때
    {
        StageManager.Instance.PlayerNextStage();
    }

    public void ButtonClick_Refrash() //리프레시
    {
        StageManager.Instance.StageRefrashBtn();
    }

    //=========================================== Fade 코루틴
    internal void NextStageUI(bool switchCheck) //화면 페이드 온, UI 실행
    {
        StartCoroutine(FadeStage(switchCheck));
    }

    internal void MainStageUI(bool switchCheck)
    {
        StartCoroutine(FadeMain(switchCheck));
    }//플레이 메인 UI실행


    //페이드 기능 만들어서 공용사용하기
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
    #endregion //////////////////////////////////플레이어 스테이지 관련

}
