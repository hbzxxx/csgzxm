using DG.Tweening;
using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GetItemFlyUpAnimView : FinishKillEffect
{
    public Text txt;
    public Image img;
    public ItemData itemData;
    public override void Init(params object[] args)
    {
        base.Init(args);
        Vector3 pos = (Vector3)args[0];

        itemData = args[1] as ItemData;
        ItemSetting setting = DataTable.FindItemSetting(itemData.settingId);
        string nameStr = "";
        string countStr = "";
        if (setting.ItemType.ToInt32() == (int)ItemType.MoneyDan)
        {
            ulong lingShiNum = setting.Price.ToUInt64();
            ItemSetting moneySetting = DataTable.FindItemSetting((int)ItemIdType.LingShi);
            lingShiNum = (ulong)Mathf.RoundToInt(lingShiNum * (1 + ((int)itemData.quality - 1) * 0.2f));
            ulong count = lingShiNum * itemData.count;
            //nameStr = moneySetting.name;
            countStr = count.ToString();
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + moneySetting.UiName);
        }
        else
        {
            //nameStr = setting.name;
            countStr = itemData.count.ToString();
            img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + setting.UiName);

        }


        txt.SetText(nameStr + "+"+ countStr);
        transform.DOKill();
        transform.position = pos;
        txt.DOKill();
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
        //transform.localPosition = Vector3.zero;
        transform.DOLocalMoveY(transform.localPosition.y+ 130, 1.5f);
        txt.DOFade(0, 1.5f);
    }
}
