#!/bin/bash

# 羊了个羊2D关卡编辑器 - Unity Package 生成脚本
# 使用方法: ./create_unitypackage.sh

echo "开始创建Unity Package..."

# 检查Unity是否安装
UNITY_PATH=""
if [[ "$OSTYPE" == "darwin"* ]]; then
    # macOS
    UNITY_PATH="/Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity"
elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "cygwin" ]]; then
    # Windows
    UNITY_PATH="C:/Program Files/Unity/Hub/Editor/*/Editor/Unity.exe"
else
    # Linux
    UNITY_PATH="/opt/unity/editor/Unity"
fi

# 查找Unity可执行文件
UNITY_EXECUTABLE=$(find $UNITY_PATH -name "Unity" -o -name "Unity.exe" 2>/dev/null | head -n 1)

if [ -z "$UNITY_EXECUTABLE" ]; then
    echo "错误: 未找到Unity可执行文件"
    echo "请确保Unity已安装，或者手动指定Unity路径"
    exit 1
fi

echo "找到Unity: $UNITY_EXECUTABLE"

# 创建临时项目目录
TEMP_PROJECT="temp_unity_project"
if [ -d "$TEMP_PROJECT" ]; then
    rm -rf "$TEMP_PROJECT"
fi

mkdir -p "$TEMP_PROJECT/Assets"

# 复制插件文件到临时项目
echo "复制插件文件..."
cp -r "Assets/羊了个羊2D关卡编辑器" "$TEMP_PROJECT/Assets/"

# 创建package.json文件
cat > "$TEMP_PROJECT/Assets/羊了个羊2D关卡编辑器/package.json" << 'EOF'
{
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
EOF

# 使用Unity导出package
echo "使用Unity导出package..."
"$UNITY_EXECUTABLE" -batchmode -quit -projectPath "$TEMP_PROJECT" -exportPackage "Assets/羊了个羊2D关卡编辑器" "../羊了个羊2D关卡编辑器_v1.0.1.unitypackage"

if [ $? -eq 0 ]; then
    echo "✅ Unity Package 创建成功!"
    echo "文件: 羊了个羊2D关卡编辑器_v1.0.1.unitypackage"
else
    echo "❌ Unity Package 创建失败"
fi

# 清理临时文件
echo "清理临时文件..."
rm -rf "$TEMP_PROJECT"

echo "完成!" 