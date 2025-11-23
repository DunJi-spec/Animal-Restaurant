using System;
using UnityEngine;

[Serializable]
public class TableData
{
    public string tableId;
    public int typeId;
    public Vector3 position;
    public Quaternion rotation;
    public bool isFlipped;
    public int level;
    
    public TableData(string id, int type, Vector3 pos, Quaternion rot, bool flipped, int lvl)
    {
        tableId = id;
        typeId = type;
        position = pos;
        rotation = rot;
        isFlipped = flipped;
        level = lvl;
    }
}
