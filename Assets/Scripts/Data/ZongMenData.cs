using System;
using System.Collections.Generic;

/// <summary>
/// 宗门数据
/// </summary>
[Serializable]
public class ZongMenData
{
    /// <summary>宗门名称</summary>
    public string ZongMenName = "";
    
    /// <summary>宗门等级</summary>
    public int ZongMenLevel;
    
    /// <summary>当前段位等级</summary>
    public int CurRankLevel;
    
    /// <summary>当前星数</summary>
    public int CurStar;
    
    /// <summary>R值（匹配分数）</summary>
    public int TheR;
    
    /// <summary>体力上限</summary>
    public int TiliLimit;
    
    /// <summary>赠送丹田数量增加</summary>
    public int SendFarmNumLimitAddNum;
    
    /// <summary>改名次数</summary>
    public int ChangeNameNum;
}

/// <summary>
/// 宗门生产数据
/// </summary>
[Serializable]
public class ZongMenProduceData
{
    /// <summary>单个宗门生产数据列表</summary>
    public List<SingleZongMenProduceData> SingleZongMenProduceDataList = new List<SingleZongMenProduceData>();
    
    /// <summary>总弟子数量</summary>
    public int TotalStudentNum;
    
    /// <summary>空闲弟子数量</summary>
    public int FreeStudentNum;
}

/// <summary>
/// 单个宗门生产数据
/// </summary>
[Serializable]
public class SingleZongMenProduceData
{
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>当前等级</summary>
    public int CurLevel;
    
    /// <summary>当前弟子数量</summary>
    public int CurStudentNum;
    
    /// <summary>当前弟子上限</summary>
    public int CurStudentLimit;
    
    /// <summary>当前产量上限</summary>
    public int CurProductLimit;
    
    /// <summary>上次收获时间</summary>
    /// public long LastHarvestTime;
}

/// <summary>
/// 其他宗门数据
/// </summary>
[Serializable]
public class OtherZongMenData
{
    /// <summary>当前场地名称</summary>
    public string curFieldName = "";
    
    /// <summary>宗门列表</summary>
    public List<SingleOtherZongMenData> zongMenList = new List<SingleOtherZongMenData>();
}

/// <summary>
/// 单个其他宗门数据
/// </summary>
[Serializable]
public class SingleOtherZongMenData
{
    /// <summary>宗门名称</summary>
    public string zongMenName = "";
    
    /// <summary>人物列表（新版本后续会删掉该配置）</summary>
    public List<PeopleData> pList = new List<PeopleData>();
    
    /// <summary>总战斗力</summary>
    public int totalZhanDouLi;
    
    /// <summary>R值（积分）</summary>
    public int theR;
    
    /// <summary>当前段位等级</summary>
    public int curRankLevel;
    
    /// <summary>是否是玩家宗门</summary>
    public bool isPlayerZongMen;
    
    /// <summary>当前星数</summary>
    public int curStar;
    
    /// <summary>人物属性索引</summary>
    public int peopleProIndex;
    
    /// <summary>简单人物列表</summary>
    public List<SimpleP> simplePList = new List<SimpleP>();
}

/// <summary>
/// 简单人物数据（用于匹配）
/// </summary>
[Serializable]
public class SimpleP
{
    /// <summary>名称</summary>
    public string name = "";
    
    /// <summary>性别</summary>
    public int gender;
    
    /// <summary>唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>头像索引列表</summary>
    public List<int> portraitIndexList = new List<int>();
    
    /// <summary>是否是玩家</summary>
    public bool isPlayer;
}
