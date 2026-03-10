using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 双倍领取
/// </summary>
public class GetAwardWithDoubleLingQuPanel : GetAwardWithStudentUpgradePanel
{

    public Action<bool> action;
    public ADType adType;
    public Button btn_lingQu;
    public Button btn_double;

    public override void Init(params object[] args)
    {
        base.Init(args);

        action = args[3] as Action<bool>;

        adType = (ADType)args[4];

        addBtnListener(btn_lingQu, () =>
        {
            action?.Invoke(false);
        });
        addBtnListener(btn_double, () =>
        {
            ADManager.Instance.WatchAD(adType);
        });
    }


}
