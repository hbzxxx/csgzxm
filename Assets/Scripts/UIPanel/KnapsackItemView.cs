using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 背包中的物品
/// </summary>
public class KnapsackItemView : ItemView
{
    public KnapsackPanel parentPanel;
    public Text txt_count;//数量
    public Image img_bg;
    public Transform trans_tag;//标签
    public Text txt_tag;
    public Image img_choosed;
    public Text txt_name;
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as KnapsackPanel;
        addBtnListener(btn, ()=> 
        {
            PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData);
            //parentPanel.OnClickedItem(this);
        });

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
     
    }

    /// <summary>
    /// 是否装备中/镶嵌中
    /// </summary>
    void ShowTag()
    {
        trans_tag.ShowItemTag(itemData);


    }

    public override void RefreshShow()
    {
        base.RefreshShow();
        ShowCount();
        //ShowTag();
        //ShowName();
    }
    void ShowName()
    {
        txt_name.SetText(itemData.setting.Name);
    }
    /// <summary>
    /// 显示数量
    /// </summary>
    public void ShowCount()
    {
        //string name = "";
        //if (awardData.awardType == AwardType.Item)
        //{
        //   name=  DataTable.FindItemSetting(awardData.awardId).name;
        //}
        //else
        //{
        //    name = DataTable.FindPropertySetting(awardData.awardId).name;

        //}
        //txt_count.SetText(name+"X"+awardData.awardCount.ToString());
        if (itemData.equipProtoData == null)
            txt_count.SetText(UIUtil.ShowBigCount((long)(ulong)itemData.count));
        else
            txt_count.SetText("Lv." + itemData.equipProtoData.curLevel);
    }

    public void OnChoosed(bool choose)
    {
        if (choose)
        {
            img_choosed.gameObject.SetActive(true);
         }
        else
        {
            img_choosed.gameObject.SetActive(false);
        }
    }

    public override void Clear()
    {
        base.Clear();
        img_choosed.gameObject.SetActive(false);
    }

}
