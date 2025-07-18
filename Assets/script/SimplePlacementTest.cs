using UnityEngine;

public class SimplePlacementTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🧪 简单放置测试已启动");
        Debug.Log("左键点击任意位置测试卡片放置");
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TestPlacement();
        }
    }
    
    void TestPlacement()
    {
        Debug.Log("=== 放置测试 ===");
        
        // 检查编辑器状态
        var editor2D = FindObjectOfType<SheepLevelEditor2D>();
        
        if (editor2D != null)
        {
            Debug.Log($"✅ 找到2D编辑器: 卡片大小={editor2D.cardSize}, 层级={editor2D.selectedLayer}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到2D编辑器，请确保场景中有SheepLevelEditor2D组件");
        }
        
        // 检查相机
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"✅ 主相机: {mainCamera.name}, 位置={mainCamera.transform.position}");
        }
        else
        {
            Debug.LogWarning("⚠️ 未找到主相机");
        }
        
        Debug.Log("=== 测试完成 ===");
    }
} 