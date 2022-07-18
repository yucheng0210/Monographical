using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : Singleton<SceneController>
{
    private GameObject player;

    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField]
    private Slider progressSlider;

    [SerializeField]
    private Text progressText;

    [SerializeField]
    private GameObject progressCanvas;

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
            progressCanvas.SetActive(true);
            DontDestroyOnLoad(player);
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                if (async.progress < 0.9f)
                    progressSlider.value = async.progress;
                else
                    progressSlider.value = 1.0f;
                if (progressSlider.value == 1.0f)
                    async.allowSceneActivation = true;
                progressText.text = (int)(progressSlider.value * 100) + "%";
                yield return null;
            }
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            progressCanvas.SetActive(false);
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
