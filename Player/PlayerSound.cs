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

    #region �÷��̾� ���� ����
    internal void Player_UseBoost()//�ν�Ʈ ��� ��
    {
        audioSource.clip = boostClip;
        audioSource.Play();
    }
    internal void Player_UseShoot()//��� �� ����
    {
        audioSource.clip = shootClip;
        audioSource.Play();
    }

    internal void Player_TouchBomb()
    {
        audioSource.PlayOneShot(bombClip);
    }

    internal void Player_SoundValue(float soundvalue)//���� ũ��
    {
        audioSource.volume = soundvalue;
    }
    #endregion �÷��̾� ���� ����
}
