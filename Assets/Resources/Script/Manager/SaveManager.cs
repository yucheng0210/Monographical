using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
            SavePlayerData();
        if (Input.GetKeyDown(KeyCode.F7))
            LoadPlayerData();
    }

    public void SavePlayerData()
    {
        GameManager.Instance.PlayerState.X =
            GameManager.Instance.PlayerState.gameObject.transform.position.x;
        GameManager.Instance.PlayerState.Y =
            GameManager.Instance.PlayerState.gameObject.transform.position.y;
        GameManager.Instance.PlayerState.Z =
            GameManager.Instance.PlayerState.gameObject.transform.position.z;
        Save(
            GameManager.Instance.PlayerState.CharacterData,
            GameManager.Instance.PlayerState.CharacterData.name
        );
    }

    public void LoadPlayerData()
    {
        Load(
            GameManager.Instance.PlayerState.CharacterData,
            GameManager.Instance.PlayerState.CharacterData.name
        );
        GameManager.Instance.PlayerState.gameObject.transform.position = new Vector3(
            GameManager.Instance.PlayerState.X,
            GameManager.Instance.PlayerState.Y,
            GameManager.Instance.PlayerState.Z
        );
    }

    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
    }
}
