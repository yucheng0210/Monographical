using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class QuestCsvToSO
{
    [MenuItem("CSVToSO/CreateQuestSO")]
    public static void CreateQuestSO()
    {
        string[] allLineData = File.ReadAllLines(
            "Assets/MyAssets/QuestSystem/QuestData/QUESTMANAGER.csv"
        );
        allLineData = allLineData[0].Split(new char[] { '\n' });
        for (int i = 1; i < allLineData.Length - 1; i++)
        {
            Debug.Log("0");
            string[] row = allLineData[i].Split(',');
            if (row[1] == "")
                break;
            Quest_SO quest = ScriptableObject.CreateInstance<Quest_SO>();
            quest.ID = int.Parse(row[0]);
            quest.TheName = row[1];
            quest.NPC = row[2];
            quest.Des = row[3];
            quest.Status = int.Parse(row[4]);
            quest.Rewards = row[5];
            quest.Task = row[6];
            quest.Parent = int.Parse(row[7]);
            AssetDatabase.CreateAsset(
                quest,
                $"Assets/MyAssets/QuestSystem/QuestData/{quest.TheName}.asset"
            );
        }
        AssetDatabase.SaveAssets();
        Debug.Log("哈哈");
    }
}
