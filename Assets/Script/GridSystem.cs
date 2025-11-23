using UnityEngine;

public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance; //单例模式

    public float gridSize = 1f;//网格大小
    public bool snapToGrid = true;//是否将物体对齐到网格
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    //获取物体在网格上的最近点
    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        if (!snapToGrid) 
        {
            return position;
        }
        position -= transform.position;
        int xCount = Mathf.RoundToInt(position.x / gridSize);
        int yCount = Mathf.RoundToInt(position.y / gridSize);
        int zCount = Mathf.RoundToInt(position.z / gridSize);

        Vector3 result = new Vector3(
            (float)xCount * gridSize,
            (float)yCount * gridSize,
            (float)zCount * gridSize);
        result += transform.position;
        
        return result;
    }
    //改变位置
    public void ChangeGridSize(float newSize)
    {
        gridSize = newSize;
    }

    // Update is called once per frame
    // 可视化网格（在Scene视图中显示）
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (float x = 0; x < 40; x += gridSize)
        {
            for (float z = 0; z < 40; z += gridSize)
            {
                var point = GetNearestPointOnGrid(new Vector3(x, 0, z));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
