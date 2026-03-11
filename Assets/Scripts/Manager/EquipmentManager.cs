using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using System;
using cfg;

public class EquipmentManager : CommonInstance<EquipmentManager>
{

    //public List<ItemData> myEquipList = new List<ItemData>();//我的装备
    
    /// <summary>
    /// 开始做装备 TODO后续改成list 为了做多个装备
    /// </summary>
    public void StartMakeEquip(EquipmentSetting setting,SingleDanFarmData singleDanFarmData)
    {
        List<List<int>> makeCost = CommonUtil.SplitCfg(setting.MakeCost);

        for (int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            if (!ItemManager.Instance.CheckIfItemEnough(cost[0], (ulong)cost[1]))
            {
                PanelManager.Instance.OpenFloatWindow(DataTable.FindItemSetting(cost[0]).Name + "不足");
                return;
            }
        }

        for(int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            //if (!ItemManager.Instance.CheckIfItemEnough(cost[0], (ulong)cost[1]))
            //{
            //    PanelManager.Instance.OpenFloatWindow(DataTable.FindItemSetting(cost[0]).name + "不足");
            //    return;
            //}
            ItemManager.Instance.LoseItem(cost[0], (ulong)cost[1]);

        }


        //int studentCount = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count;
        int totalProNum = 0;

        List<PeopleData> pList= LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(singleDanFarmData);
        for (int i = 0; i < pList.Count; i++)
        {
            PeopleData p = pList[i];
            if (p.talent == (int)StudentTalent.DuanZhao)
            {
                for (int j = 0; j < p.propertyIdList.Count; j++)
                {
                    int id = p.propertyIdList[j];
                    SinglePropertyData data = p.propertyList[j];
                    if (id == (int)PropertyIdType.ShiWu)
                    {
                        totalProNum += data.num;
                    }
                }
            }
            p.studentStatusType = (int)StudentStatusType.DanFarmWork;

        }

        RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData = new EquipMakeData();
        RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.onlyId = ConstantVal.SetId;
        RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.settingId = setting.Id.ToInt32();
        RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.farmOnlyId = singleDanFarmData.OnlyId;
        //EquipMakeTeamData equipMakeTeamData = new EquipMakeTeamData();
        //equipMakeTeamData.PeopleOnlyId = studentOnlyId;

        //RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData.CurTeamData = equipMakeTeamData;
        Quality quality = GetEquipQualityByPro(totalProNum);
        RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.quality = (int)quality;

        List<List<int>> pro = CommonUtil.SplitCfg(setting.BasePro);
        for(int i = 0; i < pro.Count; i++)
        {
            List<int> singlePro = pro[i];
           // SinglePropertyData thePro = new SinglePropertyData();
            int theId = singlePro[0];
            int ProNum = singlePro[1];

            ProNum = Mathf.RoundToInt(ProNum * (1 + ((int)quality - 1) * 0.2f));
            RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList.Add(theId);
            RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.curProNumList.Add(0);
            RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.targetProNumList.Add(ProNum);

            //RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData.CurMakingEquipTargetPropertyList.Add(thePro);
        }
        singleDanFarmData.Status = (int)DanFarmStatusType.Working;
        SetEquipTeamStatus(EquipTeamStatusType.MakingEquip);

        EventCenter.Broadcast(TheEventType.StartMakeEquip, RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData);


        ////从0加到curProNum 做个小随机
        //float average = curProNum / 30;//平均每天加这么多
        //int remain = curProNum;
        //for(int i = 0; i < 30; i++)
        //{
        //    int realPro=RandomManager.Next(0, average
        //}

        //int base = 20;

    }

    /// <summary>
    /// 通过属性得到炼制装备的品级
    /// </summary>
    /// <returns></returns>
    public Quality GetEquipQualityByPro(int proNum)
    {
        TestStudentTotalProInfluenceSetting setting = DataTable.FindtestStudentTotalProInfluenceByPro(proNum);

        List<int> weightList = CommonUtil.SplitCfgOneDepth(setting.Influence);
        List<Quality> qualityList = new List<Quality> { Quality.Green, Quality.Blue, Quality.Purple, Quality.Orange, Quality.Gold };
 
     
        int index = CommonUtil.GetIndexByWeight(weightList);
        Quality res = qualityList[index];
        return res;

    }
    /// <summary>
    /// 装备制作进度 每天更新一次 
    /// </summary>
    /// <param name="processAdd"></param>
    public void OnEquipMakeProcess()
    {
        
        if (RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData != null)
        {
            RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.processDay += 1;
            int process = Mathf.RoundToInt(100*(RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.processDay /(float) ConstantVal.equipMakeTotalDay));
         
            if (process % 10 == 0)
            {
                OnEquipMakeAddProProcess(RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData);
            }
            EventCenter.Broadcast(TheEventType.MakeEquipProcessing, RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData);

            //做好了
            if (process >= 100)
            {
                //该位置丹房变成idle
                SingleDanFarmData farmData= BuildingManager.Instance.FindDanFarmDataByOnlyId(RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.farmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData.FarmOnlyId].Status =(int) DanFarmStatusType.Idling;
                farmData.Status = (int)DanFarmStatusType.Idling;
                EquipProtoData equipData = new EquipProtoData();
                equipData.onlyId = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.onlyId;
                equipData.settingId = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.settingId;
                equipData.setting = DataTable.FindEquipSetting(equipData.settingId);
                equipData.curLevel = 1;
                equipData.curDurability = 100;
                int allYouHuaLv = 0;
                for(int i=0;i < RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList.Count; i++)
                {
                    int theId = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList[i];
                    int theNum = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.curProNumList[i];
                    if (theId == (int)PropertyIdType.RdmProDamageAdd)
                    {
                        List<PropertyIdType> candidateIdList = new List<PropertyIdType>
                        {
                            PropertyIdType.WaterDamageAdd,
                            PropertyIdType.FireDamageAdd,
                        PropertyIdType.StormDamageAdd,
                        PropertyIdType.IceDamageAdd,
                        PropertyIdType.YangProDamageAdd,
                        PropertyIdType.YinProDamageAdd};
                        int proIdIndex= RandomManager.Next(0, candidateIdList.Count);
                        theId = (int)candidateIdList[proIdIndex];
                    }
                    equipData.propertyIdList.Add(theId);
                    SinglePropertyData data = new SinglePropertyData();
                    data.id = theId;
                    data.num = theNum;
                    data.quality = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.quality;
                    equipData.propertyList.Add(data);
                    allYouHuaLv += EquipmentManager.Instance.CalcNewYouHuaLv((Quality)(int)data.quality);
                }
                equipData.youHuaLv += allYouHuaLv / RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList.Count;
                EquipmentSetting equipmentSetting = DataTable.FindEquipSetting(equipData.settingId);
                
                // 添加空值检查
                if (equipmentSetting == null)
                {
                    Debug.LogError($"EquipmentSetting not found for equipData.settingId: {equipData.settingId}");
                    return;
                }
                
                ItemSetting itemSetting = DataTable.FindItemSetting(equipmentSetting.ItemId.ToInt32());
                
                // 添加空值检查
                if (itemSetting == null)
                {
                    Debug.LogError($"ItemSetting not found for ItemId: {equipmentSetting.ItemId}");
                    return;
                }
                
                ItemData item = RoleManager.Instance.GetEquipment(equipData, itemSetting.Quality.ToInt32());


                int itemId = DataTable.FindEquipSetting(equipData.settingId).ItemId.ToInt32();
                int rarity = DataTable.FindItemSetting(itemId).Rarity.ToInt32();
                //获得经验值
                //SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[RoleManager.Instance._CurGameInfo.AllEquipmentData.CurEquipMakeData.FarmIndex];
                List<PeopleData> pList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(farmData);
                List<int> beforeStudentExp = new List<int>();
                List<PeopleData> addExpPList = new List<PeopleData>();
                for (int i = 0; i < pList.Count; i++)
                {
                    int expGet = StudentMakeEquipExpGet(rarity);
                    PeopleData p = pList[i];

                    if (p.talent == farmData.TalentType)
                    {
                        beforeStudentExp.Add(p.studentCurExp);
                        addExpPList.Add(p);
                        StudentManager.Instance.OnGetStudentExp(p, expGet);
                    }

                    p.studentStatusType = (int)StudentStatusType.DanFarmRelax;

                }
                EventCenter.Broadcast(TheEventType.FinishMakeEquip, RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData);
                SetEquipTeamStatus(EquipTeamStatusType.Idle);
                PanelManager.Instance.OpenPanel<EquipMakeFinishPanel>(PanelManager.Instance.trans_layer2, item, RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.quality, beforeStudentExp,addExpPList);

                RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData = null;
                TaskManager.Instance.TryAccomplishGuideBook(TaskType.RarityEquipNum);
                TaskManager.Instance.GetDailyAchievement(TaskType.RarityEquipNum,"1");

#if !UNITY_EDITOR
                ArchiveManager.Instance.SaveArchive();
#endif
            }
        }

        


    }

