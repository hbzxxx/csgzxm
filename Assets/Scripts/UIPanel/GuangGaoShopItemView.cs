using System.Collections;
using System.Collections.Generic;
using Framework.Data;
using UnityEngine;

public class GuangGaoShopItemView : ShopItemView
{
    public Transform trans_guangGaoShopGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    public override void RefreshShopItemShow()
    {
        base.RefreshShopItemShow();
        PanelManager.Instance.CloseAllSingle(trans_guangGaoShopGrid);

        List<List<int>> award = CommonUtil.SplitCfg(shopSetting.Param);
        for (int i = 0; i < award.Count; i++)
        {
            List<int> single = award[i];
            ItemData item = new ItemData();
            item.settingId = single[0];
            item.count = (ulong)single[1];
            if(DataTable.FindItemSetting(item.settingId)!=null)//��Ʒ��Ϊ��
                PanelManager.Instance.OpenSingle<WithCountItemView>(trans_guangGaoShopGrid, item);
        }



    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_guangGaoShopGrid);
    }
}
