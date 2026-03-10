# ES3 序列化指南

## 概述

本项目使用 Easy Save 3 (ES3) 进行存档序列化。ES3 在分析类型时会递归遍历所有可序列化字段，若存在循环引用会导致 StackOverflowException，表现为 Unity 卡死/闪退。

## 核心问题：循环引用

### 问题表现
- 点击存档时 Unity 直接卡死、闪退
- 无法看到日志（死循环导致崩溃）
- 堆栈显示 `StackOverflowException` 在 ES3 类型分析相关方法中

### 根因分析
ES3 序列化时会递归分析所有字段的类型。若类型 A 包含类型 B，而类型 B 又包含类型 A，则形成循环引用，导致无限递归。

**典型案例**：
```csharp
// ItemData 包含 EquipProtoData
public class ItemData
{
    public EquipProtoData equipProtoData;
}

// EquipProtoData 包含 List<ItemData>（宝石列表）
public class EquipProtoData
{
    public List<ItemData> gemList;  // 循环引用！
}
```

## 解决方案

### 方案一：[NonSerialized] 标记（推荐）

对运行时使用但不需要持久化的字段添加 `[NonSerialized]`：

```csharp
[NonSerialized]
public List<ItemData> gemList = new List<ItemData>();
```

### 方案二：拆分数据结构

若字段需要持久化，创建不含循环引用的中间数据类：

```csharp
// 原始类（运行时使用）
public class EquipProtoData
{
    [NonSerialized]
    public List<ItemData> gemList = new List<ItemData>();  // 不序列化
    
    public List<GemSaveData> gemSaveList = new List<GemSaveData>();  // 序列化用
}

// 中间数据类（无循环引用）
[Serializable]
public class GemSaveData
{
    public int settingId;
    public ulong onlyId;
    public int quality;
    public GemData gemData;  // GemData 不包含 ItemData，无循环
}
```

### 方案三：存档前后转换

在 `ArchiveManager` 中添加转换逻辑：

```csharp
// 存档前：运行时数据 → 序列化数据
private void PrepareEquipDataForSave(GameInfo gameInfo)
{
    foreach (var item in gameInfo.ItemModel.itemDataList)
    {
        item?.equipProtoData?.PrepareForSave();
    }
    // ... 遍历所有可能包含装备的位置
}

// 加载后：序列化数据 → 运行时数据
private void RestoreEquipDataAfterLoad(GameInfo gameInfo)
{
    foreach (var item in gameInfo.ItemModel.itemDataList)
    {
        item?.equipProtoData?.RestoreAfterLoad();
    }
    // ... 遍历所有可能包含装备的位置
}
```

## 排查循环引用的方法

### 1. 识别症状
- 存档时卡死/闪退（非卡顿）
- 无法输出日志（死循环在日志输出前就崩溃）

### 2. 定位问题类型
检查 ES3 序列化的根类型（如 `GameInfo`）及其所有嵌套类型，寻找：
- 类型 A 包含类型 B，类型 B 又包含类型 A
- 类型 A 包含 `List<B>`，类型 B 包含类型 A
- 任何形成闭环的引用链

### 3. 常见循环引用位置
- 装备与宝石（装备包含宝石列表，宝石是物品，物品包含装备数据）
- 角色与物品（角色包含物品列表，物品可能引用所属角色）
- 父子关系（父对象包含子列表，子对象引用父对象）

## 最佳实践

### 数据类设计原则
1. **分离运行时数据与持久化数据**：运行时便利的引用关系不一定适合序列化
2. **避免双向引用**：若 A→B 需要序列化，则 B→A 应标记 `[NonSerialized]`
3. **使用 ID 而非引用**：持久化时存储 ID，运行时通过 ID 查找重建引用

### 字段标记规范
```csharp
[Serializable]
public class SomeData
{
    // 需要持久化的基础数据
    public int id;
    public string name;
    
    // 运行时缓存，不序列化
    [NonSerialized]
    public SomeSetting setting;
    
    // 可能导致循环引用的字段，不序列化
    [NonSerialized]
    public ParentData parent;
    
    // 用于序列化的替代字段
    public int parentId;  // 存 ID 而非引用
}
```

### 存档流程
```
SaveArchive:
1. PrepareForSave() - 将运行时引用转换为可序列化数据
2. ES3.Save() - 执行序列化
3. （可选）清理临时数据

LoadArchive:
1. ES3.Load() - 执行反序列化
2. RestoreAfterLoad() - 将序列化数据还原为运行时引用
```

## 相关文件

- `Assets/Scripts/Data/ItemData.cs` - ItemData、EquipProtoData、GemSaveData 定义
- `Assets/Scripts/Manager/ArchiveManager.cs` - 存档管理器，包含转换逻辑
- `Assets/Scripts/Data/PeopleData.cs` - 角色数据，可能包含装备引用

## 调试技巧

### 无法输出日志时
1. 使用二分法注释代码，定位崩溃位置
2. 在 ES3.Save 前逐个字段检查，找出问题类型
3. 检查最近修改的数据类是否引入了新的引用关系

### 验证修复
修复后测试：
1. 新建存档 → 保存 → 加载 → 验证数据完整
2. 旧存档 → 加载 → 保存 → 再加载 → 验证数据完整
3. 检查运行时功能（如宝石镶嵌）是否正常
