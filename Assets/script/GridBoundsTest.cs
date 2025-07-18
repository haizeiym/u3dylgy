using UnityEngine;

public class GridBoundsTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runBoundsTest = true;
    public float testInterval = 3f;
    public bool showGridBounds = true;
    
    private SheepLevelEditor2D editor2D;
    private float lastTestTime;
    
    // 边界可视化对象
    private GameObject boundsVisualizer2D;
    
    void Start()
    {
        // 查找编辑器组件
        editor2D = FindObjectOfType<SheepLevelEditor2D>();
        
        if (editor2D == null)
        {
            Debug.LogError("未找到编辑器组件！请确保场景中有SheepLevelEditor2D组件。");
            return;
        }
        
        Debug.Log("网格边界测试脚本已启动");
        lastTestTime = Time.time;
        
        // 创建边界可视化
        if (showGridBounds)
        {
            CreateBoundsVisualizers();
        }
    }
    
    void Update()
    {
        if (!runBoundsTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunBoundsTest();
            lastTestTime = Time.time;
        }
        
        // 更新边界可视化
        if (showGridBounds)
        {
            UpdateBoundsVisualizers();
        }
    }
    
    void CreateBoundsVisualizers()
    {
        // 创建2D边界可视化
        if (editor2D != null)
        {
            boundsVisualizer2D = new GameObject("GridBounds2D");
            boundsVisualizer2D.transform.position = Vector3.zero;
            
            // 添加SpriteRenderer
            SpriteRenderer spriteRenderer = boundsVisualizer2D.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateBoundsSprite();
            spriteRenderer.color = new Color(1f, 1f, 0f, 0.3f); // 半透明黄色
            spriteRenderer.sortingOrder = -1; // 确保在背景显示
        }
    }
    
    Sprite CreateBoundsSprite()
    {
        // 创建边界精灵
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // 创建边框效果
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                if (x < 2 || x >= textureSize - 2 || y < 2 || y >= textureSize - 2)
                {
                    texture.SetPixel(x, y, Color.yellow);
                }
                else
                {
                    texture.SetPixel(x, y, new Color(1f, 1f, 0f, 0.1f));
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    void UpdateBoundsVisualizers()
    {
        // 更新2D边界可视化
        if (boundsVisualizer2D != null && editor2D != null)
        {
            float halfGridWidth = (editor2D.gridSize.x - 1) * editor2D.cardSpacing * 0.5f;
            float halfGridHeight = (editor2D.gridSize.y - 1) * editor2D.cardSpacing * 0.5f;
            
            boundsVisualizer2D.transform.localScale = new Vector3(halfGridWidth * 2, halfGridHeight * 2, 1);
            boundsVisualizer2D.transform.position = new Vector3(0, 0, -editor2D.selectedLayer * 0.1f - 0.05f);
        }
    }
    
    void RunBoundsTest()
    {
        Debug.Log("=== 网格边界测试 ===");
        
        // 测试2D编辑器
        if (editor2D != null)
        {
            Test2DGridBounds();
        }
        
        Debug.Log("=== 边界测试完成 ===");
    }
    
    void Test2DGridBounds()
    {
        Debug.Log("测试2D编辑器网格边界...");
        
        float halfGridWidth = (editor2D.gridSize.x - 1) * editor2D.cardSpacing * 0.5f;
        float halfGridHeight = (editor2D.gridSize.y - 1) * editor2D.cardSpacing * 0.5f;
        
        Debug.Log($"2D网格边界: ±({halfGridWidth}, {halfGridHeight})");
        
        // 测试边界内外的位置
        Vector2[] testPositions = {
            Vector2.zero, // 中心
            new Vector2(halfGridWidth, 0), // 右边界
            new Vector2(-halfGridWidth, 0), // 左边界
            new Vector2(0, halfGridHeight), // 上边界
            new Vector2(0, -halfGridHeight), // 下边界
            new Vector2(halfGridWidth + 1, 0), // 超出右边界
            new Vector2(0, halfGridHeight + 1), // 超出上边界
        };
        
        foreach (var pos in testPositions)
        {
            bool inBounds = IsPositionInGridBounds2D(pos);
            Debug.Log($"位置 {pos}: 在边界内 = {inBounds}");
        }
    }
    
    bool IsPositionInGridBounds2D(Vector2 position)
    {
        if (editor2D == null) return false;
        
        float halfGridWidth = (editor2D.gridSize.x - 1) * editor2D.cardSpacing * 0.5f;
        float halfGridHeight = (editor2D.gridSize.y - 1) * editor2D.cardSpacing * 0.5f;
        
        return Mathf.Abs(position.x) <= halfGridWidth && Mathf.Abs(position.y) <= halfGridHeight;
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 750, 240, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("网格边界测试", GUI.skin.box);
        
        runBoundsTest = GUILayout.Toggle(runBoundsTest, "启用自动测试");
        showGridBounds = GUILayout.Toggle(showGridBounds, "显示边界可视化");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunBoundsTest();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void OnDestroy()
    {
        // 清理可视化对象
        if (boundsVisualizer2D != null)
        {
            DestroyImmediate(boundsVisualizer2D);
        }
    }
} 