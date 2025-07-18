using UnityEngine;

public class GridBoundsDebugTest : MonoBehaviour
{
    [Header("调试设置")]
    public bool runTestOnStart = true;
    public bool showDebugInfo = true;
    
    [Header("测试位置")]
    public Vector2 testPosition = new Vector2(16.00f, -2.00f);
    
    private SheepLevelEditor2D editor;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunGridBoundsTest();
        }
    }
    
    void Update()
    {
        if (showDebugInfo && editor != null)
        {
            // 实时显示调试信息
            Vector2 actualAreaSize = editor.GetActualAreaSize();
            float halfAreaWidth = actualAreaSize.x * 0.5f;
            float halfAreaHeight = actualAreaSize.y * 0.5f;
            
            Debug.Log($"实时调试 - 区域大小: {actualAreaSize}, 边界: ±({halfAreaWidth}, {halfAreaHeight})");
        }
    }
    
    [ContextMenu("运行网格边界测试")]
    public void RunGridBoundsTest()
    {
        Debug.Log("=== 开始网格边界测试 ===");
        
        // 查找编辑器
        editor = FindObjectOfType<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.LogError("❌ 未找到SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("✅ 找到编辑器组件");
        
        // 测试1: 检查当前设置
        TestCurrentSettings();
        
        // 测试2: 测试特定位置
        TestSpecificPosition();
        
        // 测试3: 测试边界位置
        TestBoundaryPositions();
        
        // 测试4: 测试网格显示
        TestGridDisplay();
        
        Debug.Log("=== 网格边界测试完成 ===");
    }
    
    void TestCurrentSettings()
    {
        Debug.Log("--- 检查当前设置 ---");
        Debug.Log($"网格大小: {editor.gridSize}");
        Debug.Log($"卡片间距: {editor.cardSpacing}");
        Debug.Log($"卡片大小: {editor.cardSize}");
        Debug.Log($"使用自定义区域大小: {editor.useCustomAreaSize}");
        Debug.Log($"自定义区域大小: {editor.areaSize}");
        
        Vector2 actualAreaSize = editor.GetActualAreaSize();
        Debug.Log($"实际区域大小: {actualAreaSize}");
        
        float halfAreaWidth = actualAreaSize.x * 0.5f;
        float halfAreaHeight = actualAreaSize.y * 0.5f;
        Debug.Log($"边界范围: ±({halfAreaWidth}, {halfAreaHeight})");
        
        // 计算网格点数量
        int gridPointsX = Mathf.RoundToInt(editor.gridSize.x);
        int gridPointsY = Mathf.RoundToInt(editor.gridSize.y);
        Debug.Log($"网格点数: {gridPointsX} x {gridPointsY}");
        
        // 计算最大可放置位置
        float maxX = (gridPointsX - 1) * editor.cardSpacing * 0.5f;
        float maxY = (gridPointsY - 1) * editor.cardSpacing * 0.5f;
        Debug.Log($"最大可放置位置: ±({maxX}, {maxY})");
    }
    
    void TestSpecificPosition()
    {
        Debug.Log("--- 测试特定位置 ---");
        Debug.Log($"测试位置: {testPosition}");
        
        Vector2 actualAreaSize = editor.GetActualAreaSize();
        float halfAreaWidth = actualAreaSize.x * 0.5f;
        float halfAreaHeight = actualAreaSize.y * 0.5f;
        
        bool inBounds = Mathf.Abs(testPosition.x) <= halfAreaWidth && Mathf.Abs(testPosition.y) <= halfAreaHeight;
        
        Debug.Log($"区域大小: {actualAreaSize}");
        Debug.Log($"边界范围: ±({halfAreaWidth}, {halfAreaHeight})");
        Debug.Log($"X坐标检查: |{testPosition.x}| <= {halfAreaWidth} = {Mathf.Abs(testPosition.x) <= halfAreaWidth}");
        Debug.Log($"Y坐标检查: |{testPosition.y}| <= {halfAreaHeight} = {Mathf.Abs(testPosition.y) <= halfAreaHeight}");
        Debug.Log($"在边界内: {inBounds}");
        
        if (!inBounds)
        {
            if (Mathf.Abs(testPosition.x) > halfAreaWidth)
            {
                Debug.LogError($"❌ X坐标超出边界: {testPosition.x} > {halfAreaWidth}");
            }
            if (Mathf.Abs(testPosition.y) > halfAreaHeight)
            {
                Debug.LogError($"❌ Y坐标超出边界: {testPosition.y} > {halfAreaHeight}");
            }
        }
        else
        {
            Debug.Log("✅ 位置在边界内");
        }
    }
    
    void TestBoundaryPositions()
    {
        Debug.Log("--- 测试边界位置 ---");
        
        Vector2 actualAreaSize = editor.GetActualAreaSize();
        float halfAreaWidth = actualAreaSize.x * 0.5f;
        float halfAreaHeight = actualAreaSize.y * 0.5f;
        
        // 测试边界上的位置
        Vector2[] boundaryPositions = {
            new Vector2(halfAreaWidth, 0),           // 右边界
            new Vector2(-halfAreaWidth, 0),          // 左边界
            new Vector2(0, halfAreaHeight),          // 上边界
            new Vector2(0, -halfAreaHeight),         // 下边界
            new Vector2(halfAreaWidth, halfAreaHeight),   // 右上角
            new Vector2(-halfAreaWidth, -halfAreaHeight), // 左下角
        };
        
        foreach (Vector2 pos in boundaryPositions)
        {
            bool inBounds = Mathf.Abs(pos.x) <= halfAreaWidth && Mathf.Abs(pos.y) <= halfAreaHeight;
            Debug.Log($"边界位置 {pos}: 在边界内 = {inBounds}");
        }
        
        // 测试边界外的位置
        Vector2[] outsidePositions = {
            new Vector2(halfAreaWidth + 0.1f, 0),    // 右边界外
            new Vector2(-halfAreaWidth - 0.1f, 0),   // 左边界外
            new Vector2(0, halfAreaHeight + 0.1f),   // 上边界外
            new Vector2(0, -halfAreaHeight - 0.1f),  // 下边界外
        };
        
        foreach (Vector2 pos in outsidePositions)
        {
            bool inBounds = Mathf.Abs(pos.x) <= halfAreaWidth && Mathf.Abs(pos.y) <= halfAreaHeight;
            Debug.Log($"边界外位置 {pos}: 在边界内 = {inBounds}");
        }
    }
    
    void TestGridDisplay()
    {
        Debug.Log("--- 测试网格显示 ---");
        
        // 查找网格背景
        GameObject gridBackground = GameObject.Find("GridBackground");
        if (gridBackground != null)
        {
            Vector3 gridScale = gridBackground.transform.localScale;
            Vector3 gridPosition = gridBackground.transform.position;
            
            Debug.Log($"网格背景位置: {gridPosition}");
            Debug.Log($"网格背景缩放: {gridScale}");
            
            // 计算网格的实际显示范围
            float gridHalfWidth = gridScale.x * 0.5f;
            float gridHalfHeight = gridScale.y * 0.5f;
            
            Debug.Log($"网格显示范围: ±({gridHalfWidth}, {gridHalfHeight})");
            
            // 检查测试位置是否在网格显示范围内
            bool inGridDisplay = Mathf.Abs(testPosition.x) <= gridHalfWidth && Mathf.Abs(testPosition.y) <= gridHalfHeight;
            Debug.Log($"测试位置在网格显示范围内: {inGridDisplay}");
            
            if (!inGridDisplay)
            {
                Debug.LogWarning("⚠️ 位置超出网格显示范围，但可能在逻辑边界内");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到网格背景对象");
        }
        
        // 检查相机设置
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log($"相机位置: {mainCamera.transform.position}");
            Debug.Log($"相机缩放: {mainCamera.orthographicSize}");
            Debug.Log($"相机类型: {(mainCamera.orthographic ? "正交" : "透视")}");
        }
    }
    
    [ContextMenu("测试位置 (16, -2)")]
    public void TestPosition16Minus2()
    {
        testPosition = new Vector2(16.00f, -2.00f);
        TestSpecificPosition();
    }
    
    [ContextMenu("测试位置 (4, 4)")]
    public void TestPosition44()
    {
        testPosition = new Vector2(4.00f, 4.00f);
        TestSpecificPosition();
    }
    
    [ContextMenu("测试位置 (0, 0)")]
    public void TestPosition00()
    {
        testPosition = new Vector2(0.00f, 0.00f);
        TestSpecificPosition();
    }
    
    [ContextMenu("显示当前设置")]
    public void ShowCurrentSettings()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            TestCurrentSettings();
        }
        else
        {
            Debug.LogError("❌ 未找到编辑器组件");
        }
    }
    
    [ContextMenu("尝试放置测试位置")]
    public void TryPlaceTestPosition()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            Debug.Log($"尝试在位置 {testPosition} 放置卡片");
            
            // 检查位置是否在网格范围内
            if (editor.IsPositionInGridBounds2D(testPosition))
            {
                Debug.Log("✅ 位置在网格范围内，可以放置卡片");
                
                // 检查位置是否被占用
                if (!editor.IsPositionOccupied2D(testPosition))
                {
                    Debug.Log("✅ 位置未被占用，可以放置卡片");
                }
                else
                {
                    Debug.LogWarning("⚠️ 位置已被占用，无法放置卡片");
                }
            }
            else
            {
                Debug.LogError("❌ 位置超出网格范围，无法放置卡片");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到编辑器组件");
        }
    }
} 