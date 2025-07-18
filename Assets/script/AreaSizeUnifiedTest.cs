using UnityEngine;

public class AreaSizeUnifiedTest : MonoBehaviour
{
    [Header("区域大小统一测试")]
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
            RunAreaSizeUnifiedTest();
        }
    }
    
    void Update()
    {
        if (runTest && Input.GetKeyDown(KeyCode.U))
        {
            RunAreaSizeUnifiedTest();
        }
    }
    
    void RunAreaSizeUnifiedTest()
    {
        Debug.Log("=== 开始区域大小统一测试 ===");
        
        // 测试1: 检查区域大小计算
        TestAreaSizeCalculation();
        
        // 测试2: 检查网格背景大小
        TestGridBackgroundSize();
        
        // 测试3: 检查层级遮罩大小
        TestLayerMaskSizes();
        
        // 测试4: 检查可放置区域大小
        TestPlaceableAreaSize();
        
        // 测试5: 测试自定义区域大小
        TestCustomAreaSize();
        
        // 测试6: 测试区域大小同步
        TestAreaSizeSynchronization();
        
        Debug.Log("=== 区域大小统一测试完成 ===");
    }
    
    void TestAreaSizeCalculation()
    {
        if (levelEditor != null)
        {
            Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
            Debug.Log($"✓ 实际区域大小计算: {actualAreaSize.x:F2} x {actualAreaSize.y:F2}");
            
            if (levelEditor.useCustomAreaSize)
            {
                Debug.Log($"✓ 使用自定义区域大小: {levelEditor.areaSize.x:F2} x {levelEditor.areaSize.y:F2}");
            }
            else
            {
                Vector2 calculatedSize = new Vector2((levelEditor.gridSize.x - 1) * levelEditor.cardSpacing, 
                                                   (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing);
                Debug.Log($"✓ 使用计算区域大小: {calculatedSize.x:F2} x {calculatedSize.y:F2}");
            }
        }
    }
    
    void TestGridBackgroundSize()
    {
        if (gridBackground != null)
        {
            Vector3 gridScale = gridBackground.transform.localScale;
            Vector2 expectedSize = levelEditor.GetActualAreaSize();
            
            Debug.Log($"网格背景实际大小: {gridScale.x:F2} x {gridScale.y:F2}");
            Debug.Log($"期望大小: {expectedSize.x:F2} x {expectedSize.y:F2}");
            
            float widthDiff = Mathf.Abs(gridScale.x - expectedSize.x);
            float heightDiff = Mathf.Abs(gridScale.y - expectedSize.y);
            
            if (widthDiff < 0.01f && heightDiff < 0.01f)
            {
                Debug.Log("✓ 网格背景大小正确");
            }
            else
            {
                Debug.LogWarning($"⚠ 网格背景大小不匹配，差异: 宽度{widthDiff:F3}, 高度{heightDiff:F3}");
            }
        }
        else
        {
            Debug.LogError("✗ 网格背景对象不存在");
        }
    }
    
    void TestLayerMaskSizes()
    {
        if (levelEditor != null)
        {
            Vector2 expectedSize = levelEditor.GetActualAreaSize();
            bool allMasksCorrect = true;
            
            // 查找所有层级遮罩对象
            GameObject[] layerMasks = GameObject.FindGameObjectsWithTag("LayerMask");
            if (layerMasks.Length == 0)
            {
                // 如果没有标签，尝试通过名称查找
                layerMasks = GameObject.FindObjectsOfType<GameObject>()
                    .Where(obj => obj.name.StartsWith("LayerMask_"))
                    .ToArray();
            }
            
            Debug.Log($"找到 {layerMasks.Length} 个层级遮罩对象");
            
            foreach (GameObject mask in layerMasks)
            {
                Vector3 maskScale = mask.transform.localScale;
                float widthDiff = Mathf.Abs(maskScale.x - expectedSize.x);
                float heightDiff = Mathf.Abs(maskScale.y - expectedSize.y);
                
                if (widthDiff > 0.01f || heightDiff > 0.01f)
                {
                    Debug.LogWarning($"⚠ 层级遮罩 {mask.name} 大小不匹配: {maskScale.x:F2} x {maskScale.y:F2}");
                    allMasksCorrect = false;
                }
            }
            
            if (allMasksCorrect)
            {
                Debug.Log("✓ 所有层级遮罩大小正确");
            }
        }
    }
    
    void TestPlaceableAreaSize()
    {
        if (placeableAreaVisualizer != null)
        {
            // 查找可放置区域对象
            GameObject placeableArea = GameObject.Find("PlaceableArea");
            if (placeableArea != null)
            {
                Vector3 areaScale = placeableArea.transform.localScale;
                Vector2 expectedSize = levelEditor.GetActualAreaSize();
                
                Debug.Log($"可放置区域实际大小: {areaScale.x:F2} x {areaScale.y:F2}");
                Debug.Log($"期望大小: {expectedSize.x:F2} x {expectedSize.y:F2}");
                
                float widthDiff = Mathf.Abs(areaScale.x - expectedSize.x);
                float heightDiff = Mathf.Abs(areaScale.y - expectedSize.y);
                
                if (widthDiff < 0.01f && heightDiff < 0.01f)
                {
                    Debug.Log("✓ 可放置区域大小正确");
                }
                else
                {
                    Debug.LogWarning($"⚠ 可放置区域大小不匹配，差异: 宽度{widthDiff:F3}, 高度{heightDiff:F3}");
                }
            }
            else
            {
                Debug.LogWarning("⚠ 可放置区域对象不存在");
            }
        }
        else
        {
            Debug.LogWarning("⚠ PlaceableAreaVisualizer组件不存在");
        }
    }
    
    void TestCustomAreaSize()
    {
        if (levelEditor != null)
        {
            Debug.Log("测试自定义区域大小功能...");
            
            // 保存原始设置
            bool originalUseCustom = levelEditor.useCustomAreaSize;
            Vector2 originalAreaSize = levelEditor.areaSize;
            
            // 测试启用自定义区域大小
            levelEditor.useCustomAreaSize = true;
            levelEditor.areaSize = new Vector2(10f, 8f);
            levelEditor.UpdateGridAndMasks();
            
            Vector2 newAreaSize = levelEditor.GetActualAreaSize();
            Debug.Log($"自定义区域大小: {newAreaSize.x:F2} x {newAreaSize.y:F2}");
            
            if (Mathf.Abs(newAreaSize.x - 10f) < 0.01f && Mathf.Abs(newAreaSize.y - 8f) < 0.01f)
            {
                Debug.Log("✓ 自定义区域大小设置成功");
            }
            else
            {
                Debug.LogError("✗ 自定义区域大小设置失败");
            }
            
            // 恢复原始设置
            levelEditor.useCustomAreaSize = originalUseCustom;
            levelEditor.areaSize = originalAreaSize;
            levelEditor.UpdateGridAndMasks();
            
            Debug.Log("✓ 自定义区域大小测试完成");
        }
    }
    
    void TestAreaSizeSynchronization()
    {
        if (levelEditor != null)
        {
            Debug.Log("测试区域大小同步功能...");
            
            // 保存原始设置
            Vector2 originalGridSize = levelEditor.gridSize;
            float originalCardSpacing = levelEditor.cardSpacing;
            
            // 测试网格大小变化
            levelEditor.gridSize = new Vector2(10, 6);
            levelEditor.UpdateGridAndMasks();
            
            Vector2 areaSize1 = levelEditor.GetActualAreaSize();
            Debug.Log($"网格大小变化后区域大小: {areaSize1.x:F2} x {areaSize1.y:F2}");
            
            // 测试卡片间距变化
            levelEditor.cardSpacing = 1.5f;
            levelEditor.UpdateGridAndMasks();
            
            Vector2 areaSize2 = levelEditor.GetActualAreaSize();
            Debug.Log($"卡片间距变化后区域大小: {areaSize2.x:F2} x {areaSize2.y:F2}");
            
            // 检查网格背景是否同步更新
            if (gridBackground != null)
            {
                Vector3 gridScale = gridBackground.transform.localScale;
                float widthDiff = Mathf.Abs(gridScale.x - areaSize2.x);
                float heightDiff = Mathf.Abs(gridScale.y - areaSize2.y);
                
                if (widthDiff < 0.01f && heightDiff < 0.01f)
                {
                    Debug.Log("✓ 区域大小同步更新成功");
                }
                else
                {
                    Debug.LogWarning("⚠ 区域大小同步更新失败");
                }
            }
            
            // 恢复原始设置
            levelEditor.gridSize = originalGridSize;
            levelEditor.cardSpacing = originalCardSpacing;
            levelEditor.UpdateGridAndMasks();
            
            Debug.Log("✓ 区域大小同步测试完成");
        }
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 300, 10, 290, 250));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("区域大小统一测试");
            GUILayout.Space(5);
            
            if (GUILayout.Button("运行测试 (U)"))
            {
                RunAreaSizeUnifiedTest();
            }
            
            if (levelEditor != null)
            {
                Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
                GUILayout.Label($"实际区域大小: {actualAreaSize.x:F2} x {actualAreaSize.y:F2}");
                GUILayout.Label($"使用自定义: {levelEditor.useCustomAreaSize}");
                
                if (levelEditor.useCustomAreaSize)
                {
                    GUILayout.Label($"自定义大小: {levelEditor.areaSize.x:F2} x {levelEditor.areaSize.y:F2}");
                }
                else
                {
                    GUILayout.Label($"网格大小: {levelEditor.gridSize}");
                    GUILayout.Label($"卡片间距: {levelEditor.cardSpacing:F2}");
                }
                
                if (gridBackground != null)
                {
                    Vector3 gridScale = gridBackground.transform.localScale;
                    GUILayout.Label($"网格背景: {gridScale.x:F2} x {gridScale.y:F2}");
                }
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
} 