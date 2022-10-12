using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Baffle : MonoBehaviour
{
    public enum BaffleType
    {
        Up,
        Down,
        Right,
        Left
    }

    [SerializeField]
    private GameObject clueCanvas;

    [SerializeField]
    private Image ringImage;
    public Image RingImage
    {
        get { return ringImage; }
        set { ringImage = value; }
    }

    public BaffleType baffleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            clueCanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            clueCanvas.SetActive(false);
    }
}
