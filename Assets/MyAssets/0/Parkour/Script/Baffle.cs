using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Baffle : MonoBehaviour, IObserver
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

    private void Start()
    {
        GameManager.Instance.AddObservers(this);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            clueCanvas.SetActive(true);
    }

    public void EndNotify()
    {
        clueCanvas.SetActive(false);
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //throw new System.NotImplementedException();
    }
}
