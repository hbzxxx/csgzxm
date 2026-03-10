using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;
using System;

public class ShopItemView : ItemView
{
    public Text txt_itemName;
    public Text txt_count;
    public ShopItemData shopItemData;
    public ShopSetting shopSetting;
    public ItemSetting consumeSetting;
    public int singleConsumeNum;
    public Image img_consumeIcon;
    public Text txt_consume;

    public Button btn_buy;
    public Transform trans_lock;
    public Text txt_lock;
    public override void Init(params object[] args)
    {
         
        base.Init(args);
        shopItemData = args[1] as ShopItemData;
        shopSetting = args[2] as ShopSetting;
        List<int> consume = CommonUtil.SplitCfgOneDepth(shopSetting.Price);
        if (consume.Count == 2)
        {
            singleConsumeNum = consume[1];
            consumeSetting = DataTable.FindItemSetting(consume[0]);
        }
        //广告
        else
        {
            singleConsumeNum = consume[0];
            consumeSetting = null;
        }
     
        addBtnListener(btn_buy, () =>
         {
             // 检查是否为RMB现金购买，如果是则需要二次确认
             if (shopSetting.Rmb == "1")
             {
                 PanelManager.Instance.OpenCommonHint("确定购买吗？", () =>
                 {
                     // 确认购买后执行原购买逻辑
                     ExecuteBuyLogic();
                 }, null);
             }
             else
             {
                 // 非RMB购买，直接执行购买逻辑
                 ExecuteBuyLogic();
             }
          });

        addBtnListener(btn, () =>
        {
            if(shopSetting.Type.ToInt32()==(int)ShopType.Match
            || shopSetting.Type.ToInt32() == (int)ShopType.GuangGaoLing
            || shopSetting.Type.ToInt32() == (int)ShopType.DailyGuangGaoFuLi
                     || shopSetting.Type.ToInt32() == (int)ShopType.HuoYueBi)
            {
                ItemData item = new ItemData();
                item.settingId = shopItemData.ItemId;
                PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, item,true);
            }
       
        });

        RegisterEvent(TheEventType.OnBuyItem, RefreshShopItemShow);
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShopItemShow();
    }

   public virtual void RefreshShopItemShow()
    {
        txt_count.SetText(shopItemData.RemainCount + "/" + shopSetting.MaxCount);

        //if (shopSetting.type == (int)ShopType.DailyGuangGaoFuLi)
        //{
        //    PanelManager.Instance.CloseAllSingle(trans_guangGaoShopGrid)
        //}

        ShowPrice();
      
        if (!string.IsNullOrWhiteSpace(shopSetting.ItemName))
        {
            txt_itemName.SetText(shopSetting.ItemName);
        }
        else
            txt_itemName.SetText(setting.Name);
    }

    /// <summary>
    /// 显示价格
    /// </summary>
    public virtual void ShowPrice()
    {
        if (consumeSetting != null)
        {
            img_consumeIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + consumeSetting.UiName);
            string res = UIUtil.ShowBigCount(singleConsumeNum);
            txt_consume.SetText(res);

        }
        else
        {
            //这里显示广告播放按钮
            

            img_consumeIcon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + ConstantVal.icon_guangGaoPlayBtn);

            txt_consume.SetText("");

        }
    }

    /// <summary>
    /// 执行购买逻辑
    /// </summary>
    private void ExecuteBuyLogic()
    {
        List<int> consume = CommonUtil.SplitCfgOneDepth(shopSetting.Price);
        
        //非看广告的并且剩余数量大于1个
        if (shopItemData.RemainCount > 1
        &&consumeSetting!=null)
        {
          
                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;

                ItemData singlePriceItem = new ItemData();
                singlePriceItem.settingId = consume[0];
                singlePriceItem.count = (ulong)consume[1];
                Action<int> buyCallBack = OnBuy;
                PanelManager.Instance.OpenPanel<BuySliderPanel>(PanelManager.Instance.trans_layer2, itemData, buyCallBack, singlePriceItem);
            
            
        }else if(shopItemData.RemainCount == 1)
        {
            OnBuy(1);
        }
        //看广告的
        else if (consumeSetting == null
        && shopItemData.RemainCount>0)
        {
            OnBuy(1);

        }
        else if(shopItemData.RemainCount<=0)
        {
            PanelManager.Instance.OpenFloatWindow("已售罄");
        }
    }

    public void OnBuy(int num)
    {
        ShopManager.Instance.OnBuyItem(shopItemData, num);
    }

    /// <summary>
    /// 锁
    /// </summary>
    /// <param name="content"></param>
    public void Lock(string content)
    {
        trans_lock.gameObject.SetActive(true);
        txt_lock.SetText(content);

    }

    public override void Clear()
    {
        base.Clear();
       if(trans_lock!=null)
        trans_lock.gameObject.SetActive(false);
    }
}
