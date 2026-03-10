using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameWordView : SingleViewBase
{

    public Text txt;

    public override void Init(params object[] args)
    {
        base.Init(args);

        txt.SetText(((char)args[0]).ToString());
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


    }
}
