using System;
using System.Collections.Generic;
using cfg;
/// <summary>
/// 物品数据
/// </summary>
[Serializable]
public class ItemData
{
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>数量</summary>
    public ulong count;

    //宝石中不需装备数据
    /// <summary>装备数据</summary>
    //[NonSerialized]
    public EquipProtoData equipProtoData;
    
    /// <summary>宝石数据</summary>
    public GemData gemData;
    
    /// <summary>品质</summary>
    public int quality;

    //[NonSerialized]
    public ItemSetting setting;
}

/// <summary>
/// 装备数据
/// </summary>
[Serializable]
public class EquipProtoData
{
    /// <summary>唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>当前等级</summary>
    public int curLevel;
    
    /// <summary>当前经验</summary>
    public int curExp;
    
    /// <summary>当前耐久</summary>
    public int curDurability;
    
    /// <summary>属性ID列表</summary>
    public List<int> propertyIdList = new List<int>();
    
    /// <summary>属性列表</summary>
    public List<SinglePropertyData> propertyList = new List<SinglePropertyData>();

    /// <summary>宝石列表（运行时使用，不序列化）</summary>
    [NonSerialized]
    //[NonSerialized]
    public List<ItemData> gemList = new List<ItemData>();
    
    /// <summary>宝石数据列表（用于序列化）</summary>
    public List<GemSaveData> gemSaveList = new List<GemSaveData>();
    
    /// <summary>是否已装备</summary>
    public bool isEquipped;
    
    /// <summary>所属角色ID</summary>
    public ulong belongP;
    
    /// <summary>是否准备上架</summary>
    public bool isPrepareToWorld;
    
    /// <summary>优化率</summary>
    public int youHuaLv;
    
    /// <summary>精炼等级</summary>
    public int jingLianLv;

    //[NonSerialized]
    public EquipmentSetting setting;
    
    /// <summary>
    /// 刷新装备属性（精炼后调用）
    /// </summary>
    public void RefreshPro()
    {
        if (setting == null || propertyList == null || propertyList.Count == 0) return;
        
        // 根据精炼等级重新计算主属性加成
        int upgradeProAdd = setting.UpgradeProAdd.ToInt32();
        int addValue = upgradeProAdd * jingLianLv;
        
        // 属性刷新逻辑由 EquipmentManager 处理
    }
    
    /// <summary>
    /// 存档前调用：将 gemList 转换为 gemSaveList
    /// </summary>
    public void PrepareForSave()
    {
        gemSaveList.Clear();
        if (gemList == null) return;
        
        for (int i = 0; i < gemList.Count; i++)
        {
            ItemData gem = gemList[i];
            if (gem == null || gem.onlyId <= 0)
            {
                gemSaveList.Add(null);
            }
            else
            {
                GemSaveData saveData = new GemSaveData();
                saveData.settingId = gem.settingId;
                saveData.onlyId = gem.onlyId;
                saveData.quality = gem.quality;
                saveData.gemData = gem.gemData;
                gemSaveList.Add(saveData);
            }
        }
    }
    
    /// <summary>
    /// 加载后调用：将 gemSaveList 还原为 gemList
    /// </summary>
    public void RestoreAfterLoad()
    {
        gemList = new List<ItemData>();
        if (gemSaveList == null)
        {
            for (int i = 0; i < 4; i++)
                gemList.Add(null);
            return;
        }
        
        for (int i = 0; i < gemSaveList.Count; i++)
        {
            GemSaveData saveData = gemSaveList[i];
            if (saveData == null || saveData.onlyId <= 0)
            {
                gemList.Add(null);
            }
            else
            {
                ItemData gem = new ItemData();
                gem.settingId = saveData.settingId;
                gem.onlyId = saveData.onlyId;
                gem.quality = saveData.quality;
                gem.gemData = saveData.gemData;
                gemList.Add(gem);
            }
        }
    }
}

/// <summary>
/// 宝石数据
/// </summary>
[Serializable]
public class GemData
{
    /// <summary>是否已镶嵌</summary>
    public bool isInlayed;
    
    /// <summary>属性ID列表</summary>
    public List<int> propertyIdList = new List<int>();
    
    /// <summary>属性列表</summary>
    public List<SinglePropertyData> propertyList = new List<SinglePropertyData>();
    
    /// <summary>宝石配置ID</summary>
    public int gemSettingId;
}

/// <summary>
/// 宝石存档数据（用于序列化，避免循环引用）
/// </summary>
[Serializable]
public class GemSaveData
{
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>品质</summary>
    public int quality;
    
    /// <summary>宝石数据</summary>
    public GemData gemData;
}

/// <summary>
/// 物品模型（背包）
/// </summary>
[Serializable]
public class ItemModel
{
    /// <summary>物品ID列表</summary>
    public List<int> itemIdList = new List<int>();
    
    /// <summary>唯一ID列表</summary>
    public List<ulong> onlyIdList = new List<ulong>();
    
    /// <summary>物品数据列表</summary>
    public List<ItemData> itemDataList = new List<ItemData>();
    
    /// <summary>附灵石数量</summary>
    public long fuLingShiNum;
    
    /// <summary>仓库唯一ID列表</summary>
    public List<ulong> cangKuOnlyIdList = new List<ulong>();
    
    /// <summary>仓库物品ID列表</summary>
    public List<int> cangKuItemIdList = new List<int>();
    
    /// <summary>仓库物品数据列表</summary>
    public List<ItemData> cangKuItemDataList = new List<ItemData>();
}

/// <summary>
/// 所有装备数据
/// </summary>
[Serializable]
public class AllEquipmentData
{
    /// <summary>当前正在做的装备</summary>
    public EquipMakeData curEquipMakeData;
    
    /// <summary>已装备的装备列表</summary>
    public List<EquipProtoData> curEquippedEquipList = new List<EquipProtoData>();
    
    /// <summary>图纸列表</summary>
    public List<SingleEquipPictureData> pictureList = new List<SingleEquipPictureData>();
}

/// <summary>
/// 图纸数据
/// </summary>
[Serializable]
public class SingleEquipPictureData
{
    /// <summary>装备ID</summary>
    public int equipId;
    
    /// <summary>状态（是否做过了）</summary>
    public int status;
}

/// <summary>
/// 制作装备数据
/// </summary>
[Serializable]
public class EquipMakeData
{
    /// <summary>当前正在做的装备唯一ID</summary>
    public ulong onlyId;
    
    /// <summary>配置ID</summary>
    public int settingId;
    
    /// <summary>进度（做了几天）</summary>
    public int processDay;
    
    /// <summary>目标属性ID列表</summary>
    public List<int> proIdList = new List<int>();
    
    /// <summary>目标属性数值列表</summary>
    public List<int> targetProNumList = new List<int>();
    
    /// <summary>当前属性数值列表</summary>
    public List<int> curProNumList = new List<int>();
    
    /// <summary>谁在做</summary>
    public EquipMakeTeamData curTeamData;
    
    /// <summary>质量</summary>
    public int quality;
    
    /// <summary>哪个房做的</summary>
    public ulong farmOnlyId;
}
