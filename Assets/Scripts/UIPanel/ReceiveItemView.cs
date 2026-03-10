using cfg;
using DG.Tweening;
using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReceiveItemView : SingleViewBase,IPointerEnterHandler
{
    public Vector2 startPos;
    public Vector2 endPos;
    public ItemData itemData;
    public Text txt_num;

    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        startPos = (Vector2)args[1];
        endPos = (Vector2)args[2];

    }

    //public void OnBeginDrag(PointerEventData eventData)
    //{
    //    ReceiveItem();

    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    ReceiveItem();
    //}

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        txt_num.SetText(itemData.count.ToString());


        transform.DOKill();

        transform.position = startPos;
        transform.DOMove(endPos, 1f);


    }

    void ReceiveItem()
    {
        Debug.Log("收获");
        EventCenter.Broadcast(TheEventType.GetItemFlyAnim, this.transform.position, itemData);

        PanelManager.Instance.CloseSingle(this);

        //如果是钱丹 则直接得钱
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);
        if (itemSetting.ItemType.ToInt32() == (int)ItemType.MoneyDan)
        {
            ulong lingShiNum = itemSetting.Price.ToUInt64();

            lingShiNum = (ulong)Mathf.RoundToInt(lingShiNum * (1 + ((int)itemData.quality - 1) * 0.2f));
            ItemManager.Instance.GetItem((int)ItemIdType.LingShi, itemData.count * lingShiNum);

        }
        else
        {
            ItemManager.Instance.GetItem(itemData.settingId, itemData.count);

        }
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    //ItemData itemData = new ItemData();
    //    //itemData.SettingId = settingId;
    //    //itemData.Count = 1;
    //    ReceiveItem();



    //}

    public void OnPointerEnter(PointerEventData eventData)
    {
        ReceiveItem();

    }
}
