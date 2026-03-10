# NPC任务剧情引导配置表系统

## 概述

NPC任务系统原使用Unity ScriptableObject（SO文件）存储数据，现已迁移至Luban配置表系统。

## 数据来源

原始数据位于 `Assets/Resources/NPCs/` 目录下的 `.asset` 文件（10000-10009）。

## 配置表结构

### 1. npc.xlsx - NPC基础信息表

| 字段 | 类型 | 说明 |
|-----|------|------|
| id | int | NPC ID |
| name | string | 名称 |
| initAdd | bool | 是否初始添加 |
| gender | int | 性别 |
| porTraitName | string | 立绘图片名 |
| smallPeopleTextureName | string | 小人图片名 |
| des | string | 描述 |
| npcType | int | NPC类型 |
| enemyId | int | 敌人ID |

### 2. npcTask.xlsx - NPC任务表

| 字段 | 类型 | 说明 |
|-----|------|------|
| id | string | 任务ID（格式：npcId + 4位taskIndex） |
| npcId | int | 所属NPC ID |
| taskIndex | int | 任务索引 |
| block | bool | 是否禁用 |
| tagId | string | 标志性ID |
| trackingName | string | 统计名字 |
| npcPos | int | NPC出现位置 |
| taskGuideId | int | 任务引导ID |
| taskType | int | 任务类型 |
| taskRepeatType | int | 重复性质 |
| repeatTime | int | 重复时间 |
| isNeedItem | bool | 是否需要物品 |
| needReceiveItem | bool | 是否回收物品 |
| needItem | int | 需要物品ID |
| needNum | int | 需要数量 |
| isNeedKillEnemy | bool | 是否需要杀敌 |
| needKillEnemy | int | 需要杀敌ID |
| needKillEnemyNum | int | 需要杀敌数量 |
| isNeedDanFarmNum | bool | 是否需要丹炉数量 |
| needDanFarmId | int | 需要丹炉ID |
| needDanFarmNum | int | 需要丹炉数量 |
| needPlayerLevel | int | 需要境界等级 |
| commonAccomplishCondition | string | 通用完成条件 |
| des | string | 任务描述 |
| awardList | string | 奖励列表（分号分隔） |
| ifAppearInform | bool | 出现是否提示 |
| appearTxt | string | 出现提示文字 |
| ifDialogBefore | bool | 是否前置对话 |
| ifDialogAfter | bool | 是否后置对话 |

### 3. npcTaskCondition.xlsx - 任务条件表

| 字段 | 类型 | 说明 |
|-----|------|------|
| id | int | 条件ID |
| taskId | string | 所属任务ID |
| conditionIndex | int | 条件索引 |
| conditionType | int | 条件类型 |
| param | string | 条件参数 |

### 4. npcTaskDialog.xlsx - 任务对话表

| 字段 | 类型 | 说明 |
|-----|------|------|
| id | int | 对话ID |
| taskId | string | 所属任务ID |
| dialogIndex | int | 对话索引 |
| dialogPhase | string | 对话阶段（before/after） |
| belongType | int | 归属类型 |
| content | string | 对话内容 |
| ifPlayerAnswer | bool | 是否玩家回复 |
| answerList | string | 回复选项（分号分隔） |

## 数据统计

| 表 | 记录数 |
|---|--------|
| npc | 10 |
| npcTask | 166 |
| npcTaskCondition | 166 |
| npcTaskDialog | 374 |

### 各NPC任务数量

| NPC ID | 名称 | 任务数 |
|--------|------|--------|
| 10000 | 旁白 | 1 |
| 10001 | 云海宗杂役 | 0 |
| 10002 | 神秘少女 | 50 |
| 10003 | 混混头子 | 0 |
| 10004 | 云海宗门人 | 0 |
| 10005 | 云海宗长老 | 0 |
| 10006 | 帝殊 | 73 |
| 10007 | 神算子 | 0 |
| 10008 | 山贼 | 1 |
| 10009 | 苏梦岚 | 41 |

## 工具脚本

### 解析SO文件生成CSV

```bash
python Tools/parse_npc_simple.py
```

### CSV转Luban Excel

```bash
python Tools/csv_to_luban_excel.py
```

## 文件位置

- SO源文件：`Assets/Resources/NPCs/*.asset`
- CSV中间文件：`Luban/Config/Datas/npc*.csv`
- Luban配置表：`Luban/Config/Datas/npc*.xlsx`
- 解析脚本：`Tools/parse_npc_simple.py`
- 转换脚本：`Tools/csv_to_luban_excel.py`

## 已完成迁移（2026-01-13）

