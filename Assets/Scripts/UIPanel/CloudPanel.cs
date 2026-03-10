using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudPanel : PanelBase
{
    public Transform trans_cloud;
    public Transform trans_initPos;
    public Transform trans_endPos;

  
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_cloud.position = trans_initPos.position;
    }

    /// <summary>
    /// 移动云层
    /// </summary>
    public void Move(Action callBack)
    {
        trans_cloud.DOMoveY(trans_endPos.position.y, 2f).OnComplete(() =>
        {
            PanelManager.Instance.ClosePanel(this);
            TaskManager.Instance.ShowFirstGuide();
            callBack?.Invoke();
        });
    }
 
}
