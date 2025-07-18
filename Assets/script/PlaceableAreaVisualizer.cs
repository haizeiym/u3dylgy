using UnityEngine;

public class PlaceableAreaVisualizer : MonoBehaviour
{
    [Header("可放置区域设置")]
    public bool showPlaceableArea = true;
    public Color placeableAreaColor = new Color(0.2f, 0.8f, 0.2f, 0.3f); // 绿色半透明
    public Color borderColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // 绿色边框
    public float borderWidth = 0.05f;
    public float areaHeight = 0.05f;
    
    [Header("网格线设置")]
    public bool showGridLines = true;
    public Color gridLineColor = new Color(0.3f, 0.7f, 0.3f, 0.5f);
    public float gridLineWidth = 0.02f;
    
    private GameObject placeableAreaObject;
    private GameObject borderObject;
    private GameObject gridLinesObject;
    private SheepLevelEditor2D levelEditor;
    
    void Start()
    {
        levelEditor = FindObjectOfType<SheepLevelEditor2D>();
        if (levelEditor == null)
        {
            Debug.LogError("找不到SheepLevelEditor2D组件！");
            return;
        }
        
        // 初始化缓存值
        lastGridSize = levelEditor.gridSize;
        lastCardSpacing = levelEditor.cardSpacing;
        
        CreatePlaceableArea();
        CreateBorder();
        CreateGridLines();
    }
    
    private Vector2 lastGridSize;
    private float lastCardSpacing;
    
    void Update()
    {
        if (levelEditor != null && showPlaceableArea)
        {
            // 只在网格大小或卡片间距发生变化时才更新
            if (lastGridSize != levelEditor.gridSize || lastCardSpacing != levelEditor.cardSpacing)
            {
                UpdatePlaceableArea();
                lastGridSize = levelEditor.gridSize;
                lastCardSpacing = levelEditor.cardSpacing;
            }
        }
    }
    
    void CreatePlaceableArea()
    {
        if (!showPlaceableArea) return;
        
        placeableAreaObject = new GameObject("PlaceableArea");
        placeableAreaObject.transform.position = Vector3.zero;
        
        SpriteRenderer areaRenderer = placeableAreaObject.AddComponent<SpriteRenderer>();
        areaRenderer.sprite = CreateAreaSprite();
        areaRenderer.color = placeableAreaColor;
        areaRenderer.sortingOrder = -3; // 在最底层
        
        UpdateAreaSize();
    }
    
    void CreateBorder()
    {
        if (!showPlaceableArea) return;
        
        borderObject = new GameObject("PlaceableAreaBorder");
        borderObject.transform.position = Vector3.zero;
        
        SpriteRenderer borderRenderer = borderObject.AddComponent<SpriteRenderer>();
        borderRenderer.sprite = CreateBorderSprite();
        borderRenderer.color = borderColor;
        borderRenderer.sortingOrder = -2; // 在区域上面
        
        UpdateBorderSize();
    }
    
