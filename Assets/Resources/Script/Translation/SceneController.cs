using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private GameObject player;

    [SerializeField]
    private GameObject playerPrefab;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

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
                StartCoroutine(Translation(translationPoint.SceneName, translationPoint.Tag));
                break;
        }
    }

    IEnumerator Translation(string sceneName, TranslationDestination.DestinationTag destinationTag)
    {
        player = GameManager.Instance.PlayerState.gameObject;
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            DontDestroyOnLoad(player);
            yield return SceneManager.LoadSceneAsync(sceneName);
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            yield break;
        }
        else
        {
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            yield return null;
        }
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
