using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : Singleton<SceneController>, ISavable
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

    [SerializeField]
    private SceneFader sceneFaderPrefab;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.Type)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(
                    PortalTransition(SceneManager.GetActiveScene().name, transitionPoint.Tag)
                );
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(PortalTransition(transitionPoint.SceneName, transitionPoint.Tag));
                break;
        }
    }

    IEnumerator PortalTransition(
        string sceneName,
        TransitionDestination.DestinationTag destinationTag
    )
    {
        GameManager.Instance.LoadingNotify(true);
        SceneFader fade = Instantiate(sceneFaderPrefab);
        player = GameManager.Instance.PlayerState.gameObject;
        progressSlider.value = 0.0f;
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            DontDestroyOnLoad(player);
            yield return StartCoroutine(fade.FadeOut());
            progressCanvas.SetActive(true);
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;
            while (progressSlider.value < 0.99f)
            {
                progressSlider.value = Mathf.Lerp(
                    progressSlider.value,
                    async.progress / 9 * 10,
                    Time.deltaTime
                );
                progressText.text = (int)(progressSlider.value * 100) + "%";
                yield return null;
            }
            progressSlider.value = 1.0f;
            progressText.text = (int)(progressSlider.value * 100) + "%";
            yield return new WaitForSeconds(0.5f);
            progressCanvas.SetActive(false);
            async.allowSceneActivation = true;
            yield return async.isDone;
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            GameManager.Instance.LoadingNotify(false);
            yield return StartCoroutine(fade.FadeIn());
        }
        else
        {
            yield return StartCoroutine(fade.FadeOut());
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            GameManager.Instance.LoadingNotify(false);
            yield return StartCoroutine(fade.FadeIn());
            yield return null;
        }
    }

    private TransitionDestination GetDestination(
        TransitionDestination.DestinationTag destinationTag
    )
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var i in entrances)
        {
            if (i.Tag == destinationTag)
                return i;
        }
        return null;
    }

    public IEnumerator Transition(string sceneName)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        progressSlider.value = 0.0f;
        yield return StartCoroutine(fade.FadeOut());
        progressCanvas.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        while (progressSlider.value < 0.99f)
        {
            progressSlider.value = Mathf.Lerp(
                progressSlider.value,
                async.progress / 9 * 10,
                Time.deltaTime
            );
            progressText.text = (int)(progressSlider.value * 100) + "%";
            yield return null;
        }
        progressSlider.value = 1.0f;
        progressText.text = (int)(progressSlider.value * 100) + "%";
        yield return new WaitForSeconds(0.5f);
        progressCanvas.SetActive(false);
        async.allowSceneActivation = true;
        yield return StartCoroutine(fade.FadeIn());
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.currentScene = SceneManager.GetActiveScene().name;
        return gameSaveData;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        StartCoroutine(Transition(gameSaveData.currentScene));
    }
}
