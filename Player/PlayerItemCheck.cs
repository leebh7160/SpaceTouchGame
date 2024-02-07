using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCheck : MonoBehaviour
{
    [SerializeField]
    private Player player;

    private void Start()
    {
        player = this.gameObject.transform.parent.GetComponent<Player>();
    }

    public void OnTriggerEnter2D(Collider2D collision)//플레이어 대신 아이템을 쳐줌
    {
        if (collision.gameObject.tag == "Bomb")
            collision.transform.parent.GetComponent<Bomb>().Bomb_EffectOn();
        if (collision.gameObject.tag == "EnmeyBullet")
            Debug.Log("적 탄약 타격 체력 변경하기");
        else if (collision.gameObject.tag == "normalBullet" || collision.gameObject.tag == "bounceBullet")
            collision.gameObject.SetActive(false);
        else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Untagged")
            return;

        player.TriggerEnter2D(collision.gameObject.tag);
    }
}
