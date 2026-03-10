using System;
using System.Collections.Generic;

/// <summary>
/// 比赛数据
/// </summary>
[Serializable]
public class MatchData
{
    /// <summary>上次参加比赛时间</summary>
    public long LastParticipateMatchTime;
    
    /// <summary>今天参赛次数</summary>
    public int TodayParticipateMatchNum;
    
    /// <summary>今天获奖状态列表</summary>
    public List<int> TodayAwardGetStatusList = new List<int>();
    
    /// <summary>今天胜场数</summary>
    public int TodayWinNum;
    
    /// <summary>今天胜场获奖状态列表</summary>
    public List<int> TodayWinAwardGetStatusList = new List<int>();
    
    /// <summary>广告刷新了参赛次数</summary>
    public bool WatchedADAddParticipateTime;
    
    /// <summary>今日领取了结算奖励</summary>
    public bool GetJieSuanAward;
    
    /// <summary>大比次数今天刷了几次</summary>
    public int AddedDaBiParticipateNum;
    
    /// <summary>单个比赛数据列表</summary>
    public List<SingleMatchData> SingleMatchDataList = new List<SingleMatchData>();
    
    /// <summary>已报名的单个比赛数据</summary>
    public SingleMatchData EnrolledSingleMatchData;
    
    /// <summary>当前比赛数据</summary>
    public SingleMatchData CurMatchData;
}

/// <summary>
/// 单个比赛数据
/// </summary>
[Serializable]
public class SingleMatchData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>参赛次数</summary>
    public int ParticipateNum;
    
    /// <summary>冠军次数</summary>
    public int ChampionNum;
}
