using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffFunctionTxtView : FinishKillEffect
{
    public Text txt;
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.localPosition = Vector3.zero;
        txt.SetText((string)args[0]);
        txt.DOKill();
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
        txt.DOFade(0, totalTime);
    }
}
