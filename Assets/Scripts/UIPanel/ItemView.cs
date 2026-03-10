using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品
/// </summary>
public class ItemView : SingleViewBase
{
    //奖励
    protected ItemData itemData;
    public Image img_bottom;//底图
    public Image img_icon;   //icon
    public Button btn;//弹出tips
    public GameObject obj_redPoint;//红点，刚解锁和可升级时会冒出来，点了以后会消失
    public ItemSetting setting;
    public Transform trans_jingLian;
    public Text txt_jingLian;
    public Text txt_jinglian1;

    public void SetItemData(ItemData data)
    {
        this.itemData = data;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public override void Init(object[] args)
    {
        this.itemData = args[0] as ItemData;
        setting = DataTable.FindItemSetting(itemData.settingId);
        itemData.setting = setting;
    }

    public override void OnOpenIng()
    {
        RefreshShow();
        if(trans_jingLian!=null)
            trans_jingLian.gameObject.SetActive(false);
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    public virtual void RefreshShow()
    {
        //RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.Item, itemData.SettingId);
        Clear();
        ShowBottom();
        ShowIcon();
        ShowJingLian();
    }

    void ShowJingLian()
    {
        if (txt_jinglian1 != null)
            txt_jinglian1.SetText(itemData.equipProtoData.jingLianLv.ToString());
        if (trans_jingLian != null)
        {
            if (itemData.equipProtoData != null
          && itemData.equipProtoData.jingLianLv > 0)
            {
                //trans_jingLian.gameObject.SetActive(true);
                if(txt_jingLian!=null)
                    txt_jingLian.SetText(itemData.equipProtoData.jingLianLv.ToString());
            }
            else
            {
                trans_jingLian.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 显示底图
    /// </summary>
    public virtual void ShowBottom()
    {
        if(img_bottom!=null)
            img_bottom.ShowItemFrameImg(itemData);
    }

    /// <summary>
    /// 显示icon
    /// </summary>
    public virtual void ShowIcon()
    {
        //CommonUtil.ShowItemTag
        img_icon.ShowItemIcon(itemData);
    }

    public override void Clear()
    {
        base.Clear();
        if(img_bottom!=null)
            PanelManager.Instance.CloseAllSingle(img_bottom.transform);
    }
}

/// <summary>
/// 是只能看还是有功能
/// </summary>
public enum ItemViewType
{
    None=0,
    Knapsack=1,
    OnlyShow=2,
}

/// <summary>
/// 稀有度
/// </summary>
public enum Rarity
{
    None=0,
    Fan=1,//凡
    Huang=2,//黄
    Xuan=3,//玄
    Di=4,//地
    Tian=5,//天
    End=6,
}
