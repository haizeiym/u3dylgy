using UnityEngine;
using System.Collections;

public class LayerMaskFixTest : MonoBehaviour
{
    [Header("测试设置")]
    public SheepLevelEditor2D editor2D;
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
        
        Debug.Log("层级遮罩修复测试脚本已初始化");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F6))
        {
            StartLayerMaskTest();
        }
        
        if (Input.GetKeyDown(KeyCode.F7))
        {
            StopTest();
        }
    }
    
    [ContextMenu("开始层级遮罩测试")]
    public void StartLayerMaskTest()
    {
        if (isTestRunning)
        {
            Debug.LogWarning("测试正在进行中，请先停止当前测试");
            return;
        }
        
        StartCoroutine(RunLayerMaskTest());
    }
    
    IEnumerator RunLayerMaskTest()
    {
        isTestRunning = true;
        currentTestStep = 0;
        testStatus = "开始测试";
        
        Debug.Log("=== 开始层级遮罩修复测试 ===");
        
        // 步骤1: 检查初始状态
        currentTestStep = 1;
        testStatus = "步骤1: 检查初始状态";
        Debug.Log("步骤1: 检查初始状态");
        
        Debug.Log($"初始状态 - 显示层级遮罩: {editor2D.showLayerMasks}");
        Debug.Log($"初始状态 - 当前层级: {editor2D.selectedLayer}");
        Debug.Log($"初始状态 - 总层数: {editor2D.totalLayers}");
        
        int initialMaskCount = GetLayerMaskCount();
        Debug.Log($"初始状态 - 层级遮罩数量: {initialMaskCount}");
        
        yield return new WaitForSeconds(testDelay);
        
        // 步骤2: 测试开启层级遮罩
        currentTestStep = 2;
        testStatus = "步骤2: 测试开启层级遮罩";
        Debug.Log("步骤2: 测试开启层级遮罩");
        
        if (!editor2D.showLayerMasks)
        {
            Debug.Log("开启层级遮罩...");
            editor2D.showLayerMasks = true;
            editor2D.SendMessage("CreateLayerMasks");
            
            yield return new WaitForSeconds(1f);
            
            int maskCountAfterEnable = GetLayerMaskCount();
            Debug.Log($"开启后 - 层级遮罩数量: {maskCountAfterEnable}");
            
            if (maskCountAfterEnable > 0)
            {
                Debug.Log("✓ 层级遮罩开启成功");
            }
            else
            {
                Debug.LogError("✗ 层级遮罩开启失败");
            }
        }
        else
        {
            Debug.Log("层级遮罩已经开启，跳过此步骤");
        }
        
        yield return new WaitForSeconds(testDelay);
        
        // 步骤3: 测试层级切换
        currentTestStep = 3;
        testStatus = "步骤3: 测试层级切换";
        Debug.Log("步骤3: 测试层级切换");
        
        int originalLayer = editor2D.selectedLayer;
        Debug.Log($"原始层级: {originalLayer}");
        
        // 切换到下一个层级
        int newLayer = (originalLayer + 1) % editor2D.totalLayers;
        Debug.Log($"切换到层级: {newLayer}");
        
        editor2D.selectedLayer = newLayer;
        editor2D.SendMessage("UpdateLayerMasks");
        
        yield return new WaitForSeconds(1f);
        
        // 检查遮罩显示状态
        CheckLayerMaskVisibility();
        
        yield return new WaitForSeconds(testDelay);
        
        // 步骤4: 测试透明度调整
        currentTestStep = 4;
        testStatus = "步骤4: 测试透明度调整";
        Debug.Log("步骤4: 测试透明度调整");
        
        Color originalColor = editor2D.layerMaskColor;
        Debug.Log($"原始透明度: {originalColor.a}");
        
        // 调整透明度
        Color newColor = originalColor;
        newColor.a = 0.8f;
        editor2D.layerMaskColor = newColor;
        editor2D.SendMessage("UpdateLayerMasks");
        
        yield return new WaitForSeconds(1f);
        
        Debug.Log($"新透明度: {newColor.a}");
        Debug.Log("✓ 透明度调整测试完成");
        
        yield return new WaitForSeconds(testDelay);
        
        // 步骤5: 测试关闭层级遮罩
        currentTestStep = 5;
        testStatus = "步骤5: 测试关闭层级遮罩";
        Debug.Log("步骤5: 测试关闭层级遮罩");
        
        Debug.Log("关闭层级遮罩...");
        editor2D.showLayerMasks = false;
        editor2D.SendMessage("ClearLayerMasks");
        
        yield return new WaitForSeconds(1f);
        
        int maskCountAfterDisable = GetLayerMaskCount();
        Debug.Log($"关闭后 - 层级遮罩数量: {maskCountAfterDisable}");
        
        if (maskCountAfterDisable == 0)
        {
            Debug.Log("✓ 层级遮罩关闭成功");
        }
        else
        {
            Debug.LogError("✗ 层级遮罩关闭失败");
        }
        
        yield return new WaitForSeconds(testDelay);
        
        // 步骤6: 恢复原始状态
        currentTestStep = 6;
        testStatus = "步骤6: 恢复原始状态";
        Debug.Log("步骤6: 恢复原始状态");
        
        editor2D.selectedLayer = originalLayer;
        editor2D.layerMaskColor = originalColor;
        editor2D.showLayerMasks = true;
        editor2D.SendMessage("CreateLayerMasks");
        
        yield return new WaitForSeconds(1f);
        
        Debug.Log("✓ 原始状态已恢复");
        
        // 测试完成
        currentTestStep = 0;
        testStatus = "测试完成";
        isTestRunning = false;
        
        Debug.Log("=== 层级遮罩修复测试完成 ===");
    }
    
    int GetLayerMaskCount()
    {
        if (editor2D == null) return 0;
        
        // 通过反射获取layerMaskObjects列表
        var layerMaskObjectsField = typeof(SheepLevelEditor2D).GetField("layerMaskObjects", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (layerMaskObjectsField != null)
        {
            var layerMaskObjects = layerMaskObjectsField.GetValue(editor2D) as System.Collections.Generic.List<GameObject>;
            return layerMaskObjects != null ? layerMaskObjects.Count : 0;
        }
        
        return 0;
    }
    
    void CheckLayerMaskVisibility()
    {
        if (editor2D == null) return;
        
        // 通过反射获取layerMaskObjects列表
        var layerMaskObjectsField = typeof(SheepLevelEditor2D).GetField("layerMaskObjects", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (layerMaskObjectsField != null)
        {
            var layerMaskObjects = layerMaskObjectsField.GetValue(editor2D) as System.Collections.Generic.List<GameObject>;
            
            if (layerMaskObjects != null)
            {
                Debug.Log($"检查 {layerMaskObjects.Count} 个层级遮罩的可见性:");
                
                for (int i = 0; i < layerMaskObjects.Count; i++)
                {
                    if (layerMaskObjects[i] != null)
                    {
                        SpriteRenderer renderer = layerMaskObjects[i].GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            bool shouldShow = editor2D.showLayerMasks && i != editor2D.selectedLayer;
                            bool isVisible = renderer.enabled;
                            
                            Debug.Log($"  层级 {i}: 应该显示={shouldShow}, 实际显示={isVisible}, 颜色={renderer.color}");
                            
                            if (shouldShow != isVisible)
                            {
                                Debug.LogWarning($"  层级 {i} 显示状态不匹配！");
                            }
                        }
                    }
                }
            }
        }
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
            Debug.Log("层级遮罩测试已停止");
        }
    }
    
    [ContextMenu("手动检查层级遮罩")]
    public void ManualCheckLayerMasks()
    {
        Debug.Log("=== 手动检查层级遮罩 ===");
        
        if (editor2D == null)
        {
            Debug.LogError("编辑器组件未找到");
            return;
        }
        
        Debug.Log($"显示层级遮罩: {editor2D.showLayerMasks}");
        Debug.Log($"当前层级: {editor2D.selectedLayer}");
        Debug.Log($"总层数: {editor2D.totalLayers}");
        Debug.Log($"遮罩颜色: {editor2D.layerMaskColor}");
        
        int maskCount = GetLayerMaskCount();
        Debug.Log($"层级遮罩数量: {maskCount}");
        
        CheckLayerMaskVisibility();
        
        // 查找场景中的遮罩对象
        GameObject[] maskObjects = GameObject.FindGameObjectsWithTag("LayerMask");
        Debug.Log($"场景中LayerMask标签对象数量: {maskObjects.Length}");
        
        GameObject[] allMaskObjects = FindObjectsOfType<GameObject>();
        int maskNameCount = 0;
        foreach (var obj in allMaskObjects)
        {
            if (obj.name.Contains("LayerMask"))
            {
                maskNameCount++;
                Debug.Log($"找到遮罩对象: {obj.name}");
            }
        }
        Debug.Log($"包含LayerMask名称的对象总数: {maskNameCount}");
        
        Debug.Log("=== 手动检查完成 ===");
    }
    
    void OnGUI()
    {
        if (editor2D == null) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 350, 240, 330));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("层级遮罩修复测试", GUI.skin.box);
        GUILayout.Space(5);
        
        GUILayout.Label($"测试状态: {testStatus}");
        if (isTestRunning)
        {
            GUILayout.Label($"当前步骤: {currentTestStep}/6");
        }
        
        GUILayout.Space(5);
        
        if (!isTestRunning)
        {
            if (GUILayout.Button("开始测试 (F6)"))
            {
                StartLayerMaskTest();
            }
        }
        else
        {
            if (GUILayout.Button("停止测试 (F7)"))
            {
                StopTest();
            }
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("层级遮罩状态:");
        GUILayout.Label($"显示遮罩: {editor2D.showLayerMasks}");
        GUILayout.Label($"当前层级: {editor2D.selectedLayer}");
        GUILayout.Label($"总层数: {editor2D.totalLayers}");
        GUILayout.Label($"遮罩数量: {GetLayerMaskCount()}");
        GUILayout.Label($"透明度: {editor2D.layerMaskColor.a:F2}");
        
        GUILayout.Space(5);
        
        if (GUILayout.Button("手动检查"))
        {
            ManualCheckLayerMasks();
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("测试说明:");
        GUILayout.Label("1. 测试开启/关闭遮罩");
        GUILayout.Label("2. 测试层级切换");
        GUILayout.Label("3. 测试透明度调整");
        GUILayout.Label("4. 验证遮罩显示逻辑");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 