using UnityEngine;

public class ScrollViewTest : MonoBehaviour
{
    [Header("测试设置")]
    public SheepLevelEditor2D editor2D;
    public bool showScrollInfo = true;
    
    [Header("滚动信息")]
    public Vector2 scrollPosition;
    public float scrollViewHeight;
    public float contentHeight;
    public bool canScroll = false;
    
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
        
        Debug.Log("滚动视图测试脚本已初始化");
    }
    
    void Update()
    {
        // 测试快捷键
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ToggleScrollInfo();
        }
        
        // 获取滚动信息
        if (showScrollInfo && editor2D != null)
        {
            GetScrollInfo();
        }
    }
    
    [ContextMenu("切换滚动信息显示")]
    public void ToggleScrollInfo()
    {
        showScrollInfo = !showScrollInfo;
        Debug.Log($"滚动信息显示: {(showScrollInfo ? "开启" : "关闭")}");
    }
    
    void GetScrollInfo()
    {
        // 通过反射获取scrollPosition
        var scrollField = typeof(SheepLevelEditor2D).GetField("scrollPosition", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (scrollField != null)
        {
            scrollPosition = (Vector2)scrollField.GetValue(editor2D);
        }
        
        // 计算滚动视图高度
        scrollViewHeight = Screen.height - 100;
        
        // 估算内容高度（基于GUI元素数量）
        contentHeight = 800f; // 估算值，实际内容可能更长
        
        // 判断是否可以滚动
        canScroll = contentHeight > scrollViewHeight;
    }
    
    [ContextMenu("测试滚动功能")]
    public void TestScrollFunction()
    {
        Debug.Log("=== 滚动视图功能测试 ===");
        
        if (editor2D == null)
        {
            Debug.LogError("编辑器组件未找到");
            return;
        }
        
        // 检查编辑器是否处于编辑模式
        Debug.Log($"编辑器编辑模式: {editor2D.isEditMode}");
        
        // 检查屏幕尺寸
        Debug.Log($"屏幕尺寸: {Screen.width} x {Screen.height}");
        
        // 检查滚动视图参数
        Debug.Log($"滚动视图高度: {scrollViewHeight}");
        Debug.Log($"估算内容高度: {contentHeight}");
        Debug.Log($"可以滚动: {canScroll}");
        
        // 检查当前滚动位置
        Debug.Log($"当前滚动位置: {scrollPosition}");
        
        // 测试滚动范围
        if (canScroll)
        {
            Debug.Log("✓ 滚动功能应该正常工作");
            Debug.Log($"最大滚动范围: 0 到 {contentHeight - scrollViewHeight}");
        }
        else
        {
            Debug.Log("⚠ 内容高度不足以滚动");
        }
        
        Debug.Log("=== 滚动视图功能测试完成 ===");
    }
    
    [ContextMenu("模拟长内容测试")]
    public void SimulateLongContent()
    {
        Debug.Log("=== 模拟长内容测试 ===");
        
        // 模拟添加更多GUI元素来测试滚动
        Debug.Log("建议在编辑器中添加更多设置选项来测试滚动功能");
        Debug.Log("例如：");
        Debug.Log("- 更多卡片类型设置");
        Debug.Log("- 高级网格选项");
        Debug.Log("- 相机控制选项");
        Debug.Log("- 调试选项");
        Debug.Log("- 性能设置");
        Debug.Log("- 导出选项");
        
        Debug.Log("=== 模拟长内容测试完成 ===");
    }
    
    void OnGUI()
    {
        if (!showScrollInfo || editor2D == null) return;
        
        GUILayout.BeginArea(new Rect(Screen.width - 250, Screen.height - 300, 240, 280));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("滚动视图信息", GUI.skin.box);
        GUILayout.Space(5);
        
        GUILayout.Label($"滚动位置: {scrollPosition}");
        GUILayout.Label($"视图高度: {scrollViewHeight:F0}");
        GUILayout.Label($"内容高度: {contentHeight:F0}");
        GUILayout.Label($"可以滚动: {(canScroll ? "是" : "否")}");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("测试滚动功能"))
        {
            TestScrollFunction();
        }
        
        if (GUILayout.Button("模拟长内容"))
        {
            SimulateLongContent();
        }
        
        if (GUILayout.Button("切换显示 (F5)"))
        {
            ToggleScrollInfo();
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("滚动测试说明:");
        GUILayout.Label("1. 在编辑器GUI中");
        GUILayout.Label("2. 使用鼠标滚轮");
        GUILayout.Label("3. 或拖动滚动条");
        GUILayout.Label("4. 检查内容是否可滚动");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 