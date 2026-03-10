using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class ItemShowView : SingleViewBase
{
    public ItemData itemData;
    public Image icon;
    public Text txt_num;

    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ItemSetting setting = DataTable.FindItemSetting(itemData.settingId);
        icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + setting.UiName);
        txt_num.SetText(itemData.count.ToString());
    }
}
