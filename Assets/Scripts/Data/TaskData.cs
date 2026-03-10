using System;
using System.Collections.Generic;

/// <summary>
/// 所有日常任务数据
/// </summary>
[Serializable]
public class AllDailyTaskData
{
    /// <summary>日常任务列表</summary>
    public List<SingleDailyTaskData> dailyTaskList = new List<SingleDailyTaskData>();
    
    /// <summary>当前活跃度</summary>
    public int curActiveNum;
    
    /// <summary>活跃度奖励领取状态列表</summary>
    public List<int> activeAwardGetStatusList = new List<int>();
    
    /// <summary>上次刷新时间</summary>
    public long lastBrushTime;
}

/// <summary>
/// 单个日常任务数据
/// </summary>
[Serializable]
public class SingleDailyTaskData
{
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>完成状态</summary>
    public int accomplishStatus;
    
    /// <summary>当前数量</summary>
    public int curNum;
}

/// <summary>
/// 所有冒险手札数据
/// </summary>
[Serializable]
public class AllGuideBookData
{
    /// <summary>单个手札任务数据列表</summary>
     public List<SingleGuideBookTaskData> singleGuideBookTaskDataList = new List<SingleGuideBookTaskData>();
    
    /// <summary>单章列表</summary>
     public List<SingleChapterGuideBookData> singleChapterList = new List<SingleChapterGuideBookData>();
}

/// <summary>
/// 单个手札任务数据
/// </summary>
[Serializable]
public class SingleGuideBookTaskData
{
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>完成状态</summary>
    public int accomplishStatus;
    
    /// <summary>当前进度</summary>
    public int curProgress;
}

/// <summary>
/// 单章手札数据
/// </summary>
[Serializable]
public class SingleChapterGuideBookData
{
    /// <summary>章节</summary>
     public int chapter;
    
    /// <summary>进度完成状态列表</summary>
    public List<int> processAccomplishStatus = new List<int>();
    
    /// <summary>是否已解锁</summary>
    public bool reveal;
}

/// <summary>
/// 新手引导数据
/// </summary>
[Serializable]
public class NewGuideData
{
    /// <summary>已完成引导ID列表</summary>
    public List<int> finishedGuideIdList = new List<int>();
    
    /// <summary>引导ID列表</summary>
    public List<int> IdList = new List<int>();
    
    /// <summary>完成状态列表</summary>
    public List<int> AccomplishStatus = new List<int>();
    
    /// <summary>当前引导ID</summary>
    public int curGuideId;
    
    /// <summary>当前引导步骤</summary>
    public int curGuideStep;
}