    /// <summary>
    /// 装备加一次属性
    /// </summary>
    void OnEquipMakeAddProProcess(EquipMakeData equipMakeData)
    {
        for (int i = 0; i < equipMakeData.targetProNumList.Count; i++)
        {

            int curProNum = equipMakeData.curProNumList[i];
            int targetProNum = equipMakeData.targetProNumList[i];

            if (equipMakeData.processDay == ConstantVal.equipMakeTotalDay)
                curProNum = targetProNum;
            else
            {
                int rdmPercent = RandomManager.Next(0, 20);

                curProNum += Mathf.RoundToInt(targetProNum * rdmPercent * 0.01f);
                if (curProNum >= targetProNum)
                    curProNum = targetProNum;
            }


            equipMakeData.curProNumList[i] = curProNum;

        }
       
    }

 
    
    /// <summary>
    /// 设置装备制作团队的工作状态
    /// </summary>
    public void SetEquipTeamStatus(EquipTeamStatusType equipTeamStatusType)
    {
        if(RoleManager.Instance._CurGameInfo.EquipMakeTeamData!=null)
            RoleManager.Instance._CurGameInfo.EquipMakeTeamData.curStatus=(int)equipTeamStatusType;
    }


    /// <summary>
    /// 自动修装备，每过一天请求一次 每天修3%暂时 
    /// </summary>
    public void AutoFixEquipment()
    {
        return;

    }

