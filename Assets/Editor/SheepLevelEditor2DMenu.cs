using UnityEngine;
using UnityEditor;

public class SheepLevelEditor2DMenu
{
    // 统一的一级菜单：羊了个羊
    private const string MENU_ROOT = "羊了个羊/";
    
    // 编辑器核心功能
    [MenuItem(MENU_ROOT + "创建2D关卡编辑器", false, 1)]
    public static void Create2DLevelEditor()
    {
        // 查找现有的编辑器
        SheepLevelEditor2D existingEditor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (existingEditor != null)
        {
            Selection.activeGameObject = existingEditor.gameObject;
            EditorGUIUtility.PingObject(existingEditor.gameObject);
            Debug.Log("已找到现有的2D关卡编辑器，已选中");
            return;
        }
        
        // 创建新的编辑器对象
        GameObject editorObj = new GameObject("羊了个羊2D关卡编辑器");
        SheepLevelEditor2D editor = editorObj.AddComponent<SheepLevelEditor2D>();
        
        // 设置默认参数
        SetupDefaultParameters(editor);
        
        // 选中新创建的对象
        Selection.activeGameObject = editorObj;
        EditorGUIUtility.PingObject(editorObj);
        
        Debug.Log("已创建羊了个羊2D关卡编辑器");
    }
    
    [MenuItem(MENU_ROOT + "快速设置2D场景", false, 2)]
    public static void QuickSetup2D()
    {
        // 查找现有的编辑器
        SheepLevelEditor2D existingEditor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (existingEditor == null)
        {
            Create2DLevelEditor();
            existingEditor = Object.FindObjectOfType<SheepLevelEditor2D>();
        }
        
        // 应用快速设置
        SetupDefaultParameters(existingEditor);
        
        Debug.Log("已应用快速设置");
    }
    
    [MenuItem(MENU_ROOT + "完整2D设置向导", false, 3)]
    public static void Complete2DSetupWizard()
    {
        // 创建编辑器
        Create2DLevelEditor();
        
        // 设置2D场景
        Setup2DScene();
        
        // 创建默认卡片精灵
        CreateDefaultCardSprites();
        
        // 创建示例关卡
        Create2DExampleLevel();
        
        Debug.Log("完整2D设置向导已完成！");
    }
    
    // 关卡管理功能
    [MenuItem(MENU_ROOT + "关卡管理/新建关卡", false, 10)]
    public static void NewLevel()
    {
        SheepLevelEditor2D editor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (editor != null)
        {
            editor.NewLevel();
            Debug.Log("已创建新关卡");
        }
        else
        {
            Debug.LogWarning("未找到2D关卡编辑器，请先创建编辑器");
        }
    }
    
    [MenuItem(MENU_ROOT + "关卡管理/保存关卡", false, 11)]
    public static void SaveCurrentLevel()
    {
        SheepLevelEditor2D editor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (editor != null)
        {
            editor.SaveLevel();
            Debug.Log("已保存当前关卡");
        }
        else
        {
            Debug.LogWarning("未找到2D关卡编辑器，请先创建编辑器");
        }
    }
    
    [MenuItem(MENU_ROOT + "关卡管理/导出关卡", false, 12)]
    public static void ExportCurrentLevel()
    {
        SheepLevelEditor2D editor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (editor != null)
        {
            editor.ExportLevel();
            Debug.Log("已导出当前关卡");
        }
        else
        {
            Debug.LogWarning("未找到2D关卡编辑器，请先创建编辑器");
        }
    }
    
    [MenuItem(MENU_ROOT + "关卡管理/验证关卡", false, 13)]
    public static void ValidateCurrentLevel()
    {
        SheepLevelEditor2D editor = Object.FindObjectOfType<SheepLevelEditor2D>();
        if (editor != null)
        {
            editor.ValidateCurrentLevel();
        }
        else
        {
            Debug.LogWarning("未找到2D关卡编辑器，请先创建编辑器");
        }
    }
    
    // 资源创建功能
    [MenuItem(MENU_ROOT + "资源创建/创建默认卡片精灵", false, 20)]
    public static void CreateDefaultCardSprites()
    {
        // 创建Sprites文件夹
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
        }
        
        // 创建8种颜色的卡片精灵
        Color[] colors = {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.cyan, Color.magenta, Color.white, Color.gray
        };
        
