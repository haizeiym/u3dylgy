# Package Manager 修复说明

## 🔧 问题描述

在 v1.0.0 版本中，当通过 Unity Package Manager 安装插件时，会出现以下编译错误：

```
Script 'Packages/com.yanglegeyang2d.leveleditor/Editor/SheepLevelEditor2DMenu.cs' will not be compiled because it exists outside the Assets folder and does not to belong to any assembly definition file.
```

## 🎯 问题原因

这个错误是由于 Unity Package Manager 的特殊要求导致的：

1. **程序集定义缺失**: Package Manager 中的脚本需要明确的程序集定义文件 (`.asmdef`)
2. **文件结构不规范**: 没有按照 Unity 包的标准结构组织文件
3. **命名空间缺失**: 脚本没有正确的命名空间定义

## ✅ 解决方案

### 1. 添加程序集定义文件

#### 运行时程序集定义
**文件**: `Runtime/羊了个羊2D关卡编辑器.asmdef`
```json
{
    "name": "羊了个羊2D关卡编辑器",
    "rootNamespace": "YangLeGeYang2D.LevelEditor",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

#### 编辑器程序集定义
**文件**: `Editor/羊了个羊2D关卡编辑器.Editor.asmdef`
```json
{
    "name": "羊了个羊2D关卡编辑器.Editor",
    "rootNamespace": "YangLeGeYang2D.LevelEditor.Editor",
    "references": [
        "羊了个羊2D关卡编辑器"
    ],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": false
}
```

### 2. 重新组织文件结构

#### 新的文件结构
```
羊了个羊2D关卡编辑器/
├── Runtime/                    # 运行时脚本
│   ├── SheepLevelEditor2D.cs
│   ├── PlaceableAreaVisualizer.cs
│   ├── GUIEventManager.cs
│   ├── AssemblyInfo.cs
│   └── 羊了个羊2D关卡编辑器.asmdef
├── Editor/                     # 编辑器脚本
│   ├── SheepLevelEditor2DMenu.cs
│   └── 羊了个羊2D关卡编辑器.Editor.asmdef
├── Sprites/                    # 精灵图片
├── Materials/                  # 材质文件
├── Prefabs/                    # 预制体
├── Levels/                     # 示例关卡
└── Samples~/                   # 示例代码
```

### 3. 添加命名空间

#### 运行时脚本命名空间
```csharp
namespace YangLeGeYang2D.LevelEditor
{
    // 脚本内容
}
```

#### 编辑器脚本命名空间
```csharp
namespace YangLeGeYang2D.LevelEditor.Editor
{
    // 脚本内容
}
```

### 4. 更新示例代码

在示例代码中添加正确的 using 语句：
```csharp
using UnityEngine;
using YangLeGeYang2D.LevelEditor;
```

## 📋 修复内容总结

### v1.0.1 修复项目

1. **✅ 程序集定义文件**
   - 添加了运行时程序集定义
   - 添加了编辑器程序集定义
   - 正确配置了引用关系

2. **✅ 文件结构重组**
   - 将脚本移动到 Runtime 文件夹
   - 保持 Editor 脚本在 Editor 文件夹
   - 符合 Unity 包标准结构

3. **✅ 命名空间添加**
   - 为所有脚本添加了命名空间
   - 分离了运行时和编辑器命名空间
   - 更新了示例代码的引用

4. **✅ 兼容性改进**
   - 完全支持 Unity Package Manager
   - 保持向后兼容性
   - 支持所有 Unity 2021.3+ 版本

## 🚀 安装方法

### 方法1: Unity Package Manager (推荐)
1. 打开Unity项目
2. 进入 `Window > Package Manager`
3. 点击 `+` 按钮，选择 `Add package from disk`
4. 选择插件的 `package.json` 文件
5. 点击 `Add` 完成安装

### 方法2: 直接导入
1. 解压插件包
2. 将 `Assets/羊了个羊2D关卡编辑器` 文件夹复制到您的Unity项目的 `Assets` 目录
3. Unity会自动导入所有资源

## 🔍 验证修复

安装完成后，请检查：

1. **✅ 编译无错误**: Console窗口没有编译错误
2. **✅ 菜单显示**: Unity菜单栏显示"羊了个羊"菜单
3. **✅ 组件可用**: 可以在Inspector中添加插件组件
4. **✅ 示例运行**: 示例代码可以正常运行

## 📞 技术支持

如果仍然遇到问题：

1. **检查Unity版本**: 确保使用Unity 2021.3 LTS或更高版本
2. **清理缓存**: 删除Library文件夹并重新导入
3. **重新编译**: 使用 `Assets > Reimport All`
4. **提交Issue**: 在GitHub上提交详细的问题报告

---

**v1.0.1 版本已经完全修复了Package Manager的编译问题！** 🎉 