using UnityEngine;

public class TextureSizeTest : MonoBehaviour
{
    [Header("纹理大小测试")]
    public bool runTextureTest = true;
    public float testInterval = 5f;
    
    private SheepLevelEditor2D levelEditor;
    private PlaceableAreaVisualizer placeableAreaVisualizer;
    private GridBoundsTest gridBoundsTest;
    private GUIEventTest guiEventTest;
    private float lastTestTime;
    
    void Start()
    {
        levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        placeableAreaVisualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        gridBoundsTest = FindObjectOfType<GridBoundsTest>();
        guiEventTest = FindObjectOfType<GUIEventTest>();
        
        if (levelEditor == null)
        {
            Debug.LogError("未找到编辑器组件！");
            return;
        }
        
        Debug.Log("纹理大小测试脚本已启动");
        lastTestTime = Time.time;
    }
    
    void Update()
    {
        if (!runTextureTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunTextureTest();
            lastTestTime = Time.time;
        }
    }
    
    void RunTextureTest()
    {
        Debug.Log("=== 纹理大小测试 ===");
        
        // 测试主编辑器
        if (levelEditor != null)
        {
            Debug.Log($"主编辑器 textureSize: {levelEditor.textureSize}");
        }
        
        // 测试可放置区域可视化器
        if (placeableAreaVisualizer != null)
        {
            Debug.Log($"可放置区域可视化器 textureSize: {placeableAreaVisualizer.textureSize}");
        }
        
        // 测试网格边界测试
        if (gridBoundsTest != null)
        {
            Debug.Log($"网格边界测试 textureSize: {gridBoundsTest.textureSize}");
        }
        
        // 测试GUI事件测试
        if (guiEventTest != null)
        {
            Debug.Log($"GUI事件测试 textureSize: {guiEventTest.textureSize}");
        }
        
        // 验证所有textureSize是否一致
        ValidateTextureSizes();
        
        Debug.Log("=== 纹理大小测试完成 ===");
    }
    
    void ValidateTextureSizes()
    {
        int[] textureSizes = new int[4];
        string[] componentNames = new string[4];
        int index = 0;
        
        if (levelEditor != null)
        {
            textureSizes[index] = levelEditor.textureSize;
            componentNames[index] = "主编辑器";
            index++;
        }
        
        if (placeableAreaVisualizer != null)
        {
            textureSizes[index] = placeableAreaVisualizer.textureSize;
            componentNames[index] = "可放置区域可视化器";
            index++;
        }
        
        if (gridBoundsTest != null)
        {
            textureSizes[index] = gridBoundsTest.textureSize;
            componentNames[index] = "网格边界测试";
            index++;
        }
        
        if (guiEventTest != null)
        {
            textureSizes[index] = guiEventTest.textureSize;
            componentNames[index] = "GUI事件测试";
            index++;
        }
        
        // 检查是否所有textureSize都相同
        bool allSame = true;
        int firstSize = textureSizes[0];
        
        for (int i = 1; i < index; i++)
        {
            if (textureSizes[i] != firstSize)
            {
                allSame = false;
                Debug.LogWarning($"纹理大小不一致: {componentNames[0]}={firstSize}, {componentNames[i]}={textureSizes[i]}");
            }
        }
        
        if (allSame)
        {
            Debug.Log($"✅ 所有组件的textureSize都一致: {firstSize}");
        }
        else
        {
            Debug.LogError("❌ 发现textureSize不一致的组件！");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 870, 240, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("纹理大小测试", GUI.skin.box);
        
        runTextureTest = GUILayout.Toggle(runTextureTest, "启用自动测试");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunTextureTest();
        }
        
        if (GUILayout.Button("同步所有textureSize"))
        {
            SyncAllTextureSizes();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void SyncAllTextureSizes()
    {
        if (levelEditor == null) return;
        
        int targetSize = levelEditor.textureSize;
        
        if (placeableAreaVisualizer != null)
        {
            placeableAreaVisualizer.textureSize = targetSize;
        }
        
        if (gridBoundsTest != null)
        {
            gridBoundsTest.textureSize = targetSize;
        }
        
        if (guiEventTest != null)
        {
            guiEventTest.textureSize = targetSize;
        }
        
        Debug.Log($"已同步所有组件的textureSize为: {targetSize}");
    }
} 