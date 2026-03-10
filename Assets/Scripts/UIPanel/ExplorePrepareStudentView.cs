using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ExplorePrepareStudentView : BigStudentView
{
    public Button btn_up;//出战
    public Button btn_down;//下来

    public GotoExplorePreparePanel parentPanel;

    public override void Init(params object[] args)
    {
        base.Init(args);

        parentPanel = args[1] as GotoExplorePreparePanel;
        addBtnListener(btn_up, () =>
        {
            parentPanel.OnUpStudent(this);
        });
        addBtnListener(btn_down, () =>
        {
            ///该弟子已在其它密境
            if (p.studentStatusType == (int)StudentStatusType.AtExplore)
            {

                PanelManager.Instance.OpenFloatWindow("正在其它" + LanguageUtil.GetLanguageText((int)LanguageIdType.秘境));
                return;
            }
            parentPanel.PrepareExplore(p, false );
        });

        RegisterEvent(TheEventType.StudentPrepareExplore, OnSuccessUpOrDownStudent);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //if (p.IsExplore)
        //{
        //    btn_down.gameObject.SetActive(true);
        //    btn_up.gameObject.SetActive(false);
        //}
        //else
        //{
        //    btn_up.gameObject.SetActive(true);
        //    btn_down.gameObject.SetActive(false);
        //}
        RefreshShow();
    }

    public void RefreshShow()
    {
        if (p.studentStatusType==(int)StudentStatusType.AtExplore)
        {
            btn_down.gameObject.SetActive(true);
            btn_up.gameObject.SetActive(false);
        }
        else
        {
            btn_up.gameObject.SetActive(true);
            btn_down.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 成功上阵
    /// </summary>
    /// <param name="param"></param>
    public void OnSuccessUpOrDownStudent(object[] param)
    {
        PeopleData theP = param[0] as PeopleData;
        if (p.onlyId == theP.onlyId)
        {
            p = theP;
            RefreshShow();
        }
    }
}
