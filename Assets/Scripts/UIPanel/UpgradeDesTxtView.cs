using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDesTxtView : SingleViewBase
{
    public Text txt;

    public override void Init(params object[] args)
    {
        base.Init(args);
        txt.SetText(args[0].ToString());
    }
}
