
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 充值物品
/// </summary>
public class ChongZhiShopItemView : ShopItemView
{
    public GameObject obj_shouChong;//首充

    public override void RefreshShopItemShow()
    {
        base.RefreshShopItemShow();
        
        SingleShopData shopData = ShopManager.Instance.FindSingleShopDataByType(ShopType.ChongZhi);
        if (shopData.ShouChonged.Contains(shopItemData.Id))
        {
            obj_shouChong.gameObject.SetActive(false);
        }
        else
        {
            obj_shouChong.gameObject.SetActive(true);
        }
    }

    public override void ShowIcon()
    {
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ShopIconPath + shopSetting.Icon);

    }
    public override void ShowPrice()
    {
        txt_consume.SetText(shopSetting.Price);//<size=70>7</size>

    }
     
}
