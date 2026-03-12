using System;
using System.Collections.Generic;

/// <summary>
/// 角色数据
/// </summary>
[Serializable]
public class PeopleData
{
    /// <summary>唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>名称</summary>
    public string name = "";
    
    /// <summary>属性ID列表</summary>
    public List<int> propertyIdList = new List<int>();
    
    /// <summary>属性列表</summary>
    public List<SinglePropertyData> propertyList = new List<SinglePropertyData>();
    
    /// <summary>当前战斗属性ID列表</summary>
    public List<int> curBattleProIdList = new List<int>();
    
    /// <summary>当前战斗属性列表</summary>
    public List<SinglePropertyData> curBattleProList = new List<SinglePropertyData>();
    
    /// <summary>是否是玩家</summary>
    public bool isPlayer;
    
    /// <summary>技能数据</summary>
    public AllSkillData allSkillData;
    
    /// <summary>当前装备物品列表（固定6个槽位）</summary>
    public List<ItemData> curEquipItemList = new List<ItemData>() { null, null, null, null, null, null };
    
    /// <summary>性别</summary>
    public int gender;
    
    /// <summary>修炼索引</summary>
    public int trainIndex;
    
    /// <summary>弟子类型</summary>
    public int studentType;
    
    /// <summary>弟子当前能量</summary>
    public int studentCurEnergy;
    
    /// <summary>弟子等级</summary>
    public int studentLevel;
    
    /// <summary>弟子当前经验</summary>
    public int studentCurExp;
    
    /// <summary>弟子稀有度</summary>
    public int studentRarity;
    
    /// <summary>弟子品质</summary>
    public int studentQuality;

    /// <summary>敌人配置ID</summary>
    public int enemySettingId;
    
    /// <summary>头像索引列表</summary>
    public List<int> portraitIndexList = new List<int>();
    
    /// <summary>头像类型</summary>
    public int portraitType;
    
    /// <summary>上次修为增加时间</summary>
    public long lastXiuweiAddTime;
    
    /// <summary>当前修为</summary>
    public ulong curXiuwei;
    
    /// <summary>当前已吃丹药数量</summary>
    public int curEatedDanNum;
    
    /// <summary>弟子状态类型</summary>
    public int studentStatusType;
    
    /// <summary>坐镇丹田唯一ID</summary>
    public ulong zuoZhenDanFarmOnlyId;
    
    /// <summary>特殊头像</summary>
    public bool specialPortrait;
    
    /// <summary>特殊头像名称</summary>
    public string specialPortraitName = "";
    
    /// <summary>天赋</summary>
    public int talent;
    
    /// <summary>当前探索ID</summary>
    public int curAtExploreId;
    
    /// <summary>社交数据</summary>
    public SocializationData socializationData;
    
    /// <summary>当前阶段</summary>
    public int curPhase;
    
    /// <summary>总阶段</summary>
    public int totalPhase;
    
    /// <summary>是否在我的队伍</summary>
    public bool isMyTeam;
    
    /// <summary>下次突破增加值</summary>
    public int nextBreakThroughAdd;
    
    /// <summary>更换天赋次数</summary>
    public int changeTalentNum;
    
    /// <summary>上次出现的天赋列表</summary>
    public List<int> lastAppearTalentList = new List<int>();
    
    /// <summary>上次出现的天赋品质列表</summary>
    public List<int> lastAppearTalentQualityList = new List<int>();
    
    /// <summary>上次出现的元素列表</summary>
    public List<int> lastAppearYuanSuList = new List<int>();
    
    /// <summary>天赋稀有度</summary>
    public int talentRarity;
    
    /// <summary>元素</summary>
    public int yuanSu;
    
    /// <summary>血脉数据</summary>
    public XueMaiData xueMai;
    
    /// <summary>当前解锁的元素列表</summary>
    public List<int> curUnlockedYuanSuList = new List<int>();
    
    /// <summary>所在队伍索引</summary>
    public int atTeamIndex;
    
    /// <summary>队伍位置索引</summary>
    public int teamPosIndex;
    
    /// <summary>改名次数</summary>
    public int changeNameNum;
    
 
    
    /// <summary>洗髓率</summary>
    public int xiSuiRate;
    
    /// <summary>今日吃修为丹数量</summary>
    public int todayEatXiuWeiDanNum;
    
    /// <summary>当前已吃调息丹数量</summary>
    public int curEatedTiaoXiDanNum;
    
    /// <summary>重伤状态</summary>
    public bool seriousInjury;
    
    /// <summary>协战CD字典</summary>
    public Dictionary<ulong, List<int>> xieZhanCDDic = new Dictionary<ulong, List<int>>();
    
    /// <summary>飞云CD</summary>
    public float feiYunCD;
    
    /// <summary>地骨符是否激活</summary>
    public bool diGuFuHuoed;
}

/// <summary>
/// 单个属性数据
/// </summary>
[Serializable]
public class SinglePropertyData
{
    /// <summary>属性ID</summary>
    public int id;
    
    /// <summary>数值</summary>
    public int num;
    
    /// <summary>上限</summary>
    public long limit;
    
    /// <summary>品质</summary>
    public int quality;
}

/// <summary>
/// 血脉数据
/// </summary>
[Serializable]
public class XueMaiData
{
    /// <summary>血脉类型列表</summary>
    public List<XueMaiType> xueMaiTypeList = new List<XueMaiType>();
    
    /// <summary>血脉等级列表</summary>
    public List<int> xueMaiLevelList = new List<int>();
}

/// <summary>
/// 社交数据
/// </summary>
[Serializable]
public class SocializationData
{
    /// <summary>认识的人列表</summary>
    public List<ulong> knowPeopleList = new List<ulong>();
    
    /// <summary>好感度列表</summary>
    public List<int> haoGanDu = new List<int>();
    
    /// <summary>性格</summary>
    public int xingGe;
    
    /// <summary>社交活跃度</summary>
    public int socialActivity;
    
    /// <summary>社交记录列表</summary>
    public List<SocializationRecordData> socialRecordList = new List<SocializationRecordData>();
    
    /// <summary>叛变值</summary>
    public int pan;
    
    /// <summary>记录的主战人列表</summary>
    public List<ulong> recordedZhuZhanPList = new List<ulong>();
    
    /// <summary>记录的主战人好感等级</summary>
    public List<int> recordedZhuZhanPHaoGanLevel = new List<int>();
}

/// <summary>
/// 社交记录数据
/// </summary>
[Serializable]
public class SocializationRecordData
{
    /// <summary>时间</summary>
    public List<int> time = new List<int>();
    
    /// <summary>内容</summary>
    public string content = "";
}