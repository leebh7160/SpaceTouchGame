using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public void GameEnd_Play()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.Player_EndScene();
    }
}
