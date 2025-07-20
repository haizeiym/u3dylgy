# 手动创建Unity Package的说明

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
/Applications/Unity/Hub/Editor/[版本]/Unity.app/Contents/MacOS/Unity \
  -batchmode -quit \
  -projectPath [项目路径] \
  -exportPackage "Assets/羊了个羊2D关卡编辑器" \
  "[输出路径]/羊了个羊2D关卡编辑器_v1.0.1.unitypackage"

# Windows
"C:\Program Files\Unity\Hub\Editor\[版本]\Editor\Unity.exe" \
  -batchmode -quit \
  -projectPath [项目路径] \
  -exportPackage "Assets/羊了个羊2D关卡编辑器" \
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
