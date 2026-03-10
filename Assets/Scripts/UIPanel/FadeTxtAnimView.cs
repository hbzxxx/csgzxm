using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeTxtAnimView : SingleViewBase
{
    public float sustainTime;
    public float fadeTime;
    public Text txt;
    public Image bg;

    public override void Init(params object[] args)
    {
        base.Init(args);

        txt.DOKill();
        bg.DOKill();
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 1);

        txt.SetText((string)args[0]);


        txt.DOFade(1, sustainTime).OnComplete(() =>
        {
            txt.DOFade(0, fadeTime);
        });
        bg.DOFade(1, sustainTime).OnComplete(() =>
        {
            bg.DOFade(0, fadeTime);
        });

    }

    public override void Clear()
    {
        base.Clear();
 
    }
}
