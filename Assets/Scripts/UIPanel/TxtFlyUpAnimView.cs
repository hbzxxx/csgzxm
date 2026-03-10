using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TxtFlyUpAnimView : FinishKillEffect
{
    public Text txt;
    public Color txtColor;
    public override void Init(params object[] args)
    {
        base.Init(args);
        txt.SetText((string)args[0]);
        transform.DOKill();
        txt.DOKill();
        txt.color = txtColor;
        transform.localPosition = Vector3.zero;
        transform.DOLocalMoveY(230, 1.5f);
        txt.DOFade(0, 1.5f);
    }
}
