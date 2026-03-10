---
inclusion: always
---

# 项目代码规则（rule.md）

> 本文件为本仓库 Copilot Chat/Inline Chat 的权威规则来源。修改后无需额外操作，后续对话将默认参考本文件（已在工作区设置为默认指令）。

## 🔄 规则同步机制

**重要**：`.kiro/steering/` 目录是所有规则的唯一源，任何规则更新都必须保持同步。

### 规则文件位置
- **主规则源**：`.kiro/steering/` - 所有规则文件的唯一真实来源
- **Cursor 入口**：`.cursorrules` - 指向 `.kiro/steering/rule.md`
- **Windsurf 入口**：`.windsurfrules` - 指向 `.kiro/steering/rule.md`

### 规则更新流程
当修改任何规则文件时：
1. **只修改 `.kiro/steering/` 中的源文件**
2. **无需手动同步** - Cursor 直接读取 `.kiro/steering/`
3. **禁止创建副本** - 不要在其他位置创建规则文件的副本

### 规则文件清单
- `rule.md` - 主规则文件（本文件）
- `communication-style.md` - 沟通风格规范
- `code-reusability-and-documentation.md` - 代码复用与文档规范
- `change-documentation.md` - 变更文档规范
- `fact-based-consultation.md` - 基于事实的咨询规范
- `incremental-document-writing.md` - 增量文档写入策略

---

## 1. 通用原则
- 回答语言：除非用户明确要求其它语言或上下文必须，所有对话与说明文本一律使用简体中文；代码保持原始语言与标识符不强行翻译。
- 优先检查当前窗口：当用户要求修改代码时，优先查看用户当前打开/聚焦的文件，避免只搜索到其他文件中已完成的改动而误判"已完成"。同名改动可能存在于多个脚本中，必须逐一确认当前窗口文件的状态。
- 如无必要，勿增实体：
  - **禁止冗余代码**：不要随意添加兜底代码（过度判空、默认值回退、try/catch 吞错）；不要预留"可能用到"的备用方法；开发者未明确要求时不自作主张增加防御性代码。
  - **复用优先于新建**：有类似功能时，优先复用/扩展已有代码，避免重复造轮子产生大量雷同代码；确认现有代码无法满足需求后，才考虑新建。
  - **结构优化与拆分**：当单个脚本超过 1000 行时，说明可能过于臃肿，应通过 OOP 手段（提取类、组合、继承等）拆分，追求高内聚、低耦合。
  - **核心思想**："实体"指无意义的冗余代码，而非必要的模块拆分。该拆分时拆分，该复用时复用；不该加的防御代码、备用方法、重复实现才是需要避免的"实体"。
- 现有配置复用优先：涉及 linker/link.xml 等配置文件时，优先在既有文件（如 `Assets/HybridCLRGenerate/link.xml`）中追加或调整条目，非必要不要新建平行的 xml 以免失效或难以维护。
- 优先遵循本文件；当与临时需求冲突时，先在回复里指出冲突并请求确认。
- 保持改动最小化，遵循现有风格与架构。
- 所有新增/变更必须具备必要注释与可读性。
- 规则维护：除非用户明确要求，不要自行新增或修改本规则文件；如认为有必要补充，请先征询确认。

## 2. 命名规范
- C#：PascalCase（类型/公开成员），camelCase（私有字段/局部变量），常量 UPPER_SNAKE_CASE。
- Unity 资源/预制体：与脚本类名一致或前缀清晰（可在此补充）。
 - 对象池枚举命名强制规则：`ObjectPoolSingle` 中每个枚举值必须与其对应的脚本类名、预制体名三者完全一致（区分大小写）。例如：`ExpOrbView` 枚举 ↔ 预制体 `ExpOrbView.prefab` ↔ 脚本类 `ExpOrbView`。禁止出现枚举与类、预制体不匹配的情况，避免对象池路径解析与动态创建失败。

## 3. 异常与日志
 - **严禁使用 try/catch**：任何情况下都不允许使用 try/catch 包裹代码，必须让错误直接暴露以便第一时间发现和定位问题。
 - 禁止兜底代码：避免过度判空、默认值回退等"防止报错"的写法；不要掩盖问题，让错误尽早暴露。
   - **典型违规案例**：`if (value < 0) value = 0.2f;` 这种"负值回退默认值"属于兜底代码，会掩盖配置错误或计算 bug，必须删除。
 - 出现潜在错误场景时，优先用 Debug.LogError 明确打印关键信息并中止当前流程（按需 return），以便第一时间在编辑器看到并定位。

