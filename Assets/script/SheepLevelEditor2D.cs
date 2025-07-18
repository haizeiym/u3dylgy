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
}

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
    
    [Header("2D设置")]
    public Camera editorCamera2D;
    public float cameraZoom = 5f;
    public Vector2 cameraPosition = Vector2.zero;
    
    [Header("层级遮罩设置")]
    public bool showLayerMasks = true;
    public Color layerMaskColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    public float layerMaskHeight = 0.1f;
    
    [Header("可放置区域可视化")]
    public bool showPlaceableArea = true;
    public PlaceableAreaVisualizer placeableAreaVisualizer;
    
    private List<CardData2D> levelCards = new List<CardData2D>();
    private List<GameObject> cardObjects = new List<GameObject>();
    private List<GameObject> layerMaskObjects = new List<GameObject>();
    private Vector3 lastMousePosition;
    
    void Start()
    {
        Setup2DCamera();
        Create2DGrid();
        CreateLayerMasks();
        SetupPlaceableAreaVisualizer();
        LoadLevel(currentLevelId);
        UpdateCardDisplay();
        
        // 确保GUI事件管理器存在
        if (GUIEventManager.Instance != null)
        {
            GUIEventManager.Instance.SetGUIRect(new Rect(10, 10, 300, Screen.height - 20));
        }
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
        float gridWidth = (gridSize.x - 1) * cardSpacing;
        float gridHeight = (gridSize.y - 1) * cardSpacing;
        gridBackground.transform.localScale = new Vector3(gridWidth, gridHeight, 1);
        
        // 更新网格以适应卡片大小
        UpdateGridForCardSize();
    }
    
    Sprite CreateGridSprite()
    {
        // 创建简单的网格纹理
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color gridColor = Color.gray;
        Color bgColor = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                if (x % 8 == 0 || y % 8 == 0)
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
    
    void UpdateGridForCardSize()
    {
        // 根据卡片大小调整网格密度
        GameObject gridBackground = GameObject.Find("GridBackground");
        if (gridBackground != null)
        {
            SpriteRenderer gridRenderer = gridBackground.GetComponent<SpriteRenderer>();
            if (gridRenderer != null)
            {
                // 调整网格纹理以适应卡片大小
                gridRenderer.sprite = CreateGridSprite();
            }
        }
    }
    
    void CreateLayerMasks()
    {
        // 清除现有的层级遮罩
        ClearLayerMasks();
        
        if (!showLayerMasks) return;
        
        // 为每个层级创建遮罩
        for (int layer = 0; layer < totalLayers; layer++)
        {
            CreateLayerMask(layer);
        }
    }
    
    void CreateLayerMask(int layer)
    {
        GameObject maskObj = new GameObject($"LayerMask_{layer}");
        maskObj.transform.position = new Vector3(0, layer * layerMaskHeight, -0.5f);
        
        // 创建遮罩精灵
        SpriteRenderer maskRenderer = maskObj.AddComponent<SpriteRenderer>();
        maskRenderer.sprite = CreateMaskSprite();
        maskRenderer.color = layerMaskColor;
        maskRenderer.sortingOrder = -2; // 在网格后面，但在背景前面
        
        // 设置遮罩大小覆盖整个可放置区域
        float maskWidth = (gridSize.x - 1) * cardSpacing;
        float maskHeight = (gridSize.y - 1) * cardSpacing;
        maskObj.transform.localScale = new Vector3(maskWidth, maskHeight, 1);
        
        layerMaskObjects.Add(maskObj);
    }
    
    Sprite CreateMaskSprite()
    {
        // 创建简单的遮罩纹理
        int textureSize = 32;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color maskColor = Color.white;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                texture.SetPixel(x, y, maskColor);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    void ClearLayerMasks()
    {
        foreach (GameObject maskObj in layerMaskObjects)
        {
            if (maskObj != null)
            {
                DestroyImmediate(maskObj);
            }
        }
        layerMaskObjects.Clear();
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
        }
    }
    
    void UpdateLayerMasks()
    {
        // 更新层级遮罩的显示状态
        for (int i = 0; i < layerMaskObjects.Count; i++)
        {
            if (layerMaskObjects[i] != null)
            {
                SpriteRenderer maskRenderer = layerMaskObjects[i].GetComponent<SpriteRenderer>();
                if (maskRenderer != null)
                {
                    // 当前编辑层级不显示遮罩，其他层级显示遮罩
                    bool shouldShow = showLayerMasks && i != selectedLayer;
                    maskRenderer.enabled = shouldShow;
                    
                    // 更新遮罩颜色（包括透明度）
                    maskRenderer.color = layerMaskColor;
                }
            }
        }
        
        Debug.Log($"层级遮罩已更新: 显示={showLayerMasks}, 当前层级={selectedLayer}, 遮罩数量={layerMaskObjects.Count}");
    }
    
    void UpdateLayerMaskSizes()
    {
        // 更新所有层级遮罩的大小以匹配可放置区域
        float maskWidth = (gridSize.x - 1) * cardSpacing;
        float maskHeight = (gridSize.y - 1) * cardSpacing;
        
        foreach (GameObject maskObj in layerMaskObjects)
        {
            if (maskObj != null)
            {
                maskObj.transform.localScale = new Vector3(maskWidth, maskHeight, 1);
            }
        }
        
        Debug.Log($"层级遮罩大小已更新: {maskWidth} x {maskHeight}");
    }
    
    public void UpdateGridAndMasks()
    {
        // 更新网格大小覆盖整个可放置区域
        GameObject gridBackground = GameObject.Find("GridBackground");
        float gridWidth = (gridSize.x - 1) * cardSpacing;
        float gridHeight = (gridSize.y - 1) * cardSpacing;
        
        if (gridBackground != null)
        {
            gridBackground.transform.localScale = new Vector3(gridWidth, gridHeight, 1);
        }
        
        // 更新层级遮罩大小
        UpdateLayerMaskSizes();
        
        // 更新可放置区域可视化
        if (placeableAreaVisualizer != null)
        {
            placeableAreaVisualizer.SetVisible(showPlaceableArea);
        }
        
        Debug.Log($"网格和遮罩已更新: 网格大小={gridSize}, 间距={cardSpacing}, 可放置区域={gridWidth} x {gridHeight}");
    }
    
    void Update()
    {
        if (!isEditMode) return;
        
        HandleInput();
        HandleCameraMovement();
    }
    
    void HandleInput()
    {
        // 多重检查鼠标是否在GUI区域内
        bool isOverGUI = false;
        
        // 主要检查：使用GUI事件管理器
        if (GUIEventManager.Instance != null)
        {
            isOverGUI = GUIEventManager.Instance.IsMouseOverGUI();
            
            // 如果GUI事件管理器说不在GUI上，但我们在GUI区域内，强制检查
            if (!isOverGUI)
            {
                isOverGUI = GUIEventManager.Instance.IsMouseOverGUINow();
            }
        }
        
        // 备用检查方法
        if (!isOverGUI)
        {
            Vector2 mousePos = Input.mousePosition;
            Rect guiRect = new Rect(10, 10, 300, Screen.height - 20);
            isOverGUI = guiRect.Contains(mousePos);
        }
        
        // 最终检查：如果鼠标在屏幕左侧300像素内，认为在GUI上
        if (!isOverGUI && Input.mousePosition.x < 320)
        {
            isOverGUI = true;
        }
        
        if (isOverGUI)
        {
            Debug.Log($"鼠标在GUI区域内，忽略游戏输入 - 位置: {Input.mousePosition}");
            return; // 如果鼠标在GUI上，不处理游戏输入
        }
        
        // 鼠标左键放置卡片
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("处理左键点击 - 放置卡片");
            PlaceCard2D();
        }
        
        // 鼠标右键删除卡片
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("处理右键点击 - 删除卡片");
            DeleteCard2D();
        }
        
        // 滚轮切换层级
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && !Input.GetKey(KeyCode.LeftControl))
        {
            int oldLayer = selectedLayer;
            selectedLayer = Mathf.Clamp(selectedLayer + (scroll > 0 ? 1 : -1), 0, totalLayers - 1);
            
            if (oldLayer != selectedLayer)
            {
                UpdateCardDisplay();
                UpdateLayerMasks(); // 更新层级遮罩显示
                Debug.Log($"层级已切换到: {selectedLayer}");
            }
        }
        
        // 数字键切换卡片类型
        for (int i = 0; i < maxCardTypes && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentCardType = i;
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
            Vector3 worldDelta = editorCamera2D.ScreenToWorldPoint(delta) - editorCamera2D.ScreenToWorldPoint(Vector3.zero);
            editorCamera2D.transform.position -= worldDelta;
        }
        
        // Ctrl+滚轮缩放
        if (Input.GetKey(KeyCode.LeftControl))
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                cameraZoom = Mathf.Clamp(cameraZoom - scroll * 2f, 1f, 20f);
                editorCamera2D.orthographicSize = cameraZoom;
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
        
        Debug.Log($"尝试放置2D卡片: 鼠标世界坐标={worldPos2D}, 网格坐标={gridPos}");
        
        // 检查位置是否在网格范围内
        if (!IsPositionInGridBounds2D(gridPos))
        {
            Debug.Log($"位置超出网格范围: {gridPos}");
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
            Debug.Log($"成功放置2D卡片: ID={newCard.id}, 位置={gridPos}, 类型={currentCardType}, 层级={selectedLayer}");
        }
        else
        {
            Debug.Log($"位置已被占用: {gridPos}");
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
                Debug.Log($"删除2D卡片: ID={cardData.id}, 类型={cardData.type}, 层级={cardData.layer}");
            }
        }
    }
    
    Vector2 SnapToGrid2D(Vector2 worldPos)
    {
        float x = Mathf.Round(worldPos.x / cardSpacing) * cardSpacing;
        float y = Mathf.Round(worldPos.y / cardSpacing) * cardSpacing;
        return new Vector2(x, y);
    }
    
    bool IsPositionOccupied2D(Vector2 position)
    {
        // 只检查同一层级的卡片是否占用位置
        return levelCards.Exists(c => c.layer == selectedLayer && Vector2.Distance(c.position, position) < cardSize * 0.5f);
    }
    
    bool IsPositionInGridBounds2D(Vector2 position)
    {
        // 计算网格边界
        float halfGridWidth = (gridSize.x - 1) * cardSpacing * 0.5f;
        float halfGridHeight = (gridSize.y - 1) * cardSpacing * 0.5f;
        
        // 检查位置是否在网格范围内
        bool inBounds = Mathf.Abs(position.x) <= halfGridWidth && Mathf.Abs(position.y) <= halfGridHeight;
        
        Debug.Log($"网格边界检查: 位置={position}, 网格大小={gridSize}, 卡片间距={cardSpacing}, 边界范围=±({halfGridWidth}, {halfGridHeight}), 在范围内={inBounds}");
        
        return inBounds;
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
    
    void CreateCardObject2D(CardData2D cardData)
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
        foreach (var cardObj in cardObjects)
        {
            CardObject2D cardComponent = cardObj.GetComponent<CardObject2D>();
            if (cardComponent != null)
            {
                // 只显示当前层级的卡片
                cardObj.SetActive(cardComponent.layer == selectedLayer);
            }
        }
        
        // 更新层级遮罩
        UpdateLayerMasks();
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
        
        // 更新层级遮罩大小以匹配网格
        UpdateLayerMaskSizes();
        
        Debug.Log($"卡片大小已更新为: {cardSize}");
    }
    
    void SaveLevel()
    {
        LevelData2D levelData = new LevelData2D
        {
            levelName = currentLevelName,
            levelId = currentLevelId,
            cards = new List<CardData2D>(levelCards),
            totalLayers = totalLayers,
            gridSize = gridSize,
            cardSpacing = cardSpacing
        };
        
        // 保存卡片大小信息到关卡数据中
        levelData.cardSize = cardSize;
        
        string json = JsonUtility.ToJson(levelData, true);
        string filePath = Path.Combine(Application.dataPath, "Levels", $"Level2D_{currentLevelId}.json");
        
        // 确保目录存在
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        
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
            Debug.Log($"2D关卡已加载: 关卡ID={currentLevelId}, 关卡名称={currentLevelName}, 文件路径={filePath}");
        }
        else
        {
            Debug.Log($"2D关卡文件不存在: {filePath}，创建新关卡");
            NewLevel();
        }
    }
    
    void NewLevel()
    {
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
        
        // 强制更新显示
        UpdateCardDisplay();
        
        Debug.Log($"新建2D关卡: {currentLevelName}");
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
        currentLevelName = TextField("关卡名称", currentLevelName);
        
        GUILayout.Space(10);
        
        // 编辑器设置
        GUILayout.Label("编辑器设置");
        totalLayers = IntField("总层数", totalLayers);
        
        int newSelectedLayer = IntSlider("当前层级", selectedLayer, 0, totalLayers - 1);
        if (newSelectedLayer != selectedLayer)
        {
            selectedLayer = newSelectedLayer;
            UpdateCardDisplay();
            UpdateLayerMasks(); // 更新层级遮罩显示
            Debug.Log($"GUI层级已切换到: {selectedLayer}");
        }
        
        currentCardType = IntSlider("卡片类型", currentCardType, 0, maxCardTypes - 1);
        
        // 网格设置
        GUILayout.Space(5);
        GUILayout.Label("网格设置");
        
        // 网格大小X
        float newGridSizeX = GUILayout.HorizontalSlider(gridSize.x, 4f, 16f);
        GUILayout.Label($"网格宽度: {newGridSizeX:F0}");
        if (newGridSizeX != gridSize.x)
        {
            gridSize.x = newGridSizeX;
            UpdateGridAndMasks();
        }
        
        // 网格大小Y
        float newGridSizeY = GUILayout.HorizontalSlider(gridSize.y, 4f, 16f);
        GUILayout.Label($"网格高度: {newGridSizeY:F0}");
        if (newGridSizeY != gridSize.y)
        {
            gridSize.y = newGridSizeY;
            UpdateGridAndMasks();
        }
        
        // 卡片间距
        float newCardSpacing = GUILayout.HorizontalSlider(cardSpacing, 0.8f, 2.0f);
        GUILayout.Label($"卡片间距: {newCardSpacing:F2}");
        if (newCardSpacing != cardSpacing)
        {
            cardSpacing = newCardSpacing;
            UpdateGridAndMasks();
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
        }
        
        GUILayout.Space(10);
        
        // 2D相机设置
        GUILayout.Label("2D相机设置");
        cameraZoom = GUILayout.HorizontalSlider(cameraZoom, 1f, 20f);
        GUILayout.Label($"缩放: {cameraZoom:F1}");
        if (editorCamera2D != null)
        {
            editorCamera2D.orthographicSize = cameraZoom;
        }
        
        GUILayout.Space(10);
        
        // 层级遮罩设置
        GUILayout.Label("层级遮罩设置");
        bool newShowLayerMasks = GUILayout.Toggle(showLayerMasks, "显示层级遮罩");
        if (newShowLayerMasks != showLayerMasks)
        {
            showLayerMasks = newShowLayerMasks;
            
            if (showLayerMasks)
            {
                // 如果开启遮罩，重新创建所有遮罩
                CreateLayerMasks();
            }
            else
            {
                // 如果关闭遮罩，清除所有遮罩
                ClearLayerMasks();
            }
            
            Debug.Log($"层级遮罩设置已更改: {(showLayerMasks ? "显示" : "隐藏")}");
        }
        
        if (showLayerMasks)
        {
            GUILayout.Label("遮罩透明度");
            float newAlpha = GUILayout.HorizontalSlider(layerMaskColor.a, 0f, 1f);
            if (newAlpha != layerMaskColor.a)
            {
                layerMaskColor.a = newAlpha;
                UpdateLayerMasks();
            }
            GUILayout.Label($"透明度: {newAlpha:F2}");
        }
        
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
        }
        
        GUILayout.Space(10);
        
        // 高级设置
        GUILayout.Label("高级设置");
        GUILayout.Label("调试模式");
        bool debugMode = GUILayout.Toggle(false, "启用调试日志");
        
        GUILayout.Label("性能设置");
        bool highPerformance = GUILayout.Toggle(true, "高性能模式");
        
        GUILayout.Label("显示设置");
        bool showFPS = GUILayout.Toggle(false, "显示FPS");
        bool showMemory = GUILayout.Toggle(false, "显示内存使用");
        
        GUILayout.Space(5);
        
        GUILayout.Label("相机高级设置");
        float cameraSpeed = GUILayout.HorizontalSlider(1f, 0.1f, 5f);
        GUILayout.Label($"相机移动速度: {cameraSpeed:F1}");
        
        float zoomSpeed = GUILayout.HorizontalSlider(1f, 0.1f, 3f);
        GUILayout.Label($"缩放速度: {zoomSpeed:F1}");
        
        GUILayout.Space(5);
        
        GUILayout.Label("网格高级设置");
        bool showGridNumbers = GUILayout.Toggle(false, "显示网格编号");
        bool showGridCoordinates = GUILayout.Toggle(false, "显示坐标");
        
        float gridLineWidth = GUILayout.HorizontalSlider(0.02f, 0.01f, 0.1f);
        GUILayout.Label($"网格线宽度: {gridLineWidth:F3}");
        
        GUILayout.Space(5);
        
        GUILayout.Label("卡片高级设置");
        bool showCardIDs = GUILayout.Toggle(false, "显示卡片ID");
        bool showCardTypes = GUILayout.Toggle(false, "显示卡片类型");
        
        float cardHoverScale = GUILayout.HorizontalSlider(1.2f, 1.0f, 2.0f);
        GUILayout.Label($"悬停缩放: {cardHoverScale:F1}");
        
        GUILayout.Space(5);
        
        GUILayout.Label("导出设置");
        bool autoSave = GUILayout.Toggle(true, "自动保存");
        bool backupLevels = GUILayout.Toggle(true, "备份关卡");
        
        GUILayout.Label("导出格式");
        bool exportJSON = GUILayout.Toggle(true, "JSON格式");
        bool exportXML = GUILayout.Toggle(false, "XML格式");
        bool exportBinary = GUILayout.Toggle(false, "二进制格式");
        
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
    
    void ValidateCurrentLevel()
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
}

// 2D卡片对象组件
public class CardObject2D : MonoBehaviour
{
    public int cardId;
    public int cardType;
    public int layer;
    public float baseCardSize = 0.8f; // 基础卡片大小
    
    void OnMouseEnter()
    {
        // 鼠标悬停效果 - 基于基础卡片大小
        transform.localScale = Vector3.one * baseCardSize * 1.1f;
    }
    
    void OnMouseExit()
    {
        // 恢复正常大小 - 基于基础卡片大小
        transform.localScale = Vector3.one * baseCardSize;
    }
} 