using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleStudentInfluenceRateView : SingleViewBase
{
    public Image img;
    public Text txt;
    public override void Init(params object[] args)
    {
        base.Init(args);
        Quality quality = (Quality)args[0];
        int rate = (int)args[1];
        //img.color= CommonUtil.QualityColor(quality);
        img.sprite = ConstantVal.StudentInfluenceIcon(quality);
        txt.SetText(rate + "%");
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
