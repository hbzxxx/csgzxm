using Framework.Data;
using Google.Protobuf.Collections;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using cfg;


/// <summary>
/// 物品管理
/// </summary>
public class ItemManager:CommonInstance<ItemManager>
{
    /// <summary>
    /// 获取物品 不可叠加
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public ItemData GetItem(ItemData data)
    {

        ItemData res = null;

        res = AddANewItem(data);

 
        return data;
    }

    /// <summary>
    /// 获取装备物品
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public ItemData GetItem(EquipProtoData equipData, ulong count,Quality quality)
    {
        ItemData res = null;
        //    int settingId = itemData.SettingId;
        //    UInt64 count = itemData.Count;
        int equipSettingId = equipData.settingId;
        EquipmentSetting equipSetting = DataTable.FindEquipSetting(equipSettingId);
        int settingId = equipSetting.ItemId.ToInt32();

   


        //找model里面有没有该id
         //如果不可叠加
        
        count = 1;
        res= AddANewItem(equipData, count);

        for(int i = 0; i < 4; i++)
        {
            equipData.gemList.Add(null);

        }
        res.quality =(int)quality;
        EventCenter.Broadcast(TheEventType.GetItem, settingId, count);

        return res;
    }
    #region 仓库

    /// <summary>
    /// 失去仓库物品 失去不可叠加的物品另外安排
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public bool LoseCangKuItem(int settingId, UInt64 count)
    {

        if (RoleManager.Instance._CurGameInfo.ItemModel == null)
        {
            Debug.LogError("没有初始化ItemModel");
            return false;
        }
        ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
        if (itemSetting == null)
        {
            Debug.LogError("没有这个id的物品" + settingId);
            return false;
        }


        //找model里面有没有该id
        //ItemModel itemModel = RoleManager.Instance._CurGameInfo.ItemModel;

        ItemData theItem = FindCangKuItemBySettingId(settingId);
        if (theItem == null)
        {
            Debug.LogError("想要失去一个不存在的仓库物品，id为" + settingId);
            return false;
        }
        ulong myItemCount = FindCangKuItemCount(settingId);
        if (count > myItemCount)
        {
       
                Debug.LogError("尝试失去超过限度的物品");
                return false;
            

        }

        ItemData deItem = FindOveryLayNotFullCangKuItemBySettingId(settingId);
        DecreaseCangKuItemCount(deItem, count);

        //KnapsackPanelMessage knapsackPanelMessage = new KnapsackPanelMessage();
        //knapsackPanelMessage.type = TheMessageType.LoseItem;
        //knapsackPanelMessage.settingId = settingId;
        //knapsackPanelMessage.count = count;
        //EventCenter.Call(EventDef.KnapsackPanel, knapsackPanelMessage);
        //发消息给ui
        EventCenter.Broadcast(TheEventType.LoseCangKuItem, (int)settingId, (ulong)count);

        return true;
    }
    /// <summary>
    /// 减少仓库物品数量
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="count"></param>
    void DecreaseCangKuItemCount(ItemData itemData, UInt64 count)
    {
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);
        UInt64 limit = itemSetting.MaxCount.ToUInt64();

