using UnityEngine;

public class MultiLayerTest : MonoBehaviour
{
    public SheepLevelEditor2D editor2D;
    
    void Start()
    {
        // 查找编辑器
        if (editor2D == null)
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
            
        Debug.Log("🏗️ 多层卡片测试已启动");
        Debug.Log("测试不同层级卡片可以重叠放置");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.M))
        {
            TestMultiLayerPlacement();
        }
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            ShowLayerInfo();
        }
    }
    
    void TestMultiLayerPlacement()
    {
        Debug.Log("=== 多层卡片放置测试 ===");
        
        if (editor2D != null)
        {
            Debug.Log("测试2D编辑器多层放置...");
            Test2DMultiLayer();
        }
        
        Debug.Log("=== 测试完成 ===");
    }
    
    void Test2DMultiLayer()
    {
        // 在位置(0,0)放置不同层级的卡片
        Vector2 testPosition = new Vector2(0, 0);
        
        for (int layer = 0; layer < 3; layer++)
        {
            editor2D.selectedLayer = layer;
            
            // 检查位置是否被占用（应该只检查同一层级）
            bool isOccupied = IsPositionOccupied2D(testPosition, layer);
            Debug.Log($"2D层级{layer}: 位置{testPosition} 是否被占用: {isOccupied}");
            
            if (!isOccupied)
            {
                // 模拟放置卡片
                Debug.Log($"✅ 可以在2D层级{layer}放置卡片");
            }
            else
            {
                Debug.Log($"❌ 2D层级{layer}位置被占用");
            }
        }
    }
    
    bool IsPositionOccupied2D(Vector2 position, int layer)
    {
        var cardObjects = FindObjectsOfType<CardObject2D>();
        foreach (var card in cardObjects)
        {
            if (card.layer == layer) // 只检查指定层级
            {
                float distance = Vector2.Distance(card.transform.position, position);
                if (distance < 0.5f) // 使用固定值避免访问私有字段
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    void ShowLayerInfo()
    {
        Debug.Log("=== 层级信息 ===");
        
        if (editor2D != null)
        {
            Debug.Log($"2D编辑器当前层级: {editor2D.selectedLayer}");
            var cards2D = FindObjectsOfType<CardObject2D>();
            for (int i = 0; i < 3; i++)
            {
                int count = 0;
                foreach (var card in cards2D)
                {
                    if (card.layer == i) count++;
                }
                Debug.Log($"2D层级{i}: {count}张卡片");
            }
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🏗️ 多层测试");
        if (GUILayout.Button("M键: 测试多层放置"))
        {
            TestMultiLayerPlacement();
        }
        if (GUILayout.Button("L键: 显示层级信息"))
        {
            ShowLayerInfo();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 