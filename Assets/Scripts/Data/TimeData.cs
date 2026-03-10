using System;
using System.Collections.Generic;

/// <summary>
/// 时间数据
/// </summary>
[Serializable]
public class TimeData
{
    /// <summary>年</summary>
    public int Year;
    
    /// <summary>月</summary>
    public int Month;
    
    /// <summary>日</summary>
    public int Day;
    
    /// <summary>周</summary>
    public int Week;
    
    /// <summary>周几</summary>
    public int TheWeekDay;
    
    /// <summary>今天进度百分比</summary>
    public float DayProcess;
    
    /// <summary>上次恢复体力时间</summary>
    public long LastReviveTiliTime;
    
    /// <summary>今日广告体力次数</summary>
    public int TodayADTiliNum;
    
    /// <summary>上次广告恢复体力时间</summary>
    public long LastADReviveTiliTime;
    
    /// <summary>上次领取离线收益时间</summary>
    public long LastReceiveOfflineIncomeTime;
    
    /// <summary>离线物品列表</summary>
    public List<ItemData> OfflineItemList = new List<ItemData>();
    
    /// <summary>上次记录时间</summary>
    public long LastRecordedTime;
    
    /// <summary>离线总分钟数</summary>
    public int OffLineTotalMinute;
    
    /// <summary>下次随机事件剩余月数</summary>
    public int NextRdmEventRemainMonth;
    
    /// <summary>上次观看广告时间</summary>
    public long LastADWatchTime;
    
    /// <summary>总广告观看次数</summary>
    public int TotalADWatchNum;
    
    /// <summary>今日参与历练状态</summary>
    public List<int> TodayParticipatedLiLianStatus = new List<int>();
    
    /// <summary>上次参与历练时间</summary>
    public List<long> LastParticipatedLiLianTime = new List<long>();
    
    /// <summary>每日最大历练次数</summary>
    public int MaxLiLianTimePerDay;
    
    /// <summary>上次记录元神受损时间</summary>
    public long LastRecordYuanShenShouSumTime;
    
    /// <summary>上次记录云存档时间</summary>
    public long LastRecordCloudArchiveTime;
    
    /// <summary>今日参与选秀次数</summary>
    public int TodayParticipateXuanXiuNum;
    
    /// <summary>上次上传存档时间</summary>
    public long LastUploadArchiveTime;
    
    /// <summary>今日抽奖次数</summary>
    public long TodayChouNum;
}