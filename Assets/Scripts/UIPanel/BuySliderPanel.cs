using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class BuySliderPanel : ChooseItemSliderPanel
{
    public ShopItemData shopItemData;
    public ShopSetting shopSetting;

    public ItemData singlePriceItem;
    public ItemSetting singlePriceItemSetting;

    public Image img_priceIcon;
    public Text txt_consume;
 
    public override void Init(params object[] args)
    {
        base.Init(args);
        singlePriceItem = args[2] as ItemData;
        singlePriceItemSetting = DataTable.FindItemSetting(singlePriceItem.settingId);
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        img_priceIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + singlePriceItemSetting.UiName);
    }
    public override void OnChoosedNum(int num)
    {
        base.OnChoosedNum(num);
        txt_consume.SetText("-" + (int)(ulong)singlePriceItem.count * num);
  

    }
 
}
