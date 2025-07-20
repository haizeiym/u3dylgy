using UnityEngine;
using System.Collections.Generic;
using YangLeGeYang2D.LevelEditor;

/// <summary>
/// 高级关卡编辑器示例
/// 演示羊了个羊2D关卡编辑器的高级功能
/// </summary>
public class AdvancedLevelEditorExample : MonoBehaviour
{
    [Header("编辑器引用")]
    public SheepLevelEditor2D levelEditor;
    public PlaceableAreaVisualizer visualizer;
    
    [Header("高级设置")]
    public bool enableCustomAreaSize = true;
    public Vector2 customAreaSize = new Vector2(12, 12);
    public bool enableLayerPreview = true;
    public bool enableGridLabels = true;
    public bool enablePerformanceMode = true;
    
    [Header("自定义颜色")]
    public Color customAreaColor = new Color(0.2f, 0.6f, 1f, 0.3f);
    public Color customBorderColor = new Color(0.2f, 0.6f, 1f, 0.8f);
    public Color customGridColor = new Color(0.4f, 0.8f, 1f, 0.5f);
    public Color customNormalLayerColor = Color.white;
    public Color customGrayedLayerColor = new Color(0.3f, 0.3f, 0.3f, 0.7f);
    
    [Header("性能设置")]
    public int customTextureSize = 128;
    public float customInputThrottle = 0.05f;
    public float customCameraSpeed = 2f;
    public float customZoomSpeed = 2f;
    
    private List<CardData2D> complexLevelCards = new List<CardData2D>();
    
    void Start()
    {
        SetupAdvancedExample();
    }
    
