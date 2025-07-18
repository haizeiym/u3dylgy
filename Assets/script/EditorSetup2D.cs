using UnityEngine;
using UnityEditor;

public class EditorSetup2D : MonoBehaviour
{
    [MenuItem("羊了个羊2D/快速设置2D场景")]
    public static void Setup2DLevelEditorScene()
    {
        // 创建GUI事件管理器
        if (GUIEventManager.Instance == null)
        {
            GameObject guiManager = new GameObject("GUIEventManager");
            guiManager.AddComponent<GUIEventManager>();
            Debug.Log("✅ 创建GUI事件管理器");
        }
        
        // 创建主编辑器对象
        GameObject levelEditor = GameObject.Find("LevelEditor2D");
        if (levelEditor == null)
        {
            levelEditor = new GameObject("LevelEditor2D");
            levelEditor.AddComponent<SheepLevelEditor2D>();
            Debug.Log("✅ 创建LevelEditor2D对象");
        }
        
        // 设置2D相机
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";
            Debug.Log("✅ 创建Main Camera");
        }
        
        // 设置为2D相机
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // 创建材质文件夹
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
            Debug.Log("✅ 创建Materials文件夹");
        }
        
        // 创建精灵文件夹
        if (!AssetDatabase.IsValidFolder("Assets/Sprites"))
        {
            AssetDatabase.CreateFolder("Assets", "Sprites");
            Debug.Log("✅ 创建Sprites文件夹");
        }
        
        // 创建默认卡片精灵
        CreateDefaultCardSprites();
        
        // 创建Levels文件夹
        if (!AssetDatabase.IsValidFolder("Assets/Levels"))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
            Debug.Log("✅ 创建Levels文件夹");
        }
        
        // 选择LevelEditor对象
        Selection.activeGameObject = levelEditor;
        
        Debug.Log("🎉 2D场景设置完成！请检查Inspector中的参数设置。");
    }
    
    [MenuItem("羊了个羊2D/创建默认卡片精灵")]
    public static void CreateDefaultCardSprites()
    {
        string spritesPath = "Assets/Sprites/";
        
        // 定义精灵颜色
        Color[] colors = {
            Color.red,      // 类型0
            Color.blue,     // 类型1
            Color.green,    // 类型2
            Color.yellow,   // 类型3
            Color.magenta,  // 类型4
            new Color(1f, 0.5f, 0f), // 橙色 类型5
            Color.cyan,     // 类型6
            new Color(1f, 0.75f, 0.8f) // 粉色 类型7
        };
        
        string[] spriteNames = {
            "CardType_0", "CardType_1", "CardType_2", "CardType_3",
            "CardType_4", "CardType_5", "CardType_6", "CardType_7"
        };
        
        for (int i = 0; i < 8; i++)
        {
            string spritePath = spritesPath + spriteNames[i] + ".png";
            
            // 检查精灵是否已存在
            Sprite existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (existingSprite == null)
            {
                // 创建新精灵
                Sprite newSprite = CreateCardSprite(colors[i], spriteNames[i]);
                
                // 保存为PNG文件
                byte[] pngData = newSprite.texture.EncodeToPNG();
                System.IO.File.WriteAllBytes(spritePath, pngData);
                
                AssetDatabase.Refresh();
                
                // 设置精灵导入设置
                TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spritePixelsPerUnit = 32;
                    importer.filterMode = FilterMode.Bilinear;
                    importer.SaveAndReimport();
                }
                
                Debug.Log($"✅ 创建精灵: {spriteNames[i]}");
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private static Sprite CreateCardSprite(Color color, string name)
    {
        // 创建卡片精灵纹理
        int textureSize = 64;
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // 设置纹理格式
        texture.filterMode = FilterMode.Bilinear;
        
        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                if (x == 0 || x == textureSize - 1 || y == 0 || y == textureSize - 1)
                {
                    texture.SetPixel(x, y, Color.black); // 边框
                }
                else if (x < 4 || x > textureSize - 5 || y < 4 || y > textureSize - 5)
                {
                    texture.SetPixel(x, y, Color.Lerp(color, Color.white, 0.3f)); // 高光
                }
                else
                {
                    texture.SetPixel(x, y, color); // 主色
                }
            }
        }
        
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f), 32);
    }
    
    [MenuItem("羊了个羊2D/设置2D编辑器参数")]
    public static void Setup2DEditorParameters()
    {
        GameObject levelEditor = GameObject.Find("LevelEditor2D");
        if (levelEditor == null)
        {
            Debug.LogError("❌ 找不到LevelEditor2D对象，请先运行'快速设置2D场景'");
            return;
        }
        
        SheepLevelEditor2D editor = levelEditor.GetComponent<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.LogError("❌ LevelEditor2D对象上没有SheepLevelEditor2D组件");
            return;
        }
        
        // 设置默认参数
        editor.currentLevelId = 1;
        editor.currentLevelName = "Level2D_1";
        editor.totalLayers = 3;
        editor.gridSize = new Vector2(8, 8);
        editor.cardSpacing = 1.2f;
        editor.cardSize = 0.8f;
        editor.currentCardType = 0;
        editor.maxCardTypes = 8;
        editor.isEditMode = true;
        editor.showGrid = true;
        editor.selectedLayer = 0;
        editor.cameraZoom = 5f;
        editor.cameraPosition = Vector2.zero;
        
        // 加载精灵
        LoadCardSprites(editor);
        
        Debug.Log("✅ 2D编辑器参数设置完成");
    }
    
    private static void LoadCardSprites(SheepLevelEditor2D editor)
    {
        Sprite[] sprites = new Sprite[8];
        for (int i = 0; i < 8; i++)
        {
            string spritePath = $"Assets/Sprites/CardType_{i}.png";
            sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }
        
        editor.cardSprites = sprites;
        Debug.Log("✅ 卡片精灵加载完成");
    }
    
    [MenuItem("羊了个羊2D/创建2D示例关卡")]
    public static void Create2DExampleLevel()
    {
        // 确保Levels文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Levels"))
        {
            AssetDatabase.CreateFolder("Assets", "Levels");
        }
        
        // 创建示例关卡文件
        string exampleLevelPath = "Assets/Levels/Level2D_1_Example.json";
        
        // 检查是否已存在
        if (System.IO.File.Exists(exampleLevelPath))
        {
            Debug.Log("✅ 2D示例关卡文件已存在");
            return;
        }
        
        // 创建简单的2D示例关卡
        string simple2DLevel = @"{
  ""levelName"": ""Simple2D_Level"",
  ""levelId"": 1,
  ""totalLayers"": 2,
  ""gridSize"": {""x"": 6, ""y"": 6},
  ""cardSpacing"": 1.2,
  ""cards"": [
    {""id"": 1, ""type"": 0, ""position"": {""x"": -1.2, ""y"": 1.2}, ""layer"": 1, ""isVisible"": true, ""blockingCards"": []},
    {""id"": 2, ""type"": 0, ""position"": {""x"": 0.0, ""y"": 1.2}, ""layer"": 1, ""isVisible"": true, ""blockingCards"": []},
    {""id"": 3, ""type"": 0, ""position"": {""x"": 1.2, ""y"": 1.2}, ""layer"": 1, ""isVisible"": true, ""blockingCards"": []},
    {""id"": 4, ""type"": 1, ""position"": {""x"": -1.2, ""y"": 0.0}, ""layer"": 0, ""isVisible"": true, ""blockingCards"": [1]},
    {""id"": 5, ""type"": 1, ""position"": {""x"": 0.0, ""y"": 0.0}, ""layer"": 0, ""isVisible"": true, ""blockingCards"": [2]},
    {""id"": 6, ""type"": 1, ""position"": {""x"": 1.2, ""y"": 0.0}, ""layer"": 0, ""isVisible"": true, ""blockingCards"": [3]}
  ]
}";
        
        System.IO.File.WriteAllText(exampleLevelPath, simple2DLevel);
        AssetDatabase.Refresh();
        Debug.Log("✅ 创建2D简单示例关卡");
    }
    
    [MenuItem("羊了个羊2D/完整2D设置向导")]
    public static void Complete2DSetupWizard()
    {
        Debug.Log("🚀 开始羊了个羊2D关卡编辑器完整设置...");
        
        Setup2DLevelEditorScene();
        CreateDefaultCardSprites();
        Setup2DEditorParameters();
        Create2DExampleLevel();
        
        Debug.Log("🎉 2D完整设置完成！现在可以开始使用2D关卡编辑器了。");
        Debug.Log("📖 请查看Assets/script/README2D.md了解详细使用方法。");
    }
    
    [MenuItem("羊了个羊2D/创建2D卡片预制体")]
    public static void Create2DCardPrefab()
    {
        // 创建2D卡片预制体
        GameObject cardPrefab = new GameObject("CardPrefab2D");
        
        // 添加SpriteRenderer
        SpriteRenderer spriteRenderer = cardPrefab.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CreateCardSprite(Color.white, "DefaultCard");
        spriteRenderer.sortingOrder = 0;
        
        // 添加BoxCollider2D
        BoxCollider2D collider = cardPrefab.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one * 0.8f;
        
        // 添加CardObject2D组件
        cardPrefab.AddComponent<CardObject2D>();
        
        // 保存为预制体
        string prefabPath = "Assets/Prefabs/CardPrefab2D.prefab";
        
        // 确保Prefabs文件夹存在
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // 创建预制体
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cardPrefab, prefabPath);
        DestroyImmediate(cardPrefab);
        
        Debug.Log($"✅ 创建2D卡片预制体: {prefabPath}");
        Debug.Log("ℹ️ 注意: 2D编辑器现在使用动态创建卡片，不再需要预制体");
    }
} 