        UInt64 curItemDecrease = Math.Min(itemData.count, count);
        UInt64 ultraDecrease = 0;//除了减掉该减掉的，还要额外减去这么多
        //减少的物品超出单个物品的数量
        if (itemData.count < count)
        {
            ultraDecrease = count - itemData.count;
        }
        itemData.count -= curItemDecrease;
        if (itemData.count == 0 && itemData.settingId != (int)ItemIdType.LingShi)
        {
            RemoveACangKuItem(itemData.onlyId);
        }
        //如果是堆叠物品还需要额外减少的数量
        if (itemSetting.OverLay.ToInt32() == 1
            && itemSetting.MaxOverLay.ToInt32() != 0
            && ultraDecrease > 0)
        {
            int deItemCount = (int)(ultraDecrease / (ulong)itemSetting.MaxOverLay.ToInt32());//需要删掉的物品个数
            ulong alongDeCount = ultraDecrease % (ulong)itemSetting.MaxOverLay.ToInt32();

            int totalItemCount = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Count;
            for (int i = totalItemCount - 1; i >= 0; i--)
            {
                int settingId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList[i];
                UInt64 onlyId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList[i];
                if (settingId == itemData.settingId)
                {
                    if (deItemCount > 0)
                    {
                        RemoveACangKuItem(onlyId);
                        deItemCount--;
                    }
                }
            }

            ItemData aloneItem = FindOveryLayNotFullCangKuItemBySettingId(itemData.settingId);
            aloneItem.count -= alongDeCount;
            if (aloneItem.count == 0)
                RemoveACangKuItem(aloneItem.onlyId);
        }

    }
    /// <summary>
    /// 得到仓库物品
    /// </summary>
    /// <param name="settingId"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public UInt64 GetCangKuItem(int settingId,UInt64 count)
    {
        //    int settingId = itemData.SettingId;
        //    UInt64 count = itemData.Count;
        UInt64 res = 0;
        if (RoleManager.Instance._CurGameInfo.ItemModel == null)
        {
            Debug.LogError("没有初始化ItemModel");
            return 0;
        }
        ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
        if (itemSetting == null)
        {
            Debug.LogError("没有这个id的物品" + settingId);
            return 0;
        }


        //找model里面有没有该id
        //ItemModel itemModel = RoleManager.Instance._CurGameInfo.ItemModel;
        //如果不可叠加
        if (itemSetting.OverLay.ToInt32() == 0)
        {
            count = 1;
            AddANewCangKuItem(settingId, count);
        }
        //如果可叠加
        else
        {
            //如果有同类物品 找未满堆叠上限的物品
            if (RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Contains(settingId))
            {
                ItemData notFullItem = FindOveryLayNotFullCangKuItemBySettingId(settingId);
                if (notFullItem == null)
                {
                    notFullItem = AddANewCangKuItem(settingId, 0);
                }
                AddCangKuItemCount(notFullItem, count);
                res = notFullItem.onlyId;

            }
            //如果没有同类物品
            else
            {
                ItemData theItem = AddANewCangKuItem(settingId, 0);

                AddCangKuItemCount(theItem, count);
                res = theItem.onlyId;

            }

        }

        EventCenter.Broadcast(TheEventType.GetCangKuItem, (int)settingId, (ulong)count);
        return res;
    }
    /// <summary>
    /// 增加一个新的仓库物品
    /// </summary>
    ItemData AddANewCangKuItem(int theSettingId, UInt64 theCount)
    {
        ItemData newItem = new ItemData();
        newItem.settingId = theSettingId;
        newItem.count = theCount;
        newItem.onlyId = ConstantVal.SetId;

        ItemSetting itemSetting = DataTable.FindItemSetting(theSettingId);
        if (itemSetting.Quality != "-1")
        {
            newItem.quality = itemSetting.Quality.ToInt32();
        }

        RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Add(theSettingId);
        RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Add(newItem);
        RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.Add(newItem.onlyId);

        return newItem;
    }
    /// <summary>
    /// 找可以堆叠的物品中，未满堆叠上限的仓库物品
    /// </summary>
    /// <returns></returns>
    public ItemData FindOveryLayNotFullCangKuItemBySettingId(int settingId)
    {
        ItemData target = null;
        int count = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Count;
        ItemSetting setting = DataTable.FindItemSetting(settingId);
        for (int i = count - 1; i >= 0; i--)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList[i];
            ItemData theData = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            ulong theCount = theData.count;
            if (theId == settingId)
            {
                if (target == null)
                {
                    //若无堆叠上限，或无法堆叠，则就是它了
                    if (DataTable.FindItemSetting(theId).OverLay.ToInt32() == 0
                        || DataTable.FindItemSetting(theId).MaxOverLay.ToInt32() == 0)
                    {
                        target = theData;
                     }
                    //若可以堆叠，且有堆叠上限，则也是他
                    else if (setting.OverLay.ToInt32() == 1
                        && setting.MaxOverLay != "0")
                    {
                        target = theData;
                    }
                }
                //继续找更小的
                else if (theCount < target.count)
                {
                    target = theData;
                    break;
                }
            }

        }

        return target;
    }
    /// <summary>
    /// 增加仓库物品数量
    /// </summary>
    void AddCangKuItemCount(ItemData itemData, UInt64 count)
    {
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);
 
        UInt64 limit = itemSetting.MaxCount.ToUInt64();
        UInt64 toAddCount = 0;
        //有限制
        if (limit != 0)
        {
            toAddCount = itemData.count + count;
            if (toAddCount >= limit)
                toAddCount = limit;
        }
        //无限制
        else
        {
            toAddCount = itemData.count + count;
        }
        //分割物品
        int maxOverLay = itemSetting.MaxOverLay.ToInt32();

        //如果规定了该物品的叠加上限
        if (maxOverLay > 0)
        {
            //增加后，该物品的数量大于了叠加上限
            if (toAddCount > (ulong)maxOverLay)
            {
                toAddCount = count - ((ulong)maxOverLay - itemData.count);

                itemData.count = (ulong)maxOverLay;

                //需要新增多少物品
                int newItemCount = (int)(toAddCount / (ulong)maxOverLay);
                ulong deCount = 0;
                for (int i = 0; i < newItemCount; i++)
                {
                    deCount += (ulong)maxOverLay;
                    AddANewCangKuItem(itemData.settingId, (ulong)maxOverLay);
                }

                toAddCount -= deCount;


                int aloneCount = (int)(toAddCount % (ulong)maxOverLay);

                AddANewCangKuItem(itemData.settingId, (ulong)aloneCount);

            }
            //增加后，该物品的数量不大于叠加上限
            else
            {
                itemData.count = toAddCount;
            }
        }
        //无叠加上限
        else
        {
            itemData.count = toAddCount;

        }
    }
    /// <summary>
    /// 失去一个唯一仓库物品
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public bool LoseCangKuItem(UInt64 onlyId)
    {
        ItemData theItem = FindCangKuItemByOnlyId(onlyId);
        RemoveACangKuItem(onlyId);
        EventCenter.Broadcast(TheEventType.LoseCangKuItem, (int)theItem.settingId, (ulong)theItem.count);
        return true;

    }
    /// <summary>
    /// 通过唯一id找仓库物品
    /// </summary>
    public ItemData FindCangKuItemByOnlyId(UInt64 onlyId)
    {
        int count = RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.Count;
        for (int i = 0; i < count; i++)
        {
            UInt64 theOnlyId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList[i];
            if (theOnlyId == onlyId)
                return RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
        }
        //Debug.Log("仓库没有该onlyId的物品" + onlyId);
        return null;
    }

    /// <summary>
    /// 移除某个仓库物品
    /// </summary>
    /// <param name="onlyId"></param>
    void RemoveACangKuItem(UInt64 onlyId)
    {
        //如果是钱 则不移

        int index = RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.IndexOf(onlyId);
        if (index == -1)
        {
            Debug.LogError("想要移除一个不存在的唯一id物品" + onlyId);
            return;
        }
        ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[index];
        RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.RemoveAt(index);
        RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.RemoveAt(index);
        RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.RemoveAt(index);
        EventCenter.Broadcast(TheEventType.GetItem, item.settingId, item.count);
    }


    /// <summary>
    /// 从仓库中移除一个物品
    /// </summary>
    /// <param name="onlyId"></param>
    public bool RemoveAItemFromCangKu(ulong onlyId)
    {
        //ItemData item= FindCangKuItemByOnlyId(onlyId);
        
        for(int i = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count-1; i >=0; i--)
        {
            ItemData item= RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            if (item.onlyId == onlyId)
            {
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.RemoveAt(i);
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.RemoveAt(i);
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.RemoveAt(i);
                ItemSetting itemSetting = DataTable.FindItemSetting(item.settingId);
                //能叠加
                if (itemSetting.OverLay == "1")
                {
                    GetItem(item.settingId,item.count);
                }
                else
                {
                    RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Add(item.settingId);
                    RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Add(item.onlyId);
                    RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Add(item);
                    EventCenter.Broadcast(TheEventType.GetItem, item.settingId, item.count);

                }
                EventCenter.Broadcast(TheEventType.LoseCangKuItem, (int)item.settingId, (ulong)item.count);

            }
        }
        return true;
    }
    /// <summary>
    /// 仓库中增加一个物品
    /// </summary>
    public void AddAItemToCangKu(ulong onlyId,ulong count)
    {
        ItemData item = FindItemByOnlyId(onlyId);
        if (item == null)
            return;
        if (item.equipProtoData != null && item.equipProtoData.isEquipped)
        {
            PanelManager.Instance.OpenFloatWindow("已被装备，请先卸下");
            return;
        }
        ItemSetting setting = DataTable.FindItemSetting(item.settingId);
        if (FindItemByOnlyId(onlyId) == null)
            return;
        //可叠加
        if (setting.OverLay == "1")
        {
            if (!CheckIfItemEnough(item.settingId, count))
            {
                PanelManager.Instance.OpenFloatWindow("没有足够多的物品");
                return;
            }
            if (LoseItem(item.settingId, count))
            {
                GetCangKuItem(item.settingId, count);
            }
        }
        //不可叠加
        else
        {
            if (LoseItem(item.onlyId))
            {
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Add(item.settingId);
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuOnlyIdList.Add(item.onlyId);
                RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Add(item);
                EventCenter.Broadcast(TheEventType.GetCangKuItem, (int)item.settingId, (ulong)1);
            }
        }
        TaskManager.Instance.GetDailyAchievement(TaskType.AddItemToCangKu, "1");
    }

    /// <summary>
    /// 通过配置id找仓库物品
    /// </summary>
    /// <returns></returns>
    public ItemData FindCangKuItemBySettingId(int settingId)
    {
        int count = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Count;
        for (int i = 0; i < count; i++)
        {
            int id = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList[i];
            if (id == settingId)
            {
                return RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 找该仓库物品数量
    /// </summary>
    public UInt64 FindCangKuItemCount(int settingId)
    {
        UInt64 res = 0;
        int count = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Count;
        for (int i = 0; i < count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList[i];
            UInt64 singleCount = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i].count;
            if (theId == settingId)
                res += singleCount;
        }
        return res;
    }

    /// <summary>
    /// 找仓库中的合适清灵草 （灵石）用玩家的
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuStudentUpgradeMatItem(PeopleData p)
    {
        List<ItemData> res = new List<ItemData>();
        if (p.talent != (int)StudentTalent.LianGong)
        {
            if (p.studentLevel < StudentManager.Instance.GetStudentLevelLimit(p))
            {
                StudentUpgradeSetting setting = DataTable._studentUpgradeList[p.studentLevel - 1];

                if (p.studentCurExp >= setting.NeedExp.ToInt32())
                {
                    List<List<int>> matNeed = CommonUtil.SplitCfg(setting.NeedMat);
                    bool valid = true;
                    for (int i = 0; i < matNeed.Count; i++)
                    {
                        List<int> singleMat = matNeed[i];
                        int costId = singleMat[0];
                        int costNum = singleMat[1];
                        if (costId == (int)ItemIdType.LingShi)
                        {
                            //找我有没有
                            if (!CheckIfItemEnough(costId, (ulong)costNum))
                            {
                                valid = false;
                                break;
                            }
                        }
                        else
                        {
                            //找仓库有没有
                            if (!CheckIfCangKuItemEnough(costId, (ulong)costNum))
                            {
                                valid = false;
                                break;
                            }
                        }
                        ItemData item = new ItemData();
                        item.settingId = costId;
                        item.count = (ulong)costNum;
                        res.Add(item);
                    }
                    if (!valid)
                        res.Clear();
                }
            }
        }
     
        

        return res;

    }

    //仓库里有没有合适的修为丹    
    public List<ItemData> CheckCangKuHaveValidXiuWeiDan(PeopleData p)
    {
        List<ItemData> candidateList = new List<ItemData>();

        if (p.talent ==(int) StudentTalent.LianGong)
        {
            int giantLevel = RoleManager.Instance.GiantLevel(p);// p.CurTrainIndex / 30 + 1;
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
            {
                ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
                ItemSetting setting = DataTable.FindItemSetting(item.settingId);
                if (setting.ItemType.ToInt32() == (int)ItemType.Dan
                    && setting.Param3.ToInt32() == giantLevel)
                {
                    candidateList.Add(item);
                }
            }
        }
      
        return candidateList;
            
    }
    /// <summary>
    /// 找仓库中的合适功法强化材料 （灵石）用玩家的
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuSkillUpgradeMatItem(PeopleData p)
    {
        List<ItemData> res = new List<ItemData>();
        if (p.allSkillData != null)
        {
            if (p.allSkillData.equippedSkillIdList.Count >= 2)
            {
                SingleSkillData skillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1], p.allSkillData);
                List<SkillUpgradeSetting> settingList = DataTable.FindSkillUpgradeListBySkillId(skillData.skillId);

                if (skillData.skillLevel < settingList.Count)
                {
                    List<ItemData> candidateList = new List<ItemData>();
                    List<ItemData> consumeList = SkillManager.Instance.GetSkillUpgradeConsume(skillData);
                    bool valid = true;
                    for (int i = 0; i < consumeList.Count; i++)
                    {
                        ItemData item = consumeList[i];
                        if (item.settingId == (int)ItemIdType.LingShi)
                        {
                            //找我有没有
                            if (!CheckIfItemEnough(item.settingId, (ulong)item.count))
                            {
                                valid = false;
                                break;
                            }
                        }
                        else
                        {
                            //找仓库有没有
                            if (!CheckIfCangKuItemEnough(item.settingId, (ulong)item.count))
                            {
                                valid = false;
                                break;
                            }
                        }
                        res.Add(item);
                    }
                    if (!valid)
                    {
                        res.Clear();
                    }
                }
            }

        }


        return res;

    }
    /// <summary>
    /// 找仓库中的合适强化材料 （灵石）用玩家的
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuIntenseMatItem(ItemData equipItem)
    {
        
        List<ItemData> candidateList = new List<ItemData>();

        //没满级
        if (equipItem!=null&&
            !EquipmentManager.Instance.IfEquipMaxLevel(equipItem.equipProtoData))
        {
            EquipmentSetting setting = DataTable.FindEquipSetting(equipItem.equipProtoData.settingId);
            List<List<List<int>>> allCostList = CommonUtil.SplitThreeCfg(setting.UpgradeCost);
            List<List<int>> curCost = allCostList[equipItem.equipProtoData.curLevel - 1];
            bool valid = true;
            for (int j = 0; j < curCost.Count; j++)
            {
                List<int> cost = curCost[j];
                int costId = cost[0];
                int costNum = cost[1];
                if (costId == (int)ItemIdType.LingShi)
                {
                    //找我有没有
                    if (!CheckIfItemEnough(costId, (ulong)costNum))
                    {
                        valid = false;
                        break;
                    }
                }
                else
                {
                    //找仓库有没有
                    if (!CheckIfCangKuItemEnough(costId, (ulong)costNum))
                    {
                        valid = false;
                        break;
                    }
                }
                ItemData item = new ItemData();
                item.settingId = costId;
                item.count = (ulong)costNum;
                candidateList.Add(item);
            }
            if (!valid)
            {
                candidateList.Clear();
            }

        }
 
        return candidateList;
    }

    /// <summary>
    /// 找仓库中符合血脉突破的材料 灵石用玩家的
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuXueMaiUpgradeItem(PeopleData p)
    {
        List<ItemData> candidateList = new List<ItemData>();

        int limit =XueMaiManager.Instance.limitLevel(p);
        if (p.xueMai != null)
        {
            for (int i = 0; i < p.xueMai.xueMaiTypeList.Count; i++)
            {
                bool valid = true;

                XueMaiType type = p.xueMai.xueMaiTypeList[i];
                int curLevel = p.xueMai.xueMaiLevelList[i];
                if (curLevel < limit)
                {
                    XueMaiUpgradeSetting xueMaiUpgradeSetting = DataTable.FindXueMaiUpgradeSettingByType(type);

                    List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(xueMaiUpgradeSetting.NeedItem);
                    List<List<int>> itemNeed = itemNeedParam[curLevel];

                    for (int j = 0; j < itemNeed.Count; j++)
                    {
                        List<int> single = itemNeed[j];
                        int id = single[0];
                        int num = single[1];
                        if (id == (int)ItemIdType.LingShi)
                        {
                            //找我有没有
                            if (!CheckIfItemEnough(id, (ulong)num))
                            {
                                valid = false;
                                break;
                            }
                        }
                        else
                        {
                            //找仓库有没有
                            if (!CheckIfCangKuItemEnough(id, (ulong)num))
                            {
                                valid = false;
                                break;
                            }
                        }
                        ItemData item = new ItemData();
                        item.settingId = id;
                        item.count = (ulong)num;
                        candidateList.Add(item);


                    }
                    if (!valid)
                    {
                        candidateList.Clear();
                    }
                    else
                    {
                        break;
                    }
                }
            }

        }

        return candidateList;
    }

    /// <summary>
    /// 找仓库中适合生产弟子吃的经验丹
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuAddProductExpItem(PeopleData p)
    {
        List<ItemData> res = new List<ItemData>();
        // 主角和所有弟子都可以使用经验丹
        if (StudentManager.Instance.GetStudentLevelLimit(p) > p.studentLevel)
        {
            int needExp = DataTable._studentUpgradeList[p.studentLevel - 1].NeedExp.ToInt32();
            if (p.studentCurExp < needExp)
            {
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
                {
                    ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
                    ItemSetting setting = DataTable.FindItemSetting(data.settingId);
                    if (setting.ItemType.ToInt32() == (int)ItemType.ProductExpDan)
                    {
                        List<int> levelRange = CommonUtil.SplitCfgOneDepth(setting.Param2);
                        if (p.studentLevel >= levelRange[0]
                            && p.studentLevel <= levelRange[1])
                        {
                            ItemData needItem = new ItemData();
                            needItem.settingId = data.settingId;
                            needItem.count = 1;
                            res.Add(needItem);
                        }
                    }
                }
            }
        }

        return res;
    }

    /// <summary>
    /// 找仓库中的宝石
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuGemItem()
    {
        List<ItemData> candidateList = new List<ItemData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(item.settingId);
            if (setting.ItemType.ToInt32() == (int)ItemType.Gem)
            {
                candidateList.Add(item);
            }
        }
        return candidateList;
    }
    /// <summary>
    /// 找仓库中的装备
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuEquipItem()
    {
        List<ItemData> candidateList = new List<ItemData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(item.settingId);
            if (setting.ItemType.ToInt32() == (int)ItemType.Equip)
            {
                candidateList.Add(item);
            }
        }
        return candidateList;
    }
    /// <summary>
    /// 找仓库中的技能书
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuSkillBookItem(PeopleData p)
    {
        List<ItemData> candidateList = new List<ItemData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(item.settingId);
            if (setting.ItemType.ToInt32() == (int)ItemType.SkillBook)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(setting.Id.ToInt32());
                if (skillSetting.YuanSu.ToInt32() == p.yuanSu)
                {
                    candidateList.Add(item);
                }
            }
        }
        return candidateList;
    }

    //仓库里有没有合适的突破丹
    public List<ItemData> CheckCangKuHaveValidPoJingDan(PeopleData p)
    {
        TrainSetting trainSetting = DataTable._trainList[p.trainIndex];
        //判断有没有该丹
        int danId = trainSetting.SuccessDanId.ToInt32();
        List<ItemData> candidateList = new List<ItemData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            if (item.settingId == danId)
            {
                candidateList.Add(item);

            }
     
        }
        return candidateList;

    }

    /// <summary>
    /// 找仓库某个东西够不够
    /// </summary>
    /// <returns></returns>
    public bool CheckIfCangKuItemEnough(int settingId, ulong num)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemIdList[i];
            ulong theNum = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i].count;
            if (theId == settingId)
            {
                if (theNum >= num)
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }


    /// <summary>
    /// 通过类型找物品
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindCangKuItemListByType(ItemType itemType)
    {
        List<ItemData> res = new List<ItemData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.cangKuItemDataList[i];
            int id = data.settingId;
            ItemSetting itemSetting = DataTable.FindItemSetting(id);
            if (itemSetting.ItemType.ToInt32() == (int)itemType)
            {
                res.Add(data);
            }
        }
        return res;
    }

    #endregion



    /// <summary>
    /// 获取物品 获取特定物品另外安排
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public UInt64 GetItem(int settingId,ulong count)
    {
        //    int settingId = itemData.SettingId;
        //    UInt64 count = itemData.Count;
        if (count == 0)
            return 0;
        UInt64 res = 0;
        if (RoleManager.Instance._CurGameInfo.ItemModel == null)
        {
            Debug.LogError("没有ItemModel");
            return 0;
        }
        ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
        if (itemSetting == null)
        {
            Debug.LogWarning($"[GetItem] 物品配置不存在，settingId={settingId}，已替换为金币");
            settingId = 10001;
            itemSetting = DataTable.FindItemSetting(settingId);
            if (itemSetting == null)
            {
                return 0;
            }
        }
   
        
        //找model里面有没有该id
        ItemModel ItemModel = RoleManager.Instance._CurGameInfo.ItemModel;
        //如果不可叠加
        if (itemSetting.OverLay.ToInt32() == 0)
        {
            count = 1;
            AddANewItem(settingId, count);
        }
        //如果可叠加
        else
        {
            //如果有同类物品 找未满堆叠上限的物品
            if (ItemModel.itemIdList.Contains(settingId))
            {
                //如果同类物品到达总上限
                if (itemSetting.MaxCount.ToInt32()!=0
                    &&FindItemCount(settingId) >= itemSetting.MaxCount.ToUInt64())
                {
                    Debug.Log("该物品" + itemSetting.Name + "已达最大数量限制");
                }
                else
                {
                    ItemData notFullItem = FindOveryLayNotFullItemBySettingId(settingId);
                    if (notFullItem == null)
                    {
                        notFullItem = AddANewItem(settingId, 0);
                    }
                    AddItemCount(notFullItem, count);
                    res = notFullItem.onlyId;
                }
           

            }
            //如果没有同类物品
            else
            {
                ItemData theItem = AddANewItem(settingId, 0);

                AddItemCount(theItem, count);
                res = theItem.onlyId;

            }

        }
     
        EventCenter.Broadcast(TheEventType.GetItem, settingId, count);
        EventCenter.Broadcast(TheEventType.ShowUnlockFarmPosStatus);

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
            SingleNPCData singleNPCData = TaskManager.Instance.FindNPCByOnlyId(onlyId);
            if (singleNPCData != null)
            {
                TaskManager.Instance.TryAccomplishTask(singleNPCData);

            }

        }
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.ReceiveItem);

        if (itemSetting.ItemType.ToInt32() == (int)ItemType.OP)
        {
            EquipmentManager.Instance.AddEquipPicture(itemSetting.Param.ToInt32());
        }
  
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
        //    if (p.talent == (int)StudentTalent.LianGong)
        //    {
        //        StudentManager.Instance.RefreshRedPointShow(p);
        //    }
        //}
        //XueMaiManager.Instance.RefreshRedPoint();
        return res;
    }

    /// <summary>
    /// 失去一个唯一物品
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public bool LoseItem(UInt64 onlyId)
    {
        ItemData theItem = FindItemByOnlyId(onlyId);
        RemoveAItem(onlyId);
        EventCenter.Broadcast(TheEventType.LoseItem, theItem.settingId, theItem.count);
        return true;

    }

    /// <summary>
    /// 失去物品 失去不可叠加的物品另外安排
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public bool LoseItem(int settingId,ulong count)
    {
        
        if (RoleManager.Instance._CurGameInfo.ItemModel == null)
        {
            Debug.LogError("没有ItemModel");
            return false;
        }
        ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
        if (itemSetting == null)
        {
            Debug.LogError("没有这个id的物品" + settingId);
            return false;
        }


        //找model里面有没有该id
        ItemModel ItemModel = RoleManager.Instance._CurGameInfo.ItemModel;

        ItemData theItem = FindItemBySettingId(settingId);
        if (theItem == null)
        {
            Debug.LogError("想要失去一个不存在的物品，id为" + settingId);
            return false;
        }
        ulong myItemCount = FindItemCount(settingId);
        if (count> myItemCount)
        {
            //如果是灵石 则触发负灵石属性
            if (settingId == (int)ItemIdType.LingShi)
            {
                ulong remainCount = count - myItemCount;
                count= myItemCount;
                ItemModel.fuLingShiNum -= (long)remainCount;
            }
            else
            {
                Debug.LogError("尝试失去超过限度的物品");
                return false;
            }
   
        }

        ItemData deItem= FindOveryLayNotFullItemBySettingId(settingId);
        DecreaseItemCount(deItem,count);

        //KnapsackPanelMessage knapsackPanelMessage = new KnapsackPanelMessage();
        //knapsackPanelMessage.type = TheMessageType.LoseItem;
        //knapsackPanelMessage.settingId = settingId;
        //knapsackPanelMessage.count = count;
        //EventCenter.Call(EventDef.KnapsackPanel, knapsackPanelMessage);
        //发消息给ui
        EventCenter.Broadcast(TheEventType.LoseItem, settingId, count);
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
            SingleNPCData singleNPCData = TaskManager.Instance.FindNPCByOnlyId(onlyId);
            if (singleNPCData != null)
            {
                TaskManager.Instance.TryAccomplishTask(singleNPCData);

            }

        }
        return true;
    }

    /// <summary>
    /// 减少物品数量
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="count"></param>
    public ulong DecreaseItemCount(ItemData itemData,UInt64 count)
    {
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);
        UInt64 limit = itemSetting.MaxCount.ToUInt64();

        UInt64 curItemDecrease = Math.Min(itemData.count,count);
        UInt64 ultraDecrease = 0;//除了减掉该减掉的，还要额外减去这么多
        //减少的物品超出单个物品的数量
        if (itemData.count < count )
        {
            ultraDecrease = count - itemData.count;
        }
        itemData.count -= curItemDecrease;
        if (itemData.count == 0&&itemData.settingId!=(int)ItemIdType.LingShi)
        {
            RemoveAItem(itemData.onlyId);
        }
        //如果是堆叠物品还需要额外减少的数量
        if (itemSetting.OverLay.ToInt32() == 1
            &&itemSetting.MaxOverLay.ToInt32()!=0
            &&ultraDecrease>0)
        {
            int deItemCount = (int)(ultraDecrease / (ulong)itemSetting.MaxOverLay.ToInt32());//需要删掉的物品个数
            ulong alongDeCount = ultraDecrease % (ulong)itemSetting.MaxOverLay.ToInt32();

            int totalItemCount = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count;
            for (int i = totalItemCount - 1; i >= 0; i--)
            {
                int settingId = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
                UInt64 onlyId = RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList[i];
                if (settingId == itemData.settingId)
                {
                    if (deItemCount > 0)
                    {
                        RemoveAItem(onlyId);
                        deItemCount--;
                    }
                }
            }

            ItemData aloneItem = FindOveryLayNotFullItemBySettingId(itemData.settingId);
            aloneItem.count -= alongDeCount;
            if (aloneItem.count == 0)
                RemoveAItem(aloneItem.onlyId);
        }
        return itemData.count;
     
    }

    ///// <summary>
    ///// 使用物品
    ///// </summary>
    //public UseItemReturnData UseItem(int settingId)
    //{
    //    UseItemReturnData res = new UseItemReturnData();
    //    if (!LoseItem(settingId, 1))
    //    {
    //        return res;
    //    }

    //    ItemSetting setting = DataTable.FindItemSetting(settingId);
    //    ItemType type = (ItemType)setting.itemType.ToInt32();
    
    //    switch (type)
    //    {
    //        case ItemType.RedBag:


    //            res.success = true;

    //            string[] moneyArr = setting.param.Split('|');
    //            int smallMoney = moneyArr[0].ToInt32();
    //            int bigMoney = moneyArr[1].ToInt32();
    //            int val = RandomManager.Next(smallMoney, bigMoney + 1);

    //            GetItem(SettingIdConstant.moneyId, (ulong)val);
    //            res.awardDataList = new List<AwardData>();

    //            AwardData awardData = new AwardData();
    //            awardData.awardType = AwardType.Item;
    //            awardData.awardId = SettingIdConstant.moneyId;
    //            awardData.awardCount = val;

    //            res.awardDataList.Add(awardData);

    //            break;
    //    }

    //    return res;

    //}
    
    /// <summary>
    /// 卖物品
    /// </summary>
    public void SellItem(List<UInt64> onlyIdList,List<ulong> numList)
    {
        for(int i = 0; i < onlyIdList.Count; i++)
        {
            UInt64 theOnlyId = onlyIdList[i];
            ulong num = numList[i];
            ItemData data = ItemManager.Instance.FindItemByOnlyId(theOnlyId);
            int settingId = data.settingId;
            LoseItem(settingId, num);
            ItemSetting setting = DataTable.FindItemSetting(settingId);

            ulong moneyGetNum = setting.Param.ToUInt64();
            ItemManager.Instance.GetItem((int)ItemIdType.LingShi, moneyGetNum);
        }
    }



    /// <summary>
    /// 找该物品数量
    /// </summary>
    public UInt64 FindItemCount(int settingId)
    {
        UInt64 res = 0;
        int count = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count;
        for (int i = 0; i < count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
            UInt64 singleCount = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i].count;
            if (theId == settingId)
                res += singleCount;
        }
        return res;
    }

    /// <summary>
    /// 移除某个物品
    /// </summary>
    /// <param name="onlyId"></param>
    void RemoveAItem(UInt64 onlyId)
    {
        //如果是钱 则不移

        int index = RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.IndexOf(onlyId);
        if (index == -1)
        {
            Debug.LogError("想要移除一个不存在的唯一id物品" + onlyId);
            return;
        }
        int settingId = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[index];
        if (settingId == (int)ItemIdType.LingShi)
        {
            //TODO做负钱
            Debug.Log("您的钱已见底");
            return;
        }
        RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.RemoveAt(index);
        RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.RemoveAt(index);
        RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.RemoveAt(index);

    }

 
    /// <summary>
    /// 通过配置id找某物品
    /// </summary>
    /// <returns></returns>
    public ItemData FindItemBySettingId(int settingId)
    {
        int count = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count;
        for (int i = 0; i < count; i++)
        {
            int id = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
            if (id == settingId)
            {
                return RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            }
        }
        return null;
    }
    /// <summary>
    /// 通过配置id找我是否有某物品
    /// </summary>
    /// <returns></returns>
    public bool CheckIfHaveItemBySettingId(int settingId)
    {
        int count = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count;
        for (int i = 0; i < count; i++)
        {
            int id = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
            if (id == settingId)
            {
                return true;
            }
        }
        return false;
    }
 
 
    /// <summary>
    /// 通过唯一id找物品
    /// </summary>
   public ItemData FindItemByOnlyId(UInt64 onlyId)
    {
        int count = RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Count;
        for(int i = 0; i < count; i++)
        {
            UInt64 theOnlyId = RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList[i];
            if (theOnlyId == onlyId)
                return RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
        }
        Debug.Log("没有该onlyId的物品" + onlyId);
        return null;
    }

    /// <summary>
    /// 找某个东西够不够
    /// </summary>
    /// <returns></returns>
    public bool CheckIfItemEnough(int settingId,ulong num)
    {
   
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
            ulong theNum = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i].count;
            if (theId == settingId)
            {
                if (theNum >= num)
                {
                    return true;
                }
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 增加物品数量
    /// </summary>
    void AddItemCount(ItemData itemData, UInt64 count)
    {
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);

        //灵石 从负的取
        if (itemData.settingId == (int)ItemIdType.LingShi)
        {
            long theNum = (long)(long)count + RoleManager.Instance._CurGameInfo.ItemModel.fuLingShiNum;
            RoleManager.Instance._CurGameInfo.ItemModel.fuLingShiNum += (long)(long)count;
            if (RoleManager.Instance._CurGameInfo.ItemModel.fuLingShiNum >= 0)
                RoleManager.Instance._CurGameInfo.ItemModel.fuLingShiNum = 0;
            if (theNum >= 0)
            {
                count = (ulong)(ulong)theNum;
            }
            else
            {
                count = 0;

            }
        }

        UInt64 limit = itemSetting.MaxCount.ToUInt64();
        UInt64 toAddCount = 0;
        //有限制
        if (limit != 0)
        {
            toAddCount = itemData.count + count;
            if (toAddCount >= limit)
                toAddCount = limit;
        }
        //无限制
        else
        {
            toAddCount = itemData.count + count;
        }
        //分割物品
        int maxOverLay = itemSetting.MaxOverLay.ToInt32();
        
        //如果规定了该物品的叠加上限
        if (maxOverLay > 0)
        {
            //增加后，该物品的数量大于了叠加上限
            if (toAddCount > (ulong)maxOverLay)
            {
                toAddCount = count - ((ulong)maxOverLay - itemData.count);

                itemData.count = (ulong)maxOverLay;
                //需要新增多少物品
                int newItemCount =(int)(toAddCount / (ulong)maxOverLay);
                ulong deCount = 0;
                for (int i = 0; i < newItemCount; i++)
                {
                    deCount += (ulong)maxOverLay;
                    AddANewItem(itemData.settingId, (ulong)maxOverLay);
                }

                toAddCount -= deCount;


                int aloneCount = (int)(toAddCount % (ulong)maxOverLay );
                    
                AddANewItem(itemData.settingId, (ulong)aloneCount);
                
            }
            //增加后，该物品的数量不大于叠加上限
            else
            {
                itemData.count = toAddCount;
            }
        }
        //无叠加上限
        else
        {
            itemData.count = toAddCount;

        }
    }



    /// <summary>
    /// 找可以堆叠的物品中，未满堆叠上限的物品
    /// </summary>
    /// <returns></returns>
    public ItemData FindOveryLayNotFullItemBySettingId(int settingId)
    {
        ItemData target = null;

        int count = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count;
        ItemSetting setting = DataTable.FindItemSetting(settingId);
        for (int i = count-1; i >=0; i--)
        {
            int theId = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i].settingId;
            ItemData theData = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            ulong theCount = theData.count;
            if (theId == settingId)
            {
                if (target == null)
                {
                    //若无堆叠上限，或无法堆叠，则就是它了
                    if (setting.OverLay.ToInt32() == 0
                        || setting.MaxOverLay.ToInt32() == 0)
                    {
                        target = theData;
                     }
                    //若可以堆叠，且有堆叠上限，则也是他
                    else if(setting.OverLay.ToInt32() == 1
                        &&setting.MaxOverLay!="0")
                    {
                        target = theData;
                    }
                }        
                //继续找更小的
                else if(theCount < target.count)
                {
                    target = theData;
                    break;
                }
            }
    
        }

        return target;
    }
    /// <summary>
    /// 增加一个唯一物品
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ItemData AddANewItem(ItemData item)
    {
        RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Add(item.settingId);
        RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Add(item);
        RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Add(item.onlyId);

        EventCenter.Broadcast(TheEventType.GetItem, item.settingId, item.count);
        return item;
    }
    /// <summary>
    /// 增加一个新的宝石 和品质有关
    /// </summary>
    /// <param name="gemData"></param>
    /// <param name="persistedProList"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public ItemData AddANewGem(int theItemSettingId,
        List<SinglePropertyData> persistedProList,
        bool inlay, Quality realquality)
    {
 
        ItemData newItem = new ItemData();
        newItem.settingId = theItemSettingId;
        newItem.count = 1;
        newItem.onlyId = ConstantVal.SetId;
 
        ItemSetting itemSetting = DataTable.FindItemSetting(theItemSettingId);
        newItem.setting = itemSetting;
        //宝石属性
        GemSetting gemSetting = DataTable.FindGemSetting(itemSetting.Param.ToInt32());
        newItem.gemData = new GemData();
        newItem.gemData.gemSettingId = theItemSettingId;
        //根据质量再受一次影响
        int proNum = 0;
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList[i];
        //    for (int j = 0; j < p.PropertyIdList.Count; j++)
        //    {
        //        int theId = p.PropertyIdList[j];
        //        if (theId == (int)PropertyIdType.LianShi)
        //        {
        //            proNum += p.PropertyList[j].Num;
        //        }
        //    }
        //}
        //Quality realquality = GetGemQualityByPro(proNum);
        int gemProNum = gemSetting.ProRange.ToInt32();
        int gemProId = gemSetting.ProId.ToInt32();
        gemProNum = Mathf.RoundToInt(gemProNum * (1 + ((int)realquality - 1) * 0.2f));
        if (inlay)
        {
            newItem.gemData.isInlayed = true;
        }
        //是合成宝石，保留一部分属性并增加部分属性
        if (persistedProList != null)
        {
           
            for(int i = 0; i < persistedProList.Count; i++)
            {
                newItem.gemData.propertyIdList.Add(persistedProList[i].id);
                newItem.gemData.propertyList.Add(persistedProList[i]);
            }
            //增加一个新属性
            //List<List<int>> proRange = CommonUtil.SplitCfg(gemSetting.proRange);
            //int index = (int)quality - 1;
            //List<int> choosedRange = proRange[index];



            SinglePropertyData newPro = new SinglePropertyData();
            newPro.quality = (int)realquality;
            newPro.id = gemProId;
            newPro.num = gemProNum;
            newItem.gemData.propertyIdList.Add(gemProId);
            newItem.gemData.propertyList.Add(newPro);
        }
        else
        {
            //增加level个新属性
            int level = gemSetting.Level.ToInt32();
            for(int i = 0; i < level; i++)
            {
          
                SinglePropertyData newPro = new SinglePropertyData();
                newPro.id = gemProId;
                newPro.num = gemProNum;
                newPro.quality= (int)realquality;
                newItem.gemData.propertyIdList.Add(gemProId);
                newItem.gemData.propertyList.Add(newPro);
            }
        }
        newItem.quality = itemSetting.Quality.ToInt32();

        RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Add(theItemSettingId);
        RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Add(newItem);
        RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Add(newItem.onlyId);
        EventCenter.Broadcast(TheEventType.GetItem, theItemSettingId, (ulong)1);

        TaskManager.Instance.TryAccomplishAllTask();
        return newItem;
    }

    /// <summary>
    /// 增加一个新物品
    /// </summary>
    ItemData AddANewItem(int theSettingId, UInt64 theCount)
    {
        ItemData newItem = new ItemData();
        newItem.settingId = theSettingId;
        newItem.count = theCount;
        newItem.onlyId = ConstantVal.SetId;

        ItemSetting itemSetting = DataTable.FindItemSetting(theSettingId);
        if (itemSetting.Quality != "-1")
        {
            newItem.quality = itemSetting.Quality.ToInt32();
        }
        if (itemSetting.ItemType.ToInt32() == (int)ItemType.Building)
        {
            ZongMenManager.Instance.SendBuilding(itemSetting.Param.ToInt32());
        }
        newItem.setting = itemSetting;
        RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Add(theSettingId);
        RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Add(newItem);
        RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Add(newItem.onlyId);

        return newItem;
    }
    /// <summary>
    /// 增加一个新装备
    /// </summary>
    ItemData AddANewItem(EquipProtoData equipData, UInt64 theCount)
    {
        EquipmentSetting equipSetting = DataTable.FindEquipSetting(equipData.settingId);
        int settingId = equipSetting.ItemId.ToInt32();

        ItemData newItem = new ItemData();
        newItem.settingId = settingId;
        newItem.count = theCount;
        newItem.onlyId = equipData.onlyId;
        newItem.equipProtoData = equipData;
        newItem.setting = DataTable.FindItemSetting(settingId);


        RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Add(newItem.settingId);
        RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Add(newItem);
        RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Add(newItem.onlyId);

        return newItem;
    }




    ///// <summary>
    ///// 使用物品
    ///// </summary>
    //public void UseItem(int settingId,UInt64 count)
    //{
    //    //免广告次数
    //    if (settingId == SettingIdConstant.item_freeADTime)
    //    {
    //        if (LoseItem(settingId, count))
    //        {

    //        }

    //    }
    //}

    /// <summary>
    /// 得到物品且显示
    /// </summary>
    public void GetItemWithAwardPanel(List<int> settingIdList,List<ulong> numList)
    {
        List<ItemData> dataList = new List<ItemData>();
        for(int i = 0; i < settingIdList.Count; i++)
        {
            int settingId = settingIdList[i];
            ulong num = numList[i];
            if (num <= 0)
                continue;
            GetItem(settingId, num);
            ItemData item = new ItemData();
            item.settingId = settingId;
            item.count = num;
            if (DataTable.FindItemSetting(item.settingId) == null) continue;
            dataList.Add(item);
        }


        PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, dataList);
    }

    /// <summary>
    /// 得到物品且显示
    /// </summary>
    public void GetItemWithAwardPanel(List<ItemData> dataList)
    {
        // for (int i = 0; i < settingIdList.Count; i++)
        //{
        //    int settingId = settingIdList[i];
        //    ulong num = numList[i];
        //    GetItem(settingId, num);
        //    ItemData item = new ItemData();
        //    item.settingId = settingId;
        //    item.count = num;
        //    dataList.Add(item);
        //}
        for(int i = 0; i < dataList.Count; i++)
        {
            GetItem(dataList[i].settingId, dataList[i].count);
        }

        PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, dataList);
    }
    /// <summary>
    /// 得到物品且显示
    /// </summary>
    public void GetItemWithTongZhiPanel(int settingId, ulong num)
    {
        GetItem(settingId, num);


        PanelManager.Instance.AddTongZhi(TongZhiType.Consume,"",ConsumeType.Item, (int)settingId, (int)(int)num);
    }
    /// <summary>
    /// 通过类型找物品
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindItemListByType(ItemType itemType,int rarity=0)
    {
        List<ItemData> res = new List<ItemData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            int id = data.settingId;
            ItemSetting itemSetting = DataTable.FindItemSetting(id);
            if (itemSetting!=null && itemSetting.ItemType.ToInt32() == (int)itemType
                &&(rarity==0||rarity==itemSetting.Rarity.ToInt32()))
            {
                res.Add(data);
            }
        }
        return res;
    }
  
    /// <summary>
    /// 找合适的修为丹
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindValidXiuWeiDan(PeopleData p)
    {
        
        List<ItemData> res = new List<ItemData>();
        if (p.talent == (int)StudentTalent.LianGong
            ||p.onlyId==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
        {
            int giantLevel = RoleManager.Instance.GiantLevel(p);// RoleManager.Instance._CurGameInfo.playerPeople.CurTrainIndex / 30 + 1;
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
            {
                ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
                int id = data.settingId;
                ItemSetting itemSetting = DataTable.FindItemSetting(id);
                if (itemSetting!=null && itemSetting.ItemType.ToInt32() == (int)ItemType.Dan
                    && itemSetting.Param3.ToInt32() == giantLevel)
                {
                    res.Add(data);
                }
            }

        }
        // 主角和所有弟子都可以使用经验丹
        if (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
        {
            int levelLimit = StudentManager.Instance.GetStudentLevelLimit(p);
            if (p.studentLevel < levelLimit)
            {
                int needExp = DataTable._studentUpgradeList[p.studentLevel - 1].NeedExp.ToInt32();
                if (p.studentCurExp < needExp)
                {
                    for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
                    {
                        ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
                        ItemSetting setting = DataTable.FindItemSetting(data.settingId);
                        if (setting!=null && setting.ItemType.ToInt32() == (int)ItemType.ProductExpDan)
                        {
                            List<int> levelRange = CommonUtil.SplitCfgOneDepth(setting.Param2);
                            if (p.studentLevel >= levelRange[0]
                                && p.studentLevel <= levelRange[1])
                            {
                                //ItemData needItem = new ItemData();
                                //needItem.settingId = data.settingId;
                                //needItem.count = 1;
                                res.Add(data);
                            }
                        }
                    }
                }
            }
        }
      
        return res;
    }

    /// <summary>
    /// 找合适的调息丹
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindValidTiaoXiDan(PeopleData p)
    {
        List<ItemData> res = new List<ItemData>();
        if (p.talent == (int)StudentTalent.LianGong
            || p.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
        {
            int giantLevel = RoleManager.Instance.GiantLevel(p);// RoleManager.Instance._CurGameInfo.playerPeople.CurTrainIndex / 30 + 1;
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
            {
                ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
                int id = data.settingId;
                ItemSetting itemSetting = DataTable.FindItemSetting(id);
                if (itemSetting.ItemType.ToInt32() == (int)ItemType.TiaoXiDan)
                {
                    res.Add(data);
                }
            }

        }
 
        return res;
    }

    /// <summary>
    /// 找合适的修为丹 根据大境界
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindValidXiuWeiDan(int giantLevel)
    {
        List<ItemData> res = new List<ItemData>();
         for (int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            int id = data.settingId;
            ItemSetting itemSetting = DataTable.FindItemSetting(id);
            if (itemSetting.ItemType.ToInt32() == (int)ItemType.Dan
                && itemSetting.Param3.ToInt32() == giantLevel)
            {
                res.Add(data);
            }
        }
        return res;
    }
    /// <summary>
    /// 合成宝石
    /// </summary>
    public void OnCompositeGem(UInt64 gem,UInt64 mat,SingleDanFarmData farmData)
    {
        long before = RoleManager.Instance.CalcZhanDouLi();

        ItemData gemItem = FindItemByOnlyId(gem);
        ItemData matItem = FindItemByOnlyId(mat);
        ItemSetting setting = DataTable.FindItemSetting(gemItem.settingId);
        GemSetting gemSetting = DataTable.FindGemSetting(setting.Param.ToInt32());
        if (gemSetting.NextGem!="-1")
        {
            int nextGemId= gemSetting.NextGem.ToInt32();
            GemSetting nextGemSetting = DataTable.FindGemSetting(nextGemId);

            int lingShiNeed = nextGemSetting.Consume.ToInt32();
            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)lingShiNeed))
            {
                        ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
                return;
            }
            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)lingShiNeed);

            int nextItemId = nextGemSetting.ItemId.ToInt32();
            //材料和它本身要消耗掉
            LoseItem(mat);
            LoseItem(gem);
            //如果是已镶嵌的，则从装备中卸掉
            if (matItem.gemData.isInlayed)
            {
                List<ItemData> equipItemList = FindItemListByType(ItemType.Equip);
                for(int i = 0; i < equipItemList.Count; i++)
                {
                    EquipProtoData EquipProtoData = equipItemList[i].equipProtoData;
                    for(int j = 0; j < EquipProtoData.gemList.Count; j++)
                    {
                        if (EquipProtoData.gemList[j] == null || EquipProtoData.gemList[j].onlyId <= 0)
                            continue;
                        UInt64 onlyId = EquipProtoData.gemList[j].onlyId;
                        if (onlyId == mat)
                        {
                            EquipProtoData.gemList[j] =null;
                            break;
                        }
                    }

                }
            }
            //已有宝石的属性保留，并增加一条新的
            List<SinglePropertyData> proList = new List<SinglePropertyData>();
            for(int i=0;i < gemItem.gemData.propertyList.Count; i++)
            {
                proList.Add(gemItem.gemData.propertyList[i]);
            }

            ItemData newGem = null;
            int totalNum = 0;
            //经验获取
            Rarity rarity = (Rarity)setting.Rarity.ToInt32();
            int expGet =LianDanManager.Instance.StudentGemExpGet(rarity);
            List<int> beforeExpList = new List<int>();

            List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(farmData);
            List<PeopleData> validStudentList = new List<PeopleData>();
            for (int i = 0; i < studentList.Count; i++)
            {
                PeopleData p = studentList[i];
                if (p.talent == (int)StudentTalent.BaoShi)
                {
                    beforeExpList.Add(p.studentCurExp);
                    StudentManager.Instance.OnGetStudentExp(p, expGet);
                    totalNum += RoleManager.Instance.FindProperty((int)PropertyIdType.LianShi, p).num;
                    validStudentList.Add(p);
                }
            }
            Quality quality = LianDanManager.Instance.GetGemQualityByPro(totalNum);
            //如果是已镶嵌的，则把唯一id改成新物品
            if (gemItem.gemData.isInlayed)
            {
                newGem = AddANewGem(nextItemId, proList,true, quality);
                List<ItemData> equipItemList = ItemManager.Instance.FindItemListByType(ItemType.Equip);
                for (int i = 0; i < equipItemList.Count; i++)
                {
                    EquipProtoData EquipProtoData = equipItemList[i].equipProtoData;
                    for (int j = 0; j < EquipProtoData.gemList.Count; j++)
                    {
                        if (EquipProtoData.gemList[j] == null || EquipProtoData.gemList[j].onlyId <= 0)
                            continue;
                        UInt64 onlyId = EquipProtoData.gemList[j].onlyId;
                        if (onlyId == gem)
                        {
                            ItemManager.Instance.LoseItem(onlyId);
                            EquipProtoData.gemList[j] = newGem;
                            break;
                        }
                    }
                    if (EquipProtoData.isEquipped)
                    {
                        RoleManager.Instance.RefreshBattlePro(RoleManager.Instance._CurGameInfo.playerPeople);

                    }

                }
            }
            else
            {
                newGem= AddANewGem(nextItemId, proList, false, quality);
            }

         
            

            ItemData showItem = new ItemData();
            showItem.settingId = nextItemId;
            showItem.count = 1;
            List<ItemData> itemDataList = new List<ItemData> { showItem };
            EventCenter.Broadcast(TheEventType.CompositeGem, newGem.onlyId);
            //弹窗
            PanelManager.Instance.OpenPanel<GemCompositeResPanel>(PanelManager.Instance.trans_layer2, 
                gemItem.gemData.propertyList,
               newGem.gemData.propertyList,
               beforeExpList,
               validStudentList,gemItem.gemData);
        }
        long after = RoleManager.Instance.CalcZhanDouLi();
        PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);

        //TalkingDataGA.OnEvent("合成宝石", new Dictionary<string, object>() { { setting.name, 1 } });
        TaskManager.Instance.GetDailyAchievement(TaskType.CompositeGem, "1");
    }

    /// <summary>
    /// 通过id得到宝石属性最大值
    /// </summary>
    /// <returns></returns>
    public int GetGemProLimitById(int gemId)
    {
        int res = 0;
        if (gemId != 0)
        {
            GemSetting setting = DataTable.FindGemSetting(gemId);
            res = setting.ProRange.ToInt32();
            res = Mathf.RoundToInt(res * (1 + ((int)5 - 1) * 0.2f));
        }
    
        //gemProNum = Mathf.RoundToInt(gemProNum * (1 + ((int)realquality - 1) * 0.2f));
        return res;
        //switch (propertyIdType)
        //{
        //    case PropertyIdType.Attack:
        //        return 27;
        //    case PropertyIdType.Defense:
        //        return 33;
        //    case PropertyIdType.Hp:
        //        return 20;
        //    case PropertyIdType.CritRate:
        //        return 17;
        //    case PropertyIdType.CritNum:
        //        return 35;
        //    case PropertyIdType.MPSpeed:
        //        return 6;
        //    case PropertyIdType.JingTong:
        //        return 108;
        //}


        //return 0;
    }
    /// <summary>
    /// 把散物品合成整物品
    /// </summary>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public List<ItemData> CombineItemList(List<ItemData> itemList)
    {
        List<ItemData> res = new List<ItemData>();
        List<int> idList = new List<int>();
        for(int i = 0; i < itemList.Count; i++)
        {
            ulong theNum = itemList[i].count;
            int theId = itemList[i].settingId;
            ItemSetting setting = DataTable.FindItemSetting(theId);

            if (setting.OverLay == "1")
            {
                if (idList.Contains(theId))
                {
                    int index = idList.IndexOf(theId);
                    res[index].count += theNum;
                }
                else
                {
                    idList.Add(theId);
                    ItemData itemData = new ItemData();
                    itemData.settingId = theId;
                    itemData.count = theNum;
                    res.Add(itemData);
                }
            }
            else
            {
                idList.Add(theId);
                ItemData itemData = new ItemData();
                itemData.settingId = theId;
                itemData.count = theNum;
                res.Add(itemData);
            }
        }
        return res;
    }

    /// <summary>
    /// 把散钱丹合成整钱丹
    /// </summary>
    /// <param name="itemList"></param>
    /// <returns></returns>
    public List<ItemData> CombineMoneyDanItemList(List<ItemData> itemList)
    {
        List<ItemData> res = new List<ItemData>();
        for (int i = 0; i < itemList.Count; i++)
        {
            ItemData theItem = itemList[i];
            ulong theNum = theItem.count;
            //int theId = theItem.SettingId;
            //ItemSetting setting = DataTable.FindItemSetting(theId);
            bool haveSameQualityAndId = false;
            for(int j = 0; j < res.Count; j++)
            {
                ItemData combinded = res[j];
                if(combinded.settingId==theItem.settingId
                    && combinded.quality == theItem.quality)
                {
                    haveSameQualityAndId = true;
                    combinded.count += theNum;
                }
            }
            if (!haveSameQualityAndId)
            {
                res.Add(theItem);
            }
           
        }
        return res;
    }

    /// <summary>
    /// 得到所有售价
    /// </summary>
    /// <returns></returns>
    public int GetTotalSellPrice(List<ItemData> list)
    {
        int val = 0;
        for(int i = 0; i < list.Count; i++)
        {
            ItemData theData = list[i];
            int settingId = theData.settingId;
            ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
            int price = itemSetting.Price.ToInt32();
            int lingShiNum = Mathf.RoundToInt(price * (1 + (theData.quality - 1) * 0.2f));

            val += (lingShiNum * (int)theData.count);
        }
        return val;
    }



    //private static ItemIdType TestDrawEnumByChinese(ItemIdType value, GUIContent label)
    //{
    //    Type targetEnumType = typeof(ItemIdType);
    //    LabelTextAttribute targetEnumLabelTextAttribute = targetEnumType.GetAttribute<LabelTextAttribute>();

    //    FieldInfo[] fieldInfos = targetEnumType.GetFields();

    //    string[] targetItemNames = Enum.GetNames(targetEnumType);

    //    string finalShowName = "";
    //    for (int i = 0; i < targetItemNames.Length; i++)
    //    {
    //        if (targetItemNames[i] == value.ToString())
    //        {
    //            finalShowName = value.ToString();
    //            break;
    //        }
    //    }

    //    for (int i = 0; i < fieldInfos.Length; i++)
    //    {
    //        if (fieldInfos[i].Name == finalShowName)
    //        {
    //            finalShowName = fieldInfos[i].GetAttribute<LabelTextAttribute>().Text;
    //            break;
    //        }
    //    }

    //    return EnumSelector<ItemIdType>.DrawEnumField(new GUIContent(targetEnumLabelTextAttribute.Text), new GUIContent(finalShowName), value);
    //}


    /// <summary>
    /// 所有弟子的造化属性
    /// </summary>
    /// <returns></returns>
    public Quality GetAllZaoHuaQuality()
    {
        //根据质量再受一次影响
        int proNum = 0;
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.EquipMakeStudentList[i];
        //    for (int j = 0; j < p.PropertyIdList.Count; j++)
        //    {
        //        int theId = p.PropertyIdList[j];
        //        if (theId == (int)PropertyIdType.ZaoHua)
        //        {
        //            proNum += p.PropertyList[j].Num;
        //        }
        //    }
        //}
        TestStudentTotalProInfluenceSetting setting = DataTable.FindtestStudentTotalProInfluenceByPro(proNum);

        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);
        List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };


        int index = CommonUtil.GetIndexByWeight(weightList);
        Quality res = qualityList[index];
        return res;
    }
    /// <summary>
    /// 分解
    /// </summary>
    /// <param name="item"></param>
    public void OnDecomposite(ItemData item,SingleDanFarmData farmData)
    {
        //最初保留50% 根据弟子数量决定保留多少
        ItemSetting itemSetting = DataTable.FindItemSetting(item.settingId);
        //如果是装备则卸下宝石 再卸下 再失去
        //LoseItem()
        List<int> returnItemIdList = new List<int>();
        List<int> returnItemNumList = new List<int>();
        List<int> realReturnItemNumList = new List<int>();
        //这里记得卸下宝石
        if (itemSetting.ItemType.ToInt32() == (int)ItemType.Equip)
        {
            EquipmentSetting equipSetting = DataTable.FindEquipSetting(item.equipProtoData.settingId);

      
            //先把制作的结了
            List<List<int>> makeCost = CommonUtil.SplitCfg(equipSetting.MakeCost);
            for(int i = 0; i < makeCost.Count; i++)
            {
                List<int> theMat = makeCost[i];
                if (!returnItemIdList.Contains(theMat[0]))
                {
                    returnItemIdList.Add(theMat[0]);
                    returnItemNumList.Add(0);
                }
                int index = returnItemIdList.IndexOf(theMat[0]);
                returnItemNumList[index] += theMat[0];
            }
            List<List<List<int>>> allIntenseList = CommonUtil.SplitThreeCfg(equipSetting.UpgradeCost);

            for (int i = 0; i < item.equipProtoData.curLevel-1; i++)
            {
                List<List<int>> intenseCost = allIntenseList[i];
                for (int j = 0; j < intenseCost.Count; j++)
                {
                    List<int> cost = intenseCost[j];
                    if (!returnItemIdList.Contains(cost[0]))
                    {
                        returnItemIdList.Add(cost[0]);
                        returnItemNumList.Add(0);
                    }
                    int index = returnItemIdList.IndexOf(cost[0]);
                    returnItemNumList[index] += cost[0];

                }
            }
            for(int i = 0; i < returnItemNumList.Count; i++)
            {
                int theNum = returnItemNumList[i];

                theNum = Mathf.RoundToInt((theNum * (1 + ((int)GetAllZaoHuaQuality() - 1) * 0.2f)));
                realReturnItemNumList.Add(theNum);
            }


        }
        else if(itemSetting.ItemType.ToInt32() == (int)ItemType.Gem)
        {
            if (item.gemData.isInlayed)
            {
                PanelManager.Instance.OpenFloatWindow("该宝石已镶嵌，请先卸下宝石");
                return;
            }
            GemSetting gemSetting = DataTable.FindGemSetting(itemSetting.Id.ToInt32());
            int gemLevel = gemSetting.Level.ToInt32();
            int gemCount=Mathf.RoundToInt(Mathf.Pow(2, gemLevel-1));

            for(int i=0;i< gemCount; i++)
            {
                List<List<int>> makeCost = CommonUtil.SplitCfg(gemSetting.Consume);
                for(int j = 0; j < makeCost.Count; j++)
                {
                    //if (!returnItemIdList.Contains(theMat[0]))
                    //{
                    //    returnItemIdList.Add(theMat[0]);
                    //    returnItemNumList.Add(0);
                    //}
                    //int index = returnItemIdList.IndexOf(theMat[0]);
                    //returnItemNumList[index] += theMat[0];
                }
              
            }
        }
        
    }

    /// <summary>
    /// 失去物品给通知
    /// </summary>
    /// <param name="id"></param>
    /// <param name="num"></param>
    public void LoseItemWithTongZhi(int id,ulong num,string des="")
    {
        LoseItem(id, num);
        int theNum = -(int)num;
        RecordManager.Instance.AddTongZhi(des, ConsumeType.Item, id, theNum);
    }

    /// <summary>
    /// 找灵石数量
    /// </summary>
    /// <returns></returns>
    public long FindLingShiCount()
    {
        long res = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemIdList.Count; i++)
        {
            int id = RoleManager.Instance._CurGameInfo.ItemModel.itemIdList[i];
            if (id == (int)ItemIdType.LingShi)
            {
                res += (long)(ulong)RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i].count;
            }
        }

        res += RoleManager.Instance._CurGameInfo.ItemModel.fuLingShiNum;
        return res;
    }

    /// <summary>
    /// 出售
    /// </summary>
    public void SellItem(ItemData item,ulong count)
    {
        ItemSetting setting = DataTable.FindItemSetting(item.settingId);
        int singlePrice = setting.Price.ToInt32();
        if (setting.OverLay == "1")
        {
            ItemManager.Instance.LoseItem(item.settingId,count);
            ItemManager.Instance.GetItem((int)ItemIdType.LingShi, (ulong)singlePrice * count);
        }
        else
        {
            ItemManager.Instance.LoseItem(item.onlyId);
            ItemManager.Instance.GetItem((int)ItemIdType.LingShi,1);
        }
        EventCenter.Broadcast(TheEventType.CloseItemTipsPanel);
    }

 

    /// <summary>
    /// 物品特殊描述
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public string ItemSpecialDes(ItemData item)
    {
        ItemSetting itemSetting = DataTable.FindItemSetting(item.settingId);
        string res = "";
        switch ((ItemType)itemSetting.ItemType.ToInt32())
        {
            case ItemType.Dan:
                 res=("可增加" + itemSetting.Param);
                 break;

            default:
                res = "";
                break;
        }
        return res;
    }


    /// <summary>
    /// 找某个品级，超过某个等级的宝石数量
    /// </summary>
    /// <returns></returns>
    public int FindRarityMoreThanLevelGemNum(Rarity rarity,int level)
    {
        int res = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(item.settingId);
            if (setting.ItemType.ToInt32() == (int)ItemType.Gem)
            {
                 if (setting.Rarity.ToInt32() >= (int)rarity)
                {
                    GemSetting gemSetting = DataTable.FindGemSetting(setting.Param.ToInt32());
                    if (gemSetting.Level.ToInt32() >= level)
                    {
                        res++;
                    }
                }
            }
        }
        return res;
    }

        /// <summary>
    /// 通过类型找物品
    /// </summary>
    /// <param name="itemType"></param>
    /// <returns></returns>
    public List<ItemData> FindEquipItemListByPosIndex(int posIndex)
    {
        List<ItemData> res = new List<ItemData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            if(data.settingId <= 0) continue;
            if(data.equipProtoData!=null
                && data.equipProtoData.setting != null
                && data.equipProtoData.setting.Pos.ToInt32() == posIndex)
            {
                res.Add(data);
            }
           
        }
        return res;
    }
    /// <summary>
    /// 找所有装备 包括身上的
    /// </summary>
    /// <returns></returns>
    public List<ItemData> FindAllEquipmentList()
    {
        List<ItemData> res = FindItemListByType(ItemType.Equip);

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList.Count; i++)
        {
            ItemData theData = RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList[i];
            if (theData != null)
                res.Add(theData);
        }
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            for (int j=0;j< p.curEquipItemList.Count; j++)
            {
                ItemData theData = p.curEquipItemList[j];
                if (theData != null)
                    res.Add(theData);
            }
        }
        return res;
    }

    /// <summary>
    /// 超过某稀有度装备
    /// </summary>
    /// <returns></returns>
    public int FindMoreThanRarityEquipNum(Rarity rarity,int exceptEquipId)
    {
        int num = 0;
        List<ItemData> res = FindAllEquipmentList();
        for(int i = 0; i < res.Count; i++)
        {
            ItemData data = res[i];
            if (data.quality >= (int)rarity
                &&(exceptEquipId==0||data.settingId!=exceptEquipId))
            {
                num++;
            }
        }
        return num;
    }

    /// <summary>
    /// 找某个不同部位套装
    /// </summary>
    /// <returns></returns>
    public List<ItemData> FindDifferentPosTaoZhuangItemList(int taoZhuangType,int rarity=0)
    {
        List<ItemData> res = new List<ItemData>();
        List<int> posList = new List<int>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            if(item.equipProtoData!=null && item.equipProtoData.setting != null
                && item.equipProtoData.setting.TaoZhuang.ToInt32()==taoZhuangType
                && (rarity == 0 || rarity == item.setting.Rarity.ToInt32())
                && !posList.Contains(item.equipProtoData.setting.Pos.ToInt32())
                && item.equipProtoData.setting.Pos.ToInt32()!=0)
            {
                res.Add(item);
                posList.Add(item.equipProtoData.setting.Pos.ToInt32());
            }
        }
        return res;
    }

    /// <summary>
    /// 修为丹加多少修为
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int XiuWeiDanXiuWeiAdd(ItemData data)
    {
        int res = data.setting.Param.ToInt32();
        int addBaiFen = 0;
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.YouBiBaiLian) > 0)
        {
            addBaiFen = ConstantVal.youBiBaiLianAddBaiFen;
        }
        if (LianDanManager.Instance.FindMyFarmNum((int)DanFarmIdType.JingShiQingLian) > 0)
        {
            addBaiFen += ConstantVal.jingShiQingLianAddBaiFen;
        }
        res = (int)(res * (1 + addBaiFen * 0.01f));
        return res;
    }

    /// <summary>
    /// 经验丹加多少经验
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int ExpDanExpAdd(ItemData data)
    {
        if (data.setting == null)
        {
            data.setting = DataTable.FindItemSetting(data.settingId);
        }
        return data.setting.Param.ToInt32();
    }

    /// <summary>
    /// 使用经验丹，增加弟子经验并处理升级
    /// </summary>
    /// <param name="p">角色数据</param>
    /// <param name="item">经验丹物品数据</param>
    /// <returns>是否使用成功</returns>
    public bool UseExpDan(PeopleData p, ItemData item)
    {
        if (p == null || item == null)
            return false;

        // 检查是否达到等级上限
        int levelLimit = StudentManager.Instance.GetStudentLevelLimit(p);
        if (p.studentLevel >= levelLimit)
        {
            PanelManager.Instance.OpenFloatWindow("已达到等级上限，需要突破境界后才能继续升级");
            return false;
        }

        // 检查是否还有升级空间
        if (p.studentLevel <= 0 || p.studentLevel > DataTable._studentUpgradeList.Count)
        {
            return false;
        }

        // 获得的经验值
        int expAdd = ExpDanExpAdd(item);
        
        // 记录升级前的等级
        int beforeLevel = p.studentLevel;
        
        // 增加经验
        p.studentCurExp += expAdd;
        
        // 检查是否升级
        CheckAndUpgradeStudent(p);
        
        // 消耗物品
        RemoveAItem(item.onlyId);
        
        EventCenter.Broadcast(TheEventType.StudentStatusChange, p);
        
        Debug.Log($"[UseExpDan] {p.name} 使用经验丹增加 {expAdd} 经验，等级从 {beforeLevel} 升到 {p.studentLevel}");
        
        return true;
    }

    /// <summary>
    /// 检查并升级弟子
    /// </summary>
    /// <param name="p"></param>
    private void CheckAndUpgradeStudent(PeopleData p)
    {
        while (p.studentLevel > 0 && p.studentLevel <= DataTable._studentUpgradeList.Count)
        {
            int needExp = DataTable._studentUpgradeList[p.studentLevel - 1].NeedExp.ToInt32();
            if (p.studentCurExp >= needExp)
            {
                p.studentCurExp -= needExp;
                p.studentLevel++;
                
                // 升级事件
                EventCenter.Broadcast(TheEventType.OnGetStudentExp, p);
                
                Debug.Log($"[CheckAndUpgradeStudent] {p.name} 升级到 {p.studentLevel} 级");
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 返利券
    /// </summary>
    /// <returns></returns>
    public ItemData FindFanLiQuan(int shopId)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData item = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            if (item.setting!=null && item.setting.ItemType.ToInt32() == (int)ItemType.FanLiDaiJinQuan
                &&item.setting.Param.ToInt32()==shopId)
            {
                return item;
            }
        }
  
        return null;
    }
}

