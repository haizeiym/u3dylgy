using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YangLeGeYang2D.LevelEditor
{
    [System.Serializable]
    public class CardData2D
    {
        public int id;
        public int type;
        public Vector2 position;
        public int layer;
        public bool isVisible = true;
        public List<int> blockingCards = new List<int>();
    }

    [System.Serializable]
    public class LevelData2D
    {
        public string levelName = "Level_1";
        public int levelId = 1;
        public Vector2 gridSize = new Vector2(10, 10);
        public float cardSpacing = 1.2f;
        public Vector2 cardSize = new Vector2(1, 1);
        public int totalLayers = 3;
        public List<CardData2D> cards = new List<CardData2D>();
        public List<IrregularShapeData> irregularShapes = new List<IrregularShapeData>();
        
        // 高级设置
        public bool useCustomAreaSize = true;
        public Vector2 areaSize = new Vector2(16f, 16f);
        public bool enableLayerPreview = true;
        public Color normalLayerColor = Color.white;
        public Color grayedLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public bool debugMode = false;
        public bool highPerformance = true;
        public bool showFPS = false;
        public bool showMemory = false;
        public float cameraSpeed = 1f;
        public float zoomSpeed = 1f;
        public float inputThrottle = 0.1f;
        public bool showGrid = true;
        public bool showGridNumbers = false;
        public bool showGridCoordinates = false;
        public float gridLineWidth = 0.02f;
        public bool showCardIDs = false;
        public bool showCardTypes = false;
        public float cardHoverScale = 1.2f;
        public bool autoSave = true;
        public bool backupLevels = true;
        public bool exportJSON = true;
        public bool exportXML = false;
        public bool exportBinary = false;
    }

    [System.Serializable]
    public class IrregularShapeData
    {
        public int layer;
        public List<Vector2> vertices = new List<Vector2>();
        public Color shapeColor = Color.green;
        public bool isActive = true;
    }

    public class CuppingLevelEditor2D : MonoBehaviour
    {
        [Header("网格设置")]
        public Vector2 gridSize = new Vector2(10, 10);
        public float cardSpacing = 1.2f;
        public Vector2 cardSize = new Vector2(1, 1);
        public int totalLayers = 3;
        public int currentLayer = 0;

        [Header("可视化设置")]
        public Color placeableAreaColor = Color.green;
        public Color gridLineColor = Color.white;
        public Color borderColor = Color.red;
        public int textureSize = 512;
        public bool showGrid = true;
        public bool showPlaceableArea = true;
        public bool showIrregularShapes = true;

        [Header("性能设置")]
        public bool highPerformanceMode = false;
        public float inputThrottle = 0.1f;

        [Header("调试设置")]
        public bool debugMode = false;
        public bool showGridLabels = true;
        public bool showCoordinates = true;

        [Header("不规则图形设置")]
        public List<IrregularShapeData> irregularShapes = new List<IrregularShapeData>();
        public Color irregularShapeColor = Color.cyan;
        public float irregularShapeAlpha = 0.3f;
        public bool snapToIrregularShape = true;

        [Header("卡片设置")]
        public Sprite[] cardSprites;
        public int currentCardType = 0;
        public int maxCardTypes = 8;

        [Header("编辑器状态")]
        public bool isEditMode = true;
        public int selectedLayer = 0;
        public bool showGridInGUI = true;

        [Header("2D设置")]
        public Camera editorCamera2D;
        public float cameraZoom = 5f;
        public Vector2 cameraPosition = Vector2.zero;

        [Header("可放置区域可视化")]
        public bool showPlaceableAreaVisualizer = true;
        public PlaceableAreaVisualizer placeableAreaVisualizer;

        [Header("区域大小设置")]
        public Vector2 areaSize = new Vector2(16f, 16f);
        public bool useCustomAreaSize = true;

        [Header("层级预览设置")]
        public bool enableLayerPreview = true;
        public Color normalLayerColor = Color.white;
        public Color grayedLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private LevelData2D currentLevel;
        private Camera editorCamera;
        private float lastInputTime;
        private Texture2D placeableAreaTexture;
        private bool isMouseOverGUI;
        private Vector2 lastMousePosition;
        private bool isDragging;
        private Vector2 dragStartPosition;

        // 卡片对象管理
        private List<GameObject> cardObjects = new List<GameObject>();
        private List<GameObject> gridLabelObjects = new List<GameObject>();

        // 预设不规则图形
        private List<IrregularShapeData> presetShapes = new List<IrregularShapeData>();

        void Awake()
        {
            InitializeEditor();
            LoadPresetShapes();
        }

        void Start()
        {
            SetupCamera();
            CreatePlaceableAreaTexture();
            SetupPlaceableAreaVisualizer();
            NewLevel();
            UpdateCardDisplay();
            UpdateGridDisplay();
            UpdateCardLabels();
            
            // 确保GUI事件管理器存在
            if (GUIEventManager.Instance != null)
            {
                GUIEventManager.Instance.SetGUIRect(new Rect(10, 10, 300, Screen.height - 20));
            }
            
            Debug.Log("拔了个罐2D关卡编辑器已启动，所有设置已初始化");
        }

        void Update()
        {
            if (Time.time - lastInputTime < inputThrottle && !highPerformanceMode)
                return;

            HandleInput();
            UpdateMousePosition();
        }

        void OnGUI()
        {
            if (debugMode)
            {
                DrawDebugInfo();
            }
        }

        void OnDrawGizmos()
        {
            if (showGrid)
            {
                DrawGridGizmos();
            }
        }

        private void InitializeEditor()
        {
            editorCamera = Camera.main;
            if (editorCamera == null)
            {
                GameObject cameraObj = new GameObject("Editor Camera");
                editorCamera = cameraObj.AddComponent<Camera>();
                editorCamera.orthographic = true;
                editorCamera.backgroundColor = Color.black;
            }

            currentLevel = new LevelData2D();
        }

        private void LoadPresetShapes()
        {
            // 预设的不规则图形
            presetShapes.Clear();

            // 预设1: 圆形
            var circleShape = new IrregularShapeData
            {
                layer = 0,
                shapeColor = Color.cyan,
                isActive = true
            };
            for (int i = 0; i < 16; i++)
            {
                float angle = i * 22.5f * Mathf.Deg2Rad;
                float radius = 3f;
                circleShape.vertices.Add(new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                ));
            }
            presetShapes.Add(circleShape);

            // 预设2: 星形
            var starShape = new IrregularShapeData
            {
                layer = 1,
                shapeColor = Color.magenta,
                isActive = true
            };
            for (int i = 0; i < 10; i++)
            {
                float angle = i * 36f * Mathf.Deg2Rad;
                float radius = (i % 2 == 0) ? 4f : 2f;
                starShape.vertices.Add(new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius
                ));
            }
            presetShapes.Add(starShape);

            // 预设3: 矩形
            var rectangleShape = new IrregularShapeData
            {
                layer = 2,
                shapeColor = Color.yellow,
                isActive = true
            };
            rectangleShape.vertices.AddRange(new Vector2[]
            {
                new Vector2(-3, -2),
                new Vector2(3, -2),
                new Vector2(3, 2),
                new Vector2(-3, 2)
            });
            presetShapes.Add(rectangleShape);

            // 预设4: 三角形
            var triangleShape = new IrregularShapeData
            {
                layer = 0,
                shapeColor = Color.green,
                isActive = true
            };
            triangleShape.vertices.AddRange(new Vector2[]
            {
                new Vector2(0, 3),
                new Vector2(-2.5f, -2),
                new Vector2(2.5f, -2)
            });
            presetShapes.Add(triangleShape);
        }

        public void AddPresetShape(int presetIndex, int targetLayer)
        {
            if (presetIndex >= 0 && presetIndex < presetShapes.Count)
            {
                var presetShape = presetShapes[presetIndex];
                var newShape = new IrregularShapeData
                {
                    layer = targetLayer,
                    shapeColor = presetShape.shapeColor,
                    isActive = true
                };

                // 复制顶点并添加随机偏移
                foreach (var vertex in presetShape.vertices)
                {
                    Vector2 offset = new Vector2(
                        Random.Range(-1f, 1f),
                        Random.Range(-1f, 1f)
                    );
                    newShape.vertices.Add(vertex + offset);
                }

                irregularShapes.Add(newShape);
                currentLevel.irregularShapes.Add(newShape);
            }
        }

        public void RemoveIrregularShape(int index)
        {
            if (index >= 0 && index < irregularShapes.Count)
            {
                irregularShapes.RemoveAt(index);
                currentLevel.irregularShapes.RemoveAt(index);
            }
        }

        public bool IsPositionInIrregularShapes(Vector2 position, int layer)
        {
            foreach (var shape in irregularShapes)
            {
                if (shape.layer == layer && shape.isActive)
                {
                    if (IsPointInPolygon(position, shape.vertices))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsPointInPolygon(Vector2 point, List<Vector2> polygon)
        {
            if (polygon.Count < 3) return false;

            bool inside = false;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                if (((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                    (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }

        private void SetupCamera()
        {
            if (editorCamera != null)
            {
                editorCamera.orthographic = true;
                editorCamera.orthographicSize = cameraZoom;
                editorCamera.transform.position = new Vector3(cameraPosition.x, cameraPosition.y, -10);
                editorCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            }
        }

        private void SetupPlaceableAreaVisualizer()
        {
            if (showPlaceableAreaVisualizer && placeableAreaVisualizer == null)
            {
                placeableAreaVisualizer = gameObject.AddComponent<PlaceableAreaVisualizer>();
            }
        }

        private void CreatePlaceableAreaTexture()
        {
            placeableAreaTexture = new Texture2D(textureSize, textureSize);
            placeableAreaTexture.filterMode = FilterMode.Bilinear;
            UpdatePlaceableAreaTexture();
        }

        private void UpdatePlaceableAreaTexture()
        {
            if (placeableAreaTexture == null) return;

            Color[] pixels = new Color[textureSize * textureSize];
            Vector2 actualSize = GetActualAreaSize();
            float pixelSize = Mathf.Max(actualSize.x, actualSize.y) / textureSize;

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    Vector2 worldPos = new Vector2(
                        (x - textureSize * 0.5f) * pixelSize,
                        (y - textureSize * 0.5f) * pixelSize
                    );

                    bool isPlaceable = IsPositionPlaceable(worldPos);
                    pixels[y * textureSize + x] = isPlaceable ? placeableAreaColor : Color.clear;
                }
            }

            placeableAreaTexture.SetPixels(pixels);
            placeableAreaTexture.Apply();
        }

        private bool IsPositionPlaceable(Vector2 worldPos)
        {
            // 检查是否在网格边界内
            Vector2 actualSize = GetActualAreaSize();
            if (Mathf.Abs(worldPos.x) > actualSize.x * 0.5f || Mathf.Abs(worldPos.y) > actualSize.y * 0.5f)
                return false;

            // 检查是否在不规则图形内
            if (snapToIrregularShape && !IsPositionInIrregularShapes(worldPos, currentLayer))
                return false;

            // 检查是否与现有卡片重叠
            Vector2 snappedPos = SnapToGrid2D(worldPos);
            foreach (var card in currentLevel.cards)
            {
                if (Vector2.Distance(card.position, snappedPos) < cardSpacing * 0.5f)
                    return false;
            }

            return true;
        }

        private void HandleInput()
        {
            if (isMouseOverGUI) return;

            Vector2 mouseWorldPos = GetMouseWorldPosition();
            Vector2 snappedPos = SnapToGrid2D(mouseWorldPos);

            // 鼠标左键：放置卡片
            if (Input.GetMouseButtonDown(0) && IsPositionPlaceable(mouseWorldPos))
            {
                AddCard(snappedPos);
                lastInputTime = Time.time;
            }

            // 鼠标右键：删除卡片
            if (Input.GetMouseButtonDown(1))
            {
                RemoveCardAt(snappedPos);
                lastInputTime = Time.time;
            }

            // 鼠标滚轮：切换层级
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.1f)
            {
                currentLayer = Mathf.Clamp(currentLayer + (scroll > 0 ? 1 : -1), 0, totalLayers - 1);
                UpdatePlaceableAreaTexture();
                lastInputTime = Time.time;
            }

            // 相机控制
            HandleCameraControl();
        }

        private void HandleCameraControl()
        {
            // 拖拽移动相机
            if (Input.GetMouseButtonDown(2))
            {
                isDragging = true;
                dragStartPosition = GetMouseWorldPosition();
            }

            if (Input.GetMouseButtonUp(2))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector2 currentMousePos = GetMouseWorldPosition();
                Vector2 delta = dragStartPosition - currentMousePos;
                editorCamera.transform.position += new Vector3(delta.x, delta.y, 0);
                dragStartPosition = currentMousePos;
            }

            // 缩放相机
            float zoom = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(zoom) > 0.1f)
            {
                float newSize = editorCamera.orthographicSize - zoom * 2f;
                editorCamera.orthographicSize = Mathf.Clamp(newSize, 1f, 20f);
            }
        }

        private Vector2 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -editorCamera.transform.position.z;
            return editorCamera.ScreenToWorldPoint(mousePos);
        }

        private void UpdateMousePosition()
        {
            lastMousePosition = GetMouseWorldPosition();
        }

        public Vector2 SnapToGrid2D(Vector2 worldPos)
        {
            Vector2 snappedPos = new Vector2(
                Mathf.Round(worldPos.x / cardSpacing) * cardSpacing,
                Mathf.Round(worldPos.y / cardSpacing) * cardSpacing
            );
            return snappedPos;
        }

        private void AddCard(Vector2 position)
        {
            CardData2D newCard = new CardData2D
            {
                id = GetNextCardId(),
                type = currentCardType,
                position = position,
                layer = currentLayer,
                isVisible = true
            };

            currentLevel.cards.Add(newCard);
            CreateCardObject2D(newCard);
            UpdatePlaceableAreaTexture();
        }

        private void RemoveCardAt(Vector2 position)
        {
            for (int i = currentLevel.cards.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(currentLevel.cards[i].position, position) < cardSpacing * 0.5f)
                {
                    // 删除对应的GameObject
                    if (i < cardObjects.Count && cardObjects[i] != null)
                    {
                        DestroyImmediate(cardObjects[i]);
                        cardObjects.RemoveAt(i);
                    }
                    
                    currentLevel.cards.RemoveAt(i);
                    UpdatePlaceableAreaTexture();
                    break;
                }
            }
        }

        private int GetNextCardId()
        {
            int maxId = 0;
            foreach (var card in currentLevel.cards)
            {
                maxId = Mathf.Max(maxId, card.id);
            }
            return maxId + 1;
        }

        public void CreateCardObject2D(CardData2D cardData)
        {
            GameObject cardObj = new GameObject($"Card_{cardData.id}");
            cardObj.transform.position = new Vector3(cardData.position.x, cardData.position.y, -cardData.layer * 0.1f);
            
            // 添加SpriteRenderer
            SpriteRenderer renderer = cardObj.AddComponent<SpriteRenderer>();
            renderer.sprite = GetCardSprite(cardData.type);
            renderer.color = GetCardColor(cardData.layer);
            renderer.sortingOrder = cardData.layer;
            
            // 添加CardObject2D组件
            CardObject2D cardComponent = cardObj.AddComponent<CardObject2D>();
            cardComponent.cardId = cardData.id;
            cardComponent.cardType = cardData.type;
            cardComponent.layer = cardData.layer;
            
            // 设置卡片大小
            cardObj.transform.localScale = Vector3.one * cardSize.x;
            
            cardObjects.Add(cardObj);
        }

        private Sprite GetCardSprite(int cardType)
        {
            if (cardSprites != null && cardType < cardSprites.Length && cardSprites[cardType] != null)
            {
                return cardSprites[cardType];
            }
            return CreateDefaultCardSprite(cardType);
        }

        private Sprite CreateDefaultCardSprite(int cardType)
        {
            // 创建默认的卡片精灵
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            
            Color cardColor = GetCardColor(0);
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = cardColor;
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
        }

        private Color GetCardColor(int layer)
        {
            if (enableLayerPreview && layer != currentLayer)
            {
                return grayedLayerColor;
            }
            return normalLayerColor;
        }

        public void UpdateCardDisplay()
        {
            // 清除现有卡片对象
            ClearCardObjects();
            
            // 重新创建所有卡片对象
            foreach (var card in currentLevel.cards)
            {
                CreateCardObject2D(card);
            }
        }

        public void ClearCardObjects()
        {
            foreach (var cardObj in cardObjects)
            {
                if (cardObj != null)
                {
                    DestroyImmediate(cardObj);
                }
            }
            cardObjects.Clear();
        }

        public void UpdateGridDisplay()
        {
            // 这里可以添加网格显示逻辑
            if (showGrid)
            {
                // 网格显示逻辑
            }
        }

        public void UpdateCardLabels()
        {
            // 这里可以添加卡片标签显示逻辑
        }

        public void NewLevel()
        {
            currentLevel = new LevelData2D
            {
                levelName = $"Level_{GetNextLevelId()}",
                levelId = GetNextLevelId(),
                gridSize = gridSize,
                cardSpacing = cardSpacing,
                cardSize = cardSize,
                totalLayers = totalLayers,
                cards = new List<CardData2D>(),
                irregularShapes = new List<IrregularShapeData>(),
                useCustomAreaSize = useCustomAreaSize,
                areaSize = areaSize,
                enableLayerPreview = enableLayerPreview,
                normalLayerColor = normalLayerColor,
                grayedLayerColor = grayedLayerColor,
                debugMode = debugMode,
                highPerformance = highPerformanceMode,
                inputThrottle = inputThrottle,
                showGrid = showGrid
            };
            irregularShapes.Clear();
            ClearCardObjects();
            UpdatePlaceableAreaTexture();
        }

        public void SaveLevel()
        {
            string path = Path.Combine(Application.dataPath, "script/Levels");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileName = $"Level2D_{currentLevel.levelId}.json";
            string fullPath = Path.Combine(path, fileName);

            string json = JsonUtility.ToJson(currentLevel, true);
            File.WriteAllText(fullPath, json);

            Debug.Log($"关卡已保存: {fullPath}");
        }

        public void LoadLevel(int levelId)
        {
            string path = Path.Combine(Application.dataPath, "script/Levels");
            string fileName = $"Level2D_{levelId}.json";
            string fullPath = Path.Combine(path, fileName);

            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                currentLevel = JsonUtility.FromJson<LevelData2D>(json);
                
                // 更新编辑器设置
                gridSize = currentLevel.gridSize;
                cardSpacing = currentLevel.cardSpacing;
                cardSize = currentLevel.cardSize;
                totalLayers = currentLevel.totalLayers;
                irregularShapes = new List<IrregularShapeData>(currentLevel.irregularShapes);
                useCustomAreaSize = currentLevel.useCustomAreaSize;
                areaSize = currentLevel.areaSize;
                enableLayerPreview = currentLevel.enableLayerPreview;
                normalLayerColor = currentLevel.normalLayerColor;
                grayedLayerColor = currentLevel.grayedLayerColor;
                debugMode = currentLevel.debugMode;
                highPerformanceMode = currentLevel.highPerformance;
                inputThrottle = currentLevel.inputThrottle;
                showGrid = currentLevel.showGrid;
                
                UpdateCardDisplay();
                UpdatePlaceableAreaTexture();
                Debug.Log($"关卡已加载: {fullPath}");
            }
            else
            {
                Debug.LogWarning($"关卡文件不存在: {fullPath}");
            }
        }

        public void ExportLevel()
        {
            string path = Path.Combine(Application.dataPath, "script/Levels");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // 导出为JSON
            string jsonFileName = $"Level2D_{GetNextLevelId()}.json";
            string jsonPath = Path.Combine(path, jsonFileName);
            string json = JsonUtility.ToJson(currentLevel, true);
            File.WriteAllText(jsonPath, json);

            // 导出为XML
            string xmlFileName = $"Level2D_{GetNextLevelId()}.xml";
            string xmlPath = Path.Combine(path, xmlFileName);
            ExportToXML(xmlPath);

            Debug.Log($"关卡已导出: {jsonPath}, {xmlPath}");
        }

        private void ExportToXML(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(LevelData2D));
            using (StreamWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, currentLevel);
            }
        }

        public void ValidateCurrentLevel()
        {
            if (currentLevel == null)
            {
                Debug.LogWarning("没有当前关卡可以验证");
                return;
            }

            LevelValidator2D.ValidationResult result = LevelValidator2D.ValidateLevel(currentLevel);
            string report = LevelValidator2D.GetValidationReport(result);
            Debug.Log(report);
        }

        public Vector2 GetActualAreaSize()
        {
            if (useCustomAreaSize)
            {
                return areaSize;
            }
            else
            {
                return new Vector2(
                    gridSize.x * cardSpacing,
                    gridSize.y * cardSpacing
                );
            }
        }

        public void AddCardData(CardData2D cardData)
        {
            currentLevel.cards.Add(cardData);
            CreateCardObject2D(cardData);
            UpdatePlaceableAreaTexture();
        }

        private int GetNextLevelId()
        {
            string path = Path.Combine(Application.dataPath, "script/Levels");
            if (!Directory.Exists(path))
            {
                return 1;
            }

            int maxId = 0;
            string[] files = Directory.GetFiles(path, "Level2D_*.json");
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (fileName.StartsWith("Level2D_"))
                {
                    string idStr = fileName.Substring(8);
                    if (int.TryParse(idStr, out int id))
                    {
                        maxId = Mathf.Max(maxId, id);
                    }
                }
            }
            return maxId + 1;
        }

        private void DrawGridGizmos()
        {
            Gizmos.color = gridLineColor;
            Vector2 actualSize = GetActualAreaSize();

            // 绘制网格线
            for (int x = 0; x <= gridSize.x; x++)
            {
                float xPos = (x - gridSize.x * 0.5f) * cardSpacing;
                Gizmos.DrawLine(
                    new Vector3(xPos, -actualSize.y * 0.5f, 0),
                    new Vector3(xPos, actualSize.y * 0.5f, 0)
                );
            }

            for (int y = 0; y <= gridSize.y; y++)
            {
                float yPos = (y - gridSize.y * 0.5f) * cardSpacing;
                Gizmos.DrawLine(
                    new Vector3(-actualSize.x * 0.5f, yPos, 0),
                    new Vector3(actualSize.x * 0.5f, yPos, 0)
                );
            }

            // 绘制边框
            Gizmos.color = borderColor;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(actualSize.x, actualSize.y, 0.1f));

            // 绘制不规则图形
            if (showIrregularShapes)
            {
                foreach (var shape in irregularShapes)
                {
                    if (shape.isActive && shape.vertices.Count >= 3)
                    {
                        Gizmos.color = new Color(shape.shapeColor.r, shape.shapeColor.g, shape.shapeColor.b, irregularShapeAlpha);
                        
                        for (int i = 0; i < shape.vertices.Count; i++)
                        {
                            Vector3 start = new Vector3(shape.vertices[i].x, shape.vertices[i].y, 0);
                            Vector3 end = new Vector3(shape.vertices[(i + 1) % shape.vertices.Count].x, shape.vertices[(i + 1) % shape.vertices.Count].y, 0);
                            Gizmos.DrawLine(start, end);
                        }
                    }
                }
            }
        }

        private void DrawDebugInfo()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.BeginVertical("box");

            GUILayout.Label($"拔了个罐2D关卡编辑器 - 调试信息");
            GUILayout.Label($"当前层级: {currentLayer}");
            GUILayout.Label($"卡片数量: {currentLevel.cards.Count}");
            GUILayout.Label($"不规则图形数量: {irregularShapes.Count}");
            GUILayout.Label($"鼠标位置: {lastMousePosition}");
            GUILayout.Label($"相机位置: {editorCamera.transform.position}");
            GUILayout.Label($"相机大小: {editorCamera.orthographicSize}");

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        void OnDestroy()
        {
            if (placeableAreaTexture != null)
            {
                DestroyImmediate(placeableAreaTexture);
            }
        }
    }

    public class CardObject2D : MonoBehaviour
    {
        public int cardId;
        public int cardType;
        public int layer;
        public float baseCardSize = 0.8f;
        private Color originalColor;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }

        void OnMouseEnter()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.yellow;
                transform.localScale = Vector3.one * baseCardSize * 1.2f;
            }
        }

        void OnMouseExit()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
                transform.localScale = Vector3.one * baseCardSize;
            }
        }
    }
} 