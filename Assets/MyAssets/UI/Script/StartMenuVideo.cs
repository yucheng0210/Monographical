using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class StartMenuVideo : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    [SerializeField]
    private VideoClip menuClip;
    private RawImage rawImage;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        rawImage = GetComponent<RawImage>();
    }

    private void Start()
    {
        videoPlayer.isLooping = true;
        videoPlayer.clip = menuClip;
        AudioManager.Instance.MainAudio();
        VideoFade();
    }

    private void Update()
    {
        if (videoPlayer.texture == null)
            return;
        rawImage.texture = videoPlayer.texture;
    }

    private void VideoFade()
    {
        videoPlayer.Play();
        SceneFader sceneFader = GetComponent<SceneFader>();
        StartCoroutine(sceneFader.FadeOut());
    }
}
