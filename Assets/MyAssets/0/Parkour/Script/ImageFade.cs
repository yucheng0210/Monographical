using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageFade : MonoBehaviour
{
    [SerializeField]
    private DialogSystem dialogSystem;

    [SerializeField]
    private SceneFader wifeDeathImage;
    private bool isChecked;

    private void Update()
    {
        if (dialogSystem.OpenMenu)
        {
            wifeDeathImage.StartCoroutine(wifeDeathImage.FadeOutIn(5));
            dialogSystem.BlockContinue = true;
            dialogSystem.OpenMenu = false;
        }
        if (wifeDeathImage == null && !isChecked)
        {
            dialogSystem.BlockContinue = false;
            isChecked = true;
        }
    }
}
