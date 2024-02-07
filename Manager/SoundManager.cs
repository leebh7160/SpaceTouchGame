using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instant;
    public static SoundManager Instant
    {
        get { return instant; }
    }

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip bgmClip;
    [SerializeField]
    private AudioClip specialClip;
    [SerializeField]
    private AudioClip EffectClip;

    bool m_enable;


    Dictionary<string, AudioClip> BgmAudioDic = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        instant = this;
    }

    private void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    internal bool Enable
    {
        get { return m_enable; }
        set
        {
            m_enable = value;
            if(m_enable)
                audioSource.mute = false;
            else
                audioSource.mute = true;
        }
    }

    internal void PlayVolume(float value)
    {
        audioSource.volume = value;
    }

    internal void PlayEffect(string name, float volume = 1.0f)
    {
        if (volume < 0.05f)
            return;

        if (volume > 1.0f)
            volume = 1.0f;

        if(enabled)
        {
            if (!BgmAudioDic.ContainsKey(name))
                BgmAudioDic[name] = bgmClip;
        }
    }

    internal void PlayEffect(AudioSource source, float volume = 1.0f)
    {
        if (volume < 0.05f)
            return;

        if (volume > 1.0f)
            volume = 1.0f;

        source.volume = volume;
        source.Play();
    }

    internal void PlayBgm(string name = "", float volume = 0.3f)
    {
        if (volume > 1.0f)
            volume = 1.0f;

        if(enabled)
        {
            audioSource.volume = volume;
            if(string.IsNullOrEmpty(name))
            {
                audioSource.Play();
                return;
            }

            if(!BgmAudioDic.ContainsKey(name))
            {
                if (name == "SPECIAL")
                {
                    audioSource.loop = false;
                    BgmAudioDic[name] = specialClip;
                }
                else
                {
                    audioSource.loop = true;
                    BgmAudioDic[name] = bgmClip;
                }
                if (BgmAudioDic[name] == null)
                    Debug.Log("사운드 에러 확인");
            }

            audioSource.clip = BgmAudioDic[name];
            audioSource.Play();
        }    
    }

    internal void StopBgm()
    {
        audioSource.Stop();
    }
}
