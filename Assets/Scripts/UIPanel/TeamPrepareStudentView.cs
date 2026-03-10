using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TeamPrepareStudentView : BigStudentView
{
    public Button btn_up;//出战

    public TeamPanel parentPanel;
    public Transform trans_equip;
    public Transform trans_skill;
    public Text txt_name;
    public override void Init(params object[] args)
    {
        base.Init(args);

        parentPanel = args[1] as TeamPanel;
        addBtnListener(btn_up, () =>
        {
             //parentPanel.OnUpStudent(this);
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

        if (this is not TeamPrepareStudentViewE)
        {
            RefreshShow();

            PanelManager.Instance.CloseAllSingle(trans_equip);

            PanelManager.Instance.CloseAllSingle(trans_skill);


            //if (p.curEquipItem != null)
            //{
            //    PanelManager.Instance.OpenSingle<WithCountItemView>(trans_equip, p.curEquipItem);
            //}

            if (p.allSkillData.equippedSkillIdList.Count > 1)
            {
                SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1],
                    p.allSkillData);
                SingleSkillView view = PanelManager.Instance.OpenSingle<SingleSkillView>(trans_skill, singleSkillData, SkillViewType.Show);

            }
            //singleStudentView.obj_nameBg.gameObject.SetActive(false);
        }
        txt_name.SetText(p.name);
    }

    public void RefreshShow()
    {
        ShowCurWork();
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
    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_equip);

        PanelManager.Instance.CloseAllSingle(trans_skill);
    }
}
