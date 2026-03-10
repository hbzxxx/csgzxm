using cfg;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiliRevivePanel : PanelBase
{

 
    #region 源力结晶获得
    public Text txt_jieJingGet;
    public Transform trans_yuanLiJieJingParent;//源力结晶
    public Button btn_yuanLiJieJingGet;
    public Text txt_yuanLiJieJingRemain;//源力结晶剩余
    YuanLiJieJingItemView yuanLiJieJingItemView;
    #endregion

    #region 天晶购买
    public Image img_tianJingIcon;//天晶
    public Text txt_tianJingNeed;
    public Text txt_num;
    public Text txt_remain;
    public Text txt_tianJingRemain;//剩余天晶
    public Button btn;

    #endregion
    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn, () =>
         {
             if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum >= ConstantVal.dayliWatchTiliLimit)
             {
                 PanelManager.Instance.OpenFloatWindow("今天金手指获取体力次数已达上限，请明天再来哦");
                 return;
             }
             ShopManager.Instance.OnTianJingBuyTiLi();
             //ADManager.Instance.WatchAD(ADType.ReviveTiLi);
             PanelManager.Instance.ClosePanel(this);
         });
        addBtnListener(btn_yuanLiJieJingGet, () =>
        {
            yuanLiJieJingItemView.btn.onClick.Invoke();
        });
        RegisterEvent(TheEventType.LoseItem, OnLoseItem);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        InitShow();
    }
    void InitShow()
    {
        txt_num.SetText("X" + ConstantVal.tianJingReviveTiliNum);
        txt_remain.SetText("今日剩余次数（" + (ConstantVal.dayliWatchTiliLimit - RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum) + "/" + ConstantVal.dayliWatchTiliLimit + ")");
        ItemSetting tianJing = DataTable.FindItemSetting((int)ItemIdType.LingJing);
        img_tianJingIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + tianJing.UiName);
        txt_tianJingNeed.SetText(ConstantVal.ReviveTiLiNeedTianJing.ToString());
        txt_tianJingRemain.SetText("(剩余:" + ItemManager.Instance.FindItemCount((int)ItemIdType.LingJing)+")");


        ItemSetting yuanLiJieJingSetting = DataTable.FindItemSetting((int)ItemIdType.YuanLiJieJing);
        txt_yuanLiJieJingRemain.SetText("(剩余:" + ItemManager.Instance.FindItemCount((int)ItemIdType.YuanLiJieJing) + ")");
        txt_jieJingGet.SetText("X" + yuanLiJieJingSetting.Param);
        ClearCertainParentAllSingle<YuanLiJieJingItemView>(trans_yuanLiJieJingParent);
        List<ItemData> yuanLiJieJingList = ItemManager.Instance.FindItemListByType(ItemType.YuanLi);
        if (yuanLiJieJingList.Count > 0)
        {
            yuanLiJieJingItemView= AddSingle<YuanLiJieJingItemView>(trans_yuanLiJieJingParent, yuanLiJieJingList[0]);
        }
        else
        {
            ItemData item = new ItemData();
            item.settingId = (int)ItemIdType.YuanLiJieJing;
            yuanLiJieJingItemView= AddSingle<YuanLiJieJingItemView>(trans_yuanLiJieJingParent, item);

        }

    }
    void OnLoseItem(object[] args)
    {
        int id = (int)args[0];
        if (id == (int)ItemIdType.YuanLiJieJing)
        {
            InitShow();
        }
    }
}
