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
        Left,
        TurnRight,
        TurnLeft,
        Trap,
        Climb
    }

    [SerializeField]
    private GameObject clueCanvas;

    [SerializeField]
    private float slowTime = 1.2f;

    [SerializeField]
    private Image ringImage;
    public GameObject ClueCanvas
    {
        get { return clueCanvas; }
        set { clueCanvas = value; }
    }
    public Image RingImage
    {
        get { return ringImage; }
        set { ringImage = value; }
    }
    public float SlowTime
    {
        get { return slowTime; }
    }
    public BaffleType baffleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            clueCanvas.SetActive(true);
    }
}
