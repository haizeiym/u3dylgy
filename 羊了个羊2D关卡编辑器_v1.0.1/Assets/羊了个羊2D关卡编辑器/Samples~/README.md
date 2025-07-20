# 羊了个羊2D关卡编辑器 - 示例

本目录包含了羊了个羊2D关卡编辑器的示例代码和场景，帮助您快速上手使用插件。

## 📁 示例内容

### BasicExample - 基础示例
**路径**: `Samples~/BasicExample/`

基础示例展示了插件的核心功能：
- 基本的关卡编辑设置
- 简单的卡片创建和管理
- 层级预览功能
- 网格显示控制
- 调试模式使用

**包含文件**:
- `BasicLevelEditorExample.cs` - 基础示例脚本

**使用方法**:
1. 将 `BasicLevelEditorExample.cs` 添加到场景中的GameObject
2. 确保场景中有 `SheepLevelEditor2D` 组件
3. 运行场景，点击GUI按钮测试各种功能

### AdvancedExample - 高级示例
**路径**: `Samples~/AdvancedExample/`

高级示例展示了插件的进阶功能：
- 复杂的多层关卡创建
- 自定义区域大小和颜色
- 性能优化设置
- 特殊图案生成
- 批量操作和测试

**包含文件**:
- `AdvancedLevelEditorExample.cs` - 高级示例脚本

**使用方法**:
1. 将 `AdvancedLevelEditorExample.cs` 添加到场景中的GameObject
2. 配置高级设置参数
3. 运行场景，体验高级功能

## 🚀 快速开始

### 1. 导入示例
1. 在Unity中打开Package Manager
2. 找到"羊了个羊2D关卡编辑器"插件
3. 点击"Import"导入示例

### 2. 设置场景
1. 创建新的场景或使用现有场景
2. 添加一个空GameObject
3. 添加 `SheepLevelEditor2D` 组件
4. 添加示例脚本到另一个GameObject

### 3. 运行示例
1. 点击Play按钮
2. 使用GUI界面控制示例功能
3. 观察Console窗口的日志输出

## 📝 示例功能说明

### 基础示例功能
- **设置基础示例**: 自动配置编辑器基本参数
- **创建示例关卡**: 生成包含多层卡片的示例关卡
- **切换层级预览**: 开启/关闭层级预览模式
- **切换网格显示**: 显示/隐藏网格线
- **切换调试模式**: 启用/禁用调试信息

### 高级示例功能
- **设置高级示例**: 配置高级参数和自定义设置
- **创建复杂关卡**: 生成5层复杂关卡结构
- **创建心形图案**: 生成心形图案的关卡
- **性能测试**: 测试大量卡片的创建性能
- **导出测试**: 测试多格式导出功能
- **实时设置调整**: 运行时调整各种参数

## 🔧 自定义示例

您可以根据需要修改示例代码：

### 修改基础设置
```csharp
// 在BasicLevelEditorExample中修改
public Vector2 exampleGridSize = new Vector2(12, 12); // 更大的网格
public float exampleCardSpacing = 1.5f; // 更大的间距
public float exampleCardSize = 1.0f; // 更大的卡片
```

### 修改高级设置
```csharp
// 在AdvancedLevelEditorExample中修改
public Vector2 customAreaSize = new Vector2(16, 16); // 自定义区域大小
public Color customAreaColor = Color.red; // 自定义颜色
public int customTextureSize = 256; // 更高的纹理质量
```

### 创建自定义图案
```csharp
// 在AdvancedLevelEditorExample中添加新方法
void CreateCustomPattern()
{
    // 定义自定义图案的坐标点
    Vector2[] customPoints = {
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0),
        new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1),
        new Vector2(0, 2), new Vector2(1, 2), new Vector2(2, 2)
    };
    
    // 创建卡片
    foreach (var point in customPoints)
    {
        // 创建卡片逻辑
    }
}
```

## 🐛 故障排除

### 常见问题

**Q: 示例脚本无法找到编辑器组件**
A: 确保场景中有 `SheepLevelEditor2D` 组件，并且脚本中的引用正确设置

**Q: GUI按钮没有响应**
A: 检查脚本是否正确添加到GameObject，确保OnGUI方法正常工作

**Q: 性能测试卡顿**
A: 减少测试卡片数量，或启用高性能模式

**Q: 颜色设置不生效**
A: 确保 `PlaceableAreaVisualizer` 组件存在，并调用 `UpdateColors` 方法

### 调试技巧
1. 启用调试模式查看详细日志
2. 使用Console窗口监控性能
3. 检查Inspector面板的参数设置
4. 验证组件引用是否正确

## 📚 学习建议

1. **从基础开始**: 先运行基础示例，理解核心概念
2. **逐步深入**: 再尝试高级示例，学习进阶功能
3. **修改参数**: 尝试修改各种参数，观察效果变化
4. **创建自定义**: 基于示例代码创建自己的功能
5. **查看源码**: 阅读插件源码，理解实现原理

## 🤝 贡献示例

如果您创建了有用的示例，欢迎贡献：

1. 创建新的示例文件夹
2. 添加详细的README说明
3. 包含完整的代码和注释
4. 提供使用说明和截图
5. 提交Pull Request

---

**通过这些示例，您可以快速掌握羊了个羊2D关卡编辑器的使用方法！** 🎮✨ 