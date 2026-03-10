using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有丹田数据
/// </summary>
[Serializable]
public class AllDanFarmData
{
    /// <summary>丹田列表</summary>
    public List<SingleDanFarmData> DanFarmList = new List<SingleDanFarmData>();
    
    /// <summary>丹田坐镇弟子上限</summary>
    public int DanFarmZuoZhenStudentLimit;
    
    /// <summary>已解锁丹田ID列表</summary>
    public List<int> UnlockedDanFarmId = new List<int>();
    
    /// <summary>已解锁丹田数量上限</summary>
    public int UnlockedDanFarmNumLimit;
    
    /// <summary>已解锁丹田数量</summary>
    public int UnlockedFarmNum;
    
    /// <summary>重建索引列表</summary>
    public List<int> ReBuildingIndexList = new List<int>();

    public int UnlockFarmNeedLingShiNum;
}

/// <summary>
/// 单个丹田数据
/// </summary>
[Serializable]
public class SingleDanFarmData
{
    /// <summary>唯一ID</summary>
    public ulong OnlyId;
    
    /// <summary>配置ID</summary>
    public int SettingId;
    
    /// <summary>当前等级</summary>
    public int CurLevel;
    
    /// <summary>是否为空</summary>
    public bool IsEmpty;
    
    /// <summary>索引位置</summary>
    public int Index;
    
    /// <summary>当前状态（建造/升级/工作）</summary>
    public int Status;
    
    /// <summary>建造或改造剩余时间（天）</summary>
    public int RemainTime;
    
    /// <summary>产丹计时器</summary>
    public int ProcessDanTimer;
    
    /// <summary>建造或改造总需时间</summary>
    public int RebuildTotalTime;
    
    /// <summary>坐镇弟子ID列表</summary>
    public List<ulong> ZuoZhenStudentIdList = new List<ulong>();
    
    /// <summary>开启了全力产出</summary>
    public bool OpenQuanLi;
    
    /// <summary>全力产出总时间</summary>
    public int QuanLiTotalTime;
    
    /// <summary>全力产出剩余时间</summary>
    public int QuanliRemainTime;
    
    /// <summary>位置解锁状态列表</summary>
    public List<bool> PosUnlockStatusList = new List<bool>();
    
    /// <summary>丹田类型</summary>
    public int DanFarmType;
    
    /// <summary>产出速度</summary>
    public int ProcessSpeed;
    
    /// <summary>产品配置ID</summary>
     public int ProductSettingId;
    
    /// <summary>产品剩余数量</summary>
    public int ProductRemainNum;
    
    /// <summary>丹田工作类型</summary>
    public int DanFarmWorkType;
    
    /// <summary>是否已解锁</summary>
    public bool Unlocked;
    
    /// <summary>天赋类型</summary>
    public int TalentType;
    
    /// <summary>已解锁产品ID列表</summary>
    public List<int> UnlockedProductIdList = new List<int>();
    
    /// <summary>弟子使用仓库数据列表</summary>
    public List<SingleStudentUseCangKuData> StudentUseCangKuDataList = new List<SingleStudentUseCangKuData>();
    
    /// <summary>产品物品列表</summary>
    public List<ItemData> ProductItemList = new List<ItemData>();
    
    /// <summary>产品总数量</summary>
    public int ProductTotalNum;
    
    /// <summary>本地位置</summary>
    public Vector2 LocalPos = new Vector2(0, 0);
    
    /// <summary>手动停止</summary>
    public bool HandleStop;
    
    /// <summary>需要前置物品ID</summary>
    public int NeedForeItemId;
    
    /// <summary>单个丹药价格</summary>
    public int SingleDanPrice;
}

/// <summary>
/// 炼丹数据
/// </summary>
[Serializable]
public class LianDanData
{
    /// <summary>已解锁丹方ID列表</summary>
    public List<int> unlockedDanFangIdList = new List<int>();
    
    /// <summary>丹方熟练度列表</summary>
    public List<int> danFangProficiencyList = new List<int>();
    
    /// <summary>最大宝石稀有度</summary>
    public int MaxGemRarity;
}

/// <summary>
/// 装备制作团队数据
/// </summary>
[Serializable]
public class EquipMakeTeamData
{
    /// <summary>谁在做（角色唯一ID）</summary>
    public ulong peopleOnlyId;
    
    /// <summary>当前状态（工作/发呆）</summary>
    public int curStatus;
    
    /// <summary>当前正在修的装备</summary>
    public EquipProtoData curFixedEquip;
}


/// <summary>
/// 弟子使用仓库数据
/// </summary>
[Serializable]
public class SingleStudentUseCangKuData
{
    /// <summary>弟子唯一ID</summary>
    public ulong StudentOnlyId;
    
    /// <summary>弟子去仓库需求类型</summary>
    public int StudentGoCangKuNeedType;
    
    /// <summary>修为数量</summary>
    public int XiuWeiNum;
    
    /// <summary>总时间</summary>
    public int TotalTime;
    
    /// <summary>剩余时间</summary>
    public int RemainTime;
    
    /// <summary>丹田唯一ID</summary>
    public ulong FarmOnlyId;
    
    /// <summary>装备物品数据</summary>
    public ItemData EquipItemData;
    
    /// <summary>通用参数</summary>
    public string CommonParam = "";
    
    /// <summary>准备镶嵌宝石的装备</summary>
    public ItemData ReadyToInlayGemEquip;
}
