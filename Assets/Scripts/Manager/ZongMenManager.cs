using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class ZongMenManager : CommonInstance<ZongMenManager>
{
  

    /// <summary>
    /// 获取建筑的数量限制
    /// </summary>
    /// <param name="danFarmId"></param>
    public int GetFarmNumLimit(int danFarmId)
    {
        int limit = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId[i];
            if (theId == danFarmId)
                limit++;
        }
        //TODO测试用

        return limit;
    }


    /// <summary>
    /// 当前最多升级到几级宗门
    /// </summary>
    /// <returns></returns>
    public int CurMaxZongmenLevel()
    {
        return DataTable._zongMenUpgradeList.Count;
    }

    /// <summary>
    /// 宗门升级
    /// </summary>
    public void UpgradeZongMen()
    {
        int curLevel = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
        int maxLevel = CurMaxZongmenLevel();
        if (curLevel < maxLevel)
        {
            ZongMenUpgradeSetting curSetting = DataTable._zongMenUpgradeList[curLevel - 1];

            int needItem = curSetting.Cost.ToInt32();

            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)needItem))
            {
                ItemSetting itemSetting = DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
                PanelManager.Instance.OpenFloatWindow(itemSetting.Name + "不够");
                return;
            }
            if (!string.IsNullOrWhiteSpace(curSetting.SpecialMatNeed))
            {
                List<List<int>> theConsume = CommonUtil.SplitCfg(curSetting.SpecialMatNeed);
                for (int i = 0; i < theConsume.Count; i++)
                {
                    List<int> singleConsume = theConsume[i];
                    if (singleConsume.Count == 2)
                    {
                        if (!ItemManager.Instance.CheckIfItemEnough(singleConsume[0], (ulong)singleConsume[1]))
                        {
                            ItemSetting theSetting = DataTable.FindItemSetting(singleConsume[0]);
                            PanelManager.Instance.OpenFloatWindow(theSetting.Name + "不够");
                            return;
                        }
                    }

                }

            }
            if (!string.IsNullOrWhiteSpace(curSetting.SpecialMatNeed))
            {
                List<List<int>> theConsume = CommonUtil.SplitCfg(curSetting.SpecialMatNeed);
                for (int i = 0; i < theConsume.Count; i++)
                {
                    List<int> singleConsume = theConsume[i];
                    if (singleConsume.Count == 2)
                    {
                        ItemManager.Instance.LoseItem(singleConsume[0], (ulong)singleConsume[1]);

                    }

                }

            }


            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)needItem);

            RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel++;
            ZongMenUpgradeSetting afterSetting = DataTable._zongMenUpgradeList[curLevel];
            //升级效果
            List<List<int>> beforeBuilding = CommonUtil.SplitCfg(curSetting.UnlockedBuilding);
            List<List<int>> afterBuilding = CommonUtil.SplitCfg(afterSetting.UnlockedBuilding);

            //List<int> beforeNum=
            //如果新的，则解锁 如果非新 则增加上限
            for (int i = 0; i < beforeBuilding.Count; i++)
            {
                List<int> before = beforeBuilding[i];
                List<int> after = afterBuilding[i];

                int buildId = before[0];
                int beforeNum = before[1];
                int afterNum = after[1];
                DanFarmSetting setting = DataTable.FindDanFarmSetting(buildId);
                //看量对不对
                int existedNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Count; j++)
                {
                    int id = RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId[j];
                    if (id == buildId)
                    {
                        existedNum++;
                    }
                }
                while (afterNum > existedNum)
                {
                    //解锁了新建筑
                    if (beforeNum == 0)
                    {
                        PanelManager.Instance.AddTongZhi(TongZhiType.Common, "解锁新建筑：" + setting.Name);
                    }
                    LianDanManager.Instance.UnlockDanFarm(buildId);
                    existedNum++;
                }
            }
            //空地数量
            int farmNumBefore = curSetting.FarmNumLimit.ToInt32();
            int farmNumAfter = afterSetting.FarmNumLimit.ToInt32();
            if (farmNumAfter > farmNumBefore)
            {
                PanelManager.Instance.AddTongZhi(TongZhiType.Common, "有新的空地可以解锁了");

                RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit = farmNumAfter + RoleManager.Instance._CurGameInfo.allZongMenData.SendFarmNumLimitAddNum;
                EventCenter.Broadcast(TheEventType.ShowUnlockFarmPosStatus);
            }
            //体力上限
            int tiliLimitBefore = curSetting.TiliLimit.ToInt32();
            int tiliLimitAfter = afterSetting.TiliLimit.ToInt32();
            if (tiliLimitAfter - tiliLimitBefore > 0)
            {
                PanelManager.Instance.AddTongZhi(TongZhiType.Common, "体力上限增加" + tiliLimitBefore + "——" + tiliLimitAfter);
                RoleManager.Instance.AddTiLiLimit(tiliLimitAfter);
            }
            RoleManager.Instance.FullTiLi();
            PanelManager.Instance.AddTongZhi(TongZhiType.Common, "体力已回复满" + tiliLimitBefore + "——" + tiliLimitAfter);



            RoleManager.Instance._CurGameInfo.studentData.MaxStudentNum = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel * 3;
            EventCenter.Broadcast(TheEventType.ZongMenLevelUpgrade);
            PanelManager.Instance.AddTongZhi(TongZhiType.Common, "等级提升为Lv" + RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel);
            RoleManager.Instance.profile.SetLevel(RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel);
            TaskManager.Instance.TryAccomplishGuideBook(TaskType.UpgradeZongMen);

            if (Game.Instance.isLogin)
            {
#if !UNITY_EDITOR
                AddQQManager.Instance.CallAndroidMethod("OnSendRoleData", "levelUp", RoleManager.Instance._CurGameInfo.TheGuid, RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName,
                RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel,(Game.Instance.curServerIndex - 2).ToString(), M_ServerData.fuNameList[Game.Instance.curServerIndex], (int)ItemManager.Instance.FindItemCount((int)ItemIdType.TianJing));
#endif
            }
        }
        else {
            PanelManager.Instance.OpenFloatWindow("已经达到最大等级");
        }
    }
    /// <summary>
    /// 总建筑上限
    /// </summary>
    /// <returns></returns>
   
    /// <summary>
    /// 解锁一个丹房位置
    /// </summary>
    public void UnlockDanFarmPos()
    {
        if (JudgeIfCanUnlockFarm())
        {
            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum);

            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum++;
            if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum > 3)
            {
                RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum = (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum - 3) * 500;
            }
            EventCenter.Broadcast(TheEventType.UnlockDanFarm);

        }


    }

    /// <summary>
    /// 找所有解锁了的建筑 去重
    /// </summary>
    /// <returns></returns>
    public List<int> FindAllUnlockedFarmIdList()
    {
        List<int> res = new List<int>();
        if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Contains(0))
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Remove(0);
        for (int i=0;i< RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId[i];
            if (!res.Contains(theId))
            {
                res.Add(theId);
            }
        }
        return res;
    }


    /// <summary>
    /// 判断有没有某类房
    /// </summary>
    /// <returns></returns>
    public bool JudgeIfHaveTypeFarm(DanFarmType danFarmType)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (!data.IsEmpty
                && data.DanFarmType == (int)danFarmType)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 找所有该类房
    /// </summary>
    /// <returns></returns>
    public List<SingleDanFarmData> FindTypeFarmList(DanFarmType danFarmType)
    {
        List<SingleDanFarmData> farmList = new List<SingleDanFarmData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (!data.IsEmpty
                && data.DanFarmType == (int)danFarmType)
                farmList.Add(data);
        }
        return farmList;
    }

    /// <summary>
    /// 随便找一个解锁了的空田
    /// </summary>
    /// <returns></returns>
    public SingleDanFarmData FindUnlockedEmptyFarm()
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (singleDanFarmData.IsEmpty && singleDanFarmData.Unlocked)
            {
                return singleDanFarmData;
            }


        }
        return null;
    }


    /// <summary>
    /// 房间升级需要宗门等级多少
    /// </summary>
    /// <returns></returns>
    public int FarmNextLevelNeedZongMenLevel(SingleDanFarmData singleDanFarmData)
    {
        bool isSpecialLimit = false;
        int specialIndex = 0;
        //先判断是不是特殊房间
        ZongMenUpgradeSetting firstSetting = DataTable._zongMenUpgradeList[0];
        List<List<int>> firstContent = CommonUtil.SplitCfg(firstSetting.SpecialFarmLevelLimit);
        for (int j = 0; j < firstContent.Count; j++)
        {
            List<int> single = firstContent[j];
            if (single[0] == singleDanFarmData.SettingId)
            {
                isSpecialLimit = true;
                specialIndex = j;
            }
        }

        if (isSpecialLimit)
        {
            int curLevel = singleDanFarmData.CurLevel;
            ZongMenUpgradeSetting curSetting = DataTable._zongMenUpgradeList[curLevel - 1];

            List<List<int>> curContent = CommonUtil.SplitCfg(curSetting.SpecialFarmLevelLimit);
            List<int> single = curContent[specialIndex];


            int curLimit = single[1];
            //当前等级+1还小于限制
            if (singleDanFarmData.CurLevel + 1 <= curLimit)
            {
                return RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
            }
            //当前等级+1大于限制 则往后找
            else
            {
                for (int m = curLevel - 1; m < DataTable._zongMenUpgradeList.Count; m++)
                {
                    ZongMenUpgradeSetting futureSetting = DataTable._zongMenUpgradeList[m];
                    if (futureSetting.SpecialFarmLevelLimit != null)
                    {
                        List<List<int>> futureContent = CommonUtil.SplitCfg(futureSetting.SpecialFarmLevelLimit);


                        List<int> futureSingle = futureContent[specialIndex];

                        //找到了
                        if (singleDanFarmData.CurLevel + 1 <= futureSingle[1])
                        {
                            return m + 1;
                        }


                    }



                }
                return -1;//当前已达最大等级
                //如果都没找到
            }


            // int curLimit = DataTable._zongMenUpgradeList[singleDanFarmData.CurLevel - 1].;
        }
        else
        {
            if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
            {
                List<int> danPriceList = new List<int>();

                DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
                danPriceList = CommonUtil.SplitCfgOneDepth(danFarmSetting.DanPrice);
                if ( singleDanFarmData.CurLevel < danPriceList.Count)
                   return  RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
            }
            return -1;
        }




    }

    /// <summary>
    /// 临时宗门
    /// </summary>
    public void SetInitZongMenName()
    {
        RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName = "临时宗门";
    }

    /// <summary>
    /// 随机宗门名
    /// </summary>
    /// <returns></returns>
    public string RdmZongMenName()
    {
        string name = "";
        List<string> foreNameList = new List<string>();
        int count = DataTable._rdmZongMenNameList.Count;
        List<string> backNameList = new List<string>();

        for (int i = 0; i < count; i++)
        {
            string foreName = DataTable._rdmZongMenNameList[i].ForeName;
            string backName = DataTable._rdmZongMenNameList[i].BackName;

            if (!string.IsNullOrWhiteSpace(foreName))
            {
                foreNameList.Add(foreName);
            }
            if (!string.IsNullOrWhiteSpace(backName))
            {
                backNameList.Add(backName);
            }
        }
        int index1 = RandomManager.Next(0, foreNameList.Count);
        int index2 = RandomManager.Next(0, backNameList.Count);
        name = foreNameList[index1] + backNameList[index2];
        return name;
    }

    /// <summary>
    /// 判断是否可以解锁房间
    /// </summary>
    public bool JudgeIfCanUnlockFarm()
    {
       
        if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit > RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum)
        {
            if (ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 手动解锁丹房位置
    /// </summary>
    public void UnlockFarmPosUseUseHand(int index)
    {
        if (JudgeIfCanUnlockFarm())
        {
            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum);
            UnlockDanFarmPos();
        }
    }
    /// <summary>
    /// 送建筑
    /// </summary>
    public void SendBuilding(int buildId)
    {
        if (!RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Contains(buildId))
        {
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmId.Add(buildId);

            ZongMenUpgradeSetting curSetting = DataTable._zongMenUpgradeList[RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel - 1];
            RoleManager.Instance._CurGameInfo.allZongMenData.SendFarmNumLimitAddNum++;
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit = curSetting.FarmNumLimit.ToInt32() + RoleManager.Instance._CurGameInfo.allZongMenData.SendFarmNumLimitAddNum;
            UnlockDanFarmPos();
        }

    }

    public void ChangeName(string newName)
    {
        bool needItem = false;
        if (RoleManager.Instance._CurGameInfo.allZongMenData.ChangeNameNum >= 1)
        {
            needItem = true;
        }
        if (needItem)
        {
            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.ChangeZongMenNameCard, 1))
            {
                PanelManager.Instance.OpenFloatWindow("改名卡不够");
                return;
            }
        }
        if (DataTable.IsScreening(newName))
        {
            PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
            return;
        }
        if (string.IsNullOrWhiteSpace(newName))
        {
            PanelManager.Instance.OpenFloatWindow("名字不能为空");
            return;
        }
        //只有第二次及以后改名才扣道具
        if (needItem)
        {
            ItemManager.Instance.LoseItem((int)ItemIdType.ChangeZongMenNameCard, 1);
        }

        RoleManager.Instance._CurGameInfo.allZongMenData.ChangeNameNum++;
        RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName = newName;
        PanelManager.Instance.OpenFloatWindow("修改成功");
        EventCenter.Broadcast(TheEventType.ChangeZongMenName);
         TaskManager.Instance.TryAccomplishAllTask();

    }

    /// <summary>
    /// 宗门建设度
    /// </summary>
    public int ZongMenBuildRankNum()
    {
        int res = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
        for(int i=0;i< RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData farmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            res += farmData.CurLevel;
        }
        return res;
    }
}
