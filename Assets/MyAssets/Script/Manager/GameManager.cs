using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private DiasGames.ThirdPersonSystem.Health health;
    private bool gameIsOver;
    private List<Enemy> enemies;
    private CharacterState playerState;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        enemies = new List<Enemy>();
    }

    public void RegisterPlayer(CharacterState player)
    {
        playerState = player;
    }

    public CharacterState PlayerState
    {
        get { return playerState; }
    }

    public void PlayerDied()
    {
        if (Instance.gameIsOver)
            return;
        if (health.HealthValue <= 0)
        {
            Instance.gameIsOver = true;
        }
    }

    public bool GameOver()
    {
        return Instance.gameIsOver;
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