## 4. Unity/游戏相关
- **禁止使用 `AddComponent`**：热更框架对运行时动态添加组件可能存在兼容性问题，所有组件必须在预制体上预先配置，禁止代码中调用 `gameObject.AddComponent<T>()`。
- 禁止在 `OnDisable`、`OnDestroy`、`Awake` 等 Unity 生命周期钩子中编写任何时序敏感的判定逻辑（如阶段切换、胜负判定、数据结算等）。
- 所有时序相关判定必须来源清晰且唯一，优先集中在 Manager/主流程显式调用的函数中，避免因对象池回收或生命周期自动触发导致判定提前或错乱。
- 如确需在钩子中做清理，仅限于资源释放、事件解绑、缓存移除，不得影响主流程状态。
- MonoBehaviour 生命周期内避免重分配与反射；缓存组件引用。
- UI 更新走集中刷新方法，避免散落逻辑。
- 配置驱动优先：所有可配置项从 DataTable 读取。
 - 战斗属性归一：角色战斗期的派生属性统一在 `PropertyManager.RefreshProLimit(PeopleData p)` 内计算与更新（例如：`PeopleData.totalDamageAdd` 由装备类型增伤 DamagePlus 聚合得到）。禁止在其他 Manager/Panel 中重复计算或缓存这些派生值，避免时序不一致与双重结算。
 - UI/面板对象池复用：所有继承自 PanelBase 与 SingleViewBase 的对象默认通过预制体对象池复用；生命周期内任何可能被改动的字段（组件状态、事件监听、协程/定时器、子物体激活、文本/图片、列表与缓存集合等）必须在 Clear()/OnClear 钩子中统一清空与重置；Init/OnOpenIng 只做显式设置，不依赖上次状态。
   - **对象池状态重置强制检查清单**（新增字段时必查）：
     - [ ] Transform 状态（localScale、localEulerAngles、localPosition）→ 恢复初始值
     - [ ] 逻辑标志（bool、enum）→ 重置为初始状态（如 `dead=false`、`isAiming=false`）
     - [ ] pending 请求标志 → 全部置 false（如 `pendingAimRequest=false`）
     - [ ] 计时器/计数器 → 归零
     - [ ] 引用缓存（目标、路径等）→ 置 null 或 Clear()
     - [ ] 集合（List、Dict）→ Clear()
     - [ ] 组件状态（颜色、材质、enabled）→ 恢复默认
     - [ ] Spine/动画状态 → 重新初始化或切到 Idle
     - [ ] 状态机实例引用 → 置 null
   - **典型遗漏案例**：英雄死亡后 `dead=true`、`iconParent` 旋转未还原，下次进战斗表现异常。
 - 协程默认禁用：非特别指示不要使用协程；如需延时/轮询，用计时器（基于 Update/定时管理器）的方式替代，确保易终止、可复用且不泄漏。
 - **状态机请求是延迟的**：`RequestXxxState()`（如 `RequestAimState`、`RequestAttackState`、`RequestIdleState`）仅设置 pending 标记，状态切换在下一次 Tick/Update 时才真正执行。
   - **禁止假设同步切换**：调用 `RequestXxxState()` 后，不能立即假设状态已切换、`OnEnter` 已执行。
   - **跨模块协作需考虑时序**：若 A 模块调用 `RequestState()` 后，B 模块需要依赖该状态的副作用（如 `isActive` 变为 `true`），必须在 B 模块中做容错处理（如记录待处理对象，在状态真正激活后再处理）。
   - **典型案例**：`HeroView.OnStartAim()` 调用 `RequestAimState()` 后立即返回，此时 `AimState.OnEnter()` 尚未执行；若后续代码依赖 `OnEnter` 中的初始化（如 `AimHighlightManager.Activate()`），必须额外处理这种时序差。

## 5. 代码组织
- 单一职责：一个类/方法只做一件事。
- 公共 API 带简短 XML 文档注释；内部方法写行内注释说明非直观逻辑。
 - 封装优先：倾向高内聚、低耦合，小而清晰的接口；追求“简单干净”而非面面俱到的万能方法。
 - 减少直接引用：尽量减少对外部变量/字段的直接读写，优先通过封装的方法/属性访问，集中管理状态变化，避免分散副作用。
 - 抽离复用：当发现同一序列操作在多个地方出现时，立即抽离为单一函数/工具方法并复用；保持 DRY（不要重复自己）。
 - 控制扩张：优先抽到现有类/模块中，非必要不新增文件/类型，避免违背“如无必要，勿增实体”。

## 6. 性能与内存
- 少 GC：循环内避免临时 List/字符串拼接；必要时对象池。
- 大集合遍历使用 for；避免 Linq 在热路径。
 - 避免死循环：所有循环必须具备明确跳出条件或上限计数，谨慎 while(true)。

