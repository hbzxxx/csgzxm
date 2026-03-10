using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using Framework.Data;
using cfg;

public class BuildingManager : CommonInstance<BuildingManager>
{
    public MountainMode curMode;
    public bool onlyMove;//是建造还是只移动
    public override void Init()
    {
        base.Init();
        curMode = MountainMode.Common;
    }

    /// <summary>
    /// 进入建造的建造模式
    /// </summary>
    /// <param name="settingId"></param>
    public void EnterBuildingBuildingMove(int settingId)
    {
        onlyMove = false;
        curMode = MountainMode.Building;
        PanelManager.Instance.OpenPanel<BuildingModePanel>(PanelManager.Instance.trans_layer2, false, settingId);
        EventCenter.Broadcast(TheEventType.EnterBuildingMode, settingId);

    }

    /// <summary>
    /// 进入只移动的建造模式
    /// </summary>
    public void EnterOnlyMovingBuildingMode(SingleDanFarmData farmData)
    {
        curMode = MountainMode.Building;
        onlyMove = true;
        PanelManager.Instance.OpenPanel<BuildingModePanel>(PanelManager.Instance.trans_layer2,true, farmData);
        EventCenter.Broadcast(TheEventType.EnterOnlyMoveBuildingMode, farmData);
    }

    /// <summary>
    /// 离开建造模式
    /// </summary>
    public void QuitBuildingMode()
    {
        EventCenter.Broadcast(TheEventType.QuitBuildingMode);
        curMode = MountainMode.Common;
        PanelManager.Instance.OpenPanel<MainPanel>(PanelManager.Instance.trans_layer2);
        PanelManager.Instance.OpenPanel<TopPanel>(PanelManager.Instance.trans_layer2);

    }
    /// <summary>
    /// 升级
    /// </summary>
    public void Upgrade(int settingId)
    {
        int count = RoleManager.Instance._CurGameInfo.AllBuildingData.BuildList.Count;
        for (int i = 0; i < count; i++)
        {
            SingleBuildingData data = RoleManager.Instance._CurGameInfo.AllBuildingData.BuildList[i];
            if (data.SettingId == settingId)
            {
                int maxLevel = 0;
                //int limitLevel = 0;//等级限制条件
                ulong needMoney = 0;

                switch ((BuildingIdType)settingId)
                {
                    case BuildingIdType.EquipMake:
                        maxLevel = DataTable._equipBuildingUpgradeList.Count;
                        break;
                    case BuildingIdType.LianDanFang:
                        maxLevel = DataTable._lianDanBuildingUpgradeList.Count;
                        break;
                }
                if (data.CurBuildLevel >= maxLevel)
                {
                    PanelManager.Instance.OpenFloatWindow("已达到最大等级");
                    return;
                }
                switch ((BuildingIdType)settingId)
                {
                    case BuildingIdType.EquipMake:
                        needMoney = DataTable._equipBuildingUpgradeList[data.CurBuildLevel].NeedMoney.ToUInt64();
                        break;
                }
                //ulong needMoney = DataTable._buildingUpgradeList[data.CurBuildLevel].needMoney.ToUInt64();
                if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingShi, (ulong)needMoney))
                {
                         ItemSetting itemSetting= DataTable.table.TbItem.Get(((int)ItemIdType.LingShi).ToString());
                        PanelManager.Instance.OpenFloatWindow(itemSetting.Name+ "不够");
                    return;
                }

                ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, needMoney);
                data.CurBuildLevel++;
                data.MaxStudentNum = DataTable._buildingUpgradeList[data.CurBuildLevel-1].MaxStudent.ToInt32();

                switch ((BuildingIdType)settingId)
                {
                    //case BuildingIdType.EquipMake:
                    //    OnEquipMakeBuildingUpgrade(data);
                    //    break;
                    case BuildingIdType.LianDanFang:
                        OnLianDanBuildingUpgrade(data);
                        break;
                }
                PanelManager.Instance.OpenFloatWindow("升级成功！");
                EventCenter.Broadcast(TheEventType.BuildingUpgrade, data);
                break;
            }
        }
    }

    /// <summary>
    /// 炼丹房升级
    /// </summary>
    /// <param name="curLevel"></param>
    public void OnLianDanBuildingUpgrade(SingleBuildingData data)
    {
        //int curLevel = data.CurBuildLevel;

        //data.MaxNeiMenStudentNumLimit = DataTable._lianDanBuildingUpgradeList[data.CurBuildLevel - 1].unlockedStudentCount.ToInt32();

        //LianDanBuildingUpgradeSetting setting = DataTable._lianDanBuildingUpgradeList[curLevel - 1];
        //int id = RoleManager.Instance._CurGameInfo.LianDanData.CurValidMoneyDanSettingIdList[0];

        //RoleManager.Instance._CurGameInfo.LianDanData.CurValidMoneyDanSettingIdList[0] = setting.moneyDan.ToInt32();
        //RoleManager.Instance._CurGameInfo.LianDanData.MaxGemRarity = setting.unlockedGemRarity.ToInt32();

    }


    /// <summary>
    /// 通过settingid找建筑
    /// </summary>
    /// <returns></returns>
    public SingleBuildingData FindBuildingDataBySettingId(int settingId)
    {
        if (RoleManager.Instance._CurGameInfo.AllBuildingData != null)
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllBuildingData.BuildList.Count; i++)
            {
                SingleBuildingData singleData = RoleManager.Instance._CurGameInfo.AllBuildingData.BuildList[i];
                if (singleData.SettingId == settingId)
                {
                    return singleData;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 是否达到弟子最大限制
    /// </summary>
    /// <returns></returns>
    public bool CheckIfReachBuildingMaxNeiMenNumLimit()
    {
        int curNum = RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count;
        //SingleBuildingData singleData = null;
        //switch (studentType)
        //{
        //    case StudentType.LianDan:
        //         curNum = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count;
        //         singleData = FindBuildingDataBySettingId((int)BuildingIdType.LianDanFang);
        //        break;
        //    case StudentType.EquipMake:
        //         curNum = RoleManager.Instance._CurGameInfo.StudentData.LianDanStudentList.Count;
        //         singleData = FindBuildingDataBySettingId((int)BuildingIdType.EquipMake);
        //        break;
        //    case StudentType.LianGong:
        //        curNum = RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList.Count;
        //        singleData = FindBuildingDataBySettingId((int)BuildingIdType.StudentTrain);
        //        break;
        //}
    
        if (curNum >= RoleManager.Instance._CurGameInfo.studentData.MaxStudentNum)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public SingleDanFarmData FindDanFarmDataByOnlyId(ulong onlyId)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.OnlyId == onlyId)
                return data;

        }
        return null;
    }
}


public enum BuildType
{
    None=0,
    LianDanFang=1,//炼丹房
    EquipMake=2,//炼器房
}
public enum BuildingIdType
{
    None=0,
    LianDanFang=10001,//炼丹房
    EquipMake=10002,//炼器房
}

public enum MountainMode
{
    None=0,
    Building=1,
    Common=2,
}