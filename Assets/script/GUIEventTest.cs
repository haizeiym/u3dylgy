using UnityEngine;

public class GUIEventTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runEventTest = true;
    public float testInterval = 2f;
    public bool showGUIBounds = true;
    public bool enableDebugLogs = true;
    
    [Header("纹理设置")]
    public int textureSize = 128; // 纹理大小，影响GUI边界可视化的质量
    
    private SheepLevelEditor2D editor2D;
    private float lastTestTime;
    private Vector2 lastMousePosition;
    private bool lastMouseOverGUI = false;
    
    // GUI边界可视化
    private GameObject guiBoundsVisualizer;
    
    void Start()
    {
        // 查找编辑器组件
        editor2D = FindObjectOfType<SheepLevelEditor2D>();
        
        if (editor2D == null)
        {
            Debug.LogError("未找到编辑器组件！请确保场景中有SheepLevelEditor2D组件。");
            return;
        }
        
        Debug.Log("GUI事件测试脚本已启动");
        lastTestTime = Time.time;
        
        // 创建GUI边界可视化
        if (showGUIBounds)
        {
            CreateGUIBoundsVisualizer();
        }
    }
    
    void Update()
    {
        if (!runEventTest) return;
        
        // 实时检查鼠标位置和GUI状态
        CheckMouseAndGUIState();
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunEventTest();
            lastTestTime = Time.time;
        }
    }
    
    void CheckMouseAndGUIState()
    {
        Vector2 mousePos = Input.mousePosition;
        bool currentMouseOverGUI = false;
        
        // 多重检查
        if (GUIEventManager.Instance != null)
        {
            currentMouseOverGUI = GUIEventManager.Instance.IsMouseOverGUI();
        }
        
        // 手动检查
        Rect guiRect = new Rect(10, 10, 300, Screen.height - 20);
        bool manualCheck = guiRect.Contains(mousePos);
        
        // 位置检查
        bool positionCheck = mousePos.x < 320;
        
        // 状态改变时输出日志
        if (currentMouseOverGUI != lastMouseOverGUI || manualCheck != lastMouseOverGUI || positionCheck != lastMouseOverGUI)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"GUI状态变化 - 位置: {mousePos}");
                Debug.Log($"  GUI事件管理器: {currentMouseOverGUI}");
                Debug.Log($"  手动检查: {manualCheck}");
                Debug.Log($"  位置检查: {positionCheck}");
            }
            lastMouseOverGUI = currentMouseOverGUI || manualCheck || positionCheck;
        }
        
        // 更新GUI边界可视化
        UpdateGUIBoundsVisualizer();
    }
    
    void CreateGUIBoundsVisualizer()
    {
        if (!showGUIBounds) return;
        
        guiBoundsVisualizer = new GameObject("GUIBoundsVisualizer");
        guiBoundsVisualizer.transform.position = Vector3.zero;
        
        // 创建GUI边界精灵
        SpriteRenderer spriteRenderer = guiBoundsVisualizer.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateGUIBoundsSprite();
        spriteRenderer.color = new Color(1f, 0f, 0f, 0.2f); // 红色半透明
        spriteRenderer.sortingOrder = 1000; // 确保在最前面显示
    }
    
    void UpdateGUIBoundsVisualizer()
    {
        if (guiBoundsVisualizer == null || !showGUIBounds) return;
        
        // 更新GUI边界可视化位置和大小
        float guiWidth = 300;
        float guiHeight = Screen.height - 20;
        
        // 将屏幕坐标转换为世界坐标
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(guiWidth / 2 + 10, guiHeight / 2 + 10, 10));
        guiBoundsVisualizer.transform.position = worldPos;
        guiBoundsVisualizer.transform.localScale = new Vector3(guiWidth, guiHeight, 1);
    }
    
    Sprite CreateGUIBoundsSprite()
    {
        // 创建GUI边界精灵
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // 创建边框效果
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                if (x < 2 || x >= textureSize - 2 || y < 2 || y >= textureSize - 2)
                {
                    texture.SetPixel(x, y, Color.red);
                }
                else
                {
                    texture.SetPixel(x, y, new Color(1f, 0f, 0f, 0.1f));
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    void RunEventTest()
    {
        Debug.Log("=== GUI事件测试 ===");
        
        // 测试GUI事件管理器
        if (GUIEventManager.Instance != null)
        {
            TestGUIEventManager();
        }
        else
        {
            Debug.LogWarning("GUI事件管理器未找到！");
        }
        
        // 测试编辑器输入处理
        TestEditorInputHandling();
        
        Debug.Log("=== 事件测试完成 ===");
    }
    
    void TestGUIEventManager()
    {
        Debug.Log("测试GUI事件管理器...");
        
        Vector2 mousePos = Input.mousePosition;
        bool isOverGUI = GUIEventManager.Instance.IsMouseOverGUI();
        bool isOverGUINow = GUIEventManager.Instance.IsMouseOverGUINow();
        string guiInfo = GUIEventManager.Instance.GetGUIInfo();
        
        Debug.Log($"GUI事件管理器状态:");
        Debug.Log($"  鼠标位置: {mousePos}");
        Debug.Log($"  IsMouseOverGUI: {isOverGUI}");
        Debug.Log($"  IsMouseOverGUINow: {isOverGUINow}");
        Debug.Log($"  GUI信息: {guiInfo}");
        
        // 测试强制刷新
        GUIEventManager.Instance.ForceRefresh();
    }
    
    void TestEditorInputHandling()
    {
        Debug.Log("测试编辑器输入处理...");
        
        if (editor2D != null)
        {
            Debug.Log("2D编辑器输入处理测试");
            // 这里可以添加更多测试逻辑
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 990, 240, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("GUI事件测试", GUI.skin.box);
        
        runEventTest = GUILayout.Toggle(runEventTest, "启用事件测试");
        showGUIBounds = GUILayout.Toggle(showGUIBounds, "显示GUI边界");
        enableDebugLogs = GUILayout.Toggle(enableDebugLogs, "启用调试日志");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunEventTest();
        }
        
        if (GUILayout.Button("强制刷新GUI"))
        {
            if (GUIEventManager.Instance != null)
            {
                GUIEventManager.Instance.ForceRefresh();
            }
        }
        
        if (GUILayout.Button("切换GUI边界显示"))
        {
            showGUIBounds = !showGUIBounds;
            if (guiBoundsVisualizer != null)
            {
                guiBoundsVisualizer.SetActive(showGUIBounds);
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void OnDestroy()
    {
        // 清理可视化对象
        if (guiBoundsVisualizer != null) Destroy(guiBoundsVisualizer);
    }
} 