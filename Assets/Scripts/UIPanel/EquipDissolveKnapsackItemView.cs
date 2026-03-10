using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class EquipDissolveKnapsackItemView : ItemView
{
    public EquipDissolvePanel parentPanel;
    public Text txt_count;//数量
    public Image img_bg;
    public Transform trans_tag;//标签
    public Text txt_name;

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as EquipDissolvePanel;
        //addBtnListener(btn, () =>
        //{
        //    parentPanel.OnClickedItem(this);
        //});

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
        //if (RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem != null
        //   && itemData.equipProtoData.onlyId == RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem.onlyId)
        //{
        //    trans_tag.gameObject.SetActive(true);
        //}
        //else
        //{
        //    trans_tag.gameObject.SetActive(false);
        //}
    }

    public override void RefreshShow()
    {
        base.RefreshShow();
        ShowCount();
        ShowTag();
        txt_name.SetText(setting.Name);
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
            img_bg.color = ConstantVal.color_choosed;
        }
        else
        {
            img_bg.color = Color.white;
        }
    }
}
