using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class ZhengRongPanel : PanelBase
{
    public Button btn_common;
    public Button btn_gaoJi;
    PeopleData p;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
    
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
