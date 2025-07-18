using UnityEngine;

public class Test2DEditor : MonoBehaviour
{
    public SheepLevelEditor2D editor2D;
    
    void Start()
    {
        // 查找2D编辑器
        if (editor2D == null)
        {
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor2D != null)
        {
            Debug.Log("✅ 找到2D编辑器");
            Debug.Log($"编辑器模式: {editor2D.isEditMode}");
            Debug.Log($"当前层级: {editor2D.selectedLayer}");
            Debug.Log($"卡片类型: {editor2D.currentCardType}");
        }
        else
        {
            Debug.LogError("❌ 未找到2D编辑器！");
        }
    }
    
    void Update()
    {
        // 按空格键切换编辑模式
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (editor2D != null)
            {
                editor2D.isEditMode = !editor2D.isEditMode;
                Debug.Log($"编辑模式: {(editor2D.isEditMode ? "开启" : "关闭")}");
            }
        }
        
        // 按E键激活编辑器
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (editor2D != null)
            {
                editor2D.isEditMode = true;
                Debug.Log("激活2D编辑器");
            }
        }
    }
} 