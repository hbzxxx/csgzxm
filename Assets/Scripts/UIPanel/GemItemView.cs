using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemItemView : ItemView
{
    public GemCompositePanel parentPanel;
    public Text txt_count;//数量
    public Image img_bg;
    public Transform trans_tag;//装备中

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as GemCompositePanel;
        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedGemToComposite(this);
        });

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();

    }


    /// <summary>
    /// 是否镶嵌中
    /// </summary>
    void ShowTag()
    {

        if (itemData.gemData.isInlayed)
        {
            trans_tag.gameObject.SetActive(true);
        }
        else
        {
            trans_tag.gameObject.SetActive(false);
        }

    }

    public override void RefreshShow()
    {
        base.RefreshShow();
        ShowCount();
        ShowTag();
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
        txt_count.SetText(UIUtil.ShowBigCount((long)(ulong)itemData.count));
    }

    public void OnChoosed(bool choose)
    {
        return;
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
