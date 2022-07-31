using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool gameIsOver;
    private List<Enemy> enemies;
    private CharacterState playerState;
    List<IObserver> observers = new List<IObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        enemies = new List<Enemy>();
    }

    public void AddObservers(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObservers(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void EndNotifyObservers()
    {
        foreach (var observer in observers)
            observer.EndNotify();
    }

    public void LoadingNotify()
    {
        foreach (var observer in observers)
            observer.SceneLoadingNotify();
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
