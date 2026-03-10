using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
public class YuanLiJieJingItemView : ItemView
{
    public Text txt_count;
 
    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn, () =>
        {
            if (itemData.count > 0)
            {
                ItemManager.Instance.LoseItem(itemData.settingId, 1);
                RoleManager.Instance.AddProperty(PropertyIdType.Tili, setting.Param.ToInt32());
             }

        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_count.SetText(itemData.count.ToString());
        if (itemData.count == 0)
        {
            img_bottom.material = PanelManager.Instance.mat_grey;
            img_icon.material = PanelManager.Instance.mat_grey;
        }
        else
        {
            img_bottom.material = null;
            img_icon.material = null;
        }
    }
}