1. [已完成] 在 `__tables__.xlsx` 中注册4个新表
2. [已完成] 运行Luban生成C#代码（NPCSetting、NPCTaskSetting、NPCTaskConditionSetting、NPCTaskDialogSetting）
3. [已完成] 修改 `DataTable.cs` 的 `LoadSOData` 方法改为从配置表加载

### 代码实现说明

`DataTable.cs` 中新增以下方法：

- `LoadSOData()` - 入口方法，调用 `LoadNPCDataFromTable()`
- `LoadNPCDataFromTable()` - 从 TbNPC 加载 NPC 基础信息，构建 `NPC[]` 数组
- `LoadNPCTasks(int npcId)` - 从 TbNPCTask 加载指定 NPC 的任务列表
- `LoadTaskConditions(string taskId)` - 从 TbNPCTaskCondition 加载任务条件
- `LoadTaskDialogs(string taskId, string phase)` - 从 TbNPCTaskDialog 加载对话列表
- `ParseStringList(string str)` - 解析分号分隔的字符串

### API 兼容性

保留了原有 API 不变，现有业务代码无需修改：
- `DataTable.NPCArr` - NPC 配置数组
- `DataTable.FindNPCArrById(int id)` - 根据 ID 查找 NPC 配置

### 数据流

```
Luban配置表 → LoadSOData() → NPC[] 数组 → 业务代码（TaskManager、RoleManager等）
```

## 待清理事项

- 确认功能正常后，可删除 `Assets/Resources/NPCs/*.asset` 文件

## 注意事项

- 任务ID格式：`{npcId}{taskIndex:04d}`，如 `100020049` 表示NPC 10002的第49个任务
- 奖励列表和回复选项使用分号 `;` 分隔
- 对话内容中的换行符已转换为 `\n` 或 `\r\n`

## 任务系统核心流程

### 任务激活流程

```
CheckIfNPCAppear() → 检查任务条件 → ChoosedTask() → TryAccomplishTask()
```

1. `CheckIfNPCAppear()` 遍历所有 NPC，检查是否有满足条件的任务
2. `CheckIfSatisfyCondition()` 检查任务的前置条件（npcTaskCondition 表）
3. `ChoosedTask()` 激活任务，显示 NPC，调用 `TryAccomplishTask()`
4. `TryAccomplishTask()` 根据 taskType 检查任务完成条件

### 任务完成条件（taskType）

| taskType | 枚举名 | 完成条件 |
|----------|--------|----------|
| 5 | BuildDanFarm | 建造指定数量的丹房 |
| 6 | ZhaoMuDiZi | 招募指定数量弟子 |
| 15 | UpgradeZongMen | 宗门等级达到指定值 |
| 17 | ZuoZhen | 弟子坐镇建筑 |
| 18 | UpgradeDanFarm | 升级丹房到指定等级 |
| 46 | ChangeZongMenName | 宗门名不等于"临时宗门" |

### 关键时序问题

**新存档初始化顺序**（Game.cs StartGame）：

```csharp
// 正确顺序（2026-01-15 修复）
if (新存档) {
    // 先打开 SetNamePanel，由 OnConfirm 调用 CheckIfNPCAppear
    OpenPanel<SetNamePanel>();
} else {
    // 老存档直接检查 NPC
    CheckIfNPCAppear();
}
```

**错误顺序会导致的问题**：
- 如果先调用 `CheckIfNPCAppear()`，再打开 `SetNamePanel`
- 此时 `ZongMenName` 是空字符串 `""`（新存档初始值）
- 空字符串不等于 `"临时宗门"`，导致 taskType=46 的任务被误判为完成

### 数据初始值

| 数据 | 初始值 | 设置时机 |
|------|--------|----------|
| ZongMenData.ZongMenName | `""` (空字符串) | RoleManager.InitAllZongmenData |
| 设置为"临时宗门" | `"临时宗门"` | SetNamePanel.OnConfirm → SetInitZongMenName() |

### 任务条件表（npcTaskCondition）

- 如果任务在 npcTaskCondition 表中没有记录，则该任务没有前置条件
- 没有前置条件的任务会在 `CheckIfNPCAppear()` 时立即被激活

## 任务完成后特殊逻辑（tagId 触发）

### 执行时机与条件

任务完成后的特殊逻辑通过 `tagId` 触发，位于 `ChoosedTask()` 方法的 `AccomplishStatus.Accomplished` 分支内。

**关键限制**：`tagId` 触发的逻辑只在 `dialogList.Count > 0` 时执行，即任务必须有**后置对话**（`dialogListAfter`）才会执行。

