using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleMailView : SingleViewBase
{
     public MailPanel parentPanel;
    public Button btn;
    public Text txt_label;
    public Text txt_content;
    public Text txt_time;
    public GameObject obj_redPoint;
    public override void Init(params object[] args)
    {
        base.Init(args);
 
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }

    public void RefreshShow()
    {
 
    }
}
