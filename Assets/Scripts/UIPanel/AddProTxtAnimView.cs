using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AddProTxtAnimView : FinishKillEffect
{
    public Text txt;
    public Vector2 pos;
    public override void Init(params object[] args)
    {
        base.Init(args);
        txt.DOKill();
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
        int num = (int)args[0];
        pos =(Vector2)(args[1]);
        this.transform.position = pos;
        string add = "";
        if (num > 0)
            add = "+";

        txt.SetText(add + num);
        txt.DOFade(0, 1);
    }
}
