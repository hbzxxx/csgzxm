using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ShanMenPanel : PanelBase
{

    public Button btn_brush1;//普通刷新
    public Button btn_brush2;//重金刷新
    public Button btn_brush3;//一掷千金

    public Text txt_brushCost;//刷新消耗


    public Transform trans_candidateGrid;//候选弟子

   // public Transform trans_replaceStudentPanel;//更换弟子
    public Transform trans_replaceStudentGrid;//更换弟子父物体

    public Text txt_zhaoMuLingRemain;//招募令剩余
    public Text txt_thisYearBrushRemainNum;//刷新剩余次数
    public Text txt_thisYearCanRecruitStudentLimit;//还可招募人数
    public Button btn_adAddRecruitStudentLimit;//招募令招募
    public Transform trans_zhaoMuLingBrushMatGrid;//招募令需要材料
    public Text txt_todayRecruitStudentLimit;//今日还可招募弟子
    public Text txt_brushTodayRecruitCD;//刷新cd


    public override void Init(params object[] args)
    {
        base.Init(args);

        RegisterEvent(TheEventType.SuccessRecruit, OnSuccessRecruit);
        RegisterEvent(TheEventType.GeneratedNewStudent, ShowCandidateStudents);
        RegisterEvent(TheEventType.AddedRecruitStudentNumLimit, ShowCandidateStudents);

  


        addBtnListener(btn_brush1, () =>
         {
             StudentManager.Instance.MoneyBrushStudents();
         });

        addBtnListener(btn_adAddRecruitStudentLimit, () =>
        {
            StudentManager.Instance.ADBrushStudents();
            //StudentManager.Instance.ADAddRecruitStudentLimit();
        });
        
        //addBtnListener(btn_brush2, () =>
        //{
        //    StudentManager.Instance.GenerateNewCandidateStudents(1);
        //});
        //addBtnListener(btn_brush3, () =>
        //{
        //    StudentManager.Instance.GenerateNewCandidateStudents(2);
        //});
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        int zongMenBigLevel = (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel - 1) / 10 + 1;
        long cost =- 10000 * zongMenBigLevel;
        txt_brushCost.SetText(cost.ToString());

  
        ShowCandidateStudents();
        //trans_replaceStudentPanel.gameObject.SetActive(false);

    }
    
    /// <summary>
    /// 显示可招募的弟子
    /// </summary>
    void ShowCandidateStudents()
    {
        //PanelManager.Instance.CloseAllSingle(trans_candidateGrid);
        ClearCertainParentAllSingle<CandidateStudentView1>(trans_candidateGrid);
        ClearCertainParentAllSingle<SingleConsumeView>(trans_zhaoMuLingBrushMatGrid);
        AddSingle<SingleConsumeView>(trans_zhaoMuLingBrushMatGrid, (int)ItemIdType.ZhaoMuLing, 1, ConsumeType.Item);


        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent[i];
            AddSingle<CandidateStudentView1>(trans_candidateGrid, p,RecruitStudentType.ShanMen);
        }
        txt_thisYearBrushRemainNum.SetText("刷新次数：" + RoleManager.Instance._CurGameInfo.studentData.thisYearBrushStudentNum
            + "/" + 3);
        txt_thisYearCanRecruitStudentLimit.SetText("今年还可招募：" + RoleManager.Instance._CurGameInfo.studentData.thisYearRecruitedStudentNum + "/" +
            RoleManager.Instance._CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum);

        txt_todayRecruitStudentLimit.SetText("招募上限：" + RoleManager.Instance._CurGameInfo.studentData.todayRecruitStudentNum + "/" +
            ConstantVal.maxStudentRecruitNumPerDay);

        txt_zhaoMuLingRemain.SetText("招募令剩余：" + ItemManager.Instance.FindItemCount((int)ItemIdType.ZhaoMuLing));
        BrushCDShow();
    }
    void BrushCDShow()
    {
        long guardNextTimeDistance = ConstantVal.hourBeforeNextStudent*60*60;
        long theNextTimeStamp = guardNextTimeDistance + RoleManager.Instance._CurGameInfo.studentData.lastRecruitStudentTime;
        long nowToNextTimeDistance = theNextTimeStamp - CGameTime.Instance.GetTimeStamp();
        long hour = nowToNextTimeDistance / 3600;
        long min = (nowToNextTimeDistance - hour * 3600) / 60;
        long sec = nowToNextTimeDistance - hour * 3600 - min * 60;
        txt_brushTodayRecruitCD.SetText("下次刷新：" + hour + "时" + min + "分");
    }
    /// <summary>
    /// 成功招募
    /// </summary>
    void OnSuccessRecruit(object[] args)
    {
        //PanelManager.Instance.ClosePanel(this);
        ShowCandidateStudents();
    }


  

}
