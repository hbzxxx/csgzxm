using System;
using System.Collections.Generic;

/// <summary>
/// 所有地图数据
/// </summary>
[Serializable]
public class AllMapData
{
    /// <summary>地图列表</summary>
    public List<SingleMapData> MapList = new List<SingleMapData>();
    
    /// <summary>当前选择的地图ID</summary>
    public int CurChoosedMapId;
    
    /// <summary>当前选择的仙门地图ID</summary>
    public int CurChoosedXianMenMapId;
}

/// <summary>
/// 单个地图数据
/// </summary>
[Serializable]
public class SingleMapData
{
    /// <summary>地图ID</summary>
    public int MapId;
    
    /// <summary>地图状态</summary>
    public int MapStatus;
    
    /// <summary>关卡列表</summary>
    public List<SingleLevelData> LevelList = new List<SingleLevelData>();
    
    /// <summary>当前奖励列表</summary>
    public List<ItemData> CurAwardList = new List<ItemData>();
    
    /// <summary>固定关卡列表</summary>
    public List<SingleLevelData> FixedLevelList = new List<SingleLevelData>();
    
    /// <summary>裂隙地图状态</summary>
    public int LieXiMapStatus;
}

/// <summary>
/// 单个关卡数据
/// </summary>
[Serializable]
public class SingleLevelData
{
    /// <summary>关卡ID</summary>
    public string LevelId;
    
    /// <summary>关卡状态</summary>
    public int LevelStatus;
    
    /// <summary>敌人列表</summary>
    public List<PeopleData> Enemy = new List<PeopleData>();
    
    /// <summary>是否曾经通关</summary>
    public bool HaveAccomplished;
}

/// <summary>
/// 所有地图事件数据
/// </summary>
[Serializable]
public class AllMapEventData
{
    /// <summary>单个地图事件列表</summary>
    public List<SingleMapEventData> SingleMapEventList = new List<SingleMapEventData>();
    
    /// <summary>等待出现的事件列表</summary>
    public List<MapEventWaitToAppearData> WaitToAppearList = new List<MapEventWaitToAppearData>();
}

/// <summary>
/// 单个地图事件数据
/// </summary>
[Serializable]
public class SingleMapEventData
{
    /// <summary>地图场景类型</summary>
    public int MapSceneType;
    
    /// <summary>地图事件类型</summary>
    public int MapEventType;
    
    /// <summary>位置</summary>
    public List<int> Pos = new List<int>();
    
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>人物列表</summary>
    public List<PeopleData> PeopleList = new List<PeopleData>();
    
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>位置类型</summary>
    public int PosType;
    
    /// <summary>位置索引</summary>
    public int PosIndex;
    
    /// <summary>是否隐藏</summary>
    public bool IsHide;
    
    /// <summary>探索配置ID</summary>
    public int ExploreSettingId;
    
    /// <summary>逻辑位置</summary>
    public List<int> LogicPos = new List<int>();
    
    /// <summary>探索队伍唯一ID</summary>
    public ulong ExploreTeamOnlyId;
    
    /// <summary>剩余月数</summary>
    public int RemainMonth;
    
    /// <summary>是否有时间限制</summary>
    public bool HaveLimitTime;
    
    /// <summary>固定地图ID</summary>
    public int FixedMapId;
    
    /// <summary>参数</summary>
    public int Param = 0;
    
    /// <summary>问号标记</summary>
    public bool WenHao;
}

/// <summary>
/// 等待出现的地图事件数据
/// </summary>
[Serializable]
public class MapEventWaitToAppearData
{
    /// <summary>剩余月数</summary>
    public int RemainMonth;
    
    /// <summary>配置ID</summary>
    public int SettingId;
}

/// <summary>
/// 场景数据
/// </summary>
[Serializable]
public class SceneData
{
    /// <summary>当前场景类型</summary>
    public int CurSceneType;
    
    /// <summary>上一个场景类型</summary>
    public int LastSceneType;
}
