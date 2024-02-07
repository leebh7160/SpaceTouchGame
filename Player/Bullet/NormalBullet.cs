using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet
{
    internal TrailRenderer Normal_Setting(TrailRenderer trail)
    {
        trail.startColor = Color.cyan;
        trail.endColor   = Color.white;

        Normal_ImageSetting(trail.transform.GetChild(0).GetComponent<SpriteRenderer>());
        return trail;

    }
    void Normal_ImageSetting(SpriteRenderer sprite)
    {
        sprite.color = Color.cyan;
    }
}
