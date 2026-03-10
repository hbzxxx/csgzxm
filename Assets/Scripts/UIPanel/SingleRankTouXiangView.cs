using Framework.Data;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleRankTouXiangView : SingleViewBase
{
     public Image img_headIcon;
    public Portrait portrait;
    public Image img_kuangDi;
    public Image img_kuang;

    public override void Init(params object[] args)
    {
        base.Init(args);
     }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowPortrait();
    }
    public void ShowPortrait()
    {
        
    }
}
