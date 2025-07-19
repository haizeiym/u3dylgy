using UnityEngine;

public class AreaSizeDebugger : MonoBehaviour
{
    [Header("区域大小调试")]
    public bool showDebugInfo = true;
    public bool logToConsole = true;
    
    private SheepLevelEditor2D levelEditor;
    private PlaceableAreaVisualizer placeableAreaVisualizer;
    
    void Start()
    {
        levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        placeableAreaVisualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        
        if (levelEditor == null)
        {
            Debug.LogError("未找到编辑器组件！");
            return;
        }
        
        Debug.Log("区域大小调试器已启动");
    }
    
    void Update()
    {
        if (showDebugInfo && levelEditor != null)
        {
            DisplayDebugInfo();
        }
    }
    
    void DisplayDebugInfo()
    {
        Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
        Vector2 gridSize = levelEditor.gridSize;
        float cardSpacing = levelEditor.cardSpacing;
        bool useCustom = levelEditor.useCustomAreaSize;
        Vector2 customAreaSize = levelEditor.areaSize;
        
        string debugInfo = $"=== 区域大小调试信息 ===\n" +
                          $"网格大小: {gridSize.x} x {gridSize.y}\n" +
                          $"卡片间距: {cardSpacing}\n" +
                          $"使用自定义区域: {useCustom}\n" +
                          $"自定义区域大小: {customAreaSize.x} x {customAreaSize.y}\n" +
                          $"实际区域大小: {actualAreaSize.x} x {actualAreaSize.y}\n" +
                          $"计算方式: {(useCustom ? "自定义" : $"网格大小 * 间距 = {gridSize.x} * {cardSpacing} = {gridSize.x * cardSpacing}")}\n" +
                          $"区域范围: X[{-actualAreaSize.x * 0.5f}, {actualAreaSize.x * 0.5f}], Y[{-actualAreaSize.y * 0.5f}, {actualAreaSize.y * 0.5f}]";
        
        if (logToConsole)
        {
            Debug.Log(debugInfo);
        }
    }
    
    void OnGUI()
    {
        if (!showDebugInfo || levelEditor == null) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 200, 400, 190));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("区域大小调试信息", GUI.skin.box);
        
        Vector2 actualAreaSize = levelEditor.GetActualAreaSize();
        Vector2 gridSize = levelEditor.gridSize;
        float cardSpacing = levelEditor.cardSpacing;
        bool useCustom = levelEditor.useCustomAreaSize;
        Vector2 customAreaSize = levelEditor.areaSize;
        
        GUILayout.Label($"网格大小: {gridSize.x} x {gridSize.y}");
        GUILayout.Label($"卡片间距: {cardSpacing}");
        GUILayout.Label($"使用自定义区域: {useCustom}");
        GUILayout.Label($"自定义区域大小: {customAreaSize.x:F2} x {customAreaSize.y:F2}");
        GUILayout.Label($"实际区域大小: {actualAreaSize.x:F2} x {actualAreaSize.y:F2}");
        GUILayout.Label($"区域范围: X[{-actualAreaSize.x * 0.5f:F2}, {actualAreaSize.x * 0.5f:F2}]");
        GUILayout.Label($"区域范围: Y[{-actualAreaSize.y * 0.5f:F2}, {actualAreaSize.y * 0.5f:F2}]");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("设置为16x16区域"))
        {
            levelEditor.useCustomAreaSize = true;
            levelEditor.areaSize = new Vector2(16f, 16f);
            levelEditor.UpdateGridAndMasks();
            Debug.Log("已设置为16x16区域");
        }
        
        if (GUILayout.Button("使用网格计算"))
        {
            levelEditor.useCustomAreaSize = false;
            levelEditor.UpdateGridAndMasks();
            Debug.Log("已切换为网格计算");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
} 