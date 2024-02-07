using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Bullet : MonoBehaviour
{
    private GameObject bustObject;

    private Rigidbody2D bulletRigid;
    private TrailRenderer bulletTrail;

    private Vector2 bulletVelocity;

    private float bulletSpeed = 80f;

    internal bool isactive = false;

    private void Start()
    {
        bustObject = this.transform.root.Find("AlienBulletEffect").Find("EnemyBulletEffect").gameObject;
    }

    private void OnEnable()
    {
        bulletRigid = this.gameObject.GetComponent<Rigidbody2D>();
        bulletTrail = this.gameObject.GetComponent<TrailRenderer>();
        bulletVelocity = bulletRigid.transform.up * bulletSpeed;
    }

    internal void A_Bullet_Velocity(Vector2 velocity)
    {
        bulletVelocity = velocity.normalized * bulletSpeed;
    }

    internal void A_Bullet_Active()
    {
        A_Bullet_Active(true);
        bulletRigid.simulated = true;
        bulletRigid.velocity = bulletVelocity;
    }

    private void A_Bullet_Deactive()
    {
        A_Bullet_Destroy();
    }

    public void A_Bullet_Bust()
    {
        if (bustObject != null)
        {
            var clone = Instantiate(bustObject, this.transform.position, Quaternion.identity);

            Destroy(clone, 0.5f);
        }
    }

    private void A_Bullet_Active(bool active)
    {
        isactive = active;
    }

    private void A_Bullet_Destroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Bullet_TriggerActive(collider);
    }

    private void Bullet_TriggerActive(Collider2D collider)
    {
        if (collider.gameObject.transform.parent == null)
            return;

        switch (collider.gameObject.transform.parent.tag)
        {
            case "HingeWall":
            case "StaticWall":
            case "GoalWall":
            case "OutWall":
            case "BrokenWall":
                A_Bullet_Bust();
                A_Bullet_Deactive();
                break;
            case "Player":
                A_Bullet_Bust();
                A_Bullet_Deactive();
                break;
        }
    }

    private void OnBecameInvisible()
    {
        if(isactive == true)
            A_Bullet_Destroy();
    }
}
