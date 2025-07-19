using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class CardData2D
{
    public int id;
    public int type; // 卡片类型
    public Vector2 position; // 2D位置
    public int layer; // 层级
    public bool isVisible; // 是否可见
    public List<int> blockingCards; // 阻挡此卡片的卡片ID列表
}

[System.Serializable]
public class LevelData2D
{
    public string levelName;
    public int levelId;
    public List<CardData2D> cards;
    public int totalLayers;
    public Vector2 gridSize;
    public float cardSpacing;
    public float cardSize;
    public bool useCustomAreaSize;
    public Vector2 areaSize;
    public bool enableLayerPreview;
    public Color normalLayerColor;
    public Color grayedLayerColor;
    
    // 高级设置
    public bool debugMode;
    public bool highPerformance;
    public bool showFPS;
    public bool showMemory;
    public float cameraSpeed;
    public float zoomSpeed;
    public float inputThrottle;
    public bool showGrid;
    public bool showGridNumbers;
    public bool showGridCoordinates;
    public float gridLineWidth;
    public bool showCardIDs;
    public bool showCardTypes;
    public float cardHoverScale;
    public bool autoSave;
    public bool backupLevels;
    public bool exportJSON;
    public bool exportXML;
    public bool exportBinary;
}

[System.Serializable]
public class SheepLevelEditor2D : MonoBehaviour
{
    [Header("编辑器设置")]
    public int currentLevelId = 1;
    public string currentLevelName = "Level_1";
    public int totalLayers = 3;
    public Vector2 gridSize = new Vector2(8, 8);
    public float cardSpacing = 1.2f;
    public float cardSize = 0.8f;
    
    [Header("卡片设置")]
    public Sprite[] cardSprites;
    public int currentCardType = 0;
    public int maxCardTypes = 8;
    
            [Header("编辑器状态")]
        public bool isEditMode = true;
        public bool showGrid = true;
        public int selectedLayer = 0;
        
        // 添加显示网格的GUI控制
        public bool showGridInGUI = true;
    
    [Header("2D设置")]
    public Camera editorCamera2D;
    public float cameraZoom = 5f;
    public Vector2 cameraPosition = Vector2.zero;
    

    
    [Header("可放置区域可视化")]
    public bool showPlaceableArea = true;
    public PlaceableAreaVisualizer placeableAreaVisualizer;
    
    [Header("区域大小设置")]
    public Vector2 areaSize = new Vector2(16f, 16f); // 可设置的区域大小
    public bool useCustomAreaSize = true; // 是否使用自定义区域大小
    
    [Header("层级预览设置")]
    public bool enableLayerPreview = true; // 是否启用层级预览
    public Color normalLayerColor = Color.white; // 正常层级颜色
    public Color grayedLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 置灰层级颜色
    
    [Header("高级设置")]
    public bool debugMode = false; // 调试模式
    public bool highPerformance = true; // 高性能模式
    public bool showFPS = false; // 显示FPS
    public bool showMemory = false; // 显示内存使用
    public float cameraSpeed = 1f; // 相机移动速度
    public float zoomSpeed = 1f; // 缩放速度
    public float inputThrottle = 0.1f; // 输入节流间隔
    public bool showGridNumbers = false; // 显示网格编号
    public bool showGridCoordinates = false; // 显示坐标
    public float gridLineWidth = 0.02f; // 网格线宽度
    public bool showCardIDs = false; // 显示卡片ID
    public bool showCardTypes = false; // 显示卡片类型
    public float cardHoverScale = 1.2f; // 悬停缩放
    public bool autoSave = true; // 自动保存
    public bool backupLevels = true; // 备份关卡
    public bool exportJSON = true; // 导出JSON格式
    public bool exportXML = false; // 导出XML格式
    public bool exportBinary = false; // 导出二进制格式
    
    private List<CardData2D> levelCards = new List<CardData2D>();
    private List<GameObject> cardObjects = new List<GameObject>();

    private Vector3 lastMousePosition;
    
    // 输入节流变量
    private float lastInputTime = 0f;
    
    // 计算实际区域大小
    public Vector2 GetActualAreaSize()
    {
        if (useCustomAreaSize)
        {
            return areaSize;
        }
        else
        {
            // 使用网格大小和卡片间距计算
            // 网格大小表示网格点的数量，所以区域大小应该是 gridSize * cardSpacing
            return new Vector2(gridSize.x * cardSpacing, gridSize.y * cardSpacing);
        }
    }
    
    void Start()
    {
        Setup2DCamera();
        Create2DGrid();
        SetupPlaceableAreaVisualizer();
        LoadLevel(currentLevelId);
        UpdateCardDisplay();
        UpdateGridDisplay(); // 初始化网格显示
        UpdateCardLabels(); // 初始化卡片标签
        
        // 确保GUI事件管理器存在
        if (GUIEventManager.Instance != null)
        {
            GUIEventManager.Instance.SetGUIRect(new Rect(10, 10, 300, Screen.height - 20));
        }
        
        Debug.Log("羊了个羊2D关卡编辑器已启动，所有设置已初始化");
    }
    
    void Setup2DCamera()
    {
        // 查找或创建2D相机
        editorCamera2D = Camera.main;
        if (editorCamera2D == null)
        {
            GameObject cameraObj = new GameObject("EditorCamera2D");
            editorCamera2D = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
        }
        
        // 设置为2D相机
        editorCamera2D.orthographic = true;
        editorCamera2D.orthographicSize = cameraZoom;
        editorCamera2D.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
        editorCamera2D.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Debug.Log($"2D相机已设置: 缩放={cameraZoom}, 位置={cameraPosition}");
    }
    
    void Create2DGrid()
    {
        // 创建2D网格背景
        GameObject gridBackground = new GameObject("GridBackground");
        gridBackground.transform.position = Vector3.zero;
        
        // 创建网格精灵
        SpriteRenderer gridRenderer = gridBackground.AddComponent<SpriteRenderer>();
        gridRenderer.sprite = CreateGridSprite();
        gridRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        gridRenderer.sortingOrder = -1;
        
        // 设置网格大小覆盖整个可放置区域
        Vector2 actualAreaSize = GetActualAreaSize();
        gridBackground.transform.localScale = new Vector3(actualAreaSize.x, actualAreaSize.y, 1);
        
        // 更新网格以适应卡片大小
        UpdateGridForCardSize();
        
        // 初始化网格显示
        UpdateGridDisplay();
    }
    
