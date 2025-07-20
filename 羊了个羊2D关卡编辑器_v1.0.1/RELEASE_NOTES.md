# 羊了个羊2D关卡编辑器 v1.0.0 - 发布说明

## 🎉 版本信息

- **版本号**: 1.0.0
- **发布日期**: 2024年1月
- **Unity版本**: 2021.3 LTS 及以上
- **平台支持**: Windows, macOS, Linux

## 📦 插件包内容

### 核心文件
- `package.json` - Unity包配置文件
- `README.md` - 详细使用说明
- `INSTALLATION.md` - 安装指南
- `CHANGELOG.md` - 更新日志
- `LICENSE` - MIT许可证

### 脚本文件
- `Scripts/SheepLevelEditor2D.cs` - 主编辑器脚本
- `Scripts/PlaceableAreaVisualizer.cs` - 可放置区域可视化
- `Scripts/GUIEventManager.cs` - GUI事件管理
- `Editor/SheepLevelEditor2DMenu.cs` - 编辑器菜单

### 资源文件
- `Sprites/` - 8种卡片精灵图片
- `Materials/` - 8种卡片材质
- `Prefabs/` - 快速设置预制体
- `Levels/` - 示例关卡文件

### 示例代码
- `Samples~/BasicExample/` - 基础功能示例
- `Samples~/AdvancedExample/` - 高级功能示例

## 🚀 主要功能

### 核心编辑功能
- ✅ 2D关卡编辑系统
- ✅ 可配置网格和间距
- ✅ 多类型卡片管理
- ✅ 多层卡片系统
- ✅ 实时可视化预览

### 编辑器功能
- ✅ 直观的GUI界面
- ✅ 菜单系统集成
- ✅ 实时调试工具
- ✅ 性能优化模式

### 导入导出
- ✅ JSON格式支持
- ✅ XML格式支持
- ✅ 二进制格式支持
- ✅ 自动保存和备份

### 高级功能
- ✅ 自定义区域大小
- ✅ 层级预览模式
- ✅ 网格标签显示
- ✅ 相机控制
- ✅ 纹理质量设置

## 📋 系统要求

### Unity版本
- **最低**: Unity 2021.3 LTS
- **推荐**: Unity 2022.3 LTS 或更新版本

### 硬件要求
- **内存**: 4GB RAM (推荐 8GB+)
- **存储**: 100MB 可用空间
- **显卡**: OpenGL 3.0 或 DirectX 11 支持

## 🔧 安装说明

### 方法1: Unity Package Manager
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击 `+` 按钮，选择 `Add package from disk`
4. 选择插件的 `package.json` 文件

### 方法2: 直接导入
1. 将插件文件夹复制到项目的 `Assets` 目录
2. Unity会自动导入所有资源

## 🎯 快速开始

1. **创建编辑器对象**
   ```csharp
   GameObject editorObject = new GameObject("羊了个羊2D关卡编辑器");
   SheepLevelEditor2D editor = editorObject.AddComponent<SheepLevelEditor2D>();
   ```

2. **配置基本设置**
   - 设置网格大小 (Grid Size)
   - 配置卡片间距 (Card Spacing)
   - 调整卡片大小 (Card Size)

3. **开始编辑**
   - 使用菜单 `羊了个羊 > 新建关卡`
   - 在场景中点击放置卡片
   - 使用 `羊了个羊 > 保存关卡` 保存工作

## 📚 文档和示例

### 文档
- **README.md** - 完整功能说明和API参考
- **INSTALLATION.md** - 详细安装指南
- **CHANGELOG.md** - 版本更新历史

### 示例
- **基础示例** - 核心功能演示
- **高级示例** - 进阶功能展示
- **示例README** - 示例使用说明

## 🐛 已知问题

### v1.0.0 已知问题
- 在极少数情况下，大量卡片可能导致性能下降
- 某些特殊字符在关卡名称中可能显示异常
- 在低分辨率显示器上GUI可能显示不完整

### 解决方案
- 启用高性能模式
- 使用英文或数字命名关卡
- 调整Unity窗口大小

## 🔄 更新计划

### v1.1.0 (计划中)
- 添加更多卡片类型
- 优化性能表现
- 增加更多导出格式
- 改进GUI界面

### v1.2.0 (计划中)
- 添加关卡验证功能
- 支持自定义卡片精灵
- 增加批量操作功能
- 添加关卡模板系统

## 🤝 支持和反馈

### 获取帮助
- **文档**: 查看README和安装指南
- **示例**: 运行示例代码学习使用方法
- **问题反馈**: 在GitHub提交Issue

### 联系方式
- **邮箱**: support@yanglegeyang2d.com
- **GitHub**: https://github.com/yanglegeyang2d/leveleditor
- **文档**: https://github.com/yanglegeyang2d/leveleditor/wiki

## 📄 许可证

本项目采用 MIT 许可证，详情请查看 [LICENSE](LICENSE) 文件。

## 🙏 致谢

感谢所有为这个项目做出贡献的开发者！

特别感谢：
- Unity Technologies 提供的优秀开发平台
- 开源社区的支持和反馈
- 测试用户的宝贵建议

---

**羊了个羊2D关卡编辑器 v1.0.0** - 让关卡创作变得简单高效！ 🎮✨

**下载地址**: [羊了个羊2D关卡编辑器_v1.0.0.zip](../羊了个羊2D关卡编辑器_v1.0.0.zip) 