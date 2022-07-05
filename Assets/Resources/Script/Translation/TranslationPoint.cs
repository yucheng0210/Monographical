using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationPoint : MonoBehaviour
{
    public enum TranslationType
    {
        SameScene,
        DifferentScene
    }

    private bool canTrans;

    [SerializeField]
    private string sceneName;

    [SerializeField]
    private TranslationType translationType;

    [SerializeField]
    private TranslationDestination.DestinationTag destinationTag;
    public TranslationType Type
    {
        get { return translationType; }
    }
    public TranslationDestination.DestinationTag Tag
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
            SceneController.Instance.TranslationToDestination(this);
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