```csharp
// TaskManager.cs ChoosedTask() 中的结构
if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
{
    // ... 发放奖励 ...
    
    if (dialogList.Count > 0)
    {
        DialogManager.Instance.CreateDialog(dialogList, () =>
        {
            // tagId 触发的特殊逻辑在这里
            if (tagId == "10002_14") { ... }
            if (tagId == "10002_4") { ... }
        });
    }
    else
    {
        // 没有后置对话时，直接消失 NPC，跳过 tagId 逻辑
        OnDisappearNPC(singleNPCData.OnlyId);
    }
}
```

### 常见问题：tagId 逻辑不执行

**问题现象**：配表中设置了 `ifDialogAfter=true`，但对话表中没有对应的 `after` 对话记录，导致 `dialogListAfter` 为空，`tagId` 逻辑被跳过。

**排查步骤**：
1. 检查 npcTask.xlsx 中的 `ifDialogAfter` 字段
2. 检查 npcTaskDialog.xlsx 中是否有对应 `taskId` 且 `dialogPhase='after'` 的记录
3. 两者必须一致，否则逻辑不会执行

**案例**：任务 100020013（建造炼器房）的 `tagId="10002_14"` 逻辑原本用于让弟子感悟值满，但因为没有 `after` 对话记录而被跳过。

### 解决方案：任务激活时处理

对于需要在任务激活时执行的特殊逻辑（而非任务完成后），应在 `ChoosedTask()` 方法的 switch 语句中处理：

```csharp
// TaskManager.cs ChoosedTask() 中
switch (taskSOSetting.taskType)
{
    case TaskType.ZhaoMuDiZi:
        // 招募弟子任务激活时，生成候选弟子
        if (RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Count == 0)
        {
            StudentManager.Instance.RdmGenerate3CandidateStudents(GenerateCandidateStudentType.AD);
        }
        break;
    case TaskType.TianFuJueXingNum:
        // 天赋觉醒任务激活时，确保有弟子感悟值满
        EnsureStudentExpFull();
        break;
}
```

这种方式更可靠，不依赖对话配置。

## 弟子感悟值系统

### 感悟值满的判定

- 未觉醒弟子：`talent == StudentTalent.None && studentCurExp >= 120`
- 已觉醒弟子：`talent != StudentTalent.None && studentCurExp >= 120`

### 相关任务条件类型

| conditionType | 枚举名 | 说明 |
|---------------|--------|------|
| 7 | RareStudentExpFull | 有未觉醒弟子感悟值满 |
| 8 | StudentExpFull | 有已觉醒弟子感悟值满 |

### 让弟子感悟值满

```csharp
StudentManager.Instance.FullExp(PeopleData p);
```

### EnsureStudentExpFull 方法

`TaskManager.EnsureStudentExpFull()` 用于确保有弟子感悟值满：
1. 检查是否已有未觉醒弟子感悟值>=120
2. 没有则让第一个未觉醒弟子感悟值满

### 老存档兼容：Init 中检查天赋觉醒任务

**问题场景**：老存档中主线 NPC 已显示，天赋觉醒任务已激活，但弟子感悟值已被消耗（觉醒或其他操作），导致任务条件不满足。

**原因分析**：
- `EnsureStudentExpFull()` 只在任务激活时（`ChoosedTask`）调用
- 老存档加载时，NPC 已在 `CurShowNPCOnlyIdList` 中，`CheckIfNPCAppear()` 不会进入条件检查分支
- 任务条件检查 `CheckIfSatisfyCondition()` 只做判断，不会主动补充感悟值

**解决方案**：在 `TaskManager.Init()` 中，对已显示的主线 NPC 检查当前任务是否为天赋觉醒任务，如果是则调用 `EnsureStudentExpFull()`：

```csharp
// TaskManager.Init() 中
if (setting.npcType == NPCType.None)
{
    curMainlineNPCData = data;
    if (FindTaskById(curMainlineNPCData, curMainlineNPCData.CurTaskId) == null)
    {
        OnDisappearNPC(curMainlineNPCData.OnlyId);
    }
    else
    {
        //老存档兼容：已显示的主线NPC，检查当前任务是否需要感悟值满条件
        SingleTask curTaskSetting = FindTaskSettingById(setting, data.CurTaskId);
        if (curTaskSetting != null && curTaskSetting.taskType == TaskType.TianFuJueXingNum)
        {
            EnsureStudentExpFull();
        }
    }
}
```

**调用时机**：
- 游戏启动时 `TaskManager.Init()` 被调用
- 此时检查已显示的主线 NPC 的当前任务
- 如果是天赋觉醒任务，确保有弟子感悟值满
