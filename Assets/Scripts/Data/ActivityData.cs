using System;
using System.Collections.Generic;

/// <summary>
/// 活动数据
/// </summary>
[Serializable]
public class ActivityData
{
    /// <summary>等待出现的活动列表</summary>
    public List<SingleActivityData> ReadyToAppearActivityList = new List<SingleActivityData>();
    
    /// <summary>已出现的活动列表</summary>
    public List<SingleActivityData> AppearActivityList = new List<SingleActivityData>();
    
    /// <summary>已报名的活动列表</summary>
    public List<SingleActivityData> BaoMingActivityList = new List<SingleActivityData>();
}

/// <summary>
/// 单个活动数据
/// </summary>
[Serializable]
public class SingleActivityData
{
    /// <summary>是否准备出现</summary>
    public bool ReadyToAppear;
    
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>剩余周数</summary>
    public int RemainWeek;
    
    /// <summary>敌方弟子属性列表</summary>
    public List<int> EnemyStudentProList = new List<int>();
    
    /// <summary>敌方弟子名称列表</summary>
    public List<string> EnemyStudentNameList = new List<string>();
    
    /// <summary>敌方弟子性别列表</summary>
    public List<int> EnemyStudentGenderList = new List<int>();
    
    /// <summary>敌方弟子宗门名称列表</summary>
    public List<string> EnemyStudentZongMenNameList = new List<string>();
    
    /// <summary>需要的天赋</summary>
    public int NeedTalent;
    
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>总周数</summary>
    public int TotalWeek;
}

/// <summary>
/// 所有成就数据
/// </summary>
[Serializable]
public class AllAchievementData
{
    /// <summary>成就列表</summary>
    public List<SingleAchievementData> achievementList = new List<SingleAchievementData>();
    
    /// <summary>已通过的任务标签ID列表</summary>
    public List<string> PassedTaskTagIdList = new List<string>();
    
    /// <summary>已击杀的敌人记录</summary>
    public string KilledEnemy = "";
    
    /// <summary>已通过的最大地图等级</summary>
    public int PassedMaxMapLevel;
    
    /// <summary>已完成的地图事件列表</summary>
    public List<string> AccomplishedMapEvent = new List<string>();
    
    /// <summary>已完成的一次性引导列表</summary>
    public List<int> AccomplishedOnceGuideList = new List<int>();
    
    /// <summary>权力次数</summary>
    public int QuanLiNum;
    
    /// <summary>天赋觉醒次数</summary>
    public int TianFuJueXingNum;
    
    /// <summary>弟子升级次数</summary>
    public int StudentUpgradeCount;
    
    /// <summary>已通过的最大关卡</summary>
    public string PassedMaxLevel = "";
    
    /// <summary>已通过的最大猎袭关卡</summary>
    public string PassedMaxLieXiLevel = "";
    
    /// <summary>弟子使用仓库列表</summary>
    /// public List<string> StudentUseCangKuList = new List<string>();
    
    /// <summary>淘法次数列表</summary>
    public List<string> TaoFaNum = new List<string>();
    
    /// <summary>炼丹2次数列表</summary>
    public List<string> LianDan2Num = new List<string>();
    
    /// <summary>探索次数列表</summary>
    public List<string> ExploreNum = new List<string>();
    
    /// <summary>最大淘法</summary>
    public List<string> MaxTaoFa = new List<string>();
    
    /// <sumry>历练次数</summary>
    public int LiLianNum;
    
    /// <summary>制作宝石次数</summary>
    public int MakeGemNum;
}

/// <summary>
/// 单个成就数据
/// </summary>
[Serializable]
public class SingleAchievementData
{
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>完成状态</summary>
    public int accomplishStatus;
    
    /// <summary>当前进度</summary>
    public int curProgress;
}

/// <summary>
/// 队伍数据
/// </summary>
[Serializable]
public class TeamData
{
    /// <summary>队伍1成员唯一ID列表</summary>
    public List<ulong> TeamList1 = new List<ulong>();
    
    /// <summary>队伍2成员唯一ID列表</summary>
    public List<ulong> TeamList2 = new List<ulong>();
}

/// <summary>
/// UI组件解锁状态
/// </summary>
[Serializable]
public class UIComponentUnlockStatus
{
    /// <summary>UI组件类型列表</summary>
    public List<int> UIComponentTypeList = new List<int>();
    
    /// <summary>UI组件状态列表</summary>
    public List<int> UIComponentStatusList = new List<int>();
}