/// <summary>
/// 物品id
/// </summary>
[Serializable]
public enum ItemIdType
{
    None = 0,
    XiangYunBeiDouDan = 34102,//祥云北斗丹
    [EnumAttirbute("灵石")]
    LingShi = 10001,//灵石
    //[EnumAttirbute("召集令")]
    //ZhaoJiLing = 40022,//召集令

    PoSuiRedGem_Fan = 51110,//破碎红宝石 凡级
    PoSuiRedGem_Huang = 52110,//破碎红宝石 黄级
    PoSuiRedGem_Xuan = 53110,//破碎红宝石 玄级
    PoSuiRedGem_Di = 54110,//破碎红宝石 地级
    PoSuiRedGem_Tian = 55110,//破碎红宝石 天级

    PoSuiBlueGem_Fan = 51210,//破碎蓝宝石 凡级
    PoSuiBlueGem_Huang = 52210,//破碎蓝宝石 黄级
    PoSuiBlueGem_Xuan = 53210,//破碎蓝宝石 玄级
    PoSuiBlueGem_Di = 54210,//破碎蓝宝石 地级
    PoSuiBlueGem_Tian = 55210,//破碎蓝宝石 天级


    PoSuiGreenGem_Fan = 51310,//破碎绿 凡级
    PoSuiGreenGem_Huang = 52310,//破碎绿  黄级
    PoSuiGreenGem_Xuan = 53310,//破碎绿  玄级
    PoSuiGreenGem_Di = 54310,//破碎绿  地级
    PoSuiGreenGem_Tian = 55310,//破碎绿  天级


