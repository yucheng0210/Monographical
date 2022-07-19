using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {
        SameScene,
        DifferentScene
    }

    private bool canTrans;

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private TransitionType transitionType;

    [SerializeField]
    private TransitionDestination.DestinationTag destinationTag;
    public TransitionType Type
    {
        get { return transitionType; }
    }
    public TransitionDestination.DestinationTag Tag
    {
        get { return destinationTag; }
    }
    public string SceneName
    {
        get { return sceneName; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
