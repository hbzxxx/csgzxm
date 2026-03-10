using System;
using System.Collections.Generic;

/// <summary>
/// 所有广告数据
/// /// </summary>
[Serializable]
public class AllADData
{
    /// <summary>总广告观看次数</summary>
    public int allTotalADWatchNum;
    
    /// <summary>获取奖励的总广告观看次数索引</summary>
    public int getAwardTotalADWatchNumIndex;
}

/// <summary>
/// 抽奖数据
/// </summary>
[Serializable]
public class ChouJiangData
{
    /// <summary>保底10次数</summary>
    public int baoDi10Num;

    public int baoDi50Num;
    
    /// <summary>保底100次数</summary>
    public int baoDi100Num;
}

/// <summary>
/// 所有兑换码数据
/// </summary>
[Serializable]
public class AllDuiHuanMaData
{
    /// <summary>已获取批次列表</summary>
    public List<int> getPiCiList = new List<int>();
    
    /// <summary>已获取兑换码列表</summary>
    public List<string> getDuiHuanMaList = new List<string>();
}

/// <summary>
/// 所有深渊数据
/// </summary>
[Serializable]
public class AllShenYuanData
{
    /// <summary>当前层数</summary>
    public int curFloor;
    
    /// <summary>最高层数</summary>
    public int maxFloor;
    
    /// <summary>今日挑战次数</summary>
    public int todayChallengeCount;
    
    /// <summary>上次挑战时间</summary>
    public long lastChallengeTime;
    
    /// <summary>深渊层级列表</summary>
    public List<SingleShenYuanData> ShenYuanList = new List<SingleShenYuanData>();
}

/// <summary>
/// 单个深渊层级数据
/// </summary>
[Serializable]
public class SingleShenYuanData
{
    /// <summary>层级ID</summary>
    public int LevelId;
    
    /// <summary>是否已通关</summary>
    public bool IsCleared;
    
    /// <summary>第一层敌人列表</summary>
    public List<PeopleData> Layer1EnemyList = new List<PeopleData>();
}