    /// <summary>
    /// 装备是否满级
    /// </summary>
    /// <returns></returns>
    public bool IfEquipMaxLevel(EquipProtoData equip)
    {
        if (equip.curLevel >= 25)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 判断是否有装备
    /// </summary>
    /// <returns></returns>
    public bool IfHaveAnyEquip()
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData data = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(data.settingId);
            if (setting.ItemType.ToInt32() == (int)ItemType.Equip)
            {
                return true;
            }
            
        }
            return false;
    }

  

    /// <summary>
    /// 装备
    /// </summary>
    public void OnEquip(PeopleData p, ItemData data,int posIndex)
    {
        Debug.Log($"给弟子赠送武器 - 弟子: {p.name}, 武器ID: {data.settingId}, 装备位置: {posIndex}");
        long beforeZhanDouLi = RoleManager.Instance.CalcZhanDouLi(p);
        EquipmentSetting equipSetting = DataTable.FindEquipSetting(data.settingId);
        //比较前后属性 然后显示
        if (p.curEquipItemList[posIndex] != null)
        {
            OnUnEquip(p, p.curEquipItemList[posIndex],posIndex);
        }
        //普攻技能改变
        //int puGongSkillId = BattleManager.Instance.PuGongIdByYuanSu(p.yuanSu);
        SingleSkillData commonSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[0], p.allSkillData);
        commonSkill.skillLevel = data.equipProtoData.curLevel;
        p.allSkillData.equippedSkillIdList[0] = commonSkill.skillId;
        p.curEquipItemList[posIndex] = data;
        RoleManager.Instance.RefreshBattlePro(p);
        data.equipProtoData.belongP = p.onlyId;
        data.equipProtoData.isEquipped = true;
        ItemManager.Instance.LoseItem(data.onlyId);

        EventCenter.Broadcast(TheEventType.OnEquip,data);//发给ui
        long afterZhanDouLi = RoleManager.Instance.CalcZhanDouLi(p);
        
        if(p.isPlayer)   
            PanelManager.Instance.OpenZhanDouLiChangePanel(beforeZhanDouLi, afterZhanDouLi);

    }

    /// <summary>
    /// 卸下
    /// </summary>
    /// <param name="data"></param>
    public void OnUnEquip(PeopleData p, ItemData data,int posIndex)
    {
        long before = RoleManager.Instance.CalcZhanDouLi(p);
        data.equipProtoData.isEquipped = false;
        data.equipProtoData.belongP = 0;
        SingleSkillData commonSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[0], p.allSkillData);
        commonSkill.skillLevel = 1;
        p.curEquipItemList[posIndex] = null;
        RoleManager.Instance.RefreshBattlePro(p);
        ItemManager.Instance.GetItem(data);
        EventCenter.Broadcast(TheEventType.OnUnEquip, data);//发给ui
        long after= RoleManager.Instance.CalcZhanDouLi(p);

        if(p.isPlayer)
        PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);
    }



    /// <summary>
    /// 送图纸
    /// </summary>
    public void AddEquipPicture(int equipId)
    {
        bool haveSame = false;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllEquipmentData.pictureList.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.AllEquipmentData.pictureList[i].equipId; ;
            if (theId == equipId)
            {
                haveSame = true;
                break;
            }
        }
        if (!haveSame)
        {
            SingleEquipPictureData data = new SingleEquipPictureData();
            data.equipId = equipId;
            RoleManager.Instance._CurGameInfo.AllEquipmentData.pictureList.Add(data);
        }
    }


    /// <summary>
    /// 强化装备
    /// </summary>
    public void OnIntenseEquip(ItemData choosedEquipItem, SingleDanFarmData farmData=null)
    {
        long before = 0;
         PeopleData belong = null;
        // ItemData choosedEquipItem = null;
        if (choosedEquipItem.equipProtoData.belongP > 0)
        {
            belong = StudentManager.Instance.FindStudent(choosedEquipItem.equipProtoData.belongP);
            before = RoleManager.Instance.CalcZhanDouLi(belong);
        }
        ////是玩家的
        //if (belong == null)
        //{
        //    for (int i = 0; i < RoleManager.Instance._CurGameInfo.itemModel.itemDataList.Count; i++)
        //    {
        //        ItemData itemData = RoleManager.Instance._CurGameInfo.itemModel.itemDataList[i];
        //        if (itemData.onlyId == onlyId)
        //        {
        //            choosedEquipItem = itemData; 
        //            belong = RoleManager.Instance._CurGameInfo.playerPeople;
                
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    //ItemData choosedEquipItem = null;
        //    for(int i = 0; i < belong.curEquipItemList.Count; i++)
        //    {
        //        ItemData theData = belong.curEquipItemList[i];
        //        if (theData.onlyId == onlyId)
        //        {
        //            choosedEquipItem = theData;
        //            break;
        //        }
        //    }
        //    if (choosedEquipItem!=null)
        //    {
        //        //choosedEquipItem = belong.curEquipItem;
        //        string levelNeed = JudgeIfEquipIntenseSatisfyLevelCondition(choosedEquipItem.equipProtoData.curLevel);
        //        if (!string.IsNullOrWhiteSpace(levelNeed))
        //        {
        //             return;
        //        }
        //    }
        //}
        //if (choosedEquipItem == null)
        //    return;
        string levelNeed = JudgeIfEquipIntenseSatisfyLevelCondition(choosedEquipItem.equipProtoData.curLevel);
        if (!string.IsNullOrWhiteSpace(levelNeed))
        {
            PanelManager.Instance.OpenFloatWindow("通关" + levelNeed + "后可继续强化");
            return;
        }

        EquipProtoData data = choosedEquipItem.equipProtoData;

        if (IfEquipMaxLevel(data))
        {
            PanelManager.Instance.OpenFloatWindow("该装备已达最高等级");
            return;
        }
        EquipmentSetting equipSetting = DataTable.FindEquipSetting(data.settingId);

 

        //ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, needMoney);
        data.curLevel++;
        if (belong != null)
        {
            SingleSkillData commonSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(belong.allSkillData.equippedSkillIdList[0], belong.allSkillData);
            commonSkill.skillLevel = data.curLevel;
        }


        //EquipmentSetting equipmentSetting = DataTable.FindEquipSetting(data.SettingId);
        // List<List<int>> proList = CommonUtil.SplitCfg(equipmentSetting.proChange);
        // int addPro = proList[0][1];
        Rarity rarity = (Rarity)DataTable.FindItemSetting(choosedEquipItem.settingId).Rarity.ToInt32();
        int beforeMainProNum = data.propertyList[0].num;
        //主属性 5级之前不受弟子影响
        //float studentIntenseAdd = GetStudentIntenseAdd();


        bool per5LevelIntense = false;
         //每5级随机一个副属性增加 
        if (data.curLevel % 5 == 0)
        {
            
            per5LevelIntense = true;
            List<Quality> qualityList = new List<Quality>();
            List<int> afterProInitNum = new List<int>();
            afterProInitNum.Add(data.propertyList[0].num + GetMainProEquipIntenseAdd(data.jingLianLv, data.setting, 0).val);
             
            int proNum = GetFarmZuoZhenStudentTotalEquipPro(farmData);
            StudentProAddData mainProAddData = GetMainProEquipIntenseAdd(data.jingLianLv, data.setting,(Quality)(int)data.propertyList[0].quality);
            //data.propertyList[0].num += mainProAddData.val;
            int afterMainProNum = data.propertyList[0].num+mainProAddData.val;
            qualityList.Add(mainProAddData.quality);
           

            List<List<int>> upgradeAdd = CommonUtil.SplitCfg(ConstantVal.EquipIntenseAddByRarity(rarity));

            List<PropertyIdType> candidateProList = new List<PropertyIdType>();
            List<int> candidateProNumList = new List<int>();
            for (int j = 0; j < upgradeAdd.Count; j++)
            {
                List<int> thePro = upgradeAdd[j];
                candidateProList.Add((PropertyIdType)thePro[0]);
                candidateProNumList.Add(thePro[1]);
            }

            //用于显示变化的
            List<List<SinglePropertyData>> showChangeProList = new List<List<SinglePropertyData>>();
           
            SinglePropertyData before1 = new SinglePropertyData();
            before1.id = data.propertyIdList[0];
            before1.num = beforeMainProNum;
            SinglePropertyData after1 = new SinglePropertyData();
            after1.id = data.propertyIdList[0];
            after1.num = afterMainProNum;
            showChangeProList.Add(new List<SinglePropertyData> { before1, after1 });
          

           
            int index = RandomManager.Next(0, candidateProList.Count);
            PropertyIdType choosedProId = candidateProList[index];
            StudentProAddData afterViceProInitAddData = GetViceProIntenseAdd(data.jingLianLv, choosedProId, rarity, Quality.Green);
            StudentProAddData viceProAddData = GetViceProIntenseAdd(data.jingLianLv, choosedProId, rarity, mainProAddData.quality);

            int choosedProNum = afterViceProInitAddData.val;
            //StudentProAddData proAddData = GetBaseProEquipIntenseAddPer5Level(choosedProNum, farmData);
            qualityList.Add(mainProAddData.quality);
            int viceProAdd = viceProAddData.val;
            int newNum = 0;
            data.youHuaLv += CalcNewYouHuaLv(mainProAddData.quality);
            //如果该属性为已有的副属性
              
            List<SinglePropertyData> viceProList = new List<SinglePropertyData>();
            for (int j = 1; j < data.propertyIdList.Count; j++)
            {
                viceProList.Add(data.propertyList[j]);
            }
            List<SinglePropertyData> combinedProList = PropertyManager.Instance.CombineProperty(viceProList);
            List<int> viceProIdList = new List<int>();
            for (int j = 0; j < combinedProList.Count; j++)
            {
                viceProIdList.Add(combinedProList[j].id);

            }

            if (viceProIdList.Contains((int)choosedProId))
            {
                int choosedIndex = viceProIdList.IndexOf((int)choosedProId);

                int beforeViceNum = viceProList[choosedIndex].num;
                afterProInitNum.Add(viceProList[choosedIndex].num + choosedProNum);

                viceProList[choosedIndex].num += viceProAdd;

                int afterViceNum = viceProList[choosedIndex].num;
                newNum = viceProList[choosedIndex].num;

                SinglePropertyData before3 = new SinglePropertyData();
                before3.id = (int)choosedProId;
                before3.num = beforeViceNum;
                SinglePropertyData after3 = new SinglePropertyData();
                after3.id = (int)choosedProId;
                after3.num = afterViceNum;

                showChangeProList.Add(new List<SinglePropertyData> { before3, after3 });

            }
            //新属性
            else
            {
                //data.propertyIdList.Add((int)choosedProId);
                SinglePropertyData singlePropertyData = new SinglePropertyData();
                singlePropertyData.id = (int)choosedProId;
                singlePropertyData.num = viceProAdd;
                //data.propertyList.Add(singlePropertyData);
                newNum = singlePropertyData.num;
                afterProInitNum.Add(choosedProNum);

                SinglePropertyData after3 = new SinglePropertyData();
                after3.id = (int)choosedProId;
                after3.num = newNum;
                showChangeProList.Add(new List<SinglePropertyData> { after3 });

            }
            data.propertyIdList.Add((int)choosedProId);
            SinglePropertyData newVicePro = new SinglePropertyData();
            newVicePro.id = (int)choosedProId;
            newVicePro.num = viceProAdd;
            newVicePro.quality = (int)mainProAddData.quality;
            data.propertyList.Add(newVicePro);

            //弟子升级
            List<int> beforeExp = new List<int>();
            //TODO 这样查两次表太繁琐了 考虑保存稀有度
            //EquipmentSetting equipmentSetting = DataTable.FindEquipSetting(data.SettingId);
            ItemSetting itemSetting = DataTable.FindItemSetting(choosedEquipItem.settingId);
            int expGet = StudentIntenseExpGet(itemSetting.Rarity.ToInt32(), data.curLevel);
            if (farmData != null)
            {
                List<PeopleData> pList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(farmData);
                List<PeopleData> validStudentList = new List<PeopleData>();
                for (int j = 0; j < pList.Count; j++)
                { 
                    PeopleData p = pList[j];
                    if (p.talent == (int)StudentTalent.DuanZhao)
                    {
                        beforeExp.Add(p.studentCurExp);
                        StudentManager.Instance.OnGetStudentExp(p, expGet);
                        validStudentList.Add(p);
                    }
                }
                PanelManager.Instance.OpenPanel<EquipUpgradeResPanel>(PanelManager.Instance.trans_layer2, showChangeProList, data, beforeExp, validStudentList,qualityList,afterProInitNum);
            }

        }
        else
        {
            int proNum = GetFarmZuoZhenStudentTotalEquipPro(farmData);
            StudentProAddData mainProAddData = GetMainProEquipIntenseAdd(data.jingLianLv, data.setting, (Quality)(int)data.propertyList[0].quality);
            data.propertyList[0].num += mainProAddData.val;
            int afterMainProNum = data.propertyList[0].num;
        }


        if (data.isEquipped)
        {
            RoleManager.Instance.RefreshBattlePro(belong);

        }

        EventCenter.Broadcast(TheEventType.EquipIntense);
        TaskManager.Instance.TryAccomplishAllTask();
        if (belong != null)
        {
            long after = RoleManager.Instance.CalcZhanDouLi(belong);
            if (belong.isPlayer)
                PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);
        }

        TaskManager.Instance.GetDailyAchievement(TaskType.UpgradeRarityEquip, "1");
