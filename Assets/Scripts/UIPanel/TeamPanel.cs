
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPanel : PanelBase
{
    public List<ulong> curStudentPosList = new List<ulong>();//学生位
    public List<Transform> trans_UpStudentParent;//上阵学生的位置
    //public List<Text> texts ;
    public List<Button> posBtnList;//位置按钮

    public Transform trans_chooseStudentGrid;//选择学生格子SingleStudentChuZhanView
    public List<SingleStudentChuZhanView> candidateList = new List<SingleStudentChuZhanView>();
    public ScrollViewNevigation scrollViewNevigation;
    public Text txt_zhanLi;
    public override void Init(params object[] args)
    {
        base.Init(args);

  

        for(int i = 0; i < posBtnList.Count; i++)
        {
            int index = i;
            Button btn = posBtnList[i];
            addBtnListener(btn, () =>
            {
                ulong onlyId = curStudentPosList[index];

                if (onlyId > 0)
                {
                    if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                    {
                        PanelManager.Instance.OpenFloatWindow("掌门无法下阵");
                        return;
                    }
                    else
                    {
                        StudentManager.Instance.StudentPrepareTeam(onlyId, false,index);

                    }
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
        ShowGuide();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_shangZhen)
        {
            for(int i=0;i< candidateList.Count; i++)
            {
                PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, candidateList[0].gameObject);
                break;
            }
        }
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
                if (RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j + 1])> RoleManager.Instance.CalcZhanDouLi(candidateShowPList[j]))
                {
                    PeopleData temp = candidateShowPList[j];
                    candidateShowPList[j] = candidateShowPList[j + 1];
                    candidateShowPList[j + 1] = temp;

                }
            }
        }

        for(int i = 0; i < candidateShowPList.Count; i++)
        {
            SingleStudentChuZhanView view =AddSingle<SingleStudentChuZhanView>(trans_chooseStudentGrid, candidateShowPList[i], this);
            candidateList.Add(view);
        }
     
    }

    /// <summary>
    /// 显示所有上阵的弟子
    /// </summary>
    void ShowAllUpStudent()
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId > 0)
            {
                curStudentPosList[i] = onlyId;
                PeopleData p = StudentManager.Instance.FindStudent(onlyId);
                SingleStudentViewpos view = AddSingle<SingleStudentViewpos>(trans_UpStudentParent[i], p);
                //texts[i].gameObject.SetActive(false);
                view.showTips = true;
            }
        }
        
        // 更新队伍战力显示
        UpdateTeamZhanLi();
    }

    /// <summary>
    /// 弟子上阵
    /// </summary>
    public void OnUpStudent(SingleStudentChuZhanView view)
    {
        PeopleData p = view.peopleData;
        int posIndex = -1;
        for (int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong theOnlyId = curStudentPosList[i];
            if (theOnlyId == 0)
            {
                posIndex = i;
                break;
            }
        }
        if (posIndex == -1)
        {
            PanelManager.Instance.OpenFloatWindow("最多上阵" + curStudentPosList.Count + "个弟子");
            return;
        }
        StudentManager.Instance.StudentPrepareTeam(view.peopleData.onlyId, true,posIndex);
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.guide_shangZhen = false;

     }
    /// <summary>
    /// 弟子上下阵
    /// </summary>
    /// <param name="view"></param>
    public void OnSuccessUpOrDownStudent(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        //上阵
        if (p.studentStatusType==(int)StudentStatusType.AtTeam)
        {
            for (int i = 0; i < curStudentPosList.Count; i++)
            {
                if (curStudentPosList[i] == 0)
                {
                    curStudentPosList[i] = p.onlyId;
                    SingleStudentViewpos view = PanelManager.Instance.OpenSingle<SingleStudentViewpos>(trans_UpStudentParent[i], p);
                    //texts[i].gameObject.SetActive(false);
                    view.showTips = true;
                    break;
                }
            }

        }
        else
        {
            int index = curStudentPosList.IndexOf(p.onlyId);
            curStudentPosList[index] = 0;
            PanelManager.Instance.CloseAllSingle(trans_UpStudentParent[index]);
            //texts[index].gameObject.SetActive(true);
        }
        ShowAllCandidateStudent();
        
        // 计算当前上阵弟子的总战力
        UpdateTeamZhanLi();
    }
    
    /// <summary>
    /// 更新队伍战力显示
    /// </summary>
    void UpdateTeamZhanLi()
    {
        long totalZhanLi = 0;
        for (int i = 0; i < curStudentPosList.Count; i++)
        {
            ulong onlyId = curStudentPosList[i];
            if (onlyId > 0)
            {
                PeopleData student = StudentManager.Instance.FindStudent(onlyId);
                if (student != null)
                {
                    totalZhanLi += RoleManager.Instance.CalcZhanDouLi(student);
                }
            }
        }
        
        if (txt_zhanLi != null)
        {
            txt_zhanLi.text = totalZhanLi.ToString();
        }
    }

    public override void Clear()
    {
        base.Clear();

        curStudentPosList.Clear();
        for (int i = 0; i < 4; i++)
        {
            curStudentPosList.Add(0);
        }
        for (int i = 0; i < trans_UpStudentParent.Count; i++)
        {
            ClearCertainParentAllSingle<SingleViewBase>(trans_UpStudentParent[i]);
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
