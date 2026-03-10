using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpItemTipsPanel : ItemTipsPanel
{
  
      public override void Init(params object[] args)
    {
        base.Init(args);

         RegisterEvent(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.CloseHandleItemTipsPanel, OnHandleItemTipsClose);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        ShowGuide();
        transform.localPosition = Vector3.zero;
    }
    void OnEquip()
    {
        //PanelManager.Instance.ClosePanel(this);
    }
    void ShowGuide()
    {
        
    }
 
    public override void Clear()
    {
        base.Clear();
 

    }
    public override void RefreshDownBtnShow()
    {

    }
    void OnHandleItemTipsClose()
    {

        PanelManager.Instance.ClosePanel(this);
    }
}