    PoSuiYellowGem_Fan = 51410,//破碎黄 凡级
    PoSuiYellowGem_Huang = 52410,//破碎黄  黄级
    PoSuiYellowGem_Xuan = 53410,//破碎黄  玄级
    PoSuiYellowGem_Di = 54410,//破碎黄  地级
    PoSuiYellowGem_Tian = 55410,//破碎黄  天级


    PoSuiQingGem_Fan = 51510,//破碎青 凡级
    PoSuiQingGem_Huang = 52510,//破碎青  黄级
    PoSuiQingGem_Xuan = 53510,//破碎青  玄级
    PoSuiQingGem_Di = 54510,//破碎青  地级
    PoSuiQingGem_Tian = 55510,//破碎青  天级


    PoSuiWhiteGem_Fan = 51610,//破碎白 凡级
    PoSuiWhiteGem_Huang = 52610,//破碎白  黄级
    PoSuiWhiteGem_Xuan = 53610,//破碎白  玄级
    PoSuiWhiteGem_Di = 54610,//破碎白  地级
    PoSuiWhiteGem_Tian = 55610,//破碎白  天级

    PoSuiPurpleGem_Fan = 51710,//破碎白 凡级
    PoSuiPurpleGem_Huang = 52710,//破碎白  黄级
    PoSuiPurpleGem_Xuan = 53710,//破碎白  玄级
    PoSuiPurpleGem_Di = 54710,//破碎白  地级
    PoSuiPurpleGem_Tian = 55710,//破碎白  天级

