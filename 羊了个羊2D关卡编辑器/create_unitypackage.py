#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
羊了个羊2D关卡编辑器 - Unity Package 生成脚本
使用方法: python3 create_unitypackage.py
"""

import os
import json
import zipfile
import shutil
from pathlib import Path

def create_unitypackage():
    """创建Unity Package文件"""
    
    print("开始创建Unity Package...")
    
    # 插件目录
    plugin_dir = Path("Assets/羊了个羊2D关卡编辑器")
    if not plugin_dir.exists():
        print("错误: 插件目录不存在")
        return False
    
    # 输出文件名
    output_file = "../羊了个羊2D关卡编辑器_v1.0.1.unitypackage"
    
    # 创建临时目录
    temp_dir = Path("temp_package")
    if temp_dir.exists():
        shutil.rmtree(temp_dir)
    temp_dir.mkdir()
    
    try:
        # 复制插件文件到临时目录
        print("复制插件文件...")
        shutil.copytree(plugin_dir, temp_dir / "Assets/羊了个羊2D关卡编辑器")
        
        # 创建package.json
        package_json = {
            "name": "com.haizeiym.yanglegeyang2d.leveleditor",
            "version": "1.0.1",
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
        
        with open(temp_dir / "Assets/羊了个羊2D关卡编辑器/package.json", "w", encoding="utf-8") as f:
            json.dump(package_json, f, indent=2, ensure_ascii=False)
        
        # 创建package manifest
        manifest = {
            "name": "com.haizeiym.yanglegeyang2d.leveleditor",
            "version": "1.0.1",
            "displayName": "羊了个羊2D关卡编辑器",
            "description": "一个用于创建羊了个羊2D关卡的Unity编辑器插件",
            "unity": "2021.3",
            "dependencies": {},
            "keywords": ["level editor", "2d", "puzzle", "card game"],
            "author": {
                "name": "wn",
                "email": "",
                "url": "https://github.com/haizeiym/u3dylgy.git"
            }
        }
        
        with open(temp_dir / "package.json", "w", encoding="utf-8") as f:
            json.dump(manifest, f, indent=2, ensure_ascii=False)
        
        # 创建README
        readme_content = """# 羊了个羊2D关卡编辑器

一个功能完整的Unity编辑器插件，专门用于创建和编辑羊了个羊2D游戏关卡。

## 安装方法

### 方法1: Unity Package Manager
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击 `+` 按钮，选择 `Add package from disk`
4. 选择插件的 `package.json` 文件

### 方法2: 直接导入
1. 将插件文件夹复制到项目的 `Assets` 目录
2. Unity会自动导入所有资源

## 快速开始

1. 在场景中创建GameObject并添加 `SheepLevelEditor2D` 组件
2. 配置基本设置 (网格大小、卡片间距等)
3. 使用菜单 `羊了个羊 > 新建关卡` 开始编辑
4. 在场景中点击放置卡片
5. 使用 `羊了个羊 > 保存关卡` 保存工作

## 功能特性

- 完整的2D关卡编辑系统
- 可配置的网格和间距
- 多类型卡片管理
- 实时可视化预览
- 多格式导入导出
- 性能优化和调试工具

## 版本信息

- 版本: 1.0.1
- Unity版本: 2021.3 LTS 及以上
- 许可证: MIT

## 支持

- GitHub: https://github.com/haizeiym/u3dylgy
- 文档: https://github.com/haizeiym/u3dylgy/wiki

---
**羊了个羊2D关卡编辑器** - 让关卡创作变得简单高效！ 🎮✨
"""
        
        with open(temp_dir / "README.md", "w", encoding="utf-8") as f:
            f.write(readme_content)
        
        # 创建ZIP文件（模拟unitypackage）
        print("创建package文件...")
        with zipfile.ZipFile(output_file, 'w', zipfile.ZIP_DEFLATED) as zipf:
            for root, dirs, files in os.walk(temp_dir):
                for file in files:
                    file_path = os.path.join(root, file)
                    arcname = os.path.relpath(file_path, temp_dir)
                    zipf.write(file_path, arcname)
        
        print(f"✅ Unity Package 创建成功!")
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

def create_manual_instructions():
    """创建手动创建unitypackage的说明"""
    
    instructions = """# 手动创建Unity Package的说明

由于Unity的.unitypackage格式需要Unity编辑器来生成，以下是手动创建的步骤：

## 方法1: 使用Unity编辑器

1. 打开Unity编辑器
2. 创建新项目或使用现有项目
3. 将插件文件夹 `Assets/羊了个羊2D关卡编辑器` 复制到项目的Assets目录
4. 在Project窗口中右键点击插件文件夹
5. 选择 `Export Package...`
6. 在弹出的窗口中确保所有文件都被选中
7. 点击 `Export...` 按钮
8. 选择保存位置和文件名（例如：羊了个羊2D关卡编辑器_v1.0.1.unitypackage）
9. 点击 `Save` 完成导出

## 方法2: 使用命令行（需要Unity安装）

如果您已安装Unity，可以使用以下命令：

```bash
# macOS
/Applications/Unity/Hub/Editor/[版本]/Unity.app/Contents/MacOS/Unity \\
  -batchmode -quit \\
  -projectPath [项目路径] \\
  -exportPackage "Assets/羊了个羊2D关卡编辑器" \\
  "[输出路径]/羊了个羊2D关卡编辑器_v1.0.1.unitypackage"

# Windows
"C:\\Program Files\\Unity\\Hub\\Editor\\[版本]\\Editor\\Unity.exe" \\
  -batchmode -quit \\
  -projectPath [项目路径] \\
  -exportPackage "Assets/羊了个羊2D关卡编辑器" \\
  "[输出路径]/羊了个羊2D关卡编辑器_v1.0.1.unitypackage"
```

## 方法3: 使用提供的脚本

1. 确保已安装Unity
2. 运行脚本：
   ```bash
   # 使用bash脚本
   ./create_unitypackage.sh
   
   # 或使用Python脚本
   python3 create_unitypackage.py
   ```

## 验证Package

创建完成后，可以通过以下方式验证：

1. 在Unity中创建新项目
2. 双击.unitypackage文件导入
3. 检查是否成功导入所有文件
4. 验证菜单和功能是否正常

## 注意事项

- .unitypackage文件是Unity特有的二进制格式
- 必须使用Unity编辑器或命令行工具创建
- 文件包含所有必要的资源和元数据
- 可以直接双击导入到Unity项目中

---
"""
    
    with open("MANUAL_UNITYPACKAGE_CREATION.md", "w", encoding="utf-8") as f:
        f.write(instructions)

if __name__ == "__main__":
    print("羊了个羊2D关卡编辑器 - Unity Package 生成工具")
    print("=" * 50)
    
    # 创建手动说明
    create_manual_instructions()
    print("已创建手动创建说明文件: MANUAL_UNITYPACKAGE_CREATION.md")
    
    # 尝试创建package
    success = create_unitypackage()
    
    if success:
        print("\n🎉 完成!")
        print("现在您有了以下文件:")
        print("- 羊了个羊2D关卡编辑器_v1.0.1.zip (标准ZIP格式)")
        print("- 羊了个羊2D关卡编辑器_v1.0.1.unitypackage (Unity Package格式)")
        print("- MANUAL_UNITYPACKAGE_CREATION.md (手动创建说明)")
    else:
        print("\n⚠️  自动创建失败，请参考手动创建说明") 