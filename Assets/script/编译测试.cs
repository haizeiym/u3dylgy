using UnityEngine;
using YangLeGeYang2D.LevelEditor;

/// <summary>
/// 编译测试脚本 - 验证所有错误是否已修复
/// </summary>
public class 编译测试 : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 编译测试开始 ===");
        
        // 测试1: 验证命名空间和类型
        TestNamespaceAndTypes();
        
        // 测试2: 验证CardData2D创建
        TestCardData2DCreation();
        
        // 测试3: 验证LevelValidator2D访问
        TestLevelValidator2DAccess();
        
        // 测试4: 验证Vector2类型转换
        TestVector2Conversion();
        
        // 测试5: 验证float类型cardSize
        TestFloatCardSize();
        
        Debug.Log("=== 编译测试完成 ===");
    }
    
    private void TestNamespaceAndTypes()
    {
        Debug.Log("测试1: 验证命名空间和类型...");
        
        // 测试命名空间访问
        var cardData = new YangLeGeYang2D.LevelEditor.CardData2D();
        var levelData = new YangLeGeYang2D.LevelEditor.LevelData2D();
        var irregularShape = new YangLeGeYang2D.LevelEditor.IrregularShapeData();
        
        Debug.Log("✅ 命名空间和类型访问正常");
    }
    
    private void TestCardData2DCreation()
    {
        Debug.Log("测试2: 验证CardData2D创建...");
        
        // 测试CardData2D创建和属性设置
        YangLeGeYang2D.LevelEditor.CardData2D testCard = new YangLeGeYang2D.LevelEditor.CardData2D
        {
            id = 1,
            type = 0,
            position = new Vector2(0, 0),
            layer = 0,
            isVisible = true,
            blockingCards = new System.Collections.Generic.List<int>()
        };
        
        Debug.Log($"✅ CardData2D创建成功: ID={testCard.id}, Type={testCard.type}");
    }
    
    private void TestLevelValidator2DAccess()
    {
        Debug.Log("测试3: 验证LevelValidator2D访问...");
        
        // 测试LevelValidator2D静态方法访问
        var validationResult = new YangLeGeYang2D.LevelEditor.LevelValidator2D.ValidationResult();
        string report = YangLeGeYang2D.LevelEditor.LevelValidator2D.GetValidationReport(validationResult);
        
        Debug.Log("✅ LevelValidator2D访问正常");
    }
    
    private void TestVector2Conversion()
    {
        Debug.Log("测试4: 验证Vector2类型转换...");
        
        // 测试float到Vector2的转换
        float cardSizeFloat = 0.8f;
        Vector2 cardSizeVector = new Vector2(cardSizeFloat, cardSizeFloat);
        
        // 测试LevelData2D中的cardSize赋值
        YangLeGeYang2D.LevelEditor.LevelData2D testLevel = new YangLeGeYang2D.LevelEditor.LevelData2D
        {
            levelName = "TestLevel",
            levelId = 1,
            gridSize = new Vector2(8, 8),
            cardSpacing = 1.2f,
            cardSize = cardSizeVector, // 这里应该是Vector2类型
            totalLayers = 3,
            cards = new System.Collections.Generic.List<YangLeGeYang2D.LevelEditor.CardData2D>(),
            irregularShapes = new System.Collections.Generic.List<YangLeGeYang2D.LevelEditor.IrregularShapeData>()
        };
        
        Debug.Log($"✅ Vector2类型转换成功: cardSize={testLevel.cardSize}");
    }
    
    private void TestFloatCardSize()
    {
        Debug.Log("测试5: 验证float类型cardSize...");
        
        // 测试SheepLevelEditor2D中定义的LevelData2D（float cardSize）
        LevelData2D testLevel = new LevelData2D
        {
            levelName = "TestLevel",
            levelId = 1,
            gridSize = new Vector2(8, 8),
            cardSpacing = 1.2f,
            cardSize = 0.8f, // 这里是float类型
            totalLayers = 3,
            cards = new System.Collections.Generic.List<CardData2D>()
        };
        
        // 测试float类型的cardSize访问
        float cardSizeValue = testLevel.cardSize;
        Debug.Log($"✅ float类型cardSize访问成功: cardSize={cardSizeValue}");
    }
    
    [ContextMenu("运行编译测试")]
    public void RunCompilationTest()
    {
        Start();
    }
} 