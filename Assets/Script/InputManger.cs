using UnityEngine;

public class InputManger : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayerMask;

    //获取选中地图位置
    public Vector3 GetSelectedMapPosition()
    {
        //获取鼠标位置
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;

        //创建射线
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        //射线检测
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100, placementLayerMask))
        {
            // 更新最后位置
            lastPosition = hit.point;
        }
        // 返回最后位置
        return lastPosition;
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    
}
