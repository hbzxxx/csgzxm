using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

public class SingleStudentChuZhanView : SingleStudentView
{
    public Sprite bg;
    public TeamPanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);

        parentPanel = args[1] as TeamPanel;
        addBtnListener(btn, () =>
        {
            parentPanel.OnUpStudent(this);
        });
        ////弟子下阵
        //addBtnListener(btn_down, () =>
        //{
        //    StudentManager.Instance.StudentPrepareTeam(p.OnlyId, false);
        //    //MapManager.Instance.StudentPrepareExplore(p.OnlyId, false,parentPanel.curExploreId);
        //});

        RegisterEvent(TheEventType.StudentPrepareTeam, OnSuccessUpOrDownStudent);
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if(img_bg!=null)
            img_bg.sprite = bg;
    }

    public override void ShowLv()
    {
        if(txt_lv!=null)
        {
            txt_lv.gameObject.SetActive(true);
            if (peopleData.talent == (int)StudentTalent.LianGong)
            {

                //txt_lv.SetText((peopleData.trainIndex + 1) + "级");
                txt_lv.text = "Lv." + (peopleData.trainIndex + 1);
            }
            else
            {
                //txt_lv.SetText("Lv." + peopleData.studentLevel);
                txt_lv.text = "Lv." + peopleData.studentLevel;
            }
        }
    }

    public override void ShowStudentKuang()
    {
        if (peopleData.talent == (int)StudentTalent.LianGong)
        {
            if (img_yuanSu != null)
            {
                img_yuanSu.gameObject.SetActive(true);
                img_yuanSu.sprite = ConstantVal.YuanSuIcon((YuanSuType)peopleData.yuanSu);
            }
        }
        else
        {
            if (img_yuanSu != null)
            {
                img_yuanSu.gameObject.SetActive(false);
            }
        }

    }
    /// <summary>
    /// 成功上阵
    /// </summary>
    /// <param name="param"></param>
    public void OnSuccessUpOrDownStudent(object[] param)
    {
        PeopleData theP = param[0] as PeopleData;
        if (peopleData.onlyId == theP.onlyId)
        {
            peopleData = theP;
            RefreshShow();
        }
    }
}
