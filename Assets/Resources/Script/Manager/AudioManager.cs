using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("FX音效")]
    [SerializeField]
    private AudioClip buttonTouchClip;

    [Header("Menu音效")]
    [SerializeField]
    private AudioClip menuEnterClip;

    [SerializeField]
    private AudioClip menuExitClip;

    [Header("Player音效")]
    [SerializeField]
    private AudioClip hurt;

    [SerializeField]
    private AudioClip death;
    private AudioSource menuSource,
        fxSource,
        playerSource;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        menuSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
    }

    public void MenuEnterAudio()
    {
        Instance.menuSource.clip = Instance.menuEnterClip;
        Instance.menuSource.Play();
    }

    public void MenuExitAudio()
    {
        Instance.menuSource.clip = Instance.menuExitClip;
        Instance.menuSource.Play();
    }

    public void ButtonTouchAudio()
    {
        Instance.fxSource.clip = Instance.buttonTouchClip;
        Instance.fxSource.Play();
    }

    public void PlayerHurted()
    {
        Instance.playerSource.clip = Instance.hurt;
        Instance.playerSource.Play();
    }

    public void PlayerDied()
    {
        Instance.playerSource.clip = Instance.death;
        Instance.playerSource.Play();
    }
}
