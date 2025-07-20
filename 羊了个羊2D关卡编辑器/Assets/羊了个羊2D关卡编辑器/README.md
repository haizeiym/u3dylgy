# 羊了个羊2D关卡编辑器

一个功能完整的Unity编辑器插件，专门用于创建和编辑羊了个羊2D游戏关卡。

## 🎮 功能特性

### 核心功能
- **2D关卡编辑**: 完整的2D关卡编辑系统
- **网格系统**: 可配置的网格大小和间距
- **卡片管理**: 支持多种卡片类型和层级
- **可视化预览**: 实时预览可放置区域和网格
- **层级系统**: 多层卡片管理，支持层级预览
- **导入导出**: 支持JSON、XML、二进制格式

### 编辑器功能
- **直观的GUI界面**: 友好的编辑器界面
- **实时调试**: 内置调试工具和可视化
- **自动保存**: 支持自动保存和备份
- **性能优化**: 高性能模式和输入节流

### 高级功能
- **自定义区域**: 可配置的可放置区域大小
- **纹理质量**: 可调节的纹理大小设置
- **相机控制**: 2D相机缩放和平移
- **网格标签**: 支持网格编号和坐标显示

## 📦 安装说明

### 方法1: Unity Package Manager
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击 `+` 按钮，选择 `Add package from disk`
4. 选择插件的 `package.json` 文件

### 方法2: 直接导入
1. 将插件文件夹复制到项目的 `Assets` 目录
2. Unity会自动导入所有资源

## 🚀 快速开始

### 1. 设置场景
```csharp
// 在场景中创建一个空GameObject
// 添加 SheepLevelEditor2D 组件
```

### 2. 基本配置
- 设置网格大小 (Grid Size)
- 配置卡片间距 (Card Spacing)
- 调整卡片大小 (Card Size)

### 3. 开始编辑
- 使用菜单 `羊了个羊 > 新建关卡` 创建新关卡
- 在场景中点击放置卡片
- 使用 `羊了个羊 > 保存关卡` 保存工作

## 🎯 使用指南

### 基本操作
- **左键点击**: 放置卡片
- **右键点击**: 删除卡片
- **鼠标滚轮**: 缩放视图
- **拖拽**: 移动相机

### 菜单功能
- **新建关卡**: 创建新的空白关卡
- **保存关卡**: 保存当前关卡
- **加载关卡**: 加载已保存的关卡
- **导出关卡**: 导出为不同格式
- **验证关卡**: 检查关卡有效性

### 高级设置
- **层级预览**: 显示所有层级的卡片
- **网格显示**: 显示/隐藏网格线
- **调试模式**: 启用调试信息
- **性能模式**: 优化性能设置

## 📁 文件结构

```
羊了个羊2D关卡编辑器/
├── Runtime/                    # 运行时脚本
│   ├── SheepLevelEditor2D.cs   # 主编辑器
│   ├── PlaceableAreaVisualizer.cs # 区域可视化
│   ├── GUIEventManager.cs      # GUI事件管理
│   └── 羊了个羊2D关卡编辑器.asmdef # 运行时程序集定义
├── Editor/                     # 编辑器脚本
│   ├── SheepLevelEditor2DMenu.cs # 菜单系统
│   └── 羊了个羊2D关卡编辑器.Editor.asmdef # 编辑器程序集定义
├── Sprites/                    # 精灵图片
├── Materials/                  # 材质文件
├── Prefabs/                    # 预制体
├── Levels/                     # 示例关卡
└── Samples~/                   # 示例代码
```

## ⚙️ 配置选项

### 编辑器设置
- **网格大小**: 设置关卡网格的宽度和高度
- **卡片间距**: 卡片之间的间距
- **卡片大小**: 单个卡片的大小
- **层级数量**: 关卡的总层级数

### 可视化设置
- **可放置区域颜色**: 自定义区域显示颜色
- **网格线颜色**: 自定义网格线颜色
- **边框颜色**: 自定义边框颜色
- **纹理大小**: 影响可视化质量

### 高级设置
- **相机速度**: 相机移动速度
- **缩放速度**: 相机缩放速度
- **输入节流**: 输入处理频率
- **自动保存**: 是否启用自动保存

## 🔧 API参考

### 主要类

#### SheepLevelEditor2D
主编辑器类，提供关卡编辑的核心功能。

```csharp
// 创建新关卡
public void NewLevel()

// 保存关卡
public void SaveLevel()

// 加载关卡
public void LoadLevel(int levelId)

// 导出关卡
public void ExportLevel()

// 获取实际区域大小
public Vector2 GetActualAreaSize()

// 网格对齐
public Vector2 SnapToGrid2D(Vector2 worldPos)
```

#### PlaceableAreaVisualizer
可放置区域可视化组件。

```csharp
// 设置可见性
public void SetVisible(bool visible)

// 更新颜色
public void UpdateColors(Color areaColor, Color borderColor, Color gridColor)
```

#### CardObject2D
卡片对象组件。

```csharp
public int cardId;      // 卡片ID
public int cardType;    // 卡片类型
public int layer;       // 卡片层级
```

## 📝 示例代码

### 创建自定义关卡
```csharp
using UnityEngine;

public class CustomLevelCreator : MonoBehaviour
{
    public SheepLevelEditor2D levelEditor;
    
    void Start()
    {
        // 创建新关卡
        levelEditor.NewLevel();
        
        // 设置网格大小
        levelEditor.gridSize = new Vector2(10, 10);
        
        // 添加卡片
        CardData2D card = new CardData2D
        {
            id = 1,
            type = 0,
            position = new Vector2(0, 0),
            layer = 0
        };
        
        levelEditor.AddCardData(card);
        
        // 保存关卡
        levelEditor.SaveLevel();
    }
}
```

### 自定义可视化
```csharp
using UnityEngine;

public class CustomVisualizer : MonoBehaviour
{
    public PlaceableAreaVisualizer visualizer;
    
    void Start()
    {
        // 设置自定义颜色
        visualizer.UpdateColors(
            Color.blue,    // 区域颜色
            Color.red,     // 边框颜色
            Color.yellow   // 网格颜色
        );
    }
}
```

## 🐛 故障排除

### 常见问题

**Q: 编辑器菜单不显示**
A: 确保 `SheepLevelEditor2DMenu.cs` 在 `Editor` 文件夹中

**Q: 可放置区域不显示**
A: 检查 `PlaceableAreaVisualizer` 组件是否正确配置

**Q: 卡片无法放置**
A: 确认鼠标不在GUI区域，检查网格边界设置

**Q: 性能问题**
A: 启用高性能模式，调整纹理大小设置

### 调试模式
启用调试模式可以查看详细的日志信息：
1. 在 `SheepLevelEditor2D` 组件中启用 `Debug Mode`
2. 查看Console窗口的日志输出

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📞 支持

- 邮箱: support@yanglegeyang2d.com
- GitHub Issues: [提交问题](https://github.com/yanglegeyang2d/leveleditor/issues)
- 文档: [完整文档](https://github.com/yanglegeyang2d/leveleditor/wiki)

## 📈 更新日志

### v1.0.0
- 初始版本发布
- 完整的2D关卡编辑功能
- 可视化预览系统
- 多格式导入导出
- 层级管理系统

---

**羊了个羊2D关卡编辑器** - 让关卡创作变得简单高效！ 🎮✨ 