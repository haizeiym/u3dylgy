using UnityEngine;

public class GridFillTest : MonoBehaviour
{
    [Header("网格测试设置")]
    public bool runTest = false;
    public bool showDebugInfo = true;
    
    private SheepLevelEditor2D levelEditor;
    private GameObject gridBackground;
    private PlaceableAreaVisualizer placeableAreaVisualizer;
    
    void Start()
    {
        levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        if (levelEditor == null)
        {
            Debug.LogError("找不到SheepLevelEditor2D组件！");
            return;
        }
        
        gridBackground = GameObject.Find("GridBackground");
        placeableAreaVisualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        
        if (runTest)
        {
            RunGridFillTest();
        }
    }
    
    void Update()
    {
        if (runTest && Input.GetKeyDown(KeyCode.T))
        {
            RunGridFillTest();
        }
    }
    
    void RunGridFillTest()
    {
        Debug.Log("=== 开始网格充满测试 ===");
        
        // 测试1: 检查网格背景是否存在
        TestGridBackgroundExists();
        
        // 测试2: 检查网格大小是否正确
        TestGridSize();
        
        // 测试3: 检查网格纹理密度
        TestGridTextureDensity();
        
        // 测试4: 检查网格与可放置区域的对齐
        TestGridAlignment();
        
        // 测试5: 动态调整测试
        TestDynamicAdjustment();
        
        Debug.Log("=== 网格充满测试完成 ===");
    }
    
    void TestGridBackgroundExists()
    {
        if (gridBackground != null)
        {
            Debug.Log("✓ 网格背景对象存在");
            
            SpriteRenderer gridRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (gridRenderer != null && gridRenderer.sprite != null)
            {
                Debug.Log($"✓ 网格精灵存在，大小: {gridRenderer.sprite.rect.width} x {gridRenderer.sprite.rect.height}");
            }
            else
            {
                Debug.LogError("✗ 网格精灵不存在或为空");
            }
        }
        else
        {
            Debug.LogError("✗ 网格背景对象不存在");
        }
    }
    
    void TestGridSize()
    {
        if (gridBackground != null && levelEditor != null)
        {
            Vector3 gridScale = gridBackground.transform.localScale;
            float expectedWidth = (levelEditor.gridSize.x - 1) * levelEditor.cardSpacing;
            float expectedHeight = (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing;
            
            Debug.Log($"网格实际大小: {gridScale.x:F2} x {gridScale.y:F2}");
            Debug.Log($"期望大小: {expectedWidth:F2} x {expectedHeight:F2}");
            
            float widthDiff = Mathf.Abs(gridScale.x - expectedWidth);
            float heightDiff = Mathf.Abs(gridScale.y - expectedHeight);
            
            if (widthDiff < 0.01f && heightDiff < 0.01f)
            {
                Debug.Log("✓ 网格大小正确");
            }
            else
            {
                Debug.LogWarning($"⚠ 网格大小不匹配，差异: 宽度{widthDiff:F3}, 高度{heightDiff:F3}");
            }
        }
    }
    
    void TestGridTextureDensity()
    {
        if (gridBackground != null)
        {
            SpriteRenderer gridRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (gridRenderer != null && gridRenderer.sprite != null)
            {
                Texture2D texture = gridRenderer.sprite.texture;
                if (texture != null)
                {
                    Debug.Log($"网格纹理大小: {texture.width} x {texture.height}");
                    
                    // 检查纹理分辨率是否足够
                    if (texture.width >= 64 && texture.height >= 64)
                    {
                        Debug.Log("✓ 网格纹理分辨率足够");
                    }
                    else
                    {
                        Debug.LogWarning("⚠ 网格纹理分辨率可能过低");
                    }
                }
            }
        }
        
        // 测试直接调用公共方法
        if (levelEditor != null)
        {
            try
            {
                // 测试网格纹理生成
                Sprite testSprite = levelEditor.CreateGridSprite();
                if (testSprite != null)
                {
                    Debug.Log($"✓ 直接网格纹理生成成功，大小: {testSprite.rect.width} x {testSprite.rect.height}");
                }
                
                // 测试网格线间距计算
                int spacing = levelEditor.CalculateGridLineSpacing(128);
                Debug.Log($"✓ 网格线间距计算成功: {spacing}");
                
                // 测试网格更新方法
                levelEditor.UpdateGridForCardSize();
                Debug.Log("✓ 网格更新方法调用成功");
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"✗ 公共方法访问失败: {e.Message}");
            }
        }
    }
    
