using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 进行中
/// </summary>
public class ProcessingPanel : PanelBase
{
    public Image img_bar;
    public Action callBack;
    public override void Init(params object[] args)
    {
        base.Init(args);
        callBack = args[0] as Action;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        img_bar.DOKill();
        img_bar.fillAmount = 0;
        img_bar.DOFillAmount(1, 2).OnComplete(() =>
        {
            callBack();
            PanelManager.Instance.ClosePanel(this);
        });
    }
}
