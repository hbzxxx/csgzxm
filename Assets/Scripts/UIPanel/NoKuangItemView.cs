using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoKuangItemView : ItemView
{
    public Text txt_count;
    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    public override void RefreshShow()
    {
        base.RefreshShow();
        if (itemData.equipProtoData == null)
            txt_count.SetText("X"+UIUtil.ShowBigCount((long)(ulong)itemData.count));
        else
            txt_count.SetText("Lv." + itemData.equipProtoData.curLevel);

        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData,true);
        });
    }
}
