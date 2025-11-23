using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager Instance;
    
    public GameObject tablePrefab; // 餐桌预制体
    public Transform tablesParent; // 所有餐桌的父对象
    
    private Dictionary<string, DraggableTable> allTables = new Dictionary<string, DraggableTable>();
    private int tableCounter = 0;
    
    public int initialCoinLimit = 1000;
    private int currentCoins;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        currentCoins = initialCoinLimit;
    }
    
    public bool CanBuyTable(int cost)
    {
        return currentCoins >= cost;
    }
    
    public bool BuyTable(int cost)
    {
        if (CanBuyTable(cost))
        {
            currentCoins -= cost;
            return true;
        }
        return false;
    }
    
    public void SellTable(string tableId, int refundAmount)
    {
        if (allTables.ContainsKey(tableId))
        {
            currentCoins += refundAmount;
            Destroy(allTables[tableId].gameObject);
            allTables.Remove(tableId);
        }
    }
    
    public void AddTable(DraggableTable table, string id)
    {
        allTables[id] = table;
    }
    
    public void RemoveTable(string id)
    {
        if (allTables.ContainsKey(id))
            allTables.Remove(id);
    }
    
    public Dictionary<string, TableData> GetAllTableData()
    {
        Dictionary<string, TableData> data = new Dictionary<string, TableData>();
        
        foreach (var pair in allTables)
        {
            DraggableTable table = pair.Value;
            data[pair.Key] = new TableData(
                pair.Key,
                table.typeId,
                table.transform.position,
                table.transform.rotation,
                table.isFlipped,
                table.level
            );
        }
        
        return data;
    }
    
    public void LoadTables(Dictionary<string, TableData> tableData)
    {
        // 清除现有餐桌
        foreach (Transform child in tablesParent)
            Destroy(child.gameObject);
        
        allTables.Clear();
        
        // 实例化保存的餐桌
        foreach (var data in tableData.Values)
        {
            GameObject newTable = Instantiate(tablePrefab, data.position, data.rotation, tablesParent);
            DraggableTable draggableTable = newTable.GetComponent<DraggableTable>();
            
            if (draggableTable != null)
            {
                draggableTable.tableId = data.tableId;
                draggableTable.isFlipped = data.isFlipped;
                draggableTable.level = data.level;
                
                allTables[data.tableId] = draggableTable;
            }
        }
    }
}