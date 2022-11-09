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
        TurnRight
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            clueCanvas.SetActive(false);
    }

    public void EndNotify()
    {
        clueCanvas.SetActive(false);
    }

    public void SceneLoadingNotify(bool loadingBool)
    {
        //        throw new System.NotImplementedException();
    }
}
