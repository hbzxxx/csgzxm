using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

/// <summary>
/// 租金管理
/// </summary>
public class RentManager : CommonInstance<RentManager>
{
    /// <summary>
    /// 消耗租金
    /// </summary>
    public void RentConsume()
    {

        int allConsume = CalcRentConsume()*12;

        ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)allConsume);
        ItemSetting setting= DataTable.FindItemSetting((int)ItemIdType.LingShi);
        RecordManager.Instance.AddTongZhi(setting.Name+ "支出" + "<color=#ff0000ff><b>" + -allConsume + "</b></color>");
        PanelManager.Instance.OpenOnlyOkHint("扣除本年度" +setting.Name + "<color=#ff0000ff><b>" + -allConsume + "</b></color>。", null);

        if (ItemManager.Instance.FindLingShiCount() < 0)
        {
            PanelManager.Instance.OpenPanel<PoChanShowPanel>(PanelManager.Instance.trans_layer2);
    
        }
    }

    /// <summary>
    /// 年度灵石产出
    /// </summary>
    /// <returns></returns>
    public int CalcLingShiProductPerMonth()
    {
        int res = 0;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (!singleDanFarmData.IsEmpty 
                && singleDanFarmData.Status == (int)DanFarmStatusType.Working
                &&singleDanFarmData.ProductSettingId==(int)ItemIdType.LingShi)
            {
                res+=LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData);
            }
        }
        return res;
    }


    /// <summary>
    /// 计算支出
    /// </summary>
    /// <returns></returns>
    public int CalcRentConsume()
    {
        int allConsume = 0;
        int maincityConsume = 0;
        allConsume += maincityConsume;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
            if (!singleDanFarmData.IsEmpty && singleDanFarmData.Status != (int)DanFarmStatusType.Building)
            {
                if (singleDanFarmData.SettingId == 70001) continue;
                DanFarmSetting setting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
                List<int> rentList = CommonUtil.SplitCfgOneDepth(setting.UpgradeRent);

                int curRent = rentList[singleDanFarmData.CurLevel - 1];
                allConsume += curRent;
            }
        }
        ZongMenUpgradeSetting zongMenUpgradeSetting= DataTable._zongMenUpgradeList[RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel - 1];
        allConsume += zongMenUpgradeSetting.Rent.ToInt32();
        //if (ItemManager.Instance.FindItemCount((int)ItemIdType.LingShi) < (ulong)allConsume)
        //{
        //    PanelManager.Instance.OpenOnlyOkHint("您破产了，看广告复活", null);
        //    allConsume = (int)ItemManager.Instance.FindItemCount((int)ItemIdType.LingShi);
        //}
        return allConsume;
    }
}
