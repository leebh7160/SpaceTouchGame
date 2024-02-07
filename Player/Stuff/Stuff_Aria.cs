using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stuff_Aria : MonoBehaviour
{
    //�÷��̾��� �̵� ������ �����ϴ� �ڽ�
    private bool isDetact = false;

    internal bool DetactCheck()
    {
        return isDetact;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision.gameObject.layer == SortingLayer.NameToID("Wall")))
            isDetact = true;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!(collision.gameObject.layer == SortingLayer.NameToID("Wall")))
            isDetact = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!(collision.gameObject.layer == SortingLayer.NameToID("Wall")))
            isDetact = false;
    }
}
