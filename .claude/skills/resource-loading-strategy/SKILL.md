# 资源加载策略 (resource-loading-strategy)

## 概述
本 skill 定义了项目中资源加载的时机、顺序和规范，确保依赖关系正确，避免因加载顺序问题导致运行时报错。

---

## 1. 配置表加载时机规范

### 1.1 核心原则
**配置表必须在依赖它的功能之前加载完成。**

### 1.2 典型案例：敏感词检查
- **问题场景**：注册面板的敏感词检查依赖 `DataTable.IsScreening()`，如果配置表未加载会报错
- **解决方案**：在 `Game.Awake()` 中，于 `OpenLoginPanel()` 之前调用 `LoadTable()`

```csharp
// Game.cs - Awake()
private void Awake()
{
    if (!openGame) return;
    Instance = this;
    RoleManager.Instance.CheckArchive();
    SDKManager.Instance.Init();
    PanelManager.Instance.Init();

    // 配置表加载提前到注册之前，否则敏感词检查会报错
    if (!tableLoaded)
    {
        LoadTable();
    }

    PanelManager.Instance.OpenLoginPanel();
}
```

### 1.3 加载顺序检查清单
在添加新功能时，检查是否依赖配置表：
- [ ] 敏感词检查 (`DataTable.IsScreening`)
- [ ] 物品/装备数据查询
- [ ] 关卡配置读取
- [ ] NPC/任务配置读取

如果依赖配置表，确保该功能的入口在 `TableOK` 事件之后，或在 `Game.Awake()` 中 `LoadTable()` 之后。

---

## 2. 面板加载规范

### 2.1 同步加载 vs 异步加载
- **同步加载**：`OpenPanel`、`OpenSingle` - 立即返回，适用于小型 UI
- **异步加载**：`OpenPanelAsync`、`OpenSingleAsync` - 回调返回，适用于大型资源

### 2.2 禁止在异步回调中处理视图初始化逻辑
- 所有视图的初始化逻辑必须在视图自身的 `Init(params object[] args)` 或 `OnOpenIng(params object[] args)` 中处理
- 调用方只负责传递参数，不在回调中操作视图内部状态

**错误示例**：
```csharp
OpenSingleAsync("HPBar", parent, (view) => {
    view.ske.skeletonDataAsset = xxx; // 错误：在回调中设置视图状态
}, false);
```

**正确示例**：
```csharp
OpenSingleAsync("HPBar", parent, null, false, skeletonDataAsset);
// 由 HPBarView.Init() 处理初始化
```

---

## 3. 相关文件
- `Assets/Scripts/Game.cs` - 游戏入口，配置表加载
- `Assets/Scripts/Tools/DataTable.cs` - 配置表加载与查询
- `Assets/Scripts/Tools/MyHttpServer.cs` - 敏感词检查 `DataTable.IsScreening()`
