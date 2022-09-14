using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool gameIsOver;
    private List<Enemy> enemies;
    private CharacterState playerState;
    List<IObserver> observerList = new List<IObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        enemies = new List<Enemy>();
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
}
