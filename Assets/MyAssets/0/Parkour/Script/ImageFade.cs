using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFade : MonoBehaviour
{
    [SerializeField]
    private DialogSystem dialogSystem;

    [SerializeField]
    private SceneFader wifeDeathImage;

    private void Update()
    {
        if (dialogSystem.OpenMenu)
        {
            wifeDeathImage.StartCoroutine(wifeDeathImage.FadeOutIn(5));
            dialogSystem.OpenMenu = false;
        }
    }
}