    /// <summary>
    /// 设置高级示例
    /// </summary>
    public void SetupAdvancedExample()
    {
        if (levelEditor == null)
        {
            levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (levelEditor == null)
        {
            Debug.LogError("未找到SheepLevelEditor2D组件！");
            return;
        }
        
        // 配置高级设置
        ConfigureAdvancedSettings();
        
        // 设置可视化
        ConfigureVisualization();
        
        // 创建复杂关卡
        CreateComplexLevel();
        
        Debug.Log("高级示例设置完成！");
    }
    
    /// <summary>
    /// 配置高级设置
    /// </summary>
    void ConfigureAdvancedSettings()
    {
        // 基本设置
        levelEditor.gridSize = new Vector2(10, 10);
        levelEditor.cardSpacing = 1.0f;
        levelEditor.cardSize = 0.9f;
        levelEditor.totalLayers = 5;
        
        // 自定义区域大小
        if (enableCustomAreaSize)
        {
            levelEditor.useCustomAreaSize = true;
            levelEditor.areaSize = customAreaSize;
        }
        
        // 层级预览
        levelEditor.enableLayerPreview = enableLayerPreview;
        levelEditor.normalLayerColor = customNormalLayerColor;
        levelEditor.grayedLayerColor = customGrayedLayerColor;
        
        // 网格标签
        levelEditor.showGridNumbers = enableGridLabels;
        levelEditor.showGridCoordinates = enableGridLabels;
        
        // 性能设置
        if (enablePerformanceMode)
        {
            levelEditor.highPerformance = true;
            levelEditor.textureSize = customTextureSize;
            levelEditor.inputThrottle = customInputThrottle;
            levelEditor.cameraSpeed = customCameraSpeed;
            levelEditor.zoomSpeed = customZoomSpeed;
        }
        
        // 调试设置
        levelEditor.showCardIDs = true;
        levelEditor.showCardTypes = true;
        levelEditor.showFPS = true;
        levelEditor.showMemory = true;
    }
    
    /// <summary>
    /// 配置可视化
    /// </summary>
    void ConfigureVisualization()
    {
        if (visualizer == null)
        {
            visualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        }
        
        if (visualizer != null)
        {
            // 设置自定义颜色
            visualizer.placeableAreaColor = customAreaColor;
            visualizer.borderColor = customBorderColor;
            visualizer.gridLineColor = customGridColor;
            visualizer.textureSize = customTextureSize;
            
            // 更新颜色
            visualizer.UpdateColors(customAreaColor, customBorderColor, customGridColor);
        }
    }
    
    /// <summary>
    /// 创建复杂关卡
    /// </summary>
    public void CreateComplexLevel()
    {
        if (levelEditor == null) return;
        
        // 清除现有卡片
        levelEditor.ClearAllCards();
        complexLevelCards.Clear();
        
        // 创建复杂的多层关卡
        CreateMultiLayerLevel();
        
        // 添加所有卡片
        foreach (var card in complexLevelCards)
        {
            levelEditor.AddCardData(card);
        }
        
        // 保存关卡
        levelEditor.SaveLevel();
        
        Debug.Log($"复杂关卡创建完成！共创建 {complexLevelCards.Count} 张卡片");
    }
    
    /// <summary>
    /// 创建多层关卡
    /// </summary>
    void CreateMultiLayerLevel()
    {
        int cardId = 1;
        
        // 第一层：基础层
        CreateLayer(cardId, 0, 0, 8, 8, 0);
        cardId += 64;
        
        // 第二层：部分覆盖
        CreateLayer(cardId, 1, 1, 6, 6, 1);
        cardId += 36;
        
        // 第三层：更小区域
        CreateLayer(cardId, 2, 2, 4, 4, 2);
        cardId += 16;
        
        // 第四层：中心区域
        CreateLayer(cardId, 3, 3, 2, 2, 3);
        cardId += 4;
        
        // 第五层：单张卡片
        CardData2D centerCard = new CardData2D
        {
            id = cardId,
            type = Random.Range(0, 8),
            position = new Vector2(3.5f * levelEditor.cardSpacing, 3.5f * levelEditor.cardSpacing),
            layer = 4
        };
        complexLevelCards.Add(centerCard);
    }
    
    /// <summary>
    /// 创建单层卡片
    /// </summary>
    void CreateLayer(int startId, int startX, int startY, int width, int height, int layer)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CardData2D card = new CardData2D
                {
                    id = startId + x * height + y,
                    type = Random.Range(0, 8),
                    position = new Vector2((startX + x) * levelEditor.cardSpacing, (startY + y) * levelEditor.cardSpacing),
                    layer = layer
                };
                
                complexLevelCards.Add(card);
            }
        }
    }
    
    /// <summary>
    /// 创建特殊图案关卡
    /// </summary>
    public void CreatePatternLevel()
    {
        if (levelEditor == null) return;
        
        levelEditor.ClearAllCards();
        complexLevelCards.Clear();
        
        // 创建心形图案
        CreateHeartPattern();
        
        // 添加所有卡片
        foreach (var card in complexLevelCards)
        {
            levelEditor.AddCardData(card);
        }
        
        levelEditor.SaveLevel();
        Debug.Log("心形图案关卡创建完成！");
    }
    
    /// <summary>
    /// 创建心形图案
    /// </summary>
    void CreateHeartPattern()
    {
        int cardId = 1;
        
        // 心形图案的坐标点（简化版）
        Vector2[] heartPoints = {
            new Vector2(4, 6), new Vector2(5, 6), new Vector2(6, 6),
            new Vector2(3, 5), new Vector2(4, 5), new Vector2(5, 5), new Vector2(6, 5), new Vector2(7, 5),
            new Vector2(2, 4), new Vector2(3, 4), new Vector2(4, 4), new Vector2(5, 4), new Vector2(6, 4), new Vector2(7, 4), new Vector2(8, 4),
            new Vector2(2, 3), new Vector2(3, 3), new Vector2(4, 3), new Vector2(5, 3), new Vector2(6, 3), new Vector2(7, 3), new Vector2(8, 3),
            new Vector2(3, 2), new Vector2(4, 2), new Vector2(5, 2), new Vector2(6, 2), new Vector2(7, 2),
            new Vector2(4, 1), new Vector2(5, 1), new Vector2(6, 1),
            new Vector2(5, 0)
        };
        
        foreach (var point in heartPoints)
        {
            CardData2D card = new CardData2D
            {
                id = cardId++,
                type = Random.Range(0, 8),
                position = new Vector2(point.x * levelEditor.cardSpacing, point.y * levelEditor.cardSpacing),
                layer = 0
            };
            
            complexLevelCards.Add(card);
        }
    }
    
    /// <summary>
    /// 性能测试
    /// </summary>
    public void PerformanceTest()
    {
        if (levelEditor == null) return;
        
        Debug.Log("开始性能测试...");
        
        // 测试大量卡片创建
        levelEditor.ClearAllCards();
        complexLevelCards.Clear();
        
        int testCardCount = 1000;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        
        for (int i = 0; i < testCardCount; i++)
        {
            CardData2D card = new CardData2D
            {
                id = i + 1,
                type = Random.Range(0, 8),
                position = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)),
                layer = Random.Range(0, 5)
            };
            
            levelEditor.AddCardData(card);
        }
        
        stopwatch.Stop();
        
        Debug.Log($"性能测试完成！创建 {testCardCount} 张卡片耗时: {stopwatch.ElapsedMilliseconds}ms");
        Debug.Log($"平均每张卡片: {stopwatch.ElapsedMilliseconds / (float)testCardCount:F2}ms");
    }
    
    /// <summary>
    /// 导出测试
    /// </summary>
    public void ExportTest()
    {
        if (levelEditor == null) return;
        
        Debug.Log("开始导出测试...");
        
        // 设置导出选项
        levelEditor.exportJSON = true;
        levelEditor.exportXML = true;
        levelEditor.exportBinary = true;
        
        // 执行导出
        levelEditor.ExportLevel();
        
        Debug.Log("导出测试完成！");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 220, 300, 300));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("高级示例控制", GUI.skin.box);
        
        if (GUILayout.Button("设置高级示例"))
        {
            SetupAdvancedExample();
        }
        
        if (GUILayout.Button("创建复杂关卡"))
        {
            CreateComplexLevel();
        }
        
        if (GUILayout.Button("创建心形图案"))
        {
            CreatePatternLevel();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("性能测试"))
        {
            PerformanceTest();
        }
        
        if (GUILayout.Button("导出测试"))
        {
            ExportTest();
        }
        
        GUILayout.Space(10);
        
        // 实时设置调整
        GUILayout.Label("实时设置调整");
        
        if (levelEditor != null)
        {
            levelEditor.enableLayerPreview = GUILayout.Toggle(levelEditor.enableLayerPreview, "层级预览");
            levelEditor.showGridNumbers = GUILayout.Toggle(levelEditor.showGridNumbers, "网格编号");
            levelEditor.showGridCoordinates = GUILayout.Toggle(levelEditor.showGridCoordinates, "网格坐标");
            levelEditor.showFPS = GUILayout.Toggle(levelEditor.showFPS, "显示FPS");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 