    Food = 60001,//食物
    Wood = 60002,//木材
    NongJu = 60004,//农具
    [EnumAttirbute("聚灵玉佩")]
    JuLingYuPei = 70001,//聚灵玉佩
    [EnumAttirbute("五色梦魂花")]
    WuSeMengHunHua=70002,

    PoJingDan_Fan=101001,//破镜丹
    PoJingDan_Huang=102001,
    PoJingDan_Xuan=103001,
    PoJingDan_Di = 104001,
    PoJingDan_Tian = 105001,

    [EnumAttirbute("凡级月灵矿")]
    YueLingKuang_Fan = 111001,//凡级月灵矿

    [EnumAttirbute("九天玄雷决")]
    JiuTianXuanLeiJue = 141001,

    [EnumAttirbute("任意法器")]
    AnyEquip=29999,
    [EnumAttirbute("凡级益气丹")]
    FanJiYiQiDan=101101,//凡级益气丹

    [EnumAttirbute("凡级太玄果")]
    FanJiTaiXuanGuo= 91001,//凡级太玄果
    [EnumAttirbute("源力结晶")]
    YuanLiJieJing = 152001,//源力结晶
    [EnumAttirbute("凡级技能残页")]
    JiNengCanYe_Fan = 121001,//凡级技能残页


