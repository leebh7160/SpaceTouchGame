using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//goal�� ���� �� �ִٰ� �˷���
//goal�� ī�޶� �ۿ� �ִٰ� �˷���
//goal�� ī�޶� �ȿ� �ִٰ� �˷���

public class GoalIndicator : MonoBehaviour
{
    //ã���־����
    //�׳� �̱����� Ÿ�� ���� ���� ���� ������
    internal void Indicator_Show(bool show)
    {
        if (Application.isPlaying == true && GameManager.Instance != null)
            GameManager.Instance.UI_Indicator_Show(show, this.transform.position);
    }
    /*private void OnEnable()
    {
        Indicator_Show(true);
        Debug.Log("�� Ȱ��ȭ ��");
    }*/
    //ȭ�鿡 ������ ���� �� ����
    //�̹����� ��Ÿ������
    private void OnBecameInvisible()
    {
        Indicator_Show(true);
    }

    //ȭ�鿡 ������ �� ����
    //�̹����� ����������
    private void OnBecameVisible()
    {
        Indicator_Show(false);
    }
}
