using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonCardShopItemView : ShopItemView
{
    public Text txt_reachTime;
    public Text txt_des;

    public override void Init(params object[] args)
    {
        base.Init(args);
    }
 
    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_des.SetText(shopSetting.Des);
    }
    public override void ShowIcon()
    {
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ShopIconPath + shopSetting.Icon);
        //Debug.Log(shopSetting.Icon);
    }
    public override void RefreshShopItemShow()
    {
        base.RefreshShopItemShow();

        if (GameTimeManager.Instance.connectedToFuWuQiTime)
        {
            if (shopItemData.moonCardReachTime <= GameTimeManager.Instance.curFuWuQiTime)
            {
                txt_reachTime.gameObject.SetActive(false);
            }
            else
            {
                txt_reachTime.gameObject.SetActive(true);

                TimeSpan remainTime = CGameTime.Instance.GetDateTimeByTimeStamp(shopItemData.moonCardReachTime) - CGameTime.Instance.GetDateTimeByTimeStamp(GameTimeManager.Instance.curFuWuQiTime);
                int days= remainTime.Days;
                txt_reachTime.SetText("剩余" + days+"天");
            }
        }
        else
        {
            txt_reachTime.gameObject.SetActive(false);

        }

    }

    public override void ShowPrice()
    {
        txt_consume.SetText(shopSetting.Price + "元");

    }

    public override void Clear()
    {
        base.Clear();
    }
}
