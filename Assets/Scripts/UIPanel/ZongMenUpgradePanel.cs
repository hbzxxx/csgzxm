using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
/// <summary>
/// 宗门升级面板
/// </summary>
public class ZongMenUpgradePanel : PanelBase
{
    public Text txt_lvBeforeUpgrade;
    public Text txt_lvAfterUpgrade;

    public Transform trans_consumeGrid;
    public Transform trans_desGrid;
    public Button btn_upgrade;
    ZongMenUpgradeSetting curSetting;

    ZongMenUpgradeSetting nextSetting;
    public Image img_icon;

    public Text txt_rent;
    
    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_upgrade, () =>
        {
            ZongMenManager.Instance.UpgradeZongMen();
            PanelManager.Instance.ClosePanel(this);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        img_icon.gameObject.SetActive(true);
        txt_rent.gameObject.SetActive(true);

        int curLevel = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel;
        txt_lvBeforeUpgrade.SetText("Lv."+ curLevel);
        int maxDataLevel = DataTable._zongMenUpgradeList.Count;
        curSetting = DataTable._zongMenUpgradeList[curLevel - 1];

        if (curLevel >= maxDataLevel) curLevel = maxDataLevel - 1;
        if (curLevel < maxDataLevel)
        {
            nextSetting = DataTable._zongMenUpgradeList[curLevel];
            txt_lvAfterUpgrade.gameObject.SetActive(true);
            txt_lvAfterUpgrade.SetText("Lv."+(curLevel + 1).ToString());
            List<string> desList = CommonUtil.SplitCfgStringOneDepth(nextSetting.UnlockDes);
            for(int i = 0; i < desList.Count; i++)
            {
                string theStr = desList[i];
                if(!string.IsNullOrWhiteSpace(theStr))
                AddSingle<UpgradeDesTxtView>(trans_desGrid, desList[i]);
            }
            trans_consumeGrid.gameObject.SetActive(true);

            AddSingle<SingleConsumeView>(trans_consumeGrid,(int)ItemIdType.LingShi, curSetting.Cost.ToInt32(),ConsumeType.Item);
            if (!string.IsNullOrWhiteSpace(curSetting.SpecialMatNeed))
            {
                List<List<int>> theConsume = CommonUtil.SplitCfg(curSetting.SpecialMatNeed);
                for (int i = 0; i < theConsume.Count; i++)
                {
                    List<int> singleConsume = theConsume[i];
                    if (singleConsume.Count == 2)
                    {
                        AddSingle<SingleConsumeView>(trans_consumeGrid, singleConsume[0], singleConsume[1], ConsumeType.Item);

                    }
                }
             }

            //txt_rent.SetText("每月" + LanguageUtil.GetLanguageText((int)LanguageIdType.灵石) + "消耗：-" + nextSetting.Rent);
            img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "lingshi");
            txt_rent.SetText(nextSetting.Rent + "/月");
        }
        else
        {
            img_icon.gameObject.SetActive(false);
            txt_rent.gameObject.SetActive(false);
            trans_consumeGrid.gameObject.SetActive(false);
            txt_lvAfterUpgrade.gameObject.SetActive(false);
        }


    }
}