## 6.1 异步回调与闭包安全（强制）
- **禁止在异步回调中捕获可变引用**：`AddSingleAsync`、`OpenSingleAsync`、`OpenPanelAsync` 等异步方法的回调中，禁止直接使用闭包捕获的 `Monster`、`HeroView`、`PeopleData` 等可能被对象池复用的引用类型变量。
- **根因**：异步加载期间，原对象可能被对象池回收并复用给新实体，闭包捕获的引用指向的数据已变化，导致逻辑错乱。
- **正确做法**：
  1. 捕获不可变的值类型标识符（如 `ulong onlyId`）
  2. 在回调或 `Init` 中通过标识符重新查找当前有效的对象
  3. 查找失败或对象已死亡时，放弃操作或关闭视图
- **错误示例**：
  ```csharp
  // 危险：闭包捕获 p 引用，异步期间 Monster 可能被复用，p 指向的数据已变
  AddSingleAsync("HPBar", parent, (view) => {
      if (p.dead) return;  // p 可能已指向其他怪物
      view.Init(p);
  }, false, p);
  ```
- **正确示例**：
  ```csharp
  // 安全：捕获 onlyId（值类型），在 Init 中重新查找
  ulong onlyId = p.onlyId;
  AddSingleAsync("HPBar", parent, (view) => {
      // 回调中只做注册，不依赖闭包捕获的引用
  }, false, onlyId);  // 传 onlyId 而非 p
  
  // HPBarView.Init 中通过 onlyId 重新查找
  var monster = BattlePanel.Instance.FindMonsterByOnlyId(onlyId);
  if (monster == null || monster.p.dead) { Close(); return; }
  ```
- **适用场景**：所有涉及对象池复用实体（Monster、HeroView、Bullet 等）的异步加载回调。

## 7. 提交与格式
- 保持一致的缩进与风格（受 .editorconfig/IDE 约束）。
- 每次提交描述清晰：做了什么、为什么。

## 8. 事件系统
- 遇到以下关键词时，必须先读取 `event-system` skill 再作答：
  - `EventCenter`、`TheEventType`、事件订阅/广播
  - `RegisterEvent`、事件解除订阅
  - `OnBattleStateChanged`、战斗阶段事件

## 9. 运行时对象与效果
- 统一创建方式：所有运行时临时可视对象（特效、曲线点、命中提示、飘字等）一律通过 `SingleViewBase`/`PanelBase` + 预制体对象池创建（例如 `PanelManager.OpenSingle(ObjectPoolSingle.xxx, ...)`）。禁止直接 `new GameObject`/`Instantiate`。
- 统一挂载父节点：战斗场景中的效果节点统一挂到 `BattleBg.trans_effectParent` 下；若父节点为空，需 `Debug.LogError` 并中止创建，避免散落与泄漏。
- 复用与清理：
	- 在 `Init/OnOpenIng` 显式设置本次需要的状态（组件、颜色、尺寸、文本等）；
	- 在 `Clear/OnClear` 里重置全部可变状态、移除监听、停止计时器/动画并归还到对象池；
	- 不允许在局部再实现一套对象池（例如自建 dotPool）；已有全局对象池时必须复用。
- 资源与参数注入：通过 `Init/OnOpenIng` 参数传入资源（Sprite/材质/颜色/尺寸等），避免在视图内部查找全局静态或硬编码路径。TMP/字体资源遵循 TMP 规范，统一置于 `Resources/Fonts & Materials`，样式用 Material Preset 控制。
- 状态集中：跨模块共享的运行时状态必须集中管理，例如“当前操作英雄”仅由 `HeroManager.currentOperateHero` 持有；其他模块通过封装的 `Set/Get` 访问，禁止各自缓存副本或影子状态。
- 事件与生命周期：临时效果默认不订阅全局事件；确有需要时，使用视图基类提供的 `RegisterEvent(...)` 以便由面板/视图生命周期自动解除订阅，禁止手动长期持有引用导致泄漏。
- 示例参考：`CurveDotView` 作为曲线点单项视图，使用 `ObjectPoolSingle.CurveDotView` 枚举创建；`HeroOperationManager` 在玩家操作阶段按配置生成/清理圆点，并将其统一挂到 `BattleBg.trans_effectParent`。

## 10. 英雄操作分发
- 遇到以下关键词时，必须先读取 `hero-operation-dispatch` skill 再作答：
  - `HeroOperationManager`、`IHeroOperationHandler`
  - 英雄拖拽、英雄操作处理器
  - `OnEnterOperation`、`OnExitOperation`、`StartDrag`、`UpdateDrag`、`EndDrag`

