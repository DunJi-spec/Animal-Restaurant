using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator;

    [SerializeField]
    private InputManger inputManager;

    private void Update()
    {
        // 获取鼠标位置
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();;
        // 更新鼠标指示器位置
        mouseIndicator.transform.position = mousePosition;
    }
}
