using UnityEngine;

public class GridMaskAlignmentTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runAlignmentTest = true;
    public float testInterval = 3f;
    
    private SheepLevelEditor2D editor2D;
    private float lastTestTime;
    
    void Start()
    {
        // 查找编辑器组件
        editor2D = FindObjectOfType<SheepLevelEditor2D>();
        
        if (editor2D == null)
        {
            Debug.LogError("未找到编辑器组件！请确保场景中有SheepLevelEditor2D组件。");
            return;
        }
        
        Debug.Log("网格遮罩对齐测试脚本已启动");
        lastTestTime = Time.time;
    }
    
    void Update()
    {
        if (!runAlignmentTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunAlignmentTest();
            lastTestTime = Time.time;
        }
    }
    
    void RunAlignmentTest()
    {
        Debug.Log("=== 网格遮罩对齐测试 ===");
        
        // 测试2D编辑器
        if (editor2D != null)
        {
            Test2DAlignment();
        }
        
        Debug.Log("=== 对齐测试完成 ===");
    }
    
    void Test2DAlignment()
    {
        Debug.Log("测试2D编辑器网格遮罩对齐...");
        
        // 检查网格大小设置
        Vector2 expectedGridSize = editor2D.gridSize;
        float expectedSpacing = editor2D.cardSpacing;
        float expectedWidth = expectedGridSize.x * expectedSpacing;
        float expectedHeight = expectedGridSize.y * expectedSpacing;
        
        Debug.Log($"2D编辑器 - 期望网格大小: {expectedWidth} x {expectedHeight}");
        Debug.Log($"2D编辑器 - 网格设置: {expectedGridSize}, 间距: {expectedSpacing}");
        
        // 检查2D网格背景
        GameObject gridBackground = GameObject.Find("GridBackground");
        if (gridBackground != null)
        {
            Vector3 gridScale = gridBackground.transform.localScale;
            float gridWidth = gridScale.x;
            float gridHeight = gridScale.y;
            
            Debug.Log($"2D网格背景实际大小: {gridWidth} x {gridHeight}");
            
            bool gridAligned = Mathf.Approximately(gridWidth, expectedWidth) && 
                              Mathf.Approximately(gridHeight, expectedHeight);
            
            if (gridAligned)
            {
                Debug.Log("✅ 2D网格背景大小正确");
            }
            else
            {
                Debug.LogWarning("⚠️ 2D网格背景大小不匹配！");
            }
        }
        
        // 检查层级遮罩
        int maskCount = 0;
        foreach (Transform child in editor2D.transform)
        {
            if (child.name.StartsWith("LayerMask_"))
            {
                maskCount++;
                Vector3 maskScale = child.localScale;
                float maskWidth = maskScale.x;
                float maskHeight = maskScale.y;
                
                Debug.Log($"2D遮罩 {child.name} 大小: {maskWidth} x {maskHeight}");
                
                bool maskAligned = Mathf.Approximately(maskWidth, expectedWidth) && 
                                  Mathf.Approximately(maskHeight, expectedHeight);
                
                if (maskAligned)
                {
                    Debug.Log($"✅ 2D遮罩 {child.name} 大小正确");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 2D遮罩 {child.name} 大小不匹配！");
                }
            }
        }
        
        Debug.Log($"2D编辑器找到 {maskCount} 个层级遮罩");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 220, 190, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("网格遮罩对齐测试", GUI.skin.box);
        
        runAlignmentTest = GUILayout.Toggle(runAlignmentTest, "启用自动测试");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunAlignmentTest();
        }
        
        if (GUILayout.Button("调整2D网格大小"))
        {
            if (editor2D != null)
            {
                editor2D.gridSize = new Vector2(Random.Range(6, 12), Random.Range(6, 12));
                editor2D.cardSpacing = Random.Range(1.0f, 1.5f);
                editor2D.UpdateGridAndMasks();
                Debug.Log($"2D网格已调整为: {editor2D.gridSize}, 间距: {editor2D.cardSpacing}");
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 