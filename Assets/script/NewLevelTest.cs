using UnityEngine;
using System.Collections;

public class NewLevelTest : MonoBehaviour
{
    [Header("测试设置")]
    public SheepLevelEditor2D editor2D;
    public int testCardCount = 5;
    public float testDelay = 2f;
    
    [Header("测试状态")]
    public bool isTestRunning = false;
    public int currentTestStep = 0;
    public string testStatus = "未开始";
    
    void Start()
    {
        if (editor2D == null)
        {
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor2D == null)
        {
            Debug.LogError("未找到SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("新建关卡测试脚本已初始化");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartNewLevelTest();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StopTest();
        }
    }
    
    [ContextMenu("开始新建关卡测试")]
    public void StartNewLevelTest()
    {
        if (isTestRunning)
        {
            Debug.LogWarning("测试正在进行中，请先停止当前测试");
            return;
        }
        
        StartCoroutine(RunNewLevelTest());
    }
    
    IEnumerator RunNewLevelTest()
    {
        isTestRunning = true;
        currentTestStep = 0;
        testStatus = "开始测试";
        
        Debug.Log("=== 开始新建关卡测试 ===");
        
        // 步骤1: 确保编辑器处于编辑模式
        currentTestStep = 1;
        testStatus = "步骤1: 设置编辑模式";
        Debug.Log("步骤1: 设置编辑模式");
        
        if (editor2D != null)
        {
            // 直接设置isEditMode为true
            editor2D.isEditMode = true;
            Debug.Log("已设置编辑模式为true");
        }
        
        yield return new WaitForSeconds(1f);
        
        // 步骤2: 创建一些测试卡片
        currentTestStep = 2;
        testStatus = "步骤2: 创建测试卡片";
        Debug.Log("步骤2: 创建测试卡片");
        
        for (int i = 0; i < testCardCount; i++)
        {
            Vector2 testPos = new Vector2(i * 1.2f, 0);
            CreateTestCard(testPos, i % 8, 0);
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log($"已创建 {testCardCount} 个测试卡片");
        yield return new WaitForSeconds(testDelay);
        
        // 步骤3: 记录当前卡片数量
        currentTestStep = 3;
        testStatus = "步骤3: 记录当前状态";
        Debug.Log("步骤3: 记录当前状态");
        
        int cardsBeforeNewLevel = GetCurrentCardCount();
        Debug.Log($"新建关卡前卡片数量: {cardsBeforeNewLevel}");
        
        yield return new WaitForSeconds(1f);
        
        // 步骤4: 执行新建关卡
        currentTestStep = 4;
        testStatus = "步骤4: 执行新建关卡";
        Debug.Log("步骤4: 执行新建关卡");
        
        if (editor2D != null)
        {
            editor2D.SendMessage("NewLevel");
        }
        
        yield return new WaitForSeconds(1f);
        
        // 步骤5: 检查新建关卡后的状态
        currentTestStep = 5;
        testStatus = "步骤5: 检查新建关卡结果";
        Debug.Log("步骤5: 检查新建关卡结果");
        
        int cardsAfterNewLevel = GetCurrentCardCount();
        Debug.Log($"新建关卡后卡片数量: {cardsAfterNewLevel}");
        
        // 验证结果
        if (cardsAfterNewLevel == 0)
        {
            Debug.Log("✓ 测试通过: 新建关卡成功清除了所有卡片");
            testStatus = "测试通过: 卡片已清除";
        }
        else
        {
            Debug.LogError($"✗ 测试失败: 新建关卡后仍有 {cardsAfterNewLevel} 个卡片未被清除");
            testStatus = "测试失败: 卡片未清除";
        }
        
        // 步骤6: 验证场景中的GameObject
        currentTestStep = 6;
        testStatus = "步骤6: 验证场景GameObject";
        Debug.Log("步骤6: 验证场景GameObject");
        
        GameObject[] cardGameObjects = GameObject.FindGameObjectsWithTag("Card");
        if (cardGameObjects.Length == 0)
        {
            Debug.Log("✓ 场景中未找到Card标签的GameObject");
        }
        else
        {
            Debug.LogWarning($"场景中找到 {cardGameObjects.Length} 个Card标签的GameObject");
        }
        
        // 查找所有可能的卡片对象
        CardObject2D[] cardComponents = FindObjectsOfType<CardObject2D>();
        Debug.Log($"场景中找到 {cardComponents.Length} 个CardObject2D组件");
        
        if (cardComponents.Length == 0)
        {
            Debug.Log("✓ 场景中未找到CardObject2D组件");
        }
        else
        {
            Debug.LogWarning("✗ 场景中仍有CardObject2D组件存在");
            foreach (var card in cardComponents)
            {
                Debug.LogWarning($"  剩余卡片: ID={card.cardId}, 类型={card.cardType}, 层级={card.layer}");
            }
        }
        
        yield return new WaitForSeconds(2f);
        
        // 测试完成
        currentTestStep = 0;
        testStatus = "测试完成";
        isTestRunning = false;
        
        Debug.Log("=== 新建关卡测试完成 ===");
    }
    
    void CreateTestCard(Vector2 position, int cardType, int layer)
    {
        if (editor2D == null) return;
        
        try
        {
            // 创建测试卡片数据
            var cardData = new CardData2D
            {
                id = Random.Range(1000, 9999),
                type = cardType,
                position = position,
                layer = layer,
                isVisible = true,
                blockingCards = new System.Collections.Generic.List<int>()
            };
            
            // 通过反射调用CreateCardObject2D方法
            var createCardMethod = typeof(SheepLevelEditor2D).GetMethod("CreateCardObject2D", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (createCardMethod != null)
            {
                createCardMethod.Invoke(editor2D, new object[] { cardData });
                Debug.Log($"创建测试卡片: 位置={position}, 类型={cardType}, 层级={layer}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"创建测试卡片失败: {e.Message}");
        }
    }
    
    int GetCurrentCardCount()
    {
        if (editor2D == null) return 0;
        
        // 通过反射获取levelCards列表
        var levelCardsField = typeof(SheepLevelEditor2D).GetField("levelCards", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (levelCardsField != null)
        {
            var levelCards = levelCardsField.GetValue(editor2D) as System.Collections.Generic.List<CardData2D>;
            return levelCards != null ? levelCards.Count : 0;
        }
        
        return 0;
    }
    
    [ContextMenu("停止测试")]
    public void StopTest()
    {
        if (isTestRunning)
        {
            StopAllCoroutines();
            isTestRunning = false;
            currentTestStep = 0;
            testStatus = "测试已停止";
            Debug.Log("测试已停止");
        }
    }
    
    void OnGUI()
    {
        if (editor2D == null) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("新建关卡测试", GUI.skin.box);
        GUILayout.Space(5);
        
        GUILayout.Label($"测试状态: {testStatus}");
        if (isTestRunning)
        {
            GUILayout.Label($"当前步骤: {currentTestStep}/6");
        }
        
        GUILayout.Space(5);
        
        if (!isTestRunning)
        {
            if (GUILayout.Button("开始测试 (F1)"))
            {
                StartNewLevelTest();
            }
        }
        else
        {
            if (GUILayout.Button("停止测试 (F2)"))
            {
                StopTest();
            }
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("当前卡片数量:");
        GUILayout.Label($"数据层: {GetCurrentCardCount()}");
        GUILayout.Label($"场景对象: {FindObjectsOfType<CardObject2D>().Length}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 