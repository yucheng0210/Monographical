using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkButton : MonoBehaviour
{
    public GameObject Button;
    public GameObject talkUI;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            Button.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            Button.SetActive(false);
    }

    private void Update()
    {
        if (Button.activeSelf && Input.GetKeyDown(KeyCode.R))
            talkUI.SetActive(true);
    }
}