    void TestGridAlignment()
    {
        if (levelEditor != null && placeableAreaVisualizer != null)
        {
            // 检查网格是否与可放置区域对齐
            Vector3 gridScale = gridBackground != null ? gridBackground.transform.localScale : Vector3.zero;
            
            Debug.Log($"网格覆盖区域: {gridScale.x:F2} x {gridScale.y:F2}");
            Debug.Log($"可放置区域: {(levelEditor.gridSize.x - 1) * levelEditor.cardSpacing:F2} x {(levelEditor.gridSize.y - 1) * levelEditor.cardSpacing:F2}");
            
            // 检查网格是否完全覆盖可放置区域
            float expectedWidth = (levelEditor.gridSize.x - 1) * levelEditor.cardSpacing;
            float expectedHeight = (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing;
            
            if (Mathf.Abs(gridScale.x - expectedWidth) < 0.01f && Mathf.Abs(gridScale.y - expectedHeight) < 0.01f)
            {
                Debug.Log("✓ 网格与可放置区域完美对齐");
            }
            else
            {
                Debug.LogWarning("⚠ 网格与可放置区域未对齐");
            }
        }
    }
    
    void TestDynamicAdjustment()
    {
        if (levelEditor != null)
        {
            Debug.Log("测试动态调整功能...");
            
            // 保存原始值
            Vector2 originalGridSize = levelEditor.gridSize;
            float originalCardSpacing = levelEditor.cardSpacing;
            float originalCardSize = levelEditor.cardSize;
            
            // 测试不同的网格大小
            TestGridAdjustment(new Vector2(6, 6), 1.0f, 0.6f, "小网格");
            TestGridAdjustment(new Vector2(12, 12), 1.5f, 1.0f, "大网格");
            TestGridAdjustment(originalGridSize, originalCardSpacing, originalCardSize, "恢复原始");
            
            Debug.Log("✓ 动态调整测试完成");
        }
    }
    
    void TestGridAdjustment(Vector2 gridSize, float cardSpacing, float cardSize, string testName)
    {
        if (levelEditor != null)
        {
            Debug.Log($"测试 {testName}: 网格{gridSize}, 间距{cardSpacing}, 大小{cardSize}");
            
            // 应用新设置
            levelEditor.gridSize = gridSize;
            levelEditor.cardSpacing = cardSpacing;
            levelEditor.cardSize = cardSize;
            
            // 触发更新
            levelEditor.UpdateGridAndMasks();
            levelEditor.UpdateGridForCardSize();
            
            // 等待一帧让更新生效
            StartCoroutine(CheckAdjustmentResult(testName));
        }
    }
    
    System.Collections.IEnumerator CheckAdjustmentResult(string testName)
    {
        yield return new WaitForEndOfFrame();
        
        if (gridBackground != null)
        {
            Vector3 gridScale = gridBackground.transform.localScale;
            float expectedWidth = (levelEditor.gridSize.x - 1) * levelEditor.cardSpacing;
            float expectedHeight = (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing;
            
            float widthDiff = Mathf.Abs(gridScale.x - expectedWidth);
            float heightDiff = Mathf.Abs(gridScale.y - expectedHeight);
            
            if (widthDiff < 0.01f && heightDiff < 0.01f)
            {
                Debug.Log($"✓ {testName} 调整成功");
            }
            else
            {
                Debug.LogWarning($"⚠ {testName} 调整失败，差异: 宽度{widthDiff:F3}, 高度{heightDiff:F3}");
            }
        }
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("网格充满测试");
            GUILayout.Space(5);
            
            if (GUILayout.Button("运行测试 (T)"))
            {
                RunGridFillTest();
            }
            
            if (levelEditor != null)
            {
                GUILayout.Label($"网格大小: {levelEditor.gridSize}");
                GUILayout.Label($"卡片间距: {levelEditor.cardSpacing:F2}");
                GUILayout.Label($"卡片大小: {levelEditor.cardSize:F2}");
                
                if (gridBackground != null)
                {
                    Vector3 gridScale = gridBackground.transform.localScale;
                    GUILayout.Label($"网格覆盖: {gridScale.x:F2} x {gridScale.y:F2}");
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
} 