    public Sprite CreateGridSprite()
    {
        // 创建动态网格纹理，根据卡片间距调整网格密度
        int textureSize = 128; // 增加纹理大小以获得更好的分辨率
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color gridColor = Color.gray;
        Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        
        // 计算网格线间距，使其与卡片间距匹配
        // 纹理代表一个卡片间距的大小，所以我们需要在纹理中显示网格线
        int gridLineSpacing = CalculateGridLineSpacing(textureSize);
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // 创建网格线，在边缘和中心位置显示
                bool isGridLine = (x % gridLineSpacing == 0 || y % gridLineSpacing == 0) ||
                                 (x == 0 || x == textureSize - 1 || y == 0 || y == textureSize - 1);
                
                if (isGridLine)
                {
                    texture.SetPixel(x, y, gridColor);
                }
                else
                {
                    texture.SetPixel(x, y, bgColor);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    public void UpdateGridDisplay()
    {
        // 更新网格显示，包括编号和坐标
        GameObject gridBackground = GameObject.Find("GridBackground");
        if (gridBackground != null)
        {
            SpriteRenderer gridRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (gridRenderer != null)
            {
                // 根据showGrid设置显示或隐藏网格
                gridRenderer.enabled = showGrid;
                
                if (showGrid)
                {
                    // 重新创建网格纹理
                    gridRenderer.sprite = CreateGridSprite();
                    
                    // 更新网格大小以覆盖整个可放置区域
                    Vector2 actualAreaSize = GetActualAreaSize();
                    gridBackground.transform.localScale = new Vector3(actualAreaSize.x, actualAreaSize.y, 1);
                }
            }
        }
        
        // 如果启用了网格编号或坐标显示，创建文本对象
        if (showGrid && (showGridNumbers || showGridCoordinates))
        {
            CreateGridLabels();
        }
        else
        {
            ClearGridLabels();
        }
    }
    
    private List<GameObject> gridLabelObjects = new List<GameObject>();
    
    void CreateGridLabels()
    {
        // 清除现有标签
        ClearGridLabels();
        
        Vector2 actualAreaSize = GetActualAreaSize();
        int gridWidth = Mathf.RoundToInt(gridSize.x);
        int gridHeight = Mathf.RoundToInt(gridSize.y);
        
        // 计算网格的起始位置（左下角）
        Vector2 gridStart = new Vector2(-actualAreaSize.x * 0.5f, -actualAreaSize.y * 0.5f);
        
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPos = gridStart + new Vector2(x * cardSpacing, y * cardSpacing);
                
                GameObject labelObj = new GameObject($"GridLabel_{x}_{y}");
                labelObj.transform.position = new Vector3(worldPos.x, worldPos.y, -0.1f);
                
                // 创建文本显示
                if (showGridNumbers)
                {
                    CreateGridNumberLabel(labelObj, x, y);
                }
                
                if (showGridCoordinates)
                {
                    CreateGridCoordinateLabel(labelObj, worldPos);
                }
                
                gridLabelObjects.Add(labelObj);
            }
        }
    }
    
    void CreateGridNumberLabel(GameObject parent, int x, int y)
    {
        GameObject numberObj = new GameObject("Number");
        numberObj.transform.SetParent(parent.transform);
        numberObj.transform.localPosition = Vector3.zero;
        
        // 这里可以添加文本组件来显示编号
        // 由于Unity的GUI文本在3D空间中比较复杂，这里只是创建占位符
        Debug.Log($"创建网格编号: ({x}, {y})");
    }
    
    void CreateGridCoordinateLabel(GameObject parent, Vector2 worldPos)
    {
        GameObject coordObj = new GameObject("Coordinate");
        coordObj.transform.SetParent(parent.transform);
        coordObj.transform.localPosition = Vector3.zero;
        
        // 这里可以添加文本组件来显示坐标
        Debug.Log($"创建网格坐标: {worldPos}");
    }
    
    void ClearGridLabels()
    {
        foreach (GameObject labelObj in gridLabelObjects)
        {
            if (labelObj != null)
            {
                DestroyImmediate(labelObj);
            }
        }
        gridLabelObjects.Clear();
    }
    
    void CreateCardLabel(GameObject cardObj, CardObject2D cardComponent)
    {
        GameObject labelObj = new GameObject("CardLabel");
        labelObj.transform.SetParent(cardObj.transform);
        labelObj.transform.localPosition = new Vector3(0, 0.6f, 0); // 在卡片上方显示
        
        // 这里可以添加文本组件来显示卡片信息
        // 由于Unity的GUI文本在3D空间中比较复杂，这里只是创建占位符
        if (showCardIDs)
        {
            Debug.Log($"显示卡片ID: {cardComponent.cardId}");
        }
        
        if (showCardTypes)
        {
            Debug.Log($"显示卡片类型: {cardComponent.cardType}");
        }
    }
    
    public void UpdateCardLabels()
    {
        // 更新所有卡片的标签显示
        foreach (var cardObj in cardObjects)
        {
            CardObject2D cardComponent = cardObj.GetComponent<CardObject2D>();
            if (cardComponent != null)
            {
                // 清除现有标签
                Transform existingLabel = cardObj.transform.Find("CardLabel");
                if (existingLabel != null)
                {
                    DestroyImmediate(existingLabel.gameObject);
                }
                
                // 如果启用了标签显示，创建新标签
                if (showCardIDs || showCardTypes)
                {
                    CreateCardLabel(cardObj, cardComponent);
                }
            }
        }
    }
    
    public int CalculateGridLineSpacing(int textureSize)
    {
        // 根据卡片间距计算合适的网格线间距
        // 目标是让网格线能够清晰地显示卡片放置位置
        float spacingRatio = cardSpacing / cardSize;
        
        // 根据间距比例调整网格密度
        if (spacingRatio >= 2.0f)
        {
            return textureSize / 4; // 大间距，稀疏网格
        }
        else if (spacingRatio >= 1.5f)
        {
            return textureSize / 6; // 中等间距，中等密度网格
        }
        else
        {
            return textureSize / 8; // 小间距，密集网格
        }
    }
    
    public void UpdateGridForCardSize()
    {
        // 根据卡片大小和间距调整网格
        GameObject gridBackground = GameObject.Find("GridBackground");
        if (gridBackground != null)
        {
            SpriteRenderer gridRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (gridRenderer != null)
            {
                // 重新创建网格纹理以适应新的卡片大小和间距
                gridRenderer.sprite = CreateGridSprite();
                
                // 更新网格大小以覆盖整个可放置区域
                Vector2 actualAreaSize = GetActualAreaSize();
                gridBackground.transform.localScale = new Vector3(actualAreaSize.x, actualAreaSize.y, 1);
                
                // 更新网格显示
                UpdateGridDisplay();
                
                Debug.Log($"网格已更新: 卡片大小={cardSize}, 间距={cardSpacing}, 区域大小={actualAreaSize.x} x {actualAreaSize.y}");
            }
        }
    }
    

    
    void SetupPlaceableAreaVisualizer()
    {
        // 查找或创建可放置区域可视化组件
        placeableAreaVisualizer = FindObjectOfType<PlaceableAreaVisualizer>();
        if (placeableAreaVisualizer == null)
        {
            GameObject visualizerObj = new GameObject("PlaceableAreaVisualizer");
            placeableAreaVisualizer = visualizerObj.AddComponent<PlaceableAreaVisualizer>();
        }
        
        // 设置可视化状态
        if (placeableAreaVisualizer != null)
        {
            placeableAreaVisualizer.SetVisible(showPlaceableArea);
            Debug.Log($"可放置区域可视化已设置: 显示={showPlaceableArea}");
        }
    }
    

    
    public void UpdateGridAndMasks()
    {
        // 更新网格大小覆盖整个可放置区域
        GameObject gridBackground = GameObject.Find("GridBackground");
        Vector2 actualAreaSize = GetActualAreaSize();
        
        if (gridBackground != null)
        {
            gridBackground.transform.localScale = new Vector3(actualAreaSize.x, actualAreaSize.y, 1);
        }
        
        // 更新可放置区域可视化
        if (placeableAreaVisualizer != null)
        {
            placeableAreaVisualizer.SetVisible(showPlaceableArea);
        }
        
        // 更新网格显示
        UpdateGridDisplay();
        
        Debug.Log($"网格已更新: 网格大小={gridSize}, 间距={cardSpacing}, 区域大小={actualAreaSize.x} x {actualAreaSize.y}");
    }
    
