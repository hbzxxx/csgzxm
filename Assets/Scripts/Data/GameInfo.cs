using System;
using System.Collections.Generic;

/// <summary>
/// 游戏存档主数据类 - 纯 C# 类，用于 ES3 序列化
/// </summary>
[Serializable]
public class GameInfo
{
    /// <summary>玩家角色数据</summary>
    public PeopleData playerPeople;
    
    /// <summary>时间数据</summary>
    public TimeData timeData;
    
    /// <summary>唯一ID生成器</summary>
    public ulong TheId;
    
    /// <summary>装备制作团队数据</summary>
    public EquipMakeTeamData EquipMakeTeamData;
    
    /// <summary>所有装备数据</summary>
    public AllEquipmentData AllEquipmentData;
    
    /// <summary>比赛数据</summary>
    public MatchData MatchData;
    
    /// <summary>炼丹数据</summary>
    public LianDanData LianDanData;
    
    /// <summary>弟子数据</summary>
    public StudentData studentData;
    
    /// <summary>建筑数据</summary>
    public AllBuildingData AllBuildingData;
    
    /// <summary>Buff数据</summary>
    public AllBuffData AllBuffData;
    
    /// <summary>物品数据</summary>
    public ItemModel ItemModel;
    
    /// <summary>新手引导数据</summary>
    public NewGuideData NewGuideData;
    
    /// <summary>地图数据</summary>
    public AllMapData AllMapData;
    
    /// <summary>秘境派遣数据</summary>
    public AllMiJingPaiQianData AllMiJingPaiQianData;
    
    /// <summary>宗门生产数据</summary>
    public ZongMenProduceData AllZongMenProduceData;
    
    /// <summary>场景数据</summary>
    public SceneData SceneData;
    
    /// <summary>NPC数据</summary>
    public AllNPCData allNPCData;
    
    /// <summary>成就数据</summary>
    public AllAchievementData AllAchievementData;
    
    /// <summary>仙门大开</summary>
    public bool XianMenOpen;
    
    /// <summary>丹田数据</summary>
    public AllDanFarmData allDanFarmData;
    
    /// <summary>研究数据</summary>
    public AllResearchData AllResearchData;
    
    /// <summary>地图事件数据</summary>
    public AllMapEventData allMapEventData;
    
    /// <summary>UI组件解锁状态</summary>
    public UIComponentUnlockStatus AllUIComponentUnlockStatus;
    
    /// <summary>探索数据</summary>
    public AllExploreData AllExploreData;
    
    /// <summary>宗门数据</summary>
    public ZongMenData allZongMenData;
    
    /// <summary>队伍数据</summary>
    public TeamData AllTeamData;
    
    /// <summary>活动数据</summary>
    public ActivityData AllActivityData;
    
    /// <summary>其他宗门数据（竞技场）- 不参与存档，由 MatchManager 按需生成</summary>
    [NonSerialized]
    public OtherZongMenData AllOtherZongMenData;
    
    /// <summary>账户GUID</summary>
    public string TheGuid = "";
    
    /// <summary>存档时间戳</summary>
    public long SaveTime;
    
    /// <summary>冒险手札数据</summary>
    public AllGuideBookData AllGuideBookData;
    
    /// <summary>版本名称</summary>
    public string VersionName = "";
    
    /// <summary>商店数据</summary>
    public AllShopData allShopData;
    
    /// <summary>广告数据</summary>
    public AllADData AllADData;
    
    /// <summary>日常任务数据</summary>
    public AllDailyTaskData AllDailyTaskData;
    
    /// <summary>签到数据</summary>
    public QianDaoData QianDaoData;
    
    /// <summary>邮件数据</summary>
    public AllMailData AllMailData;
    
    /// <summary>昵称</summary>
    public string NickName = "";
    
    /// <summary>角色ID</summary>
    public int roleId;
    
    /// <summary>记录数据</summary>
    public string RecordData = "";
    
    /// <summary>区服索引</summary>
    public int quIndex;
    
    /// <summary>深渊数据</summary>
    public AllShenYuanData AllShenYuanData;
    
    /// <summary>是否被封号</summary>
    public bool IsFeng;
    
    /// <summary>抽奖数据</summary>
    public ChouJiangData ChouJiangData;
    
    /// <summary>头像（头像和框用|隔开）</summary>
    public string TouXiang = "";
    
    /// <summary>兑换码数据</summary>
    public AllDuiHuanMaData AllDuiHuanMaData;
    
    /// <summary>创号时间</summary>
    public long CreateTime;
    
    /// <summary>所有角色列表</summary>
    public List<PeopleData> AllPeopleList = new List<PeopleData>();
    
    /// <summary>记录的角色数据</summary>
    public NotedPeopleData NotedPeopleData;
}

public enum Direction
{
    None=0,
    Left=1,
    Right=2,
    Up=3,
    Down=4,
}