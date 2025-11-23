using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableTable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string tableId;
    public int typeId;
    public int cost = 100;
    public int upgradeCost = 50;
    public int level = 1;
    public bool isFlipped = false;
    
    private Camera mainCamera;
    private Vector3 offset;
    private bool isDragging = false;
    private GameObject previewObject;
    private Renderer[] renderers;
    private Material validMaterial;
    private Material invalidMaterial;
    
    public ParticleSystem placeEffect;
    public AudioClip placeSound;
    
    void Start()
    {
        mainCamera = Camera.main;
        renderers = GetComponentsInChildren<Renderer>();
        
        // 创建预览材质
        validMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        validMaterial.color = new Color(0, 1, 0, 0.5f); // 半透明绿色
        
        invalidMaterial = new Material(Shader.Find("Transparent/Diffuse"));
        invalidMaterial.color = new Color(1, 0, 0, 0.5f); // 半透明红色
        
        // 生成唯一ID（如果还没有）
        if (string.IsNullOrEmpty(tableId))
            tableId = System.Guid.NewGuid().ToString();
            
        // 注册到管理器
        TableManager.Instance.AddTable(this, tableId);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!TableManager.Instance.CanBuyTable(cost))
            return;
            
        isDragging = true;
        
        // 创建预览对象
        previewObject = Instantiate(gameObject, transform.position, transform.rotation);
        previewObject.name = "TablePreview";
        
        // 移除预览对象上的所有组件（除了渲染器）
        foreach (var component in previewObject.GetComponents<Component>())
        {
            if (!(component is Transform) && !(component is Renderer) && !(component is MeshFilter))
                Destroy(component);
        }
        
        // 设置预览材质
        Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in previewRenderers)
        {
            r.material = validMaterial;
        }
        
        // 隐藏原对象
        SetRenderersEnabled(false);
        
        // 计算偏移量
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
        offset = transform.position - worldPosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        // 计算目标位置
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, mainCamera.WorldToScreenPoint(transform.position).z));
        Vector3 targetPosition = worldPosition + offset;
        
        // 对齐到网格
        targetPosition = GridSystem.Instance.GetNearestPointOnGrid(targetPosition);
        previewObject.transform.position = targetPosition;
        
        // 检查位置是否有效
        bool isValidPosition = IsValidPosition(targetPosition);
        
        // 更新预览材质
        Renderer[] previewRenderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in previewRenderers)
        {
            r.material = isValidPosition ? validMaterial : invalidMaterial;
        }
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        
        isDragging = false;
        
        // 检查位置是否有效并购买餐桌
        bool isValidPosition = IsValidPosition(previewObject.transform.position);
        bool purchased = TableManager.Instance.BuyTable(cost);
        
        if (isValidPosition && purchased)
        {
            // 放置餐桌
            transform.position = previewObject.transform.position;
            
            // 显示原对象
            SetRenderersEnabled(true);
            
            // 播放效果
            if (placeEffect != null)
                Instantiate(placeEffect, transform.position, Quaternion.identity);
                
            if (placeSound != null)
                AudioSource.PlayClipAtPoint(placeSound, transform.position);
        }
        else
        {
            // 取消放置
            if (!purchased)
                Debug.Log("金币不足，无法购买餐桌");
            else
                Debug.Log("位置无效，无法放置餐桌");
                
            // 显示原对象
            SetRenderersEnabled(true);
        }
        
        // 销毁预览对象
        Destroy(previewObject);
    }
    
    private bool IsValidPosition(Vector3 position)
    {
        // 检查碰撞
        Collider[] hitColliders = Physics.OverlapBox(position, GetComponent<Collider>().bounds.extents);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject && hitCollider.gameObject != previewObject)
                return false;
        }
        
        return true;
    }
    
    private void SetRenderersEnabled(bool enabled)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = enabled;
        }
    }
    
    public void FlipTable()
    {
        isFlipped = !isFlipped;
        transform.Rotate(0, 180, 0);
    }
    
    public void UpgradeTable()
    {
        if (TableManager.Instance.CanBuyTable(upgradeCost))
        {
            TableManager.Instance.BuyTable(upgradeCost);
            level++;
            // 这里可以添加升级效果，比如改变模型、增加功能等
        }
    }
    
    public void RemoveTable()
    {
        TableManager.Instance.SellTable(tableId, cost / 2); // 半价回收
    }
    
    void OnDestroy()
    {
        TableManager.Instance.RemoveTable(tableId);
    }
}