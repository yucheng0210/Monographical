using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Volume : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource menuAudio;
    void Start()
    {
        volumeSlider.value=0.5f;
    }
    void Update()
    {
        menuAudio.volume=volumeSlider.value;
    }
}