    BoGuo=61001,//柏果
    JiuLingGuo=61002,//酒灵果
    YinYangGuo=61003,//阴阳果
    QingMingGuo=61004,//青冥果
    LongLingGuo=61005,//龙灵果


    HuWangShenJin=161001,//虎王神金
    YuSui=162001,//玉髓
    XingSui = 163001,//星髓
    XianZhuShi = 164001,//仙柱石
    BuTianShi = 165001,//补天石
    RongYuBi=230001,//荣誉币
    TianJiTianFu= 305001,//天级天赋晶石

    GuangGaoLing=280001,//广告令

    FanJiShuiNianZhu=121002,//凡级水念珠
    FanJiHuoNianZhu=121003,//凡级火念珠
    FanJiLeiNianZhu=121004,//凡级雷念珠
    FanJiBingNianZhu=121005,//凡级冰念珠
    FanJiYangNianZhu=121006,//凡级阳念珠
    FanJiYinNianZhu=121007,//凡级阴念珠
    FreeAD=355001,//金手指
    LingJing= 365001,//灵晶
    ZhuanHunDan=381001,//转魂丹
    ZhaoMuLing=391001,//招募令
    TianJiJiaoHuaDan= 325001,//天机教化丹 改性格

    ChangeZongMenNameCard = 420001,//宗门改名卡

