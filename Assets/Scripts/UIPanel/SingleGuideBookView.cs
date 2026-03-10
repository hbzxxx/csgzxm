using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;

public class SingleGuideBookView : SingleViewBase
{
    public SingleGuideBookTaskData guideBookTaskData;
    public GuideBookSetting setting;
    //public TaskSetting taskSetting;

    public Text txt_des;
    public Text txt_process;
    public Transform trans_accomplished;
    //public Transform trans_processing;
    public Transform trans_getAwarded;
    public Transform trans_awardGrid;
    public Button btn_getAward;
    public Button btn_go;


    public override void Init(params object[] args)
    {
        base.Init(args);
        guideBookTaskData = args[0] as SingleGuideBookTaskData;
        setting = args[1] as GuideBookSetting;
        //taskSetting = DataTable.FindTaskSetting(guideBookTaskData.settingId);
        addBtnListener(btn_getAward, () =>
        {
            TaskManager.Instance.OnGetGuideBookAward(guideBookTaskData, setting);
        });

        addBtnListener(btn_go, () =>
        {
            TaskType taskType = (TaskType)setting.Type.ToInt32();
            TaskManager.Instance.TryAccomplishGuideBook(taskType);
        });
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_des.SetText(setting.Des);
        txt_process.SetText(GuideBookProcess());
        List<List<int>> award = CommonUtil.SplitCfg(setting.Award);
        PanelManager.Instance.CloseAllSingle(trans_awardGrid);
        for(int i=0;i< award.Count; i++)
        {
            List<int> single = award[i];
            ItemData item = new ItemData();
            item.settingId = single[0];
            item.count = (ulong)single[1];
            if (DataTable.FindItemSetting(item.settingId) != null)
                PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid, item);
        }

        if (guideBookTaskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
        {
            trans_accomplished.gameObject.SetActive(true);
            //trans_processing.gameObject.SetActive(false);
            btn_go.gameObject.SetActive(false);
            trans_getAwarded.gameObject.SetActive(false);
        }
        else if (guideBookTaskData.accomplishStatus == (int)AccomplishStatus.Processing)
        {
            trans_accomplished.gameObject.SetActive(false);
            //trans_processing.gameObject.SetActive(true);
            btn_go.gameObject.SetActive(true);
            trans_getAwarded.gameObject.SetActive(false);
        }
        else if (guideBookTaskData.accomplishStatus == (int)AccomplishStatus.GetAward)
        {
            trans_accomplished.gameObject.SetActive(false);
            //trans_processing.gameObject.SetActive(false);
            btn_go.gameObject.SetActive(false);
            trans_getAwarded.gameObject.SetActive(true);
        }
    }



