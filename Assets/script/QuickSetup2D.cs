using UnityEngine;

public class QuickSetup2D : MonoBehaviour
{
    [Header("快速设置")]
    public bool autoSetup = true;
    public bool createTestCards = false;
    
    void Start()
    {
        if (autoSetup)
        {
            Setup2DEditor();
        }
    }
    
    void Setup2DEditor()
    {
        Debug.Log("🔧 开始2D编辑器快速设置...");
        
        // 查找或创建2D编辑器
        SheepLevelEditor2D editor = FindObjectOfType<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.Log("创建2D编辑器...");
            GameObject editorObj = new GameObject("SheepLevelEditor2D");
            editor = editorObj.AddComponent<SheepLevelEditor2D>();
        }
        
        // 确保编辑器处于激活状态
        editor.isEditMode = true;
        
        // 设置基本参数
        editor.currentLevelId = 1;
        editor.currentLevelName = "Level2D_1";
        editor.totalLayers = 3;
        editor.selectedLayer = 0;
        editor.currentCardType = 0;
        editor.cardSize = 0.8f;
        editor.cardSpacing = 1.2f;
        editor.cameraZoom = 5f;
        
        // 确保有主相机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.Log("创建主相机...");
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
        
        // 确保GUI事件管理器存在
        GUIEventManager eventManager = FindObjectOfType<GUIEventManager>();
        if (eventManager == null)
        {
            Debug.Log("创建GUI事件管理器...");
            GameObject eventManagerObj = new GameObject("GUIEventManager");
            eventManager = eventManagerObj.AddComponent<GUIEventManager>();
        }
        
        // 创建测试卡片（可选）
        if (createTestCards)
        {
            CreateTestCards(editor);
        }
        
        Debug.Log("✅ 2D编辑器设置完成！");
        Debug.Log("操作说明:");
        Debug.Log("- 左键: 放置卡片");
        Debug.Log("- 右键: 删除卡片");
        Debug.Log("- 滚轮: 切换层级");
        Debug.Log("- Ctrl+滚轮: 缩放相机");
        Debug.Log("- 数字键1-8: 切换卡片类型");
        Debug.Log("- Ctrl+S: 保存关卡");
        Debug.Log("- Ctrl+L: 加载关卡");
        Debug.Log("- Ctrl+N: 新建关卡");
    }
    
    void CreateTestCards(SheepLevelEditor2D editor)
    {
        Debug.Log("创建测试卡片...");
        
        // 创建一些测试卡片数据
        var testCards = new System.Collections.Generic.List<CardData2D>
        {
            new CardData2D { id = 1, type = 0, position = new Vector2(0, 0), layer = 0, isVisible = true, blockingCards = new System.Collections.Generic.List<int>() },
            new CardData2D { id = 2, type = 1, position = new Vector2(1.2f, 0), layer = 0, isVisible = true, blockingCards = new System.Collections.Generic.List<int>() },
            new CardData2D { id = 3, type = 2, position = new Vector2(0, 1.2f), layer = 0, isVisible = true, blockingCards = new System.Collections.Generic.List<int>() },
            new CardData2D { id = 4, type = 0, position = new Vector2(0.6f, 0.6f), layer = 1, isVisible = true, blockingCards = new System.Collections.Generic.List<int>() },
            new CardData2D { id = 5, type = 1, position = new Vector2(1.8f, 0.6f), layer = 1, isVisible = true, blockingCards = new System.Collections.Generic.List<int>() }
        };
        
        // 通过反射设置私有字段（仅用于测试）
        var levelCardsField = typeof(SheepLevelEditor2D).GetField("levelCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (levelCardsField != null)
        {
            levelCardsField.SetValue(editor, testCards);
        }
        
        // 重新创建卡片对象
        editor.SendMessage("NewLevel");
        foreach (var cardData in testCards)
        {
            editor.SendMessage("CreateCardObject2D", cardData);
        }
        
        Debug.Log($"创建了 {testCards.Count} 个测试卡片");
    }
    
    void Update()
    {
        // 按R键重新设置
        if (Input.GetKeyDown(KeyCode.R))
        {
            Setup2DEditor();
        }
    }
} 