    void CreateGridLines()
    {
        if (!showGridLines) return;
        
        // 如果GridLines对象已存在，先销毁它
        if (gridLinesObject != null)
        {
            DestroyImmediate(gridLinesObject);
        }
        
        gridLinesObject = new GameObject("GridLines");
        gridLinesObject.transform.position = Vector3.zero;
        
        // 创建水平线
        for (int y = 0; y <= levelEditor.gridSize.y; y++)
        {
            CreateGridLine(new Vector2(-levelEditor.cardSpacing * 0.5f, y * levelEditor.cardSpacing - levelEditor.cardSpacing * 0.5f),
                          new Vector2((levelEditor.gridSize.x - 1) * levelEditor.cardSpacing + levelEditor.cardSpacing * 0.5f, y * levelEditor.cardSpacing - levelEditor.cardSpacing * 0.5f),
                          true);
        }
        
        // 创建垂直线
        for (int x = 0; x <= levelEditor.gridSize.x; x++)
        {
            CreateGridLine(new Vector2(x * levelEditor.cardSpacing - levelEditor.cardSpacing * 0.5f, -levelEditor.cardSpacing * 0.5f),
                          new Vector2(x * levelEditor.cardSpacing - levelEditor.cardSpacing * 0.5f, (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing + levelEditor.cardSpacing * 0.5f),
                          false);
        }
    }
    
    void CreateGridLine(Vector2 start, Vector2 end, bool isHorizontal)
    {
        GameObject lineObj = new GameObject($"GridLine_{(isHorizontal ? "H" : "V")}_{(isHorizontal ? start.y : start.x)}");
        lineObj.transform.SetParent(gridLinesObject.transform);
        
        SpriteRenderer lineRenderer = lineObj.AddComponent<SpriteRenderer>();
        lineRenderer.sprite = CreateLineSprite();
        lineRenderer.color = gridLineColor;
        lineRenderer.sortingOrder = -1;
        
        // 设置位置和大小
        Vector2 center = (start + end) * 0.5f;
        lineObj.transform.position = new Vector3(center.x, center.y, -0.1f);
        
        float length = Vector2.Distance(start, end);
        lineObj.transform.localScale = new Vector3(length, gridLineWidth, 1);
    }
    
    Sprite CreateAreaSprite()
    {
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color areaColor = Color.white;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                texture.SetPixel(x, y, areaColor);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    Sprite CreateBorderSprite()
    {
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color borderColor = Color.white;
        Color transparentColor = new Color(1, 1, 1, 0);
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                // 创建边框效果
                if (x < 4 || x >= textureSize - 4 || y < 4 || y >= textureSize - 4)
                {
                    texture.SetPixel(x, y, borderColor);
                }
                else
                {
                    texture.SetPixel(x, y, transparentColor);
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    Sprite CreateLineSprite()
    {
        int textureSize = 16;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        Color lineColor = Color.white;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                texture.SetPixel(x, y, lineColor);
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f));
    }
    
    void UpdatePlaceableArea()
    {
        UpdateAreaSize();
        UpdateBorderSize();
        UpdateGridLines();
    }
    
    void UpdateAreaSize()
    {
        if (placeableAreaObject != null && levelEditor != null)
        {
            float areaWidth = (levelEditor.gridSize.x - 1) * levelEditor.cardSpacing;
            float areaHeight = (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing;
            placeableAreaObject.transform.localScale = new Vector3(areaWidth, areaHeight, 1);
        }
    }
    
    void UpdateBorderSize()
    {
        if (borderObject != null && levelEditor != null)
        {
            float borderWidth = (levelEditor.gridSize.x - 1) * levelEditor.cardSpacing + this.borderWidth * 2;
            float borderHeight = (levelEditor.gridSize.y - 1) * levelEditor.cardSpacing + this.borderWidth * 2;
            borderObject.transform.localScale = new Vector3(borderWidth, borderHeight, 1);
        }
    }
    
    void UpdateGridLines()
    {
        if (gridLinesObject != null)
        {
            // 清除现有网格线
            foreach (Transform child in gridLinesObject.transform)
            {
                if (child != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
            // 重新创建网格线
            CreateGridLines();
        }
    }
    
    public void SetVisible(bool visible)
    {
        showPlaceableArea = visible;
        showGridLines = visible;
        
        if (placeableAreaObject != null)
            placeableAreaObject.SetActive(visible);
        if (borderObject != null)
            borderObject.SetActive(visible);
        if (gridLinesObject != null)
            gridLinesObject.SetActive(visible);
    }
    
    public void UpdateColors(Color areaColor, Color borderColor, Color gridColor)
    {
        placeableAreaColor = areaColor;
        this.borderColor = borderColor;
        gridLineColor = gridColor;
        
        if (placeableAreaObject != null)
        {
            SpriteRenderer renderer = placeableAreaObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = placeableAreaColor;
        }
        
        if (borderObject != null)
        {
            SpriteRenderer renderer = borderObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
                renderer.color = this.borderColor;
        }
        
        if (gridLinesObject != null)
        {
            SpriteRenderer[] renderers = gridLinesObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in renderers)
            {
                renderer.color = gridLineColor;
            }
        }
    }
} 