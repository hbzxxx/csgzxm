using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 弹幕txt
/// </summary>
public class DanMuTxtView : SingleViewBase
{
    public Text txt;
    public int fontsize_big;
    public int fontsize_small;
    public int logicPosY;
    public override void Init(params object[] args)
    {
        base.Init(args);
        transform.DOKill();
        txt.SetText((string)args[0]);
        bool important = (bool)args[1];
        logicPosY = (int)args[2];
        if (important)
        {
            txt.fontSize = fontsize_big;
            txt.color = Color.red;
        }
        else
        {
            txt.fontSize = fontsize_small;
            txt.color = Color.black;
        }
    }
}
