using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZhanDouLiPropertyView : SingleViewBase
{
    PeopleData p;
    public Text txt;

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        long res= RoleManager.Instance.CalcZhanDouLi(p);
        txt.SetText(res.ToString());
    }
}
