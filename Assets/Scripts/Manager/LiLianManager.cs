using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class LiLianManager : CommonInstance<LiLianManager>
{
    public bool liLianEnabled = false;//历练功能开关，用于测试
    public bool useMineMode = true;//使用矿洞挖掘玩法开关
    public List<PeopleData> curParticipatedPList = new List<PeopleData>();
    public List<ItemData> receivedItemList=new List<ItemData>();
    public ItemData totalItemData;
    public int totalHaoGanAdd;
    public int curLiLianId;

    /// <summary>
    /// 历练物品配置
    /// </summary>
    [System.Serializable]
    public class LiLianItemConfig
    {
        public int itemId;           // 物品ID
        public int weight;           // 权重
        public int minCount;         // 最少数量
        public int maxCount;         // 最多数量
    }

    /// <summary>
    /// 历练物品配置列表
    /// </summary>
    public static List<LiLianItemConfig> liLianItemConfigs = new List<LiLianItemConfig>
    {
        new LiLianItemConfig { itemId = (int)ItemIdType.LingShi, weight = 100, minCount = 50, maxCount = 150 },
        new LiLianItemConfig { itemId = 51110, weight = 30, minCount = 5, maxCount = 15 }, // 破碎红宝石 凡级
        new LiLianItemConfig { itemId = 51210, weight = 20, minCount = 3, maxCount = 8 },  // 破碎蓝宝石 凡级
        new LiLianItemConfig { itemId = 51310, weight = 15, minCount = 2, maxCount = 6 },  // 破碎绿宝石 凡级
        new LiLianItemConfig { itemId = 51410, weight = 25, minCount = 10, maxCount = 25 } // 破碎黄宝石 凡级
    };

    /// <summary>
    /// 设置历练开关
    /// </summary>
    public void SetLiLianEnabled(bool enabled)
    {
        liLianEnabled = enabled;
        Debug.Log("历练功能已" + (enabled ? "开启" : "关闭"));
    }

    /// <summary>
    /// 根据权重随机选择物品配置
    /// </summary>
    LiLianItemConfig GetRandomItemConfig()
    {
        int totalWeight = 0;
        foreach (var config in liLianItemConfigs)
        {
            totalWeight += config.weight;
        }

        int randomWeight = RandomManager.Next(0, totalWeight);
        int currentWeight = 0;
        
        foreach (var config in liLianItemConfigs)
        {
            currentWeight += config.weight;
            if (randomWeight < currentWeight)
            {
                return config;
            }
        }
        
        return liLianItemConfigs[0]; // 默认返回第一个
    }

    /// <summary>
    /// 生成历练和各个事件
    /// </summary>
    public SingleLiLianData OnGenerate(int liLianId,List<PeopleData> pList)
    {
        curLiLianId = liLianId;
        curParticipatedPList = pList;
        SingleLiLianData data = new SingleLiLianData();
        LiLianSetting setting = DataTable.FindLiLianSetting(liLianId);
        int eventNum = RandomManager.Next(4, 7);

        // 使用配置的物品系统
        LiLianItemConfig itemConfig = GetRandomItemConfig();
        int itemNum = RandomManager.Next(itemConfig.minCount, itemConfig.maxCount + 1);

        List<int> haoGanAdd = CommonUtil.SplitCfgOneDepth(setting.HaoGanAdd);
        int haoGanAddNum = RandomManager.Next(haoGanAdd[0], haoGanAdd[1]);

        if (pList.Count == 1)
            haoGanAddNum = 0;

        totalHaoGanAdd = haoGanAddNum;
        totalItemData = new ItemData();
        totalItemData.settingId = itemConfig.itemId;
        totalItemData.count = (ulong)itemNum;
        receivedItemList = new List<ItemData>();
        
        for (int i = 0; i < eventNum; i++)
        {
            if (itemNum > 0)
            {
                int singleItemNum = RandomManager.Next(0, Mathf.CeilToInt((itemNum+1)/2));
                if (i == eventNum - 1)
                    singleItemNum = itemNum;
                itemNum -= singleItemNum;
                if (singleItemNum > 0)
                {
                    ItemData ItemData = new ItemData();
                    ItemData.settingId = itemConfig.itemId;
                    ItemData.count = (ulong)singleItemNum;
                    data.itemList.Add(ItemData);
                }
                else
                {
                    data.itemList.Add(null);
                }
            }
            else
            {
                data.itemList.Add(null);
            }

            if (haoGanAddNum > 0)
            {
                int singleHaoGanAddNum = RandomManager.Next(0, haoGanAddNum + 1);
                if (i == eventNum - 1)
                    singleHaoGanAddNum = haoGanAddNum;
                haoGanAddNum -= singleHaoGanAddNum;
                data.haoGanList.Add(singleHaoGanAddNum);
            }
            else
            {
                data.haoGanList.Add(0);
            }
        }
        return data;
    }

    public bool isLiLian(int id) {
        if (!Instance.liLianEnabled)
        {
            if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex < 10)
            {
                PanelManager.Instance.OpenFloatWindow((RoleManager.Instance._CurGameInfo.playerPeople.trainIndex + 1) + "级开启");
                return true;
            }
        }
        LiLianSetting setting = DataTable.FindLiLianSetting(id);
        int timeIndex = DataTable._liLianList.IndexOf(setting);
        int curParticipatedNum = RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus[timeIndex];
        if (!Instance.liLianEnabled)
        {
            if (curParticipatedNum >= RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay)
            {
                PanelManager.Instance.OpenFloatWindow("今日参加历练次数已达上限");
                return false;
            }
        }
        else
        {
            PanelManager.Instance.OpenPanel<LiLianPanel>(PanelManager.Instance.trans_layer2, id);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 得奖
    /// </summary>
    public void OnGetAward(bool doubleGet)
    {
        if (curParticipatedPList.Count == 2)
        {
            PeopleData p1 = curParticipatedPList[0];
            PeopleData p2 = curParticipatedPList[1];
            //如不认识 则互相认识
            if (!p1.socializationData.knowPeopleList.Contains(p2.onlyId))
            {
                p1.socializationData.knowPeopleList.Add(p2.onlyId);
                p1.socializationData.haoGanDu.Add(0);

                p2.socializationData.knowPeopleList.Add(p1.onlyId);
                p2.socializationData.haoGanDu.Add(0);
            }
            StudentManager.Instance.ChangeHaoGanDu(p1, p2, totalHaoGanAdd);
            StudentManager.Instance.ChangeHaoGanDu(p2, p1, totalHaoGanAdd);


        }
        if (doubleGet)
        {
            totalItemData.count *= 2;
        }
            ItemManager.Instance.GetItemWithTongZhiPanel(totalItemData.settingId, totalItemData.count);

        for (int i = 0; i < receivedItemList.Count; i++)
        {
            if (doubleGet)
            {
                receivedItemList[i].count *= 2;
            }
            ItemManager.Instance.GetItemWithTongZhiPanel(receivedItemList[i].settingId, receivedItemList[i].count);

        }
        LiLianSetting setting = DataTable.FindLiLianSetting(curLiLianId);
        int timeIndex = DataTable._liLianList.IndexOf(setting);

        // 添加边界检查，防止数组越界
        if (timeIndex >= 0 && timeIndex < RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus.Count)
        {
            RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus[timeIndex]++;
        }
        else
        {
            Debug.LogWarning($"LiLianManager: timeIndex {timeIndex} 超出 TodayParticipatedLiLianStatus 数组范围，跳过更新");
        }
        EventCenter.Broadcast(TheEventType.RefreshLiLianShow);
        TaskManager.Instance.GetAchievement(AchievementType.LiLian, "1");//历练次数
    }

    /// <summary>
    /// 月卡增加历练次数
    /// </summary>
    public void OnMoonCardAddLiLianLimit()
    {
        RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay = 2;

    }

    /// <summary>
    /// 刷新今日历练时间
    /// </summary>
    public void RefreshTodayLiLianTime(long x)
    {
        RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus.Clear();
        RoleManager.Instance._CurGameInfo.timeData.LastParticipatedLiLianTime.Clear();
        for (int i = 0; i < DataTable._liLianList.Count; i++)
        {
            RoleManager.Instance._CurGameInfo.timeData.TodayParticipatedLiLianStatus.Add(0);
            RoleManager.Instance._CurGameInfo.timeData.LastParticipatedLiLianTime.Add(x);
        }
        InitMaxLiLianTime(x);

        EventCenter.Broadcast(TheEventType.RefreshLiLianShow);
    }

    /// <summary>
    /// 初始化最大历练次数
    /// </summary>
    /// <param name="x"></param>
    public void InitMaxLiLianTime(long x)
    {
        if (x > 0)
        {     //月卡
            if (ShopManager.Instance.CheckIfHaveMoonCard2(x) != null)
            {
                RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay = 2;
            }
            else
            {
                RoleManager.Instance._CurGameInfo.timeData.MaxLiLianTimePerDay = 1;
            }

        }
   
    }
}

/// <summary>
/// 单个历练数据
/// </summary>
public class SingleLiLianData
{
    public List<ItemData> itemList=new List<ItemData>();
    public List<int> haoGanList = new List<int>();
    public List<string> eventStrList = new List<string>();
} 