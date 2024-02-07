using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BULLET_KIND
{
    Normal = 0, Bounce = 1,
}

public class Bullet : MonoBehaviour, IBullet
{
    //=========================
    private BULLET_KIND bullet_kind;

    private NormalBullet normalbullet;
    private BounceBullet bouncebullet;
    //=========================

    private Rigidbody2D bulletRigid;
    private TrailRenderer bulletTrail;
    private GameObject bustObject;
    private ParticleSystem bustEffect;
    private CircleCollider2D circleCollider;

    private Vector2 bulletVelocty;
    private Vector2 bulletRefrashPosition;

    private float bulletSpeed = 100f;

    internal bool bulletMove = false;

    private int bulletBounce = 0;

    void Awake()
    {
        bullet_kind  = BULLET_KIND.Normal;
        normalbullet = new NormalBullet();
        bouncebullet = new BounceBullet();
        bulletRigid  = this.gameObject.GetComponent<Rigidbody2D>();
        bulletTrail  = this.gameObject.GetComponent<TrailRenderer>();
        bulletVelocty = bulletRigid.transform.up * bulletSpeed;
        bulletRefrashPosition = new Vector2(-1500, 0);
        circleCollider = this.gameObject.GetComponent<CircleCollider2D>();
        Bullet_SetEffect();
    }

    public void Bullet_SetEffect()
    {
        if (bullet_kind == BULLET_KIND.Normal)
        {
            circleCollider.isTrigger = true;
            bustObject = this.transform.root.Find("ShootEffect").Find("BulletEffect").gameObject;
            bustEffect = bustObject.GetComponent<ParticleSystem>();
        }
        else if(bullet_kind == BULLET_KIND.Bounce)
        {
            circleCollider.isTrigger = false;
            bustObject = this.transform.root.Find("ShootEffect").Find("BounceBulletEffect").gameObject;
            bustEffect = bustObject.GetComponent<ParticleSystem>();
        }
    }

    internal void Bullet_Velocity(Vector2 velocity)
    {
        bulletVelocty = velocity.normalized * bulletSpeed;
    }
    private bool Bullet_Collider_BounceCheck(Collision2D collision)
    {
        if (bulletBounce > 0)
        {
            bulletBounce -= 1;

            float curspeed = bulletVelocty.magnitude;
            Vector2 tempDirection = Vector2.Reflect(bulletVelocty.normalized, collision.contacts[0].normal);

            bulletRigid.velocity = tempDirection * Mathf.Max(curspeed, 0);

            return true;
        }
        return false;
    }

    internal void Bullet_Active()
    {
        Bullet_KindChange();
        bulletTrail.emitting    = true;
        bulletRigid.simulated   = true;
        bulletRigid.velocity    = bulletVelocty;
        bulletTrail.time        = 0.3f;
        bulletMove              = true;
    }

    internal void Bullet_Refrash()
    {
        Bullet_Deactive();
        this.transform.position = bulletRefrashPosition;
        bullet_kind = BULLET_KIND.Normal;
        Bullet_SetEffect();
    }

    internal void Bullet_Setting(BULLET_KIND kind, float speed)
    {
        bullet_kind = kind;
        Bullet_SetEffect();
    }

    internal void Bullet_Deactive()
    {
        circleCollider.isTrigger = true;
        bulletTrail.emitting    = false;
        bulletRigid.simulated   = false;
        bulletRigid.velocity    = Vector2.zero;
        bulletTrail.time        = 0f;
        bulletMove              = false;
        this.gameObject.SetActive(false);
    }

    private void Bullet_KindChange()
    {
        switch (bullet_kind)
        {
            case BULLET_KIND.Normal:
                bulletBounce = 0;
                circleCollider.isTrigger = true;
                bulletTrail = normalbullet.Normal_Setting(bulletTrail);
                break;
            case BULLET_KIND.Bounce:
                bulletBounce = 1;
                circleCollider.isTrigger = false;
                bulletTrail = bouncebullet.Bounce_Setting(bulletTrail);
                break;
        }
    }

    public void Bullet_CountUp()
    {
        GameManager.Instance.Player_ShootCountUp();
    }

    internal bool Bullet_IsMove()
    {
        return bulletMove;
    }

    private void Bullet_Collision()
    {
        Bullet_CountUp();
        Bullet_Deactive();
        //폭발 이펙트 실행 필요
    }

    public void Bullet_Bust()
    {
        var clone = Instantiate(bustObject, this.transform.position,Quaternion.identity);

        Destroy(clone, 0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.parent == null)
            return;

        if (collision.gameObject.transform.parent.tag == "ShootWall")
        {
            this.transform.position = bulletRefrashPosition;
            Bullet_Collision();
        }

        Rigid_Event(collision);
        Bullet_ColliderActive(collision, collision.gameObject.transform.parent.tag);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.transform.parent == null)
            return;

        if (collider.gameObject.transform.parent.tag == "ShootWall")
        {
            this.transform.position = bulletRefrashPosition;
            Bullet_Collision();
        }

        Rigid_Event(null, collider);
        Bullet_TriggerActive(collider, collider.gameObject.transform.parent.tag);
    }

    public void Bullet_ColliderActive(Collision2D collision, string tag)
    {
        switch (collision.gameObject.transform.tag)
        {
            case "HingeWall":
            case "StaticWall":
            case "GoalWall":
            case "OutWall":
                Bullet_Bust();
                if (Bullet_Collider_BounceCheck(collision) == true){
                    circleCollider.isTrigger = true;
                    break;
                }
                Bullet_ColliderActive_BulletRefrash();
                break;
            case "BrokenWall":
                collision.gameObject.GetComponent<Explodable>().explode();
                ExplosionForce ef = GameManager.Instance.GetComponent<ExplosionForce>();
                ef.doExplosion(this.transform.position);
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
            case "Bomb":
                collision.gameObject.transform.parent.GetComponent<Bomb>().Bomb_EffectOn();
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
        }
    }

    //총알 트리거 진행 시 행동
    public void Bullet_TriggerActive(Collider2D collision, string tag)
    {
        switch (collision.gameObject.transform.parent.tag)
        {
            case "HingeWall":
            case "StaticWall":
            case "GoalWall":
            case "OutWall":
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
            case "BrokenWall":
                collision.gameObject.GetComponent<Explodable>().explode();
                ExplosionForce ef = GameManager.Instance.GetComponent<ExplosionForce>();
                ef.doExplosion(this.transform.position);
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
            case "Bomb":
                collision.gameObject.transform.parent.GetComponent<Bomb>().Bomb_EffectOn();
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
            case "Enemy":
                Bullet_Bust();
                Bullet_ColliderActive_BulletRefrash();
                break;
        }
    }

    private void Rigid_Event(Collision2D collision = null, Collider2D collider = null)
    {
        bulletTrail.emitting = true;
    }

    private void Bullet_ColliderActive_BulletRefrash()
    {
        this.transform.position = bulletRefrashPosition;
        this.gameObject.SetActive(false);
        Bullet_Collision();
    }



    private void OnBecameInvisible()
    {
        bulletTrail.emitting = false;
        this.transform.position = bulletRefrashPosition;
        Bullet_CountUp();
        Bullet_Deactive();
        this.gameObject.SetActive(false);
    }
}
