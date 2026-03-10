using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnapsackDownBtnView :SingleViewBase
{
    public Button btn;
    public Text txt;
    public KnapsackDownBtnType btnType;
    public ItemTipsPanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);
        btnType = (KnapsackDownBtnType)args[0];
        parentPanel = args[1] as ItemTipsPanel;
        ItemData data = parentPanel.itemData;
        ItemSetting setting = DataTable.FindItemSetting(data.settingId);
        addBtnListener(btn, () =>
        {
            switch (btnType)
            {
                case KnapsackDownBtnType.Equip:
                    TaskManager.Instance.triggerGuide_Equip = false;
                    PanelManager.Instance.ClosePanel(parentPanel);
                    PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2,StudentBigTag.LianGong);
                    //EquipmentSetting equipSetting = DataTable.FindEquipSetting(parentPanel.curChoosedItemView.setting.id.ToInt32());
                    //EquipmentManager.Instance.OnEquip(RoleManager.Instance._CurGameInfo.playerPeople, parentPanel.curChoosedItemView.GetItemData(), equipSetting.pos.ToInt32());
                    //AuditionManager.Instance.PlayVoice(AudioClipType.Equip);
                    if (TaskManager.Instance.guide_equipEquip)
                    {
                        TaskManager.Instance.guide_equipEquip = false;
                        PanelManager.Instance.CloseTaskGuidePanel();
                    }
                    break;
                case KnapsackDownBtnType.Intense: 
                
                    PanelManager.Instance.ClosePanel(parentPanel);
                    PanelManager.Instance.OpenPanel<EquipHandlePanel>(PanelManager.Instance.trans_layer2,parentPanel.itemData);
                 
                    break;
                //case KnapsackDownBtnType.UnEquip:
                //    EquipmentSetting equipSetting2 = DataTable.FindEquipSetting(parentPanel.curChoosedItemView.setting.id.ToInt32());
                //    EquipmentManager.Instance.OnUnEquip(RoleManager.Instance._CurGameInfo.playerPeople, parentPanel.curChoosedItemView.GetItemData(), equipSetting2.pos.ToInt32());
                //    break;
                //case KnapsackDownBtnType.Intense:
                //    JumpPageManager.Instance.JumpToIntense(parentPanel.curChoosedItemView.GetItemData().onlyId);
                //    //PanelManager.Instance.OpenPanel<EquipIntensePanel>(PanelManager.Instance.trans_layer2, parentPanel.curChoosedItemView.GetItemData().equipProtoData);
                //    break;
                //case KnapsackDownBtnType.AddGem:
                //    PanelManager.Instance.OpenPanel<EquipAddGemPanel>(PanelManager.Instance.trans_layer2, parentPanel.curChoosedItemView.GetItemData());
                //    break;
                case KnapsackDownBtnType.Composite:

                    if (string.IsNullOrWhiteSpace(setting.Param))
                    {
                        PanelManager.Instance.OpenFloatWindow("该宝石已经满级");
                        return;
                    }
                    else
                    {
                        //跳转八卦炉
                        JumpPageManager.Instance.JumpToGemBuilding();
                        //PanelManager.Instance.OpenPanel<GemCompositePanel>(PanelManager.Instance.trans_layer2, data);

                    }
                    break;
                case KnapsackDownBtnType.Sell:
                    ItemData sellData = parentPanel.itemData;
                    if (sellData.count <= 1)
                    {
                        PanelManager.Instance.OpenCommonHint("确定出售吗？", () =>
                        {
                            ItemManager.Instance.SellItem(sellData, (ulong)1);

                        }, null);
                    }
                    else
                    {
                        PanelManager.Instance.OpenPanel<SellSliderPanel>(PanelManager.Instance.trans_layer2, sellData);
                     }
                    break;
                    //学习技能
                case KnapsackDownBtnType.Study:
                    if (TaskManager.Instance.guide_studySkill)
                    {

                        if (parentPanel.itemData.setting.ItemType.ToInt32() == (int)ItemType.SkillBook)
                        {
                            PanelManager.Instance.CloseTaskGuidePanel();
                        }

                    }
                    PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
                    PanelManager.Instance.ClosePanel(parentPanel);
                    break;
                    //使用修为丹
                case KnapsackDownBtnType.Use:
                    if (setting.ItemType.ToInt32() == (int)ItemType.Dan)
                    {
                        StudentHandlePanel trainPanel= PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
                        PanelManager.Instance.ClosePanel(parentPanel);
                    }
                    //功法书I
                    else if (setting.ItemType.ToInt32() == (int)ItemType.GongFaShu)
                    {
                        int count = (int)(ulong)data.count;
                        List<int> candidateList = new List<int>();

                        List<int> choosedIdList = new List<int>();
                        List<ulong> choosedCountList = new List<ulong>();
                        List<ItemData> awardList = new List<ItemData>();

                        //默认全部打开
                        if (setting.Param == "0")
                        {
                            List<SkillSetting> list1 = DataTable.FindCanStudySkillListByYuanSu();


                            for (int i = 0; i < list1.Count; i++)
                            {
                                candidateList.Add(list1[i].Id.ToInt32());
                            }
                            //for (int i = 0; i < idList2.Count; i++)
                            //{
                            //    candidateList.Add((int)idList2[i]);
                            //}
                            //for (int i = 0; i < idList3.Count; i++)
                            //{
                            //    candidateList.Add((int)idList3[i]);
                            //}
                            //for (int i = 0; i < idList4.Count; i++)
                            //{
                            //    candidateList.Add((int)idList4[i]);
                            //}
                            //for (int i = 0; i < idList5.Count; i++)
                            //{
                            //    candidateList.Add((int)idList5[i]);
                            //}
                            //for (int i = 0; i < idList6.Count; i++)
                            //{
                            //    candidateList.Add((int)idList6[i]);
                            //}
                        }
                        //水雷阳
                        else if (setting.Param == "1")
                        {
                            List<SkillSetting> list1 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Water);
                            List<SkillSetting> list2 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Storm);
                            List<SkillSetting> list3 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Light);

                            //List<SkillIdType> idList1 = ConstantVal.BigSkillIdListByYuanSu(YuanSuType.Water);
                            //List<SkillIdType> idList2 = ConstantVal.BigSkillIdListByYuanSu(YuaPnSuType.Storm);
                            //List<SkillIdType> idList3 = ConstantVal.BigSkillIdListByYuanSu(YuanSuType.Light);
                            for (int i = 0; i < list1.Count; i++)
                            {
                                candidateList.Add( list1[i].Id.ToInt32());
                            }
                            for (int i = 0; i < list2.Count; i++)
                            {
                                candidateList.Add(list2[i].Id.ToInt32());
                            }
                            for (int i = 0; i < list3.Count; i++)
                            {
                                candidateList.Add(list3[i].Id.ToInt32());
                            }
                        }
                        //火冰暗
                        else if (setting.Param == "2")
                        {
                            List<SkillSetting> list1 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Fire);
                            List<SkillSetting> list2 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Ice);
                            List<SkillSetting> list3 = DataTable.FindCanStudySkillListByYuanSu(YuanSuType.Dark);
                            //List<SkillIdType> idList1 = ConstantVal.BigSkillIdListByYuanSu(YuanSuType.Fire);
                            //List<SkillIdType> idList2 = ConstantVal.BigSkillIdListByYuanSu(YuanSuType.Ice);
                            //List<SkillIdType> idList3 = ConstantVal.BigSkillIdListByYuanSu(YuanSuType.Dark);
                            for (int i = 0; i < list1.Count; i++)
                            {
                                candidateList.Add(list1[i].Id.ToInt32());
                            }
                            for (int i = 0; i < list2.Count; i++)
                            {
                                candidateList.Add(list2[i].Id.ToInt32());
                            }
                            for (int i = 0; i < list3.Count; i++)
                            {
                                candidateList.Add(list3[i].Id.ToInt32());
                            }
                        }
                        ulong num = (ulong)count;
                        for (ulong i = 0; i < num; i++)
                        {
                            int index = RandomManager.Next(0, candidateList.Count);
                            int id = candidateList[index];
                            if (!choosedIdList.Contains(id))
                            {
                                choosedIdList.Add(id);
                                choosedCountList.Add(1);

                            }
                            else
                            {
                                int existIndex = choosedIdList.IndexOf(id);
                                choosedCountList[existIndex]++;
                            }
                        }
                        ItemManager.Instance.LoseItem(data.onlyId);
                        ItemManager.Instance.GetItemWithAwardPanel(choosedIdList, choosedCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //源力结晶I
                    else if (setting.ItemType.ToInt32() == (int)ItemType.YuanLi)
                    {
                        if (ItemManager.Instance.CheckIfItemEnough(data.settingId, 1))
                        {
                            ItemManager.Instance.LoseItem(data.settingId, 1);
                            RoleManager.Instance.AddProperty(PropertyIdType.Tili, setting.Param.ToInt32());
                            PanelManager.Instance.OpenFloatWindow("已回复");
                            if (!ItemManager.Instance.CheckIfItemEnough(data.settingId, 1))
                                PanelManager.Instance.ClosePanel(parentPanel);
                        }
                        else 
                        { 
                            
                        }
                    }   
                    //宝石粉末箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.XingChenBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int count = paramList[0];
                        int rarity = paramList[1];
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                 
                            List<int> possibleList = new List<int>();
                            foreach (var theSetting in DataTable.table.TbItem.DataList)
                            {
                                if (theSetting.ItemType.ToInt32() == (int)ItemType.XingChen
                                     && theSetting.Rarity.ToInt32() == rarity)
                                    possibleList.Add(theSetting.Id.ToInt32());
                            }
                            int index = RandomManager.Next(0, possibleList.Count);
                            int choosedId = possibleList[index];
                            if (!awardIdList.Contains(choosedId))
                            {
                                awardIdList.Add(choosedId);
                                awardCountList.Add(0);
                            }
                            int theIndex = awardIdList.IndexOf(choosedId);
                            awardCountList[theIndex] += (ulong)count;
                        }

                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //念珠箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.NianZhuBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int count = paramList[0];
                        int rarity = paramList[1];
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            List<int> possibleList = new List<int>();
                            foreach (var theSetting in DataTable.table.TbItem.DataList)
                            {
                                if (theSetting.ItemType.ToInt32() == (int)ItemType.LingQuan
                                     && theSetting.Rarity.ToInt32() == rarity
                                     && theSetting.Id.ToInt32() % 10 != 1)
                                    possibleList.Add(theSetting.Id.ToInt32());
                            }

                            int index = RandomManager.Next(0, possibleList.Count);
                            int choosedId = possibleList[index];

                            if (!awardIdList.Contains(choosedId))
                            {
                                awardIdList.Add(choosedId);
                                awardCountList.Add(0);
                            }
                            int theIndex = awardIdList.IndexOf(choosedId);
                            awardCountList[theIndex] += (ulong)count;
                        }
                      

                        ItemManager.Instance.LoseItem(data.settingId, data.count);

                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //建筑材料箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.BuildingUpgradeBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int theId = paramList[0];
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            int count = RandomManager.Next(paramList[1], paramList[2] + 1);
                            awardIdList.Add(theId);
                            awardCountList.Add((ulong)count);
                        }

                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //清灵草箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.QingLingCaoBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int theId = paramList[0];
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            int count = paramList[1];
                            int bigRate = ConstantVal.bigBoxRate;
                            int theVal = RandomManager.Next(0, 1000);
                            //大奖
                            if (theVal < bigRate)
                            {
                                count *= 3;
                            }
                            awardIdList.Add(theId);
                            awardCountList.Add((ulong)count);
                        }
                        //排序
                        for (int i = 0; i < awardCountList.Count - 1; i++)
                        {
                            for (int j = 0; j < awardCountList.Count - 1 - i; j++)
                            {
                                //前面的小于于后面的，则二者交换
                                if (awardCountList[j]
                                    < awardCountList[j + 1])
                                {
                                    var temp = awardCountList[j];
                                    awardCountList[j] = awardCountList[j + 1];
                                    awardCountList[j + 1] = temp;

                                }
                            }
                        }
                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //灵石福袋
                    else if (setting.ItemType.ToInt32() == (int)ItemType.LingShiFuDai)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int theId = paramList[0];
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            int count = paramList[1];
                            int bigRate = ConstantVal.bigBoxRate;
                            int theVal = RandomManager.Next(0, 1000);
                            //大奖
                            if (theVal < bigRate)
                            {
                                count *= 10;
                            }
                            awardIdList.Add(theId);
                            awardCountList.Add((ulong)count);
                        }
                        //排序
                        for (int i = 0; i < awardCountList.Count - 1; i++)
                        {
                            for (int j = 0; j < awardCountList.Count - 1 - i; j++)
                            {
                                //前面的小于于后面的，则二者交换
                                if (awardCountList[j]
                                    < awardCountList[j + 1])
                                {
                                    var temp = awardCountList[j];
                                    awardCountList[j] = awardCountList[j + 1];
                                    awardCountList[j + 1] = temp;

                                }
                            }
                        }
                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //天级弟子接引令
                    else if (setting.ItemType.ToInt32() == (int)ItemType.JieYinLing)
                    {
                        //已达上限无法使用 已达最大招募次数无法使用
                        if (BuildingManager.Instance.CheckIfReachBuildingMaxNeiMenNumLimit())
                        {
            PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.当前弟子人数已达上限请提升宗门等级));
                            return;
                        }
                        else if (RoleManager.Instance._CurGameInfo.studentData.thisYearRecruitedStudentNum >= RoleManager.Instance._CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum)
                        {
                            PanelManager.Instance.OpenPanel<AddStudentRecruitNumADPanel>(PanelManager.Instance.trans_layer2);
                            return;
                        }
                  
                        //招募成功
                        else
                        {
                            ItemManager.Instance.LoseItem(data.settingId, 1);

                            int gender = RandomManager.Next(1, 3);
                            PeopleData p= StudentManager.Instance.RdmGenerateStudent(setting.Param.ToInt32(), setting.Param.ToInt32(), (Gender)gender);
                            StudentManager.Instance.RecruitStudent(p,RecruitStudentType.JieYinLing);
                        }
                        PanelManager.Instance.ClosePanel(parentPanel);


                    }
                    //原胚箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.OPBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        //List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.param);
                        //int theId = paramList[0];
                        List<ItemSetting> candidateList= DataTable.FindItemSettingListByType(ItemType.OP, setting.Rarity.ToInt32());
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            int theId =candidateList[ RandomManager.Next(0, candidateList.Count)].Id.ToInt32();
                            int count = 1;
                            awardIdList.Add(theId);
                            awardCountList.Add((ulong)count);
                        }

                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }  
                    //器火箱子
                    else if (setting.ItemType.ToInt32() == (int)ItemType.QiHuoBox)
                    {
                        List<int> awardIdList = new List<int>();
                        List<ulong> awardCountList = new List<ulong>();
                        //List<int> paramList = CommonUtil.SplitCfgOneDepth(setting.param);
                        //int theId = paramList[0];
                        List<ItemSetting> candidateList = DataTable.FindItemSettingListByType(ItemType.QiHuo, setting.Rarity.ToInt32());
                        //默认全部打开
                        for (ulong i = 0; i < data.count; i++)
                        {
                            int theId = candidateList[RandomManager.Next(0, candidateList.Count)].Id.ToInt32();
                            int count = setting.Param.ToInt32();
                            awardIdList.Add(theId);
                            awardCountList.Add((ulong)count);
                        }

                        ItemManager.Instance.LoseItem(data.settingId, data.count);
                        ItemManager.Instance.GetItemWithAwardPanel(awardIdList, awardCountList);
                        PanelManager.Instance.ClosePanel(parentPanel);

                    }
                    //代金券P
                    else if (setting.ItemType.ToInt32() == (int)ItemType.FanLiDaiJinQuan)
                    {
                        //返利代金券
                        if (GameTimeManager.Instance.connectedToFuWuQiTime
                    && Game.Instance.isRightEdition
                    && Game.Instance.isLogin)
                        {
                            ShopItemData shopItem = ShopManager.Instance.FindShopItemDataByShopId(data.setting.Param.ToInt32());
                            if (shopItem != null)
                            {
                                if (ItemManager.Instance.LoseItem(data.settingId, data.count))
                                {

                                    ShopManager.Instance.OnRMBBuy(shopItem, GameTimeManager.Instance.curFuWuQiTime);
                                    PanelManager.Instance.ClosePanel(parentPanel);
                                    PanelManager.Instance.OpenFloatWindow("使用成功");


                                }
                            }



                        }
                        else
                        {
                            PanelManager.Instance.OpenFloatWindow("未连接到服务器，请重新登录后使用");
                        }

                    }
                    break;
            }

        });

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        switch (btnType)
        {
            case KnapsackDownBtnType.AddGem:
                txt.SetText("宝石");
                break;
            case KnapsackDownBtnType.Composite:
                txt.SetText("合成");
                break;
            case KnapsackDownBtnType.Intense:
                txt.SetText("强化");
                break;
            case KnapsackDownBtnType.Sell:
                txt.SetText("出售");
                break;
            case KnapsackDownBtnType.Equip:
                txt.SetText("装备");
                break;
            case KnapsackDownBtnType.UnEquip:
                txt.SetText("卸下");
                break;
            case KnapsackDownBtnType.Study:
                txt.SetText("学习");
                break;
            case KnapsackDownBtnType.Use:
                txt.SetText("使用");
                break;
        }
    }

}


public enum KnapsackDownBtnType
{
    None=0,
    Intense,//强化
    AddGem,//镶嵌宝石
    Sell,//出售
    Composite,//合成宝石
    Equip,//装备
    UnEquip,//卸下
    Study,//学习 技能
    Use,//使用
}