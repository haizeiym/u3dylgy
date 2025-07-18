using UnityEngine;

public class PlacementDebugger : MonoBehaviour
{
    public SheepLevelEditor2D editor2D;
    public bool showDebugInfo = true;
    
    void Start()
    {
        // 查找编辑器
        if (editor2D == null)
            editor2D = FindObjectOfType<SheepLevelEditor2D>();
            
        Debug.Log("🔍 卡片放置调试器已启动");
    }
    
    void Update()
    {
        if (!showDebugInfo) return;
        
        // 显示鼠标位置信息
        if (Input.GetMouseButtonDown(0))
        {
            DebugPlacementInfo();
        }
        
        // 按D键切换调试信息
        if (Input.GetKeyDown(KeyCode.D))
        {
            showDebugInfo = !showDebugInfo;
            Debug.Log($"调试信息: {(showDebugInfo ? "开启" : "关闭")}");
        }
    }
    
    void DebugPlacementInfo()
    {
        Debug.Log("=== 卡片放置调试信息 ===");
        
        // 鼠标屏幕坐标
        Vector3 mouseScreenPos = Input.mousePosition;
        Debug.Log($"鼠标屏幕坐标: {mouseScreenPos}");
        
        // 2D编辑器信息
        if (editor2D != null)
        {
            if (editor2D.editorCamera2D != null)
            {
                Vector3 mouseWorldPos = editor2D.editorCamera2D.ScreenToWorldPoint(mouseScreenPos);
                Debug.Log($"2D鼠标世界坐标: {mouseWorldPos}");
                
                Vector2 gridPos = SnapToGrid2D(new Vector2(mouseWorldPos.x, mouseWorldPos.y));
                Debug.Log($"2D网格坐标: {gridPos}");
                
                bool isOccupied = IsPositionOccupied2D(gridPos);
                Debug.Log($"2D位置是否被占用: {isOccupied}");
            }
            else
            {
                Debug.Log("2D编辑器相机未找到");
            }
        }
        else
        {
            Debug.Log("2D编辑器未找到");
        }
        
        Debug.Log("=== 调试信息结束 ===");
    }
    
    // 复制2D编辑器的网格对齐方法
    Vector2 SnapToGrid2D(Vector2 worldPos)
    {
        if (editor2D == null) return worldPos;
        
        float x = Mathf.Round(worldPos.x / editor2D.cardSpacing) * editor2D.cardSpacing;
        float y = Mathf.Round(worldPos.y / editor2D.cardSpacing) * editor2D.cardSpacing;
        return new Vector2(x, y);
    }
    
    // 复制2D编辑器的位置占用检查方法
    bool IsPositionOccupied2D(Vector2 position)
    {
        if (editor2D == null) return false;
        
        // 只检查同一层级的卡片是否占用位置
        var cardObjects = FindObjectsOfType<CardObject2D>();
        foreach (var card in cardObjects)
        {
            if (card.layer == editor2D.selectedLayer) // 只检查当前层级
            {
                float distance = Vector2.Distance(card.transform.position, position);
                if (distance < editor2D.cardSize * 0.5f)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        GUILayout.BeginArea(new Rect(10, Screen.height - 150, 300, 140));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("🔍 放置调试器");
        GUILayout.Label($"调试模式: {(showDebugInfo ? "开启" : "关闭")}");
        GUILayout.Label("左键: 显示放置信息");
        GUILayout.Label("D键: 切换调试模式");
        
        if (GUILayout.Button("显示当前状态"))
        {
            ShowCurrentStatus();
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    void ShowCurrentStatus()
    {
        Debug.Log("=== 当前编辑器状态 ===");
        
        if (editor2D != null)
        {
            Debug.Log($"2D编辑器: 卡片大小={editor2D.cardSize}, 间距={editor2D.cardSpacing}, 层级={editor2D.selectedLayer}");
        }
        
        var cards2D = FindObjectsOfType<CardObject2D>();
        Debug.Log($"当前场景: {cards2D.Length} 个2D卡片");
    }
} 