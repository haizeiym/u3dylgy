using UnityEngine;
using System.Collections;

public class GridLinesTest : MonoBehaviour
{
    [Header("测试设置")]
    public PlaceableAreaVisualizer visualizer;
    public SheepLevelEditor2D editor2D;
    public float testDuration = 10f;
    public float checkInterval = 1f;
    
    [Header("测试状态")]
    public bool isTestRunning = false;
    public float testTime = 0f;
    public int gridLinesCount = 0;
    public int lastGridLinesCount = 0;
    public string testStatus = "未开始";
    
    void Start()
    {
        if (visualizer == null)
        {
            visualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        }
        
        if (editor2D == null)
        {
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (visualizer == null || editor2D == null)
        {
            Debug.LogError("未找到PlaceableAreaVisualizer或SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("GridLines测试脚本已初始化");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F3))
        {
            StartGridLinesTest();
        }
        
        if (Input.GetKeyDown(KeyCode.F4))
        {
            StopTest();
        }
        
        // 实时监控GridLines数量
        if (isTestRunning)
        {
            CountGridLines();
        }
    }
    
    [ContextMenu("开始GridLines测试")]
    public void StartGridLinesTest()
    {
        if (isTestRunning)
        {
            Debug.LogWarning("测试正在进行中，请先停止当前测试");
            return;
        }
        
        StartCoroutine(RunGridLinesTest());
    }
    
    IEnumerator RunGridLinesTest()
    {
        isTestRunning = true;
        testTime = 0f;
        testStatus = "开始测试";
        
        Debug.Log("=== 开始GridLines累积测试 ===");
        
        // 步骤1: 确保可视化器可见
        testStatus = "步骤1: 设置可视化器";
        Debug.Log("步骤1: 设置可视化器");
        
        if (visualizer != null)
        {
            visualizer.SetVisible(true);
        }
        
        yield return new WaitForSeconds(1f);
        
        // 步骤2: 记录初始GridLines数量
        testStatus = "步骤2: 记录初始状态";
        Debug.Log("步骤2: 记录初始状态");
        
        CountGridLines();
        lastGridLinesCount = gridLinesCount;
        Debug.Log($"初始GridLines数量: {gridLinesCount}");
        
        yield return new WaitForSeconds(1f);
        
        // 步骤3: 运行测试，监控GridLines数量变化
        testStatus = "步骤3: 监控GridLines变化";
        Debug.Log("步骤3: 监控GridLines变化");
        
        float lastCheckTime = 0f;
        
        while (testTime < testDuration)
        {
            testTime += Time.deltaTime;
            
            // 定期检查GridLines数量
            if (testTime - lastCheckTime >= checkInterval)
            {
                lastCheckTime = testTime;
                CountGridLines();
                
                if (gridLinesCount != lastGridLinesCount)
                {
                    Debug.LogWarning($"GridLines数量发生变化: {lastGridLinesCount} -> {gridLinesCount} (时间: {testTime:F1}s)");
                    lastGridLinesCount = gridLinesCount;
                }
                else
                {
                    Debug.Log($"GridLines数量稳定: {gridLinesCount} (时间: {testTime:F1}s)");
                }
            }
            
            yield return null;
        }
        
        // 步骤4: 测试结果分析
        testStatus = "步骤4: 分析测试结果";
        Debug.Log("步骤4: 分析测试结果");
        
        CountGridLines();
        
        if (gridLinesCount == lastGridLinesCount)
        {
            Debug.Log("✓ 测试通过: GridLines数量保持稳定，没有累积");
            testStatus = "测试通过: 无累积";
        }
        else
        {
            Debug.LogError($"✗ 测试失败: GridLines数量从 {lastGridLinesCount} 增加到 {gridLinesCount}");
            testStatus = "测试失败: 有累积";
        }
        
        // 步骤5: 详细检查场景中的GridLines对象
        testStatus = "步骤5: 详细检查";
        Debug.Log("步骤5: 详细检查场景中的GridLines对象");
        
        GameObject[] gridLinesObjects = GameObject.FindGameObjectsWithTag("GridLines");
        Debug.Log($"场景中找到 {gridLinesObjects.Length} 个GridLines标签的GameObject");
        
        GameObject[] gridLineObjects = GameObject.FindGameObjectsWithTag("GridLine");
        Debug.Log($"场景中找到 {gridLineObjects.Length} 个GridLine标签的GameObject");
        
        // 查找所有可能的网格线对象
        GameObject[] allGridLines = GameObject.FindGameObjectsWithTag("GridLines");
        if (allGridLines.Length > 1)
        {
            Debug.LogWarning("✗ 场景中有多个GridLines对象");
            foreach (var obj in allGridLines)
            {
                Debug.LogWarning($"  重复的GridLines对象: {obj.name}");
            }
        }
        else if (allGridLines.Length == 1)
        {
            Debug.Log("✓ 场景中只有一个GridLines对象");
        }
        else
        {
            Debug.Log("场景中没有GridLines对象");
        }
        
        yield return new WaitForSeconds(2f);
        
        // 测试完成
        testStatus = "测试完成";
        isTestRunning = false;
        
        Debug.Log("=== GridLines累积测试完成 ===");
    }
    
    void CountGridLines()
    {
        if (visualizer == null) return;
        
        // 通过反射获取gridLinesObject
        var gridLinesField = typeof(PlaceableAreaVisualizer).GetField("gridLinesObject", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (gridLinesField != null)
        {
            GameObject gridLinesObj = gridLinesField.GetValue(visualizer) as GameObject;
            if (gridLinesObj != null)
            {
                gridLinesCount = gridLinesObj.transform.childCount;
            }
            else
            {
                gridLinesCount = 0;
            }
        }
        else
        {
            gridLinesCount = 0;
        }
    }
    
    [ContextMenu("停止测试")]
    public void StopTest()
    {
        if (isTestRunning)
        {
            StopAllCoroutines();
            isTestRunning = false;
            testTime = 0f;
            testStatus = "测试已停止";
            Debug.Log("GridLines测试已停止");
        }
    }
    
    [ContextMenu("手动检查GridLines")]
    public void ManualCheckGridLines()
    {
        CountGridLines();
        Debug.Log($"当前GridLines数量: {gridLinesCount}");
        
        // 查找所有GridLines相关的对象
        GameObject[] gridLinesObjects = GameObject.FindGameObjectsWithTag("GridLines");
        Debug.Log($"场景中GridLines标签对象数量: {gridLinesObjects.Length}");
        
        GameObject[] gridLineObjects = GameObject.FindGameObjectsWithTag("GridLine");
        Debug.Log($"场景中GridLine标签对象数量: {gridLineObjects.Length}");
        
        // 查找所有包含"GridLine"名称的对象
        GameObject[] allGridLineObjects = FindObjectsOfType<GameObject>();
        int gridLineNameCount = 0;
        foreach (var obj in allGridLineObjects)
        {
            if (obj.name.Contains("GridLine"))
            {
                gridLineNameCount++;
                Debug.Log($"找到GridLine对象: {obj.name}");
            }
        }
        Debug.Log($"包含GridLine名称的对象总数: {gridLineNameCount}");
    }
    
    void OnGUI()
    {
        if (visualizer == null) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 200, 240, 180));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("GridLines测试", GUI.skin.box);
        GUILayout.Space(5);
        
        GUILayout.Label($"测试状态: {testStatus}");
        if (isTestRunning)
        {
            GUILayout.Label($"测试时间: {testTime:F1}s / {testDuration}s");
        }
        
        GUILayout.Space(5);
        
        if (!isTestRunning)
        {
            if (GUILayout.Button("开始测试 (F3)"))
            {
                StartGridLinesTest();
            }
        }
        else
        {
            if (GUILayout.Button("停止测试 (F4)"))
            {
                StopTest();
            }
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("GridLines监控:");
        GUILayout.Label($"当前数量: {gridLinesCount}");
        GUILayout.Label($"上次数量: {lastGridLinesCount}");
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("手动检查"))
        {
            ManualCheckGridLines();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 