        for (int i = 0; i < colors.Length; i++)
        {
            string spritePath = $"Assets/Sprites/CardType_{i}.png";
            if (!System.IO.File.Exists(spritePath))
            {
                CreateColoredSprite(spritePath, colors[i], $"CardType_{i}");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log("已创建默认卡片精灵");
    }
    
    [MenuItem(MENU_ROOT + "资源创建/创建2D示例关卡", false, 21)]
    public static void Create2DExampleLevel()
    {
        // 创建Levels文件夹
        if (!AssetDatabase.IsValidFolder("Assets/Levels"))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
        }
        
        // 创建示例关卡数据
        var levelData = new LevelData2D
        {
            levelId = 1,
            levelName = "Level2D_1_Example",
            totalLayers = 3,
            gridSize = new Vector2(8, 8),
            cardSpacing = 1.2f,
            cardSize = 0.8f,
            cards = new System.Collections.Generic.List<CardData2D>()
        };
        
        // 添加一些示例卡片
        for (int layer = 0; layer < 3; layer++)
        {
            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    levelData.cards.Add(new CardData2D
                    {
                        id = levelData.cards.Count,
                        type = (x + y) % 8,
                        position = new Vector2(x * 1.2f - 1.2f, y * 1.2f - 1.2f),
                        layer = layer,
                        isVisible = true,
                        blockingCards = new System.Collections.Generic.List<int>()
                    });
                }
            }
        }
        
        // 保存示例关卡
        string json = JsonUtility.ToJson(levelData, true);
        string filePath = "Assets/Levels/Level2D_1_Example.json";
        System.IO.File.WriteAllText(filePath, json);
        
        AssetDatabase.Refresh();
        Debug.Log("已创建2D示例关卡");
    }
    
    // 验证菜单项是否可用
    [MenuItem(MENU_ROOT + "关卡管理/新建关卡", true)]
    [MenuItem(MENU_ROOT + "关卡管理/保存关卡", true)]
    [MenuItem(MENU_ROOT + "关卡管理/导出关卡", true)]
    [MenuItem(MENU_ROOT + "关卡管理/验证关卡", true)]
    public static bool ValidateEditorExists()
    {
        return Object.FindObjectOfType<SheepLevelEditor2D>() != null;
    }
    
    // 关于菜单项
    [MenuItem(MENU_ROOT + "关于/版本信息", false, 30)]
    public static void ShowVersionInfo()
    {
        EditorUtility.DisplayDialog("羊了个羊关卡编辑器", 
            "版本: 2.0.0\n" +
            "作者: AI Assistant\n" +
            "功能: 2D关卡编辑器\n" +
            "支持: 多层关卡、卡片编辑、导出功能", 
            "确定");
    }
    
    // 辅助方法：设置默认参数
    private static void SetupDefaultParameters(SheepLevelEditor2D editor)
    {
        if (editor == null) return;
        
        // 基本设置
        editor.currentLevelId = 1;
        editor.currentLevelName = "Level2D_1";
        editor.totalLayers = 3;
        editor.gridSize = new Vector2(8, 8);
        editor.cardSpacing = 1.2f;
        editor.cardSize = 0.8f;
        editor.selectedLayer = 0;
        editor.currentCardType = 0;
        editor.cameraZoom = 5f;
        
        // 显示设置
        editor.showLayerMasks = true;
        editor.showPlaceableArea = true;
        editor.useCustomAreaSize = false;
        editor.areaSize = new Vector2(8.4f, 8.4f);
        editor.enableLayerPreview = true;
        editor.normalLayerColor = Color.white;
        editor.grayedLayerColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        // 高级设置
        editor.debugMode = false;
        editor.highPerformance = true;
        editor.showFPS = false;
        editor.showMemory = false;
        editor.cameraSpeed = 1f;
        editor.zoomSpeed = 1f;
        editor.inputThrottle = 0.1f;
        editor.showGrid = true;
        editor.showGridNumbers = false;
        editor.showGridCoordinates = false;
        editor.gridLineWidth = 0.02f;
        editor.showCardIDs = false;
        editor.showCardTypes = false;
        editor.cardHoverScale = 1.2f;
        editor.autoSave = true;
        editor.backupLevels = true;
        editor.exportJSON = true;
        editor.exportXML = false;
        editor.exportBinary = false;
    }
    
    // 辅助方法：设置2D场景
    private static void Setup2DScene()
    {
        // 设置主相机为2D正交相机
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f;
            mainCamera.transform.position = new Vector3(0, 0, -10);
            Debug.Log("已设置2D正交相机");
        }
        else
        {
            Debug.LogWarning("未找到主相机");
        }
    }
    
    // 辅助方法：创建彩色精灵
    private static void CreateColoredSprite(string path, Color color, string name)
    {
        // 创建32x32的纹理
        int size = 32;
        Texture2D texture = new Texture2D(size, size);
        
        // 填充颜色
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        
        // 保存为PNG
        byte[] pngData = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, pngData);
        
        // 设置纹理导入设置
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.SaveAndReimport();
        }
        
        Object.DestroyImmediate(texture);
    }
} 