#if !UNITY_EDITOR
        if(per5LevelIntense)
                ArchiveManager.Instance.SaveArchive();
#endif
    }

    /// <summary>
    /// 装备属性刷新 处理老存档装备异常属性的问题 TODO正式版 删掉
    /// </summary>
    public void EquipRefreshPro(ItemData itemData)
    {
        //EquipmentSetting equipSetting = DataTable.FindEquipSetting(itemData.equipProtoData.SettingId);
        //List<List<int>> upgradeAdd = CommonUtil.SplitCfg(ConstantVal.EquipIntenseAddByRarity  );
        //int baseHPNum = 0;
        // for (int j = 0; j < upgradeAdd.Count; j++)
        //{
        //    List<int> thePro = upgradeAdd[j];
        //    if (thePro[0] ==(int) PropertyIdType.Hp)
        //    {
        //        baseHPNum = thePro[1];
        //    }
        
        //}

        //if (itemData.equipProtoData.CurLevel >= 5)
        //{
        //    for(int i = 2; i < itemData.equipProtoData.PropertyList.Count; i++)
        //    {
        //        SinglePropertyData thePro= itemData.equipProtoData.PropertyList[i];
        //        if (thePro.Id == (int)PropertyIdType.Hp&&thePro.Num>= baseHPNum*5*2f)
        //        {
        //            itemData.equipProtoData.PropertyList[i].Num /= 10;
        //        }
        //    }
        //}
    }


    /// <summary>
    /// 弟子强化装备经验获取
    /// </summary>
    /// <returns></returns>
    public int StudentMakeEquipExpGet(int rarity)
    {
        switch ((Rarity)rarity)
        {
            case Rarity.Fan:
                return 140;
            case Rarity.Huang:
                return 200;
            case Rarity.Xuan:
                return 260;
            case Rarity.Di:
                return 320;
            case Rarity.Tian:
                return 380;
        }
        return 0;
    }

    /// <summary>
    /// 弟子强化装备经验获取 每5级+一次
    /// </summary>
    /// <returns></returns>
    public int StudentIntenseExpGet(int rarity,int equipLevel)
    {
        int theVal = 0;
        switch ((Rarity)rarity)
        {
            case Rarity.Fan:
                theVal = 70;
                break;
            case Rarity.Huang:
                theVal = 100;
                break;

            case Rarity.Xuan:
                theVal = 130;
                break;

            case Rarity.Di:
                theVal = 160;
                break;

            case Rarity.Tian:
                theVal = 190;
                break;

        }
        return Mathf.RoundToInt(theVal * (equipLevel / 5f));
        //return 0;
    }



    /// <summary>
    /// 得到主属性的强化增加
    /// </summary>
    /// <param name="equipSettingId"></param>
    /// <returns></returns>
    public StudentProAddData GetMainProEquipIntenseAdd(int jingLianLv, EquipmentSetting setting, Quality quality)
    {
        int res = setting.UpgradeProAdd.ToInt32();
 
        res = GetMainProEquipIntenseNumAdd(jingLianLv, res, quality);

        StudentProAddData resData = new StudentProAddData(quality, res);
        return resData;
    }
    /// <summary>
    /// 得到主属性的强化增加
    /// </summary>
    /// <param name="equipSettingId"></param>
    /// <returns></returns>
    public StudentProAddData GetMainProEquipIntenseAdd(int jingLianLv, EquipmentSetting setting,int studentPro)
    {
        int res = setting.UpgradeProAdd.ToInt32();
 
  
        Quality quality = GetEquipQualityByPro(studentPro);
        res = GetMainProEquipIntenseNumAdd(jingLianLv, res, quality);

        StudentProAddData resData = new StudentProAddData(quality, res);
        return resData;
    }

    /// <summary>
    /// 得到主属性的强化增加
    /// </summary>
    /// <param name="equipSettingId"></param>
    /// <returns></returns>
    public int GetMainProEquipIntenseNumAdd(int jingLianLv, int baseNum, Quality quality)
    {
        int res = Mathf.RoundToInt(baseNum * (1 + jingLianLv * 0.2f) * (1 + ((int)quality - 1) * 0.2f));
        return res;
    }
    /// <summary>
    /// 副属性强化是多少
    /// </summary>
    public StudentProAddData GetViceProIntenseAdd(int jingLianLv, PropertyIdType proId, Rarity rarity,Quality quality)
    {
        StudentProAddData resData = null;
        List<List<int>> upgradeAdd = CommonUtil.SplitCfg(ConstantVal.EquipIntenseAddByRarity(rarity));

        List<PropertyIdType> candidateProList = new List<PropertyIdType>();
        List<int> candidateProNumList = new List<int>();
        for (int j = 0; j < upgradeAdd.Count; j++)
        {
            List<int> thePro = upgradeAdd[j];
            PropertyIdType theId = (PropertyIdType)thePro[0];
            int theNum = thePro[1];
            if(theId== proId)
            {
                theNum = GetMainProEquipIntenseNumAdd(jingLianLv, theNum, quality);
                resData= new StudentProAddData(quality, theNum);
 
            }
 
        }
        return resData;
    }

    /// <summary>
    /// 装备强化时 每5级的额外属性加多少
    /// </summary>
    /// <param name="propertyIdType"></param>
    /// <returns></returns>
    public StudentProAddData GetBaseProEquipIntenseAddPer5Level(int jingLianLv, int res,SingleDanFarmData farmData)
    {
   
        //根据质量再受一次影响
        int proNum = 0;
        if (farmData != null)
        {
            List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(farmData);
            for (int i = 0; i < studentList.Count; i++)
            {
                PeopleData p = studentList[i];
                if (p.talent == (int)StudentTalent.DuanZhao)
                {
                    for (int j = 0; j < p.propertyIdList.Count; j++)
                    {
                        int theId = p.propertyIdList[j];
                        if (theId == (int)PropertyIdType.ShiWu)
                        {
                            proNum += p.propertyList[j].num;
                        }
                    }
                }
            }
        }
  
        Quality quality = GetEquipQualityByPro(proNum);
        res =Mathf.RoundToInt(res * (1 + jingLianLv * 0.2f) * (1 + ((int)quality - 1) * 0.2f));
        return new StudentProAddData(quality,res);
    }

    /// <summary>
    /// 得到弟子属性强化加成
    /// </summary>
    /// <returns></returns>
    public float GetStudentIntenseAdd(int proNum)
    {
        //int proNum = 0;
        //for(int i = 0; i < RoleManager.Instance._CurGameInfo.StudentData.AllStudentList.Count; i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.StudentData.AllStudentList[i];
        //    if (p.Talent ==(int)StudentTalent.EquipMake)
        //    {
        //        for (int j = 0; j < p.PropertyIdList.Count; j++)
        //        {
        //            int theId = p.PropertyIdList[j];
        //            if (theId == (int)PropertyIdType.ShiWu)
        //            {
        //                proNum += p.PropertyList[j].Num;
        //            }
        //        }
        //    }
              
        //}
        //公式为1/380 *x+ 36/38 =y
        Quality quality = GetEquipQualityByPro(proNum);
        float res = (1 + ((int)quality - 1) * 0.2f);
        return res;
    }

    /// <summary>
    /// 镶嵌宝石
    /// </summary>
    public void OnInlayGem(PeopleData p, EquipProtoData equipData,ItemData gemItem,int index)
    {
        long before = RoleManager.Instance.CalcZhanDouLi();

        //equipProtoData = FindEquipProtoData(equipProtoData.OnlyId);
        if (equipData.gemList[index]== null|| equipData.gemList[index].onlyId<=0)
        {

            equipData.gemList[index] = gemItem;
                gemItem.gemData.isInlayed = true;
                if (equipData.isEquipped)
                {
                    RoleManager.Instance.RefreshBattlePro(p);
                    long after = RoleManager.Instance.CalcZhanDouLi();
                if(p.isPlayer)
                    PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);
                }
            ItemSetting setting = DataTable.FindItemSetting(gemItem.settingId);
            //TalkingDataGA.OnEvent("镶嵌宝石", new Dictionary<string, object>() { { setting.name, 1 } });
            TaskManager.Instance.TryAccomplishGuideBook(TaskType.InlayRarityGem);
            TaskManager.Instance.TryAccomplishAllTask();
            EventCenter.Broadcast(TheEventType.OnInlayGem);
            

        }
    }
    /// <summary>
    /// 卸下宝石
    /// </summary>
    /// <param name="equipProtoData"></param>
    /// <param name="gemItem"></param>
    /// <param name="index"></param>
    public void OnOffLayGem(PeopleData p, EquipProtoData equipData,int index)
    {
        long before = RoleManager.Instance.CalcZhanDouLi();

        //equipProtoData = FindEquipProtoData(equipProtoData.OnlyId);
        if (equipData.gemList[index]!=null && equipData.gemList[index].onlyId > 0)
        {
            ItemData gemItem = equipData.gemList[index];
            equipData.gemList[index] = null;
            gemItem.gemData.isInlayed = false;
            ItemManager.Instance.AddANewItem(gemItem);
            if (equipData.isEquipped)
            {
                RoleManager.Instance.RefreshBattlePro(p);
                long after = RoleManager.Instance.CalcZhanDouLi();
                if(p.isPlayer)
                PanelManager.Instance.OpenZhanDouLiChangePanel(before, after);
            }
            ItemSetting setting = DataTable.FindItemSetting(gemItem.settingId);
           // TalkingDataGA.OnEvent("卸下宝石", new Dictionary<string, object>() { { setting.name, 1 } });

            EventCenter.Broadcast(TheEventType.OnOffLayGem);
        }
    }



  
    /// <summary>
    /// 得到坐镇弟子的总属性
    /// </summary>
    /// <returns></returns>
    public int GetFarmZuoZhenStudentTotalEquipPro(SingleDanFarmData data)
    {
        if (data == null)
            return 0;
        int proNum = 0;
        List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(data);
        for(int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            for(int j = 0; j < p.propertyIdList.Count; j++)
            {
                int theId = p.propertyIdList[j];
                if (theId == (int)PropertyIdType.ShiWu)
                {
                    proNum += p.propertyList[j].num;
                }
            }
         
        }
        return proNum;
    }



    /// <summary>
    /// 装备分解
    /// </summary>
    public void OnEquipDissolve(ItemData item,SingleDanFarmData farmData)
    {
        if (item.equipProtoData.isEquipped)
        {
            PanelManager.Instance.OpenFloatWindow("已被装备，请先卸下");
            return;
        }
        for (int i = 0; i < item.equipProtoData.gemList.Count; i++)
        {
            ItemData gem = item.equipProtoData.gemList[i];
            if (gem!=null
                &&gem.onlyId != 0)
            {
                PanelManager.Instance.OpenFloatWindow("已镶嵌宝石，请先卸下");
                return;
            }
        }
        //材料
        List<int> matIdList = new List<int>();
        List<ulong> matNumList = new List<ulong>();
        EquipmentSetting setting = DataTable.FindEquipSetting(item.equipProtoData.settingId);

        List<List<int>> makeCost = CommonUtil.SplitCfg(setting.MakeCost);
        //制造的材料
        for (int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            if (!matIdList.Contains(cost[0]))
            {
                matIdList.Add(cost[0]);
                matNumList.Add(0);
            }
            int index = matIdList.IndexOf(cost[0]);
            matNumList[index] += (ulong)cost[1];
        }
        //强化的材料
        List<List<List<int>>> allCostList = CommonUtil.SplitThreeCfg(setting.UpgradeCost);
        for(int i=0;i< item.equipProtoData.curLevel-1; i++)
        {
            List<List<int>> curCost = allCostList[i];
            for (int j = 0; j < curCost.Count; j++)
            {
                List<int> cost = curCost[j];
                int costId = cost[0];
                int costNum = cost[1];
                if (!matIdList.Contains(cost[0]))
                {
                    matIdList.Add(cost[0]);
                    matNumList.Add(0);
                }
            
                int index = matIdList.IndexOf(costId);
                matNumList[index] += (ulong)costNum;

            }
        }
        int proNum = 0;
        List<PeopleData> studentList = LianDanManager.Instance.FindSingleFarmAllZuoZhenStudent(farmData);
        for (int i = 0; i < studentList.Count; i++)
        {
            PeopleData p = studentList[i];
            if (p.talent == (int)StudentTalent.DuanZhao)
            {
                for (int j = 0; j < p.propertyIdList.Count; j++)
                {
                    int theId = p.propertyIdList[j];
                    if (theId == (int)PropertyIdType.ShiWu)
                    {
                        proNum += p.propertyList[j].num;
                    }
                }
            }
        }
        float rate = Mathf.Lerp(0.2f, 0.8f,proNum/900f);
        List<ulong> initMatNumList = new List<ulong>();//弟子不加成的情况

        //最高0.8 最低0.8/1.8
        for (int i = 0; i < matNumList.Count; i++)
        {
            ulong theNum = matNumList[i];
            ulong initNum = (ulong)Mathf.RoundToInt(theNum * 0.2f);
            initMatNumList.Add(initNum);
            ulong actualNum = (ulong)Mathf.RoundToInt(theNum * rate);
            int costId = matIdList[i];
            ItemSetting itemSetting = DataTable.FindItemSetting(costId);
            if (itemSetting.ItemType.ToInt32() == (int)ItemType.OP)
            {
                actualNum = 1;
            }
            matNumList[i] = actualNum;
        }
        ItemManager.Instance.LoseItem(item.onlyId);
    
        for(int i = 0; i < matIdList.Count; i++)
        {
            ItemManager.Instance.GetItem(matIdList[i], matNumList[i]);
        }
        int quality = 1;
        float range = (0.8f - 0.2f) / 4;
        float left = 0.2f;
        float right = left + range;
        for(int i = 0; i < 5; i++)
        {
            left = left + range* i;
            right = left + range;
            if (rate >= left && rate <= right)
            {
                quality = i + 1;
                break;
            }
        }
        //显示
        EventCenter.Broadcast(TheEventType.DissolvedEquip, matIdList, matNumList, initMatNumList, quality);


    }

    /// <summary>
    /// 强化需要通过哪一关
    /// </summary>
    /// <param name="intenseLevel"></param>
    /// <returns></returns>
    public static string equipIntenseLevelNeed(int intenseLevel)
    {
        if (intenseLevel == 5)
        {
            return "0-5";
        }else if (intenseLevel == 10)
        {
            return "0-10";
        }
        else if (intenseLevel == 15)
        {
            return "0-10";
        }
        else if (intenseLevel == 20)
        {
            return "1-5";
        }
        else if (intenseLevel == 25)
        {
            return "1-10";
        }

        return "";
    }

    /// <summary>
    /// 判断是否装备强化满足关卡需求 如果返回空白字符串 说明可以强化 否则  返回应通过XX-XX的字符串 禁止强化
    /// </summary>
    /// <returns></returns>
    public string JudgeIfEquipIntenseSatisfyLevelCondition(int curlevel)
    {
        int nextLevel = curlevel + 1;
        string levelNeed = EquipmentManager.equipIntenseLevelNeed(nextLevel);
        if (!string.IsNullOrWhiteSpace(levelNeed))
        {
            int logicLevelNeed = ConstantVal.GetLevelByLevelStr(levelNeed);
            string recordLevel = TaskManager.Instance.FindAchievement(AchievementType.PassedMaxLevel);
            int logicRecordLevel = 0;
            if (!string.IsNullOrWhiteSpace(recordLevel))
                logicRecordLevel = ConstantVal.GetLevelByLevelStr(recordLevel);
            if (logicLevelNeed > logicRecordLevel)
            {
                return levelNeed;
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";

        }
    }

    /// <summary>
    /// 该装备是否有空位能装宝石
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public int CheckIfEquipCanInlayGem(ItemData itemData)
    {
        for(int i=0;i< itemData.equipProtoData.gemList.Count; i++)
        {
            ItemData gemItem = itemData.equipProtoData.gemList[i];
            if (gemItem==null||gemItem.onlyId<=0)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 计算装备优化率
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int CalcNewYouHuaLv(Quality newAddQuality)
    {
        return (int)(newAddQuality-1)*20;
    }

    public string PosTxt(int posIndex)
    {
        string res = "";
        switch (posIndex) 
        {
            case 0:
                res = LanguageUtil.GetLanguageText((int)LanguageIdType.法器);
                break;
            case 1:
                res = LanguageUtil.GetLanguageText((int)LanguageIdType.锦衣);
                break;
            case 2:
                res = LanguageUtil.GetLanguageText((int)LanguageIdType.鞋子);
                break;
            case 3:
                res = LanguageUtil.GetLanguageText((int)LanguageIdType.璎珞);
                break;

        }
        return res;

    }

    public List<EquipTaoZhuangType> CheckEquipTaoZhuang(PeopleData p)
    {
        List<EquipTaoZhuangType> typeList = new List<EquipTaoZhuangType>();

        Dictionary<int, int> taoZhuangDic = new Dictionary<int, int>();
        for (int i = 0; i < p.curEquipItemList.Count; i++)
        {
            ItemData data = p.curEquipItemList[i];
            if (data != null && data.settingId > 0)
            { 
                if (data.equipProtoData!=null && data.equipProtoData.setting!=null && !string.IsNullOrWhiteSpace(data.equipProtoData.setting.TaoZhuang))
                {
                    int taoZhuang = data.equipProtoData.setting.TaoZhuang.ToInt32();
                    if (!taoZhuangDic.ContainsKey(taoZhuang))
                        taoZhuangDic.Add(taoZhuang, 0);
                    taoZhuangDic[taoZhuang]++;
                }
            }
        }
        foreach(var item in taoZhuangDic)
        {
            //四件套齐全
            if (item.Value >= 4)
            {
                typeList.Add((EquipTaoZhuangType)item.Key);
                typeList.Add((EquipTaoZhuangType)item.Key);

            }
            //不到四件套 是2件套
            else if (item.Value >= 2)
            {
                typeList.Add((EquipTaoZhuangType)item.Key);

            }
        }
        return typeList;
    }

    /// <summary>
    /// 套装描述
    /// </summary>
    /// <param name="type"></param>
    /// <param name="siJian"></param>
    /// <returns></returns>
    public string TaoZhuangDes(EquipTaoZhuangType type,bool siJian = false)
    {
        string res = "";
        EquipTaoZhuangSetting setting = DataTable.FindEquipTaoZhuangSetting((int)type);
        string[] desArr = setting.Des.Split('|');
       
                if (!siJian)
                    res ="2件套："+ desArr[0];
                else
                    res ="4件套："+ desArr[1];
              
        
        return res;
    }
    /// <summary>
    /// 精炼消耗
    /// </summary>
    /// <param name="rarity"></param>
    /// <param name="curJingLianNum"></param>
    /// <returns></returns>
    public List<ItemData> EquipJingLianCostList(EquipmentSetting setting,int curJingLianNum)
    {
        List<ItemData> res = new List<ItemData>();
        ItemSetting itemSetting = DataTable.FindItemSetting(setting.Id.ToInt32());
        int rarity = itemSetting.Rarity.ToInt32();
        EquipUpgradeSetting upgradeSetting = DataTable._equipUpgradeList[rarity - 1];
        ItemSetting kuangSetting= DataTable.FindItemSettingByType(ItemType.EquipMat, rarity);
        List<int> kuangNumList = CommonUtil.SplitCfgOneDepth(upgradeSetting.NeedKuang);
        ulong needKuangNum =(ulong)kuangNumList[curJingLianNum];
        ItemData kuang = new ItemData();
        kuang.settingId = kuangSetting.Id.ToInt32();
        kuang.count = needKuangNum;
        res.Add(kuang);
        //器火
        if (!string.IsNullOrWhiteSpace(setting.QiHuo))
        {
            int qiHuoId = setting.QiHuo.ToInt32();
            List<int> qiHuoNumList = CommonUtil.SplitCfgOneDepth(upgradeSetting.NeedQiHuo);
            ulong needQiHuoNum = (ulong)qiHuoNumList[curJingLianNum];
            ItemData qiHuo = new ItemData();
            qiHuo.settingId = qiHuoId;
            qiHuo.count = needQiHuoNum;
            res.Add(qiHuo);
        }
        //原胚
        if (!string.IsNullOrWhiteSpace(setting.TuZhi))
        {
            int yuanPeiId = setting.TuZhi.ToInt32();
            List<int> yuanPeiNumList = CommonUtil.SplitCfgOneDepth(upgradeSetting.NeedYuanPei);
            ulong needYuanPeiNum = (ulong)yuanPeiNumList[curJingLianNum];
            ItemData yuanPei = new ItemData();
            yuanPei.settingId = yuanPeiId;
            yuanPei.count = needYuanPeiNum;
            res.Add(yuanPei);
        }
        return res;
    }

    public void OnJingLianEquip(ItemData data)
    {
        if (data.equipProtoData.jingLianLv <5)
        {
            List<ItemData> matList = EquipJingLianCostList(data.equipProtoData.setting, data.equipProtoData.jingLianLv);
            for(int i = 0; i < matList.Count; i++)
            {
                ItemData theData = matList[i];
                if (!ItemManager.Instance.CheckIfItemEnough(theData.settingId, theData.count))
                {
                    ItemSetting setting = DataTable.FindItemSetting(theData.settingId);
                    PanelManager.Instance.OpenFloatWindow(setting.Name + "不够");
                    return;
                }
            }
            for (int i = 0; i < matList.Count; i++)
            {
                ItemData theData = matList[i];
                ItemManager.Instance.LoseItem(theData.settingId, theData.count);
            }
            data.equipProtoData.jingLianLv++;
            data.equipProtoData.RefreshPro();
            if (data.equipProtoData.isEquipped)
            {
                PeopleData p = StudentManager.Instance.FindStudent(data.equipProtoData.belongP);
                RoleManager.Instance.RefreshBattlePro(p);
            }
            EventCenter.Broadcast(TheEventType.JingLianEquip);
        }
         
    }

    /// <summary>
    /// 自动装备
    /// </summary>
    public void AuToEquip(PeopleData p)
    {
        //找最高等级的
        List<ItemData> allEqiupList = ItemManager.Instance.FindItemListByType(ItemType.Equip);
        //过滤掉没有装备原型数据的物品
        allEqiupList = allEqiupList.FindAll(x => x.equipProtoData != null && x.setting != null);
        if (allEqiupList.Count == 0) return;
        
        //先按品级排
        for (int i = 0; i < allEqiupList.Count - 1; i++)
        {
            for (int j = 0; j < allEqiupList.Count - 1 - i; j++)
            {
                //前面的小于于后面的，则二者交换
                if (allEqiupList[j].setting.Rarity.ToInt32()
                    < allEqiupList[j + 1].setting.Rarity.ToInt32())
                {
                    var temp = allEqiupList[j];
                    allEqiupList[j] = allEqiupList[j + 1];
                    allEqiupList[j + 1] = temp;

                }
            }
        }
        //再按套装属性比较多的排
        for (int i = 0; i < allEqiupList.Count - 1; i++)
        {
            for (int j = 0; j < allEqiupList.Count - 1 - i; j++)
            {
                if(ItemManager.Instance.FindDifferentPosTaoZhuangItemList(allEqiupList[j].equipProtoData.setting.TaoZhuang.ToInt32(), allEqiupList[j].setting.Rarity.ToInt32()).Count
                    < ItemManager.Instance.FindDifferentPosTaoZhuangItemList(allEqiupList[j+1].equipProtoData.setting.TaoZhuang.ToInt32(), allEqiupList[j + 1].setting.Rarity.ToInt32()).Count)
                {
                    var temp = allEqiupList[j];
                    allEqiupList[j] = allEqiupList[j + 1];
                    allEqiupList[j + 1] = temp;
                }
            }
        }
        //再按星级比较高的排
        for (int i = 0; i < allEqiupList.Count - 1; i++)
        {
            for (int j = 0; j < allEqiupList.Count - 1 - i; j++)
            {
                if (allEqiupList[j].equipProtoData.jingLianLv
                    < allEqiupList[j + 1].equipProtoData.jingLianLv)
                {
                    var temp = allEqiupList[j];
                    allEqiupList[j] = allEqiupList[j + 1];
                    allEqiupList[j + 1] = temp;
                }
            }
        }
        //再按等级比较高的排
        for (int i = 0; i < allEqiupList.Count - 1; i++)
        {
            for (int j = 0; j < allEqiupList.Count - 1 - i; j++)
            {
                if (allEqiupList[j].equipProtoData.curLevel
                    < allEqiupList[j + 1].equipProtoData.curLevel)
                {
                    var temp = allEqiupList[j];
                    allEqiupList[j] = allEqiupList[j + 1];
                    allEqiupList[j + 1] = temp;
                }
            }
        }
        //一个个装上去
        for (int i = 0; i < p.curEquipItemList.Count; i++)
        {
            if (p.curEquipItemList[i] == null)
            {
                for(int j = 0; j < allEqiupList.Count; j++)
                {
                    ItemData item = allEqiupList[j];
                    if (item.equipProtoData.setting.Pos.ToInt32() == i)
                    {
                        OnEquip(p, item, i);
                        break;
                    }
                }
            }
        }

    }
}

public enum EquipTaoZhuangType
{
    None=0,
    DiXue=1,//帝血套
    TianLong=2,//天龙套
    ShenFeng=3,//神风套
    YuanShi=4,//原始套
    XuanWu=5,//玄武套
    FeiYun=6,//飞云套
    YueHua=7,//月华套
    YiMu=8,//乙木套
    DiGu=9,//帝骨套
    HongHuang=10,//洪荒套
}

//装备等级数据
public class LevelInfo
{
    public int canReachLevel;//能达到哪一级
    public int ExpAfterUpgrade;//生完所有级后剩余的经验值
    public int beforeExp;//之前的经验
    public int beforeLevel;//之前的等级
    public LevelInfo(int canReachLevel, int ExpAfterUpgrade, int beforeExp, int beforeLevel)
    {
        this.canReachLevel = canReachLevel;
        this.ExpAfterUpgrade = ExpAfterUpgrade;
        this.beforeExp = beforeExp;
        this.beforeLevel = beforeLevel;
    }
}

/// <summary>
/// 装备状态
/// </summary>
public enum EquipTeamStatusType
{
    None=0,
    Idle=1,
    MakingEquip=2,//做装备
    FixingEquip=3,//修理
}

/// <summary>
/// 装备id
/// </summary>
public enum EquipIdType  
{
    JieCaoJie=21101,//结草戒
    TaoMuJian=21201,//桃木剑
    DuoXianJian=24201,//堕仙剑
}

/// <summary>
/// 弟子属性加成数值
/// </summary>
public class StudentProAddData
{
    public StudentProAddData(Quality theQuality,int theVal)
    {
        quality = theQuality;
        val = theVal;
    }
    public Quality quality;
    public int val;
}