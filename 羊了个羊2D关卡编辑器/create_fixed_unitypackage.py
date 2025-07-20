#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
羊了个羊2D关卡编辑器 - 修复版Unity Package生成脚本
修复LevelValidator2D缺失问题
"""

import os
import json
import zipfile
import shutil
from pathlib import Path

def create_fixed_unitypackage():
    """创建修复版的Unity Package文件"""
    
    print("创建修复版Unity Package...")
    
    # 插件目录
    plugin_dir = Path("Assets/羊了个羊2D关卡编辑器")
    if not plugin_dir.exists():
        print("错误: 插件目录不存在")
        return False
    
    # 输出文件名
    output_file = "../羊了个羊2D关卡编辑器_v1.0.2.unitypackage"
    
    # 创建临时目录
    temp_dir = Path("temp_fixed_package")
    if temp_dir.exists():
        shutil.rmtree(temp_dir)
    temp_dir.mkdir()
    
    try:
        # 复制插件文件到临时目录
        print("复制插件文件...")
        shutil.copytree(plugin_dir, temp_dir / "Assets/羊了个羊2D关卡编辑器")
        
        # 创建package.json（根目录）
        root_package_json = {
            "name": "com.haizeiym.yanglegeyang2d.leveleditor",
            "version": "1.0.2",
            "displayName": "羊了个羊2D关卡编辑器",
            "description": "一个用于创建羊了个羊2D关卡的Unity编辑器插件，提供完整的关卡编辑功能，包括卡片放置、网格系统、层级管理、可视化预览等。",
            "unity": "2021.3",
            "dependencies": {},
            "keywords": [
                "level editor",
                "2d",
                "puzzle",
                "card game",
                "yanglegeyang",
                "关卡编辑器",
                "羊了个羊"
            ],
            "author": {
                "name": "wn",
                "email": "",
                "url": "https://github.com/haizeiym/u3dylgy.git"
            },
            "license": "MIT",
            "repository": {
                "type": "git",
                "url": "https://github.com/haizeiym/u3dylgy.git"
            },
            "bugs": {
                "url": "https://github.com/haizeiym/u3dylgy/issues"
            },
            "homepage": "https://github.com/haizeiym/u3dylgy#readme",
            "documentationUrl": "https://github.com/haizeiym/u3dylgy/wiki",
            "changelogUrl": "https://github.com/haizeiym/u3dylgy/blob/main/CHANGELOG.md",
            "licensesUrl": "https://github.com/haizeiym/u3dylgy/blob/main/LICENSE",
            "samples": [
                {
                    "displayName": "基础示例",
                    "description": "包含基础关卡编辑功能的示例场景和脚本",
                    "path": "Samples~/BasicExample"
                },
                {
                    "displayName": "高级功能示例",
                    "description": "展示高级功能如层级预览、自定义区域等的示例",
                    "path": "Samples~/AdvancedExample"
                }
            ]
        }
        
        with open(temp_dir / "package.json", "w", encoding="utf-8") as f:
            json.dump(root_package_json, f, indent=2, ensure_ascii=False)
        
        # 创建README
        readme_content = """# 羊了个羊2D关卡编辑器 v1.0.2

一个功能完整的Unity编辑器插件，专门用于创建和编辑羊了个羊2D游戏关卡。

## 🎮 功能特性

### 核心功能
- **2D关卡编辑**: 完整的2D关卡编辑系统
- **网格系统**: 可配置的网格大小和间距
- **卡片管理**: 支持多种卡片类型和层级
- **可视化预览**: 实时预览可放置区域和网格
- **层级系统**: 多层卡片管理，支持层级预览
- **导入导出**: 支持JSON、XML、二进制格式
- **关卡验证**: 自动验证关卡的有效性和可解性

### 编辑器功能
- **直观的GUI界面**: 友好的编辑器界面
- **实时调试**: 内置调试工具和可视化
- **自动保存**: 支持自动保存和备份
- **性能优化**: 高性能模式和输入节流

## 📦 安装说明

### 方法1: Unity Package Manager (推荐)
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
- 使用 `羊了个羊 > 验证关卡` 检查关卡有效性

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

## 📁 文件结构

```
羊了个羊2D关卡编辑器/
├── Runtime/                    # 运行时脚本
│   ├── SheepLevelEditor2D.cs   # 主编辑器
│   ├── PlaceableAreaVisualizer.cs # 区域可视化
│   ├── GUIEventManager.cs      # GUI事件管理
│   ├── LevelValidator2D.cs     # 关卡验证器
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

// 验证关卡
public void ValidateCurrentLevel()

// 获取实际区域大小
public Vector2 GetActualAreaSize()

// 网格对齐
public Vector2 SnapToGrid2D(Vector2 worldPos)
```

#### LevelValidator2D
关卡验证器，提供关卡有效性检查。

```csharp
// 验证关卡
public static ValidationResult ValidateLevel(LevelData2D levelData)

// 获取验证报告
public static string GetValidationReport(ValidationResult result)
```

## 📝 示例代码

