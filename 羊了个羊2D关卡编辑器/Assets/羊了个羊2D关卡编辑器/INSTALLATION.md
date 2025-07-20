# 安装指南

本文档详细说明了如何安装和配置羊了个羊2D关卡编辑器插件。

## 📋 系统要求

### Unity版本
- **最低版本**: Unity 2021.3 LTS
- **推荐版本**: Unity 2022.3 LTS 或更新版本
- **支持平台**: Windows, macOS, Linux

### 硬件要求
- **内存**: 最低 4GB RAM，推荐 8GB 或更多
- **存储**: 至少 100MB 可用空间
- **显卡**: 支持 OpenGL 3.0 或 DirectX 11

## 📦 安装方法

### 方法1: Unity Package Manager (推荐)

#### 从本地安装
1. 下载插件包到本地
2. 打开Unity项目
3. 进入 `Window > Package Manager`
4. 点击左上角的 `+` 按钮
5. 选择 `Add package from disk`
6. 浏览并选择插件的 `package.json` 文件
7. 点击 `Add` 完成安装

#### 从Git仓库安装
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击左上角的 `+` 按钮
4. 选择 `Add package from git URL`
5. 输入Git仓库地址：
   ```
   https://github.com/yanglegeyang2d/leveleditor.git
   ```
6. 点击 `Add` 完成安装

### 方法2: 直接导入Assets

1. 下载插件包
2. 解压到本地文件夹
3. 将 `Assets/羊了个羊2D关卡编辑器` 文件夹复制到您的Unity项目的 `Assets` 目录
4. Unity会自动导入所有资源

### 方法3: Unity Asset Store (如果可用)

1. 打开Unity项目
2. 进入 `Window > Asset Store`
3. 搜索 "羊了个羊2D关卡编辑器"
4. 点击 `Download` 下载
5. 下载完成后点击 `Import` 导入

## ⚙️ 初始配置

### 1. 创建编辑器对象

在场景中创建一个新的GameObject：

```csharp
// 方法1: 通过代码创建
GameObject editorObject = new GameObject("羊了个羊2D关卡编辑器");
SheepLevelEditor2D editor = editorObject.AddComponent<SheepLevelEditor2D>();
```

或者：

1. 在Hierarchy窗口中右键
2. 选择 `Create Empty`
3. 重命名为 "羊了个羊2D关卡编辑器"
4. 在Inspector中点击 `Add Component`
5. 搜索并添加 `SheepLevelEditor2D` 组件

### 2. 配置基本设置

在Inspector面板中配置以下基本设置：

#### 编辑器设置
- **Grid Size**: 设置网格大小，例如 (8, 8)
- **Card Spacing**: 设置卡片间距，例如 1.2
- **Card Size**: 设置卡片大小，例如 0.8
- **Total Layers**: 设置层级数量，例如 3

#### 可视化设置
- **Show Grid**: 启用网格显示
- **Show Placeable Area**: 启用可放置区域显示
- **Grid Line Color**: 设置网格线颜色
- **Area Color**: 设置区域颜色

### 3. 设置相机

确保场景中有2D相机：

1. 选择Main Camera
2. 将Projection设置为 `Orthographic`
3. 调整Size为合适的值（例如5）
4. 将Position设置为 (0, 0, -10)

### 4. 添加可视化组件

如果需要可放置区域可视化：

1. 选择编辑器GameObject
2. 添加 `PlaceableAreaVisualizer` 组件
3. 配置颜色和显示选项

## 🔧 验证安装

### 1. 检查菜单

安装完成后，您应该能在Unity菜单栏看到：
- `羊了个羊` 菜单
- 包含各种编辑器功能的子菜单

### 2. 运行测试

1. 点击Play按钮
2. 在场景中点击放置卡片
3. 使用菜单保存和加载关卡
4. 检查Console窗口是否有错误信息

### 3. 导入示例

1. 在Package Manager中找到插件
2. 点击 `Import` 导入示例
3. 运行示例场景验证功能

## 🐛 常见安装问题

### 问题1: 菜单不显示
**症状**: Unity菜单栏中没有"羊了个羊"菜单

**解决方案**:
1. 检查 `SheepLevelEditor2DMenu.cs` 是否在 `Editor` 文件夹中
2. 重新编译项目 (Ctrl+R 或 Cmd+R)
3. 重启Unity编辑器

### 问题2: 编译错误
**症状**: Console窗口显示编译错误

**解决方案**:
1. 检查Unity版本是否满足要求
2. 确保所有脚本文件都正确导入
3. 检查是否有命名空间冲突
4. 清理并重新导入资源

### 问题3: 组件无法添加
**症状**: 无法在Inspector中添加插件组件

**解决方案**:
1. 检查脚本是否在正确的文件夹中
2. 确保脚本没有编译错误
3. 重新导入插件包

### 问题4: 资源文件缺失
**症状**: 精灵图片或材质显示为粉色

**解决方案**:
1. 检查 `Sprites` 和 `Materials` 文件夹是否完整
2. 重新导入缺失的资源文件
3. 检查文件路径是否正确

## 📚 下一步

安装完成后，建议按以下顺序学习：

1. **阅读README**: 了解插件的基本功能
2. **运行基础示例**: 熟悉核心操作
3. **尝试高级示例**: 学习进阶功能
4. **查看API文档**: 了解编程接口
5. **创建自定义关卡**: 实践应用

## 🆘 获取帮助

如果遇到安装问题：

1. **查看文档**: 阅读README和故障排除指南
2. **检查日志**: 查看Unity Console窗口的错误信息
3. **搜索问题**: 在GitHub Issues中搜索类似问题
4. **提交Issue**: 如果问题仍未解决，提交新的Issue

### 联系方式
- **邮箱**: support@yanglegeyang2d.com
- **GitHub**: https://github.com/yanglegeyang2d/leveleditor/issues
- **文档**: https://github.com/yanglegeyang2d/leveleditor/wiki

---

**祝您使用愉快！** 🎮✨ 