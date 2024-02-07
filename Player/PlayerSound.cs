using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound
{
    AudioSource audioSource;
    private AudioClip boostClip;
    private AudioClip shootClip;
    private AudioClip bombClip;

    public PlayerSound(AudioSource audioSource, AudioClip _boostClip, AudioClip _shootClip, AudioClip _bombClip)
    {
        this.audioSource = audioSource;
        this.boostClip = _boostClip;
        this.shootClip = _shootClip;
        this.bombClip = _bombClip;
    }

    #region 플레이어 관련 사운드
    internal void Player_UseBoost()//부스트 사용 시
    {
        audioSource.clip = boostClip;
        audioSource.Play();
    }
    internal void Player_UseShoot()//사격 시 사운드
    {
        audioSource.clip = shootClip;
        audioSource.Play();
    }

    internal void Player_TouchBomb()
    {
        audioSource.PlayOneShot(bombClip);
    }

    internal void Player_SoundValue(float soundvalue)//사운드 크기
    {
        audioSource.volume = soundvalue;
    }
    #endregion 플레이어 관련 사운드
}
