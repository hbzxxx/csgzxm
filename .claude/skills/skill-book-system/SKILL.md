# 技能书/功法书系统

## 核心概念

### ID 对应关系
- **技能ID** (`SkillSetting.Id`) 与 **技能书物品ID** (`ItemSetting.Id`) 是相同的
- 例如：技能ID 440001 对应物品ID 440001 的技能书
- 通过 `DataTable.FindSkillSetting(setting.Id.ToInt32())` 可从物品获取技能配置

### CanStudy 字段
- `SkillSetting.CanStudy`：标识该技能是否可通过功法书学习
- **非空** = 有对应的功法书物品，可学习
- **空字符串** = 无对应功法书，不可通过物品学习（可能是系统技能、怪物技能等）

### 物品类型
- `ItemType.SkillBook = 14`：技能书
- `ItemType.GongFaShu = 18`：功法书（神秘技能书/黑盒技能书）

## 关键方法

### DataTable.FindCanStudySkillListByYuanSu
```csharp
// 按元素获取可学习技能列表
public static List<SkillSetting> FindCanStudySkillListByYuanSu(YuanSuType yuanSu)
{
    List<SkillSetting> res = new List<SkillSetting>();
    foreach (var setting in table.TbSkill.DataList)
    {
        // 必须同时满足：元素匹配 且 CanStudy 非空
        if (setting.YuanSu.ToInt32() == (int)yuanSu && !string.IsNullOrWhiteSpace(setting.CanStudy))
            res.Add(setting);
    }
    return res;
}
```

### 正确的 CanStudy 检查
```csharp
// 正确：检查 CanStudy 非空
if (!string.IsNullOrWhiteSpace(setting.CanStudy))
    candidateList.Add(setting);

// 错误：只按元素筛选，可能返回无对应物品的技能
if (setting.YuanSu.ToInt32() == (int)yuanSu)
    candidateList.Add(setting);
```

## 常见错误

### 1. 技能ID当物品ID使用但物品不存在
```csharp
// 危险：如果技能没有对应的功法书物品，会报错
int skillId = DataTable.FindCanStudySkillListByYuanSu(yuanSu)[0].Id.ToInt32();
ItemManager.Instance.GetItemWithTongZhiPanel(skillId, 1);  // 物品不存在！
```

**根因**：`FindCanStudySkillListByYuanSu` 没有检查 `CanStudy` 字段，返回了没有对应物品的技能。

### 2. 发放功法书奖励前的检查
```csharp
// 安全做法：先确认技能有对应物品
List<SkillSetting> canStudyList = DataTable.FindCanStudySkillListByYuanSu(yuanSu);
if (canStudyList.Count > 0)
{
    int skillId = canStudyList[0].Id.ToInt32();
    ItemManager.Instance.GetItemWithTongZhiPanel(skillId, 1);
}
```

## 使用场景

### 任务奖励发放功法书
```csharp
// TaskManager.cs - 引导学习技能
if (curTaskSetting.tagId == "10002_10")
{
    var skillList = DataTable.FindCanStudySkillListByYuanSu((YuanSuType)playerPeople.yuanSu);
    if (skillList.Count > 0)
    {
        ItemManager.Instance.GetItemWithTongZhiPanel(skillList[0].Id.ToInt32(), 1);
    }
}
```

### 从仓库查找技能书
```csharp
// ItemManager.FindCangKuSkillBookItem
foreach (var item in cangKuItemDataList)
{
    ItemSetting setting = DataTable.FindItemSetting(item.settingId);
    if (setting.ItemType.ToInt32() == (int)ItemType.SkillBook)
    {
        // 物品ID和技能ID相同，可直接查找技能配置
        SkillSetting skillSetting = DataTable.FindSkillSetting(setting.Id.ToInt32());
        if (skillSetting.YuanSu.ToInt32() == p.yuanSu)
            candidateList.Add(item);
    }
}
```

## 检查清单

涉及技能书/功法书时：
- [ ] 是否检查了 `CanStudy` 非空？
- [ ] 是否确认技能ID有对应的物品？
- [ ] 发放奖励前是否检查列表非空？
