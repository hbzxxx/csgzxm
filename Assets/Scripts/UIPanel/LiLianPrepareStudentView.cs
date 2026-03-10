using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiLianPrepareStudentView : SingleStudentView
{

    public Button btn_up;//出战

    public LiLianPanel parentPanel;
    public Image img_haoGan;
    public Text txt_haoGan;
    public override void Init(params object[] args)
    {
        base.Init(args);
        peopleData = args[0] as PeopleData;
        parentPanel = args[1] as LiLianPanel;
        addBtnListener(btn_up, () =>
        {
            parentPanel.OnUpStudent(this);
        });
   
     }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshHaoGanShow();
    }

    /// <summary>
    /// 刷新好感度
    /// </summary>
    public void RefreshHaoGanShow()
    {
        if (parentPanel.curStudentPosList[0] != 0 && parentPanel.curStudentPosList[1] == 0)
        {
            img_haoGan.gameObject.SetActive(true);
            PeopleData p1 = StudentManager.Instance.FindStudent(parentPanel.curStudentPosList[0]);

            int haoGanDu= StudentManager.Instance.FindHaoGanDu(p1, peopleData);
            if(haoGanDu>=0)
            img_haoGan.sprite = ConstantVal.HaoGanBg();
            //else
            //    img_haoGan.sprite = ConstantVal.ChouHenBg();

            txt_haoGan.SetText(Mathf.Abs(haoGanDu).ToString());
        }
        else
        {
            img_haoGan.gameObject.SetActive(false);
        }
    }
}
