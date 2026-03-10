using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetAwardWithCloseActionPanel : GetAwardPanel
{
    public Action callBack;
    public override void Init(params object[] args)
    {
        base.Init(args);
        callBack = args[1] as Action;
        addBtnListener(btn_emptyClose, () =>
        {
            PanelManager.Instance.ClosePanel(this);

            callBack();
         });
    }
}
