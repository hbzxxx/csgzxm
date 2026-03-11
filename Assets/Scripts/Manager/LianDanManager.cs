using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using Framework.Data;
using System;
using Google.Protobuf.Collections;
using cfg;

public class LianDanManager : CommonInstance<LianDanManager>
{
    public GameInfo gameInfo;
    ///// <summary>
    ///// 开始炼丹，暂定4周练
    ///// </summary>
    //public void StartLianDan()
    //{
    //    //RoleManager.Instance._CurGameInfo.LianDanData.SettingIdList.Clear();
    //    ////TODO这里根据弟子数量算出炼丹比例
    //    ////黑炭 1品 2品 3品的权重
    //    //List<int> weight = new List<int> { 30, 50, 15, 5 };
    //    //List<int> idList = new List<int> { 10010, 10001, 10002, 10003 };
    //    //for(int i = 0; i < 100; i++)
    //    //{
    //    //    int index = CommonUtil.GetIndexByWeight(weight);
    //    //    RoleManager.Instance._CurGameInfo.LianDanData.SettingIdList.Add(idList[index]);
    //    //}

    //    //RoleManager.Instance._CurGameInfo.LianDanData.RemainDay = 30;
    //}
    public SingleDanFarmData curChoosedDanFarmData;//当前选择坐镇的丹田
    public int curChoosedDanFarmStudentPos;//当前选择坐镇的丹田的弟子位

    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;
        for(int i = 0; i < gameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = gameInfo.allDanFarmData.DanFarmList[i];
            DanFarmSetting setting = DataTable.FindDanFarmSetting(data.SettingId);
            if (data.SettingId == 70001) continue;
            data.NeedForeItemId = setting.NeedForeItem.ToInt32();
        }
    }

    /// <summary>
    /// 中止炼丹
    /// </summary>
    public void StopLianDan()
    {

        EventCenter.Broadcast(TheEventType.ChangeLianDanStatus);

    }

    /// <summary>
    /// 开始炼丹
    /// </summary>
    public void StartLianDan()
    {
        string lastTime = RoleManager.Instance._CurGameInfo.timeData.Year + "|" + RoleManager.Instance._CurGameInfo.timeData.Month + "|" + RoleManager.Instance._CurGameInfo.timeData.Day;
        EventCenter.Broadcast(TheEventType.ChangeLianDanStatus);
    }




    /// <summary>
    /// 钱丹上限
    /// </summary>
    /// <returns></returns>
    public int LianDanNumLimit()
    {
        return 0;
        //int buildingLevel = BuildingManager.Instance.FindBuildingDataBySettingId((int)BuildingIdType.LianDanFang).CurBuildLevel;
        //LianDanBuildingUpgradeSetting setting = DataTable._lianDanBuildingUpgradeList[buildingLevel - 1];
        //return setting.moneyDanLimit.ToInt32();
    }


    /// <summary>
    /// 开始炼制宝石
    /// </summary>
    /// <returns></returns>
    public void StartMakeGem(int itemId,SingleDanFarmData singleDanFarmData)
    {
        if (itemId == 0)
        {
            PanelManager.Instance.OpenFloatWindow("请先选择宝石");
            return;
        }
        //消耗钱 后续改成材料
        ItemSetting itemSetting = DataTable.FindItemSetting(itemId);
        GemSetting gemSetting = DataTable.FindGemSetting(itemSetting.Param.ToInt32());

        List<List<int>> consumeList = CommonUtil.SplitCfg(gemSetting.Consume);
        for (int i = 0; i < consumeList.Count; i++)
        {
            List<int> singleConsume = consumeList[i];
            int id = singleConsume[0];
            int count = singleConsume[1];

            if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)count))
            {
                ItemSetting consumeSetting = DataTable.FindItemSetting(id);
                PanelManager.Instance.OpenFloatWindow(consumeSetting.Name + "不够");
                return;
            }
        }
        for (int i = 0; i < consumeList.Count; i++)
        {
            List<int> singleConsume = consumeList[i];
            int id = singleConsume[0];
            int count = singleConsume[1];
            ItemManager.Instance.LoseItem(id, (ulong)count);
        }
        int proNum = 0;
        List<PeopleData> studentList = FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        List<PeopleData> validStudentList = new List<PeopleData>();
        for (int i=0;i< studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            if (p.talent == (int)StudentTalent.BaoShi)
            {
                proNum += RoleManager.Instance.FindProperty((int)PropertyIdType.LianShi, p).num;
                validStudentList.Add(p);
            }
        }
        Quality quality= GetGemQualityByPro(proNum);

        //ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, moneyNum);
        ItemData gem = ItemManager.Instance.AddANewGem(gemSetting.ItemId.ToInt32(), null, false, quality);
        //经验获取
        Rarity rarity = (Rarity)DataTable.FindItemSetting(gem.settingId).Rarity.ToInt32();
        int expGet = StudentGemExpGet(rarity);
        List<int> beforeExpList = new List<int>();
     
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            if (p.talent == singleDanFarmData.TalentType)
            {
                beforeExpList.Add(p.studentCurExp);
                StudentManager.Instance.OnGetStudentExp(p, expGet);

            }
        }
        SinglePropertyData afterPro = gem.gemData.propertyList[0];       
 
        //List<ItemData> dataList = new List<ItemData>();
        //dataList.Add(gem);

        //PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, dataList);

        PanelManager.Instance.OpenPanel<GemCompositeResPanel>(PanelManager.Instance.trans_layer2, new List<SinglePropertyData>(), new List<SinglePropertyData> { afterPro }, beforeExpList, validStudentList,gem.gemData);
        //埋点
        //TalkingDataGA.OnEvent("制造宝石", new Dictionary<string, object>() { { itemSetting.name, 1 } });
        TaskManager.Instance.GetDailyAchievement(TaskType.MakeGem, "1");
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.MakeGem);
        TaskManager.Instance.GetAchievement(AchievementType.MakeGem, "1");

        EventCenter.Broadcast(TheEventType.StartMakeGem);
    }

    /// <summary>
    /// 开始炼制普通丹
    /// </summary>
    /// <returns></returns>
    public void StartMakeCommonDan(int itemId)
    {
        if (itemId == 0)
        {
            PanelManager.Instance.OpenFloatWindow("请先选择图纸");
            return;
        }
        //消耗钱 后续改成材料
        ItemSetting itemSetting = DataTable.FindItemSetting(itemId);

        List<List<int>> consumeList = CommonUtil.SplitCfg(itemSetting.Param);
        for (int i = 0; i < consumeList.Count; i++)
        {
            List<int> singleConsume = consumeList[i];
            int id = singleConsume[0];
            int count = singleConsume[1];
            if (!ItemManager.Instance.CheckIfItemEnough(id, (ulong)count))
            {
                ItemSetting consumeSetting = DataTable.FindItemSetting(id);
                PanelManager.Instance.OpenFloatWindow(consumeSetting.Name + "不够");
                return;
            }
        }
        for (int i = 0; i < consumeList.Count; i++)
        {
            List<int> singleConsume = consumeList[i];
            int id = singleConsume[0];
            int count = singleConsume[1];
            ItemManager.Instance.LoseItem(id, (ulong)count);
        }

        ulong danOnlyId = ItemManager.Instance.GetItem(itemId, (ulong)1);
        //经验获取
        Rarity rarity = (Rarity)DataTable.FindItemSetting(itemId).Rarity.ToInt32();
        int expGet = StudentGemExpGet(rarity);
        List<int> beforeExpList = new List<int>();
        List<PeopleData> studentList = new List<PeopleData>();
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList[i];
        //    beforeExpList.Add(p.StudentCurExp);
        //    StudentManager.Instance.OnGetStudentExp(p, expGet);
        //    studentList.Add(p);
        //}
        List<ItemData> dataList = new List<ItemData>();
        dataList.Add(ItemManager.Instance.FindItemByOnlyId(danOnlyId));

        //PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, dataList);
        PanelManager.Instance.OpenPanel<GetAwardWithStudentUpgradePanel>(PanelManager.Instance.trans_layer2,
                      dataList, beforeExpList, studentList);
        //埋点
        //TalkingDataGA.OnEvent("炼制丹药", new Dictionary<string, object>() { { itemSetting.name, 1 } });
    }
    /// <summary>
    /// 获取炼石等级
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int GetTotalLianShiProNum()
    {
        int lianShiProNum = 0;
        //判定宝石品级
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList[i];
        //    for (int j = 0; j < p.PropertyList.Count; j++)
        //    {
        //        SinglePropertyData pro = p.PropertyList[j];
        //        //炼石属性总和
        //        if (pro.Id == (int)PropertyIdType.LianShi)
        //        {
        //            lianShiProNum += pro.Num;
        //        }
        //    }
        //}
        return lianShiProNum;
    }

    /// <summary>
    /// 通过属性得到炼制钱丹的品级
    /// </summary>
    /// <returns></returns>
    public Quality GetMoneyDanQualityByPro(int proNum)
    {
        List<int> weightList = GetMoneyDanQualityWeight(proNum);
        List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };


        int index = CommonUtil.GetIndexByWeight(weightList);
        Quality res = qualityList[index];
        return res;

    }
    /// <summary>
    /// 钱丹质量权重
    /// </summary>
    /// <returns></returns>
    public List<int> GetMoneyDanQualityWeight(int proNum)
    {
        TestStudentSingleProInfluenceSetting setting = DataTable.FindTestStudentSingleProInfluenceByPro(proNum);
        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);


        return weightList;
    }

    /// <summary>
    /// 获取炼制宝石的质量的权重
    /// </summary>
    /// <param name="proNum"></param>
    /// <returns></returns>
    public List<int> GetGemQualityWeightList(int proNum)
    {
        TestStudentTotalProInfluenceSetting setting = DataTable.FindTestStudentTotalProInfluenceByPro(proNum);

        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);


        return weightList;
    }
    /// <summary>
    /// 炼丹经验加成
    /// </summary>
    /// <returns></returns>
    public int StudentLianDanExpGet(Rarity rarity)
    {
        switch ((Rarity)rarity)
        {
            case Rarity.Fan:
                return 3;
            case Rarity.Huang:
                return 6;
            case Rarity.Xuan:
                return 9;
            case Rarity.Di:
                return 12;
            case Rarity.Tian:
                return 15;
        }
        return 0;
    }

    /// <summary>
    /// 炼宝石经验加成
    /// </summary>
    /// <returns></returns>
    public int StudentGemExpGet(Rarity rarity)
    {
        switch ((Rarity)rarity)
        {
            case Rarity.Fan:
                return 90;
            case Rarity.Huang:
                return 150;
            case Rarity.Xuan:
                return 210;
            case Rarity.Di:
                return 270;
            case Rarity.Tian:
                return 330;
        }
        return 0;
    }

    ///// <summary>
    ///// 通过属性得到炼制宝石的品级
    ///// </summary>
    ///// <returns></returns>
    //public Quality GetGemQualityByPro(int proNum)
    //{
    //    List<int> weightList = GetGemQualityWeightList(proNum);
    //    List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };

    //    int index = CommonUtil.GetIndexByWeight(weightList);
    //    Quality res = qualityList[index];
    //    return res;

    //}

    //增加一个炼丹弟子
    public void AddALianDanStudent(PeopleData p)
    {

        p.talent = (int)StudentTalent.LianJing;
    }

    ///// <summary>
    ///// 获取钱丹满仓时间
    ///// </summary>
    //public int GetMoneyDanFullStorageWeek()
    //{
    //    int studentCount = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count;
    //    int moneyDanLimit = RoleManager.Instance._CurGameInfo.LianDanData.MoneyDanNumLimit;
    //    int curNum = RoleManager.Instance._CurGameInfo.LianDanData.AccomplishedMoneyDanSettingIdList.Count;
    //    //每3天产studentCount个
    //    int remainNum = moneyDanLimit - curNum;
    //    //多少天能填满
    //    float DayFull= 3 * remainNum / (float)studentCount;
    //    //多少周能填满
    //    return Mathf.RoundToInt(DayFull /7 ) + 1;
    //}

    ///// <summary>
    ///// 获取钱丹产丹速度
    ///// </summary>
    ///// <returns></returns>
    //public float GetMoneyDanProduceSpeed()
    //{
    //    float res = 0;
    //    int studentCount = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count;
    //    //每3天产studentCount个
    //    res= 7 *(studentCount / (float)3);
    //    return res;
    //}

    ///// <summary>
    ///// 亲自炼丹
    ///// </summary>
    //public void SelfLianDan()
    //{
    //    //判断够不够物品
     
    //        Action finishedAction = OnSelfLianDanProcessFinished;
    //        PanelManager.Instance.OpenPanel<ProcessingPanel>(PanelManager.Instance.trans_layer2, finishedAction);
    //        //PanelManager.Instance.OpenSingle<ProcessingPanel>(PanelManager.Instance.trans_layer2, () =>
    //        //{

    //        //});
        
    //    //Action finishedAction = OnSelfLianDanProcessFinished;
    //    //PanelManager.Instance.OpenPanel<ProcessingPanel>(PanelManager.Instance.trans_layer2, finishedAction);
    //}

    //void OnSelfLianDanProcessFinished()
    //{
    //    RoleManager.Instance.DeProperty(PropertyIdType.Tili, -30);

    //    //获得 10倍当前产出的量 目的让玩家后期别点
    //    int id = RoleManager.Instance._CurGameInfo.LianDanData.CurValidMoneyDanSettingIdList[0];
    //    int num = RandomManager.Next(8, 12);
    //    int itemNum = RandomManager.Next(3, 5);
    //    int curNum = num / itemNum;


    //    for (int i = 0; i < itemNum; i++)
    //    {
    //        int realNum = 0;
    //        if (i >= itemNum - 1)
    //        {
    //            realNum = num;
    //        }
    //        else
    //        {
    //            realNum = curNum;
    //            num -= realNum;
    //        }
    //        ItemData itemData = new ItemData();
    //        itemData.Quality = (int)Quality.Gold;
    //        itemData.SettingId = id;
    //        itemData.Count = (ulong)realNum;

    //        EventCenter.Broadcast(TheEventType.SelfLianDan, itemData);



    //    }
    //}

    public void DanFarmUpgrade(SingleDanFarmData singleDanFarmData)
    {
        if (RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count >= ConstantVal.farmBuildMaxQueueNum)
        {
            PanelManager.Instance.OpenFloatWindow("已达最大建筑队列，请等待建造完成");
            return;
        }
        DanFarmSetting setting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        List<int> upgradeCostList = CommonUtil.SplitCfgOneDepth(setting.UpgradeCost);
        if (singleDanFarmData.CurLevel >= upgradeCostList.Count)
        {
            PanelManager.Instance.OpenFloatWindow("当前已达最大等级");
            return;
        }
        int cost = upgradeCostList[singleDanFarmData.CurLevel - 1];
        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)cost))
        {
                   ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
            return;
        }
        if (!string.IsNullOrWhiteSpace(setting.UpgradeNeedOtherMat))
        {
            List<List<int>> otherConsume = CommonUtil.SplitCfg(setting.UpgradeNeedOtherMat);
            List<int> theConsume = otherConsume[singleDanFarmData.CurLevel - 1];
            if (theConsume.Count > 1)
            {
                if (!ItemManager.Instance.CheckIfItemEnough(theConsume[0], (ulong)theConsume[1]))
                {
                    ItemSetting itemSetting = DataTable.FindItemSetting(theConsume[0]);
                    PanelManager.Instance.OpenFloatWindow(itemSetting.Name + "不够");
                    return;
                }
            }

        }

        int needZongMenLevel = ZongMenManager.Instance.FarmNextLevelNeedZongMenLevel(singleDanFarmData);

        if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel < needZongMenLevel)
        {
            PanelManager.Instance.OpenFloatWindow("需要" + needZongMenLevel + "级"+LanguageUtil.GetLanguageText((int)LanguageIdType.宗门)+"才能升级");
            return;
        }


        if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.NeedMat
            && singleDanFarmData.Status == (int)DanFarmStatusType.Working)
        {
            PanelManager.Instance.OpenFloatWindow("当前正在生产中，请先停止生产");
            return;
        }

        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)cost);
        if (!string.IsNullOrWhiteSpace(setting.UpgradeNeedOtherMat))
        {
            List<List<int>> otherConsume = CommonUtil.SplitCfg(setting.UpgradeNeedOtherMat);
            List<int> theConsume = otherConsume[singleDanFarmData.CurLevel - 1];
    
            if(theConsume.Count==2)
            ItemManager.Instance.LoseItem(theConsume[0], (ulong)theConsume[1]);


        }

        singleDanFarmData.Status = (int)DanFarmStatusType.Upgrading;
        List<int> upgradeTimeList = CommonUtil.SplitCfgOneDepth(setting.UpgradeDay);
        singleDanFarmData.RebuildTotalTime = upgradeTimeList[singleDanFarmData.CurLevel];
        singleDanFarmData.RemainTime = singleDanFarmData.RebuildTotalTime;
        RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Add(singleDanFarmData.SettingId);
        EventCenter.Broadcast(TheEventType.UpgradeDanFarm, singleDanFarmData);
        AuditionManager.Instance.PlayVoice(AudioClipType.Building);

    }

    /// <summary>
    /// 测试用：同步升级丹炉到满级
    /// </summary>
    public void DanFarmUpgradeToMaxForTest(SingleDanFarmData danFarm)
    {
        if (danFarm == null) return;
        
        DanFarmSetting setting = DataTable.FindDanFarmSetting(danFarm.SettingId);
        if (setting == null) return;
        
        List<int> upgradeCostList = CommonUtil.SplitCfgOneDepth(setting.UpgradeCost);
        int maxLevel = 5;
        
        for (int level = danFarm.CurLevel; level < maxLevel; level++)
        {
            danFarm.CurLevel = level + 1;
            
            if (setting.WorkType.ToInt32() == (int)DanFarmWorkType.Common)
            {
                List<int> priceList = CommonUtil.SplitCfgOneDepth(setting.DanPrice);
                if (level < priceList.Count)
                {
                    danFarm.SingleDanPrice = priceList[level];
                }
            }
            
            UnlockStudentPos(danFarm, setting);
            
            if (setting.Id.ToInt32() == (int)DanFarmIdType.LianDanLu)
            {
                UnlockDanFarmProduct(danFarm, setting);
            }
            else if (setting.Id.ToInt32() == (int)DanFarmIdType.EquipMake)
            {
                OnEquipMakeBuildingUpgrade(danFarm);
            }
            else if (setting.Id.ToInt32() == (int)DanFarmIdType.BaGuaLu)
            {
                OnBaGuaLuUpgrade(danFarm);
            }
        }
        
        Debug.Log($"[TestMod] 丹炉 {setting.Id} 已升级到满级 {maxLevel}");
    }

    /// <summary>
    /// 开始建新丹田
    /// </summary>
    public void OnBuildNewDanFarm(int settingId,Vector2 localPos)
    {
        if (RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Count >= ConstantVal.farmBuildMaxQueueNum)
        {
            PanelManager.Instance.OpenFloatWindow("已达最大建筑队列，请等待建造完成");
            return;
        }
        DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(settingId);
        ulong buildCost = danFarmSetting.BuildCost.ToUInt64();

        if (ZongMenManager.Instance.GetFarmNumLimit(settingId) <= FindMyFarmNum(settingId))
        {
            PanelManager.Instance.OpenFloatWindow("该建筑数量已达上限，请提升"+LanguageUtil.GetLanguageText((int)LanguageIdType.宗门)+"等级");
            return;
        }

        if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, buildCost))
        {
                     ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
            return;
        }
        SingleDanFarmData data = AddADanFarm();

        if (!data.IsEmpty)
        {
            PanelManager.Instance.OpenFloatWindow("该位置已被占据");
            return;
        }

        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, buildCost);
        data.LocalPos = localPos;
        data.Unlocked = true;
        data.CurLevel = 0;
        data.Status = (int)DanFarmStatusType.Building;
        data.IsEmpty = false;
        data.SettingId = settingId;
        data.RemainTime = danFarmSetting.BuildDay.ToInt32();
        data.RebuildTotalTime = danFarmSetting.BuildDay.ToInt32();
        data.DanFarmType = danFarmSetting.Type.ToInt32();
        data.DanFarmWorkType = danFarmSetting.WorkType.ToInt32();
        data.TalentType = danFarmSetting.TalentType.ToInt32();
        if (danFarmSetting.WorkType.ToInt32() == (int)DanFarmWorkType.Common)
        {
            data.ProductSettingId = danFarmSetting.Param.ToInt32();
            data.ProcessSpeed = danFarmSetting.ProcessSpeed.ToInt32();
            data.NeedForeItemId = danFarmSetting.NeedForeItem.ToInt32();
        }
     
        RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.Add(settingId);
        EventCenter.Broadcast(TheEventType.StartFarmBuild, data, localPos);
        AuditionManager.Instance.PlayVoice(AudioClipType.Building);
    }

    /// <summary>
    /// 炼丹房解锁丹方id
    /// </summary>
    public void UnlockDanFarmProduct(SingleDanFarmData singleDanFarmData, cfg.DanFarmSetting danFarmSetting)
    {
        int level = singleDanFarmData.CurLevel;
        string product= DataTable._lianDanBuildingUpgradeList[level - 1].UnLockProductId;
        List<int> productList = CommonUtil.SplitCfgOneDepth(product);
        for(int i = 0; i < productList.Count; i++)
        {
            int theId = productList[i];
            if (!singleDanFarmData.UnlockedProductIdList.Contains(theId))
            {
                singleDanFarmData.UnlockedProductIdList.Add(theId);
            }
        }
    }
    /// <summary>
    /// 解锁弟子位置
    /// </summary>
    public void UnlockStudentPos(SingleDanFarmData singleDanFarmData, DanFarmSetting danFarmSetting)
    {
        List<int> unlockLevelList = StudentPosUnlockLevelList(danFarmSetting);
        int level = singleDanFarmData.CurLevel;
        
        for(int i = 0; i < unlockLevelList.Count; i++)
        {
            if (level >= unlockLevelList[i])
            {
                singleDanFarmData.PosUnlockStatusList[i] = true;
            }
        }
    }

    /// <summary>
    /// 拆除还原弟子位
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    public void ReviveStudentPos(SingleDanFarmData singleDanFarmData)
    {
        for(int i=0;i < singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            singleDanFarmData.PosUnlockStatusList[i] = false;
        }
    }

    /// <summary>
    /// 弟子解锁位
    /// </summary>
    /// <param name="danFarmSetting"></param>
    /// <returns></returns>
    public List<int> StudentPosUnlockLevelList(cfg.DanFarmSetting danFarmSetting)
    {
        List<int> unlockLevelList = new List<int>();

        if (danFarmSetting.Id.ToInt32() == (int)DanFarmIdType.LianDanLu)
        {
            unlockLevelList = new List<int> { 1, 4, 7, 10 };
        }
        else if (danFarmSetting.Id.ToInt32() == (int)DanFarmIdType.EquipMake)
        {
            unlockLevelList = new List<int> { 1, 2, 3, 4 };

        }
        else if (danFarmSetting.Id.ToInt32() == (int)DanFarmIdType.BaGuaLu)
        {
            unlockLevelList = new List<int> { 1, 2, 3, 4 };

        }
        else
        {
            unlockLevelList = new List<int> { 1, 12, 24, 36 };

        }
        return unlockLevelList;
    }
    /// <summary>
    /// 一天开始时danfarm进度
    /// </summary>
    public void StartProcessDanFarm()
    {
    

        //工作
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.IsEmpty)
                continue;
            if (data.Status == (int)DanFarmStatusType.Working)
            {
                //得到物品
                if (data.ProcessDanTimer <= 0)
                {

                    if (data.OpenQuanLi)
                        data.ProcessDanTimer = data.ProcessSpeed / 3;
                    else
                        data.ProcessDanTimer = data.ProcessSpeed;
                }

                //发消息给ui
                EventCenter.Broadcast(TheEventType.DanFarmProduceProcess, data);

            }else if (data.Status == (int)DanFarmStatusType.Building || data.Status == (int)DanFarmStatusType.Upgrading)
            {
                if (data.RemainTime <= 0)
                {
                }
                else
                {
                    //预告
                    float process = (data.RebuildTotalTime - (data.RemainTime - 1)) / (float)data.RebuildTotalTime;
                    EventCenter.Broadcast(TheEventType.DanFarmRebuildProcess, data);

                }
            }
        }

    }
    /// <summary>
    /// 时间每过一天丹田操作
    /// </summary>
    public void OnProcessingDanFarm()
    {
  

        //工作
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.IsEmpty)
                continue;


            if (data.Status == (int)DanFarmStatusType.Working)
            {

                data.ProcessDanTimer--;
                if (data.OpenQuanLi)
                {
                    data.QuanliRemainTime--;


                    if (data.QuanliRemainTime <= 0)
                    {
                        data.QuanliRemainTime = 0;
                        //结束
                        OnEndQuanLiDanFarm(data);
                    }
                }
                //得到物品
                if (data.ProcessDanTimer <= 0)
                {

                    if (data.OpenQuanLi)
                        data.ProcessDanTimer = data.ProcessSpeed / 3;
                    else
                        data.ProcessDanTimer = data.ProcessSpeed;

                    int danPrice = CalcDanPrice(data);

                    ////灵石炉
                    //if (data.DanFarmType == (int)DanFarmType.MoneyDan)
                    //{
                    //    //得钱
                    //}
                    //else
                    //{
                    //    ItemManager.Instance.GetItem(data.ProductSettingId, (ulong)danPrice);

                    //}
                    if (data.DanFarmWorkType != (int)DanFarmWorkType.Special)
                    {
                        if (data.DanFarmWorkType == (int)DanFarmWorkType.NeedMat)
                        {
                            data.ProductRemainNum--;
                            //炼丹炉产出什么丹
                            if (data.SettingId == (int)DanFarmIdType.LianDanLu)
                            {
                                //有玩家出现bug 待查
                                if (data.ProductSettingId == 0)
                                {
                                    data.Status = (int)DanFarmStatusType.Idling;
                                    EventCenter.Broadcast(TheEventType.StopMatDanFarmProduce, data);
                                    return;
                                }
                                ItemSetting itemSetting = DataTable.FindItemSetting(data.ProductSettingId);
                                //炼的有品质之分的修为丹
                                if (!string.IsNullOrWhiteSpace(itemSetting.Param4))
                                {
                                    int studentPro = GetFarmZuoZhenStudentTotalLianDanPro(data);
                                    Quality quality = EquipmentManager.Instance.GetEquipQualityByPro(studentPro);
                                    List<int> productIdList = CommonUtil.SplitCfgOneDepth(itemSetting.Param4);
                                    int settingId = productIdList[(int)quality - 1];
                                    bool haveSameId = false;
                                    for (int j = 0; j < data.ProductItemList.Count; j++)
                                    {
                                        ItemData existItem = data.ProductItemList[j];
                                        if (existItem.settingId == settingId)
                                        {
                                            haveSameId = true;
                                            existItem.count += 1;
                                            break;
                                        }
                                    }
                                    if (!haveSameId)
                                    {
                                        ItemData item = new ItemData();
                                        item.settingId = settingId;
                                        item.count = 1;
                                        data.ProductItemList.Add(item);
                                    }
                                }
                                //炼的突破丹
                                else
                                {
                                    int settingId = itemSetting.Id.ToInt32();
                                    bool haveSameId = false;
                                    for (int j = 0; j < data.ProductItemList.Count; j++)
                                    {
                                        ItemData existItem = data.ProductItemList[j];
                                        if (existItem.settingId == settingId)
                                        {
                                            haveSameId = true;
                                            existItem.count += 1;
                                            break;
                                        }
                                    }
                                    if (!haveSameId)
                                    {
                                        ItemData item = new ItemData();
                                        item.settingId = settingId;
                                        item.count = 1;
                                        data.ProductItemList.Add(item);
                                    }

                                }
                                //埋点
                                //TalkingDataGA.OnEvent("炼制丹药", new Dictionary<string, object>() { { itemSetting.name, 1 } });
                                TaskManager.Instance.GetAchievement(AchievementType.LianDan2, data.ProductSettingId.ToString());
                                TaskManager.Instance.GetDailyAchievement(TaskType.LianDan2, "1");
                                TaskManager.Instance.TryAccomplishGuideBook(TaskType.LianDan2);
                            }
                            if (data.ProductRemainNum <= 0)
                            {
                                data.Status = (int)DanFarmStatusType.Idling;
                                EventCenter.Broadcast(TheEventType.StopMatDanFarmProduce, data);
                            
                            }
                        }
                        else
                        {
                            bool canGetItem = true;
                            //普通丹房
                            if (data.NeedForeItemId != 0)
                            {
                                if(!ItemManager.Instance.CheckIfItemEnough(data.NeedForeItemId, (ulong)danPrice))
                                {
                                    ChangeFarmWorkStatus(data, DanFarmStatusType.Idling);
                                    canGetItem = false;
                                }
                                else
                                {
                                    canGetItem= ItemManager.Instance.LoseItem(data.NeedForeItemId, (ulong)danPrice);
                                }
                              
                            }
                            if(canGetItem)
                            {
                                ItemManager.Instance.GetItem(data.ProductSettingId, (ulong)danPrice);
                                EventCenter.Broadcast(TheEventType.DanFarmProduceAItem, data);
                            }
                  
                        }
                    }
                   

                }

                //发消息给ui
                // EventCenter.Broadcast(TheEventType.DanFarmProduceProcess, data);


            }
            //建造中
            else if (data.Status == (int)DanFarmStatusType.Building || data.Status == (int)DanFarmStatusType.Upgrading)
            {
                data.RemainTime -= 1;
                if (data.RemainTime <= 0)
                {
                    RoleManager.Instance._CurGameInfo.allDanFarmData.ReBuildingIndexList.RemoveAt(0);
                    data.CurLevel++;
                    DanFarmSetting setting = DataTable.FindDanFarmSetting(data.SettingId);

                    if (setting.WorkType.ToInt32() == (int)DanFarmWorkType.Common)
                    {
                        List<int> priceList = CommonUtil.SplitCfgOneDepth(setting.DanPrice);
                        data.SingleDanPrice = priceList[data.CurLevel - 1];
                        data.Status = (int)DanFarmStatusType.Working;
                        UnlockStudentPos(data, setting);

                    }
                    else
                    {
                        UnlockStudentPos(data, setting);
                        //炼丹房解锁丹方id
                        if (setting.Id.ToInt32() == (int)DanFarmIdType.LianDanLu)
                        {
                            UnlockDanFarmProduct(data, setting);
                        }
                        else if (setting.Id.ToInt32() == (int)DanFarmIdType.EquipMake)
                        {
                            OnEquipMakeBuildingUpgrade(data);
                        }
                        else if (setting.Id.ToInt32() == (int)DanFarmIdType.BaGuaLu)
                        {
                            OnBaGuaLuUpgrade(data);
                        }
                        data.Status = (int)DanFarmStatusType.Idling;

                    }

                    data.ProcessDanTimer = data.ProcessSpeed;
                    EventCenter.Broadcast(TheEventType.StopDanFarmRebuildProcess, data);


                    //PanelManager.Instance.OpenOnlyOkHint("建造成功！",()=> 
                    //{

                    //});
                    AuditionManager.Instance.PlayVoice(AudioClipType.Building);
                    RecordManager.Instance.AddTongZhi(setting.Name + "Lv" + data.CurLevel + "建造成功！");

                    for (int j = 0; j < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; j++)
                    {
                        ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[j];
                        SingleNPCData singleNPCData = TaskManager.Instance.FindNPCByOnlyId(onlyId);
                        if (singleNPCData != null)
                        {
                            TaskManager.Instance.TryAccomplishTask(singleNPCData);

                        }
                    }
                    TaskManager.Instance.TryAccomplishGuideBook(TaskType.DanFarmNum);
                    TaskManager.Instance.TryAccomplishGuideBook(TaskType.HaveABLevelCFarm);

                    //埋点
                    //TalkingDataGA.OnEvent("建筑等级提升", new Dictionary<string, object>() { { setting.name, data.CurLevel } });
                }
                else
                {
                    float process = (data.RebuildTotalTime - data.RemainTime) / (float)data.RebuildTotalTime;
                    EventCenter.Broadcast(TheEventType.DanFarmRebuildProcess, data);

                }
            }
            //需要前置材料的idle 判定是否重启
            else if (data.Status == (int)DanFarmStatusType.Idling
                &&data.NeedForeItemId>0
                &&!data.HandleStop)
            {
                int danPrice = CalcDanPrice(data);
                if (ItemManager.Instance.CheckIfItemEnough(data.NeedForeItemId, (ulong)danPrice))
                {
                    ChangeFarmWorkStatus(data, DanFarmStatusType.Working);
                }
            }
        }


    }

    /// <summary>
    /// 加速炼丹
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    public void OnJiaSuLianDan(SingleDanFarmData singleDanFarmData)
    {
        if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.NeedMat
           && singleDanFarmData.Status == (int)DanFarmStatusType.Working)
        {
            int needTiLi = JiaSuLianDanNeedTili(singleDanFarmData);
            if (RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, needTiLi))
            {
                RoleManager.Instance.DeProperty(PropertyIdType.Tili, -needTiLi);
                //立即完成
                if (singleDanFarmData.SettingId == (int)DanFarmIdType.LianDanLu)
                {
                    for(int i = 0; i < singleDanFarmData.ProductRemainNum; i++)
                    {
                        //有玩家出现bug 待查
                        if (singleDanFarmData.ProductSettingId == 0)
                        {
                            singleDanFarmData.Status = (int)DanFarmStatusType.Idling;
                            EventCenter.Broadcast(TheEventType.StopMatDanFarmProduce, singleDanFarmData);
                            return;
                        }
                        ItemSetting itemSetting = DataTable.FindItemSetting(singleDanFarmData.ProductSettingId);
                        //炼的有品质之分的修为丹
                        if (!string.IsNullOrWhiteSpace(itemSetting.Param4))
                        {
                            int studentPro = GetFarmZuoZhenStudentTotalLianDanPro(singleDanFarmData);
                            Quality quality = EquipmentManager.Instance.GetEquipQualityByPro(studentPro);
                            List<int> productIdList = CommonUtil.SplitCfgOneDepth(itemSetting.Param4);
                            int settingId = productIdList[(int)quality - 1];
                            bool haveSameId = false;
                            for (int j = 0; j < singleDanFarmData.ProductItemList.Count; j++)
                            {
                                ItemData existItem = singleDanFarmData.ProductItemList[j];
                                if (existItem.settingId == settingId)
                                {
                                    haveSameId = true;
                                    existItem.count += 1;
                                    break;
                                }
                            }
                            if (!haveSameId)
                            {
                                ItemData item = new ItemData();
                                item.settingId = settingId;
                                item.count = 1;
                                singleDanFarmData.ProductItemList.Add(item);
                            }
                        }
                        //炼的突破丹
                        else
                        {
                            int settingId = itemSetting.Id.ToInt32();
                            bool haveSameId = false;
                            for (int j = 0; j < singleDanFarmData.ProductItemList.Count; j++)
                            {
                                ItemData existItem = singleDanFarmData.ProductItemList[j];
                                if (existItem.settingId == settingId)
                                {
                                    haveSameId = true;
                                    existItem.count += 1;
                                    break;
                                }
                            }
                            if (!haveSameId)
                            {
                                ItemData item = new ItemData();
                                item.settingId = settingId;
                                item.count = 1;
                                singleDanFarmData.ProductItemList.Add(item);
                            }

                        }
                        //埋点
                        //TalkingDataGA.OnEvent("炼制丹药", new Dictionary<string, object>() { { itemSetting.name, 1 } });
                        TaskManager.Instance.GetAchievement(AchievementType.LianDan2, singleDanFarmData.ProductSettingId.ToString());
                        TaskManager.Instance.TryAccomplishGuideBook(TaskType.LianDan2);
                        TaskManager.Instance.GetDailyAchievement(TaskType.LianDan2, "1");

                    }
                    singleDanFarmData.ProductRemainNum = 0;
                }
               
                    singleDanFarmData.Status = (int)DanFarmStatusType.Idling;
                    EventCenter.Broadcast(TheEventType.StopMatDanFarmProduce, singleDanFarmData);
                
            }
        }
    }
    /// <summary>
    /// 停止炼丹 返还材料
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    public void OnStopLianDan(SingleDanFarmData singleDanFarmData)
    {
        if(singleDanFarmData.DanFarmWorkType==(int)DanFarmWorkType.NeedMat
            && singleDanFarmData.Status == (int)DanFarmStatusType.Working)
        {
            int itemId = singleDanFarmData.ProductSettingId;
            int itemNum = singleDanFarmData.ProductRemainNum;
            ItemSetting itemSetting = DataTable.FindItemSetting(itemId);
            List<List<int>> mat = CommonUtil.SplitCfg(itemSetting.Param2);
            for(int i = 0; i < mat.Count; i++)
            {
                List<int> singleMat = mat[i];
                int matId = singleMat[0];
                int matNum = singleMat[1];
                ItemManager.Instance.GetItem(matId, (ulong)(matNum * itemNum));
            }

            singleDanFarmData.ProductSettingId = 0;
            if (singleDanFarmData.ProductItemList.Count > 0)
            {
                singleDanFarmData.ProductTotalNum = (int)singleDanFarmData.ProductItemList[0].count;

            }
            else
            {
                singleDanFarmData.ProductTotalNum = 0;
            }

            singleDanFarmData.ProductRemainNum = 0;
            singleDanFarmData.Status = (int)DanFarmStatusType.Idling;
            EventCenter.Broadcast(TheEventType.StopMatDanFarmProduce, singleDanFarmData);
        }
    }

    /// <summary>
    /// 八卦炉升级
    /// </summary>
    public void OnBaGuaLuUpgrade(SingleDanFarmData singleDanFarmData)
    {
        int level = singleDanFarmData.CurLevel;
        if (RoleManager.Instance._CurGameInfo.LianDanData == null)
        {
            Debug.LogError($"炼丹数据为空");
            return;
        }
        if (RoleManager.Instance._CurGameInfo.LianDanData.MaxGemRarity<=level)
        RoleManager.Instance._CurGameInfo.LianDanData.MaxGemRarity = level;
    
    }
    /// <summary>
    /// 炼器房升级
    /// </summary>
    public void OnEquipMakeBuildingUpgrade(SingleDanFarmData singleDanFarmData)
    {
        int level = singleDanFarmData.CurLevel;

        string product = DataTable.FindDanFarmSetting((int)DanFarmIdType.EquipMake).Param;
        List<List<int>> allProductList = CommonUtil.SplitCfg(product);
        for (int i = 0; i < allProductList.Count; i++)
        {
            List<int> theProdut = allProductList[i];
            if (level-1 >= i)
            {
                for(int j = 0; j < theProdut.Count; j++)
                {
                    int theId = theProdut[j];
                    if (theId == 0)
                        continue;
                    EquipmentManager.Instance.AddEquipPicture(theId);
                }
            }
          
        }
    }
    /// <summary>
    /// 坐镇弟子炼丹总属性
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    int GetFarmZuoZhenStudentTotalLianDanPro(SingleDanFarmData data)
    {
        if (data == null)
            return 0;
        int proNum = 0;
        List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(data);
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            for (int j = 0; j < p.propertyIdList.Count; j++)
            {
                int theId = p.propertyIdList[j];
                if (theId == (int)PropertyIdType.JieDan)
                {
                    proNum += p.propertyList[j].num;
                }
            }

        }
        return proNum;
    }
    /// <summary>
    /// 结束全力炼丹
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    public void OnEndQuanLiDanFarm(SingleDanFarmData singleDanFarmData)
    {
        singleDanFarmData.OpenQuanLi = false;
        //弟子进入休息
        for (int i = 0; i < singleDanFarmData.ZuoZhenStudentIdList.Count; i++)
        {
            ulong theId = singleDanFarmData.ZuoZhenStudentIdList[i];
            if (theId == 0)
                continue;
            PeopleData p = StudentManager.Instance.FindStudent(theId);
            p.studentStatusType = (int)StudentStatusType.DanFarmRelax;
        }

        //singleDanFarmData.ZuoZhenStudentIdList.Clear();
        EventCenter.Broadcast(TheEventType.EndQuanLiDanFarm, singleDanFarmData);
    }

    /// <summary>
    /// 生产需要材料的物品
    /// </summary>
    public void ProductNeedMatItem(SingleDanFarmData singleDanFarmData, int itemId, int num)
    {
        if (num <= 0)
            return;
        //消耗物品


        DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        singleDanFarmData.ProductSettingId = itemId;

        ItemSetting itemSetting = DataTable.FindItemSetting(itemId);
        List<List<int>> matList = CommonUtil.SplitCfg(itemSetting.Param2);
        for (int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int theId = singleMat[0];
            int singleNum = singleMat[1];
            int needNum = singleNum * num;
            if (ItemManager.Instance.FindItemCount(theId) < (ulong)needNum)
            {
                ItemSetting matSetting = DataTable.FindItemSetting(theId);
                PanelManager.Instance.OpenFloatWindow(matSetting.Name + "不够");
                return;
            }
        }

        for (int i = 0; i < matList.Count; i++)
        {
            List<int> singleMat = matList[i];
            int theId = singleMat[0];
            int singleNum = singleMat[1];
            int needNum = singleNum * num;
            ItemManager.Instance.LoseItem(theId, (ulong)needNum);
        }

        List<int> productIdList = CommonUtil.SplitCfgOneDepth(danFarmSetting.Param);
        List<int> speedList = CommonUtil.SplitCfgOneDepth(danFarmSetting.ProcessSpeed);
        int index = productIdList.IndexOf(itemId);
        singleDanFarmData.ProcessSpeed = speedList[0];
        singleDanFarmData.ProcessDanTimer = singleDanFarmData.ProcessSpeed;
        singleDanFarmData.ProductRemainNum = num;
        singleDanFarmData.ProductTotalNum = num;
        singleDanFarmData.SingleDanPrice = 1;
        singleDanFarmData.Status = (int)DanFarmStatusType.Working;

        EventCenter.Broadcast(TheEventType.StartMatDanFarmProduce, singleDanFarmData);
    }

    /// <summary>
    /// 计算丹价格
    /// </summary>
    public int CalcDanPrice(SingleDanFarmData data)
    {
       
        int price = data.SingleDanPrice;
        float addRate = 0;
        //自己弟子的加成
        for (int i = 0; i < data.ZuoZhenStudentIdList.Count; i++)
        {
            ulong onlyId = data.ZuoZhenStudentIdList[i];
            if (onlyId > 0)
            {
                addRate += 20;

                PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                if (p == null)
                {
                    continue;
                }
                if (p.talent == data.TalentType && p.propertyList != null && p.propertyList.Count > 0)
                {
                    //如果天赋是该天赋则加额外值
                    
                    float theval = (float)p.propertyList[0].num*80f/300f;
                    addRate += theval;
                }
            }
        }
        //if (data.ZuoZhenStudentIdList.Count > 0)
        //{
        //    addRate += 100;
        //}
        //计算未坐镇的弟子影响 平均分
        int usefulNum = 0;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            //未坐镇
            if (p.studentStatusType == (int)StudentStatusType.None)
            {
                usefulNum += 1;
            }
        }
        int danFarmNum = 0;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i].Status == (int)DanFarmStatusType.Working)
                danFarmNum++;
        }
        float theRate = 0;

        //如果没有任何丹田 则为0
        if (danFarmNum == 0)
        {
            theRate = 0;
        }
        else
        {
            theRate = usefulNum / (float)danFarmNum;
        }


        addRate = theRate * 20 + addRate+EfficientBuildingAdd(data.DanFarmType);

        if (data.DanFarmWorkType == (int)DanFarmWorkType.NeedMat)
            addRate = 0;

        price = Mathf.RoundToInt((1 + addRate / 100f) * price);


        return price;
    }

    /// <summary>
    /// 计算丹房效率
    /// </summary>
    /// <returns></returns>
    public int CalcDanFarmEfficient(SingleDanFarmData data)
    {
        int quanLiParam = 1;
        if (data.OpenQuanLi)
            quanLiParam *= 3;
        int thePrice = CalcDanPrice(data);
        return Mathf.RoundToInt((thePrice / (float)data.SingleDanPrice - 1) * 100 * quanLiParam);
    }

    /// <summary>
    /// 计算丹房月产出
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int CalcDanFarmProducePerMonth(SingleDanFarmData data, bool huLveQuanLi = false)
    {
        int quanLiParam = 1;
        
        if (!huLveQuanLi&&data.OpenQuanLi)
            quanLiParam *= 3;
        //int moonParam=
        //if (data.DanFarmWorkType == (int)DanFarmWorkType.NeedMat)
        int speed = data.ProcessSpeed;
        int numPerMoon = 1;
        //speed为0的时候是材料丹房没有任务
        if (speed != 0)
            numPerMoon = 30 / speed;
      
        return (CalcDanPrice(data) * numPerMoon) * quanLiParam;
    }

    /// <summary>
    /// 增加一个建筑 锁着的
    /// </summary>
    public SingleDanFarmData AddADanFarm()
    {
        SingleDanFarmData singleDanFarmData = new SingleDanFarmData();
        singleDanFarmData.IsEmpty = true;
        singleDanFarmData.Index = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count;
        singleDanFarmData.OnlyId = ConstantVal.SetId;
        for (int i = 0; i < 4; i++)
        {
            singleDanFarmData.ZuoZhenStudentIdList.Add(0);
        }
        //解锁弟子位
        for (int i = 0; i < 4; i++)
        {
            singleDanFarmData.PosUnlockStatusList.Add(false);
        }
        int limit = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmZuoZhenStudentLimit;
        for (int i = 0; i < limit; i++)
        {
            singleDanFarmData.PosUnlockStatusList[i] = true;
        }

        RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Add(singleDanFarmData);
        return singleDanFarmData;

    }



  

    /// <summary>
    /// 坐镇
    /// </summary>
    /// <param name="p"></param>
    /// <param name="data"></param>
    /// <param name="pos"></param>
    public void ZuoZhen(PeopleData p, SingleDanFarmData data, int pos)
    {
        data.ZuoZhenStudentIdList[pos] = p.onlyId;
        p.studentStatusType = (int)StudentStatusType.DanFarmWork;
        p.zuoZhenDanFarmOnlyId = data.OnlyId;

        AuditionManager.Instance.PlayVoice(AudioClipType.ZuoZhen);
        EventCenter.Broadcast(TheEventType.OnZuoZhenStudent, data, pos);
        DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(data.SettingId);
        RecordManager.Instance.AddTongZhi(p.name + "正在坐镇" + danFarmSetting.Name);

        TaskManager.Instance.TryAccomplishAllTask();
    }

    /// <summary>
    /// 找弟子在第几个位置坐镇
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public int FindStudentZuoZhenPos(PeopleData p)
    {
        if (p.studentStatusType == (int)StudentStatusType.DanFarmRelax
            || p.studentStatusType == (int)StudentStatusType.DanFarmWork
             || p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
        {
            //ulong farmIndex = p.zuoZhenDanFarmIndex;
            SingleDanFarmData data = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);//  RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmOnlyId];
            for(int i = 0; i < data.ZuoZhenStudentIdList.Count; i++)
            {
                ulong theId = data.ZuoZhenStudentIdList[i];
                if (theId == p.onlyId)
                    return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 全力产丹
    /// </summary>
    public void OnQuanLiFarm(SingleDanFarmData data)
    {
        List<PeopleData> curZuoZhenStudentList = new List<PeopleData>();

        if (JudgeIfCanQuanLi(data))
        {
            PanelManager.Instance.OpenCommonHint("确定消耗5点源力进行加速吗？", () =>
             {
                 StartQuanLi(data);

             }, null);

            



        }
    }

    /// <summary>
    /// 开始全力加速生产
    /// </summary>
    public void StartQuanLi(SingleDanFarmData data)
    {
        if (RoleManager.Instance.FindMyProperty((int)PropertyIdType.Tili).num < ConstantVal.quanLiCost)
        {
            PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
        }
        else
        {
            RoleManager.Instance.DeProperty(PropertyIdType.Tili, -ConstantVal.quanLiCost);

            data.OpenQuanLi = true;//全力产丹
            data.QuanLiTotalTime = 30;
            data.QuanliRemainTime = 30;//TODO用实验室升级
            for (int i = 0; i < data.ZuoZhenStudentIdList.Count; i++)
            {
                ulong theId = data.ZuoZhenStudentIdList[i];
                if (theId > 0)
                {
                    PeopleData p = StudentManager.Instance.FindStudent(theId);
                    p.studentStatusType = (int)StudentStatusType.DanFarmQuanLi;

                }
            }
            //for (int i = 0; i < curZuoZhenStudentList.Count; i++)
            //{


            //}
            TaskManager.Instance.GetAchievement(AchievementType.QuanLiTime, "");//全力生产
            EventCenter.Broadcast(TheEventType.QuanLiDanFarm, data);
            //埋点
            DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(data.SettingId);
           // TalkingDataGA.OnEvent("全力生产", new Dictionary<string, object>() { { danFarmSetting.name, 1 } });
        }
  
    }

    /// <summary>
    /// 停止坐镇
    /// </summary>
    public void StopZuoZhen(ulong onlyId)
    {


        PeopleData p = StudentManager.Instance.FindStudent(onlyId);
        int studentPosIndex = FindStudentZuoZhenPos(p);

        p.studentStatusType = (int)StudentStatusType.None;
        //int danFarmIndex = p.zuoZhenDanFarmIndex;
        SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[danFarmIndex];

        DanFarmSetting setting = DataTable.FindDanFarmSetting(danFarmData.SettingId);

        if (danFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
          && p.studentStatusType == (int)DanFarmStatusType.Working)
        {
            PanelManager.Instance.OpenCommonHint("请等待工作完成。", null, null);
            return;
        }


        p.zuoZhenDanFarmOnlyId = 0;

        danFarmData.ZuoZhenStudentIdList[studentPosIndex]=0;

        RecordManager.Instance.AddTongZhi(p.name + "离开" + setting.Name+",将协助所有建筑的生产");

        EventCenter.Broadcast(TheEventType.StopZuoZhenStudent, danFarmData);
    }

    /// <summary>
    /// 判定是否能全力
    /// </summary>
    /// <returns></returns>
    public bool JudgeIfCanQuanLi(SingleDanFarmData singleDanFarmData)
    {
        int cost = ConstantVal.quanLiCost;
        if (!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, cost))
        {
            return false;
        }
        if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Special
            || singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.NeedMat)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 拆除个丹房
    /// </summary>
    public void OnRemoveADanFarm(ulong farmonlyId)
    {
        SingleDanFarmData data = BuildingManager.Instance.FindDanFarmDataByOnlyId(farmonlyId);
        OnEndQuanLiDanFarm(data);

        //if (data.ZuoZhenStudentIdList.Count > 0)
        //{
        //    for(int i = data.ZuoZhenStudentIdList.Count-1; i >= 0; i--)
        //    {
        //        StopZuoZhen(data.ZuoZhenStudentIdList[i]);
        //    }
        //}
        for(int i = 0; i < data.ZuoZhenStudentIdList.Count; i++)
        {
            ulong onlyId = data.ZuoZhenStudentIdList[i];
            if (onlyId > 0)
            {
                StopZuoZhen(onlyId);
            }
        }
        data.CurLevel = 0;
        data.SettingId = 0;
        data.IsEmpty = true;
        data.Status =(int)DanFarmStatusType.None;
        data.DanFarmWorkType = (int)DanFarmWorkType.None;
        ReviveStudentPos(data);
        RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Remove(data);
        EventCenter.Broadcast(TheEventType.RemoveDanFarm, data);
    }

    public void ChangeFarmWorkStatus(SingleDanFarmData data,DanFarmStatusType status)
    {
        data.Status = (int)status;
        EventCenter.Broadcast(TheEventType.ChangeLianDanStatus, data);
    }
    /// <summary>
    /// 解锁丹房
    /// </summary>
    public void UnlockDanFarm(int danFarmId)
    {
        //if (!RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Contains(danFarmId))
        //{

            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Add(danFarmId);
        //}

    }

    /// <summary>
    /// 丹田状态显示
    /// </summary>
    /// <param name="danFarmStatusType"></param>
    /// <returns></returns>
    public string DanFarmStatusShow(DanFarmStatusType danFarmStatusType)
    {
        switch (danFarmStatusType)
        {
            case DanFarmStatusType.Working:
                return "工作中";
            case DanFarmStatusType.Building:
                return "建造中";
            case DanFarmStatusType.Upgrading:
                return "升级中";
            case DanFarmStatusType.Idling:
                return "空闲";
        }

        return "";
    }

    /// <summary>
    /// 找我有多少田
    /// </summary>
    /// <returns></returns>
    public int FindMyFarmNum(int farmId)
    {
        int res = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.SettingId == farmId)
            {
                res++;
            }
        }
        return res;
    }
    /// <summary>
    /// 找我的某田
    /// </summary>
    /// <returns></returns>
    public SingleDanFarmData FindMyFarm (int farmId)
    {
         for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.SettingId == farmId)
            {
                return data;
            }
        }
        return null;
    }
    /// <summary>
    /// 找某个房坐镇的所有弟子
    /// </summary>
    /// <returns></returns>
    public List<PeopleData> FindSingleFarmAllZuoZhenStudent(SingleDanFarmData data)
    {
        List<PeopleData> res = new List<PeopleData>();
        for(int i = 0; i < data.ZuoZhenStudentIdList.Count; i++)
        {
            ulong theId = data.ZuoZhenStudentIdList[i];
            if (theId > 0)
            {
                PeopleData p = StudentManager.Instance.FindStudent(theId);
                res.Add(p);
            }
        }
        return res;
    }

    /// <summary>
    /// 显示该房间坐镇的所有弟子
    /// </summary>
    /// <returns></returns>
    public void ShowSingleFarmAllStudent(Transform trans,SingleDanFarmData singleDanFarmData)
    {
        List<PeopleData> pList = FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        for(int i = 0; i < pList.Count; i++)
        {
            PanelManager.Instance.OpenSingle<SingleStudentView>(trans, pList[i]);
        }
    }

    /// <summary>
    /// 建筑解锁了的弟子位
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    /// <returns></returns>
    public int SingleFarmDataUnlockedStudentPosCount(SingleDanFarmData singleDanFarmData)
    {
        int res = 0;
        List<SingleDanFarmData> posList = new List<SingleDanFarmData>();

        for(int i = 0; i < singleDanFarmData.PosUnlockStatusList.Count; i++)
        {
            if (singleDanFarmData.PosUnlockStatusList[i])
                res++;
        }
        return res;
    }

    /// <summary>
    /// 找最好的炼器房
    /// </summary>
    /// <returns></returns>
    public SingleDanFarmData FindBestEquipFarm()
    {
        List<SingleDanFarmData> candidate = new List<SingleDanFarmData>();
        int maxVal = int.MinValue;
        SingleDanFarmData best = null;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.DanFarmType == (int)DanFarmType.LianQi)
            {
                List<PeopleData> pList = FindSingleFarmAllZuoZhenStudent(data);
                int proNum = 0;

                for (int j = 0; j < pList.Count; j++)
                {
                    PeopleData p = pList[j];
                    if (p.talent == (int)StudentTalent.DuanZhao)
                    {
                        for (int m = 0; m < p.propertyIdList.Count; m++)
                        {
                            int theId = p.propertyIdList[m];
                            if (theId == (int)PropertyIdType.ShiWu)
                            {
                                proNum += p.propertyList[m].num;
                            }
                        }
                    }

                }
                if(proNum>= maxVal)
                {
                    maxVal = proNum;
                    best = data;
                }
                //candidate.Add(data);
            }
        }
        return best;
    }

    /// <summary>
    /// 通过属性得到炼制宝石的品级
    /// </summary>
    /// <returns></returns>
    public Quality GetGemQualityByPro(int proNum)
    {
        TestStudentTotalProInfluenceSetting setting = DataTable.FindTestStudentTotalProInfluenceByPro(proNum);

        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);
        List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };


        int index = CommonUtil.GetIndexByWeight(weightList);
        Quality res = qualityList[index];
        return res;

    }
    /// <summary>
    /// 加速炼丹需要体力
    /// </summary>
    /// <param name="singleDanFarmData"></param>
    /// <returns></returns>
    public int JiaSuLianDanNeedTili(SingleDanFarmData singleDanFarmData)
    {
        int remainNum = singleDanFarmData.ProductRemainNum;
        int speed = singleDanFarmData.ProcessSpeed;
        int remainDay = speed * remainNum;
        //1点体力能减这么多天
        int single = ConstantVal.tiliReviveMinute * 60 / 2;
        int needTiLi = Mathf.CeilToInt(remainDay / (float)single);
        return needTiLi;
    }

    /// <summary>
    /// 渡修为给练功房
    /// </summary>
    public void DuXiuWei(SingleDanFarmData farm)
    {
        TrainSetting trainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex];

        int lastXiuWeiNeed = 0;
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex >= 1)
        {
            TrainSetting lastTrainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex - 1];
            lastXiuWeiNeed = lastTrainSetting.XiuWeiNeed.ToInt32();
        }

        int totalXiuWei = trainSetting.XiuWeiNeed.ToInt32() - lastXiuWeiNeed;
        int remainXiuWei = (int)(ulong)RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei- lastXiuWeiNeed;
        int theNum = totalXiuWei / 10;
        if (theNum > remainXiuWei)
            theNum = remainXiuWei;
        if (theNum <= 0)
        {
            PanelManager.Instance.OpenFloatWindow("您的修为不足，无法再减少了");

        }
        else
        {
            if (RoleManager.Instance.DeXiuWei(RoleManager.Instance._CurGameInfo.playerPeople, theNum))
            {
                farm.ProductRemainNum += theNum;
            }
        }
     
   

    }
  
    /// <summary>
    /// 停止产出
    /// </summary>
    /// <param name="data"></param>
    public void OnStopProduct(SingleDanFarmData data)
    {
        if(data.DanFarmWorkType==(int)DanFarmWorkType.Common
            && data.NeedForeItemId != 0)
        {
            data.HandleStop = true;
            ChangeFarmWorkStatus(data, DanFarmStatusType.Idling);
         
        }
    }

    /// <summary>
    /// 开始产出
    /// </summary>
    /// <param name="data"></param>
    public void OnStartProduct(SingleDanFarmData data)
    {
   
        int danPrice = CalcDanPrice(data);
        if (ItemManager.Instance.CheckIfItemEnough(data.NeedForeItemId, (ulong)danPrice))
        {
            data.HandleStop = false;
            ChangeFarmWorkStatus(data, DanFarmStatusType.Working);

        }
        else
        {
            ItemSetting itemSetting = DataTable.FindItemSetting(data.NeedForeItemId);
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name + "不够，无法生产");
        }
    }

    public void OnClickedFarm(SingleDanFarmData singleDanFarmData)
    {

        switch ((DanFarmType)singleDanFarmData.DanFarmType)
        {
            case DanFarmType.LianQi:
                PanelManager.Instance.OpenPanel<EquipMakeBuildingPanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);
                break;
            case DanFarmType.BaguaLu:
                PanelManager.Instance.OpenPanel<LianDanBuildingPanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);
                break;
            case DanFarmType.SpecialJiaCheng:
                PanelManager.Instance.OpenPanel<OtherFunctionFarmDetailPanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);
                break;
            default:
                PanelManager.Instance.OpenPanel<SingleDanFarmDetailPanel>(PanelManager.Instance.trans_layer2, singleDanFarmData);

                break;
        }



    }

    /// <summary>
    /// 是否有效率增加的建筑
    /// </summary>
    public int EfficientBuildingAdd(int farmType)
    {
         
        int res = 0;
        //灵树
        if (farmType == (int)DanFarmType.LingShu)
        {
            bool hongMengTianDiSatisfied=false;
            if(FindMyFarmNum((int)DanFarmIdType.HongMengShuLing) > 0)
            {
                res = ConstantVal.hongMengLingShuEfficientAdd;

                if(FindMyFarmNum((int)DanFarmIdType.HongMengKuangLing)>0
                    && FindMyFarmNum((int)DanFarmIdType.HongMengTianDao) > 0)
                {
                    hongMengTianDiSatisfied = true;
                    res += ConstantVal.hongMengTianDiEfficientAdd;
                }

            }

        }
        //矿场
        else if (farmType == (int)DanFarmType.YueLingkuang)
        {
            if (FindMyFarmNum((int)DanFarmIdType.HongMengKuangLing) > 0)
            {
                res = ConstantVal.hongMengKuangLingEfficientAdd;

                if (FindMyFarmNum((int)DanFarmIdType.HongMengShuLing) > 0
                  && FindMyFarmNum((int)DanFarmIdType.HongMengTianDao) > 0)
                {
                     res += ConstantVal.hongMengTianDiEfficientAdd;
                }
            }
        }
        //灵田
        else if (farmType == (int)DanFarmType.MoneyDan)
        {
            bool juBaoPen = false;
            bool shangGuPiXiu = false;
            bool xianZhuJinChan = false;
            //聚宝盆
            if (FindMyFarmNum((int)DanFarmIdType.JuBaoPen) > 0)
            {
                res = ConstantVal.juBaoPenEfficientAdd;
                juBaoPen = true;
            }
            //上古貔貅
            if (FindMyFarmNum((int)DanFarmIdType.ShangGuPiXiu) > 0)
            {
                res += ConstantVal.shangGuPiXiuEfficientAdd;
                shangGuPiXiu = true;
            }
            //衔珠金蟾
            if (FindMyFarmNum((int)DanFarmIdType.XianZhuJinChan) > 0)
            {
                res += ConstantVal.xianZhuJinChanEfficientAdd;
                xianZhuJinChan = true;
            }
            //财源滚滚效果
            if (juBaoPen && shangGuPiXiu && xianZhuJinChan)
            {
                res += ConstantVal.caiYuanGunGun;

            }
        }
        //藏经阁
        else if (farmType == (int)DanFarmType.CangJingGe)
        {
            if (FindMyFarmNum((int)DanFarmIdType.HongMengTianDao) > 0)
            {
                res = ConstantVal.hongMengTianDaoEfficientAdd;

                if (FindMyFarmNum((int)DanFarmIdType.HongMengShuLing) > 0
                  && FindMyFarmNum((int)DanFarmIdType.HongMengKuangLing) > 0)
                {
                     res += ConstantVal.hongMengTianDiEfficientAdd;
                }
            }
        }
        return res;
    }
}
/// <summary>
/// 炼丹状态 休息还是工作还是空闲
/// </summary>
public enum LianDanStudentStatusType
{
    None=0,
    Resting=1,
    Working=2,
    Idle=3,
}

/// <summary>
/// 丹房类型
/// </summary>
public enum DanFarmType
{
    None=0,
    MoneyDan=1,
    YueLingkuang=2,//月灵矿炉
    LingShu=3,//灵树  
    LianDanLu=4,//炼丹炉
    LianQi=5,//炼器房
    BaguaLu=6,//八卦炉 炼宝石
    CangJingGe=7,//藏经阁 生产技能残页
    CangKu=8,//仓库 用于弟子练功
    SpecialJiaCheng=9,//特殊加成
}

/// <summary>
/// 丹房工作类型
/// </summary>
public enum DanFarmWorkType
{
    None=0,
    Common=1,//普通丹房
    NeedMat=2,//需要原材料的丹房
    Special=3,//特殊
    CangKu=4,//仓库
    Other=5,//其它
}