    ChangePNameCard = 420002,//角色改名卡
    TianJiFu= 445001,//天机符
    TianJing= 365001,//天晶
    DiJiDiZiJieYinLing= 214001,//地级弟子接引令
    TianJiDiZiJieYinLing= 215001,//天级弟子接引令

    ZhengRong=475001,//整容
    GaoJiZhengRong=475002,//高级整容

    PuTongTouXiang= 495004,//无头像
    PuTongTouXiangKuang= 505004,//普通头像框
}



public enum ItemType
{
    None=0,
    LingShi = 1,//钱
    Equip = 2,//装备

    MoneyDan = 3,//钱丹
    ZhaoJiLing = 4,//召集令

    Gem = 5,//宝石
    Task=7,//任务物品

    StudentBook=8,//弟子经验书 清灵草
    LianDanMat=9,//炼丹材料
    Dan=10,//丹
    EquipMat=11,//装备强化材料
    LingQuan=12,//灵泉 用于经脉技能升级
    XingChen=13,//星尘 用于宝石强化
    SkillBook=14,//技能书
    YuanLi=15,//源力
    PoJingDan=16,//破境丹
    Box=17,//宝箱
    GongFaShu=18,//功法书 相当于黑盒技能书 神秘技能书
    XingChenBox=19,//宝石粉末箱 任意宝石粉末
    NianZhuBox=20,//念珠箱子
    JieYinLing=21,//弟子接引令
    RemoveTalent=22,//天赋移除丹
    RongYuBi=23,//荣誉币
    BuildingUpgradeBox=24,//建筑升级宝箱
    QingLingCaoBox=25,//清灵草宝箱
    LingShiFuDai=26,//灵石福袋
    XueMai=27,//提升血脉
    GuangGaoLing=28,//广告令
    XiSui=29,//洗髓 改变弟子天赋等级
    TianJiTianFu=30,//天级天赋晶石
    ProductExpDan=31,//生产弟子经验丹
    ChangeXingGe=32,//改性格的丹
    OP=34,//原胚
    QiHuo=41,//器火
    TiaoXiDan=43,//调息丹
    OPBox=45,//原胚箱子
    QiHuoBox=46,//器火箱子
    ZhengRong=47,//整容卡
    Building=48,//获得就直接获得建筑
    TouXiang=49,//头像
    TouXiangKuang=50,//头像框
    FanLiDaiJinQuan=51,//返利代金券 一个代金券代表一个商品
    FeiQi=999,//废弃
}



public enum Quality
{
    None=0,
    Green = 1,//绿
    Blue = 2,//蓝
    Purple =3,//紫
    Orange =4,//橙色
    Gold=5,//金
    End=6,
}