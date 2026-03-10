using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainDanItemView : ItemView
{
    public Text txt_count;
    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    public override void RefreshShow()
    {
        base.RefreshShow();
        txt_count.SetText(itemData.count.ToString());

        addBtnListener(btn, () =>
        {
            //如果我没有，则弹窗提示
            if (ItemManager.Instance.CheckIfItemEnough(itemData.settingId, 1))
            {
                PanelManager.Instance.OpenFloatWindow("缺少" + setting.Name);
                return;
            }
            else
            {
                PanelManager.Instance.OpenCommonHint("服用" + setting.Name + "以增加5%突破成功率",()=> 
                { 

                },null);

            }
        });
    }
}
