using UnityEngine;

public class TestCardSize : MonoBehaviour
{
    public SheepLevelEditor2D editor2D;
    
    void Start()
    {
        // 查找编辑器
        if (editor2D == null)
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
            
        Debug.Log("🧪 卡片大小测试脚本已启动");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestCardSizeChanges();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            TestMouseHoverEffect();
        }
        
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestCardSizeDirectly();
        }
    }
    
    void TestCardSizeChanges()
    {
        Debug.Log("🧪 测试卡片大小变化...");
        
        if (editor2D != null)
        {
            Debug.Log($"2D编辑器卡片大小: {editor2D.cardSize}");
            // 改变卡片大小
            editor2D.cardSize = 1.2f;
            editor2D.UpdateAllCardSizes();
            Debug.Log($"2D编辑器卡片大小已改为: {editor2D.cardSize}");
        }
        else
        {
            Debug.Log("ℹ️ 未找到2D编辑器");
        }
    }
    
    void TestCardSizeDirectly()
    {
        Debug.Log("🧪 直接测试卡片大小变化...");
        
        // 直接查找所有卡片对象并修改它们的大小
        CardObject2D[] cards2D = FindObjectsOfType<CardObject2D>();
        
        Debug.Log($"找到 {cards2D.Length} 个2D卡片");
        
        foreach (var card in cards2D)
        {
            card.baseCardSize = 1.5f;
            card.transform.localScale = Vector3.one * 1.5f;
        }
        
        Debug.Log("✅ 直接修改卡片大小完成");
    }
    
    void TestMouseHoverEffect()
    {
        Debug.Log("🧪 测试鼠标悬停效果...");
        Debug.Log("请将鼠标悬停在卡片上，然后移开，观察卡片大小是否保持正确");
        Debug.Log("卡片应该基于用户设置的大小进行悬停效果，而不是固定值");
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 150));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("卡片大小测试");
        if (GUILayout.Button("T键: 测试大小变化"))
        {
            TestCardSizeChanges();
        }
        if (GUILayout.Button("U键: 直接测试"))
        {
            TestCardSizeDirectly();
        }
        if (GUILayout.Button("Y键: 测试悬停效果"))
        {
            TestMouseHoverEffect();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 