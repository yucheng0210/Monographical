using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, ISavable
{
    private bool gameIsOver;
    private List<Enemy> enemies;
    private CharacterState playerState;
    private float gameTime;
    List<IObserver> observerList = new List<IObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        enemies = new List<Enemy>();
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
        IEffectFactory effectFactory = new EffectFactory();
        BackpackManager.Instance.RegisterFactory(effectFactory);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
            gameTime = 0;
        else
            gameTime += Time.unscaledDeltaTime;
    }

    public void AddObservers(IObserver observer)
    {
        observerList.Add(observer);
    }

    public void RemoveObservers(IObserver observer)
    {
        observerList.Remove(observer);
    }

    public void EndNotifyObservers()
    {
        foreach (var observer in observerList)
            observer.EndNotify();
    }

    public void LoadingNotify(bool loadingBool)
    {
        foreach (var observer in observerList)
            observer.SceneLoadingNotify(loadingBool);
    }

    public void RegisterPlayer(CharacterState player)
    {
        playerState = player;
    }

    public CharacterState PlayerState
    {
        get { return playerState; }
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void AddSavableRegister()
    {
        SaveLoadManager.Instance.AddRegister(this);
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData gameSaveData = new GameSaveData();
        gameSaveData.gameTime = this.gameTime;
        return gameSaveData;
    }

    public void RestoreGameData(GameSaveData gameSaveData)
    {
        this.gameTime = gameSaveData.gameTime;
    }
}
