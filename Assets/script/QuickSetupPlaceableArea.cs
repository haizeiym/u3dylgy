using UnityEngine;

public class QuickSetupPlaceableArea : MonoBehaviour
{
    [Header("快速设置选项")]
    public bool setup2DEditor = true;
    public bool addTestScript = true;
    
    [Header("可视化设置")]
    public bool showPlaceableArea = true;
    public Color areaColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
    public Color borderColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
    public Color gridColor = new Color(0.3f, 0.7f, 0.3f, 0.5f);
    
    void Start()
    {
        Debug.Log("=== 开始快速设置可放置区域可视化 ===");
        
        // 设置2D编辑器
        if (setup2DEditor)
        {
            Setup2DEditor();
        }
        
        // 添加测试脚本
        if (addTestScript)
        {
            AddTestScript();
        }
        
        Debug.Log("=== 可放置区域可视化快速设置完成 ===");
    }
    
    void Setup2DEditor()
    {
        Debug.Log("设置2D编辑器...");
        
        SheepLevelEditor2D editor2D = FindObjectOfType<SheepLevelEditor2D>();
        if (editor2D == null)
        {
            Debug.LogWarning("未找到2D编辑器，跳过2D编辑器设置");
            return;
        }
        
        // 设置可放置区域可视化
        editor2D.showPlaceableArea = showPlaceableArea;
        
        // 查找或创建可视化组件
        PlaceableAreaVisualizer visualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        if (visualizer == null)
        {
            GameObject visualizerObj = new GameObject("PlaceableAreaVisualizer");
            visualizer = visualizerObj.AddComponent<PlaceableAreaVisualizer>();
        }
        
        // 设置可视化参数
        visualizer.showPlaceableArea = showPlaceableArea;
        visualizer.placeableAreaColor = areaColor;
        visualizer.borderColor = borderColor;
        visualizer.gridLineColor = gridColor;
        
        // 关联编辑器
        editor2D.placeableAreaVisualizer = visualizer;
        
        // 更新编辑器
        editor2D.UpdateGridAndMasks();
        
        Debug.Log("✅ 2D编辑器可放置区域可视化设置完成");
    }
    

    
    void AddTestScript()
    {
        Debug.Log("添加测试脚本...");
        
        // 检查是否已有测试脚本
        PlaceableAreaTest testScript = FindObjectOfType<PlaceableAreaTest>();
        if (testScript != null)
        {
            Debug.Log("测试脚本已存在，跳过添加");
            return;
        }
        
        // 创建测试脚本
        GameObject testObj = new GameObject("PlaceableAreaTest");
        testScript = testObj.AddComponent<PlaceableAreaTest>();
        
        Debug.Log("✅ 测试脚本添加完成");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 150, 240, 140));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("可放置区域快速设置", GUI.skin.box);
        GUILayout.Label("设置已完成！");
        
        if (GUILayout.Button("重新设置"))
        {
            Setup2DEditor();
        }
        
        if (GUILayout.Button("运行测试"))
        {
            PlaceableAreaTest testScript = FindObjectOfType<PlaceableAreaTest>();
            if (testScript != null)
            {
                testScript.RunPlaceableAreaTest();
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 