using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;

public class StudentEventPanel : PanelBase
{
    public PeopleData p1;
    public PeopleData p2;
    public Portrait porTrait1;
    public Portrait porTrait2;

    public StudentEventType studentEventType;
    public Text txt;

    List<string> btnStrList = new List<string>();
    List<Action> callBackList = new List<Action>();
    public Transform trans_chooseBtnGrid;
    #region 邪恶叛宗
    int lingShiDeRate = 0;
    #endregion
    public override void Init(params object[] args)
    {
        base.Init(args);
        p1 = args[0] as PeopleData;
        p2 = args[1] as PeopleData;
        studentEventType = (StudentEventType)args[2];
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        btnStrList.Clear();
        callBackList.Clear();
        porTrait1.Refresh(p1);
        porTrait2.Refresh(p2);
        switch (studentEventType)
        {
            //叛宗的
            case StudentEventType.XieEPanZong:
                btnStrList.Add("人各有志，随他们去");
                btnStrList.Add("命令交还灵石，不予追究");
                btnStrList.Add("清理门户！");
                callBackList.Add(BuYuZhuiJiu);
                callBackList.Add(JiaoHuanLingShi);
                callBackList.Add(QingLiMenHu);

                lingShiDeRate = (int)(RandomManager.Next(15, 30) * 0.01f * ItemManager.Instance.FindItemCount((int)ItemIdType.LingShi));
                txt.SetText(p1.name + "与" + p2.name + "二人向往魔道已久，一拍即合，决定一同叛宗离去，并偷走了" + lingShiDeRate + "灵石。");
                break;

        }

        for (int i = 0; i < btnStrList.Count; i++)
        {
          
            PanelManager.Instance.OpenSingle<ChooseBtnView>(trans_chooseBtnGrid, btnStrList[i], callBackList[i], this);
            
        }

    }
    #region 邪恶叛宗事件
    //不予追究
    public void BuYuZhuiJiu()
    {
        DialogData data = new DialogData(p1, "多谢掌门成全");
        List<DialogData> dialogDataList = new List<DialogData>() { data };

        DialogManager.Instance.CreateDialog(dialogDataList, () =>
        {
            YuanSuType yuanSu = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);

            if (p1.talent != (int)StudentTalent.LianGong)
            {
                p1.talent = (int)StudentTalent.LianGong;
                StudentManager.Instance.GenerateLianGongStudent(p1, (Quality)(int)p1.studentQuality, yuanSu);

            }
            if (p2.talent != (int)StudentTalent.LianGong)
            {
                p2.talent = (int)StudentTalent.LianGong;
                StudentManager.Instance.GenerateLianGongStudent(p2, (Quality)(int)p2.studentQuality, yuanSu);
            }
            //减少物品
            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)lingShiDeRate);
            RoleManager.Instance.AddNotedPeople(p1, NotedPeopleType.PanZong);
            RoleManager.Instance.AddNotedPeople(p2, NotedPeopleType.PanZong);
            PanelManager.Instance.ClosePanel(this);

            StudentManager.Instance.RemoveStudent(p1);
            StudentManager.Instance.RemoveStudent(p2);
        });


    }
    //交还灵石
    public void JiaoHuanLingShi()
    {
        YuanSuType yuanSu = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);

        if (p1.talent != (int)StudentTalent.LianGong)
        {
            p1.talent = (int)StudentTalent.LianGong;
            StudentManager.Instance.GenerateLianGongStudent(p1, (Quality)(int)p1.studentQuality, yuanSu);
        }
        if (p2.talent != (int)StudentTalent.LianGong)
        {
            p2.talent = (int)StudentTalent.LianGong;
            StudentManager.Instance.GenerateLianGongStudent(p2, (Quality)(int)p2.studentQuality, yuanSu);
        }
        //减少物品
        //ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)lingShiDeRate);
        RoleManager.Instance.AddNotedPeople(p1, NotedPeopleType.PanZong);
        RoleManager.Instance.AddNotedPeople(p2, NotedPeopleType.PanZong);

        StudentManager.Instance.RemoveStudent(p1);
        StudentManager.Instance.RemoveStudent(p2);
        PanelManager.Instance.ClosePanel(this);

    }
    //清理门户
    public void QingLiMenHu()
    {
        YuanSuType yuanSu = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.End);

        DialogData data = new DialogData(p1, "掌门，得罪了");
        if (p1.talent != (int)StudentTalent.LianGong)
        {
            p1.talent = (int)StudentTalent.LianGong;
            StudentManager.Instance.GenerateLianGongStudent(p1, (Quality)(int)p1.studentQuality, yuanSu);

        }
        if (p2.talent != (int)StudentTalent.LianGong)
        {
            p2.talent = (int)StudentTalent.LianGong;
            StudentManager.Instance.GenerateLianGongStudent(p2, (Quality)(int)p2.studentQuality, yuanSu);
        }
        List<DialogData> dialogDataList = new List<DialogData>() { data };
        DialogManager.Instance.CreateDialog(dialogDataList, () =>
         {
             List<PeopleData> theP1List = new List<PeopleData>();
             for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
             {
                 ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
                 if (onlyId <= 0)
                     continue;
                 if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                     theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
                 else
                     theP1List.Add(StudentManager.Instance.FindStudent(onlyId));
             }
             BattleManager.Instance.StartPanZongBattle(theP1List, new List<PeopleData> { p1, p2 });

         });
        PanelManager.Instance.ClosePanel(this);

    }
    #endregion

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_chooseBtnGrid);
    }

}


/// <summary>
/// 弟子事件
/// </summary>
public enum StudentEventType
{
    None=0,
    XieEPanZong=1,//邪恶叛宗
}
