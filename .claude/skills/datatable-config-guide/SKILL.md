# 配置表系统指南

## 主要配置表目录

| 目录 | 说明 |
|------|------|
| `Luban/Config/Datas/` | **源Excel配置表**（主要编辑位置） |
| `Assets/StreamingAssets/res/DataTable/` | 导出的加密JSON数据（运行时读取） |
| `Assets/Scripts/Gen/` | Luban生成的C#代码（自动生成，勿手动修改） |

## 配置表加载逻辑

- 代码位置: `Assets/Scripts/Tools/DataTable.cs`
- JSON文件使用DES加密，密钥在 `ConstantVal.mm`
- 通过 `cfg.Tables` 类访问所有配置表

## 常用配置表对应关系

| 功能 | Excel文件 | 生成的类 |
|------|-----------|----------|
| 签到 | `qianDao.xlsx` | `TbQianDao` / `QianDaoSetting` |
| 物品 | `item.xlsx` | `TbItem` / `ItemSetting` |
| 商店 | `shop.xlsx` | `TbShop` / `ShopSetting` |
| 技能 | `skill.xlsx` | `TbSkill` / `SkillSetting` |
| 关卡 | `level.xlsx` | `TbLevel` / `LevelSetting` |
| 任务 | `task.xlsx` | `TbTask` / `TaskSetting` |
| 装备 | `equipment.xlsx` | `TbEquipment` / `EquipmentSetting` |
| NPC | `npc.xlsx` | `TbNPC` / `NPCSetting` |
| 地图 | `map.xlsx` | `TbMap` / `MapSetting` |
| 宝石 | `gem.xlsx` | `TbGem` / `GemSetting` |
| 抽奖 | `chouJiang.xlsx` | `TbChouJiang` / `ChouJiangSetting` |
| 新手引导 | `newGuide.xlsx` | `TbNewGuide` / `NewGuideSetting` |
| 建筑升级 | `buildingUpgrade.xlsx` | `TbBuildingUpgrade` / `BuildingUpgradeSetting` |
| 累充 | `leiChong.xlsx` | `TbLeiChong` / `LeiChongSetting` |

## 修改配置表流程

1. 编辑 `Luban/Config/Datas/` 下的Excel文件
2. 运行Luban导表工具生成加密JSON和C#代码
3. 加密后的JSON会输出到 `Assets/StreamingAssets/res/DataTable/`

## 代码中访问配置表

```csharp
// 通过 DataTable 静态方法访问
var itemSetting = DataTable.FindItemSetting(10001);
var qianDaoSetting = DataTable.FindQianDaoSetting(10001);

// 或直接通过 Tables 访问
var setting = DataTable.table.TbItem.Get("10001");
var list = DataTable.table.TbItem.DataList;
```

## 奖励格式

配置表中的奖励字段通常使用以下格式：
- 单个奖励: `物品ID|数量`
- 多个奖励: `物品ID|数量$物品ID|数量$...`

例如: `10001|5000$365001|10` 表示 5000金币 + 10个365001物品
