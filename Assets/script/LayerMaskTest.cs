using UnityEngine;

public class LayerMaskTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool testLayerMasks = true;
    public float testInterval = 2f;
    
    private SheepLevelEditor2D editor2D;
    private float lastTestTime;
    
    void Start()
    {
        // 查找编辑器组件
        editor2D = FindObjectOfType<SheepLevelEditor2D>();
        
        if (editor2D == null)
        {
            Debug.LogError("未找到编辑器组件！请确保场景中有SheepLevelEditor2D组件。");
            return;
        }
        
        Debug.Log("层级遮罩测试脚本已启动");
        lastTestTime = Time.time;
    }
    
    void Update()
    {
        if (!testLayerMasks) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunLayerMaskTest();
            lastTestTime = Time.time;
        }
    }
    
    void RunLayerMaskTest()
    {
        Debug.Log("=== 层级遮罩功能测试 ===");
        
        // 测试2D编辑器
        if (editor2D != null)
        {
            Test2DEditorLayerMasks();
        }
        
        Debug.Log("=== 测试完成 ===");
    }
    
    void Test2DEditorLayerMasks()
    {
        Debug.Log("测试2D编辑器层级遮罩...");
        
        // 检查层级遮罩设置
        Debug.Log($"2D编辑器 - 显示层级遮罩: {editor2D.showLayerMasks}");
        Debug.Log($"2D编辑器 - 遮罩颜色: {editor2D.layerMaskColor}");
        Debug.Log($"2D编辑器 - 遮罩高度: {editor2D.layerMaskHeight}");
        Debug.Log($"2D编辑器 - 当前层级: {editor2D.selectedLayer}");
        Debug.Log($"2D编辑器 - 总层数: {editor2D.totalLayers}");
        
        // 测试层级切换
        int originalLayer = editor2D.selectedLayer;
        int newLayer = (originalLayer + 1) % editor2D.totalLayers;
        
        Debug.Log($"切换2D编辑器层级: {originalLayer} -> {newLayer}");
        editor2D.selectedLayer = newLayer;
        editor2D.UpdateCardDisplay();
        
        // 恢复原层级
        editor2D.selectedLayer = originalLayer;
        editor2D.UpdateCardDisplay();
        
        Debug.Log("2D编辑器层级遮罩测试完成");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("层级遮罩测试", GUI.skin.box);
        
        testLayerMasks = GUILayout.Toggle(testLayerMasks, "启用自动测试");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunLayerMaskTest();
        }
        
        if (GUILayout.Button("切换2D层级"))
        {
            if (editor2D != null)
            {
                editor2D.selectedLayer = (editor2D.selectedLayer + 1) % editor2D.totalLayers;
                editor2D.UpdateCardDisplay();
                Debug.Log($"2D编辑器层级已切换到: {editor2D.selectedLayer}");
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 