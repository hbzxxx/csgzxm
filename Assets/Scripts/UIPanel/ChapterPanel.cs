using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChapterPanel : PanelBase
{
    public Image bg;
    public Text txt;
    public override void Init(params object[] args)
    {
        base.Init(args);

        bg.DOKill();
        txt.DOKill();
        txt.SetText((string)args[0]);
        //bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, 0);
        txt.color= new Color(txt.color.r, txt.color.g, txt.color.b, 0);
        txt.DOFade(1, 1).OnComplete(() =>
        {
            txt.DOFade(1, 1).OnComplete(() =>
            {
                txt.DOFade(0, 1).OnComplete(() =>
                {
                    PanelManager.Instance.ClosePanel(this);
                });
            });
        });
    }

}
