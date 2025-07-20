using UnityEngine;
using YangLeGeYang2D.LevelEditor;

/// <summary>
/// 基础关卡编辑器示例
/// 演示如何使用羊了个羊2D关卡编辑器的基本功能
/// </summary>
public class BasicLevelEditorExample : MonoBehaviour
{
    [Header("编辑器引用")]
    public SheepLevelEditor2D levelEditor;
    public PlaceableAreaVisualizer visualizer;
    
    [Header("示例设置")]
    public bool autoSetup = true;
    public Vector2 exampleGridSize = new Vector2(8, 8);
    public float exampleCardSpacing = 1.2f;
    public float exampleCardSize = 0.8f;
    
    void Start()
    {
        if (autoSetup)
        {
            SetupBasicExample();
        }
    }
    
    /// <summary>
    /// 设置基础示例
    /// </summary>
    public void SetupBasicExample()
    {
        if (levelEditor == null)
        {
            levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (levelEditor == null)
        {
            Debug.LogError("未找到SheepLevelEditor2D组件！请确保场景中有该组件。");
            return;
        }
        
        // 配置基本设置
        levelEditor.gridSize = exampleGridSize;
        levelEditor.cardSpacing = exampleCardSpacing;
        levelEditor.cardSize = exampleCardSize;
        levelEditor.totalLayers = 3;
        
        // 创建新关卡
        levelEditor.NewLevel();
        
        // 设置可视化
        if (visualizer == null)
        {
            visualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        }
        
        if (visualizer != null)
        {
            // 设置自定义颜色
            visualizer.placeableAreaColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
            visualizer.borderColor = new Color(0.2f, 0.8f, 0.2f, 0.8f);
            visualizer.gridLineColor = new Color(0.3f, 0.7f, 0.3f, 0.5f);
        }
        
        Debug.Log("基础示例设置完成！");
        Debug.Log($"网格大小: {levelEditor.gridSize}");
        Debug.Log($"卡片间距: {levelEditor.cardSpacing}");
        Debug.Log($"卡片大小: {levelEditor.cardSize}");
        Debug.Log($"层级数量: {levelEditor.totalLayers}");
    }
    
    /// <summary>
    /// 创建示例关卡
    /// </summary>
    public void CreateExampleLevel()
    {
        if (levelEditor == null) return;
        
        // 清除现有卡片
        levelEditor.ClearAllCards();
        
        // 创建一些示例卡片
        CreateExampleCards();
        
        // 保存关卡
        levelEditor.SaveLevel();
        
        Debug.Log("示例关卡创建完成！");
    }
    
    /// <summary>
    /// 创建示例卡片
    /// </summary>
    void CreateExampleCards()
    {
        // 第一层卡片
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                CardData2D card = new CardData2D
                {
                    id = x * 4 + y + 1,
                    type = Random.Range(0, 4), // 随机卡片类型
                    position = new Vector2(x * levelEditor.cardSpacing, y * levelEditor.cardSpacing),
                    layer = 0
                };
                
                levelEditor.AddCardData(card);
            }
        }
        
        // 第二层卡片（部分覆盖）
        for (int x = 1; x < 3; x++)
        {
            for (int y = 1; y < 3; y++)
            {
                CardData2D card = new CardData2D
                {
                    id = 16 + (x - 1) * 2 + (y - 1) + 1,
                    type = Random.Range(0, 4),
                    position = new Vector2(x * levelEditor.cardSpacing, y * levelEditor.cardSpacing),
                    layer = 1
                };
                
                levelEditor.AddCardData(card);
            }
        }
        
        // 第三层卡片（中心位置）
        CardData2D centerCard = new CardData2D
        {
            id = 21,
            type = Random.Range(0, 4),
            position = new Vector2(1.5f * levelEditor.cardSpacing, 1.5f * levelEditor.cardSpacing),
            layer = 2
        };
        
        levelEditor.AddCardData(centerCard);
    }
    
    /// <summary>
    /// 切换层级预览
    /// </summary>
    public void ToggleLayerPreview()
    {
        if (levelEditor != null)
        {
            levelEditor.enableLayerPreview = !levelEditor.enableLayerPreview;
            levelEditor.UpdateCardDisplay();
            
            Debug.Log($"层级预览: {(levelEditor.enableLayerPreview ? "开启" : "关闭")}");
        }
    }
    
    /// <summary>
    /// 切换网格显示
    /// </summary>
    public void ToggleGridDisplay()
    {
        if (levelEditor != null)
        {
            levelEditor.showGrid = !levelEditor.showGrid;
            levelEditor.UpdateGridDisplay();
            
            Debug.Log($"网格显示: {(levelEditor.showGrid ? "开启" : "关闭")}");
        }
    }
    
    /// <summary>
    /// 切换调试模式
    /// </summary>
    public void ToggleDebugMode()
    {
        if (levelEditor != null)
        {
            levelEditor.debugMode = !levelEditor.debugMode;
            
            Debug.Log($"调试模式: {(levelEditor.debugMode ? "开启" : "关闭")}");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("基础示例控制", GUI.skin.box);
        
        if (GUILayout.Button("设置基础示例"))
        {
            SetupBasicExample();
        }
        
        if (GUILayout.Button("创建示例关卡"))
        {
            CreateExampleLevel();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("切换层级预览"))
        {
            ToggleLayerPreview();
        }
        
        if (GUILayout.Button("切换网格显示"))
        {
            ToggleGridDisplay();
        }
        
        if (GUILayout.Button("切换调试模式"))
        {
            ToggleDebugMode();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 