using UnityEngine;

public class LevelIDSyncTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runSyncTest = true;
    public float testInterval = 5f;
    
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
        
        Debug.Log("关卡ID同步测试脚本已启动");
        lastTestTime = Time.time;
    }
    
    void Update()
    {
        if (!runSyncTest) return;
        
        if (Time.time - lastTestTime >= testInterval)
        {
            RunSyncTest();
            lastTestTime = Time.time;
        }
    }
    
    void RunSyncTest()
    {
        Debug.Log("=== 关卡ID同步测试 ===");
        
        // 测试2D编辑器
        if (editor2D != null)
        {
            Test2DLevelIDSync();
        }
        
        Debug.Log("=== 同步测试完成 ===");
    }
    
    void Test2DLevelIDSync()
    {
        Debug.Log("测试2D编辑器关卡ID同步...");
        
        int currentID = editor2D.currentLevelId;
        string currentName = editor2D.currentLevelName;
        
        Debug.Log($"2D编辑器当前状态 - 关卡ID: {currentID}, 关卡名称: {currentName}");
        
        // 测试加载不同关卡
        int testLevelId = Random.Range(1, 10);
        Debug.Log($"测试加载2D关卡: {testLevelId}");
        
        editor2D.currentLevelId = testLevelId;
        editor2D.LoadLevel(testLevelId);
        
        // 验证同步
        if (editor2D.currentLevelId == testLevelId)
        {
            Debug.Log($"✅ 2D编辑器关卡ID同步成功: {editor2D.currentLevelId}");
        }
        else
        {
            Debug.LogWarning($"⚠️ 2D编辑器关卡ID同步失败: 期望={testLevelId}, 实际={editor2D.currentLevelId}");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250, 590, 240, 120));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("关卡ID同步测试", GUI.skin.box);
        
        runSyncTest = GUILayout.Toggle(runSyncTest, "启用自动测试");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("手动运行测试"))
        {
            RunSyncTest();
        }
        
        if (GUILayout.Button("测试2D关卡切换"))
        {
            if (editor2D != null)
            {
                int newLevelId = Random.Range(1, 10);
                Debug.Log($"手动切换2D关卡到: {newLevelId}");
                editor2D.currentLevelId = newLevelId;
                editor2D.LoadLevel(newLevelId);
            }
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 