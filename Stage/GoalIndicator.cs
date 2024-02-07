using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//goal이 있을 때 있다고 알려줌
//goal이 카메라 밖에 있다고 알려줌
//goal이 카메라 안에 있다고 알려줌

public class GoalIndicator : MonoBehaviour
{
    //찾아주어야함
    //그냥 싱글톤을 타고 가는 것이 낫지 않을까
    internal void Indicator_Show(bool show)
    {
        if (Application.isPlaying == true && GameManager.Instance != null)
            GameManager.Instance.UI_Indicator_Show(show, this.transform.position);
    }
    /*private void OnEnable()
    {
        Indicator_Show(true);
        Debug.Log("골 활성화 됨");
    }*/
    //화면에 보이지 않을 때 실행
    //이미지가 나타나야함
    private void OnBecameInvisible()
    {
        Indicator_Show(true);
    }

    //화면에 보였을 때 실행
    //이미지가 없어져야함
    private void OnBecameVisible()
    {
        Indicator_Show(false);
    }
}
