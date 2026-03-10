
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShenYuanTeamPanel : PanelBase
{
    public List<ulong> curStudentPosList1 = new List<ulong>();//学生位
    public List<ulong> curStudentPosList2 = new List<ulong>();//学生位
    public List<Button> posBtnList1;//位置按钮
    public List<Button> posBtnList2;//位置按钮

    public List<Transform> trans_UpStudentParent1;//上阵学生的位置
    public List<Transform> trans_UpStudentParent2;//上阵学生的位置

    public Transform trans_chooseStudentGrid;//选择学生格子
    public List<TeamPrepareStudentView> candidateList = new List<TeamPrepareStudentView>();
    public ScrollViewNevigation scrollViewNevigation;
    public override void Init(params object[] args)
    {
        base.Init(args);

        for (int i = 0; i < posBtnList1.Count; i++)
        {
            int index = i;
            Button btn = posBtnList1[i];
            addBtnListener(btn, () =>
            {
                ulong onlyId = curStudentPosList1[index];

                if (onlyId > 0)
                {
                    StudentManager.Instance.StudentPrepareTeam(onlyId, false,0);
                }

            });
        }

        for (int i = 0; i < posBtnList2.Count; i++)
        {
            int index = i;
            Button btn = posBtnList2[i];
            addBtnListener(btn, () =>
            {
                ulong onlyId = curStudentPosList2[index];

                if (onlyId > 0)
                {
                    StudentManager.Instance.StudentPrepareTeam(onlyId, false, 1);
                }

            });
        }

        RegisterEvent(TheEventType.StudentPrepareTeam, OnSuccessUpOrDownStudent);

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowAllUpStudent();
        ShowAllCandidateStudent();
     }
 

    void ShowAllCandidateStudent()
    {
        PanelManager.Instance.CloseAllSingle(trans_chooseStudentGrid);
        candidateList.Clear();
        List<PeopleData> candidateShowPList = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.LianGong
                && p.studentStatusType != (int)StudentStatusType.AtTeam)
            {
                candidateShowPList.Add(p);

            }
        }

        for (int i = 0; i < candidateShowPList.Count - 1; i++)
        {
            for (int j = 0; j < candidateShowPList.Count - 1 - i; j++)
            {
                //后面的战力高 则二者交换
                if (RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j + 1]) > RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j]))
                {
                    PeopleData temp = candidateShowPList[j];
                    candidateShowPList[j] = candidateShowPList[j + 1];
                    candidateShowPList[j + 1] = temp;

                }
            }
        }
        for (int i = 0; i < candidateShowPList.Count; i++)
        {
            TeamPrepareStudentView view = AddSingle<TeamPrepareStudentView>(trans_chooseStudentGrid, candidateShowPList[i], this);
            candidateList.Add(view);
        }

    }

    /// <summary>
    /// 显示所有上阵的弟子
    /// </summary>
    void ShowAllUpStudent()
    {
        //curStudentPosList[0] = RoleManager.Instance._CurGameInfo.playerPeople.onlyId;
        int theIndex1 = 0;
        int theIndex2 = 0;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.studentStatusType == (int)StudentStatusType.AtTeam)
            {
                if (p.atTeamIndex == 0)
                {
                    curStudentPosList1[theIndex1] = p.onlyId;
                    theIndex1++;
                }
                else if (p.atTeamIndex == 1)
                {
                    curStudentPosList2[theIndex2] = p.onlyId;
                    theIndex2++;
                }
            }
        }

        for (int i = 1; i < curStudentPosList1.Count; i++)
        {
            ulong onlyId = curStudentPosList1[i];
            if (onlyId > 0)
            {
                PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                SingleStudentView view = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_UpStudentParent1[i], p);
                view.showTips = true;
            }

        }
        for (int i = 1; i < curStudentPosList2.Count; i++)
        {
            ulong onlyId = curStudentPosList2[i];
            if (onlyId > 0)
            {
                PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                SingleStudentView view = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_UpStudentParent2[i], p);
                view.showTips = true;
            }

        }
    }

    /// <summary>
    /// 弟子上阵
    /// </summary>
    public void OnUpStudent(int teamIndex, TeamPrepareStudentView view)
    {
        PeopleData p = view.p;
        if (teamIndex == 0)
        {
            int posIndex1 = -1;
            for (int i = 0; i < curStudentPosList1.Count; i++)
            {
                ulong theOnlyId = curStudentPosList1[i];
                if (theOnlyId == 0)
                {
                    posIndex1 = i;
                    break;
                }
            }
            if (posIndex1 != -1)
            {
                StudentManager.Instance.StudentPrepareTeam(view.p.onlyId, true, 0);

                 return;
            }
        }else if (teamIndex == 1)
        {
            int posIndex2 = -1;
            for (int i = 0; i < curStudentPosList2.Count; i++)
            {
                ulong theOnlyId = curStudentPosList2[i];
                if (theOnlyId == 0)
                {
                    posIndex2 = i;
                    break;
                }
            }
            if (posIndex2 != -1)
            {
                StudentManager.Instance.StudentPrepareTeam(view.p.onlyId, true, 1);

                 return;
            }
        }




    }
    /// <summary>
    /// 弟子上下阵
    /// </summary>
    /// <param name="view"></param>
    public void OnSuccessUpOrDownStudent(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        //上阵
        if (p.studentStatusType == (int)StudentStatusType.AtTeam)
        {
            for (int i = 0; i < curStudentPosList1.Count; i++)
            {
                if (curStudentPosList1[i] == 0)
                {
                    curStudentPosList1[i] = p.onlyId;
                    SingleStudentView view = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_UpStudentParent1[i], p);
                    view.showTips = true;
                    break;
                }
            }
            for (int i = 0; i < curStudentPosList2.Count; i++)
            {
                if (curStudentPosList2[i] == 0)
                {
                    curStudentPosList2[i] = p.onlyId;
                    SingleStudentView view = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_UpStudentParent2[i], p);
                    view.showTips = true;
                    break;
                }
            }
        }
        //下阵
        else
        {
            int index1 = curStudentPosList1.IndexOf(p.onlyId);
            if (index1 > 0)
            {
                curStudentPosList1[index1] = 0;
                PanelManager.Instance.CloseAllSingle(trans_UpStudentParent1[index1]);

            }
            int index2 = curStudentPosList2.IndexOf(p.onlyId);
            if (index2 > 0)
            {
                curStudentPosList2[index2] = 0;
                PanelManager.Instance.CloseAllSingle(trans_UpStudentParent1[index2]);

            }
        }
        ShowAllCandidateStudent();
    }

    public override void Clear()
    {
        base.Clear();

        curStudentPosList1.Clear();
        for (int i = 0; i < 4; i++)
        {
            curStudentPosList1.Add(0);
        }
        for (int i = 0; i < trans_UpStudentParent1.Count; i++)
        {
            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent1[i]);
        }

        curStudentPosList2.Clear();
        for (int i = 0; i < 4; i++)
        {
            curStudentPosList2.Add(0);
        }
        for (int i = 0; i < trans_UpStudentParent1.Count; i++)
        {
            ClearCertainParentAllSingle<SingleViewBase>(trans_UpStudentParent1[i]);
        }
        for (int i = 0; i < trans_UpStudentParent2.Count; i++)
        {
            ClearCertainParentAllSingle<SingleViewBase>(trans_UpStudentParent2[i]);
        }
        ClearCertainParentAllSingle<SingleViewBase>(trans_chooseStudentGrid);

        //PanelManager.Instance.CloseAllSingle(trans_mapGrid);
        //allMapViewList.Clear();

    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();

        TaskManager.Instance.guide_shangZhen = false;
    }
}
