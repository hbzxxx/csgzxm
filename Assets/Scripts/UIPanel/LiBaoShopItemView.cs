
using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiBaoShopItemView : ShopItemView
{
    public Text txt_remain;
    public Transform trans_awardGrid;
    public Sprite img_box;
    public override void RefreshShopItemShow()
    {
        base.RefreshShopItemShow();
        if (shopItemData.RemainCount <= 0
            &&shopSetting.Type.ToInt32()==(int)ShopType.XinShouLiBao)
        {
            PanelManager.Instance.CloseSingle(this);
            return;
        }
        PanelManager.Instance.CloseAllSingle(trans_awardGrid);

        List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
        for(int i = 0; i < award.Count; i++)
        {
            List<int> single = award[i];
            ItemData item = new ItemData();
            item.settingId = single[0];
            item.count = (ulong)single[1];
            ShopSetting setting = DataTable.FindShopSetting(item.settingId);
            if(setting!=null)
                PanelManager.Instance.OpenSingle<WithCountItemView>(trans_awardGrid,item);
        }
        ShowRemain();
    }
    void ShowRemain()
    {
        if (shopSetting.Type.ToInt32() ==(int)ShopType.XinShouLiBao)
        {
            txt_remain.SetText("限一次");
        }
        else
        {
            txt_remain.SetText(shopItemData.RemainCount + "/" + shopSetting.MaxCount);
        }
    }
    public override void ShowIcon()
    {
        //img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ShopIconPath + shopSetting.Icon);
        img_icon.sprite = img_box;
    }
    public override void ShowPrice()
    {
        string res = "";
        if (!string.IsNullOrWhiteSpace(shopSetting.Rmb))
        {
            img_consumeIcon.gameObject.SetActive(false);
            int num = shopSetting.Price.ToInt32();
            if (num <= 0)
            {
                res = "免费";
            }
            else
            {
                res = num + "元";
            }
        }
        else
        {
            img_consumeIcon.gameObject.SetActive(true);
            img_consumeIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + consumeSetting.UiName);
            res=singleConsumeNum.ToString();
        }

        txt_consume.SetText(res);

    }
    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_awardGrid);

    }

}
