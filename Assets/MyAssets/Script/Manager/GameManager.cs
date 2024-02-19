using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>, ISavable
{
    private List<Enemy> enemies;
    private CharacterState playerState;
    private float gameTime;
    List<IObserver> observerList = new List<IObserver>();
    public Character PlayerData { get; private set; }
    public Transform PlayerTrans { get; set; }
    public Animator PlayerAni { get; private set; }
    public Weapon PlayerEquipWeapon { get; set; }
    public List<Character> EnemyList { get; private set; }
    public Dictionary<int, int> CurrentTotalKill { get; set; }
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        enemies = new List<Enemy>();
        EnemyList = new List<Character>();
        CurrentTotalKill = new Dictionary<int, int>();
    }

    private void Start()
    {
        ISavable savable = this;
        savable.AddSavableRegister();
        InitializeData();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartMenu")
            gameTime = 0;
        else
            gameTime += Time.unscaledDeltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && !UIManager.Instance.UIDict["MineSweaperMenu"].OpenBool)
        {
            if (!UIManager.Instance.MenuIsOpen)
                UIManager.Instance.ShowUI("MainMenu");
            else
                UIManager.Instance.HideAllUI();
        }
    }
    private void InitializeData()
    {
        BackpackManager.Instance.AddWeapon(1);
        PlayerEquipWeapon = DataManager.Instance.WeaponBag[1];
    }
    public void AddCurrentTotalKill(int id)
    {
        if (CurrentTotalKill.ContainsKey(id))
            CurrentTotalKill[id] += 1;
        else
            CurrentTotalKill.Add(id, 1);
    }
    public void TakeDamage(Character attacker, Character defender)
    {
        float weaponDamage = PlayerEquipWeapon.WeaponAttack;
        float damage = Random.Range(attacker.MinAttack + weaponDamage, attacker.MaxAttack + weaponDamage);
        bool isCritical = Random.value < (attacker.CriticalChance + PlayerEquipWeapon.WeaponCriticalChance);
        if (isCritical)
            damage *= (attacker.CriticalMultiplier + PlayerEquipWeapon.WeaponCriticalMultiplier);
        defender.CurrentHealth -= (int)Mathf.Max(damage - defender.CurrentDefence, 0);
        defender.CurrentPoise -= attacker.PoiseAttack;
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
        /*  foreach (var observer in observerList)
              observer.SceneLoadingNotify(loadingBool);*/
    }

    public void RegisterPlayer(Character playerData, Transform playerTrans, Animator playerAni)
    {
        PlayerData = playerData;
        PlayerTrans = playerTrans;
        PlayerAni = playerAni;
        PlayerData.CurrentHealth = PlayerData.MaxHealth;
        PlayerData.CurrentDefence = PlayerData.BaseDefence;
        PlayerData.CurrentPoise = PlayerData.MaxPoise;
    }

    public CharacterState PlayerState
    {
        get { return playerState; }
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
