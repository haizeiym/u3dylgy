# 羊了个羊2D关卡编辑器 - 导入指南

## 🔧 "Nothing to import" 问题解决方案

如果您在导入 `.unitypackage` 文件时遇到 "Nothing to import" 错误，这是因为Unity Package Manager格式的特殊要求。以下是几种解决方案：

## 🚀 推荐解决方案

### 方法1: 使用ZIP格式 (最推荐)

**步骤**:
1. 下载 `羊了个羊2D关卡编辑器_v1.0.2.zip` 文件
2. 解压ZIP文件
3. 将解压后的 `羊了个羊2D关卡编辑器` 文件夹复制到您的Unity项目的 `Assets` 目录
4. Unity会自动导入所有资源

**优点**:
- ✅ 100% 可靠
- ✅ 简单直接
- ✅ 无格式问题
- ✅ 所有文件都会正确导入

### 方法2: 手动创建Package Manager格式

**步骤**:
1. 在Unity中创建新项目
2. 将ZIP文件解压并复制到 `Assets` 目录
3. 在Project窗口中右键点击插件文件夹
4. 选择 `Export Package...`
5. 确保所有文件都被选中
6. 点击 `Export...` 保存为 `.unitypackage` 文件
7. 在新项目中使用这个导出的文件

### 方法3: 使用Package Manager (需要正确格式)

**步骤**:
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击左上角的 `+` 按钮
4. 选择 `Add package from disk`
5. 选择插件目录中的 `package.json` 文件

## 📦 文件格式说明

### ZIP格式 (推荐)
- **文件名**: `羊了个羊2D关卡编辑器_v1.0.2.zip`
- **大小**: 约 63KB
- **内容**: 完整的插件文件夹
- **导入方式**: 解压到Assets目录

### Unity Package格式
- **文件名**: `羊了个羊2D关卡编辑器_v1.0.2.unitypackage`
- **大小**: 约 62KB
- **内容**: Unity Package Manager格式
- **导入方式**: 双击导入或Package Manager

## 🔍 验证导入成功

导入完成后，请检查以下项目：

### ✅ 编译检查
- [ ] Console窗口没有编译错误
- [ ] 所有脚本都能正常编译
- [ ] 没有 "LevelValidator2D not found" 错误

### ✅ 功能检查
- [ ] Unity菜单栏显示"羊了个羊"菜单
- [ ] 可以在Inspector中添加插件组件
- [ ] 示例代码可以正常运行
- [ ] 所有资源文件正确导入

### ✅ 文件结构检查
```
Assets/
└── 羊了个羊2D关卡编辑器/
    ├── Runtime/
    │   ├── SheepLevelEditor2D.cs
    │   ├── PlaceableAreaVisualizer.cs
    │   ├── GUIEventManager.cs
    │   ├── LevelValidator2D.cs
    │   └── 羊了个羊2D关卡编辑器.asmdef
    ├── Editor/
    │   ├── SheepLevelEditor2DMenu.cs
    │   └── 羊了个羊2D关卡编辑器.Editor.asmdef
    ├── Sprites/
    ├── Materials/
    ├── Prefabs/
    ├── Levels/
    ├── Samples~/
    ├── package.json
    ├── README.md
    └── CHANGELOG.md
```

## 🎮 快速开始

### 1. 导入插件
使用上述任一方法导入插件

### 2. 创建编辑器对象
```csharp
// 在场景中创建一个空GameObject
GameObject editorObject = new GameObject("羊了个羊2D关卡编辑器");
SheepLevelEditor2D editor = editorObject.AddComponent<SheepLevelEditor2D>();
```

### 3. 开始编辑
- 使用菜单 `羊了个羊 > 新建关卡` 创建新关卡
- 在场景中点击放置卡片
- 使用 `羊了个羊 > 保存关卡` 保存工作
- 使用 `羊了个羊 > 验证关卡` 检查关卡有效性

## 🐛 常见问题

### Q: 导入时显示 "Nothing to import"
**A**: 使用ZIP格式替代，或者手动导出Package

### Q: 编译错误 "LevelValidator2D not found"
**A**: 确保使用 v1.0.2 版本，该版本已修复此问题

### Q: 菜单不显示
**A**: 确保 `SheepLevelEditor2DMenu.cs` 在 `Editor` 文件夹中

### Q: 可放置区域不显示
**A**: 检查 `PlaceableAreaVisualizer` 组件是否正确配置

## 📞 技术支持

如果仍然遇到问题：

1. **检查版本**: 确保使用 v1.0.2 或更高版本
2. **清理缓存**: 删除Library文件夹并重新导入
3. **重新编译**: 使用 `Assets > Reimport All`
4. **提交Issue**: 在GitHub上提交详细的问题报告

## 🎉 总结

**推荐使用ZIP格式导入**，这是最可靠的方法：

1. 下载 `羊了个羊2D关卡编辑器_v1.0.2.zip`
2. 解压到Unity项目的 `Assets` 目录
3. 开始使用插件

这样可以避免Unity Package Manager格式的复杂性，确保插件能够正确导入和使用。

---

**现在您可以顺利导入和使用羊了个羊2D关卡编辑器插件了！** 🎮✨ 