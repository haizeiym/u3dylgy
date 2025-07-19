using UnityEngine;

public class PlaceableAreaAlignmentTest : MonoBehaviour
{
    [Header("可放置区域对齐测试")]
    public bool runAlignmentTest = true;
    public float testInterval = 5f;
    
    private SheepLevelEditor2D levelEditor;
    private PlaceableAreaVisualizer placeableAreaVisualizer;
    private float lastTestTime;
    
    void Start()
    {
        // 查找编辑器组件
        levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        placeableAreaVisualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        
        if (levelEditor == null)
        {
            Debug.LogError("未找到编辑器组件！请确保场景中有SheepLevelEditor2D组件。");
            return;
        }
        
        Debug.Log("可放置区域对齐测试脚本已启动");
        lastTestTime = Time.time;
    }
    
    void Update()
    {
        if (!runAlignmentTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunAlignmentTest();
            lastTestTime = Time.time;
        }
    }
    
    void RunAlignmentTest()
    {
        Debug.Log("=== 可放置区域对齐测试 ===");
        
        // 测试1: 检查区域大小计算
        TestAreaSizeCalculation();
        
        // 测试2: 检查可放置区域可视化
        TestPlaceableAreaVisualization();
        
        // 测试3: 检查网格对齐
        TestGridAlignment();
        
        // 测试4: 检查卡片放置边界
        TestCardPlacementBounds();
        
        Debug.Log("=== 对齐测试完成 ===");
    }
    
    void TestAreaSizeCalculation()
    {
        if (levelEditor != null)
        {
            Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
            Debug.Log($"✓ 实际区域大小: {actualAreaSize.x:F2} x {actualAreaSize.y:F2}");
            
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
    
    void TestPlaceableAreaVisualization()
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
                Debug.LogError("✗ 可放置区域对象不存在");
            }
        }
        else
        {
            Debug.LogWarning("⚠ PlaceableAreaVisualizer组件不存在");
        }
    }
    
    void TestGridAlignment()
    {
        if (levelEditor != null)
        {
            Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
            Vector2 gridStart = new Vector2(-actualAreaSize.x * 0.5f, -actualAreaSize.y * 0.5f);
            
            Debug.Log($"网格起始位置: {gridStart}");
            Debug.Log($"网格结束位置: {gridStart + actualAreaSize}");
            
            // 测试几个关键位置的网格对齐
            Vector2[] testPositions = {
                gridStart, // 左下角
                gridStart + new Vector2(actualAreaSize.x, 0), // 右下角
                gridStart + new Vector2(0, actualAreaSize.y), // 左上角
                gridStart + actualAreaSize, // 右上角
                gridStart + actualAreaSize * 0.5f // 中心
            };
            
            foreach (Vector2 pos in testPositions)
            {
                Vector2 snappedPos = levelEditor.SnapToGrid2D(pos);
                float distance = Vector2.Distance(pos, snappedPos);
                
                if (distance < 0.01f)
                {
                    Debug.Log($"✓ 位置 {pos} 网格对齐正确");
                }
                else
                {
                    Debug.LogWarning($"⚠ 位置 {pos} 网格对齐偏差: {distance:F3}");
                }
            }
        }
    }
    
    void TestCardPlacementBounds()
    {
        if (levelEditor != null)
        {
            Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
            Vector2 gridStart = new Vector2(-actualAreaSize.x * 0.5f, -actualAreaSize.y * 0.5f);
            
            // 测试边界内外的位置
            Vector2[] insidePositions = {
                gridStart + new Vector2(0.1f, 0.1f),
                gridStart + actualAreaSize * 0.5f,
                gridStart + actualAreaSize - new Vector2(0.1f, 0.1f)
            };
            
            Vector2[] outsidePositions = {
                gridStart - new Vector2(0.1f, 0.1f),
                gridStart + actualAreaSize + new Vector2(0.1f, 0.1f),
                gridStart + new Vector2(actualAreaSize.x + 0.5f, 0.5f)
            };
            
            Debug.Log("测试边界内位置:");
            foreach (Vector2 pos in insidePositions)
            {
                bool inBounds = levelEditor.IsPositionInGridBounds2D(pos);
                if (inBounds)
                {
                    Debug.Log($"✓ 位置 {pos} 在边界内");
                }
                else
                {
                    Debug.LogWarning($"⚠ 位置 {pos} 被错误判断为边界外");
                }
            }
            
            Debug.Log("测试边界外位置:");
            foreach (Vector2 pos in outsidePositions)
            {
                bool inBounds = levelEditor.IsPositionInGridBounds2D(pos);
                if (!inBounds)
                {
                    Debug.Log($"✓ 位置 {pos} 在边界外");
                }
                else
                {
                    Debug.LogWarning($"⚠ 位置 {pos} 被错误判断为边界内");
                }
            }
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("可放置区域对齐测试", GUI.skin.box);
        
        runAlignmentTest = GUILayout.Toggle(runAlignmentTest, "启用自动测试");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunAlignmentTest();
        }
        
        if (GUILayout.Button("调整网格大小"))
        {
            if (levelEditor != null)
            {
                levelEditor.gridSize = new Vector2(Random.Range(6, 12), Random.Range(6, 12));
                levelEditor.cardSpacing = Random.Range(1.0f, 1.5f);
                levelEditor.UpdateGridAndMasks();
                Debug.Log($"网格已调整为: {levelEditor.gridSize}, 间距: {levelEditor.cardSpacing}");
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 