using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json; // 需要导入Newtonsoft.Json包

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;
    
    private string savePath;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        savePath = Path.Combine(Application.persistentDataPath, "tablesave.json");
    }
    
    [System.Serializable]
    private class SaveData
    {
        public List<TableData> tables = new List<TableData>();
    }
    
    public void SaveTables()
    {
        Dictionary<string, TableData> tableData = TableManager.Instance.GetAllTableData();
        SaveData saveData = new SaveData();
        
        foreach (var data in tableData.Values)
        {
            saveData.tables.Add(data);
        }
        
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(savePath, json);
        
        Debug.Log("游戏已保存: " + savePath);
    }
    
    public void LoadTables()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
            
            Dictionary<string, TableData> tableData = new Dictionary<string, TableData>();
            foreach (TableData data in saveData.tables)
            {
                tableData[data.tableId] = data;
            }
            
            TableManager.Instance.LoadTables(tableData);
            Debug.Log("游戏已加载: " + savePath);
        }
        else
        {
            Debug.Log("无保存文件，开始新游戏");
        }
    }
    
    void OnApplicationQuit()
    {
        SaveTables();
    }
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SaveTables();
    }
}