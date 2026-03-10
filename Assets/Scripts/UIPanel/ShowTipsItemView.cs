using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 显示装备
/// </summary>
public class ShowTipsItemView : ItemView
{
    public Transform trans_tag;//标签

    public override void Init(params object[] args)
    {
        base.Init(args);

        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<ItemTipsPanel>(PanelManager.Instance.trans_layer2, itemData, true);
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
        ShowTag();
    }



}
