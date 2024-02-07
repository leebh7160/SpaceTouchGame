using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceBullet
{
    internal TrailRenderer Bounce_Setting(TrailRenderer trail)
    {
        trail.startColor    = Color.magenta;
        trail.endColor      = Color.white;

        Bounce_ImageSetting(trail.transform.GetChild(0).GetComponent<SpriteRenderer>());
        return trail;
    }

    private void Bounce_ImageSetting(SpriteRenderer sprite)
    {
        sprite.color = Color.magenta;
    }
}
