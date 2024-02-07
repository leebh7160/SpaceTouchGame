using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActive : MonoBehaviour
{
    private void OnEnable()
    {
        Trigger_Play();
    }
    public void Trigger_Play()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance.UI_TriggerNextStage();
    }
}
