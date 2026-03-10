using System;
using System.Collections.Generic;

/// <summary>
/// 所有探索数据
/// </summary>
[Serializable]
public class AllExploreData
{
    /// <summary>探索列表</summary>
    public List<SingleExploreData> ExploreList = new List<SingleExploreData>();
}

/// <summary>
/// 单个探索数据
/// </summary>
[Serializable]
public class SingleExploreData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>探索队伍数据</summary>
    public ExploreTeamData ExploreTeamData;
    
    /// <summary>是否正在探索</summary>
    public bool IsExploring;
    
    /// <summary>开始探索时间</summary>
    public long StartExploreTime;
    
    /// <summary>是否已解锁</summary>
    public bool Unlocked;
    
    /// <summary>所有事件数量</summary>
    public int AllEventNum;
}

/// <summary>
/// 探索队伍数据
/// </summary>
[Serializable]
public class ExploreTeamData
{
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>探索配置ID</summary>
    public int ExploreId;
    
    /// <summary>弟子唯一ID列表</summary>
    public List<ulong> StudentOnlyIdList = new List<ulong>();
    
    /// <summary>探索时长（秒）</summary>
    public int ExploreDuration;
    
    /// <summary>获得的物品列表</summary>
    public List<ItemData> ItemList = new List<ItemData>();
    
    /// <summary>逻辑坐标（x,y）</summary>
    public List<int> LogicPos = new List<int>();
    
    /// <summary>逻辑坐标索引</summary>
    public int LogicPosIndex;
    
    /// <summary>剩余补给</summary>
    public int RemainBuJi;
    
    /// <summary>目标事件唯一ID</summary>
    public ulong TargetEventOnlyId;
    
    /// <summary>队伍状态</summary>
    public int Status;
    
    /// <summary>总天数</summary>
    public int TotalDay;
    
    /// <summary>剩余天数</summary>
    public int RemainDay;
    
    /// <summary>当前位置（x,y）</summary>
    public List<float> Pos = new List<float>();
    
    /// <summary>移动前的本地位置（x,y）</summary>
    public List<float> LocalPosBeforeMove = new List<float>();
}

/// <summary>
/// 所有秘境派遣数据
/// </summary>
[Serializable]
public class AllMiJingPaiQianData
{
    /// <summary>派遣列表</summary>
    public List<SingleMiJingPaiQianData> PaiqianList = new List<SingleMiJingPaiQianData>();
}

/// <summary>
/// 单个秘境派遣数据
/// </summary>
[Serializable]
public class SingleMiJingPaiQianData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>关卡列表</summary>
    public List<SingleMiJingLevelData> LevelList = new List<SingleMiJingLevelData>();
    
    /// <summary>开始派遣时间</summary>
    public long StartPaiQianTime;
    
    /// <summary>是否正在派遣</summary>
    public bool IsPaiQian;
    
    /// <summary>上次击杀怪物时间</summary>
    public long LastKillMonsterTime;
    
    /// <summary>每日最高次数</summary>
    public int DayliHighNum;
    
    /// <summary>每日最高次数上限</summary>
    public int MaxDayliHighNum;
    
    /// <summary>当前周类型</summary>
    public int WeekType;
    
    /// <summary>最高关卡等级</summary>
    public int HighestLevelLevel;
}

/// <summary>
/// 单个秘境关卡数据
/// </summary>
[Serializable]
public class SingleMiJingLevelData
{
    /// <summary>秘境ID</summary>
    public int MiJingId;
    
    /// <summary>关卡ID</summary>
    public int LevelId;
    
    /// <summary>完成状态</summary>
    public int AccomplishStatus;
}

/// <summary>
/// 所有研究数据
/// </summary>
[Serializable]
public class AllResearchData
{
    /// <summary>研究列表</summary>
    public List<SingleResearchData> ResearchList = new List<SingleResearchData>();
    
    /// <summary>研究队列（正在研究的配置ID列表）</summary>
    public List<int> ResearchingQueue = new List<int>();
}

/// <summary>
/// 单个研究数据
/// </summary>
[Serializable]
public class SingleResearchData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>当前等级</summary>
    public int CurLevel;
    
    /// <summary>等级上限</summary>
    public int LevelLimit;
    
    /// <summary>是否正在研究</summary>
    public bool IsResearching;
    
    /// <summary>总时间</summary>
    public int TotalTime;
    
    /// <summary>剩余时间</summary>
    public int RemainTime;
}