    void Update()
    {
        if (!isEditMode) return;
        
        // 高性能模式：降低更新频率
        if (highPerformance && Time.frameCount % 3 != 0)
        {
            return;
        }
        
        HandleInput();
        HandleCameraMovement();
    }
    
    void HandleInput()
    {
        // 输入节流：限制输入频率
        if (Time.time - lastInputTime < inputThrottle)
        {
            return;
        }
        
        // 简化的GUI区域检查
        bool isOverGUI = Input.mousePosition.x < 320;
        
        if (isOverGUI)
        {
            return; // 如果鼠标在GUI上，不处理游戏输入
        }
        
        // 鼠标左键放置卡片
        if (Input.GetMouseButtonDown(0))
        {
            PlaceCard2D();
            lastInputTime = Time.time;
        }
        
        // 鼠标右键删除卡片
        if (Input.GetMouseButtonDown(1))
        {
            DeleteCard2D();
            lastInputTime = Time.time;
        }
        
        // 滚轮切换层级（添加防抖）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.1f && !Input.GetKey(KeyCode.LeftControl))
        {
            int oldLayer = selectedLayer;
            selectedLayer = Mathf.Clamp(selectedLayer + (scroll > 0 ? 1 : -1), 0, totalLayers - 1);
            
            if (oldLayer != selectedLayer)
            {
                UpdateCardDisplay();
                UpdateCardLabels();
                lastInputTime = Time.time;
            }
        }
        
        // 数字键切换卡片类型（添加防抖）
        for (int i = 0; i < maxCardTypes && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                int oldType = currentCardType;
                currentCardType = i;
                if (oldType != currentCardType)
                {
                    UpdateCardLabels();
                    lastInputTime = Time.time;
                }
            }
        }
        
