# GridLines累积问题修复说明

## 问题描述
在启动时，GridLines会一直增加，导致场景中累积大量的网格线对象，影响性能。

## 问题原因
在`PlaceableAreaVisualizer`的`Update()`方法中，每一帧都会调用`UpdatePlaceableArea()`，而`UpdatePlaceableArea()`又会调用`UpdateGridLines()`，`UpdateGridLines()`会重新创建所有的网格线。这导致了GridLines不断累积。

### 问题流程
1. `Update()`方法每帧都调用`UpdatePlaceableArea()`
2. `UpdatePlaceableArea()`调用`UpdateGridLines()`
3. `UpdateGridLines()`重新创建所有网格线
4. 旧的网格线没有被正确清理，导致累积

## 修复方案

### 1. 优化Update逻辑
只在网格大小或卡片间距发生变化时才更新，而不是每帧都更新。

### 2. 改进对象清理
使用`DestroyImmediate()`确保对象立即销毁，避免延迟删除导致的问题。

### 3. 防止重复创建
在创建新的GridLines对象前，先检查并销毁已存在的对象。

## 修复的文件和方法

### PlaceableAreaVisualizer.cs

#### 1. 添加缓存变量
```csharp
private Vector2 lastGridSize;
private float lastCardSpacing;
```

#### 2. 优化Update方法
```csharp
// 修复前
void Update()
{
    if (levelEditor != null && showPlaceableArea)
    {
        UpdatePlaceableArea();
    }
}

// 修复后
void Update()
{
    if (levelEditor != null && showPlaceableArea)
    {
        // 只在网格大小或卡片间距发生变化时才更新
        if (lastGridSize != levelEditor.gridSize || lastCardSpacing != levelEditor.cardSpacing)
        {
            UpdatePlaceableArea();
            lastGridSize = levelEditor.gridSize;
            lastCardSpacing = levelEditor.cardSpacing;
        }
    }
}
```

#### 3. 改进CreateGridLines方法
```csharp
// 修复前
void CreateGridLines()
{
    if (!showGridLines) return;
    
    gridLinesObject = new GameObject("GridLines");
    // ... 创建网格线
}

// 修复后
void CreateGridLines()
{
    if (!showGridLines) return;
    
    // 如果GridLines对象已存在，先销毁它
    if (gridLinesObject != null)
    {
        DestroyImmediate(gridLinesObject);
    }
    
    gridLinesObject = new GameObject("GridLines");
    // ... 创建网格线
}
```

#### 4. 改进UpdateGridLines方法
```csharp
// 修复前
void UpdateGridLines()
{
    if (gridLinesObject != null)
    {
        foreach (Transform child in gridLinesObject.transform)
        {
            Destroy(child.gameObject);
        }
        CreateGridLines();
    }
}

// 修复后
void UpdateGridLines()
{
    if (gridLinesObject != null)
    {
        foreach (Transform child in gridLinesObject.transform)
        {
            if (child != null)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        CreateGridLines();
    }
}
```

#### 5. 初始化缓存值
```csharp
void Start()
{
    levelEditor = FindObjectOfType<SheepLevelEditor2D>();
    if (levelEditor == null)
    {
        Debug.LogError("找不到SheepLevelEditor2D组件！");
        return;
    }
    
    // 初始化缓存值
    lastGridSize = levelEditor.gridSize;
    lastCardSpacing = levelEditor.cardSpacing;
    
    CreatePlaceableArea();
    CreateBorder();
    CreateGridLines();
}
```

## 测试验证

### 测试脚本
创建了`GridLinesTest.cs`测试脚本，提供：
- **自动测试**: 按F3键开始测试
- **实时监控**: 监控GridLines数量变化
- **手动检查**: 右键菜单检查当前状态
- **详细日志**: 追踪每个测试步骤

### 测试功能
1. **累积检测**: 监控GridLines数量是否稳定
2. **对象检查**: 检查场景中的GridLines对象
3. **性能监控**: 实时显示GridLines数量
4. **手动验证**: 提供手动检查功能

### 使用方法
1. 将`GridLinesTest.cs`脚本添加到场景中的GameObject上
2. 确保`visualizer`和`editor2D`字段指向正确的组件
3. 运行游戏，按F3开始测试

## 修复效果

修复后：
- ✅ GridLines只在需要时更新（网格大小或间距变化）
- ✅ 旧的GridLines对象被正确清理
- ✅ 避免了每帧重新创建网格线
- ✅ 提高了性能，减少了内存占用
- ✅ 消除了GridLines累积问题

## 性能优化

### 修复前
- 每帧都重新创建所有网格线
- 旧的网格线对象没有被及时清理
- 内存占用持续增加

### 修复后
- 只在必要时更新网格线
- 旧的网格线对象立即被清理
- 内存占用保持稳定

## 注意事项
1. 修复后，网格线只会在网格大小或卡片间距变化时更新
2. 如果需要强制更新网格线，可以手动调用`UpdatePlaceableArea()`
3. 建议在运行时测试，而不是在编辑模式下测试
4. 如果仍有问题，检查是否有其他脚本在创建GridLines对象 