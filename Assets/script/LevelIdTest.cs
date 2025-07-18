using UnityEngine;
using System.IO;

public class LevelIdTest : MonoBehaviour
{
    [Header("测试设置")]
    public bool runTestOnStart = true;
    public bool createTestLevels = true;
    
    private SheepLevelEditor2D editor;
    
    void Start()
    {
        if (runTestOnStart)
        {
            RunLevelIdTest();
        }
    }
    
    [ContextMenu("运行关卡ID测试")]
    public void RunLevelIdTest()
    {
        Debug.Log("=== 开始关卡ID自增测试 ===");
        
        // 查找编辑器
        editor = FindObjectOfType<SheepLevelEditor2D>();
        if (editor == null)
        {
            Debug.LogError("❌ 未找到SheepLevelEditor2D组件！");
            return;
        }
        
        Debug.Log("✅ 找到编辑器组件");
        
        // 测试1: 检查当前关卡ID
        TestCurrentLevelId();
        
        // 测试2: 测试关卡ID自增
        TestLevelIdIncrement();
        
        // 测试3: 测试文件系统扫描
        TestFileSystemScan();
        
        Debug.Log("=== 关卡ID自增测试完成 ===");
    }
    
    void TestCurrentLevelId()
    {
        Debug.Log("--- 测试当前关卡ID ---");
        Debug.Log($"当前关卡ID: {editor.currentLevelId}");
        Debug.Log($"当前关卡名称: {editor.currentLevelName}");
        
        // 测试获取下一个关卡ID
        int nextLevelId = GetNextLevelId();
        Debug.Log($"下一个可用关卡ID: {nextLevelId}");
        
        if (nextLevelId > editor.currentLevelId)
        {
            Debug.Log("✅ 关卡ID自增逻辑正常");
        }
        else
        {
            Debug.LogWarning("⚠️ 关卡ID自增逻辑可能有问题");
        }
    }
    
    void TestLevelIdIncrement()
    {
        Debug.Log("--- 测试关卡ID自增 ---");
        
        int originalLevelId = editor.currentLevelId;
        Debug.Log($"原始关卡ID: {originalLevelId}");
        
        // 保存当前关卡
        editor.SaveLevel();
        Debug.Log("✅ 保存当前关卡");
        
        // 创建新关卡
        editor.NewLevel();
        Debug.Log($"新建关卡ID: {editor.currentLevelId}");
        
        if (editor.currentLevelId > originalLevelId)
        {
            Debug.Log("✅ 关卡ID自增成功");
        }
        else
        {
            Debug.LogError("❌ 关卡ID自增失败");
        }
        
        // 再创建几个关卡测试
        for (int i = 0; i < 3; i++)
        {
            int beforeId = editor.currentLevelId;
            editor.NewLevel();
            Debug.Log($"第{i+1}次新建关卡: {beforeId} -> {editor.currentLevelId}");
            
            if (editor.currentLevelId <= beforeId)
            {
                Debug.LogError($"❌ 第{i+1}次关卡ID自增失败");
            }
        }
    }
    
    void TestFileSystemScan()
    {
        Debug.Log("--- 测试文件系统扫描 ---");
        
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        Debug.Log($"关卡目录: {levelsPath}");
        
        if (Directory.Exists(levelsPath))
        {
            string[] levelFiles = Directory.GetFiles(levelsPath, "Level2D_*.json");
            Debug.Log($"找到关卡文件数量: {levelFiles.Length}");
            
            int maxLevelId = 0;
            foreach (string file in levelFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                Debug.Log($"关卡文件: {fileName}");
                
                if (fileName.StartsWith("Level2D_"))
                {
                    string idStr = fileName.Substring("Level2D_".Length);
                    if (int.TryParse(idStr, out int levelId))
                    {
                        maxLevelId = Mathf.Max(maxLevelId, levelId);
                        Debug.Log($"  解析关卡ID: {levelId}");
                    }
                }
            }
            
            Debug.Log($"最大关卡ID: {maxLevelId}");
            int nextId = maxLevelId + 1;
            Debug.Log($"下一个可用关卡ID: {nextId}");
        }
        else
        {
            Debug.Log("⚠️ 关卡目录不存在，将创建新目录");
        }
    }
    
    int GetNextLevelId()
    {
        int maxLevelId = 0;
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        
        if (Directory.Exists(levelsPath))
        {
            string[] levelFiles = Directory.GetFiles(levelsPath, "Level2D_*.json");
            
            foreach (string file in levelFiles)
            {
                try
                {
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
    
    [ContextMenu("创建测试关卡文件")]
    public void CreateTestLevelFiles()
    {
        Debug.Log("--- 创建测试关卡文件 ---");
        
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        Directory.CreateDirectory(levelsPath);
        
        // 创建一些测试关卡文件
        for (int i = 1; i <= 5; i++)
        {
            string filePath = Path.Combine(levelsPath, $"Level2D_{i}.json");
            string testContent = $"{{\"levelId\":{i},\"levelName\":\"TestLevel_{i}\"}}";
            
            File.WriteAllText(filePath, testContent);
            Debug.Log($"创建测试关卡文件: {filePath}");
        }
        
        Debug.Log("✅ 测试关卡文件创建完成");
    }
    
    [ContextMenu("清理测试关卡文件")]
    public void CleanupTestLevelFiles()
    {
        Debug.Log("--- 清理测试关卡文件 ---");
        
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        
        if (Directory.Exists(levelsPath))
        {
            string[] testFiles = Directory.GetFiles(levelsPath, "Level2D_*.json");
            
            foreach (string file in testFiles)
            {
                try
                {
                    File.Delete(file);
                    Debug.Log($"删除测试文件: {file}");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"删除文件失败: {file}, 错误: {e.Message}");
                }
            }
        }
        
        Debug.Log("✅ 测试关卡文件清理完成");
    }
    
    [ContextMenu("显示所有关卡文件")]
    public void ShowAllLevelFiles()
    {
        Debug.Log("--- 显示所有关卡文件 ---");
        
        string levelsPath = Path.Combine(Application.dataPath, "Levels");
        
        if (Directory.Exists(levelsPath))
        {
            string[] levelFiles = Directory.GetFiles(levelsPath, "Level2D_*.json");
            
            if (levelFiles.Length == 0)
            {
                Debug.Log("没有找到关卡文件");
            }
            else
            {
                Debug.Log($"找到 {levelFiles.Length} 个关卡文件:");
                foreach (string file in levelFiles)
                {
                    string fileName = Path.GetFileName(file);
                    Debug.Log($"  {fileName}");
                }
            }
        }
        else
        {
            Debug.Log("关卡目录不存在");
        }
    }
    
    [ContextMenu("测试新建关卡")]
    public void TestNewLevel()
    {
        if (editor == null)
        {
            editor = FindObjectOfType<SheepLevelEditor2D>();
        }
        
        if (editor != null)
        {
            int beforeId = editor.currentLevelId;
            Debug.Log($"新建关卡前ID: {beforeId}");
            
            editor.NewLevel();
            
            Debug.Log($"新建关卡后ID: {editor.currentLevelId}");
            Debug.Log($"新建关卡名称: {editor.currentLevelName}");
            
            if (editor.currentLevelId > beforeId)
            {
                Debug.Log("✅ 关卡ID自增成功");
            }
            else
            {
                Debug.LogError("❌ 关卡ID自增失败");
            }
        }
        else
        {
            Debug.LogError("❌ 未找到编辑器组件");
        }
    }
} 