        // 快捷键
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveLevel();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadLevel(currentLevelId);
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                NewLevel();
            }
        }
    }
    
    void HandleCameraMovement()
    {
        // 鼠标中键拖拽移动相机
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            if (delta.magnitude > 1f) // 添加移动阈值
            {
                Vector3 worldDelta = editorCamera2D.ScreenToWorldPoint(delta) - editorCamera2D.ScreenToWorldPoint(Vector3.zero);
                editorCamera2D.transform.position -= worldDelta * cameraSpeed;
            }
        }
        
        // Ctrl+滚轮缩放（添加防抖）
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.05f)
            {
                float oldZoom = cameraZoom;
                cameraZoom = Mathf.Clamp(cameraZoom - scroll * 2f * zoomSpeed, 1f, 20f);
                if (Mathf.Abs(oldZoom - cameraZoom) > 0.1f)
                {
                    editorCamera2D.orthographicSize = cameraZoom;
                }
            }
        }
        
        lastMousePosition = Input.mousePosition;
    }
    
    bool IsMouseOverGUI()
    {
        // 使用GUI事件管理器检查
        if (GUIEventManager.Instance != null)
        {
            return GUIEventManager.Instance.IsMouseOverGUI();
        }
        
        // 备用检查方法
        Vector2 mousePos = Input.mousePosition;
        Rect guiRect = new Rect(10, 10, 300, Screen.height - 20);
        return guiRect.Contains(mousePos);
    }
    
    void PlaceCard2D()
    {
        Vector3 mouseWorldPos = editorCamera2D.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        Vector2 gridPos = SnapToGrid2D(worldPos2D);
        
        // 检查位置是否在网格范围内
        if (!IsPositionInGridBounds2D(gridPos))
        {
            return;
        }
        
        // 检查位置是否已有卡片
        if (!IsPositionOccupied2D(gridPos))
        {
            CardData2D newCard = new CardData2D
            {
                id = GetNextCardId(),
                type = currentCardType,
                position = gridPos,
                layer = selectedLayer,
                isVisible = true,
                blockingCards = new List<int>()
            };
            
            levelCards.Add(newCard);
            CreateCardObject2D(newCard);
            
            // 自动保存
            if (autoSave)
            {
                SaveLevel();
            }
        }
    }
    
    void DeleteCard2D()
    {
        Vector3 mouseWorldPos = editorCamera2D.ScreenToWorldPoint(Input.mousePosition);
        Vector2 worldPos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        
        // 查找最近的卡片
        GameObject cardToDelete = null;
        float minDistance = float.MaxValue;
        
        foreach (var cardObj in cardObjects)
        {
            float distance = Vector2.Distance(cardObj.transform.position, worldPos2D);
            if (distance < minDistance && distance < cardSize * 0.5f) // 点击范围基于卡片大小
            {
                minDistance = distance;
                cardToDelete = cardObj;
            }
        }
        
        if (cardToDelete != null)
        {
            CardData2D cardData = levelCards.Find(c => c.id == cardToDelete.GetComponent<CardObject2D>()?.cardId);
            if (cardData != null)
            {
                levelCards.Remove(cardData);
                DestroyImmediate(cardToDelete);
                cardObjects.Remove(cardToDelete);
                
                // 自动保存
                if (autoSave)
                {
                    SaveLevel();
                }
            }
        }
    }
    
    public Vector2 SnapToGrid2D(Vector2 worldPos)
    {
        // 计算网格的起始位置（左下角）
        Vector2 actualAreaSize = GetActualAreaSize();
        Vector2 gridStart = new Vector2(-actualAreaSize.x * 0.5f, -actualAreaSize.y * 0.5f);
        
        // 将世界坐标转换为网格坐标
        Vector2 gridPos = worldPos - gridStart;
        
        // 对齐到网格
        float gridX = Mathf.Round(gridPos.x / cardSpacing) * cardSpacing;
        float gridY = Mathf.Round(gridPos.y / cardSpacing) * cardSpacing;
        
        // 转换回世界坐标
        return gridStart + new Vector2(gridX, gridY);
    }
    
    public bool IsPositionOccupied2D(Vector2 position)
    {
        // 只检查同一层级的卡片是否占用位置
        return levelCards.Exists(c => c.layer == selectedLayer && Vector2.Distance(c.position, position) < cardSize * 0.5f);
    }
    
    public bool IsPositionInGridBounds2D(Vector2 position)
    {
        // 使用实际区域大小计算边界
        Vector2 actualAreaSize = GetActualAreaSize();
        float halfAreaWidth = actualAreaSize.x * 0.5f;
        float halfAreaHeight = actualAreaSize.y * 0.5f;
        
        // 检查位置是否在区域范围内
        return Mathf.Abs(position.x) <= halfAreaWidth && Mathf.Abs(position.y) <= halfAreaHeight;
    }
    
    int GetNextCardId()
    {
        int maxId = 0;
        foreach (var card in levelCards)
        {
            maxId = Mathf.Max(maxId, card.id);
        }
        return maxId + 1;
    }
    
    int GetNextLevelId()
    {
        int maxLevelId = 0;
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        
        // 确保目录存在
        if (Directory.Exists(levelsPath))
        {
            // 查找所有Level2D_*.json文件
            string[] levelFiles = Directory.GetFiles(levelsPath, "Level2D_*.json");
            
            foreach (string file in levelFiles)
            {
                try
                {
                    // 从文件名中提取关卡ID
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.StartsWith("Level2D_"))
                    {
                        string idStr = fileName.Substring("Level2D_".Length);
                        if (int.TryParse(idStr, out int levelId))
                        {
                            maxLevelId = Mathf.Max(maxLevelId, levelId);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"解析关卡文件失败: {file}, 错误: {e.Message}");
                }
            }
        }
        
        return maxLevelId + 1;
    }
    
    public void CreateCardObject2D(CardData2D cardData)
    {
        // 总是创建默认2D卡片，不依赖预制体
        GameObject cardObj = new GameObject($"Card_{cardData.id}");
        cardObj.transform.position = new Vector3(cardData.position.x, cardData.position.y, -cardData.layer * 0.1f);
        
        // 添加SpriteRenderer
        SpriteRenderer spriteRenderer = cardObj.AddComponent<SpriteRenderer>();
        if (cardSprites != null && cardData.type < cardSprites.Length)
        {
            spriteRenderer.sprite = cardSprites[cardData.type];
        }
        else
        {
            // 创建默认精灵
            spriteRenderer.sprite = CreateDefaultCardSprite(cardData.type);
        }
        
        spriteRenderer.sortingOrder = cardData.layer;
        
        // 设置卡片大小
        cardObj.transform.localScale = Vector3.one * cardSize;
        
        // 添加碰撞器用于点击检测
        BoxCollider2D collider = cardObj.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 0.8f; // 碰撞器大小固定，但会根据scale缩放
        
        // 添加CardObject2D组件
        CardObject2D cardComponent = cardObj.AddComponent<CardObject2D>();
        cardComponent.cardId = cardData.id;
        cardComponent.cardType = cardData.type;
        cardComponent.layer = cardData.layer;
        cardComponent.baseCardSize = cardSize; // 设置基础卡片大小
        
        // 设置初始层级预览颜色
        if (enableLayerPreview)
        {
            UpdateCardLayerPreview(cardObj, cardComponent);
        }
        
        // 添加卡片标签显示
        if (showCardIDs || showCardTypes)
        {
            CreateCardLabel(cardObj, cardComponent);
        }
        
        cardObjects.Add(cardObj);
        
        Debug.Log($"创建2D卡片: ID={cardData.id}, 类型={cardData.type}, 层级={cardData.layer}");
    }
    
    Sprite CreateDefaultCardSprite(int cardType)
    {
        // 创建简单的彩色方块精灵
        int textureSize = 32;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color[] colors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.magenta, new Color(1f, 0.5f, 0f), Color.cyan, new Color(1f, 0.75f, 0.8f)
        };
        
        Color cardColor = cardType < colors.Length ? colors[cardType] : Color.white;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                if (x == 0 || x == textureSize - 1 || y == 0 || y == textureSize - 1)
                {
                    texture.SetPixel(x, y, Color.black); // 边框
                }
                else
                {
                    texture.SetPixel(x, y, cardColor);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    public void UpdateCardDisplay()
    {
        Debug.Log($"更新卡片显示 - 层级预览: {enableLayerPreview}, 当前层级: {selectedLayer}");
        
        foreach (var cardObj in cardObjects)
        {
            CardObject2D cardComponent = cardObj.GetComponent<CardObject2D>();
            if (cardComponent != null)
            {
                if (enableLayerPreview)
                {
                    // 层级预览模式：显示所有层级的卡片，但颜色不同
                    cardObj.SetActive(true);
                    UpdateCardLayerPreview(cardObj, cardComponent);
                    Debug.Log($"层级预览模式 - 卡片ID:{cardComponent.cardId}, 层级:{cardComponent.layer}, 激活:true");
                }
                else
                {
                    // 传统模式：只显示当前层级的卡片
                    bool shouldShow = cardComponent.layer == selectedLayer;
                    cardObj.SetActive(shouldShow);
                    // 重置颜色为正常
                    SpriteRenderer spriteRenderer = cardObj.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.color = normalLayerColor;
                    }
                    Debug.Log($"传统模式 - 卡片ID:{cardComponent.cardId}, 层级:{cardComponent.layer}, 激活:{shouldShow}");
                }
            }
        }
        

        
        // 更新卡片标签
        UpdateCardLabels();
    }
    
    public void UpdateCardLayerPreview(GameObject cardObj, CardObject2D cardComponent)
    {
        SpriteRenderer spriteRenderer = cardObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (cardComponent.layer == selectedLayer)
            {
                // 当前层级：正常颜色
                spriteRenderer.color = normalLayerColor;
                Debug.Log($"卡片ID:{cardComponent.cardId} - 当前层级({cardComponent.layer})，使用正常颜色: {normalLayerColor}");
            }
            else
            {
                // 其他层级：置灰颜色
                spriteRenderer.color = grayedLayerColor;
                Debug.Log($"卡片ID:{cardComponent.cardId} - 其他层级({cardComponent.layer})，使用置灰颜色: {grayedLayerColor}");
            }
        }
    }
    
    public void UpdateAllCardSizes()
    {
        // 更新所有现有卡片的大小
        foreach (var cardObj in cardObjects)
        {
            cardObj.transform.localScale = Vector3.one * cardSize;
            
            // 同时更新CardObject2D组件的基础卡片大小
            CardObject2D cardComponent = cardObj.GetComponent<CardObject2D>();
            if (cardComponent != null)
            {
                cardComponent.baseCardSize = cardSize;
            }
        }
        
        // 更新网格以适应新的卡片大小
        UpdateGridForCardSize();
        

        
        // 更新卡片标签
        UpdateCardLabels();
        
        Debug.Log($"卡片大小已更新为: {cardSize}");
    }
    
    public void SaveLevel()
    {
        LevelData2D levelData = new LevelData2D
        {
            levelName = currentLevelName,
            levelId = currentLevelId,
            cards = new List<CardData2D>(levelCards),
            totalLayers = totalLayers,
            gridSize = gridSize,
            cardSpacing = cardSpacing,
            cardSize = cardSize,
            useCustomAreaSize = useCustomAreaSize,
            areaSize = areaSize,
            enableLayerPreview = enableLayerPreview,
            normalLayerColor = normalLayerColor,
            grayedLayerColor = grayedLayerColor,
            
            // 保存高级设置
            debugMode = debugMode,
            highPerformance = highPerformance,
            showFPS = showFPS,
            showMemory = showMemory,
            cameraSpeed = cameraSpeed,
            zoomSpeed = zoomSpeed,
            inputThrottle = inputThrottle,
            showGrid = showGrid,
            showGridNumbers = showGridNumbers,
            showGridCoordinates = showGridCoordinates,
            gridLineWidth = gridLineWidth,
            showCardIDs = showCardIDs,
            showCardTypes = showCardTypes,
            cardHoverScale = cardHoverScale,
            autoSave = autoSave,
            backupLevels = backupLevels,
            exportJSON = exportJSON,
            exportXML = exportXML,
            exportBinary = exportBinary
        };
        
        string json = JsonUtility.ToJson(levelData, true);
        string filePath = Path.Combine(Application.dataPath, "Levels", $"Level2D_{currentLevelId}.json");
        
        // 确保目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        
        // 如果启用备份且文件已存在，先创建备份
        if (backupLevels && File.Exists(filePath))
        {
            string backupPath = Path.Combine(Application.dataPath, "Levels", $"Level2D_{currentLevelId}_backup.json");
            File.Copy(filePath, backupPath, true);
            Debug.Log($"已创建备份文件: {backupPath}");
        }
        
        File.WriteAllText(filePath, json);
        Debug.Log($"2D关卡已保存: {filePath}");
    }
    
    public void LoadLevel(int levelId)
    {
        Debug.Log($"尝试加载2D关卡: {levelId}");
        string filePath = Path.Combine(Application.dataPath, "Levels", $"Level2D_{levelId}.json");
        
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            LevelData2D levelData = JsonUtility.FromJson<LevelData2D>(json);
            
            // 确保关卡ID同步
            currentLevelId = levelData.levelId;
            currentLevelName = levelData.levelName;
            totalLayers = levelData.totalLayers;
            gridSize = levelData.gridSize;
            cardSpacing = levelData.cardSpacing;
            
            // 加载卡片大小，如果没有则使用默认值
            if (levelData.cardSize > 0)
            {
                cardSize = levelData.cardSize;
            }
            
            // 加载区域大小设置
            useCustomAreaSize = levelData.useCustomAreaSize;
            areaSize = levelData.areaSize;
            
            // 加载层级预览设置
            enableLayerPreview = levelData.enableLayerPreview;
            normalLayerColor = levelData.normalLayerColor;
            grayedLayerColor = levelData.grayedLayerColor;
            
            // 加载高级设置
            debugMode = levelData.debugMode;
            highPerformance = levelData.highPerformance;
            showFPS = levelData.showFPS;
            showMemory = levelData.showMemory;
            cameraSpeed = levelData.cameraSpeed;
            zoomSpeed = levelData.zoomSpeed;
            inputThrottle = levelData.inputThrottle;
            showGrid = levelData.showGrid;
            showGridNumbers = levelData.showGridNumbers;
            showGridCoordinates = levelData.showGridCoordinates;
            gridLineWidth = levelData.gridLineWidth;
            showCardIDs = levelData.showCardIDs;
            showCardTypes = levelData.showCardTypes;
            cardHoverScale = levelData.cardHoverScale;
            autoSave = levelData.autoSave;
            backupLevels = levelData.backupLevels;
            exportJSON = levelData.exportJSON;
            exportXML = levelData.exportXML;
            exportBinary = levelData.exportBinary;
            
            levelCards = new List<CardData2D>(levelData.cards);
            
            // 清除现有卡片对象
            foreach (var cardObj in cardObjects)
            {
                if (cardObj != null)
                {
                    DestroyImmediate(cardObj);
                }
            }
            cardObjects.Clear();
            
            // 重新创建卡片对象
            foreach (var cardData in levelCards)
            {
                CreateCardObject2D(cardData);
            }
            
            UpdateCardDisplay();
            UpdateAllCardSizes(); // 确保使用正确的卡片大小
            UpdateGridDisplay(); // 更新网格显示
            UpdateCardLabels(); // 更新卡片标签
            Debug.Log($"2D关卡已加载: 关卡ID={currentLevelId}, 关卡名称={currentLevelName}, 文件路径={filePath}");
        }
        else
        {
            Debug.Log($"2D关卡文件不存在: {filePath}，创建新关卡");
            NewLevel();
        }
    }
    
    public void NewLevel()
    {
        // 获取下一个可用的关卡ID
        int newLevelId = GetNextLevelId();
        currentLevelId = newLevelId;
        
        // 清除关卡数据
        levelCards.Clear();
        
        // 立即销毁所有卡片对象
        foreach (var cardObj in cardObjects)
        {
            if (cardObj != null)
            {
                DestroyImmediate(cardObj);
            }
        }
        cardObjects.Clear();
        
        // 更新关卡名称
        currentLevelName = $"Level2D_{currentLevelId}";
        
        // 重置区域大小设置
        useCustomAreaSize = true;
        areaSize = new Vector2(16f, 16f);
        
        // 重置层级预览设置
        enableLayerPreview = true;
        normalLayerColor = Color.white;
        grayedLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        // 重置高级设置
        debugMode = false;
        highPerformance = true;
        showFPS = false;
        showMemory = false;
        cameraSpeed = 1f;
        zoomSpeed = 1f;
        inputThrottle = 0.1f;
        showGrid = true;
        showGridNumbers = false;
        showGridCoordinates = false;
        gridLineWidth = 0.02f;
        showCardIDs = false;
        showCardTypes = false;
        cardHoverScale = 1.2f;
        autoSave = true;
        backupLevels = true;
        exportJSON = true;
        exportXML = false;
        exportBinary = false;
        
        // 强制更新显示
        UpdateCardDisplay();
        UpdateGridAndMasks();
        UpdateGridDisplay(); // 更新网格显示
        UpdateCardLabels(); // 更新卡片标签
        
        Debug.Log($"新建2D关卡: ID={currentLevelId}, 名称={currentLevelName}");
    }
    
    private Vector2 scrollPosition = Vector2.zero;
    
    void OnGUI()
    {
        if (!isEditMode) return;
        
        // 开始GUI区域
        GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height - 20));
        
        // GUI事件管理器会自动处理事件穿透问题
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("羊了个羊2D关卡编辑器", GUI.skin.box);
        
        GUILayout.Space(10);
        
        // 开始滚动视图
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height - 100));
        
        // 关卡信息
        GUILayout.Label("关卡信息");
        int newLevelId = IntField("关卡ID", currentLevelId);
        if (newLevelId != currentLevelId)
        {
            currentLevelId = newLevelId;
            LoadLevel(currentLevelId);
        }
        
        string newLevelName = TextField("关卡名称", currentLevelName);
        if (newLevelName != currentLevelName)
        {
            currentLevelName = newLevelName;
        }
        
        GUILayout.Space(10);
        
        // 编辑器设置
        GUILayout.Label("编辑器设置");
        int newTotalLayers = IntField("总层数", totalLayers);
        if (newTotalLayers != totalLayers)
        {
            totalLayers = newTotalLayers;
            UpdateGridDisplay();
            Debug.Log($"总层数已更新: {totalLayers}");
        }
        
        int newSelectedLayer = IntSlider("当前层级", selectedLayer, 0, totalLayers - 1);
        if (newSelectedLayer != selectedLayer)
        {
            selectedLayer = newSelectedLayer;
            UpdateCardDisplay();
            UpdateCardLabels(); // 更新卡片标签
            Debug.Log($"GUI层级已切换到: {selectedLayer}");
        }
        
        int newCurrentCardType = IntSlider("卡片类型", currentCardType, 0, maxCardTypes - 1);
        if (newCurrentCardType != currentCardType)
        {
            currentCardType = newCurrentCardType;
            UpdateCardLabels(); // 更新卡片标签
        }
        
        // 网格设置
        GUILayout.Space(5);
        GUILayout.Label("网格设置");
        
        bool newShowGrid = GUILayout.Toggle(showGrid, "显示网格");
        if (newShowGrid != showGrid)
        {
            showGrid = newShowGrid;
            UpdateGridDisplay(); // 更新网格显示
        }
        
        // 网格大小X
        float newGridSizeX = GUILayout.HorizontalSlider(gridSize.x, 4f, 16f);
        GUILayout.Label($"网格宽度: {newGridSizeX:F0}");
        if (newGridSizeX != gridSize.x)
        {
            gridSize.x = newGridSizeX;
            UpdateGridAndMasks();
            UpdateGridDisplay(); // 更新网格显示
        }
        
        // 网格大小Y
        float newGridSizeY = GUILayout.HorizontalSlider(gridSize.y, 4f, 16f);
        GUILayout.Label($"网格高度: {newGridSizeY:F0}");
        if (newGridSizeY != gridSize.y)
        {
            gridSize.y = newGridSizeY;
            UpdateGridAndMasks();
            UpdateGridDisplay(); // 更新网格显示
        }
        
        // 卡片间距
        float newCardSpacing = GUILayout.HorizontalSlider(cardSpacing, 0.8f, 2.0f);
        GUILayout.Label($"卡片间距: {newCardSpacing:F2}");
        if (newCardSpacing != cardSpacing)
        {
            cardSpacing = newCardSpacing;
            UpdateGridAndMasks();
            UpdateGridForCardSize(); // 更新网格纹理以适应新的间距
            UpdateGridDisplay(); // 更新网格显示
        }
        
        // 卡片大小设置
        GUILayout.Space(5);
        GUILayout.Label("卡片设置");
        float newCardSize = GUILayout.HorizontalSlider(cardSize, 0.3f, 2.0f);
        GUILayout.Label($"卡片大小: {newCardSize:F2}");
        if (newCardSize != cardSize)
        {
            cardSize = newCardSize;
            UpdateAllCardSizes();
            UpdateGridForCardSize(); // 更新网格纹理以适应新的大小
            UpdateGridDisplay(); // 更新网格显示
        }
        
        GUILayout.Space(10);
        
        // 2D相机设置
        GUILayout.Label("2D相机设置");
        float newCameraZoom = GUILayout.HorizontalSlider(cameraZoom, 1f, 20f);
        if (newCameraZoom != cameraZoom)
        {
            cameraZoom = newCameraZoom;
            if (editorCamera2D != null)
            {
                editorCamera2D.orthographicSize = cameraZoom;
            }
        }
        GUILayout.Label($"缩放: {cameraZoom:F1}");
        
        GUILayout.Space(10);
        

        
        GUILayout.Space(10);
        
        // 可放置区域可视化设置
        GUILayout.Label("可放置区域可视化");
        bool newShowPlaceableArea = GUILayout.Toggle(showPlaceableArea, "显示可放置区域");
        if (newShowPlaceableArea != showPlaceableArea)
        {
            showPlaceableArea = newShowPlaceableArea;
            if (placeableAreaVisualizer != null)
            {
                placeableAreaVisualizer.SetVisible(showPlaceableArea);
            }
            UpdateGridDisplay(); // 更新网格显示
        }
        
        GUILayout.Space(10);
        
        // 区域大小设置
        GUILayout.Label("区域大小设置");
        bool newUseCustomAreaSize = GUILayout.Toggle(useCustomAreaSize, "使用自定义区域大小");
        if (newUseCustomAreaSize != useCustomAreaSize)
        {
            useCustomAreaSize = newUseCustomAreaSize;
            UpdateGridAndMasks();
            UpdateGridDisplay(); // 更新网格显示
            Debug.Log($"自定义区域大小设置已更改: {(useCustomAreaSize ? "启用" : "禁用")}");
        }
        
        if (useCustomAreaSize)
        {
            // 区域宽度
            float newAreaSizeX = GUILayout.HorizontalSlider(areaSize.x, 2f, 20f);
            GUILayout.Label($"区域宽度: {newAreaSizeX:F2}");
            if (newAreaSizeX != areaSize.x)
            {
                areaSize.x = newAreaSizeX;
                UpdateGridAndMasks();
                UpdateGridDisplay(); // 更新网格显示
            }
            
            // 区域高度
            float newAreaSizeY = GUILayout.HorizontalSlider(areaSize.y, 2f, 20f);
            GUILayout.Label($"区域高度: {newAreaSizeY:F2}");
            if (newAreaSizeY != areaSize.y)
            {
                areaSize.y = newAreaSizeY;
                UpdateGridAndMasks();
                UpdateGridDisplay(); // 更新网格显示
            }
        }
        else
        {
            Vector2 actualAreaSize = GetActualAreaSize();
            GUILayout.Label($"当前区域大小: {actualAreaSize.x:F2} x {actualAreaSize.y:F2}");
            GUILayout.Label("(基于网格大小和卡片间距自动计算)");
        }
        
        GUILayout.Space(10);
        
        // 层级预览设置
        GUILayout.Label("层级预览设置");
        bool newEnableLayerPreview = GUILayout.Toggle(enableLayerPreview, "启用层级预览");
        if (newEnableLayerPreview != enableLayerPreview)
        {
            enableLayerPreview = newEnableLayerPreview;
            UpdateCardDisplay();
            UpdateCardLabels(); // 更新卡片标签
            Debug.Log($"层级预览设置已更改: {(enableLayerPreview ? "启用" : "禁用")}");
        }
        
        if (enableLayerPreview)
        {
            GUILayout.Label("正常层级颜色");
            Color newNormalColor = ColorField("正常颜色", normalLayerColor);
            if (newNormalColor != normalLayerColor)
            {
                normalLayerColor = newNormalColor;
                UpdateCardDisplay();
                UpdateCardLabels(); // 更新卡片标签
            }
            
            GUILayout.Label("置灰层级颜色");
            Color newGrayedColor = ColorField("置灰颜色", grayedLayerColor);
            if (newGrayedColor != grayedLayerColor)
            {
                grayedLayerColor = newGrayedColor;
                UpdateCardDisplay();
                UpdateCardLabels(); // 更新卡片标签
            }
            
            GUILayout.Label("说明: 最上层正常显示，其他层级置灰");
        }
        
        GUILayout.Space(10);
        
        // 高级设置
        GUILayout.Label("高级设置");
        bool newDebugMode = GUILayout.Toggle(debugMode, "启用调试日志");
        if (newDebugMode != debugMode)
        {
            debugMode = newDebugMode;
        }
        
        bool newHighPerformance = GUILayout.Toggle(highPerformance, "高性能模式");
        if (newHighPerformance != highPerformance)
        {
            highPerformance = newHighPerformance;
        }
        
        bool newShowFPS = GUILayout.Toggle(showFPS, "显示FPS");
        if (newShowFPS != showFPS)
        {
            showFPS = newShowFPS;
        }
        
        bool newShowMemory = GUILayout.Toggle(showMemory, "显示内存使用");
        if (newShowMemory != showMemory)
        {
            showMemory = newShowMemory;
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("相机高级设置");
        float newCameraSpeed = GUILayout.HorizontalSlider(cameraSpeed, 0.1f, 5f);
        if (newCameraSpeed != cameraSpeed)
        {
            cameraSpeed = newCameraSpeed;
        }
        GUILayout.Label($"相机移动速度: {cameraSpeed:F1}");
        
        float newZoomSpeed = GUILayout.HorizontalSlider(zoomSpeed, 0.1f, 3f);
        if (newZoomSpeed != zoomSpeed)
        {
            zoomSpeed = newZoomSpeed;
        }
        GUILayout.Label($"缩放速度: {zoomSpeed:F1}");
        
        GUILayout.Label("输入设置");
        float newInputThrottle = GUILayout.HorizontalSlider(inputThrottle, 0.01f, 0.5f);
        if (newInputThrottle != inputThrottle)
        {
            inputThrottle = newInputThrottle;
        }
        GUILayout.Label($"输入节流间隔: {inputThrottle:F2}s");
        
        GUILayout.Space(5);
        
        GUILayout.Label("网格高级设置");
        bool newShowGridNumbers = GUILayout.Toggle(showGridNumbers, "显示网格编号");
        if (newShowGridNumbers != showGridNumbers)
        {
            showGridNumbers = newShowGridNumbers;
            UpdateGridDisplay();
        }
        
        bool newShowGridCoordinates = GUILayout.Toggle(showGridCoordinates, "显示坐标");
        if (newShowGridCoordinates != showGridCoordinates)
        {
            showGridCoordinates = newShowGridCoordinates;
            UpdateGridDisplay();
        }
        
        float newGridLineWidth = GUILayout.HorizontalSlider(gridLineWidth, 0.01f, 0.1f);
        if (newGridLineWidth != gridLineWidth)
        {
            gridLineWidth = newGridLineWidth;
            UpdateGridDisplay();
        }
        GUILayout.Label($"网格线宽度: {gridLineWidth:F3}");
        
        // 注意：网格线宽度设置需要重新生成网格纹理才能生效
        if (GUILayout.Button("重新生成网格"))
        {
            UpdateGridDisplay();
        }
        
        GUILayout.Space(5);
        
        GUILayout.Label("卡片高级设置");
        bool newShowCardIDs = GUILayout.Toggle(showCardIDs, "显示卡片ID");
        if (newShowCardIDs != showCardIDs)
        {
            showCardIDs = newShowCardIDs;
            UpdateCardLabels();
        }
        
        bool newShowCardTypes = GUILayout.Toggle(showCardTypes, "显示卡片类型");
        if (newShowCardTypes != showCardTypes)
        {
            showCardTypes = newShowCardTypes;
            UpdateCardLabels();
        }
        
        float newCardHoverScale = GUILayout.HorizontalSlider(cardHoverScale, 1.0f, 2.0f);
        if (newCardHoverScale != cardHoverScale)
        {
            cardHoverScale = newCardHoverScale;
            // 悬停缩放会在鼠标悬停时自动应用
        }
        GUILayout.Label($"悬停缩放: {cardHoverScale:F1}");
        
        GUILayout.Space(5);
        
        GUILayout.Label("导出设置");
        bool newAutoSave = GUILayout.Toggle(autoSave, "自动保存");
        if (newAutoSave != autoSave)
        {
            autoSave = newAutoSave;
        }
        
        bool newBackupLevels = GUILayout.Toggle(backupLevels, "备份关卡");
        if (newBackupLevels != backupLevels)
        {
            backupLevels = newBackupLevels;
        }
        
        GUILayout.Label("导出格式");
        bool newExportJSON = GUILayout.Toggle(exportJSON, "JSON格式");
        if (newExportJSON != exportJSON)
        {
            exportJSON = newExportJSON;
        }
        
        bool newExportXML = GUILayout.Toggle(exportXML, "XML格式");
        if (newExportXML != exportXML)
        {
            exportXML = newExportXML;
        }
        
        bool newExportBinary = GUILayout.Toggle(exportBinary, "二进制格式");
        if (newExportBinary != exportBinary)
        {
            exportBinary = newExportBinary;
        }
        
        GUILayout.Space(10);
        
        // 操作按钮
        GUILayout.Label("操作");
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("保存关卡"))
        {
            SaveLevel();
        }
        if (GUILayout.Button("加载关卡"))
        {
            LoadLevel(currentLevelId);
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("新建关卡"))
        {
            NewLevel();
        }
        
        if (GUILayout.Button("验证关卡"))
        {
            ValidateCurrentLevel();
        }
        
        if (GUILayout.Button("导出关卡"))
        {
            ExportLevel();
        }
        
        GUILayout.Space(10);
        
        // 统计信息
        GUILayout.Label("统计信息");
        GUILayout.Label($"总卡片数: {levelCards.Count}");
        GUILayout.Label($"当前层级卡片: {levelCards.FindAll(c => c.layer == selectedLayer).Count}");
        
        GUILayout.Space(10);
        
        // 操作说明
        GUILayout.Label("操作说明");
        GUILayout.Label("左键: 放置卡片");
        GUILayout.Label("右键: 删除卡片");
        GUILayout.Label("滚轮: 切换层级");
        GUILayout.Label("Ctrl+滚轮: 缩放");
        GUILayout.Label("中键: 移动视角");
        GUILayout.Label("数字键1-8: 切换卡片类型");
        GUILayout.Label("Ctrl+S: 保存关卡");
        GUILayout.Label("Ctrl+L: 加载关卡");
        GUILayout.Label("Ctrl+N: 新建关卡");
        
        // 结束滚动视图
        GUILayout.EndScrollView();
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
        
        // 调试模式：显示调试信息
        if (debugMode)
        {
            DisplayDebugInfo();
        }
    }
    
    void DisplayDebugInfo()
    {
        // 在屏幕右上角显示调试信息
        GUILayout.BeginArea(new Rect(Screen.width - 200, 10, 190, 200));
        GUILayout.BeginVertical("box");
        GUILayout.Label("调试信息", GUI.skin.box);
        
        if (showFPS)
        {
            GUILayout.Label($"FPS: {Mathf.RoundToInt(1f / Time.deltaTime)}");
        }
        
        if (showMemory)
        {
            GUILayout.Label($"内存: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB");
        }
        
        GUILayout.Label($"卡片数: {levelCards.Count}");
        GUILayout.Label($"当前层级: {selectedLayer}");
        GUILayout.Label($"鼠标位置: {Input.mousePosition}");
        
        Vector3 mouseWorldPos = editorCamera2D.ScreenToWorldPoint(Input.mousePosition);
        GUILayout.Label($"世界坐标: {mouseWorldPos}");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    
    // 辅助输入方法，放在SheepLevelEditor2D类内部
    int IntField(string label, int value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        string text = GUILayout.TextField(value.ToString());
        GUILayout.EndHorizontal();
        int result;
        if (int.TryParse(text, out result))
        {
            return result;
        }
        return value;
    }
    
    string TextField(string label, string value)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        string result = GUILayout.TextField(value);
        GUILayout.EndHorizontal();
        return result;
    }
    
    int IntSlider(string label, int value, int min, int max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        int result = (int)GUILayout.HorizontalSlider(value, min, max);
        GUILayout.Label(result.ToString());
        GUILayout.EndHorizontal();
        return result;
    }
    
    Color ColorField(string label, Color color)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        
        // 显示当前颜色
        GUI.backgroundColor = color;
        if (GUILayout.Button("", GUILayout.Width(50), GUILayout.Height(20)))
        {
            // 简单的颜色选择：在几个预设颜色中循环
            if (color == Color.white)
                color = Color.red;
            else if (color == Color.red)
                color = Color.green;
            else if (color == Color.green)
                color = Color.blue;
            else if (color == Color.blue)
                color = Color.yellow;
            else if (color == Color.yellow)
                color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 灰色
            else
                color = Color.white;
        }
        GUI.backgroundColor = Color.white;
        
        GUILayout.Label($"R:{color.r:F2} G:{color.g:F2} B:{color.b:F2} A:{color.a:F2}");
        GUILayout.EndHorizontal();
        
        return color;
    }
    
    public void ValidateCurrentLevel()
    {
        LevelData2D currentLevel = new LevelData2D
        {
            levelName = currentLevelName,
            levelId = currentLevelId,
            cards = new List<CardData2D>(levelCards),
            totalLayers = totalLayers,
            gridSize = gridSize,
            cardSpacing = cardSpacing
        };
        
        LevelValidator2D.ValidationResult result = LevelValidator2D.ValidateLevel(currentLevel);
        string report = LevelValidator2D.GetValidationReport(result);
        
        Debug.Log(report);
        
        if (result.isValid)
        {
            Debug.Log("✅ 2D关卡验证通过！");
        }
        else
        {
            Debug.LogError("❌ 2D关卡验证失败！请检查错误信息。");
        }
    }
    
    public void ExportLevel()
    {
        LevelData2D levelData = new LevelData2D
        {
            levelName = currentLevelName,
            levelId = currentLevelId,
            cards = new List<CardData2D>(levelCards),
            totalLayers = totalLayers,
            gridSize = gridSize,
            cardSpacing = cardSpacing,
            cardSize = cardSize,
            useCustomAreaSize = useCustomAreaSize,
            areaSize = areaSize,
            enableLayerPreview = enableLayerPreview,
            normalLayerColor = normalLayerColor,
            grayedLayerColor = grayedLayerColor,
            
            // 导出高级设置
            debugMode = debugMode,
            highPerformance = highPerformance,
            showFPS = showFPS,
            showMemory = showMemory,
            cameraSpeed = cameraSpeed,
            zoomSpeed = zoomSpeed,
            inputThrottle = inputThrottle,
            showGrid = showGrid,
            showGridNumbers = showGridNumbers,
            showGridCoordinates = showGridCoordinates,
            gridLineWidth = gridLineWidth,
            showCardIDs = showCardIDs,
            showCardTypes = showCardTypes,
            cardHoverScale = cardHoverScale,
            autoSave = autoSave,
            backupLevels = backupLevels,
            exportJSON = exportJSON,
            exportXML = exportXML,
            exportBinary = exportBinary
        };
        
        string exportPath = Path.Combine(Application.dataPath, "Levels", "Exports");
        Directory.CreateDirectory(exportPath);
        
        if (exportJSON)
        {
            string jsonPath = Path.Combine(exportPath, $"Level2D_{currentLevelId}_export.json");
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(jsonPath, json);
            Debug.Log($"已导出JSON: {jsonPath}");
        }
        
        if (exportXML)
        {
            string xmlPath = Path.Combine(exportPath, $"Level2D_{currentLevelId}_export.xml");
            string xml = ConvertToXML(levelData);
            File.WriteAllText(xmlPath, xml);
            Debug.Log($"已导出XML: {xmlPath}");
        }
        
        if (exportBinary)
        {
            string binaryPath = Path.Combine(exportPath, $"Level2D_{currentLevelId}_export.bin");
            byte[] binary = ConvertToBinary(levelData);
            File.WriteAllBytes(binaryPath, binary);
            Debug.Log($"已导出二进制: {binaryPath}");
        }
        
        Debug.Log("关卡导出完成！");
    }
    
    string ConvertToXML(LevelData2D levelData)
    {
        // 简单的XML转换
        System.Text.StringBuilder xml = new System.Text.StringBuilder();
        xml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xml.AppendLine("<LevelData>");
        xml.AppendLine($"  <levelName>{levelData.levelName}</levelName>");
        xml.AppendLine($"  <levelId>{levelData.levelId}</levelId>");
        xml.AppendLine($"  <totalLayers>{levelData.totalLayers}</totalLayers>");
        xml.AppendLine($"  <gridSize>{levelData.gridSize.x},{levelData.gridSize.y}</gridSize>");
        xml.AppendLine($"  <cardSpacing>{levelData.cardSpacing}</cardSpacing>");
        xml.AppendLine($"  <cardSize>{levelData.cardSize}</cardSize>");
        xml.AppendLine("  <cards>");
        
        foreach (var card in levelData.cards)
        {
            xml.AppendLine("    <card>");
            xml.AppendLine($"      <id>{card.id}</id>");
            xml.AppendLine($"      <type>{card.type}</type>");
            xml.AppendLine($"      <position>{card.position.x},{card.position.y}</position>");
            xml.AppendLine($"      <layer>{card.layer}</layer>");
            xml.AppendLine($"      <isVisible>{card.isVisible}</isVisible>");
            xml.AppendLine("    </card>");
        }
        
        xml.AppendLine("  </cards>");
        xml.AppendLine("</LevelData>");
        
        return xml.ToString();
    }
    
    byte[] ConvertToBinary(LevelData2D levelData)
    {
        // 简单的二进制转换
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(levelData.levelName);
            writer.Write(levelData.levelId);
            writer.Write(levelData.totalLayers);
            writer.Write(levelData.gridSize.x);
            writer.Write(levelData.gridSize.y);
            writer.Write(levelData.cardSpacing);
            writer.Write(levelData.cardSize);
            writer.Write(levelData.cards.Count);
            
            foreach (var card in levelData.cards)
            {
                writer.Write(card.id);
                writer.Write(card.type);
                writer.Write(card.position.x);
                writer.Write(card.position.y);
                writer.Write(card.layer);
                writer.Write(card.isVisible);
            }
            
            return stream.ToArray();
        }
    }
    
    // 公共访问方法，用于测试脚本
    public List<CardData2D> GetLevelCards()
    {
        return new List<CardData2D>(levelCards);
    }
    
    public List<GameObject> GetCardObjects()
    {
        return new List<GameObject>(cardObjects);
    }
    
    public void AddCardData(CardData2D cardData)
    {
        levelCards.Add(cardData);
    }
    
    public void ClearCardObjects()
    {
        cardObjects.Clear();
    }
    
    public void ClearAllCards()
    {
        levelCards.Clear();
        foreach (var cardObj in cardObjects)
        {
            if (cardObj != null)
            {
                DestroyImmediate(cardObj);
            }
        }
        cardObjects.Clear();
        UpdateCardDisplay();
        UpdateCardLabels(); // 更新卡片标签
    }
}

// 2D卡片对象组件
public class CardObject2D : MonoBehaviour
{
    public int cardId;
    public int cardType;
    public int layer;
    public float baseCardSize = 0.8f; // 基础卡片大小
    private Color originalColor; // 保存原始颜色
    
    void Start()
    {
        // 保存原始颜色
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    void OnMouseEnter()
    {
        // 获取编辑器实例以访问设置
        SheepLevelEditor2D editor = FindObjectOfType<SheepLevelEditor2D>();
        float hoverScale = editor != null ? editor.cardHoverScale : 1.2f;
        
        // 鼠标悬停时放大卡片并高亮
        transform.localScale = Vector3.one * (baseCardSize * hoverScale);
        
        // 悬停时使用高亮颜色
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow; // 悬停高亮颜色
        }
    }
    
    void OnMouseExit()
    {
        // 鼠标离开时恢复原始大小和颜色
        transform.localScale = Vector3.one * baseCardSize;
        
        // 恢复原始颜色（由层级预览系统管理）
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 通知编辑器更新颜色
            SheepLevelEditor2D editor = FindObjectOfType<SheepLevelEditor2D>();
            if (editor != null)
            {
                editor.UpdateCardLayerPreview(gameObject, this);
            }
        }
    }
} 