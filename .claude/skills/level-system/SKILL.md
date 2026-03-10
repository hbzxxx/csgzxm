# 关卡系统 (Level System)

## 概述
本项目包含两套关卡系统：普通世界（固定关卡）和里世界（裂隙关卡），它们共享相同的数据结构但使用不同的初始化和管理逻辑。

---

## 1. 关卡状态枚举

```csharp
public enum AccomplishStatus
{
    Locked = 0,         // 锁定，不可挑战
    UnAccomplished = 1, // 解锁但未通关，可挑战
    Accomplished = 2    // 已通关
}
```

---

## 2. 关卡类型区分

| 类型 | 数据列表 | 初始化方法 | 场景类型 | 查找方法 |
|------|----------|------------|----------|----------|
| 普通世界（固定关卡） | `SingleMapData.FixedLevelList` | `MapManager.InitAllFixedLevel()` | `MapSceneType.Fixed` | `FindFixedLevelById()` |
| 里世界（裂隙关卡） | `SingleMapData.LevelList` | `MapManager.InitAllLieXiLevel()` | `MapSceneType.XianMen` | `FindLevelById()` |

---

## 3. 关卡配置表字段 (LevelSetting)

| 字段 | 值 | 含义 |
|------|-----|------|
| `IsFirst` | `"1"` | 首关，默认解锁 |
| `IsFirst` | `""` | 非首关，需要前置关卡通关才能解锁 |
| `ForeLevel` | 关卡ID | 前置关卡 ID |
| `NextLevel` | 关卡ID | 下一关卡 ID |
| `IsEndLevel` | `"1"` | 最终关卡 |
| `IsFixed` | `"1"` | 固定关卡（普通世界） |
| `BelongMap` | 地图ID | 所属地图 |
| `Leveltype` | 枚举值 | 关卡类型（Battle/JingYingBattle/ZhongZhuanZhan/BossBattle） |

---

## 4. 关卡解锁逻辑

### 4.1 解锁判断方法
```csharp
// 裂隙关卡
MapManager.Instance.CheckIfUnlockLevel(mapId, levelId)
MapManager.Instance.CheckIfUnlockLevel(levelId)

// 固定关卡
MapManager.Instance.CheckIfUnlockFixedLevel(mapId, levelId)
MapManager.Instance.CheckIfUnlockFixedLevel(levelId)
```

### 4.2 解锁规则
1. **首关**（`IsFirst == "1"`）→ 直接解锁
2. **非首关** → 检查前置关卡（`ForeLevel`）是否已通关（`Accomplished`）

### 4.3 正确的状态初始化代码
```csharp
// 正确写法
if (levelSetting.IsFirst == "1")
{
    singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
}
else
{
    singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
}

// 错误写法（会导致所有非首关都解锁）
if (levelSetting.IsFirst == "")  // ❌ 判断条件写反了
{
    singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
}
```

---

## 5. 老存档兼容性处理

### 5.1 问题背景
新存档的关卡状态在 `RoleManager.InitAllMap()` 中初始化并保存到存档。老存档的关卡状态已持久化，如果初始化逻辑有 bug，老存档中的错误状态不会自动修复。

### 5.2 解决方案
在进入地图时（`InitAllFixedLevel` / `InitAllLieXiLevel`）重新校验并修正状态：

```csharp
public void InitAllFixedLevel(int MapId)
{
    SingleMapData map = FindMapById(MapId);

    for (int i = 0; i < map.FixedLevelList.Count; i++)
    {
        SingleLevelData singleLevelData = map.FixedLevelList[i];
        LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

        // 只处理未通关的关卡（已通关的保持不变）
        if (singleLevelData.LevelStatus != (int)AccomplishStatus.Accomplished)
        {
            if (levelSetting.IsFirst == "1")
            {
                // 首关解锁
                singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
            }
            else
            {
                // 非首关：检查前置关卡是否已通关
                if (!string.IsNullOrWhiteSpace(levelSetting.ForeLevel))
                {
                    SingleLevelData foreLevel = FindFixedLevelById(levelSetting.ForeLevel);
                    if (foreLevel != null && foreLevel.LevelStatus == (int)AccomplishStatus.Accomplished)
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                    }
                    else
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    }
                }
                else
                {
                    singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                }
            }
        }

        BattleManager.Instance.GenerateMainLevelEnemy(singleLevelData);
    }
}
```

---

## 6. 关键文件

| 文件 | 职责 |
|------|------|
| `Assets/Scripts/Manager/RoleManager.cs` | 新存档初始化 `InitAllMap()` |
| `Assets/Scripts/Manager/MapManager.cs` | 进入地图时初始化、关卡解锁判断、关卡完成处理 |
| `Assets/Scripts/UIPanel/SingleLevelView.cs` | 关卡 UI 点击逻辑 |
| `Assets/Scripts/UIPanel/SingleMapScenePanel.cs` | 地图场景面板 |
| `Assets/Scripts/UIPanel/SingleMapView.cs` | 世界地图中的单个地图视图 |
| `Assets/Scripts/UIPanel/WorldMapPanel.cs` | 世界地图面板（切换普通世界/里世界） |
| `Assets/Scripts/Data/MapData.cs` | 地图和关卡数据结构 |

---

## 7. 常见问题排查

### 7.1 关卡可以跳关挑战
**可能原因**：
1. `RoleManager.InitAllMap()` 中 `IsFirst` 判断条件写反
2. `InitAllFixedLevel` / `InitAllLieXiLevel` 没有重置老存档的错误状态

**排查步骤**：
1. 检查 `RoleManager.cs` 中关卡状态初始化逻辑
2. 检查 `MapManager.cs` 中进入地图时的状态重置逻辑
3. 确认 `SingleLevelView.cs` 中点击时有 `LevelStatus == Locked` 的判断

### 7.2 关卡状态不正确
**排查步骤**：
1. 确认配置表中 `IsFirst`、`ForeLevel`、`NextLevel` 字段正确
2. 检查 `AccomplishLevel` / `AccomplishFixedLevel` 是否正确解锁下一关
3. 检查存档数据是否被正确保存和加载

---

## 8. 命名约定

- **裂隙/里世界**：代码中使用 `LieXi` 命名（如 `InitAllLieXiLevel`、`LieXiMapStatus`）
- **固定关卡/普通世界**：代码中使用 `Fixed` 命名（如 `FixedLevelList`、`InitAllFixedLevel`）