## 11. 战斗更新调度（集中驱动）
- 遇到以下关键词时，必须先读取 `battle-update-schedule` skill 再作答：
  - `BattleBg.Update`、管理器 `Tick`
  - 战斗每帧更新、集中调度
  - 管理器初始化归位、`LevelManager.EnterLevel`

## 12. Manager 架构原则（不订阅事件）
- Manager 仅负责"数据与业务状态"的修改与维护，不直接订阅/监听全局事件；必要时由 `BattleManager` 或上层流程在恰当时机直接调用 Manager 的公开方法（`Manager.Instance.SomeMethod()`）。
- Manager 间协作通过单例直接调用完成（如 `AManager.Instance.CallB()`），禁止通过事件在 Manager 之间隐式通讯，降低时序不确定性与耦合。
- 事件通知面向 UI：业务完成后，如需刷新 UI/特效，由上层（通常是 `BattleManager` 或具体视图/面板控制器）通过 `EventCenter.Broadcast(...)` 广播给 View/Panel；Manager 本体不订阅这些事件。

## 13. 资源加载规则引用
- 遇到以下关键词时，必须先读取 `resource-loading-strategy` skill（使用 `openskills read resource-loading-strategy`）再作答：
  - OpenPanel、OpenSingle、OpenPanelAsync、OpenSingleAsync
  - ObjectPoolSingle（已废弃标识）
  - 异步加载、同步加载、预制体加载
  - Loading 遮罩、showLoading
  - 对象池、PanelManager
- **禁止在异步回调中处理视图初始化逻辑**：
  - 所有视图的初始化逻辑（加载资源、设置位置、播放动画等）必须在视图自身的 `Init(params object[] args)` 或 `OnOpenIng(params object[] args)` 中处理
  - 调用方只负责传递参数，不在回调中操作视图内部状态
  - **错误示例**：在 `OpenSingleAsync` 回调中设置 `view.ske.skeletonDataAsset = xxx`
  - **正确示例**：将参数传给 `OpenSingleAsync`，由视图的 `Init` 方法处理初始化

## 14. 异步加载与战斗状态机协调
- **警惕异步加载对战斗流程的影响**：异步加载存在时间差，可能导致状态机误判（如经验球未生成完毕就判定为"已清空"）。
- 遇到以下场景时，必须先读取 `battle-state-machine` skill（使用 `openskills read battle-state-machine`）再作答：
  - 战斗中异步加载影响阶段判定的对象（经验球、能量球、怪物、英雄等）
  - `BattleState`、`BattlePhaseConditionManager`、阶段切换条件
  - 涉及 `AreExpAndEnergyOrbsCleared`、`HasPendingOrbs` 等判定逻辑
- **必要时引入待生成计数器**：异步加载开始时 +1，完成时 -1，状态判定时检查计数器，防止误判。

## 14.1 子弹追踪机制
- 遇到以下关键词时，必须先读取 `skill-bullet-tracking` skill 再作答：
  - `HeroVolleyTracker`、`batchId`、`ActionBatchId`
  - 子弹生成追踪、子弹关闭回调
  - `BeginHeroVolley`、`BeginDageSkillTracker`
  - `OnBulletSpawned`、`OnBulletClosedFromBullet`、`OnVolleySpawningCompleted`
  - `AreAllHeroBulletsCleared`、`AreAllTrackersSealed`
- **子弹追踪流程**：BeginTracker → OnUnitSpawned（每个子弹）→ OnSpawningCompleted → OnUnitClosed（子弹关闭）
- **状态机条件依赖**：`HeroAction → BuffSettlement` 和 `PlayerOperation → BuffSettlement`（大哥技能）都依赖子弹追踪条件。

## 15. OpenSkills 技能协作（强制执行 - 最高优先级）

> **此规则为强制约束，违反将导致代码质量问题。AI 必须严格执行前后询问流程。**

### 话题-skill 映射表
遇到以下话题时，**必须先询问用户是否需要查看对应 skill**，用户确认后再阅读：

