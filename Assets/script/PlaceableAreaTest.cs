using UnityEngine;

public class PlaceableAreaTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTest = false;
    public KeyCode testKey = KeyCode.T;
    
    private SheepLevelEditor2D editor2D;
    private PlaceableAreaVisualizer visualizer;
    
    void Start()
    {
        // 查找编辑器组件
        editor2D = FindObjectOfType<SheepLevelEditor2D>();
        visualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        
        Debug.Log("可放置区域测试脚本已启动");
        Debug.Log($"找到2D编辑器: {editor2D != null}");
        Debug.Log($"找到可视化组件: {visualizer != null}");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            RunPlaceableAreaTest();
        }
    }
    
    public void RunPlaceableAreaTest()
    {
        Debug.Log("=== 开始可放置区域可视化测试 ===");
        
        // 测试1: 检查可视化组件是否存在
        TestVisualizerExists();
        
        // 测试2: 测试显示/隐藏功能
        TestVisibilityToggle();
        
        // 测试3: 测试颜色更新
        TestColorUpdate();
        
        // 测试4: 测试网格大小变化
        TestGridSizeChange();
        
        Debug.Log("=== 可放置区域可视化测试完成 ===");
    }
    
    void TestVisualizerExists()
    {
        Debug.Log("测试1: 检查可视化组件");
        
        if (visualizer == null)
        {
            Debug.LogError("❌ 未找到PlaceableAreaVisualizer组件");
            return;
        }
        
        Debug.Log("✅ PlaceableAreaVisualizer组件存在");
        
        // 检查编辑器引用
        if (editor2D != null)
        {
            Debug.Log($"✅ 2D编辑器引用: {editor2D.placeableAreaVisualizer != null}");
        }
    }
    
    void TestVisibilityToggle()
    {
        Debug.Log("测试2: 测试显示/隐藏功能");
        
        if (visualizer == null) return;
        
        // 测试隐藏
        visualizer.SetVisible(false);
        Debug.Log("✅ 已设置可视化为隐藏状态");
        
        // 等待一帧
        StartCoroutine(DelayedTest(() => {
            // 测试显示
            visualizer.SetVisible(true);
            Debug.Log("✅ 已设置可视化为显示状态");
        }));
    }
    
    void TestColorUpdate()
    {
        Debug.Log("测试3: 测试颜色更新");
        
        if (visualizer == null) return;
        
        // 测试不同颜色
        Color testAreaColor = new Color(1f, 0f, 0f, 0.5f); // 红色半透明
        Color testBorderColor = new Color(1f, 1f, 0f, 0.8f); // 黄色边框
        Color testGridColor = new Color(0f, 1f, 1f, 0.6f); // 青色网格
        
        visualizer.UpdateColors(testAreaColor, testBorderColor, testGridColor);
        Debug.Log("✅ 已更新可视化颜色");
        
        // 恢复默认颜色
        StartCoroutine(DelayedTest(() => {
            Color defaultAreaColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
            Color defaultBorderColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            Color defaultGridColor = new Color(0.3f, 0.7f, 0.3f, 0.5f);
            
            visualizer.UpdateColors(defaultAreaColor, defaultBorderColor, defaultGridColor);
            Debug.Log("✅ 已恢复默认颜色");
        }));
    }
    
    void TestGridSizeChange()
    {
        Debug.Log("测试4: 测试网格大小变化");
        
        if (editor2D != null)
        {
            // 测试2D编辑器网格大小变化
            Vector2 originalSize = editor2D.gridSize;
            Vector2 newSize = new Vector2(12, 12);
            
            editor2D.gridSize = newSize;
            editor2D.UpdateGridAndMasks();
            Debug.Log($"✅ 2D编辑器网格大小已更改为: {newSize}");
            
            // 恢复原始大小
            StartCoroutine(DelayedTest(() => {
                editor2D.gridSize = originalSize;
                editor2D.UpdateGridAndMasks();
                Debug.Log($"✅ 2D编辑器网格大小已恢复为: {originalSize}");
            }));
        }
        

    }
    
    System.Collections.IEnumerator DelayedTest(System.Action action)
    {
        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 100));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("可放置区域测试", GUI.skin.box);
        GUILayout.Label($"按 {testKey} 运行测试");
        
        if (GUILayout.Button("运行测试"))
        {
            RunPlaceableAreaTest();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 