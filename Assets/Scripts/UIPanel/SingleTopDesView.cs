using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTopDesView : SingleViewBase
{
    public Image img_highLight;
    public Text txt;
    bool highLight;
    string content;
     Color highLightTxtColor=new Color32(255,172,88,255);
     Color commonTxtColor = Color.white;

    public override void Init(params object[] args)
    {
        base.Init(args);
        txt.SetText((string)args[0]);
        highLight = (bool)args[1];
        if (highLight)
        {
            img_highLight.color = new Color(img_highLight.color.r, img_highLight.color.g, img_highLight.color.b, 1);
            txt.color = highLightTxtColor;
        }
        else
        {
            img_highLight.color = new Color(img_highLight.color.r, img_highLight.color.g, img_highLight.color.b, 0);
            txt.color = commonTxtColor;

        }
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }
}
