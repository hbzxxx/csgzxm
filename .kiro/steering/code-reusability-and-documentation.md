---
inclusion: always
---

# 代码复用与注释规范

## 核心原则

### 1. 优先复用，避免重复造轮子
- 在编写新代码前，先检查项目中是否已有类似功能
- 优先使用已封装的方法库、工具函数、通用组件
- 避免创建功能雷同的新方法

### 2. 代码必须有清晰注释
- 所有函数/方法必须有功能说明
- 复杂逻辑必须有行内注释解释
- 关键参数和返回值必须注明类型和用途

## 代码复用策略

### 开发前检查清单
在编写新功能前，按以下顺序检查：

1. **项目内已有代码**
   - 搜索相似的函数名、类名
   - 检查 `utils/`、`helpers/`、`common/` 等工具目录
   - 查看已有组件库

2. **框架内置方法**
   - Python/TypeScript/JavaScript 标准库
   - 已安装的第三方包
   - 框架内置 API

3. **确认需要新建**
   - 现有代码无法满足需求
   - 扩展现有方法比新建更复杂
   - 功能具有独特性

### 复用方式

#### 直接调用
```typescript
// 正确：使用已有工具函数
import { formatDate } from '@/utils/dateHelper';
const formattedDate = formatDate(new Date());

// 错误：重复实现
function myFormatDate(date: Date): string {
  // 重复实现了已有的 formatDate 功能
  return `${date.getFullYear()}-${date.getMonth()+1}-${date.getDate()}`;
}
```

#### 扩展现有方法
```typescript
// 正确：基于已有方法扩展
import { formatDate } from '@/utils/dateHelper';

/**
 * 格式化日期并添加时区信息
 * @param date 日期对象
 * @param timezone 时区，默认为 'UTC+8'
 * @returns 格式化后的日期字符串，包含时区
 */
export function formatDateWithTimezone(date: Date, timezone: string = 'UTC+8'): string {
  return `${formatDate(date)} ${timezone}`;
}
```

#### 创建通用方法
当发现某段逻辑在多处使用时：
1. 抽取为独立函数
2. 放入适当的工具模块（如 `utils/`）
3. 添加完整注释
4. 在其他地方引用

## 注释规范

### 函数/方法注释（必须）

```typescript
/**
 * 计算两点之间的距离
 * @param point1 第一个坐标点 {x: number, y: number}
 * @param point2 第二个坐标点 {x: number, y: number}
 * @returns 两点间的欧几里得距离
 * @example
 * const distance = calculateDistance({x: 0, y: 0}, {x: 3, y: 4});
 * // 返回 5
 */
function calculateDistance(
  point1: {x: number, y: number}, 
  point2: {x: number, y: number}
): number {
  const dx = point2.x - point1.x;
  const dy = point2.y - point1.y;
  return Math.sqrt(dx * dx + dy * dy);
}
```

### 类注释（必须）

```typescript
/**
 * 数据源管理器
 * 负责数据源的创建、销毁、状态管理
 * 单例模式，通过 DataSourceManager.getInstance() 获取实例
 */
export class DataSourceManager {
  private static instance: DataSourceManager;
  
  /**
   * 获取数据源管理器的单例实例
   * @returns DataSourceManager 实例
   */
  public static getInstance(): DataSourceManager {
    if (!DataSourceManager.instance) {
      DataSourceManager.instance = new DataSourceManager();
    }
    return DataSourceManager.instance;
  }
}
```

### 复杂逻辑注释（建议）

```typescript
function calculateDataScore(data: DataPoint, weights: ScoreWeights): number {
  // 基础分数 = 数据质量 * 权重
  let baseScore = data.quality * weights.qualityWeight;
  
  // 时效性判定：如果数据在24小时内，分数加倍
  if (isWithin24Hours(data.timestamp)) {
    baseScore *= 2;
  }
  
  // 来源可信度调整
  const sourceBonus = getSourceReliabilityBonus(data.source);
  baseScore *= sourceBonus;
  
  // 最小分数保护：确保至少有 1 分
  return Math.max(1, Math.floor(baseScore));
}
```

### 常量和配置注释（建议）

```typescript
// 数据采集配置常量
export const DataCollectionConfig = {
  /** 默认采集间隔（毫秒） */
  DEFAULT_INTERVAL: 5000,
  
  /** 最大重试次数 */
  MAX_RETRY_COUNT: 3,
  
  /** 请求超时时间（毫秒） */
  REQUEST_TIMEOUT: 30000,
  
  /** 批量处理大小 */
  BATCH_SIZE: 100,
};
```

### TODO 标记（特殊情况）

```typescript
// TODO: 优化性能，考虑使用连接池
function createDataSource(type: string): DataSource {
  return new DataSource(type);
}

// FIXME: 当数据源数量超过 100 时会导致性能下降
function updateDataSources(sources: DataSource[]): void {
  sources.forEach(source => source.update());
}

// NOTE: 此方法依赖于 API 返回的数据格式
function parseAPIResponse(data: any): CollectedData {
  // ...
}
```

## 检查流程

### 编写新功能时
1. 明确功能需求
2. 搜索项目中是否已有类似实现
3. 检查框架/库是否提供现成方法
4. 决定：复用/扩展/新建
5. 添加完整注释
6. 如果是通用功能，考虑放入 `utils/` 或 `common/`

### Code Review 检查点
- 是否有重复造轮子的代码？
- 函数/类是否有清晰注释？
- 复杂逻辑是否有解释？
- 工具函数是否放在合适位置？

## 注释语言

- 优先使用中文注释（项目主要语言）
- 代码命名使用英文（遵循编程规范）
- 对外API可提供双语注释

```typescript
/**
 * 加载数据源
 * Load a data source
 * @param sourceName 数据源名称 / Data source name
 */
function loadDataSource(sourceName: string): void {
  // ...
}
```

## 与AI协作的建议

当使用 AI 辅助开发时：
1. 明确告知 AI 项目中已有的工具和组件
2. 要求 AI 优先使用现有代码
3. 要求 AI 生成的代码包含完整注释
4. 定期整理 AI 生成的代码，提取可复用部分

## 强制要求

### 必须有注释的情况
- 所有导出的函数/类/接口
- 所有公共方法（public methods）
- 复杂的算法或业务逻辑
- 配置文件和常量定义

### 可以省略注释的情况
- 自解释的简单代码（如 getter/setter）
- 私有辅助函数（但建议仍添加简短说明）
- 测试代码（但应有测试用例说明）

## 违规示例

### 错误示例 1：重复造轮子
```typescript
// 项目中已有 utils/mathHelper.ts 中的 clamp 方法
// 但在新文件中又重新实现了一遍
function limitValue(value: number, min: number, max: number): number {
  return Math.min(Math.max(value, min), max);
}
```

### 错误示例 2：缺少注释
```typescript
// 没有任何注释说明这个函数的用途
export function calc(a: DataPoint, b: DataPoint): number {
  let d = a.score - b.weight * 0.5;
  if (Math.random() < a.priority) d *= 2;
  return Math.max(1, Math.floor(d));
}
```

### 正确示例
```typescript
import { clamp } from '@/utils/mathHelper';

/**
 * 限制数据值范围
 * @param value 当前数值
 * @param bounds 边界范围 {min: number, max: number}
 * @returns 限制后的数值
 */
function limitDataValue(
  value: number, 
  bounds: {min: number, max: number}
): number {
  return clamp(value, bounds.min, bounds.max);
}
```
