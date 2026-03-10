using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleLeiChongView : SingleViewBase
{
    public int leiChongIndex;
    public Text txt_label;
    public Image img_processBar;
    public Text txt_process;
    public Transform itemGrid;
    public Button btn_get;
    public GameObject btn_weiwc;
    public GameObject obj_unAccomplished;
    public GameObject obj_awardGot;

    public GameObject txt_linqu;
    LeiChongSetting setting;
    public override void Init(params object[] args)
    {
        base.Init(args);
        leiChongIndex = (int)args[0];
        setting = DataTable._leiChongList[leiChongIndex];

        addBtnListener(btn_get, () =>
         {
             ShopManager.Instance.GetLeiChongAward(leiChongIndex);
         });

        RegisterEvent(TheEventType.GetLeiChongAward, RefreshShow);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }

    void RefreshShow()
    {
        PanelManager.Instance.CloseAllSingle(itemGrid);
        txt_label.SetText("累计充值" + setting.Price + "元");
        img_processBar.fillAmount = RoleManager.Instance._CurGameInfo.allShopData.totalChargeNum / setting.Price.ToFloat();
        txt_process.SetText(RoleManager.Instance._CurGameInfo.allShopData.totalChargeNum + "/" + setting.Price);

        if (setting.Award == "") return;
        List<List<int>> award = CommonUtil.SplitCfg(setting.Award);
        for (int i = 0; i < award.Count; i++)
        {
            List<int> singleAward = award[i];
            ItemData item = new ItemData();
            item.settingId = singleAward[0];
            item.count = (ulong)singleAward[1];
            PanelManager.Instance.OpenSingle<WithCountItemViewLeichong>(itemGrid, item);
        }
        int accomplishStatus = RoleManager.Instance._CurGameInfo.allShopData.accomplishStatusList[leiChongIndex];
        if (accomplishStatus == (int)AccomplishStatus.UnAccomplished)
        {
            btn_get.gameObject.SetActive(false);
            obj_awardGot.SetActive(false);
            obj_unAccomplished.SetActive(true);
            btn_weiwc.SetActive(true);
        }
        else if (accomplishStatus == (int)AccomplishStatus.Accomplished)
        {
            btn_get.gameObject.SetActive(true);
            btn_get.enabled = true;
            obj_awardGot.SetActive(false);
            txt_linqu.SetActive(true);
            obj_unAccomplished.SetActive(false);
            btn_weiwc.SetActive(false);
        }
        else if (accomplishStatus == (int)AccomplishStatus.GetAward)
        {
            btn_get.gameObject.SetActive(true);
            btn_get.enabled = false;
            txt_linqu.SetActive(false);
            obj_awardGot.SetActive(true);
            obj_unAccomplished.SetActive(false);
            btn_weiwc.SetActive(false);
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(itemGrid);
    }
}
