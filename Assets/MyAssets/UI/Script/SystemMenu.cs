using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SystemMenu : UIBase
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button backToStartMenuButton;
    [SerializeField]
    private Button audioButton;

    [Header("音量")]
    [SerializeField]
    private GameObject audioMenu;
    [SerializeField]
    private Slider masterVolumeSlider;
    [SerializeField]
    private Slider menuVolumeSlider;
    [SerializeField]
    private Slider seVolumeSlider;
    [SerializeField]
    private Slider playerVolumeSlider;
    [SerializeField]
    private Slider bgmVolumeSlider;

    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    protected override void Update()
    {
        base.Update();
        UpdateValue();
    }
    private void Initialize()
    {
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        if (backToStartMenuButton != null)
            backToStartMenuButton.onClick.AddListener(() => StartCoroutine(SceneController.Instance.Transition("StartMenu")));
        audioButton.onClick.AddListener(() => audioMenu.SetActive(true));
        if (SceneManager.GetActiveScene().name == "StartMenu")
            AudioManager.Instance.MainAudio();
    }

    private void UpdateValue()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            audioMenu.SetActive(false);
        masterVolumeSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("Master", value));
        menuVolumeSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("Menu", value));
        seVolumeSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("SE", value));
        playerVolumeSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("Player", value));
        bgmVolumeSlider.onValueChanged.AddListener((float value) => AudioManager.Instance.ChanageAudioVolume("BGM", value));
        //ControllerOtherVolume();
    }
    private void ControllerOtherVolume()
    {
        if (menuVolumeSlider.value > masterVolumeSlider.value)
            menuVolumeSlider.value = masterVolumeSlider.value;
        if (seVolumeSlider.value > masterVolumeSlider.value)
            seVolumeSlider.value = masterVolumeSlider.value;
        if (playerVolumeSlider.value > masterVolumeSlider.value)
            playerVolumeSlider.value = masterVolumeSlider.value;
        if (bgmVolumeSlider.value > masterVolumeSlider.value)
            bgmVolumeSlider.value = masterVolumeSlider.value;
    }
    private void ExitGame()
    {
        SaveLoadManager.Instance.AutoSave();
        //UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
