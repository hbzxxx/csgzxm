using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SocializationPanel : PanelBase
{
    public PeopleData p;

    public Button btn_haoGan;
    public Button btn_chouHen;
    public Button btn_record;

    public Transform trans_scroll;//scroll
    public Transform trans_grid;//认识的人

    public Transform trans_record;//社交动态
    public Transform trans_recordGrid;//动态格子

    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        addBtnListener(btn_haoGan, () =>
        {
            ShowHaoGan();
        });
        addBtnListener(btn_chouHen, () =>
        {
            ShowHate();
        });
        addBtnListener(btn_record, () =>
        {
            ShowRecord();
        });
    }


    /// <summary>
    /// 显示有好感的人
    /// </summary>
    void ShowHaoGan()
    {
        trans_scroll.gameObject.SetActive(true);
        trans_record.gameObject.SetActive(false);
        ClearCertainParentAllSingle<SingleHaoGanDuStudentView>(trans_grid);

        for(int i = 0; i < p.socializationData.knowPeopleList.Count; i++)
        {
            PeopleData knowP = StudentManager.Instance.FindStudent( p.socializationData.knowPeopleList[i]);
            if (StudentManager.Instance.FindHaoGanDu(p, knowP) >= 0)
            {
                AddSingle<SingleHaoGanDuStudentView>(trans_grid, p, knowP);
            }
        }

    }


    /// <summary>
    /// 显示有仇的人
    /// </summary>
    void ShowHate()
    {
        trans_scroll.gameObject.SetActive(true);
        trans_record.gameObject.SetActive(false);

        ClearCertainParentAllSingle<SingleHaoGanDuStudentView>(trans_grid);

        for (int i = 0; i < p.socializationData.knowPeopleList.Count; i++)
        {
            PeopleData knowP = StudentManager.Instance.FindStudent(p.socializationData.knowPeopleList[i]);
            if (StudentManager.Instance.FindHaoGanDu(p, knowP) < 0)
            {
                AddSingle<SingleHaoGanDuStudentView>(trans_grid, p, knowP);
            }
        }

    }

    /// <summary>
    /// 显示记录
    /// </summary>
    void ShowRecord()
    {
        trans_scroll.gameObject.SetActive(false);
        trans_record.gameObject.SetActive(true);

        ClearCertainParentAllSingle<SingleSocializationRecordVew>(trans_recordGrid);
        for(int i = p.socializationData.socialRecordList.Count - 1; i >= 0; i--)
        {
            SocializationRecordData recordData = p.socializationData.socialRecordList[i];
            AddSingle<SingleSocializationRecordVew>(trans_recordGrid, recordData);
        }
    }
}
