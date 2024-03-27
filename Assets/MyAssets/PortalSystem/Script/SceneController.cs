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
    private CanvasGroup fadeMenu;
    private Dictionary<int, string> sceneNameDic = new Dictionary<int, string>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
        EventManager.Instance.AddEventRegister(EventDefinition.eventDialogEvent, EventDialogEvent);
        AddSceneName();
    }
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(1);
    }*/

    private void AddSceneName()
    {
        sceneNameDic.Add(0, "0");
        sceneNameDic.Add(1, "序章：下雨中的市集");
        sceneNameDic.Add(2, "序章：宋江廟");
        sceneNameDic.Add(3, "第一章：宋江陣根據地");
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.Type)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(PortalTransition(SceneManager.GetActiveScene().name, transitionPoint.Tag));
                break;
            case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(PortalTransition(transitionPoint.SceneName, transitionPoint.Tag));
                break;
        }
    }

    private IEnumerator PortalTransition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        Main.Manager.GameManager.Instance.LoadingNotify(true);
        player = Main.Manager.GameManager.Instance.PlayerTrans.gameObject;
        progressSlider.value = 0.0f;
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            DontDestroyOnLoad(player);
            yield return StartCoroutine(UIManager.Instance.FadeOut(fadeMenu, 0.5f));
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
            Main.Manager.GameManager.Instance.LoadingNotify(false);
            yield return StartCoroutine(UIManager.Instance.FadeIn(fadeMenu, 0.5f));
        }
        else
        {
            yield return StartCoroutine(UIManager.Instance.FadeOut(fadeMenu, 0.5f));
            player.transform.SetPositionAndRotation(
                GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation
            );
            Main.Manager.GameManager.Instance.LoadingNotify(false);
            yield return StartCoroutine(UIManager.Instance.FadeIn(fadeMenu, 0.5f));
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
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
        //Main.Manager.GameManager.Instance.LoadingNotify(true);
        EventManager.Instance.DispatchEvent(EventDefinition.eventSceneLoading);
        progressSlider.value = 0.0f;
        UIManager.Instance.UIDict.Clear();
        yield return StartCoroutine(UIManager.Instance.FadeOut(fadeMenu, 0.5f));
        progressCanvas.SetActive(true);
        progressText.text = (int)(progressSlider.value * 100) + "%";
        yield return StartCoroutine(UIManager.Instance.FadeIn(fadeMenu, 0.5f));
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        async.completed += operation => { StartCoroutine(UIManager.Instance.FadeIn(fadeMenu, 0.5f)); };
        while (!async.isDone)
        {
            if (progressSlider.value < 0.99f)
            {
                progressSlider.value = Mathf.Lerp(
                    progressSlider.value,
                    async.progress / 9 * 10,
                    Time.unscaledDeltaTime
                );
                progressText.text = (int)(progressSlider.value * 100) + "%";
            }
            else if (!async.allowSceneActivation)
            {
                progressSlider.value = 1.0f;
                progressText.text = (int)(progressSlider.value * 100) + "%";
                AudioManager.Instance.ClearAllAudioClip();
                yield return StartCoroutine(UIManager.Instance.FadeOut(fadeMenu, 0.5f));//等待淡出(這時畫面淡出是黑屏，避免提早出現下個場景畫面)
                progressCanvas.SetActive(false);//隱藏加載畫面
                async.allowSceneActivation = true;//允許場景轉換
            }
            yield return null;
        }
        // Main.Manager.GameManager.Instance.LoadingNotify(false);
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        if (SceneManager.GetActiveScene().name == "StartMenu")
        {
            gameSaveData.currentScene = "Prologue_1";
            gameSaveData.dataName = sceneNameDic[SceneManager.GetActiveScene().buildIndex + 1];
        }
        else
        {
            gameSaveData.currentScene = SceneManager.GetActiveScene().name;
            gameSaveData.dataName = sceneNameDic[SceneManager.GetActiveScene().buildIndex];
        }
        return gameSaveData;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        StartCoroutine(Transition(gameSaveData.currentScene));
    }
    private void EventDialogEvent(params object[] args)
    {
        Debug.Log("change");
        if ((string)args[0] == "CHANGESCENE")
        {
            StartCoroutine(Transition("ChapterOne"));
        }
    }
}
