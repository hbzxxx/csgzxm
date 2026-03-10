using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;

public class DanFarmUpgradePanel : PanelBase
{
    public Text txt_beforeUpgradeLevel;
    public Text txt_afterUpgradeLevel;
    public Text txt_beforeUpgradeIncome;
    public Text txt_afterUpgardeIncome;
    public Transform trans_afterUpgradeRent;
    public Text txt_beforeUpgradeRent;
    public Text txt_afterUpgradeRent;
    public Text txt_upgradeCost;
    public Transform trans_upgradeConsume;//消耗
    public Transform trans_consumeGrid;//消耗格子

    public SingleDanFarmData singleDanFarmData;

    public Button btn_upgrade;//升级
    public Text txt_needZongMenLevel;//需要宗门等级

    public override void Init(params object[] args)
    {
        base.Init(args);


        singleDanFarmData = args[0] as SingleDanFarmData;
        DanFarmSetting danFarmSetting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
        List<int> danPriceList = new List<int>();
        if (singleDanFarmData.DanFarmWorkType ==(int) DanFarmWorkType.Common)
        {
            danPriceList = CommonUtil.SplitCfgOneDepth(danFarmSetting.DanPrice);
        }
      
        List<int> upgradeCostList = CommonUtil.SplitCfgOneDepth(danFarmSetting.UpgradeCost);
        List<int> upgradeRentList = CommonUtil.SplitCfgOneDepth(danFarmSetting.UpgradeRent);

        //升级前
        txt_beforeUpgradeLevel.SetText("Lv." + singleDanFarmData.CurLevel);
        txt_afterUpgradeLevel.SetText("Lv." + (singleDanFarmData.CurLevel + 1));

        int needZongMenLevel=ZongMenManager.Instance.FarmNextLevelNeedZongMenLevel(singleDanFarmData);
        //需要宗门等级
        if (needZongMenLevel != -1)
        {
            txt_afterUpgradeLevel.gameObject.SetActive(true);

            if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel < needZongMenLevel)
            {
                txt_needZongMenLevel.gameObject.SetActive(true);
                txt_needZongMenLevel.SetText("需要" + (needZongMenLevel) + "级宗门");
                trans_upgradeConsume.gameObject.SetActive(false);
            }
            else
            {
                trans_upgradeConsume.gameObject.SetActive(true);
                txt_needZongMenLevel.gameObject.SetActive(false);

                //显示额外需求材料
                ClearCertainParentAllSingle<SingleConsumeView>(trans_consumeGrid);
                if (!string.IsNullOrWhiteSpace(danFarmSetting.UpgradeNeedOtherMat))
                {
                    List<List<int>> otherConsume = CommonUtil.SplitCfg(danFarmSetting.UpgradeNeedOtherMat);
                    List<int> theConsume = otherConsume[singleDanFarmData.CurLevel - 1];
                    if (theConsume.Count > 1)
                    {
                        AddSingle<SingleConsumeView>(trans_consumeGrid, theConsume[0], theConsume[1], ConsumeType.Item);
                    }
                }
         
                AddSingle<SingleConsumeView>(trans_consumeGrid, (int)ItemIdType.LingShi, upgradeCostList[singleDanFarmData.CurLevel - 1], ConsumeType.Item);

           

            }
            if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
            {
                txt_afterUpgardeIncome.SetText("X" + danPriceList[singleDanFarmData.CurLevel] * 10 + "/月");
                txt_afterUpgardeIncome.gameObject.SetActive(true);
            }
            else
            {
                txt_afterUpgardeIncome.gameObject.SetActive(false);

            }

            txt_afterUpgradeRent.SetText("X" + upgradeRentList[singleDanFarmData.CurLevel] + "/月");


            txt_afterUpgradeRent.gameObject.SetActive(true);
         }
        else
        {
            txt_needZongMenLevel.gameObject.SetActive(true);

            txt_needZongMenLevel.SetText("当前已达最大等级");
            trans_upgradeConsume.gameObject.SetActive(false);
            txt_afterUpgradeLevel.gameObject.SetActive(false);
            txt_afterUpgradeRent.gameObject.SetActive(false);
            txt_afterUpgardeIncome.gameObject.SetActive(false);
        }


        if (singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
        {
            txt_beforeUpgradeIncome.SetText("X" + danPriceList[singleDanFarmData.CurLevel - 1] * 10 + "/月");
        }
        else
        {
            txt_beforeUpgradeIncome.gameObject.SetActive(false);
        }
   

        txt_beforeUpgradeRent.SetText("X" + upgradeRentList[singleDanFarmData.CurLevel - 1] + "/月");
   


        addBtnListener(btn_upgrade, () =>
         {
             LianDanManager.Instance.DanFarmUpgrade(singleDanFarmData);
             PanelManager.Instance.ClosePanel(this);
         });

            //升级
            //放弟子
          if (TaskManager.Instance.danFarmUpgrade
            && TaskManager.Instance.danFarmUpgradeOnlyId == singleDanFarmData.OnlyId)
        {
            PanelManager.Instance.ShowTaskGuidePanel(btn_upgrade.gameObject);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.danFarmUpgrade = false;

    }


}
