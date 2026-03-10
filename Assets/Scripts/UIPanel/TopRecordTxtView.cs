using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopRecordTxtView : SingleViewBase
{
    public Text txt;


    public override void Init(params object[] args)
    {
        base.Init(args);
        string str = (string)args[0];
        txt.SetText(str);
    }
}
