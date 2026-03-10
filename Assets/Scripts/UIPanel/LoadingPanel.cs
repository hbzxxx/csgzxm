using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingPanel : PanelBase
{
    public override void Init(params object[] args)
    {
        base.Init(args);
        RegisterEvent(TheEventType.SuccessLogIn, OnSuccessLogin);
                RegisterEvent(TheEventType.FinishLoading, OnFinishLoading);

    }

    void OnFinishLoading()
    {
        PanelManager.Instance.ClosePanel(this);
    }
    void OnSuccessLogin()
    {
        PanelManager.Instance.ClosePanel(this);
    }
}