    public string GuideBookProcess()
    {
        int left = 0;
        int right = 0;
        switch ((TaskType)setting.Type.ToInt32())
        {
            //杀怪拿东西
            case TaskType.KillMonsterGetItem:
                List<int> itemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                 ItemData item = ItemManager.Instance.FindItemBySettingId(itemParam[0]);
                if(item != null)
                  left = (int)(ulong)item.count;

                right= itemParam[1];
                break;
             //收集物品
            case TaskType.ReceiveItem:
                List<int> itemParam2 = CommonUtil.SplitCfgOneDepth(setting.Param);
                ItemData item2 = ItemManager.Instance.FindItemBySettingId(itemParam2[0]);
                 if (item2!=null)
                 left = (int)(ulong)item2.count;
                 right= itemParam2[1];
                break;

            //炼丹
            case TaskType.LianDan:
                List<int> liandanParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int itemType = liandanParam[0];
                int num = liandanParam[1];

                left = ItemManager.Instance.FindItemListByType((ItemType)itemType).Count;
                right = num;
       
                break;
            //打怪
            case TaskType.KillEnemy:
                List<int> enemyParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int enemyId = enemyParam[0];
                int needKillEnemyNum = enemyParam[1];
                int enemyNum =TaskManager.Instance.FindAchievement(AchievementType.KillEnmey, enemyId.ToString()).ToInt32();
                left = enemyNum;
                right = needKillEnemyNum;
  
                break;

            //引导建个丹炉
            case TaskType.DanFarmNum:
                List<int> farmParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int needFarmId = farmParam[0];
                int needFarmNum = farmParam[1];
                int validNum = 0;

                if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count > 0)
                {
                    for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; j++)
                    {
                        SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j];
                        if (singleDanFarmData.IsEmpty)
                            continue;
                        if (singleDanFarmData.Status == (int)DanFarmStatusType.Building)
                            continue;
                        if (needFarmId == (int)DanFarmIdType.Any)
                        {

                            validNum++;

                        }
                        else
                        {
                            if (needFarmId == singleDanFarmData.SettingId)
                            {
                                validNum++;
                            }
                        }
                    }
 

                }
                left = validNum;
                right = needFarmNum;
                break;
            //引导招募
            case TaskType.ZhaoMuDiZi:
                int recruitStudentNum = setting.Param.ToInt32();
      
                left = RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count;
                right = recruitStudentNum;
                break;

            //炼制法器数量
            case TaskType.EquipNum:
                List<int> equipParam = CommonUtil.SplitCfgOneDepth(setting.Param);

                ItemIdType idType = (ItemIdType)equipParam[0];
                int needequipNum = equipParam[1];
                //curTaskSetting.needNum==1
                int haveNum = ItemManager.Instance.FindAllEquipmentList().Count;

                left = haveNum;
                right = needequipNum;
    
                break;
            //某级别法器数量
            case TaskType.RarityEquipNum:
                List<int> RarityEquipNumParam = CommonUtil.SplitCfgOneDepth(setting.Param);

                int rarity = RarityEquipNumParam[0];
                int needRarityEquipNum = RarityEquipNumParam[1];
                //curTaskSetting.needNum==1
                int haveRarityNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; j++)
                {
                    int settingId = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[j].settingId;
                    ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
                    if (itemSetting!=null && itemSetting.ItemType.ToInt32() == (int)ItemType.Equip
                        &&itemSetting.Rarity.ToInt32()==rarity)
                    {
                        haveRarityNum++;

                    }
                }
                List<PeopleData> RarityEquipNumParampList = StudentManager.Instance.FindAllMyTalentStudentList(StudentTalent.LianGong);
                for (int j = 0; j < RarityEquipNumParampList.Count; j++)
                {
                    PeopleData p = RarityEquipNumParampList[j];
                    for(int k = 0; k < p.curEquipItemList.Count; k++)
                    {
                        ItemData data = p.curEquipItemList[k];
                        if (data != null)
                        {
                            int settingId = data.settingId;
                            ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
                            if (itemSetting!=null && itemSetting.ItemType.ToInt32() == (int)ItemType.Equip
                                                  && itemSetting.Rarity.ToInt32() == rarity)
                            {
                                haveRarityNum++;

                            }
                        }
                    }
                   
                }
                left = haveRarityNum;
                right = needRarityEquipNum;

                break;
            //完成地图事件次数
            case TaskType.AccomplishMapEvent:
                List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int theMapEventType = mapEventParam[0];
                int needNum = mapEventParam[1];
                int mapEventNum =TaskManager.Instance.FindAchievement(AchievementType.AccomplishedMapEvent, theMapEventType.ToString()).ToInt32();
                left = mapEventNum;
                right = needNum;
          
                break;
            //玩家修为
            case TaskType.PlayerLevel: 
                int level = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex + 1;
                if (level >= setting.Param.ToInt32())
                {
                    left = 1;
                }
                right = 1;
                break;
            //装备技能
            case TaskType.EquipSkill:
                int skillId = setting.Param.ToInt32();
                if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList.Count >= 2)
                {
                    left = 1;
                }
             
                right = 1;

                break;
            //学习技能
            case TaskType.StudySkill:
                int StudySkillId = setting.Param.ToInt32();
                if (StudySkillId == -1)
                {
                    if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count >= 2)
                        left = 1;
                }
                else
                {
                    for (int j = 0; j < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; j++)
                    {
                        SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[j];
                        if (singleSkillData.skillId == StudySkillId)
                        {
                            left = 1;
                        }
                    }
                }
              
                right = 1;

                break;
            //升级技能
            case TaskType.UpgradeSkill:
                List<int> upgradeSkillParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                int UpgradeSkillId = upgradeSkillParamList[0];
                int UpgradeSkillLevel = upgradeSkillParamList[1];
                int maxSkillLevel = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; j++)
                {
                    SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[j];
                    if ((singleSkillData.skillId == UpgradeSkillId
                                || UpgradeSkillId == 0))
                    {
                        if(singleSkillData.skillLevel >= maxSkillLevel)
                        maxSkillLevel = singleSkillData.skillLevel;
                        break;
                    }
                }
                left = maxSkillLevel;
                right = UpgradeSkillLevel;
                break;
            //升级宗门
            case TaskType.UpgradeZongMen:
                int zongmenLevelNeed = setting.Param.ToInt32();
            
                left = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
                right = zongmenLevelNeed;

                break;
            case TaskType.QuanLiNum:
                int quanLiNumNeed = setting.Param.ToInt32();
          
                left = TaskManager.Instance.FindAchievement(AchievementType.QuanLiTime, "").ToInt32();
                right = quanLiNumNeed;
                break;
            //驻守建筑 建筑id|弟子数量|如果有 则是弟子天赋
            case TaskType.StudentZuoZhen:
                List<int> StudentZuoZhenParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                int StudentZuoZhenfarmId = StudentZuoZhenParamList[0];
                int StudentZuoZhenNum = StudentZuoZhenParamList[1];
                bool haveTalentCondition = false;//有天赋需求
                StudentTalent needTalent = StudentTalent.None;
                if (StudentZuoZhenParamList.Count >= 3)
                {
                    haveTalentCondition = true;
                    needTalent = (StudentTalent)StudentZuoZhenParamList[2];
                }
                int StudentZuoZhenvalidNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    if (p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                        || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                        || p.studentStatusType == (int)StudentStatusType.DanFarmWork)
                    {
                        SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];
                        if (danFarmData.SettingId == StudentZuoZhenfarmId)
                        {
                            if (haveTalentCondition)
                            {
                                if (p.talent == (int)needTalent)
                                {
                                    StudentZuoZhenvalidNum++;
                                }
                            }
                            else
                            {
                                StudentZuoZhenvalidNum++;
                            }
                        }

                    }
                }
                left = StudentZuoZhenvalidNum;
                right = StudentZuoZhenNum;
     
                break;
            //拥有a个b级c建筑 
            case TaskType.HaveABLevelCFarm:
                List<int> HaveABLevelCFarmParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                int farmId = HaveABLevelCFarmParamList[2];
                int needLevel = HaveABLevelCFarmParamList[1];
                int needHaveABLevelCFarmParamNum = HaveABLevelCFarmParamList[0];
                int validHaveABLevelCFarmParamNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; j++)
                {
                    SingleDanFarmData danFarm = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j];
                    if (danFarm.SettingId == farmId
                        && danFarm.CurLevel >= needLevel)
                    {
                        validHaveABLevelCFarmParamNum++;
                    }

                }
                left = validHaveABLevelCFarmParamNum;
                right = needHaveABLevelCFarmParamNum;
   

                break;
            //天赋觉醒次数
            case TaskType.TianFuJueXingNum:
                int TianFuJueXingNumNeed = setting.Param.ToInt32();
                left = TaskManager.Instance.FindAchievement(AchievementType.TianFuJueXingNum, "").ToInt32();
                right = TianFuJueXingNumNeed;
           
                break;
            //弟子突破次数
            case TaskType.StudentUpgradeCount:
                int StudentUpgradeCountNeed = setting.Param.ToInt32();
                left = TaskManager.Instance.FindAchievement(AchievementType.StudentUpgradeCount, "").ToInt32();
                right = StudentUpgradeCountNeed;


                break;
            //升级法器等级
            case TaskType.UpgradeEquip:
                int UpgradeEquipNeed = setting.Param.ToInt32();
                List<ItemData> equipItemList = ItemManager.Instance.FindAllEquipmentList();
                int maxLevel = 0;
                for (int j = 0; j < equipItemList.Count; j++)
                {
                    ItemData itemData = equipItemList[j];
                     
                        if (itemData.equipProtoData.curLevel > maxLevel)
                            maxLevel=itemData.equipProtoData.curLevel;
                   
                     
                }
                left = maxLevel;
                right = UpgradeEquipNeed;
                break;
            //升级某级别法器等级
            case TaskType.UpgradeRarityEquip:
                List<int> UpgradeRarityEquipNeed =CommonUtil.SplitCfgOneDepth(setting.Param);
                List<ItemData> UpgradeRarityEquipNeedequipItemList = ItemManager.Instance.FindAllEquipmentList();
                int UpgradeRarityEquipNeedmaxLevel = 0;
                for (int j = 0; j < UpgradeRarityEquipNeedequipItemList.Count; j++)
                {
                    ItemData itemData = UpgradeRarityEquipNeedequipItemList[j];
                    if (itemData.quality== UpgradeRarityEquipNeed[0])
                    {
                        if (itemData.equipProtoData != null && itemData.equipProtoData.curLevel > UpgradeRarityEquipNeedmaxLevel)
                            UpgradeRarityEquipNeedmaxLevel= itemData.equipProtoData.curLevel;

                    }
                }
                left = UpgradeRarityEquipNeedmaxLevel;
                right = UpgradeRarityEquipNeed[1];
                break;
            //解锁空地数量
            case TaskType.UnlockFarmPosNum:
                int UnlockFarmPosNumNeed = setting.Param.ToInt32();
               
                left = RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum;
                right = UnlockFarmPosNumNeed;
                break;
            //通过哪一关
            case TaskType.PassLevel:
                string needpassLevel = setting.Param;
                string myPassedMaxLevel =TaskManager.Instance.FindAchievement(AchievementType.PassedMaxLevel);
                int needLogicLevel = ConstantVal.GetLevelByLevelStr(needpassLevel);
                int myLogicLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLevel);
                if (myLogicLevel >= needLogicLevel)
                {
                    left = 1;
                }
                right = 1;
                 break;
            //通过哪一关裂隙
            case TaskType.PassedMaxLieXiLevel:
                string needpassLieXiLevel = setting.Param;
                string myPassedMaxLieXiLevel = TaskManager.Instance.FindAchievement(AchievementType.PassedMaxLieXiLevel);
                int needLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(needpassLieXiLevel);
                int myLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLieXiLevel);
                if (myLogicLieXiLevel >= needLogicLieXiLevel)
                {
                    left = 1;
                }
                right = 1;
                break;
            //弟子上阵
            case TaskType.ShangZhen:
                int needShangZhenNum = setting.Param.ToInt32();
                if (TeamManager.Instance.FindMyTeam1PNum() >= needShangZhenNum)
                {
                    left = 1;
                }
                right = 1;
                break;
            //装备法器
            case TaskType.EquipEquip:
                for(int i=0;i< RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList.Count; i++)
                {
                    ItemData data = RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList[i];
                    if (data != null)
                    {
                        left = 1;
                    }
                }
               
                right = 1;
                 break;
            //炼制宝石
            case TaskType.MakeGem:
                List<ItemData> gemItemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
                if (gemItemList.Count > 0)
                {
                    left = gemItemList.Count;
                }
                right = setting.Param.ToInt32();
                break;
            //镶嵌某级别宝石
            case TaskType.InlayRarityGem:
                List<int> inlayGemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int validGemNum = 0;
                List<ItemData> equipList = ItemManager.Instance.FindAllEquipmentList();
                for (int j = 0; j < equipList.Count; j++)
                {
                    ItemData itemData = equipList[j];
                    if (itemData.equipProtoData != null)
                    {
                        for (int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                        {
                            ItemData gem = itemData.equipProtoData.gemList[k];
                            if (gem != null&&gem.onlyId>0)
                            {
                                if (gem.quality == inlayGemParam[0])
                                    validGemNum++;
                            }
                        }
                    }
                }
                //for (int j = 0; j < RoleManager.Instance._CurGameInfo.itemModel.itemDataList.Count; j++)
                //{
                //    ItemData itemData = RoleManager.Instance._CurGameInfo.itemModel.itemDataList[j];
                //    if (itemData.equipProtoData != null)
                //    {
                //        for (int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                //        {
                //            ItemData gem = itemData.equipProtoData.gemList[k];
                //            if (gem != null)
                //            {
                //                if (gem.quality == inlayGemParam[0])
                //                    validGemNum++;
                                 
                //            }
                //        }
                //    }
                //}
                //List<PeopleData> studentList = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                //for (int j = 0; j < studentList.Count; j++)
                //{
                //    PeopleData p = studentList[j];
                //    for(int k = 0; k < p.curEquipItemList.Count; k++)
                //    {
                //        ItemData data = p.curEquipItemList[k];
                //        if (data != null
                //       && data.equipProtoData != null)
                //        {
                //            for (int m = 0; m < data.equipProtoData.gemList.Count; m++)
                //            {
                //                ItemData gem = data.equipProtoData.gemList[m];
                //                if (gem != null)
                //                {
                //                    if (gem.quality == inlayGemParam[0])
                //                        validGemNum++;
                //                }
                //            }
                //        }
                //    }
                   
                //}
                left = validGemNum;
                right = inlayGemParam[1];
                break;
            case TaskType.StudentHaveLevelGem:
                List<int> studentHaveGemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int validStudentGemNum = 0;
                List<PeopleData> studentList2 = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                for (int j = 0; j < studentList2.Count; j++)
                {
                    PeopleData p = studentList2[j];
                    for(int k = 0; k < p.curEquipItemList.Count; k++)
                    {
                        ItemData data = p.curEquipItemList[k];
                        if (data != null
                     && data.equipProtoData != null)
                        {
                            for (int m = 0; m < data.equipProtoData.gemList.Count; m++)
                            {
                                ItemData gem = data.equipProtoData.gemList[m];

                                if (gem != null&&gem.onlyId>0)
                                {
                                    GemSetting setting = DataTable.FindGemSetting(gem.settingId);
                                    if (setting.Level.ToInt32() == studentHaveGemParam[0]
                                        ||studentHaveGemParam[0]==0)
                                        validStudentGemNum++;
                                }
                            }
                        }
                    }
                 
                }
                left = validStudentGemNum;
                right = studentHaveGemParam[1];
               


                break;
            //拥有a名b天赋弟子
            case TaskType.HaveATalentBStudent:
                
                List<int> HaveATalentBStudentParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int needTalentNum = HaveATalentBStudentParam[0];
                int needTalentTalent = HaveATalentBStudentParam[1];
                int myNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    if (p.talent == needTalentTalent
                        ||needTalentTalent==0)
                    {
                        myNum++;
                    }
                }
                left = myNum;
                right = needTalentNum;
                break;
            //讨伐
            case TaskType.TaoFa:
                List<int> TaoFaParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int taoFaType = TaoFaParam[0];
                int taoFaNum = TaoFaParam[1];
               
                left = TaskManager.Instance.FindAchievement(AchievementType.TaoFa, taoFaType.ToString()).ToInt32();
                right = taoFaNum;
                break;
            //炼丹
            case TaskType.LianDan2:
                List<int> lianDan2Param = CommonUtil.SplitCfgOneDepth(setting.Param);
                int lianDan2Id = lianDan2Param[0];
                int lianDan2Count = lianDan2Param[1];
                left = TaskManager.Instance.FindAchievement(AchievementType.LianDan2, lianDan2Id.ToString()).ToInt32();
                right = lianDan2Count;
            
                break;
            //坐镇弟子数
            case TaskType.StudentZuoZhenTotalNum:
                int needZuoZhenNum = setting.Param.ToInt32();
                int totalZuoZhenNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    if (p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                        || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                        || p.studentStatusType == (int)StudentStatusType.DanFarmWork)
                    {
                        totalZuoZhenNum++;

                    }
                }
                left = totalZuoZhenNum;
                right = needZuoZhenNum;

                break;
            //秘境次数
            case TaskType.Explore:
                List<int> ExploreParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int needExploreNum = ExploreParam[1];
                int needExploreId = ExploreParam[0];
                int validExploreNum =TaskManager.Instance.FindAchievement(AchievementType.Explore, needExploreId.ToString()).ToInt32();
                left = validExploreNum;
                right = needExploreNum;
                break;
            //秘境次数
            case TaskType.MaxTaoFa:
                List<int> MaxTaoFaParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                int MaxTaoFaType = MaxTaoFaParam[0];
                int needMaxTaoFaLevel = MaxTaoFaParam[1];
                int myMaxTaoFaLevel = TaskManager.Instance.FindAchievement(AchievementType.MaxTaoFa, MaxTaoFaType.ToString()).ToInt32();
                left = myMaxTaoFaLevel;
                right = needMaxTaoFaLevel;
                break;
        }
        if (left > right)
            left = right;
        return left + "/" + right;
    }
}
