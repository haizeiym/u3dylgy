using UnityEngine;

public class LayerPreviewTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTestOnStart = true;
    public bool enableDebugLogs = true;
    
    [Header("测试结果")]
    public bool layerPreviewEnabled = false;
    public Color currentNormalColor = Color.white;
    public Color currentGrayedColor = Color.gray;
    public int currentSelectedLayer = 0;
    public int totalLayers = 3;
    
    private SheepLevelEditor2D editor;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunLayerPreviewTest();
        }
    }
    
    void Update()
    {
        // 实时监控层级预览状态
        if (editor != null)
        {
            layerPreviewEnabled = editor.enableLayerPreview;
            currentNormalColor = editor.normalLayerColor;
            currentGrayedColor = editor.grayedLayerColor;
            currentSelectedLayer = editor.selectedLayer;
            totalLayers = editor.totalLayers;
        }
    }
    
    [ContextMenu("运行层级预览测试")]
    public void RunLayerPreviewTest()
    {
        Debug.Log("=== 开始层级预览功能测试 ===");
        
        // 查找编辑器
        editor = FindObjectOfType<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.LogError("❌ 未找到SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("✅ 找到编辑器组件");
        
        // 测试1: 检查层级预览设置
        TestLayerPreviewSettings();
        
        // 测试2: 测试层级切换
        TestLayerSwitching();
        
        // 测试3: 测试颜色变化
        TestColorChanges();
        
        // 测试4: 测试保存和加载
        TestSaveAndLoad();
        
        Debug.Log("=== 层级预览功能测试完成 ===");
    }
    
    void TestLayerPreviewSettings()
    {
        Debug.Log("--- 测试层级预览设置 ---");
        
        // 检查默认设置
        Debug.Log($"默认层级预览状态: {editor.enableLayerPreview}");
        Debug.Log($"默认正常颜色: {editor.normalLayerColor}");
        Debug.Log($"默认置灰颜色: {editor.grayedLayerColor}");
        
        // 测试启用层级预览
        editor.enableLayerPreview = true;
        editor.UpdateCardDisplay();
        Debug.Log("✅ 启用层级预览");
        
        // 测试禁用层级预览
        editor.enableLayerPreview = false;
        editor.UpdateCardDisplay();
        Debug.Log("✅ 禁用层级预览");
        
        // 恢复默认设置
        editor.enableLayerPreview = true;
        editor.UpdateCardDisplay();
    }
    
    void TestLayerSwitching()
    {
        Debug.Log("--- 测试层级切换 ---");
        
        int originalLayer = editor.selectedLayer;
        
        // 切换到不同层级
        for (int i = 0; i < editor.totalLayers; i++)
        {
            editor.selectedLayer = i;
            editor.UpdateCardDisplay();
            Debug.Log($"✅ 切换到层级 {i}");
            
            // 检查卡片显示状态
            CheckCardDisplayStatus();
        }
        
        // 恢复原始层级
        editor.selectedLayer = originalLayer;
        editor.UpdateCardDisplay();
    }
    
    void TestColorChanges()
    {
        Debug.Log("--- 测试颜色变化 ---");
        
        // 测试正常颜色变化
        Color originalNormalColor = editor.normalLayerColor;
        editor.normalLayerColor = Color.red;
        editor.UpdateCardDisplay();
        Debug.Log("✅ 正常颜色已更改为红色");
        
        // 测试置灰颜色变化
        Color originalGrayedColor = editor.grayedLayerColor;
        editor.grayedLayerColor = Color.blue;
        editor.UpdateCardDisplay();
        Debug.Log("✅ 置灰颜色已更改为蓝色");
        
        // 恢复原始颜色
        editor.normalLayerColor = originalNormalColor;
        editor.grayedLayerColor = originalGrayedColor;
        editor.UpdateCardDisplay();
    }
    
    void TestSaveAndLoad()
    {
        Debug.Log("--- 测试保存和加载 ---");
        
        // 保存当前设置
        string originalLevelName = editor.currentLevelName;
        int originalLevelId = editor.currentLevelId;
        
        // 修改设置
        editor.enableLayerPreview = true;
        editor.normalLayerColor = Color.green;
        editor.grayedLayerColor = Color.yellow;
        
        // 保存关卡
        editor.SaveLevel();
        Debug.Log("✅ 保存关卡（包含层级预览设置）");
        
        // 修改设置
        editor.enableLayerPreview = false;
        editor.normalLayerColor = Color.black;
        editor.grayedLayerColor = Color.white;
        
        // 重新加载关卡
        editor.LoadLevel(editor.currentLevelId);
        Debug.Log("✅ 重新加载关卡");
        
        // 检查设置是否恢复
        if (editor.enableLayerPreview)
        {
            Debug.Log("✅ 层级预览设置已正确恢复");
        }
        else
        {
            Debug.LogWarning("⚠️ 层级预览设置未正确恢复");
        }
        
        if (editor.normalLayerColor == Color.green)
        {
            Debug.Log("✅ 正常颜色设置已正确恢复");
        }
        else
        {
            Debug.LogWarning("⚠️ 正常颜色设置未正确恢复");
        }
        
        if (editor.grayedLayerColor == Color.yellow)
        {
            Debug.Log("✅ 置灰颜色设置已正确恢复");
        }
        else
        {
            Debug.LogWarning("⚠️ 置灰颜色设置未正确恢复");
        }
    }
    
    void CheckCardDisplayStatus()
    {
        int normalColorCards = 0;
        int grayedColorCards = 0;
        int hiddenCards = 0;
        
        foreach (var cardObj in editor.GetCardObjects())
        {
            if (cardObj != null && cardObj.activeSelf)
            {
                SpriteRenderer spriteRenderer = cardObj.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    if (spriteRenderer.color == editor.normalLayerColor)
                    {
                        normalColorCards++;
                    }
                    else if (spriteRenderer.color == editor.grayedLayerColor)
                    {
                        grayedColorCards++;
                    }
                }
            }
            else
            {
                hiddenCards++;
            }
        }
        
        Debug.Log($"当前层级 {editor.selectedLayer}: 正常颜色卡片={normalColorCards}, 置灰卡片={grayedColorCards}, 隐藏卡片={hiddenCards}");
    }
    
    [ContextMenu("创建测试卡片")]
    public void CreateTestCards()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor == null)
        {
            Debug.LogError("❌ 未找到编辑器！");
            return;
        }
        
        // 创建测试卡片
        for (int layer = 0; layer < editor.totalLayers; layer++)
        {
            for (int type = 0; type < 3; type++)
            {
                CardData2D testCard = new CardData2D
                {
                    id = Random.Range(1000, 9999),
                    type = type,
                    position = new Vector2(layer * 2, type * 2),
                    layer = layer,
                    isVisible = true,
                    blockingCards = new System.Collections.Generic.List<int>()
                };
                
                editor.AddCardData(testCard);
                editor.CreateCardObject2D(testCard);
            }
        }
        
        editor.UpdateCardDisplay();
        Debug.Log("✅ 已创建测试卡片");
    }
    
    [ContextMenu("清除所有卡片")]
    public void ClearAllCards()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor == null)
        {
            Debug.LogError("❌ 未找到编辑器！");
            return;
        }
        
        editor.ClearAllCards();
        editor.UpdateCardDisplay();
        Debug.Log("✅ 已清除所有卡片");
    }
} 