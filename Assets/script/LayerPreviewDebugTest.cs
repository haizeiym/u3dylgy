using UnityEngine;

public class LayerPreviewDebugTest : MonoBehaviour
{
    [Header("调试设置")]
    public bool runTestOnStart = true;
    public bool createTestCards = true;
    
    private SheepLevelEditor2D editor;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunDebugTest();
        }
    }
    
    [ContextMenu("运行调试测试")]
    public void RunDebugTest()
    {
        Debug.Log("=== 开始层级预览调试测试 ===");
        
        // 查找编辑器
        editor = FindObjectOfType<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.LogError("❌ 未找到SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("✅ 找到编辑器组件");
        
        // 检查初始状态
        Debug.Log($"初始状态 - 层级预览: {editor.enableLayerPreview}");
        Debug.Log($"初始状态 - 当前层级: {editor.selectedLayer}");
        Debug.Log($"初始状态 - 总层数: {editor.totalLayers}");
        Debug.Log($"初始状态 - 正常颜色: {editor.normalLayerColor}");
        Debug.Log($"初始状态 - 置灰颜色: {editor.grayedLayerColor}");
        
        if (createTestCards)
        {
            CreateTestCards();
        }
        
        // 测试层级预览功能
        TestLayerPreview();
        
        Debug.Log("=== 层级预览调试测试完成 ===");
    }
    
    void CreateTestCards()
    {
        Debug.Log("--- 创建测试卡片 ---");
        
        // 清除现有卡片
        editor.ClearAllCards();
        
        // 创建测试卡片
        for (int layer = 0; layer < editor.totalLayers; layer++)
        {
            for (int type = 0; type < 3; type++)
            {
                CardData2D testCard = new CardData2D
                {
                    id = layer * 10 + type + 1,
                    type = type,
                    position = new Vector2(layer * 2, type * 2),
                    layer = layer,
                    isVisible = true,
                    blockingCards = new System.Collections.Generic.List<int>()
                };
                
                editor.AddCardData(testCard);
                editor.CreateCardObject2D(testCard);
                Debug.Log($"创建卡片: ID={testCard.id}, 类型={testCard.type}, 层级={testCard.layer}, 位置={testCard.position}");
            }
        }
        
        editor.UpdateCardDisplay();
        Debug.Log("✅ 测试卡片创建完成");
    }
    
    void TestLayerPreview()
    {
        Debug.Log("--- 测试层级预览功能 ---");
        
        // 确保启用层级预览
        editor.enableLayerPreview = true;
        Debug.Log("✅ 启用层级预览");
        
        // 测试每个层级
        for (int layer = 0; layer < editor.totalLayers; layer++)
        {
            Debug.Log($"--- 测试层级 {layer} ---");
            
            // 切换到指定层级
            editor.selectedLayer = layer;
            editor.UpdateCardDisplay();
            
            // 检查卡片状态
            CheckCardStatus();
            
            // 等待一秒
            System.Threading.Thread.Sleep(1000);
        }
        
        Debug.Log("✅ 层级预览测试完成");
    }
    
    void CheckCardStatus()
    {
        Debug.Log($"当前层级: {editor.selectedLayer}");
        
        var cardObjects = editor.GetCardObjects();
        Debug.Log($"总卡片数: {cardObjects.Count}");
        
        int normalColorCount = 0;
        int grayedColorCount = 0;
        int hiddenCount = 0;
        
        foreach (var cardObj in cardObjects)
        {
            if (cardObj == null) continue;
            
            CardObject2D cardComponent = cardObj.GetComponent<CardObject2D>();
            if (cardComponent == null) continue;
            
            if (cardObj.activeSelf)
            {
                SpriteRenderer spriteRenderer = cardObj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    if (spriteRenderer.color == editor.normalLayerColor)
                    {
                        normalColorCount++;
                        Debug.Log($"  ✅ 卡片ID:{cardComponent.cardId} 层级:{cardComponent.layer} - 正常颜色");
                    }
                    else if (spriteRenderer.color == editor.grayedLayerColor)
                    {
                        grayedColorCount++;
                        Debug.Log($"  🔶 卡片ID:{cardComponent.cardId} 层级:{cardComponent.layer} - 置灰颜色");
                    }
                    else
                    {
                        Debug.Log($"  ❓ 卡片ID:{cardComponent.cardId} 层级:{cardComponent.layer} - 未知颜色: {spriteRenderer.color}");
                    }
                }
            }
            else
            {
                hiddenCount++;
                Debug.Log($"  ❌ 卡片ID:{cardComponent.cardId} 层级:{cardComponent.layer} - 隐藏");
            }
        }
        
        Debug.Log($"统计: 正常颜色={normalColorCount}, 置灰颜色={grayedColorCount}, 隐藏={hiddenCount}");
        
        // 验证结果
        if (normalColorCount == 3 && grayedColorCount == (editor.totalLayers - 1) * 3)
        {
            Debug.Log("✅ 层级预览功能正常！");
        }
        else
        {
            Debug.LogError("❌ 层级预览功能异常！");
        }
    }
    
    [ContextMenu("切换层级预览")]
    public void ToggleLayerPreview()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            editor.enableLayerPreview = !editor.enableLayerPreview;
            editor.UpdateCardDisplay();
            Debug.Log($"层级预览已切换为: {(editor.enableLayerPreview ? "启用" : "禁用")}");
        }
    }
    
    [ContextMenu("切换到下一层级")]
    public void NextLayer()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            editor.selectedLayer = (editor.selectedLayer + 1) % editor.totalLayers;
            editor.UpdateCardDisplay();
            Debug.Log($"切换到层级: {editor.selectedLayer}");
        }
    }
    
    [ContextMenu("切换到上一层级")]
    public void PreviousLayer()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            editor.selectedLayer = (editor.selectedLayer - 1 + editor.totalLayers) % editor.totalLayers;
            editor.UpdateCardDisplay();
            Debug.Log($"切换到层级: {editor.selectedLayer}");
        }
    }
} 