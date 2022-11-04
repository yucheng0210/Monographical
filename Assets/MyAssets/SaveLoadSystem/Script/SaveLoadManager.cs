using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadManager : Singleton<SaveLoadManager>
{
    private string jsonFolder;
    private List<ISavable> savableList = new List<ISavable>();
    private Dictionary<string, GameSaveData> saveDataDic = new Dictionary<string, GameSaveData>();
    private int currentPathId = 0;

    protected override void Awake()
    {
        base.Awake();
        jsonFolder = Application.dataPath + "/SAVE/";
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
            Save(0); /*
        if (Input.GetKeyDown(KeyCode.F7))
            Load();*/
    }

    public void AddRegister(ISavable savable)
    {
        savableList.Add(savable);
    }

    public void Save(int id)
    {
        saveDataDic.Clear();
        foreach (var savable in savableList)
        {
            saveDataDic.Add(savable.GetType().Name, savable.GenerateGameData());
        }
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        var jsonData = JsonConvert.SerializeObject(saveDataDic, Formatting.Indented);
        if (!File.Exists(resultPath))
            Directory.CreateDirectory(jsonFolder);
        File.WriteAllText(resultPath, jsonData);
    }

    public void Load(int id)
    {
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        if (!File.Exists(resultPath))
            return;
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);
        foreach (var savable in savableList)
        {
            savable.RestoreGameData(jsonData[savable.GetType().Name]);
        }
        currentPathId = id;
    }

    public string GetDataName(int id)
    {
        string dataName = "NODATA";
        var resultPath = jsonFolder + "data" + (id + 1).ToString() + ".sav";
        if (!File.Exists(resultPath))
            return dataName;
        var stringData = File.ReadAllText(resultPath);
        var jsonData = JsonConvert.DeserializeObject<Dictionary<string, GameSaveData>>(stringData);
        return jsonData["SceneController"].dataName;
    }

    public void AutoSave()
    {
        Save(currentPathId);
        Debug.Log("autosave" + currentPathId.ToString());
    }
}
