using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakShip : MonoBehaviour
{
    public void Break()
    {
        this.gameObject.GetComponent<Animator>().Rebind();
        this.gameObject.SetActive(false);
    }
}