| 话题关键词 | 对应 skill |
|-----------|-----------|
| 资源加载、同步/异步加载、预制体、对象池、Loading、PanelManager、OpenSingle、OpenPanel、Init参数传递、OnClear状态重置、下次进战斗状态残留 | `resource-loading-strategy` |
| 事件系统、EventCenter、订阅/广播、RegisterEvent | `event-system` |
| 属性系统、PropertyManager、RefreshProLimit、moveSpeed、curMoveSpeed、SetMoveSpeed | `property-system` |
| NPC任务、剧情引导、LoadSOData、NPC配置表、npcTask、npcTaskCondition、npcTaskDialog、SingleTask、TaskCondition | `npc-task-config-system` |
| 换皮、修为改等级、境界改等级、领主达到xx级、train表等级对照 | `reskin-principles` |
| 配置表、DataTable、Luban、Excel配置、tbItem、tbShop、tbQianDao、签到奖励、物品表、商店表 | `datatable-config-guide` |
| 关卡系统、关卡解锁、LevelStatus、里世界、裂隙、固定关卡、InitAllFixedLevel、InitAllLieXiLevel、CheckIfUnlockLevel、AccomplishStatus | `level-system` |

### 前置询问流程（实现前 - 强制）

**触发条件**：用户需求涉及上表任一话题关键词

**执行步骤**：
1. **立即暂停**：不要开始编写代码
2. **识别领域**：明确涉及哪个 skill
3. **主动询问**：使用以下格式询问用户：
   ```
   该需求涉及 [领域名称]，是否需要我先查看 `skill名称` skill？
   ```
4. **等待确认**：用户回复"是/要/需要/好"等肯定词后，再阅读 skill
5. **阅读 skill**：使用 `read_file` 工具读取对应 skill 文件
6. **遵循规范**：严格按照 skill 内容实现，禁止凭记忆或猜测

**示例对话**：
```
用户：帮我修复 OpenSingleAsync 的调用问题
AI：该需求涉及资源加载领域，是否需要我先查看 `resource-loading-strategy` skill？
用户：需要
AI：[读取 skill 后开始实现]
```

### 后置反馈流程（实现后 - 强制）

**触发条件**：完成涉及 skill 领域的实现后

**执行步骤**：
1. **自查新规范**：检查本次实现是否涉及 skill 未覆盖的场景
2. **主动询问**：如有新内容，使用以下格式询问用户：
   ```
   本次实现涉及 [新内容描述]，是否需要补充到 `skill名称` skill 中？
   ```
3. **等待确认**：用户确认后，更新 skill 文件
4. **不得遗漏**：即使没有新内容，也应简要说明"本次实现符合现有 skill 规范"

**示例对话**：
```
AI：[完成实现后]
    本次实现涉及"禁止在回调中调用 Init"的新规范，是否需要补充到 `resource-loading-strategy` skill 中？
用户：要补充
AI：[更新 skill 文件]
```

### 禁止的行为（严格执行）

| 禁止行为 | 后果 |
|---------|------|
| 涉及 skill 领域时不询问就直接实现 | 可能违反项目规范，产生错误代码 |
| 凭记忆回答 skill 相关问题而不阅读 skill | skill 可能已更新，记忆不准确 |
| 遗漏后置反馈环节 | 新规范无法沉淀，团队知识流失 |
| 用户未确认就开始读取 skill | 浪费用户时间，可能用户已知晓 |

### 自检清单

每次涉及 skill 领域的对话结束前，AI 必须自检：
- [ ] 是否在实现前询问了用户？
- [ ] 是否在用户确认后才阅读 skill？
- [ ] 是否严格按照 skill 规范实现？
- [ ] 是否在完成后检查并询问了 skill 扩展？

### Skill 创建/更新规则（强制）
**每次创建或更新 skill 时，必须同步更新本文件的导航**：
1. 在"话题-skill 映射表"中添加/更新关键词映射
2. 在"常用 skill 完整路径"表中添加/更新文件路径
3. 禁止只创建 skill 而不更新 rule.md 导航

### Skill 文件路径
所有 skill 文件位于 `.claude/skills/` 目录下，每个 skill 对应一个子文件夹，主文件为 `SKILL.md`（部分旧 skill 为 `skill.md`）。

**常用 skill 完整路径**：
| skill 名称 | 文件路径 |
|-----------|----------|
| `npc-task-config-system` | `.claude/skills/npc-task-config-system/SKILL.md` |
| `reskin-principles` | `.claude/skills/reskin-principles/SKILL.md` |
| `datatable-config-guide` | `.claude/skills/datatable-config-guide/SKILL.md` |
| `level-system` | `.claude/skills/level-system/SKILL.md` |
| `resource-loading-strategy` | `.claude/skills/resource-loading-strategy/SKILL.md` |

### 工具命令
- 查看已安装 skill：`openskills list`
- 阅读 skill 内容：`openskills read <skill-name>` 或直接 `read_file`
- 项目 skill 目录：`.claude/skills/`

---

在 Copilot 回答中可引用条目编号，例如：遵循 2. 命名规范、4. Unity/游戏相关。

> 按需自由修改、补充各条目。文件路径：`/rule.md`。
