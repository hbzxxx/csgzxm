using Framework.Data;
  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleRoleRangeView : SingleViewBase
{
 
 
    public Text txt_range;
    public Text txt_zongMenName;
    public Text txt_num;
    public Text txt_zongMenTime;

    public Transform trans_rank;
    public Image img_rank;
    public Transform trans_starShow;
    int rank = 0;
    public Image img_headIcon;
    public Image img_kuangDi;
    public Image img_kuang;
    public Portrait portrait;
    public Text txt_uid;
    public override void Init(params object[] args)
    {
        base.Init(args);
       
     }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        string numStr = "";
 
    }

    public void ShowPortrait()
    {
        
    }
}
