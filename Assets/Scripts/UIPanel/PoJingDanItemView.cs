using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoJingDanItemView : ItemView
{
    public Text txt_count;

    StudentHandlePanel studentHandlePanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        studentHandlePanel = args[1] as StudentHandlePanel;
        addBtnListener(btn, () =>
        {
             if (itemData.count <= 0)
             {
                 PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData);
             }
             else
             {
                 if (itemData.setting.ItemType.ToInt32() ==(int) ItemType.PoJingDan)
                 {
                     RoleManager.Instance.UseBreakDan(studentHandlePanel.curChoosedP, itemData);

                 }
                 else
                 {
                     PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData);
                 }

             }
        });

        RegisterEvent(TheEventType.UsedBreakDan, OnUseBreakDan);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        
        txt_count.SetText(itemData.count.ToString());
        ulong myNum = ItemManager.Instance.FindItemCount(itemData.settingId);
        if (myNum >= itemData.count)
        {
            txt_count.color = new Color32(0, 99, 0, 255);
        }
        else
        {
            txt_count.color = Color.red;
        }
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

    void OnUseBreakDan()
    {
        txt_count.SetText(itemData.count.ToString());
    }
}
