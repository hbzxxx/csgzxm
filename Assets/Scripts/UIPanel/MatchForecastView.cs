using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 预报
/// </summary>
public class MatchForecastView : SingleViewBase
{
    public Text txt;
    public override void Init(params object[] args)
    {
        base.Init(args);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }

    public void RefreshShow()
    {

    }
}
