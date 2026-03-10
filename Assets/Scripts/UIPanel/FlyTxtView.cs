using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTxtView : FinishKillEffect
{
    float offset = 100;
    public Text txt;

    public override void Init(params object[] args)
    {
        base.Init(args);
        txt.SetText((string)args[0]);
        txt.color = (Color)args[1];
        transform.position = (Vector3)args[2];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        transform.DOLocalMoveY(transform.localPosition.y + offset, 1f);
    }
    public override void Clear()
    {
        base.Clear();
        transform.DOKill();
    }
}
