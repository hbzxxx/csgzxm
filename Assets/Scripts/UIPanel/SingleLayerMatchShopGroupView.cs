using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using Framework.Data;

public class SingleLayerMatchShopGroupView : SingleViewBase
{
    public Text txt_layer;

    public List<ShopItemData> shopItemList;
    public int duanWeiNeed;
    public int layer;
    public Transform grid_shopItem;

    public Transform trans_lock;
    public Text txt_lock;
    public override void Init(params object[] args)
    {
        base.Init(args);

        shopItemList = args[0] as List<ShopItemData>;

        duanWeiNeed = (int)args[1];
        layer = (int)args[2];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        txt_layer.SetText(layer + "层");

        if (RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel < duanWeiNeed)
        {
            trans_lock.gameObject.SetActive(true);
            txt_lock.SetText("声望达到" + ConstantVal.MatchRankName(duanWeiNeed) + "解锁");
        }
        else
        {
            trans_lock.gameObject.SetActive(false);
            PanelManager.Instance.CloseAllSingle(grid_shopItem);
            for (int i = 0; i < shopItemList.Count; i++)
            {
                ShopItemData shopItemData = shopItemList[i];
                ItemData itemData = new ItemData();
                itemData.settingId = shopItemData.ItemId;
                itemData.count = (ulong)shopItemData.RemainCount;
                ShopSetting setting = DataTable.FindShopSetting(shopItemData.Id);
                PanelManager.Instance.OpenSingle<ShopItemView>(grid_shopItem, itemData, shopItemData, setting);
            }
        }


    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseAllSingle(grid_shopItem);
    }

}
