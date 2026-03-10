using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedMainMapUIPanel : PanelBase
{

    public Transform trans_role;//角色
    public Transform trans_student;//弟子

    public Button btn_leave;//离开
    public override void Init(params object[] args)
    {
        base.Init(args);
        RegisterEvent(TheEventType.LevelResult, RefreshShow);
        RegisterEvent(TheEventType.OnEquip, OnEquip);
        RegisterEvent(TheEventType.OnUnEquip, OnEquip);


        btn_leave.gameObject.SetActive(false);
        addBtnListener(btn_leave, () =>
        {
            MapManager.Instance.LeaveFixedMap();
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        RefreshShow();
         
    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="param"></param>
    void OnEquip(object[] param)
    {
        RefreshShow();
    }

    void RefreshShow()
    {
        ShowRole();
        ShowStudent();
        btn_leave.gameObject.SetActive(false);


    }
    void ShowRole()
    {
        //显示角色
        PanelManager.Instance.CloseAllSingle(trans_role);
        PanelManager.Instance.OpenSingle<SingleStudentView>(trans_role, RoleManager.Instance._CurGameInfo.playerPeople);
    }

    void ShowStudent()
    {    //显示弟子
        PanelManager.Instance.CloseAllSingle(trans_student);

        List<PeopleData> studentList = RoleManager.Instance.FindMyBattleTeamList(false, 0);

        for (int i = 0; i < studentList.Count; i++)
        {
            PanelManager.Instance.OpenSingle<SingleStudentView>(trans_student, studentList[i]);
        }

    }



    /// <summary>
    /// 打赢了报告帝姝
    /// </summary>
    public void OnReportDiShu()
    {
        List<DialogData> dialogList = new List<DialogData>();
        DialogData data = new DialogData(TaskManager.Instance.FindNPCById((int)NPCIDType.DiShu).PeopleData, RoleManager.Instance._CurGameInfo.playerPeople.name+"，对不起……");
        DialogData data1 = new DialogData(null, "？？？");

        DialogData data2 = new DialogData(null, "帝殊突然朝你发动了攻击！！");

        dialogList.Add(data);
        dialogList.Add(data1);
        dialogList.Add(data2);

        DialogManager.Instance.CreateDialog(dialogList, () =>
        {
            BattleManager.Instance.StartDiShuFirstBattle(TaskManager.Instance.FindNPCById((int)NPCIDType.DiShu).PeopleData.onlyId);
        });

    }

    /// <summary>
    /// 相信山海宗掌门
    /// </summary>
    public void BelieveZhangMen()
    {
        List<DialogData> dialogList = new List<DialogData>();

        SingleNPCData diShu = TaskManager.Instance.AddNPC(DataTable.FindNPCArrById((int)NPCIDType.DiShu));

        DialogData data1 = new DialogData(diShu.PeopleData, "看来，你不是我要找的人。");
        DialogData data2 = new DialogData(null, "帝殊突然朝你发动了攻击！！");

        //DialogData data2 = new DialogData(TaskManager.Instance.FindNPCById((int)NPCIDType.ShenMiShaoNv).PeopleData, "看来，你不是我要找的人。");
        dialogList.Add(data1);
        dialogList.Add(data2);

        //看来你已经
        //    但我不得
        DialogManager.Instance.CreateDialog(dialogList, () =>
        {
            BattleManager.Instance.StartDiShuFirstBattle(diShu.PeopleData.onlyId);
        });
    }

    /// <summary>
    /// 相信帝姝
    /// </summary>
    public void BelieveDiShu()
    {
        SingleNPCData diShu = TaskManager.Instance.AddNPC(DataTable.FindNPCArrById((int)NPCIDType.DiShu));

        List<DialogData> dialogList = new List<DialogData>();
        DialogData data1 = new DialogData(null, "你对云海宗掌门摆出战斗的架势，云海宗掌门的身形在滚滚黑烟中撕裂，竟现出了一只巨大的狸猫人！");
        EnemySetting enemySetting = DataTable.FindEnemySetting((int)EnemyIdType.LiMaoZhangMen);
        PeopleData p = BattleManager.Instance.GenerateEnemy(enemySetting,1, enemySetting.Level.ToInt32());
        DialogData data2 = new DialogData(p, "嗷——不识好歹的小子！纳命来！");
        dialogList.Add(data1);
        dialogList.Add(data2);
        DialogManager.Instance.CreateDialog(dialogList, () =>
        {
            BattleManager.Instance.StartLiMaoZhangMenBattle(p);
        });
    }

    /// <summary>
    /// 第二句要说的黑幕话
    /// </summary>
    public void OnsecondBlackWord()
    {

        string content = "";
        //完成该任务
        SingleNPCData data = TaskManager.Instance.FindNPCById((int)NPCIDType.ShenMiShaoNv);
        SingleTaskProtoData taskData = TaskManager.Instance.FindTaskByTagId(data, "10002_12");
        //taskData.AccomplishStatus = (int)AccomplishStatus.GetAward;

        TaskManager.Instance.AccomplishTask(taskData);

        //TaskManager.Instance.ChangeTaskStatus(taskData, AccomplishStatus.GetAward);
        MapManager.Instance.LogicLeaveFixedMap();
        MapManager.Instance.UnlockExploreMap(10000);
        TaskManager.Instance.RemoveNPC(data.OnlyId);
        PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.AfterFirstGuide;
        Action backAction = OnBack;
        
        PanelManager.Instance.OpenPanel<PangBaiPanel>(PanelManager.Instance.trans_layer3, content, backAction);
    }

    
    public void OnBack()
    {
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);
       // PanelManager.Instance.OpenPanel<ChapterPanel>(PanelManager.Instance.trans_layer2, "第一章 狸猫");
        //苏梦岚出现
        SingleNPCData suMengLan = TaskManager.Instance.AddNPC(DataTable.FindNPCArrById((int)NPCIDType.SuMengLan));
        //suMengLan.CurShowScene =(int)SceneType.Mountain;
        //TaskManager.Instance.ShowNPC(suMengLan);

    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_role);
        PanelManager.Instance.CloseAllSingle(trans_student);
    }
}
