 using cfg;
using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChouJiangManager : CommonInstance<ChouJiangManager>
{
    public GameInfo curGameInfo;

    public override void Init()
    {
        base.Init();
        curGameInfo = RoleManager.Instance._CurGameInfo;
        if (curGameInfo.ChouJiangData == null)
        {
            curGameInfo.ChouJiangData = new ChouJiangData();
        }
        //if (curGameInfo.chouJiangDataDic.Count <= 0)
        //{
        //     ChouJiangData data1 = new ChouJiangData();
        //    data1.type = ChouJiangType.XinWu;
        //    ChouJiangData data2 = new ChouJiangData();
        //    data2.type = ChouJiangType.Skill;

        //    curGameInfo.chouJiangDataDic.Add(data1.type, data1);
        //    curGameInfo.chouJiangDataDic.Add(data2.type,data2);

        //}

    }
    /// <summary>
    /// 抽奖
    /// </summary>
    public void OnChou(ChouJiangData data,int num)
    {
        // 检查每日抽奖次数限制
        int dailyLimit = 50;
        int todayUsedTimes = (int)curGameInfo.timeData.TodayChouNum;
        
        if (todayUsedTimes + num > dailyLimit)
        {
            int remainTimes = dailyLimit - todayUsedTimes;
            if (remainTimes <= 0)
            {
                PanelManager.Instance.OpenOnlyOkHint("今日抽奖次数已达上限（50次），请明天再来！", null);
                return;
            }
            else
            {
                PanelManager.Instance.OpenOnlyOkHint($"今日还可抽奖{remainTimes}次，无法进行{num}连抽。", null);
                return;
            }
        }
        
        ChouJiangResData finalRes=new ChouJiangResData();
        int needQuanNum = num;
        if (num == 10)
            needQuanNum = 9;
        //long zuanShiNeed = ConstantVal.chouJiangNeedZuanShi * num;
        ulong myItemNum = ItemManager.Instance.FindItemCount((int)ItemIdType.TianJiFu);
        if (myItemNum< (ulong)needQuanNum)
        {
            ItemSetting needItemSetting = DataTable.FindItemSetting((int)ItemIdType.TianJiFu);
            ulong needMoreNum = (ulong)needQuanNum - myItemNum;
            ulong buyNeedTianJingNum = needItemSetting.Param.ToUInt64() * needMoreNum;
            PanelManager.Instance.OpenCommonHint(
                needItemSetting.Name + "不够,是否花费" + buyNeedTianJingNum + "天晶补充" + needMoreNum + "张？", 
                ()=>
                {
                    if (ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.TianJing, buyNeedTianJingNum))
                    {
                        if (ItemManager.Instance.LoseItem((int)ItemIdType.TianJing, buyNeedTianJingNum))
                        {
                            ItemManager.Instance.GetItem((int)ItemIdType.TianJiFu, needMoreNum);
                            OnChou(data, num);
                        }
                    }else
                    {
                        PanelManager.Instance.OpenCommonHint("天晶不够，是否前往购买？", () =>
                        {
                            PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.ChongZhi);
                        }, null);
                    }
                }
                ,null);
            return;
        }
        ItemManager.Instance.LoseItem((int)ItemIdType.TianJiFu, (ulong)needQuanNum);
        List<ItemData> res = new List<ItemData>();

        Rarity resRarity = Rarity.Xuan;
        for (int i = 0; i < num; i++)
        {
            data.baoDi10Num++;
            data.baoDi50Num++;
            ItemData item = SingleChou(data);
            res.Add(item);
            //地级弟子接引令
            if (item.settingId == (int)ItemIdType.DiJiDiZiJieYinLing)
            {
                if (resRarity < Rarity.Di)
                    resRarity = Rarity.Di;
            }
            //天级弟子接引令
            else if (item.settingId == (int)ItemIdType.TianJiDiZiJieYinLing)
            {
                if (resRarity < Rarity.Tian)
                    resRarity = Rarity.Tian;
            }
            ItemManager.Instance.GetItem(item.settingId,item.count);
        }
        finalRes.resList = res;

        finalRes.resRarity = resRarity;
        
        // 增加今日抽奖次数
        curGameInfo.timeData.TodayChouNum += num;

        EventCenter.Broadcast(TheEventType.OnChou, finalRes);
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
    }

    /// <summary>
    /// 单抽
    /// </summary>
    public ItemData SingleChou(ChouJiangData data)
    {
        string resStr = "";
        //保底没有
        if (data.baoDi50Num % 50 == 0)
        {
            resStr = (int)ItemIdType.TianJiDiZiJieYinLing + "|"+1;
            data.baoDi50Num = 0;
        }
        else if (data.baoDi10Num % 10 == 0)
        {
            resStr = (int)ItemIdType.DiJiDiZiJieYinLing + "|" + 1;
            data.baoDi10Num = 0;
        }
        else
        {
            List<string> candidate = new List<string>();
            List<int> weightList = new List<int> ();
          
            for(int i = 0; i < DataTable.table.TbChouJiang.DataList.Count; i++)
            {
                ChouJiangSetting setting = DataTable.table.TbChouJiang.DataList[i];
                weightList.Add(setting.Weight.ToInt32());
                candidate.Add(setting.Item);
            }
            int index = CommonUtil.GetIndexByWeight(weightList);
            resStr = candidate[index];
            ////天级弟子
            //if (resStr == "5")
            //{
            //    data.BaoDi50Num = 0;
            //    theRarity = 5;
            //}     //天级弟子
            //else if (resStr == "4")
            //{
            //    data.BaoDi10Num = 0;
            //    theRarity = 4;
            //}
 
        }
        List<int> resList = CommonUtil.SplitCfgOneDepth(resStr);
        ItemData item = new ItemData();
        item.settingId = resList[0];
        item.count = (ulong)resList[1];
        if (item.settingId ==(int) ItemIdType.DiJiDiZiJieYinLing)
        {
            data.baoDi10Num = 0;

        }
        else if (item.settingId == (int)ItemIdType.TianJiDiZiJieYinLing)
        {
            data.baoDi50Num = 0;
            data.baoDi10Num = 0;
        }
        return item;
        ////抽到的物品
        //if (resList.Count > 0)
        //{

        //}
        //else
        //{

        //}
        //if (theRarity > 0)
        //{

        //    List<ItemSetting> candidate = DataTable.FindRarityTypeItemSetting(cfg.ItemType.SkillBook, skillRarity);
        //    ItemSetting choosedSetting = candidate[RandomManager.Next(0, candidate.Count)];
        //    itemId = choosedSetting.Id;
        //    itemNum = ConstantVal.StudySkillNeedNum((int)choosedSetting.Rarity);

        //    //if(baoDi)
        //    //itemNum = ConstantVal.StudySkillNeedNum((int)choosedSetting.Rarity);
        //    //else 
        //    //itemNum = 1;
        //}


        //ItemData item = new ItemData();
        //item.settingId = itemId;
        //item.num = itemNum;
        //return item;
    }
}

public class ChouJiangResData
{
    public Rarity resRarity;
    public List<ItemData> resList=new List<ItemData>();
 
}
public enum ChouJiangResType
{
    None=0,
    Student=1,
    Item=2,
}
public enum ChouJiangType
{
    None=0,
    XinWu=1,//信物
    Skill=2,//功法
}