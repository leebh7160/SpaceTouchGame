using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public bool active = false;
    private GameObject imageObj;
    private SpriteRenderer effectRender;
    private Animator EffectOn; //¿Ã∆Â∆Æ Ω««‡
    private AudioSource bombAudio;

    private void Start()
    {
        //EffectOn = this.transform.GetChild(0).GetComponent<Animator>();
        EffectOn        = this.transform.GetComponent<Animator>();
        imageObj        = this.transform.GetChild(0).gameObject;
        effectRender    = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        bombAudio       = this.transform.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (EffectOn == null)
            return;

        this.transform.localScale = new Vector3(2.5f, 2.5f);
        imageObj.SetActive(true);
        active = false;
    }

    public void BombDeactive()
    {
        this.gameObject.SetActive(false);
    }

    internal void Bomb_EffectOn()
    {
        EffectOn.SetTrigger("Bomb_Effect");
        imageObj.SetActive(false);
        bombAudio.Play();
        if (active == true)
        {
            BombDeactive();
        }
    }
}
