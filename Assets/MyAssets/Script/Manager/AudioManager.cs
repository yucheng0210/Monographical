using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("主音效")]
    [SerializeField]
    private AudioClip mainClip;

    [SerializeField]
    private AudioClip battleClip;

    [Header("FX音效")]
    [SerializeField]
    private AudioClip buttonTouchClip;

    [SerializeField]
    private AudioClip swordSlashClip_1;

    [SerializeField]
    private AudioClip swordSlashClip_2;

    [SerializeField]
    private AudioClip impact;

    [SerializeField]
    private List<AudioClip> heavyAttackClips;

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
    [Header("跑酷音效")]
    [SerializeField]
    private AudioClip runningBreathingClip;
    [SerializeField]
    private AudioClip rainRunStepClip;
    private AudioSource menuSource,
        fxSource,
        playerSource,
        mainSource;

    protected override void Awake()
    {
        base.Awake();
        // DontDestroyOnLoad(this);
        menuSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        playerSource = gameObject.AddComponent<AudioSource>();
        mainSource = gameObject.AddComponent<AudioSource>();
        Instance.mainSource.loop = true;
    }
    public void ParkourAudio()
    {
        Instance.mainSource.clip = Instance.rainRunStepClip;
        Instance.mainSource.spread = 2;
        Instance.mainSource.Play();
        Instance.playerSource.clip = Instance.runningBreathingClip;
        Instance.playerSource.loop = true;
        Instance.playerSource.Play();
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

    public void HeavyAttackAudio(int id)
    {
        Instance.fxSource.clip = Instance.heavyAttackClips[id];
        Instance.fxSource.Play();
    }

    public void PlayerHurted()
    {
        Instance.playerSource.clip = Instance.hurt;
        Instance.playerSource.loop = false;
        Instance.playerSource.Play();
    }

    public void PlayerDied()
    {
        Instance.playerSource.clip = Instance.death;
        Instance.playerSource.loop = false;
        Instance.playerSource.Play();
    }

    public void MainAudio()
    {
        if (Instance.mainSource.clip == Instance.mainClip)
            return;
        Instance.mainSource.clip = Instance.mainClip;
        Instance.mainSource.Play();
    }

    public void BattleAudio()
    {
        if (Instance.mainSource.clip == Instance.battleClip)
            return;
        Instance.mainSource.clip = Instance.battleClip;
        Instance.mainSource.Play();
    }

    public void Impact()
    {
        Instance.fxSource.clip = Instance.impact;
        Instance.fxSource.Play();
    }

    public void SlashAudio(int count)
    {
        switch (count)
        {
            case 1:
                Instance.fxSource.clip = Instance.swordSlashClip_1;
                break;
            case 2:
                Instance.fxSource.clip = Instance.swordSlashClip_2;
                break;
        }
        Instance.fxSource.Play();
    }
}
