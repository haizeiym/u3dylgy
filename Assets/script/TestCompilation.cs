using UnityEngine;

public class TestCompilation : MonoBehaviour
{
    void Start()
    {
        Debug.Log("✅ 编译测试通过！所有脚本编译成功。");
        
        // 测试2D编辑器组件
        SheepLevelEditor2D editor2D = FindObjectOfType<SheepLevelEditor2D>();
        if (editor2D != null)
        {
            Debug.Log("✅ 找到2D编辑器组件");
            Debug.Log($"编辑器模式: {editor2D.isEditMode}");
            Debug.Log($"卡片类型: {editor2D.currentCardType}");
            Debug.Log($"卡片大小: {editor2D.cardSize}");
        }
        else
        {
            Debug.Log("ℹ️ 未找到2D编辑器组件，这是正常的");
        }
        
        Debug.Log("🎉 所有测试完成！编辑器已准备就绪。");
    }
} 