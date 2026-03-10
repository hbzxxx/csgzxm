# 存档优化指南

## 概述

本项目使用 ES3 进行存档序列化。存档文件过大或保存耗时过长会影响用户体验。本文档记录存档优化的经验和最佳实践。

## 存档大小分析

### 问题诊断
当存档文件异常大（如新存档就有 2MB）时，需要分析 `GameInfo` 中哪些字段占用空间最大。

### 常见大数据来源

| 数据类型 | 典型大小 | 原因 |
|---------|---------|------|
| 竞技场宗门数据 | 1-2MB | 2000+ 宗门，每个宗门 4 个 PeopleData |
| 物品列表 | 100KB-500KB | 大量物品，每个物品有装备数据 |
| 弟子列表 | 50KB-200KB | 每个弟子是完整的 PeopleData |
| NPC 任务数据 | 10KB-50KB | 任务状态、对话记录 |

### 案例：竞技场宗门数据

**问题**：`AllOtherZongMenData` 在游戏启动时生成 2000+ 个宗门，每个宗门包含 4 个 `PeopleData`，导致存档达到 2MB。

**分析**：
```csharp
// MatchManager.GenerateAllZongMen()
for (int i = 0; i < 2000; i++)  // 2000 次循环
{
    // 每次生成 1-4 个宗门
    for (int n = 0; n < num; n++)
    {
        SingleOtherZongMenData theData = new SingleOtherZongMenData();
        // 每个宗门 4 个 PeopleData
        for (int j = 0; j < 4; j++)
        {
            PeopleData p = new PeopleData();
            // ... 大量属性初始化
        }
    }
}
```

**解决方案**：
```csharp
// GameInfo.cs - 标记为不序列化
[NonSerialized]
public OtherZongMenData AllOtherZongMenData;

// RoleManager.cs - 只初始化容器，不生成数据
public void InitAllOtherZongMenData(bool newPlayer)
{
    if (_CurGameInfo.AllOtherZongMenData == null)
    {
        _CurGameInfo.AllOtherZongMenData = new OtherZongMenData();
    }
    // 不再调用 GenerateAllZongMen()
}
```

## 优化策略

### 策略一：[NonSerialized] 排除

适用于：可以在运行时重新生成的数据

```csharp
[NonSerialized]
public SomeLargeData largeData;
```

**注意**：加载存档后需要重新初始化这些字段。

### 策略二：延迟生成

适用于：不是每次游戏都需要的数据

```csharp
// 不在启动时生成，在需要时才生成
public void EnsureDataGenerated()
{
    if (data == null || data.Count == 0)
    {
        GenerateData();
    }
}
```

### 策略三：精简数据结构

适用于：必须保存但数据量大的情况

```csharp
// 原始：保存完整 PeopleData
public List<PeopleData> pList;

// 优化：只保存必要字段
public List<SimpleP> simplePList;

[Serializable]
public class SimpleP
{
    public string name;
    public int gender;
    public ulong onlyId;
    public List<int> portraitIndexList;
}
```

### 策略四：按需加载

适用于：大量历史数据

```csharp
// 只保存最近 N 条记录
public List<RecordData> recentRecords;  // 最多 100 条

// 历史记录单独存储，按需加载
[NonSerialized]
public List<RecordData> historyRecords;
```

## 存档性能优化

### 同步 vs 异步

当前使用同步保存：
```csharp
ES3.Save<GameInfo>(key, gameInfo, path, settings);
```

如需异步保存（避免卡顿）：
```csharp
// 注意：异步保存期间数据可能变化，需要深拷贝或锁定
ES3.SaveAsync<GameInfo>(key, gameInfo, path, settings);
```

### 减少保存频率

```csharp
// 避免频繁保存
private float lastSaveTime;
private const float SAVE_INTERVAL = 5f;

public void TrySave()
{
    if (Time.time - lastSaveTime < SAVE_INTERVAL)
        return;
    SaveArchive();
    lastSaveTime = Time.time;
}
```

## 排查存档大小问题

### 步骤一：定位大字段

1. 在 `SaveArchive` 前后添加日志，记录各字段的大致大小
2. 或使用 JSON 序列化临时输出，分析各字段占比

### 步骤二：分析数据结构

检查 `GameInfo` 中的 `List<T>` 字段：
- 列表有多少元素？
- 每个元素有多大？
- 是否有嵌套列表？

### 步骤三：评估必要性

对于大字段，问：
- 这个数据必须保存吗？
- 能否在运行时重新生成？
- 能否只保存关键字段？

## 相关文件

- `Assets/Scripts/Data/GameInfo.cs` - 存档主数据类
- `Assets/Scripts/Manager/ArchiveManager.cs` - 存档管理器
- `Assets/Scripts/Manager/RoleManager.cs` - 数据初始化
- `Assets/Scripts/Manager/MatchManager.cs` - 竞技场数据生成
- `Assets/Scripts/Data/ZongMenData.cs` - 宗门数据结构

## 修改记录

### 2026-01-15：移除竞技场宗门数据序列化
- `GameInfo.AllOtherZongMenData` 添加 `[NonSerialized]`
- `RoleManager.InitAllOtherZongMenData` 不再自动生成宗门
- 存档大小从 2MB 降至约 100KB（新存档）
