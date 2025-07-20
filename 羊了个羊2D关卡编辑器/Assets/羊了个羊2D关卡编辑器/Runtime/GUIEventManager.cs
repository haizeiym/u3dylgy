using UnityEngine;

namespace YangLeGeYang2D.LevelEditor
{

public class GUIEventManager : MonoBehaviour
{
    private static GUIEventManager instance;
    public static GUIEventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GUIEventManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GUIEventManager");
                    instance = go.AddComponent<GUIEventManager>();
                    Debug.Log("GUI事件管理器已自动创建");
                }
            }
            return instance;
        }
    }
    
    [Header("GUI区域设置")]
    public Rect guiRect = new Rect(10, 10, 300, Screen.height - 20);
    
    private bool isMouseOverGUI = false;
    private bool isGUIActive = false;
    private bool isGUIVisible = false;
    private Vector2 lastMousePosition;
    private float lastCheckTime = 0f;
    private const float CHECK_INTERVAL = 0.01f; // 每10ms检查一次
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GUI事件管理器已初始化");
        }
        else if (instance != this)
        {
            Debug.Log("销毁重复的GUI事件管理器");
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // 确保在Start中设置GUI区域
        UpdateGUIRect();
        Debug.Log($"GUI事件管理器启动，GUI区域: {guiRect}");
    }
    
    void Update()
    {
        // 限制检查频率以提高性能
        if (Time.time - lastCheckTime >= CHECK_INTERVAL)
        {
            CheckMousePosition();
            lastCheckTime = Time.time;
        }
    }
    
    void CheckMousePosition()
    {
        Vector2 mousePos = Input.mousePosition;
        bool wasOverGUI = isMouseOverGUI;
        isMouseOverGUI = guiRect.Contains(mousePos);
        
        // 只在状态改变时输出日志
        if (wasOverGUI != isMouseOverGUI)
        {
            if (isMouseOverGUI)
            {
                Debug.Log($"鼠标进入GUI区域: {mousePos}");
            }
            else
            {
                Debug.Log($"鼠标离开GUI区域: {mousePos}");
            }
        }
        
        lastMousePosition = mousePos;
    }
    
    void OnGUI()
    {
        // 标记GUI为活动状态
        isGUIActive = true;
        isGUIVisible = true;
        
        // 强制检查当前鼠标位置
        Vector2 currentMousePos = Input.mousePosition;
        bool currentMouseOverGUI = guiRect.Contains(currentMousePos);
        
        // 如果鼠标在GUI上，处理所有事件
        if (currentMouseOverGUI)
        {
            Event currentEvent = Event.current;
            
            // 处理所有可能的鼠标事件
            if (currentEvent.type == EventType.MouseDown || 
                currentEvent.type == EventType.MouseUp || 
                currentEvent.type == EventType.MouseDrag ||
                currentEvent.type == EventType.ScrollWheel ||
                currentEvent.type == EventType.MouseMove)
            {
                currentEvent.Use();
                Debug.Log($"GUI事件管理器阻止了事件: {currentEvent.type} at {currentMousePos}");
            }
        }
        
        // 在GUI结束时重置标记
        if (Event.current.type == EventType.Repaint)
        {
            isGUIActive = false;
        }
    }
    
    void LateUpdate()
    {
        // 在LateUpdate中重置GUI可见性标记
        isGUIVisible = false;
    }
    
    public bool IsMouseOverGUI()
    {
        // 多重检查确保准确性
        bool result = isMouseOverGUI || guiRect.Contains(Input.mousePosition);
        
        // 如果GUI正在活动，额外检查
        if (isGUIActive || isGUIVisible)
        {
            result = result || guiRect.Contains(Input.mousePosition);
        }
        
        return result;
    }
    
    public bool IsMouseOverGUINow()
    {
        Vector2 mousePos = Input.mousePosition;
        bool result = guiRect.Contains(mousePos);
        
        // 如果GUI正在活动，强制检查
        if (isGUIActive || isGUIVisible)
        {
            result = result || guiRect.Contains(mousePos);
        }
        
        if (result)
        {
            Debug.Log($"实时检查：鼠标在GUI区域内: {mousePos}");
        }
        
        return result;
    }
    
    public void SetGUIRect(Rect rect)
    {
        guiRect = rect;
        Debug.Log($"GUI区域已设置为: {rect}");
    }
    
    public void UpdateGUIRect()
    {
        // 根据屏幕大小更新GUI区域
        guiRect = new Rect(10, 10, 300, Screen.height - 20);
        Debug.Log($"GUI区域已更新: {guiRect}");
    }
    
    public Rect GetGUIRect()
    {
        return guiRect;
    }
    
    public bool IsPointInGUI(Vector2 screenPoint)
    {
        return guiRect.Contains(screenPoint);
    }
    
    public string GetGUIInfo()
    {
        Vector2 mousePos = Input.mousePosition;
        return $"GUI区域: {guiRect}, 鼠标位置: {mousePos}, 在GUI上: {isMouseOverGUI}, GUI活动: {isGUIActive}, GUI可见: {isGUIVisible}";
    }
    
    // 强制刷新GUI状态
    public void ForceRefresh()
    {
        CheckMousePosition();
        Debug.Log($"GUI状态已强制刷新: {GetGUIInfo()}");
    }
}
}