### 创建自定义关卡
```csharp
using UnityEngine;
using YangLeGeYang2D.LevelEditor;

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
        
        // 验证关卡
        levelEditor.ValidateCurrentLevel();
        
        // 保存关卡
        levelEditor.SaveLevel();
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

**Q: 编译错误 "LevelValidator2D not found"**
A: 确保 `LevelValidator2D.cs` 文件在 `Runtime` 文件夹中

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

## 📞 支持

- GitHub: https://github.com/haizeiym/u3dylgy
- 文档: https://github.com/haizeiym/u3dylgy/wiki

## 📈 更新日志

### v1.0.2
- 🔧 **修复LevelValidator2D缺失问题** - 添加了缺失的LevelValidator2D类
- 🔧 **完善关卡验证功能** - 支持关卡有效性检查和可解性验证
- 🔧 **优化编译错误** - 修复了所有编译错误和引用问题

### v1.0.1
- 🔧 **修复Package Manager编译错误** - 添加了正确的程序集定义文件
- 🔧 **修复命名空间问题** - 为所有脚本添加了命名空间
- 🔧 **优化文件结构** - 符合Unity包标准

### v1.0.0
- 🎮 **初始版本发布** - 完整的2D关卡编辑功能

---

**羊了个羊2D关卡编辑器** - 让关卡创作变得简单高效！ 🎮✨
"""
        
        with open(temp_dir / "README.md", "w", encoding="utf-8") as f:
            f.write(readme_content)
        
        # 创建CHANGELOG
        changelog_content = """# 更新日志

本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/) 规范。

## [1.0.2] - 2024-01-XX

### 修复
- 🔧 **修复LevelValidator2D缺失问题** - 添加了缺失的LevelValidator2D类
- 🔧 **完善关卡验证功能** - 支持关卡有效性检查和可解性验证
- 🔧 **优化编译错误** - 修复了所有编译错误和引用问题

### 新增功能
- ✅ **关卡验证系统** - 自动检查关卡的有效性
- ✅ **可解性检查** - 验证关卡是否可以被解决
- ✅ **详细验证报告** - 提供详细的错误和警告信息

## [1.0.1] - 2024-01-XX

### 修复
- 🔧 **修复Package Manager编译错误** - 添加了正确的程序集定义文件
- 🔧 **修复命名空间问题** - 为所有脚本添加了 `YangLeGeYang2D.LevelEditor` 命名空间
- 🔧 **修复Editor脚本编译** - 创建了专门的Editor程序集定义
- 🔧 **优化文件结构** - 将脚本移动到Runtime文件夹，符合Unity包标准

### 技术改进
- 📁 **标准化的包结构** - 使用Runtime和Editor文件夹分离
- 📁 **程序集定义** - 添加了 `.asmdef` 文件以正确管理编译
- 📁 **命名空间组织** - 运行时使用 `YangLeGeYang2D.LevelEditor`，编辑器使用 `YangLeGeYang2D.LevelEditor.Editor`

### 兼容性
- ✅ **Unity Package Manager** - 现在完全支持通过Package Manager安装
- ✅ **Unity 2021.3+** - 保持向后兼容性
- ✅ **跨平台** - Windows, macOS, Linux 支持

## [1.0.0] - 2024-01-XX

### 新增功能
- 🎮 完整的2D关卡编辑系统
- 📐 可配置的网格系统和间距
- 🃏 多类型卡片管理和层级系统
- 👁️ 实时可视化预览功能
- 💾 多格式导入导出 (JSON, XML, 二进制)
- 🎨 自定义颜色和纹理质量设置
- 📷 2D相机控制和缩放
- 🔧 内置调试工具和性能优化
- 📋 网格标签和坐标显示
- 💾 自动保存和备份功能

---
"""
        
        with open(temp_dir / "CHANGELOG.md", "w", encoding="utf-8") as f:
            f.write(changelog_content)
        
        # 创建LICENSE
        license_content = """MIT License

Copyright (c) 2024 羊了个羊2D关卡编辑器开发团队

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
"""
        
        with open(temp_dir / "LICENSE", "w", encoding="utf-8") as f:
            f.write(license_content)
        
        # 创建ZIP文件
        print("创建package文件...")
        with zipfile.ZipFile(output_file, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(temp_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, temp_dir)
                    zipf.write(file_path, arcname)
        
        print(f"✅ 修复版Unity Package 创建成功!")
        print(f"文件: {output_file}")
        
        # 获取文件大小
        file_size = os.path.getsize(output_file)
        print(f"大小: {file_size / 1024:.1f} KB")
        
        return True
        
    except Exception as e:
        print(f"❌ 创建失败: {e}")
        return False
    
    finally:
        # 清理临时文件
        print("清理临时文件...")
        if temp_dir.exists():
            shutil.rmtree(temp_dir)

if __name__ == "__main__":
    print("羊了个羊2D关卡编辑器 - 修复版Unity Package生成工具")
    print("=" * 50)
    
    success = create_fixed_unitypackage()
    
    if success:
        print("\n🎉 完成!")
        print("已创建修复版Unity Package文件")
        print("修复内容:")
        print("- 添加了缺失的LevelValidator2D类")
        print("- 修复了编译错误")
        print("- 完善了关卡验证功能")
    else:
        print("\n❌ 创建失败") 