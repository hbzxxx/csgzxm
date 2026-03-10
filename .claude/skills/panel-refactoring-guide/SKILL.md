# 面板重构与合并指南

## 概述

本文档记录 UI 面板重构、合并、删除时的注意事项和检查清单，避免遗漏关联代码导致编译错误。

## 面板合并检查清单

### 1. 合并前分析

在合并两个面板前，必须检查：

1. **两个面板的完整代码**：了解各自功能
2. **调用位置**：搜索 `OpenPanel<面板名>` 找到所有调用点
3. **子组件依赖**：检查面板内使用的 `SingleViewBase` 子视图
4. **对象池枚举**：检查 `ObjectPoolSingle` 中是否有对应枚举值

### 2. 合并时操作

1. **保留的面板**：
   - 添加被合并面板的字段和方法
   - 更新 `Init()`、`OnOpenIng()`、`Clear()` 生命周期
   - 保持原有功能不受影响

2. **被删除的面板**：
   - 修改所有调用点，改为调用保留的面板
   - 删除 `.cs` 文件
   - 删除对应预制体（如有）
   - 删除 `ObjectPoolSingle` 中的枚举值（如有）

3. **子组件处理**：
   - 如果子组件只被删除的面板使用，一并删除
   - 如果子组件被多处使用，保留并检查兼容性

### 3. 合并后验证

1. **编译检查**：确保无编译错误
2. **引用检查**：搜索被删除类名，确保无残留引用
3. **预制体配置**：在 Unity 编辑器中配置新增字段的绑定

## 案例记录

### SetNamePanel 与 XueMaiChoosePanel 合并

**背景**：将血脉选择功能从 `XueMaiChoosePanel` 合并到 `SetNamePanel`，同时移除 `SetNamePanel` 的捏脸功能。

**改动清单**：

1. **修改 `SetNamePanel.cs`**：
   - 移除捏脸相关：`Portrait`、`SingleChangeFaceView[]`、`btn_rdmFace`、`curFace`、`SetFace()`、`RdmFace()`
   - 新增血脉选择：`xueMaiBtnList`、`typeList`、`curChoosedXueMai`、`img_xueMaiDesImg`
   - 新增 `OnChoosedSingleXueMai()` 方法
   - 修改 `OnConfirm()`：增加血脉检查、设置血脉数据、自动随机脸

2. **修改 `Game.cs`**：
   - `OpenPanel<XueMaiChoosePanel>` 改为 `OpenPanel<SetNamePanel>`

3. **删除文件**：
   - `XueMaiChoosePanel.cs`：功能已合并
   - `SingleChangeFaceView.cs`：捏脸组件不再需要

**预制体配置**：
- 需要在 `SetNamePanel` 预制体上绑定：
  - `xueMaiBtnList`：血脉选择按钮列表
  - `typeList`：对应的 `YuanSuType` 枚举列表
  - `img_xueMaiDesImg`：血脉描述图片

**遗漏教训**：
- 初次修改时遗漏了 `SingleChangeFaceView`，该组件引用了 `SetNamePanel.SetFace()` 方法
- 教训：删除面板方法时，必须搜索该方法的所有调用者

## 通用搜索命令

```
# 查找面板调用位置
搜索: OpenPanel<面板名>

# 查找类引用
搜索: 类名

# 查找方法调用
搜索: .方法名(
```

## 注意事项

1. **不要假设子组件只在一处使用**：始终搜索确认
2. **生命周期方法必须完整**：`Init`、`OnOpenIng`、`Clear` 都要检查
3. **对象池状态重置**：合并后的面板如果新增字段，必须在 `Clear()` 中重置
4. **预制体同步**：代码改完后，Unity 预制体也需要同步配置
