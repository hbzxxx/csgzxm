using System;
using System.Collections.Generic;

/// <summary>
/// 所有建筑数据
/// </summary>
[Serializable]
public class AllBuildingData
{
    /// <summary>建筑列表</summary>
    public List<SingleBuildingData> BuildList = new List<SingleBuildingData>();
    
    /// <summary>山门等级</summary>
    public int MountainLevel;
}

/// <summary>
/// 单个建筑数据
/// </summary>
[Serializable]
public class SingleBuildingData
{
    /// <summary>最大弟子数量</summary>
    public int MaxStudentNum;
    
    /// <summary>当前弟子数量</summary>
    public int StudentNum;
    
    /// <summary>建筑类型ID</summary>
    public int BuildTypeId;
    
    /// <summary>当前建筑等级</summary>
    public int CurBuildLevel;
    
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>内门弟子数量上限</summary>
    public int MaxNeiMenStudentNumLimit;
}

/// <summary>
/// 所有Buff数据
/// </summary>
[Serializable]
public class AllBuffData
{
    /// <summary>Buff列表</summary>
    public List<SingleBuffData> BuffList = new List<SingleBuffData>();
}

/// <summary>
/// 单个Buff数据
/// </summary>
[Serializable]
public class SingleBuffData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>剩余天数</summary>
    public int RemainDay;
    
    /// <summary>叠加层数</summary>
    public int StackCount;
}
