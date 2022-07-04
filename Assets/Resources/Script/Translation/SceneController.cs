using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private GameObject player;

    public void TranslationToDestination(TranslationPoint translationPoint)
    {
        switch (translationPoint.Type)
        {
            case TranslationPoint.TranslationType.SameScene:
                StartCoroutine(
                    Translation(SceneManager.GetActiveScene().name, translationPoint.Tag)
                );
                break;
            case TranslationPoint.TranslationType.DifferentScene:
                break;
        }
    }

    IEnumerator Translation(string sceneName, TranslationDestination.DestinationTag destinationTag)
    {
        player = GameManager.Instance.PlayerState.gameObject;
        player.transform.SetPositionAndRotation(
            GetDestination(destinationTag).transform.position,
            GetDestination(destinationTag).transform.rotation
        );
        yield return null;
    }

    private TranslationDestination GetDestination(
        TranslationDestination.DestinationTag destinationTag
    )
    {
        var entrances = FindObjectsOfType<TranslationDestination>();
        foreach (var i in entrances)
        {
            if (i.Tag == destinationTag)
                return i;
        }
        return null;
    }
}
