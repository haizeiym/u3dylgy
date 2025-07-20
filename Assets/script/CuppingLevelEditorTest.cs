using UnityEngine;
using YangLeGeYang2D.LevelEditor;

/// <summary>
/// 拔了个罐2D关卡编辑器测试脚本
/// 用于验证编辑器在script目录中的集成是否成功
/// </summary>
public class CuppingLevelEditorTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool autoTestOnStart = true;
    public bool createTestLevel = true;
    public bool addTestShapes = true;
    
    [Header("测试关卡设置")]
    public Vector2 testGridSize = new Vector2(8, 8);
    public float testCardSpacing = 1.0f;
    public Vector2 testCardSize = new Vector2(0.8f, 0.8f);
    public int testTotalLayers = 3;
    
    private CuppingLevelEditor2D levelEditor;
    
    void Start()
    {
        if (autoTestOnStart)
        {
            RunTest();
        }
    }
    
    [ContextMenu("运行测试")]
    public void RunTest()
    {
        Debug.Log("=== 拔了个罐2D关卡编辑器测试开始 ===");
        
        // 查找或创建编辑器
        levelEditor = FindObjectOfType<CuppingLevelEditor2D>();
        if (levelEditor == null)
        {
            Debug.Log("未找到CuppingLevelEditor2D，创建新的编辑器对象");
            GameObject editorObj = new GameObject("拔了个罐2D关卡编辑器");
            levelEditor = editorObj.AddComponent<CuppingLevelEditor2D>();
        }
        
        // 测试基本功能
        TestBasicFunctions();
        
        // 测试不规则图形功能
        if (addTestShapes)
        {
            TestIrregularShapes();
        }
        
        // 测试关卡创建
        if (createTestLevel)
        {
            TestLevelCreation();
        }
        
        // 测试验证功能
        TestValidation();
        
        Debug.Log("=== 拔了个罐2D关卡编辑器测试完成 ===");
    }
    
    private void TestBasicFunctions()
    {
        Debug.Log("测试基本功能...");
        
        // 测试设置
        levelEditor.gridSize = testGridSize;
        levelEditor.cardSpacing = testCardSpacing;
        levelEditor.cardSize = testCardSize;
        levelEditor.totalLayers = testTotalLayers;
        levelEditor.currentLayer = 0;
        
        // 测试区域大小计算
        Vector2 actualSize = levelEditor.GetActualAreaSize();
        Debug.Log($"实际区域大小: {actualSize}");
        
        // 测试网格对齐
        Vector2 testPos = new Vector2(1.5f, 2.3f);
        Vector2 snappedPos = levelEditor.SnapToGrid2D(testPos);
        Debug.Log($"测试位置 {testPos} 对齐到网格: {snappedPos}");
        
        Debug.Log("基本功能测试完成");
    }
    
    private void TestIrregularShapes()
    {
        Debug.Log("测试不规则图形功能...");
        
        // 添加预设图形
        levelEditor.AddPresetShape(0, 0); // 圆形
        levelEditor.AddPresetShape(1, 1); // 星形
        levelEditor.AddPresetShape(2, 2); // 矩形
        
        Debug.Log($"已添加 {levelEditor.irregularShapes.Count} 个不规则图形");
        
        // 测试位置检查
        Vector2 testPoint = new Vector2(0, 0);
        bool inShape = levelEditor.IsPositionInIrregularShapes(testPoint, 0);
        Debug.Log($"测试点 {testPoint} 在层级0的图形内: {inShape}");
        
        Debug.Log("不规则图形功能测试完成");
    }
    
    private void TestLevelCreation()
    {
        Debug.Log("测试关卡创建...");
        
        // 创建新关卡
        levelEditor.NewLevel();
        Debug.Log("已创建新关卡");
        
        // 添加测试卡片 - 使用正确的命名空间
        YangLeGeYang2D.LevelEditor.CardData2D testCard = new YangLeGeYang2D.LevelEditor.CardData2D
        {
            id = 1,
            type = 0,
            position = new Vector2(0, 0),
            layer = 0,
            isVisible = true
        };
        
        levelEditor.AddCardData(testCard);
        Debug.Log("已添加测试卡片");
        
        // 保存关卡
        levelEditor.SaveLevel();
        Debug.Log("已保存测试关卡");
        
        Debug.Log("关卡创建测试完成");
    }
    
    private void TestValidation()
    {
        Debug.Log("测试关卡验证...");
        
        // 运行验证
        levelEditor.ValidateCurrentLevel();
        
        Debug.Log("关卡验证测试完成");
    }
    
    [ContextMenu("清理测试数据")]
    public void CleanupTestData()
    {
        Debug.Log("清理测试数据...");
        
        if (levelEditor != null)
        {
            // 清除所有卡片
            levelEditor.ClearCardObjects();
            
            // 清除不规则图形
            levelEditor.irregularShapes.Clear();
            
            Debug.Log("测试数据已清理");
        }
    }
    
    [ContextMenu("显示编辑器信息")]
    public void ShowEditorInfo()
    {
        if (levelEditor != null)
        {
            Debug.Log("=== 编辑器信息 ===");
            Debug.Log($"网格大小: {levelEditor.gridSize}");
            Debug.Log($"卡片间距: {levelEditor.cardSpacing}");
            Debug.Log($"卡片大小: {levelEditor.cardSize}");
            Debug.Log($"总层级数: {levelEditor.totalLayers}");
            Debug.Log($"当前层级: {levelEditor.currentLayer}");
            Debug.Log($"不规则图形数量: {levelEditor.irregularShapes.Count}");
            // 移除对currentLevel的直接访问，因为它是private的
            Debug.Log("==================");
        }
        else
        {
            Debug.LogWarning("未找到CuppingLevelEditor2D组件");
        }
    }
} 