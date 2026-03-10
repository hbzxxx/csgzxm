using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using System.Linq;
using Framework.Data;
using System;
using cfg;
 
public class TaskManager : MonoInstance<TaskManager>
{
    public TaskGuidePanel taskGuidePanel;//手指

    //public SingleNPCData curShowNPCData;//当前显示哪个npc
    //public SingleTask curTaskSetting;//当前任务
    //找npc出现的时机

    //public SingleTaskProtoData curTaskData;//当前执行的任务

    public ulong curGuideNPCOnlyId;//当前引导的npc
    public SingleNPCData curMainlineNPCData;//当前主线npc
    public int npcTotalDialogNum;//npc总共多少对话
    public int curNPCDialogIndex;//当前npc对话到哪条了
    public SingleNPCData curDialogNPCData;//当前谁在对话
    public SingleTaskProtoData curDialogTaskData;//当前对话的任务
    public bool triggerGuide_Equip;//触发引导-装备
    public ulong curGuideItemOnlyId;//当前任务指引物品的唯一id

    #region 特殊
    public bool chooseValidStudentZuoZhen = false;
    public ulong chooseValidStudentZuoZhenFarmOnlyId = 0;//哪个房
    public StudentTalent chooseValidStudentZuoZhenTalent = StudentTalent.None;

    public bool danFarmUpgrade = false;//引导丹房升级
    public ulong danFarmUpgradeOnlyId = 0;//哪个房升级

    public bool guide_buildFarm = false;//引导建丹房
    public int guide_buildFarmId = 0;//引导建哪个房的id

    public bool guide_makeEquip = false;//引导做装备
    public int guide_makeEquipId = 0;

    public bool guide_intenseEquip = false;//引导强装备
    public int guide_intenseEquipId = 0;

    public bool guide_makeGem = false;//引导炼宝石


    public bool guide_mapEvent = false;//参与一次灵气修炼事件
    public ulong guide_mapEventOnlyId = 0;

    public bool guide_lianDan = false;//引导炼丹
    public int guide_lianDanId = 0;

    public bool guide_studySkill = false;//引导学习技能
    public int guide_studySkillId = 0;//学什么技能

    public bool guide_equipSkill = false;//引导装备技能
    public int guide_equipSkillId = 0;//装备什么技能

    public bool guide_upgradeSkill = false;//引导升级技能
    public int guide_upgradeSkillId = 0;//升级什么技能

    public bool guide_passFixLevel = false;//通过关卡
    public int guide_passFixLevelMapId = 0;//通过哪一关地图
    public string guide_passFixlevelTag = "";//哪一关

    public bool guide_shangZhen = false;//引导上阵

    public bool guide_equipEquip = false;//引导装备法器
    public bool guide_studentTuPo = false;//引导弟子突破
    public bool guide_studentJueXing = false;//引导弟子觉醒

    public bool guide_unlockPos = false;//解锁空地
    #endregion

    public override void Init()
    {
         base.Init();
        for(int i=0;i< RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
            SingleNPCData data = FindNPCByOnlyId(onlyId);
            NPC setting = DataTable.FindNPCArrById(data.Id);
            if (setting.tasks != null)
            {
                for(int j = 0; j < setting.tasks.Count; j++)
                {
                    setting.tasks[j].index = j;
                }
            }
            if (setting.npcType == NPCType.None)
            {
                curMainlineNPCData = data;
                //主线任务删除了，进下一个任务
                if (FindTaskById(curMainlineNPCData, curMainlineNPCData.CurTaskId) == null)
                {
                    OnDisappearNPC(curMainlineNPCData.OnlyId);
                }
                else
                {
                    //老存档兼容：已显示的主线NPC，检查当前任务是否需要感悟值满条件
                    SingleTask curTaskSetting = FindTaskSettingById(setting, data.CurTaskId);
                    if (curTaskSetting != null && curTaskSetting.taskType == TaskType.TianFuJueXingNum)
                    {
                        EnsureStudentExpFull();
                    }
                }
            }
        }
    }
    //每周调用一次
    public void CheckIfNPCAppear()
    {
        

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData npcData = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            NPC NPCSOSetting = DataTable.FindNPCArrById(npcData.Id);
             
            //主线npc只能出现一个
            if (NPCSOSetting.npcType == NPCType.None&& !RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(npcData.OnlyId))
            {
                for (int j = 0; j < npcData.AllTaskList.Count; j++)
                {
                    //如果有满足需求且未做完的一次性任务，或者满足需求的多次任务
                    SingleTaskProtoData taskProtoData = npcData.AllTaskList[j];
                    SingleTask taskSOSetting = NPCSOSetting.tasks[j];
                    if (taskSOSetting.block)
                        continue;
                    if (taskSOSetting.taskRepeatType == TaskRepeatType.Once
                        && taskProtoData.AccomplishStatus == (int)AccomplishStatus.GetAward)
                        continue;
                    if (CheckIfSatisfyCondition(npcData, taskSOSetting))
                    {
                        //如果是锁定的 则解锁
                        if (taskProtoData.AccomplishStatus == (int)AccomplishStatus.Locked)
                        {
                            ChangeTaskStatus(taskProtoData, AccomplishStatus.UnAccomplished);
                            //taskProtoData.AccomplishStatus = (int)AccomplishStatus.UnAccomplished;

                        }
                        //未完成或进行中
                        if (taskProtoData.AccomplishStatus == (int)AccomplishStatus.UnAccomplished
                            || taskProtoData.AccomplishStatus == (int)AccomplishStatus.Processing)
                        {
                            //如果是重复任务，且未完成，则看是不是满足时间到的条件

                            if (taskSOSetting.taskRepeatType == TaskRepeatType.Repeat)
                            {
                                //检查时间对不对(单位分钟
                                long lastTime = taskProtoData.LastAccomplishTime ;
                                //时间对了
                                if ((CGameTime.Instance.GetTimeStamp() - lastTime) >= taskSOSetting.repeatTime * 60)
                                {
                                    //服务器验证时间对不对
                                    GameTimeManager.Instance.GetServiceTime((x =>
                                    {
                                        if (x > 0)
                                        {
                                            //服务器时间也对 激活任务
                                            if (x - lastTime >= taskSOSetting.repeatTime * 60)
                                            {
                                                ChoosedTask(taskSOSetting, taskProtoData, npcData);
                                                return;
                                            }
                                        }
                                      
                                    }));
                                }


                            }
                            //非重复性任务，直接ok
                            else
                            {
                                ChoosedTask(taskSOSetting, taskProtoData, npcData);
                                return;

                            }
                        }

                    }
                }
            }
            //支线npc
            else if(NPCSOSetting.npcType == NPCType.BranchLine && !RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(npcData.OnlyId))
            {
                for (int j = 0; j < npcData.AllTaskList.Count; j++)
                {
                    //如果有满足需求且未做完的一次性任务，或者满足需求的多次任务
                    SingleTaskProtoData taskProtoData = npcData.AllTaskList[j];
                    SingleTask taskSOSetting = NPCSOSetting.tasks[j];
                    if (taskSOSetting.block)
                        continue;
                    if (taskSOSetting.taskRepeatType == TaskRepeatType.Once
                        && taskProtoData.AccomplishStatus == (int)AccomplishStatus.GetAward)
                        continue;
                    if (CheckIfSatisfyCondition(npcData, taskSOSetting))
                    {
                        //如果是锁定的 则解锁
                        if (taskProtoData.AccomplishStatus == (int)AccomplishStatus.Locked)
                        {
                            ChangeTaskStatus(taskProtoData, AccomplishStatus.UnAccomplished);
                            //taskProtoData.AccomplishStatus = (int)AccomplishStatus.UnAccomplished;

                        }
                        //未完成或进行中
                        if (taskProtoData.AccomplishStatus == (int)AccomplishStatus.UnAccomplished
                            || taskProtoData.AccomplishStatus == (int)AccomplishStatus.Processing)
                        {
                            //如果是重复任务，且未完成，则看是不是满足时间到的条件

                            if (taskSOSetting.taskRepeatType == TaskRepeatType.Repeat)
                            {
                                //检查时间对不对(单位分钟
                                long lastTime = taskProtoData.LastAccomplishTime ;
                                //时间对了
                                if ((CGameTime.Instance.GetTimeStamp() - lastTime) >= taskSOSetting.repeatTime * 60)
                                {
                                    //服务器验证时间对不对
                                    GameTimeManager.Instance.GetServiceTime((x =>
                                    {
                                        if (x > 0)
                                        {
                                            //服务器时间也对 激活任务
                                            if (x - lastTime >= taskSOSetting.repeatTime * 60)
                                            {
                                                ChoosedTask(taskSOSetting, taskProtoData, npcData);
                                                return;
                                            }
                                        }
                                      
                                    }));
                                }


                            }
                            //非重复性任务，直接ok
                            else
                            {
                                ChoosedTask(taskSOSetting, taskProtoData, npcData);
                                return;

                            }
                        }
                    }
                }
            }
            ////神算子
            //else if (NPCSOSetting.npcType == NPCType.ShenSuanZi && !RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(npcData.OnlyId))
            //{
            //    if (npcData.NextAppearTime.Count > 0)
            //    {
            //        int year = npcData.NextAppearTime[0];
            //        int moon = npcData.NextAppearTime[1];
            //        int week = npcData.NextAppearTime[1];

            //        //应该出现
            //        if (GameTimeManager.Instance.GetWeekNumFromTheTimeToNow(year, moon, week) >= 0)
            //        {
                       
            //        }
            //    }
            //    else
            //    {
            //        //从未出现过 先直接出现 TODO过完引导后有
            //        SingleNPCData data = FindNPCById((int)NPCIDType.ShenSuanZi);
            //        data.CurShowScene = (int)SceneType.OutsideMap;
            //        data.LocalPos.Add(100);
            //        data.LocalPos.Add(100);
            //        ShowNPC(data);
            //    }
            //}
        

        }
    }
    /// <summary>
    /// 选中该任务
    /// </summary>
    public void ChoosedTask(SingleTask taskSOSetting,SingleTaskProtoData taskProtoData,SingleNPCData npcData)
    {
        //curTaskSetting = taskSOSetting;
        //curTaskData = taskProtoData;
        
        npcData.CurTaskId = taskProtoData.TheId;
        npcData.CurShowScene =(int)GetSceneByPosType(taskSOSetting.NPCPos);
        //生成npc
        EventCenter.Broadcast(TheEventType.ShowNPC, npcData);
     
        if(!RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(npcData.OnlyId))
        RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Add(npcData.OnlyId);

        NPC npcSetting = DataTable.FindNPCArrById(npcData.Id);
        SingleTask curTaskSetting = FindTaskSettingById(npcSetting, npcData.CurTaskId);
        if (npcSetting.npcType == NPCType.None)
        {
            curMainlineNPCData = npcData;
        }
        //选中了
        //如果有指引 启动任务指引
        if (curTaskSetting.NPCPos != NPCAppearPosType.None)
        {
            npcData.CurShowScene =(int)GetSceneByPosType(curTaskSetting.NPCPos);
            //StartGuide(npcData.OnlyId);
        }
        //如果需要提示
        if (taskSOSetting.ifAppearInform)
        {
            List<DialogData> dialogList = new List<DialogData>();
            dialogList.Add(new DialogData(null, taskSOSetting.appearTxt));
            DialogManager.Instance.CreateDialog(dialogList, null);
            //ShowGuide();
        }
        //特殊任务特殊处理
        switch (taskSOSetting.taskType)
        {
            case TaskType.ZhaoMuDiZi:
                if (RoleManager.Instance._CurGameInfo.studentData.recruitCandidateStudent.Count == 0)
                {
                    StudentManager.Instance.RdmGenerate3CandidateStudents(GenerateCandidateStudentType.AD);
                }

                break;
            //天赋觉醒任务激活时，确保有弟子感悟值满
            case TaskType.TianFuJueXingNum:
                EnsureStudentExpFull();
                break;
        }
        //
        //试图完成任务
        TryAccomplishTask(npcData);
    }

    /// <summary>
    /// 确保有弟子感悟值满（用于天赋觉醒任务激活时）
    /// 检查是否有未觉醒弟子感悟值>=120，没有则让第一个未觉醒弟子感悟值满
    /// </summary>
    void EnsureStudentExpFull()
    {
        //检查是否已有感悟值满的未觉醒弟子
        bool hasExpFullStudent = false;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.None && p.studentCurExp >= 120)
            {
                hasExpFullStudent = true;
                break;
            }
        }
        //没有则让第一个未觉醒弟子感悟值满
        if (!hasExpFullStudent && RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count > 0)
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
            {
                PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                if (p.talent == (int)StudentTalent.None)
                {
                    StudentManager.Instance.FullExp(p);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 增加npc
    /// </summary>
    public SingleNPCData AddNPC(NPC setting)
    {
        SingleNPCData singleNPCData = new SingleNPCData();
        singleNPCData.Id = setting.id;
        singleNPCData.Name = setting.Name;
        PeopleData p = new PeopleData();
        if (!string.IsNullOrWhiteSpace(setting.porTraitName))
        {
            p.specialPortrait = true;
            p.specialPortraitName = setting.porTraitName;
        }
        singleNPCData.PeopleData = p;
        //if (setting.npcType == NPCType.Enemy)
        //{
            singleNPCData.PeopleData.enemySettingId =(int)setting.enemyId;
        //}
        RoleManager.Instance.InitNPCPro(singleNPCData);
        //初始化属性
        
        p.onlyId = ConstantVal.SetId;
        p.gender = (int)setting.gender;
        p.name = setting.Name;
        PeopleData neiCunP = p;
        RoleManager.Instance.RdmFace(neiCunP);
        p.portraitIndexList.Clear();
        for(int i = 0; i < neiCunP.portraitIndexList.Count; i++)
        {
            p.portraitIndexList.Add(neiCunP.portraitIndexList[i]);
        }
        p.portraitType =(int)PortraitType.ChangeFace;

        singleNPCData.OnlyId = p.onlyId;
        if (setting.tasks != null)
        {
            for (int j = 0; j < setting.tasks.Count; j++)
            {
                SingleTask taskSetting = setting.tasks[j];
                SingleTaskProtoData taskData = new SingleTaskProtoData();
                taskData.TaskIndex = j;
                taskData.LastAccomplishTime = 0;
                taskData.TheId = taskSetting.theId;
                taskData.AccomplishStatus = (int)AccomplishStatus.Locked;
                taskData.NpcId = setting.id;
                singleNPCData.AllTaskList.Add(taskData);

            }
        }
        if(setting.smallPeopleTextureName!=null)
        singleNPCData.SmallPeopleTextureName = setting.smallPeopleTextureName;
       RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Add(singleNPCData);
        return singleNPCData;
    }
    /// <summary>
    /// npc出现
    /// </summary>
    public void ShowNPC(SingleNPCData nPCData)
    {
        if (!RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(nPCData.OnlyId))
        {
            RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Add(nPCData.OnlyId);
            EventCenter.Broadcast(TheEventType.ShowNPC, nPCData);

        }
    }

    /// <summary>
    /// 点击了npc
    /// </summary>
    public void OnClickedNPC(SingleNPCData singleNPCData)
    {

        NPC npcSetting = DataTable.FindNPCArrById(singleNPCData.Id);

        if(npcSetting.npcType==NPCType.None
            ||npcSetting.npcType == NPCType.BranchLine)
        {
            SingleTaskProtoData curTaskData = FindTaskById(singleNPCData, singleNPCData.CurTaskId);
            SingleTask curTaskSetting = FindTaskSettingById(npcSetting, singleNPCData.CurTaskId);

            //现有任务变成进行时 并且俩人对话
            if (curTaskData.AccomplishStatus == (int)AccomplishStatus.UnAccomplished)
            {
                //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                //开始任务指引
                //StartGuide(singleNPCData.OnlyId);
                //ShowGuide();
                List<DialogData> dialogList = new List<DialogData>();
                if (curTaskSetting.dialogList.Count > 0)
                {
                    curDialogTaskData = curTaskData;
                    npcTotalDialogNum = curTaskSetting.dialogList.Count;
                    curDialogNPCData = singleNPCData;
                    for (int i = curNPCDialogIndex; i < curTaskSetting.dialogList.Count; i++)
                    {
                        DialogEditorData dialogEditorData = curTaskSetting.dialogList[i];
                        PeopleData p = null;
                
                        switch (dialogEditorData.type)
                        {
                            case TaskDialogBelongType.Self:
                                p = singleNPCData.PeopleData;
                                break;
                            case TaskDialogBelongType.None:
                                p = null;
                                break;
                            case TaskDialogBelongType.Player:
                                p =RoleManager.Instance._CurGameInfo.playerPeople;
                                break;
                        }
                        DialogData dialogData = new DialogData(p, dialogEditorData.content);
                        dialogList.Add(dialogData);
                        curNPCDialogIndex++;

                        //如果玩家需要回复，则在这里中断
                        if (dialogEditorData.ifPlayerAnswer)
                        {
                            List<Action> afterAnswerActionList = new List<Action>();
                            for(int j = 0; j < dialogEditorData.answerList.Count; j++)
                            {
                                afterAnswerActionList.Add(AfterAnswerAction);

                            }
                            DialogManager.Instance.CreateDialog(dialogList,
                                    dialogEditorData.answerList, afterAnswerActionList
                                    );
                            
                            return;
                        }
                        
                    }
                 
                }
                if (dialogList.Count > 0)
                {  
                  
                    
                        //对话完打开任务面板
                        DialogManager.Instance.CreateDialog(dialogList, () =>
                        {   
                            ////是否取名任务
                            //if (curTaskSetting.tagId == "10002_1")
                            //{
                            //    PanelManager.Instance.OpenPanel<SetNamePanel>(PanelManager.Instance.trans_layer2);

                            //}else
                            //{
                            
                            //}
                            curNPCDialogIndex = 0;
                            TDGAMission.OnBegin(curTaskSetting.trackingName);
                            ChangeTaskStatus(curTaskData, AccomplishStatus.Processing);
                            //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                            if (!string.IsNullOrWhiteSpace(curTaskSetting.des))
                                PanelManager.Instance.OpenPanel<NPCTaskPanel>(PanelManager.Instance.trans_layer2, singleNPCData);

                        });

                    
              
                }
                else
                {  
                
                   
                        curNPCDialogIndex = 0;
                     TDGAMission.OnBegin(curTaskSetting.trackingName);
                    ChangeTaskStatus(curTaskData, AccomplishStatus.Processing);
                    //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                    if (!string.IsNullOrWhiteSpace(curTaskSetting.des))
                        PanelManager.Instance.OpenPanel<NPCTaskPanel>(PanelManager.Instance.trans_layer2, singleNPCData);
                    
       

                }
                //引导去山下修炼
                if (curTaskSetting.tagId == "10002_19")
                {
                    MapEventManager.Instance.AddMapEventAtIndex((int)MapEventIdType.LingQi, SceneType.OutsideMap, 0);
                    //MapEventManager.Instance.AddMapEventAtIndex((int)MapEventIdType.LingQi, SceneType.OutsideMap, 8);

                }
                ////引导炼丹，解锁炼丹炉
                //if (curTaskSetting.tagId == "10002_8")
                //{
                //    LianDanManager.Instance.UnlockDanFarm((int)DanFarmIdType.LianDanLu);
                //}
                //引导-学习技能
                if (curTaskSetting.tagId == "10002_10")
                {   
                    ItemManager.Instance.GetItemWithTongZhiPanel( DataTable.FindCanStudySkillListByYuanSu((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu)[0].Id.ToInt32() ,  1 );

                }

            }
            //进行中，弹出面板
            else if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Processing)
            {
                if(!string.IsNullOrWhiteSpace(curTaskSetting.des))
                PanelManager.Instance.OpenPanel<NPCTaskPanel>(PanelManager.Instance.trans_layer2,singleNPCData);
            }
            //已完成 则完成对话，并给奖励
            else if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
            {
                string tagId = curTaskSetting.tagId;

                curTaskData.CurNum = 0;

                //杀怪拿东西
                if (curTaskSetting.taskType == TaskType.KillMonsterGetItem
                    || curTaskSetting.taskType == TaskType.ReceiveItem
                    ||curTaskSetting.taskType==TaskType.LianDan)
                {
                    if (curTaskSetting.needReceiveItem)
                    {
                        int needId = (int)curTaskSetting.needItem;
                        int needNum = curTaskSetting.needNum;
                        ItemManager.Instance.LoseItem(needId, (ulong)needNum);
                    }

                }
                List<ItemData> award = new List<ItemData>();
                foreach(string str in curTaskSetting.awardList)
                {
                    string[] arr = str.Split('|');
                    ItemData singleAward = new ItemData();
                    singleAward.settingId = arr[0].ToInt32();
                    singleAward.count = arr[1].ToUInt64();
                    award.Add(singleAward);

                    PanelManager.Instance.AddTongZhi(TongZhiType.Consume, "",ConsumeType.Item, singleAward.settingId, (int)(int)(ulong)singleAward.count);
                    ItemManager.Instance.GetItem(singleAward.settingId, singleAward.count);

                }

                List<DialogData> dialogList = new List<DialogData>();
                if (curTaskSetting.dialogListAfter.Count > 0)
                {
                    curDialogTaskData = curTaskData;
                    npcTotalDialogNum = curTaskSetting.dialogListAfter.Count;
                    curDialogNPCData = singleNPCData;
                    for (int i = curNPCDialogIndex; i < curTaskSetting.dialogListAfter.Count; i++)
                    {
                        DialogEditorData dialogEditorData = curTaskSetting.dialogListAfter[i];
                        PeopleData p = null;
                        switch (dialogEditorData.type)
                        {
                            case TaskDialogBelongType.Self:
                                p = singleNPCData.PeopleData;
                                break;
                            case TaskDialogBelongType.None:
                                p = null;
                                break;
                            case TaskDialogBelongType.Player:
                                p =RoleManager.Instance._CurGameInfo.playerPeople;
                                break;
                        }
                        DialogData dialogData = new DialogData(p, dialogEditorData.content);
                        dialogList.Add(dialogData);

                        curNPCDialogIndex++;
                        //如果玩家需要回复，则在这里中断
                        if (dialogEditorData.ifPlayerAnswer)
                        {
                            List<Action> afterAnswerActionList = new List<Action>();
                            for (int j = 0; j < dialogEditorData.answerList.Count; j++)
                            {
                                afterAnswerActionList.Add(AfterAccomplishDialogAnswerAction);

                            }
                            DialogManager.Instance.CreateDialog(dialogList,
                                    dialogEditorData.answerList, afterAnswerActionList
                                    );

                            return;
                        }
                    }


                }
                if (dialogList.Count > 0)
                {

                    //对话完打开奖励面板 并结束任务
                    DialogManager.Instance.CreateDialog(dialogList, () =>
                    {
                        curNPCDialogIndex = 0;
                        //是否给感悟值 TODO前期不能裁员
                        if (tagId == "10002_14")
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[0];
                            StudentManager.Instance.FullExp(p);
                            CheckIfNPCAppear();
                        }

                        //给技能材料
                        if (tagId == "10002_10")
                        {
                            int id= ConstantVal.NianZhuIdByYuanSu((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu);
                            ItemManager.Instance.GetItemWithTongZhiPanel(id, 10);

                        }

                    });
                }

                else
                {
                    curNPCDialogIndex = 0;

                    //PanelManager.Instance.OpenPanel<GetAwardPanel>(PanelManager.Instance.trans_layer2, award );
                    TaskManager.Instance.OnDisappearNPC(singleNPCData.OnlyId);
                }

                //RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Remove(singleNPCData.OnlyId);
                ChangeTaskStatus(curTaskData, AccomplishStatus.GetAward);
                //curTaskData.AccomplishStatus = (int)AccomplishStatus.GetAward;
                TryRecordPassedTaskTagId(curTaskSetting.tagId);
                //记录一下完成时间 这里改成读本地时间
                SingleTaskProtoData saveTimeTaskProtoData = curTaskData;
                saveTimeTaskProtoData.LastAccomplishTime = -1;

                GameTimeManager.Instance.GetServiceTime((x) =>
                {
                    //没能正确得到服务器时间
                    if (x == -1)
                    {
                        Debug.Log("没能正确得到服务器时间");
                    }
                    saveTimeTaskProtoData.LastAccomplishTime = x ;


                });
                if (curTaskSetting.taskRepeatType == TaskRepeatType.Repeat)
                {
                    ChangeTaskStatus(curTaskData, AccomplishStatus.Locked);
                   // curTaskData.AccomplishStatus = (int)AccomplishStatus.Locked;
                }
                //如果可重复，则变成未完成
                //curTaskData.LastAccomplishTime = RoleManager.Instance._CurGameInfo.timeData.Year + "|" + RoleManager.Instance._CurGameInfo.timeData.Month + "|" + RoleManager.Instance._CurGameInfo.timeData.Day;
                CGameTime.Instance.GetTimeStamp();

                CloseTaskGuide(curTaskSetting.taskType);
                curTaskData = null;
                curTaskSetting = null;

                //if(tagId== "10002_6")
                //{
                //    SpecialGuide_EquipTeach();
                //}
                TaskManager.Instance.OnDisappearNPC(singleNPCData.OnlyId);

                CheckIfNPCAppear();
            }
            //拿过奖励 则消失npc或者下一个任务
            else if (curTaskData.AccomplishStatus== (int)AccomplishStatus.GetAward)
            {
                OnDisappearNPC(singleNPCData.OnlyId);
            }
        }
        //算命
        else if (npcSetting.npcType == NPCType.ShenSuanZi)
        {

        }
   

    }

    #region 特殊-穿装备引导
    public void SpecialGuide_EquipTeach(ulong onlyId)
    {
        curGuideItemOnlyId = onlyId;
        string path1 = "Canvas/Panel/Layer2/MainPanel/btn_knapsack";
        GameObject obj = GameObject.Find(path1);
        if (obj == null)
            return;
        PanelManager.Instance.ShowTaskGuidePanel(obj);
        //string path2= "Canvas/Panel/Layer2/PlayerPanel/PlayerPeoplePanel/sonPanelParent/";
    }
    #endregion
    //#region 特殊-学技能引导
    //public void SpecialGuide_EquipTeach(ulong onlyId)
    //{
    //    curGuideItemOnlyId = onlyId;
    //    string path1 = "Canvas/Panel/Layer2/MainPanel/btn_knapsack";
    //    GameObject obj = GameObject.Find(path1);
    //    PanelManager.Instance.ShowTaskGuidePanel(obj.transform.position);

    //    //string path2= "Canvas/Panel/Layer2/PlayerPanel/PlayerPeoplePanel/sonPanelParent/";
    //}
    //#endregion

    /// <summary>
    /// 完成任务 回答以后
    /// </summary>
    public void AfterAccomplishDialogAnswerAction()
    {
        OnClickedNPC(curDialogNPCData);

    }

    /// <summary>
    /// 回答以后
    /// </summary>
    public void AfterAnswerAction()
    {
        OnClickedNPC(curDialogNPCData);
        return;
        //如果到底了 直接弹任务面板
        if (curNPCDialogIndex >=npcTotalDialogNum-1)
        {
            curNPCDialogIndex = 0;
            ChangeTaskStatus(curDialogTaskData, AccomplishStatus.Processing);
            //curDialogTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
            PanelManager.Instance.OpenPanel<NPCTaskPanel>(PanelManager.Instance.trans_layer2, curDialogNPCData);
        }
        //还有对话
        else
        {
            EventCenter.Broadcast(TheEventType.CloseCurDialog);
            OnClickedNPC(curDialogNPCData);
        }
    }
    /// <summary>
    /// 改变任务状态
    /// </summary>
    public void ChangeTaskStatus(SingleTaskProtoData taskData, AccomplishStatus accomplishStatus)
    {
        taskData.AccomplishStatus = (int)accomplishStatus;
        EventCenter.Broadcast(TheEventType.ChangeTaskStatus);
    }
    /// <summary>
    /// 任务指引
    /// </summary>
    public void ShowGuide()
    {
        SingleNPCData curGuideNPCData = FindNPCByOnlyId(curGuideNPCOnlyId);
        if (curGuideNPCData == null)
            return;
        SingleTaskProtoData curTaskData = FindTaskById(curGuideNPCData, curGuideNPCData.CurTaskId);
        if (curTaskData == null)
            return;
        NPC npcSetting = DataTable.FindNPCArrById(curGuideNPCData.Id);
        SingleTask curTaskSetting = FindTaskSettingById(npcSetting, curGuideNPCData.CurTaskId);

        if (!curTaskData.StartGuide)
            return;
    
        int guideId= curTaskSetting.TaskGuideId;

        if (NewGuideManager.Instance.newGuideCanvas != null
            && NewGuideManager.Instance.newGuideCanvas.gameObject.activeInHierarchy
            && NewGuideManager.Instance.newGuideCanvas.setting.Id.ToInt32()==guideId)
            return;

        if (PanelManager.Instance.curTaskGuidePanel != null && PanelManager.Instance.curTaskGuidePanel.gameObject.activeInHierarchy)
            PanelManager.Instance.CloseTaskGuidePanel();


        //场景不对 跳场景
        if (curTaskSetting.taskType == TaskType.DanFarmNum
            || curTaskSetting.taskType == TaskType.ZhaoMuStudent
            || curTaskSetting.taskType == TaskType.ZhaoMuDiZi
            || curTaskSetting.taskType == TaskType.StudentZuoZhen
            || curTaskSetting.taskType == TaskType.UpgradeZongMen
            || curTaskSetting.taskType == TaskType.HaveABLevelCFarm
            || curTaskSetting.taskType == TaskType.QuanLiNum
            || curTaskSetting.taskType == TaskType.EquipNum
            || curTaskSetting.taskType == TaskType.UnlockFarmPosNum
            || curTaskSetting.taskType == TaskType.LianDan
             || curTaskSetting.taskType == TaskType.UpgradeEquip
             || curTaskSetting.taskType == TaskType.MakeGem)
        {
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.Mountain)
                GameSceneManager.Instance.GoToScene(SceneType.Mountain);
            //直接打开引导面板
            MountainPanel mountainPanel = GameObject.Find("MountainPanel").GetComponent<MountainPanel>();

            //如果丹田数量不对 引导开个新田 建造中也算完成
            if (curTaskSetting.taskType == TaskType.DanFarmNum)
            {
                MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
                PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_building.gameObject);
                guide_buildFarm = true;
                guide_buildFarmId = guide_buildFarmId = (int)curTaskSetting.needDanFarmId;
    
            }
            //找到对的丹房坐镇
            else if (curTaskSetting.taskType == TaskType.StudentZuoZhen)
            {
                List<int> StudentZuoZhenParamList = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int StudentZuoZhenfarmId = StudentZuoZhenParamList[0];
                SingleFarmView farmView = mountainPanel.FindFarmViewById(StudentZuoZhenfarmId);
                if (farmView == null)
                {
                    PanelManager.Instance.OpenFloatWindow("您尚未建造该建筑");
                    return;
                }
                EventCenter.Broadcast(TheEventType.NevigateToMountainPos, farmView.GetComponent<RectTransform>());

                chooseValidStudentZuoZhenFarmOnlyId = farmView.singleDanFarmData.OnlyId;
                PanelManager.Instance.ShowTaskGuidePanel(farmView.gameObject);

                bool haveTalentCondition = false;//有天赋需求
                StudentTalent needTalent = StudentTalent.None;
                if (StudentZuoZhenParamList.Count >= 3)
                {
                    haveTalentCondition = true;
                    needTalent = (StudentTalent)StudentZuoZhenParamList[2];
                }
                chooseValidStudentZuoZhen = true;
                chooseValidStudentZuoZhenTalent = needTalent;

                return;
            }
            //拥有a个b级c建筑
            else if (curTaskSetting.taskType == TaskType.HaveABLevelCFarm)
            {
                List<int> HaveABLevelCFarmParamList = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int farmId = HaveABLevelCFarmParamList[2];
                int needLevel = HaveABLevelCFarmParamList[1];
                int needHaveABLevelCFarmParamNum = HaveABLevelCFarmParamList[0];
                int validHaveABLevelCFarmParamNum = 0;
                SingleDanFarmData choosedFarm = null;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData danFarm = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    if (danFarm.SettingId == farmId
                        && danFarm.CurLevel < needLevel)
                    {
                        //指它 同时陆续引导到升级
                        choosedFarm = danFarm;
                        break;
                    }

                }
                //引导到空位开个新田
                if (choosedFarm == null)
                {

                    PanelManager.Instance.OpenFloatWindow("您尚未建造" + DataTable.FindDanFarmSetting(farmId).Name + "，请先建造");
                    return;
                }
                else
                {
                    if (choosedFarm.Status == (int)DanFarmStatusType.Upgrading)
                    {
                        PanelManager.Instance.OpenFloatWindow("请等待" + DataTable.FindDanFarmSetting(farmId).Name + "建造或升级完毕");
                        return;
                    }
                    SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(choosedFarm.OnlyId);
                    if (farmView == null)
                    {
                        PanelManager.Instance.OpenFloatWindow("您尚未建造该建筑");
                        return;
                    }
                    EventCenter.Broadcast(TheEventType.NevigateToMountainPos, farmView.GetComponent<RectTransform>());
                    PanelManager.Instance.ShowTaskGuidePanel(farmView.gameObject);
                    danFarmUpgrade = true;
                    danFarmUpgradeOnlyId = choosedFarm.OnlyId;
                    return;
                }
            }
            //加速生产
            else if (curTaskSetting.taskType == TaskType.QuanLiNum)
            {

                SingleDanFarmData choosedFarm = null;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData danFarm = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    if (danFarm.Status == (int)DanFarmStatusType.Working
                        && danFarm.DanFarmWorkType == (int)DanFarmWorkType.Common
                        && !danFarm.OpenQuanLi)
                    {
                        //指它 同时陆续引导到确定加速
                        choosedFarm = danFarm;
                        break;
                    }

                }
                //引导到空位开个新田
                if (choosedFarm == null)
                {
                    PanelManager.Instance.OpenFloatWindow("尚未满足允许全力生产的建筑");
                    return;
                }
                else
                {

                    SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(choosedFarm.OnlyId);
                    if (farmView == null)
                    {
                        PanelManager.Instance.OpenFloatWindow("您尚未建造该建筑");
                        return;
                    }
                    EventCenter.Broadcast(TheEventType.NevigateToMountainPos, farmView.GetComponent<RectTransform>());
                    PanelManager.Instance.ShowTaskGuidePanel(farmView.btn_zone.gameObject);

                    return;
                }
            }
            //拥有法器
            else if (curTaskSetting.taskType == TaskType.EquipNum)
            {
                List<SingleDanFarmData> equipFarmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi);
                if (equipFarmList.Count <= 0)
                {
                    PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有炼器房请先建造一个));
                    return;
                }
                SingleFarmView farmView = mountainPanel.FindFarmViewByOnlyId(equipFarmList[0].OnlyId);
                guide_makeEquip = true;
                guide_makeEquipId = (int)curTaskSetting.needItem;
                mountainPanel.scrollViewNevigation.NevigateImmediately(farmView.GetComponent<RectTransform>());
                PanelManager.Instance.ShowTaskGuidePanel(farmView.gameObject);
                return;
            }

            //解锁空地
            else if (curTaskSetting.taskType == TaskType.UnlockFarmPosNum)
            {
                if (ZongMenManager.Instance.JudgeIfCanUnlockFarm())
                {
                    MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
                    PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_building.gameObject);
                    //for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                    //{
                    //    SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    //    //定位过去
                    //    if (!singleDanFarmData.Unlocked)
                    //    {
                    //        SingleFarmView view = mountainPanel.FindFarmViewByOnlyId(singleDanFarmData.OnlyId);
                    //        mountainPanel.scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                    //        PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                    //        break;
                    //    }
                    //}
                    guide_unlockPos = true;
                }
                else
                {
                    PanelManager.Instance.OpenFloatWindow("还需" + RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum + "灵石");
                }

                return;
            }
            //炼丹
            else if (curTaskSetting.taskType == TaskType.LianDan)
            {
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    //定位过去
                    if (singleDanFarmData.SettingId == (int)DanFarmIdType.LianDanLu)
                    {
                        SingleFarmView view = mountainPanel.FindFarmViewByOnlyId(singleDanFarmData.OnlyId);
                        mountainPanel.scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                        PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                        guide_lianDan = true;
                        guide_lianDanId = (int)curTaskSetting.needItem;
                        break;
                    }
                }
                return;
            }
            //强化法器
            else if (curTaskSetting.taskType == TaskType.UpgradeEquip)
            {
                List<SingleDanFarmData> equipFarmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.LianQi);
                if (equipFarmList.Count <= 0)
                {
                    PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有炼器房请先建造一个));
                    return;
                }
                MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();

                PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
                guide_intenseEquip = true;
                //for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                //{
                //    SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                //    //定位过去
                //    if (singleDanFarmData.SettingId == (int)DanFarmIdType.EquipMake)
                //    {
                //        SingleFarmView view = mountainPanel.FindFarmViewByOnlyId(singleDanFarmData.OnlyId);
                //        mountainPanel.scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                //        PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                //        guide_intenseEquip = true;
                //        break;
                //    }
                //}
                return;
            }
            //炼制宝石
            else if (curTaskSetting.taskType == TaskType.MakeGem)
            {
                List<SingleDanFarmData> farmList = ZongMenManager.Instance.FindTypeFarmList(DanFarmType.BaguaLu);
                if (farmList.Count <= 0)
                {
                    PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.您还没有八卦炉请先建造一个));
                    return;
                }
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    //定位过去
                    if (singleDanFarmData.SettingId == (int)DanFarmIdType.BaGuaLu)
                    {
                        SingleFarmView view = mountainPanel.FindFarmViewByOnlyId(singleDanFarmData.OnlyId);
                        mountainPanel.scrollViewNevigation.NevigateImmediately(view.GetComponent<RectTransform>());
                        PanelManager.Instance.ShowTaskGuidePanel(view.gameObject);
                        guide_makeGem = true;
                        break;
                    }
                }
                return;
            }
        }

        //执行事件 只看山下有没有 山下没有 去地图上找
        else if (curTaskSetting.taskType == TaskType.AccomplishMapEvent)
        {
            List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
            int theMapEventType = mapEventParam[0];
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
            {
                SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
                MapEventSetting mapEventSetting = DataTable.FindMapEventSetting(data.SettingId);
                if (data.MapSceneType == (int)SceneType.OutsideMap
                    && mapEventSetting.Type.ToInt32() == theMapEventType)
                {
                    guide_mapEvent = true;
                    guide_mapEventOnlyId = data.OnlyId;
                    //指向山门
                    if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.OutsideMap)
                    {

                        MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
                        PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_outside.gameObject);
                        break;
                    }
                    //指向灵气
                    else
                    {
                        OutsidePanel outsidePanel = GameObject.Find("OutsidePanel").GetComponent<OutsidePanel>();
                        outsidePanel.ShowGuide();
                        break;
                    }

                }
            }
            return;
        }
        //学习技能
        else if (curTaskSetting.taskType == TaskType.StudySkill)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            guide_studySkill = true;
            guide_studySkillId = curTaskSetting.commonAccomplishCondition.ToInt32();

            return;
        }
        //携带技能
        else if (curTaskSetting.taskType == TaskType.EquipSkill)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            guide_equipSkill = true;
            //携带刚刚学习的技能
            if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count >= 2)
            {
                guide_equipSkillId = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[1].skillId;
            }
            return;
        }
        else if (curTaskSetting.taskType == TaskType.StudentHaveARarityBLevelCNumEquip)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);


            return;
        }
        //升级技能
        else if (curTaskSetting.taskType == TaskType.UpgradeSkill)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            List<int> upgradeSkillParamList = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
            guide_upgradeSkill = true;
            guide_upgradeSkillId = upgradeSkillParamList[0];
            if (guide_upgradeSkillId == 0)
            {
                if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count >= 2)
                {
                    guide_upgradeSkillId = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[1].skillId;
                }
            }
            //给功法升级材料（念珠）
            int nianZhuId = ConstantVal.NianZhuIdByYuanSu((YuanSuType)RoleManager.Instance._CurGameInfo.playerPeople.yuanSu);
            ItemManager.Instance.GetItemWithTongZhiPanel(nianZhuId, 10);
            return;
        }
        //杀云海宗长老
        else if (curTaskSetting.taskType == TaskType.KillEnemy
            && curTaskSetting.needKillEnemy == EnemyIdType.HunHunTouZi)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_outside.gameObject);


            return;
        }
        //通过关卡
        else if (curTaskSetting.taskType == TaskType.PassLevel)
        {
            guide_passFixLevel = true;
            guide_passFixlevelTag = curTaskSetting.commonAccomplishCondition;
            guide_passFixLevelMapId = curTaskSetting.commonAccomplishCondition.Split('-')[0].ToInt32();
            int needMapId = DataTable.FindMapSettingByMapLevel(guide_passFixLevelMapId).Id.ToInt32();
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.FixedMap
                || (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.FixedMap
                && RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId != needMapId))
            {

                if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.WorldMap)
                {
                    WorldMapPanel worldMapPanel = GameObject.Find("WorldMapPanel").GetComponent<WorldMapPanel>();
                    for (int i = 0; i < worldMapPanel.singleMapViewList.Count; i++)
                    {
                        SingleMapView singleMapView = worldMapPanel.singleMapViewList[i];
                        if (singleMapView.mapSetting.MapLevel.ToInt32() == guide_passFixLevelMapId)
                        {
                            PanelManager.Instance.ShowTaskGuidePanel(singleMapView.btn.gameObject);
                        }
                    }

                }
                else if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType == (int)SceneType.OutsideMap)
                {
                    OutsidePanel outsidePanel = GameObject.Find("OutsidePanel").GetComponent<OutsidePanel>();
                    PanelManager.Instance.ShowTaskGuidePanel(outsidePanel.btn_worldEnter.gameObject);

                }
                else
                {
                    MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
                    PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_outside.gameObject);
                }

            }




            return;
        }
        //上阵
        else if (curTaskSetting.taskType == TaskType.ShangZhen)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_team.gameObject);
            guide_shangZhen = true;

            return;
        }
        //装备法器
        else if (curTaskSetting.taskType == TaskType.EquipEquip)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            guide_equipEquip = true;

            return;
        }
        //弟子突破次数
        else if (curTaskSetting.taskType == TaskType.StudentUpgradeCount)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            guide_studentTuPo = true;

            return;
        }
        //弟子觉醒次数
        else if (curTaskSetting.taskType == TaskType.TianFuJueXingNum)
        {
            MainPanel mainPanel = GameObject.Find("MainPanel").GetComponent<MainPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(mainPanel.btn_student.gameObject);
            guide_studentJueXing = true;

            return;
        }
        //改宗门名：指向头像按钮，打开 TrainPanel 后可改名
        else if (curTaskSetting.taskType == TaskType.ChangeZongMenName)
        {
            TopPanel topPanel = GameObject.Find("TopPanel").GetComponent<TopPanel>();
            PanelManager.Instance.ShowTaskGuidePanel(topPanel.btn_headIcon.gameObject);
            return;
        }
        else
        {
            //兜底：未处理的任务类型，打印警告便于排查
            Debug.LogError($"[TaskManager.ShowGuide] 未处理的任务类型: {curTaskSetting.taskType}, tagId: {curTaskSetting.tagId}");
            return;
        }




        if (guideId != 0)
        {
            NewGuideSetting guideSetting = DataTable.FindNewGuideSetting(guideId);

            GameObject tarGet = GameObject.Find(guideSetting.HighLightObjPath);
            if (tarGet != null && tarGet.activeInHierarchy)
            {
                EventCenter.Broadcast(TheEventType.NevigateToMountainPos, tarGet.GetComponent<RectTransform>());

            }
            PanelManager.Instance.OpenNewGuideCanvas(guideSetting);
        }
        else
        {
            curTaskData.StartGuide = false;

        }






    }
    /// <summary>
    /// npc消失
    /// </summary>
    public void OnDisappearNPC(ulong onlyId)
    {
        if (RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(onlyId))
        {
            RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Remove(onlyId);
            EventCenter.Broadcast(TheEventType.DisappearNPC, onlyId);
            if (curMainlineNPCData != null && curMainlineNPCData.OnlyId == onlyId)
            {
                //检查该主线NPC是否还有未完成的任务，只有全部完成才清空
                bool hasUnfinishedTask = false;
                NPC npcSetting = DataTable.FindNPCArrById(curMainlineNPCData.Id);
                if (npcSetting != null && npcSetting.tasks != null)
                {
                    for (int i = 0; i < curMainlineNPCData.AllTaskList.Count; i++)
                    {
                        SingleTaskProtoData taskData = curMainlineNPCData.AllTaskList[i];
                        SingleTask taskSetting = npcSetting.tasks[i];
                        //跳过block的任务
                        if (taskSetting.block)
                            continue;
                        //如果是一次性任务且未领取奖励，说明还有任务未完成
                        if (taskSetting.taskRepeatType == TaskRepeatType.Once
                            && taskData.AccomplishStatus != (int)AccomplishStatus.GetAward)
                        {
                            hasUnfinishedTask = true;
                            break;
                        }
                    }
                }
                //只有所有任务都完成了才清空curMainlineNPCData
                if (!hasUnfinishedTask)
                    curMainlineNPCData = null;
            }
            curNPCDialogIndex = 0;
        }
    }


    //public void CheckIfShowLianDanBuildingGuide()
    //{

    //}


    public SingleTaskProtoData GetSavedTaskData(SingleTaskProtoData singleTaskProtoData)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData singleNPCData = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            for (int j = 0; j < singleNPCData.AllTaskList.Count; j++)
            {
                if (singleTaskProtoData == singleNPCData.AllTaskList[j])
                    return singleNPCData.AllTaskList[j];
            }
        }
        return null;
    }
    /// <summary>
    /// 找是否满足条件
    /// </summary>
    public bool CheckIfSatisfyCondition(SingleNPCData npcData, SingleTask taskSOSetting)
    {
        int curLevel = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex + 1;

        bool satisfy = true;
        if (taskSOSetting.condition != null)
        {
            for (int i = 0; i < taskSOSetting.condition.Count; i++)
            {
                //有一个不满足 就是false
                 TaskCondition taskCondition = taskSOSetting.condition[i];
                TaskConditionType conditionType = taskCondition.taskConditionType;
                 string param = taskCondition.param;

                switch (conditionType)
                {
                    ////练功房弟子必须大于等于多少个
                    //case TaskConditionType.LianGongStudentNum:
                    //    if (RoleManager.Instance._CurGameInfo.StudentData.LianGongStudentList.Count < param.ToInt32())
                    //        satisfy = false;
                    //    break;
                    //玩家等级
                    case TaskConditionType.PlayerTrainLevel:
                        if (curLevel < param.ToInt32())
                            satisfy = false;
                        break;
                    //玩家等级在某范围
                    case TaskConditionType.PlayerTrainLevelRange:
                        List<int> levelRange = CommonUtil.SplitCfgOneDepth(param);
                        if (curLevel > levelRange[1]
                            || curLevel < levelRange[0])
                            satisfy = false;
                        break;
                    //完成了上个任务
                    case TaskConditionType.AccomplishedLastTask:
                        int theTaskIndex = -1;
                        for (int j = 0; j < npcData.AllTaskList.Count; j++)
                        {
                            SingleTaskProtoData theTaskProtoData = npcData.AllTaskList[j];
                            if (theTaskProtoData.TheId == taskSOSetting.theId)
                            {
                                theTaskIndex = j;
                                break;
                            }
                        }
                        if (theTaskIndex > 0)
                        {
                            // 获取NPC配置，用于检查任务是否block
                            NPC npcSetting = DataTable.FindNPCArrById(npcData.Id);
                            // 向前查找最近的非block任务
                            int prevTaskIndex = theTaskIndex - 1;
                            while (prevTaskIndex >= 0 && npcSetting.tasks[prevTaskIndex].block)
                            {
                                prevTaskIndex--;
                            }
                            // 如果找到了非block的前置任务，检查其完成状态
                            if (prevTaskIndex >= 0)
                            {
                                if (npcData.AllTaskList[prevTaskIndex].AccomplishStatus != (int)AccomplishStatus.GetAward)
                                {
                                    satisfy = false;
                                }
                            }
                            // 如果所有前置任务都是block，则视为满足条件
                        }
                    
                        break;
                    //物品数量大于等于
                    case TaskConditionType.ItemCountMoreThan:
                        List<int> item = CommonUtil.SplitCfgOneDepth(param);
                        if (!ItemManager.Instance.CheckIfItemEnough(item[0], (ulong)item[1]))
                        {
                            satisfy = false;

                        }
            
                        break;
                        //完成了某个主线任务
                    case TaskConditionType.AccomplishedMainLineTask:
                        string theTagId = param.ToString();
                        if (RoleManager.Instance._CurGameInfo.AllAchievementData.PassedTaskTagIdList.Contains(theTagId))
                            satisfy = true;
                        else
                            satisfy = false;
                        break;
                    //有弟子感悟值满
                    case TaskConditionType.RareStudentExpFull:
                        bool haveSatisfiedStudent = false;
                        for(int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                            if (p.talent == (int)StudentTalent.None&&p.studentCurExp >= 120)
                            {
                                haveSatisfiedStudent = true;
                                break;
                            }
                        }
                        satisfy = haveSatisfiedStudent;
                        break;
                    //有觉醒后的弟子感悟值满
                    case TaskConditionType.StudentExpFull:
                        bool haveSatisfiedexpFullStudent = false;
                        for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                            if (p.talent != (int)StudentTalent.None && p.studentCurExp >= 120)
                            {
                                haveSatisfiedexpFullStudent = true;
                                break;
                            }
                        }
                        if (haveSatisfiedexpFullStudent)
                            satisfy = true;
                        else
                            satisfy = false;
                        break;
                }
                if (!satisfy)
                    break;
            }
        }

        //if(taskSOSetting.condition.type)
        return satisfy;
    }

    /// <summary>
    /// 开始指引
    /// </summary>
    public void StartGuide(ulong npcId)
    {
        curGuideNPCOnlyId = npcId;
        SingleNPCData curGuideNPCData = FindNPCByOnlyId(npcId);
        SingleTaskProtoData taskProtoData = FindTaskById(curGuideNPCData, curGuideNPCData.CurTaskId);
        NPC npcSetting = DataTable.FindNPCArrById(curGuideNPCData.Id);
        SingleTask taskSetting = FindTaskSettingById(npcSetting,curGuideNPCData.CurTaskId);
        switch (taskSetting.taskType)
        {
       
            default:
                taskProtoData.StartGuide = true;
                ShowGuide();
                break;
        }


    }


    ///// <summary>
    ///// 找个空地点
    ///// </summary>
    //public void FindAEmptyFarmClick()
    //{
    //    for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
    //    {
    //        SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
    //        if (data.Unlocked && data.IsEmpty)
    //        {
    //            int index = data.Index;
    //            EventCenter.Broadcast(TheEventType.ClickFarm, index);
    //        }
    //    }
    //}

    /// <summary>
    /// 通过任务tagId找任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleTaskProtoData FindTaskByTagId(SingleNPCData npcData, string tagId)
    {
        for (int i = 0; i < npcData.AllTaskList.Count; i++)
        {
            SingleTaskProtoData taskProtoData = npcData.AllTaskList[i];
            NPC npcSetting = DataTable.FindNPCArrById(npcData.Id);
            SingleTask taskSetting = FindTaskSettingById(npcSetting,taskProtoData.TheId);
            if (taskSetting.tagId == tagId)
            {
                return taskProtoData;
            }
        }
        return null;
    }

    /// <summary>
    /// 通过任务id找任务
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleTaskProtoData FindTaskById(SingleNPCData npcData, ulong id)
    {
        for(int i = 0; i < npcData.AllTaskList.Count; i++)
        {
            SingleTaskProtoData taskProtoData = npcData.AllTaskList[i];
            if (id == taskProtoData.TheId)
            {
                return taskProtoData;
            }
        }
        return null;
    }
    /// <summary>
    /// 通过任务id找任务setting
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleTask FindTaskSettingById(NPC npcSetting, ulong id)
    {
        if (npcSetting.tasks != null)
        {
            for (int i = 0; i < npcSetting.tasks.Count; i++)
            {
                SingleTask taskSetting = npcSetting.tasks[i];
                if (id == taskSetting.theId)
                {
                    return taskSetting;
                }
            }
        }
      
        return null;
    }

    /// <summary>
    /// 试图在当前存在的npc里面完成任务
    /// </summary>
    public void TryAccomplishAllTask()
    {
        TryAccomplishAllGuideBook();
        if (RoleManager.Instance._CurGameInfo.allNPCData == null)
            return;
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
            SingleNPCData npcData = FindNPCByOnlyId(onlyId);
            TryAccomplishTask(npcData);
        }

        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.SingleGuideBookTaskDataList.Count; i++)
        //{
        //    SingleGuideBookTaskData singleGuideBookTaskData = RoleManager.Instance._CurGameInfo.AllGuideBookData.SingleGuideBookTaskDataList[i];
        //    if(singleGuideBookTaskData.type)
        //    TryAccomplishTask(npcData);
        //}
    }

    /// <summary>
    /// 试图完成当前任务
    /// </summary>
    public void TryAccomplishTask(SingleNPCData npcData)
    {
        SingleTaskProtoData curTaskData = FindTaskById(npcData, npcData.CurTaskId);
        if (curTaskData == null)
            return;
        NPC npcSetting = DataTable.FindNPCArrById(npcData.Id);
        SingleTask curTaskSetting = FindTaskSettingById(npcSetting, npcData.CurTaskId);
        if (curTaskSetting == null)
            return;
        switch (curTaskSetting.taskType)
        {
            //杀怪拿东西
            case TaskType.KillMonsterGetItem:
                ItemData theItem = ItemManager.Instance.FindItemBySettingId((int)curTaskSetting.needItem);
                if (theItem != null)
                {
                    ulong itemCount = theItem.count;
                    if (itemCount >= (ulong)curTaskSetting.needNum)
                    {
                        //完成任务
                        AccomplishTask(curTaskData);

                    }
                    else
                    {
                        //如果已完成，则变未完成
                        if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
                        {
                            ChangeTaskStatus(curTaskData, AccomplishStatus.Processing);

                        }
                        //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                    }

                }
                else
                {
                    //如果已完成，则变未完成
                    if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
                    {
                        ChangeTaskStatus(curTaskData, AccomplishStatus.Processing);
                    }
                        //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                }

                break;
                //收集物品
            case TaskType.ReceiveItem:
                ItemData theReceiveItem = ItemManager.Instance.FindItemBySettingId((int)curTaskSetting.needItem);
                if (theReceiveItem != null)
                {
                    ulong itemCount = theReceiveItem.count;
                    if (itemCount >= (ulong)curTaskSetting.needNum)
                    {
                        //完成任务
                        AccomplishTask(curTaskData);
                    }
                    else
                    {
                        //如果已完成，则变未完成
                        if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
                        {
                            ChangeTaskStatus(curTaskData, AccomplishStatus.Processing);

                        }
                        //curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                    }
                }
                else
                {
                    //如果已完成，则变未完成
                    if (curTaskData.AccomplishStatus == (int)AccomplishStatus.Accomplished)
                        curTaskData.AccomplishStatus = (int)AccomplishStatus.Processing;
                }
                break;
            //收集物品
            case TaskType.LianDan:
                int itemType = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (ItemManager.Instance.FindItemListByType((ItemType)itemType).Count > 0)
                {
                    //完成任务
                    AccomplishTask(curTaskData);
                }
        
                break;
            case TaskType.KillEnemy:
                int enemyId = (int)curTaskSetting.needKillEnemy;
                int enemyNum = curTaskData.CurNum;
                //能提前杀
                if (curTaskSetting.commonAccomplishCondition == "1")
                {
                    enemyNum = FindAchievement(AchievementType.KillEnmey, enemyId.ToString()).ToInt32();

                }
                if (enemyNum >= curTaskSetting.needKillEnemyNum)
                {
                    //完成任务
                    AccomplishTask(curTaskData);
                    //curTaskData.CurNum = 0;

                }
                break;
            case TaskType.QieCuo:
                if (curTaskData.CurNum > 0)
                {
                    //完成任务
                    AccomplishTask(curTaskData);
                    //curTaskData.CurNum = 0;
                }
                break;
            //引导建个丹炉
            case TaskType.DanFarmNum:
                if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count > 0)
                {
                    int validNum = 0;
                    for(int i=0;i< RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                    {
                        SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                        if (singleDanFarmData.IsEmpty)
                            continue;
                        if (singleDanFarmData.Status == (int)DanFarmStatusType.Building)
                            continue;
                        if (curTaskSetting.needDanFarmId == DanFarmIdType.Any)
                        {
                            
                            validNum++;
                   
                        }
                        else
                        {
                            if ((int)curTaskSetting.needDanFarmId == singleDanFarmData.SettingId)
                            {
                                validNum++;
                            }
                        }
                    }
                    if (validNum >= curTaskSetting.needDanFarmNum)
                    {
                        AccomplishTask(curTaskData);

                    }

                }
                break;
            //引导招募炼丹师
            case TaskType.ZhaoMuDiZi:
                int recruitStudentNum= curTaskSetting.commonAccomplishCondition.ToInt32();
                if (RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count>= recruitStudentNum)
                {
                    //完成任务
                    AccomplishTask(curTaskData);
                    //curTaskData.CurNum = 0;
                }
                break;
                //炼制法器数量
            case TaskType.EquipNum:
                ItemIdType idType = curTaskSetting.needItem;
                //curTaskSetting.needNum==1
                int haveNum = ItemManager.Instance.FindAllEquipmentList().Count;
                if(haveNum>= curTaskSetting.needNum)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            //升级某级别法器等级
            case TaskType.UpgradeRarityEquip:
                List<int> UpgradeRarityEquipNeed = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int needUpgradeRarityEquipNeedRarity = UpgradeRarityEquipNeed[0];
                int needUpgradeRarityEquipNeedLevel = UpgradeRarityEquipNeed[1];
                List<ItemData> UpgradeRarityEquipNeedequipItemList = ItemManager.Instance.FindAllEquipmentList();
                for (int j = 0; j < UpgradeRarityEquipNeedequipItemList.Count; j++)
                {
                    ItemData itemData = UpgradeRarityEquipNeedequipItemList[j];
                    if (itemData.quality == needUpgradeRarityEquipNeedRarity
                        && itemData.equipProtoData.curLevel >= needUpgradeRarityEquipNeedLevel)
                    {
                        AccomplishTask(curTaskData);
                        break;
                    }
                }
                break;
            //弟子拥有a个b等级法器
            case TaskType.StudentHaveANumBLevelEquip:
                List<int> HaveANumBLevelEquipNeed = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int HaveANumBLevelEquipNumNeed = HaveANumBLevelEquipNeed[0];
                int HaveANumBLevelEquipNeedLevel = HaveANumBLevelEquipNeed[1];
                int HaveANumBLevelEquipNeedMyNum = 0;
                 for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    for(int k = 0; k < p.curEquipItemList.Count; k++)
                    {
                        ItemData data = p.curEquipItemList[k];
                        if (p.talent == (int)StudentTalent.LianGong
                    && data != null
                    && data.equipProtoData != null
                    && data.equipProtoData.curLevel >= HaveANumBLevelEquipNeedLevel)
                        {
                            HaveANumBLevelEquipNeedMyNum++;
                            if (HaveANumBLevelEquipNeedMyNum >= HaveANumBLevelEquipNumNeed)
                            {
                                AccomplishTask(curTaskData);
                                break;

                            }
                        }
                    }
                
                 
                }
           

                break;
            //完成地图事件次数
            case TaskType.AccomplishMapEvent:
                List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int theMapEventType = mapEventParam[0];
                int needNum = mapEventParam[1];
                int mapEventNum= FindAchievement(AchievementType.AccomplishedMapEvent, theMapEventType.ToString()).ToInt32();
                if (mapEventNum >= needNum)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            case TaskType.PlayerLevel:
                int level = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex+1;
                if (level >= curTaskSetting.needPlayerLevel)
                {
                    AccomplishTask(curTaskData);

                }
                break;
                //装备技能
            case TaskType.EquipSkill:
                int skillId = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList.Count >= 2)
                {
                    AccomplishTask(curTaskData);
                }
                break;
                //学习技能
            case TaskType.StudySkill:
                int StudySkillId = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (StudySkillId == -1)
                {
                    if(RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count>=2)
                        AccomplishTask(curTaskData);

                }
                else
                {
                    for (int i = 0; i < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; i++)
                    {
                        SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[i];
                        if (singleSkillData.skillId == StudySkillId)
                            AccomplishTask(curTaskData);

                    }
                }
               

                break;
            //升级功法
            case TaskType.UpgradeSkill:
                List<int> upgradeSkillParamList = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int UpgradeSkillId = upgradeSkillParamList[0];
                int UpgradeSkillLevel= upgradeSkillParamList[1];
                for (int i = 1; i < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; i++)
                {
                    
                    SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[i];
                    //非普攻
                    if ((singleSkillData.skillId == UpgradeSkillId
                        ||UpgradeSkillId==0)
                        && singleSkillData.skillLevel>=UpgradeSkillLevel)
                        AccomplishTask(curTaskData);

                }

                break;
            //拥有a名b天赋弟子
            case TaskType.HaveATalentBStudent:
                List<int> HaveATalentBStudentParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int needTalentNum = HaveATalentBStudentParam[0];
                int needTalentTalent = HaveATalentBStudentParam[1];
                int myNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    if (p.talent == needTalentTalent
                        || needTalentTalent == 0)
                    {
                        myNum++;
                    }
                }
                if (myNum >= needTalentNum)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //拥有a名b等级的生产弟子
            case TaskType.HaveABLevelProductStudent:
                List<int> HaveABLevelProductStudentParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int needStudentNum = HaveABLevelProductStudentParam[0];
                int needStudentLevel = HaveABLevelProductStudentParam[1];
                int validStudentNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                    if (p.talent != (int)StudentTalent.LianGong
                         && p.talent != (int)StudentTalent.None
                        && p.studentLevel >= needStudentLevel)
                    {
                        validStudentNum++;

                    }
                }
                if (validStudentNum >= needStudentNum)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            case TaskType.UpgradeZongMen:
                int zongmenLevelNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel >= zongmenLevelNeed)
                {
                    AccomplishTask(curTaskData);
                }

                break;
            //秘境次数
            case TaskType.Explore:
                List<int> ExploreParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int needExploreNum = ExploreParam[1];
                int needExploreId = ExploreParam[0];
                int validExploreNum = FindAchievement(AchievementType.Explore, needExploreId.ToString()).ToInt32();
                if (validExploreNum >= needExploreNum)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            case TaskType.QuanLiNum:
                int quanLiNumNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (FindAchievement(AchievementType.QuanLiTime,"").ToInt32()>=quanLiNumNeed)
                {
                    AccomplishTask(curTaskData);
                }

                break;
            case TaskType.LiLian:
                
                int liLianNumNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (FindAchievement(AchievementType.LiLian, "").ToInt32() >= liLianNumNeed)
                {
                    AccomplishTask(curTaskData);
                }

                break;
            //最大讨伐
            case TaskType.MaxTaoFa:
                List<int> MaxTaoFaParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int MaxTaoFaType = MaxTaoFaParam[0];
                int needMaxTaoFaLevel = MaxTaoFaParam[1];
                int myMaxTaoFaLevel = TaskManager.Instance.FindAchievement(AchievementType.MaxTaoFa, MaxTaoFaType.ToString()).ToInt32();
                if (myMaxTaoFaLevel >= needMaxTaoFaLevel)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            //驻守建筑 建筑id|弟子数量|如果有 则是弟子天赋
            case TaskType.StudentZuoZhen:
                List<int> StudentZuoZhenParamList = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int StudentZuoZhenfarmId = StudentZuoZhenParamList[0];
                int StudentZuoZhenNum = StudentZuoZhenParamList[1];
                bool haveTalentCondition = false;//有天赋需求
                StudentTalent needTalent = StudentTalent.None;
                if (StudentZuoZhenParamList.Count >= 3)
                {
                    haveTalentCondition = true;
                    needTalent = (StudentTalent)StudentZuoZhenParamList[2];
                }
                int StudentZuoZhenvalidNum = 0;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                    if(p.studentStatusType==(int)StudentStatusType.DanFarmQuanLi
                        || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                        ||p.studentStatusType == (int)StudentStatusType.DanFarmWork)
                    {
                        SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];
                        if (danFarmData.SettingId == StudentZuoZhenfarmId)
                        {
                            if (haveTalentCondition)
                            {
                                if (p.talent ==(int)needTalent)
                                {
                                    StudentZuoZhenvalidNum++;
                                }
                            }
                            else
                            {
                                StudentZuoZhenvalidNum++;
                            }
                        }

                    }
                }
                if (StudentZuoZhenvalidNum >= StudentZuoZhenNum)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //拥有a个b级c建筑 
            case TaskType.HaveABLevelCFarm:
                List<int> HaveABLevelCFarmParamList=CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int farmId = HaveABLevelCFarmParamList[2];
                int needLevel = HaveABLevelCFarmParamList[1];
                int needHaveABLevelCFarmParamNum = HaveABLevelCFarmParamList[0];
                int validHaveABLevelCFarmParamNum = 0;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData danFarm = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    if (danFarm.SettingId==farmId
                        && danFarm.CurLevel >= needLevel)
                    {
                        validHaveABLevelCFarmParamNum++;
                    }
 
                }
                if (validHaveABLevelCFarmParamNum >= needHaveABLevelCFarmParamNum)
                {
                    AccomplishTask(curTaskData);

                }

                break;
            //天赋觉醒次数
            case TaskType.TianFuJueXingNum:
                int TianFuJueXingNumNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (FindAchievement(AchievementType.TianFuJueXingNum, "").ToInt32() >= TianFuJueXingNumNeed)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //弟子突破次数
            case TaskType.StudentUpgradeCount:
                int StudentUpgradeCountNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (FindAchievement(AchievementType.StudentUpgradeCount, "").ToInt32() >= StudentUpgradeCountNeed)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //升级法器等级
            case TaskType.UpgradeEquip:
                int UpgradeEquipNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                List<ItemData> equipItemList = ItemManager.Instance.FindAllEquipmentList();
                for (int i = 0; i < equipItemList.Count; i++)
                {
                    ItemData itemData = equipItemList[i];
                    if (itemData.equipProtoData.curLevel >= UpgradeEquipNeed)
                    {
                        AccomplishTask(curTaskData);
                        break;
                    }
                }
             
                break;
            //炼制某等级法器数量
            case TaskType.RarityEquipNum:
                List<int> RarityequipParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);

                int rarity = RarityequipParam[0];
                int needRarityEquipNum = RarityequipParam[1];
                //curTaskSetting.needNum==1
                int haveRarityNum = 0;
                for (int j = 0; j < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; j++)
                {
                    int settingId = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[j].settingId;
                    ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
                    if (itemSetting.Rarity.ToInt32() == rarity && itemSetting.ItemType.ToInt32() == (int)ItemType.Equip)
                    {
                        haveRarityNum++;

                    }
                }
                List<PeopleData> RarityequipParampList = StudentManager.Instance.FindAllMyTalentStudentList(StudentTalent.LianGong);
                for (int j = 0; j < RarityequipParampList.Count; j++)
                {
                    PeopleData p = RarityequipParampList[j];
                    for(int k = 0; k < p.curEquipItemList.Count; k++)
                    {
                        ItemData theData = p.curEquipItemList[k];
                        if (theData != null
                       && theData.equipProtoData != null)
                        {
                            int settingId = theData.settingId;
                            ItemSetting itemSetting = DataTable.FindItemSetting(settingId);

                            if (itemSetting.Rarity.ToInt32() == rarity)
                            {
                                haveRarityNum++;

                            }
                        }
                    }
                   
                }
                if (haveRarityNum >= needRarityEquipNum)
                {
                    AccomplishTask(curTaskData);

                }
                break;
            //解锁空地数量
            case TaskType.UnlockFarmPosNum:
                int UnlockFarmPosNumNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum >= UnlockFarmPosNumNeed)
                {
                    AccomplishTask(curTaskData);
                }

                break;
                //通过哪一关
            case TaskType.PassLevel:
                string needpassLevel = curTaskSetting.commonAccomplishCondition;
                string myPassedMaxLevel = FindAchievement(AchievementType.PassedMaxLevel);
                int needLogicLevel= ConstantVal.GetLevelByLevelStr(needpassLevel);
                int myLogicLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLevel);
                if (myLogicLevel >= needLogicLevel)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //通过哪一关裂隙
            case TaskType.PassedMaxLieXiLevel:
                string needpassLieXiLevel = curTaskSetting.commonAccomplishCondition;
                string myPassedMaxLieXiLevel = FindAchievement(AchievementType.PassedMaxLieXiLevel);
                int needLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(needpassLieXiLevel);
                int myLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLieXiLevel);
                if (myLogicLieXiLevel >= needLogicLieXiLevel)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //弟子上阵
            case TaskType.ShangZhen:
                int needShangZhenNum = curTaskSetting.commonAccomplishCondition.ToInt32();
                 
                if (TeamManager.Instance.FindMyTeam1PNum() >= needShangZhenNum)
                {
                    AccomplishTask(curTaskData);
                }

                break;
            //装备法器
            case TaskType.EquipEquip:
                for(int i=0;i< RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList.Count; i++)
                {
                    if (RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList[i] != null)
                    {
                        AccomplishTask(curTaskData);
                        break;
                    }
             
                }
         
                break;
            //炼制宝石
            case TaskType.MakeGem:
                 List<ItemData> gemItemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
                if (gemItemList.Count > 0)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //所有宝石数量
            case TaskType.TotalGemNum:
                int TotalGemNumNeedNum = curTaskSetting.commonAccomplishCondition.ToInt32();
                int TotalGemNumMyNum = FindAchievement(AchievementType.MakeGem).ToInt32();
        

                if (TotalGemNumMyNum>= TotalGemNumNeedNum)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //镶嵌某级别宝石
            case TaskType.InlayRarityGem:
                List<int> inlayGemParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int validGemNum = 0;
                List<ItemData> equipList = ItemManager.Instance.FindAllEquipmentList();
                for(int j=0;j< equipList.Count; j++)
                {
                    ItemData itemData = equipList[j];
                    if (itemData.equipProtoData != null)
                    {
                        for (int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                        {
                            ItemData gem = itemData.equipProtoData.gemList[k];
                            if (gem != null&&gem.onlyId>0)
                            {
                                if (gem.quality == inlayGemParam[0])
                                    validGemNum++;
                            }
                        }
                    }
                }
                //for (int j = 0; j < RoleManager.Instance._CurGameInfo.itemModel.itemDataList.Count; j++)
                //{
                //    ItemData itemData = RoleManager.Instance._CurGameInfo.itemModel.itemDataList[j];
                //    if (itemData.equipProtoData != null)
                //    {
                //        for (int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                //        {
                //            ItemData gem = itemData.equipProtoData.gemList[k];
                //            if (gem != null)
                //            {
                //                if (gem.quality == inlayGemParam[0])
                //                    validGemNum++;
                //            }
                //        }
                //    }
                //}
                //List<PeopleData> studentList = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                //for (int j = 0; j < studentList.Count; j++)
                //{
                //    PeopleData p = studentList[j];
                //    for(int k = 0; k < p.curEquipItemList.Count; k++)
                //    {
                //        ItemData data = p.curEquipItemList[k];
                //        if (data != null
                //     && data.equipProtoData != null)
                //        {
                //            for (int m = 0; m < data.equipProtoData.gemList.Count; m++)
                //            {
                //                ItemData gem = data.equipProtoData.gemList[m];
                //                if (gem != null)
                //                {
                //                    if (gem.quality == inlayGemParam[0])
                //                        validGemNum++;
                //                }
                //            }
                //        }
                //    }
                 
                //}
                if (validGemNum >= inlayGemParam[1])
                {
                    AccomplishTask(curTaskData);
                }
                break;

            case TaskType.StudentHaveLevelGem:
                List<int> studentHaveGemParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int validStudentGemNum = 0;
                List<PeopleData> studentList2 = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                for (int j = 0; j < studentList2.Count; j++)
                {
                    PeopleData p = studentList2[j];
                    for(int k = 0; k < p.curEquipItemList.Count; k++)
                    {
                        ItemData data = p.curEquipItemList[k];
                        if (data != null
                  && data.equipProtoData != null)
                        {
                            for (int m = 0; m < data.equipProtoData.gemList.Count; m++)
                            {
                                ItemData gem = data.equipProtoData.gemList[k];

                                if (gem != null && gem.onlyId>0)
                                {
                                    GemSetting setting = DataTable.FindGemSetting(gem.settingId);
                                    if (setting.Level.ToInt32() == studentHaveGemParam[0]
                                        || studentHaveGemParam[0] == 0)
                                        validStudentGemNum++;
                                }
                            }
                        }
                    }
              
                }
                if (validStudentGemNum >= studentHaveGemParam[1])
                {
                    AccomplishTask(curTaskData);
                }

                break;

            //讨伐
            case TaskType.TaoFa:
                List<int> TaoFaParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int taoFaType = TaoFaParam[0];
                int taoFaNum = TaoFaParam[1];
                if (FindAchievement(AchievementType.TaoFa, taoFaType.ToString()).ToInt32()>= taoFaNum)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //改宗门名
            case TaskType.ChangeZongMenName:
                if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName != "临时宗门")
                {
                    AccomplishTask(curTaskData);

                }
                break;
                //强化血脉
            case TaskType.UpgradeXueMai:
                int totalXueMaiUpgradeNum = 0;
                for(int i = 1; i < (int)XueMaiType.End; i++)
                {
                    totalXueMaiUpgradeNum += XueMaiManager.Instance.FindXueMaiLevel(RoleManager.Instance._CurGameInfo.playerPeople, (XueMaiType)i);
                }
               
                if (totalXueMaiUpgradeNum >= curTaskSetting.commonAccomplishCondition.ToInt32())
                {
                    AccomplishTask(curTaskData);

                }
               
                break;
            case TaskType.CompositeGem:

                List<int> compositeGemParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                if (ItemManager.Instance.FindRarityMoreThanLevelGemNum((Rarity)compositeGemParam[0], compositeGemParam[1]) >= compositeGemParam[2])
                {
                    AccomplishTask(curTaskData);
                }
                

                break;
                //弟子拥有法器
            case TaskType.StudentHaveEquip:

                int faQiNeed = curTaskSetting.commonAccomplishCondition.ToInt32();
                int haveFaQiStudentNum = 0;
                for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                    for(int j = 0; j < p.curEquipItemList.Count; j++)
                    {
                        ItemData data = p.curEquipItemList[j];
                        if (data != null && data.equipProtoData != null)
                        {
                            haveFaQiStudentNum++;
                        }
                    }
                 
                }
                if (haveFaQiStudentNum>= faQiNeed)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //弟子拥有a级别b等级c数量法器
            case TaskType.StudentHaveARarityBLevelCNumEquip:
                List<int> StudentHaveARarityBLevelCNumParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int needRarity = StudentHaveARarityBLevelCNumParam[0];
                int StudentHaveARarityBLevelCNumParamNeedLevel = StudentHaveARarityBLevelCNumParam[1];
                int StudentHaveARarityBLevelCNumParamNeedNum = StudentHaveARarityBLevelCNumParam[2];
                int StudentHaveARarityBLevelCNumvalidNum = 0;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                    for(int j = 0; j < p.curEquipItemList.Count; j++)
                    {
                        ItemData data = p.curEquipItemList[j];
                        if (data != null
               && data.equipProtoData != null
               && data.quality >= needRarity
               && data.equipProtoData.curLevel >= StudentHaveARarityBLevelCNumParamNeedLevel)
                        {
                            StudentHaveARarityBLevelCNumvalidNum++;
                            if (StudentHaveARarityBLevelCNumvalidNum >= StudentHaveARarityBLevelCNumParamNeedNum)
                            {
                                AccomplishTask(curTaskData);
                                break;
                            }
                        }
                    }
          
                }
           
                break;
            //炼丹
            case TaskType.LianDan2:
                List<int> lianDan2Param = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition); 
                int LianDan2Id = lianDan2Param[0];
                int LianDan2Num = lianDan2Param[1];
                if (FindAchievement(AchievementType.LianDan2, LianDan2Id.ToString()).ToInt32() >= LianDan2Num)
                {
                    AccomplishTask(curTaskData);
                }
                break;
            //拥有技能的弟子数量
            case TaskType.HaveABLevelSkillStudentNum:
                List<int> HaveABLevelSkillStudentNumParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int HaveABLevelSkillStudentNumStudentNum = HaveABLevelSkillStudentNumParam[0];
                int HaveABLevelSkillStudentNumSkillLevel = HaveABLevelSkillStudentNumParam[1];
                int HaveABLevelSkillStudentNumMyNum = 0;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                    if (p.allSkillData != null
                        &&p.allSkillData.skillList.Count>=2)
                    {
                        if(p.allSkillData.skillList[1].skillLevel>= HaveABLevelSkillStudentNumSkillLevel)
                        {
                            HaveABLevelSkillStudentNumMyNum++;
                            if(HaveABLevelSkillStudentNumMyNum >= HaveABLevelSkillStudentNumStudentNum)
                            {
                                AccomplishTask(curTaskData);
                                break;

                            }
                        }
                    }
                }

                break;

            //坐镇a建筑需要b总数弟子
            case TaskType.ZuoZhenAFarmBStudentNum: 
                List<int> StudentZuoZhenTotalNumParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int ZuoZhenAFarmBStudentNumType = StudentZuoZhenTotalNumParam[0];
                int StudentZuoZhenTotalNumNum = StudentZuoZhenTotalNumParam[1];
                int myTotalZuoZhenNum = 0;
                for(int i = 0; i < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
                {
                    SingleDanFarmData data = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[i];
                    if (data.DanFarmType== ZuoZhenAFarmBStudentNumType
                        && data.ZuoZhenStudentIdList.Count > 0)
                    {
                        for(int j=0;j< data.ZuoZhenStudentIdList.Count; j++)
                        {
                            ulong theId = data.ZuoZhenStudentIdList[j];
                            if (theId > 0)
                            {
                                myTotalZuoZhenNum++;
                                if (myTotalZuoZhenNum >= StudentZuoZhenTotalNumNum)
                                {
                                    AccomplishTask(curTaskData);
                                    break;
                                }
                            }
                        }
                    }
                }
           
                break;
            //队伍中有a个b元素的人
            case TaskType.TeamHaveABYuanSuP:
                List<int> TeamHaveABYuanSuPParam = CommonUtil.SplitCfgOneDepth(curTaskSetting.commonAccomplishCondition);
                int TeamHaveABYuanSuPNum = TeamHaveABYuanSuPParam[0];
                int TeamHaveABYuanSuPYuanSu = TeamHaveABYuanSuPParam[1];
                int TeamHaveABYuanSuPMyNum = 0;
                if (RoleManager.Instance._CurGameInfo.playerPeople.yuanSu == TeamHaveABYuanSuPNum)
                {
                    TeamHaveABYuanSuPMyNum++;
                }
                List<PeopleData> teamList = RoleManager.Instance.FindMyBattleTeamList(false, 0);

                for (int i=0;i< teamList.Count; i++)
                {
                    PeopleData p = teamList[i];
                    if (p.yuanSu == TeamHaveABYuanSuPYuanSu)
                    {
                        TeamHaveABYuanSuPMyNum++;
                    }
                    if(TeamHaveABYuanSuPMyNum>= TeamHaveABYuanSuPNum)
                    {
                        AccomplishTask(curTaskData);
                        break;
                    }
                }
             
                break;

        }
        //这里发消息刷新npc的显示
        EventCenter.Broadcast(TheEventType.AccomplishTask);
    }

    public void AccomplishTask(SingleTaskProtoData taskProtoData)
    {
 
        //任务完成ui
        if(taskProtoData.AccomplishStatus==(int)AccomplishStatus.Processing
            || taskProtoData.AccomplishStatus == (int)AccomplishStatus.UnAccomplished)
        {
            NPC npcSetting = DataTable.FindNPCArrById(taskProtoData.NpcId);
            SingleTask taskSetting = FindTaskSettingById(npcSetting, taskProtoData.TheId);
            TryRecordPassedTaskTagId(taskSetting.tagId);
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (!string.IsNullOrWhiteSpace(taskSetting.trackingName))
            {
                dic.Add(taskSetting.trackingName, 1);
                TDGAMission.OnCompleted(taskSetting.trackingName);
            }
          
             
            PanelManager.Instance.OpenPanel<AccomplishTaskPanel>(PanelManager.Instance.trans_layer2);

            ChangeTaskStatus(taskProtoData, AccomplishStatus.Accomplished);

        }
        // taskProtoData.AccomplishStatus = (int);

    }

    /// <summary>
    /// 得到每日任务的进度
    /// </summary>
    /// <param name="taskType"></param>
    public void GetDailyAchievement(TaskType taskType,string param)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList.Count; i++)
        {
            SingleDailyTaskData data = RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList[i];
            TaskSetting taskSetting = DataTable.FindTaskSetting(data.settingId);
            if (taskSetting.TaskType.ToInt32() == (int)taskType)
            {
                switch (taskType)
                {
                    case TaskType.TaoFa:
                        List<int> taofaParam = CommonUtil.SplitCfgOneDepth(param);
                        int accomplishTaoFaType = taofaParam[0];
                        List<int> taoFaParam =CommonUtil.SplitCfgOneDepth(taskSetting.Param);
                        if (accomplishTaoFaType == taoFaParam[0])
                        {
                            data.curNum++;
                        }
                        break;
                    case TaskType.AccomplishMapEvent:
                        int mapEventType = param.ToInt32();
                        List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(taskSetting.Param);
                        if (mapEventType == mapEventParam[0])
                        {
                            data.curNum++;
                        }
                        break;
                    case TaskType.LianDan2:
                            data.curNum+=param.ToInt32();
                        break;
                    case TaskType.RarityEquipNum:
                        data.curNum++;
                        break;
                    case TaskType.UpgradeRarityEquip:
                        data.curNum++;
                        break;
                    case TaskType.Explore:
                        data.curNum++;
                        break;
                    case TaskType.PassedMaxLieXiLevel:
                        data.curNum++;
                        break;

                    case TaskType.ZhaoMuDiZi:
                        data.curNum++;
                        break;
                    case TaskType.TianFuJueXingNum:
                        data.curNum++;
                        break;
                    case TaskType.StudentUpgradeCount:
                        data.curNum++;
                        break;
                    case TaskType.LiLian:
                        data.curNum++;
                        break;
                    case TaskType.MakeGem:
                        data.curNum++;
                        break;
                    case TaskType.CompositeGem:
                        data.curNum++;
                        break;
                }
                TryAccomplishDailyTask(taskType);
            }
        }
    }
 
    /// <summary>
    /// 完成每日任务
    /// </summary>
    /// <param name="taskType"></param>
    public void TryAccomplishDailyTask(TaskType taskType)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList.Count; i++)
        {
            SingleDailyTaskData data = RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList[i];
            TaskSetting taskSetting = DataTable.FindTaskSetting(data.settingId);
            if (data.accomplishStatus == (int)AccomplishStatus.Processing
              && taskSetting.TaskType.ToInt32() == (int)taskType)
            {
                switch (taskType)
                {
                    case TaskType.TaoFa:
                        List<int> taoFaParam = CommonUtil.SplitCfgOneDepth(taskSetting.Param);
                        if(data.curNum >= taoFaParam[1])
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.AccomplishMapEvent:
                        List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(taskSetting.Param);
                        if (data.curNum>= mapEventParam[1])
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.LianDan2:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.RarityEquipNum:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.UpgradeRarityEquip:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.Explore:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.PassedMaxLieXiLevel:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;

                    case TaskType.ZhaoMuDiZi:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.TianFuJueXingNum:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.StudentUpgradeCount:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.LiLian:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.MakeGem:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                    case TaskType.CompositeGem:
                        if (data.curNum >= taskSetting.Param.ToInt32())
                        {
                            AccomplishDailyTask(data);
                        }
                        break;
                }
            }
        }

    }
    /// <summary>
    /// 完成日常任务
    /// </summary>
    /// <param name="data"></param>
    public void AccomplishDailyTask(SingleDailyTaskData data)
    {
        if (data.accomplishStatus == (int)AccomplishStatus.Processing)
        {
            data.accomplishStatus = (int)AccomplishStatus.Accomplished;
            RefreshDailyRedPointShow(data);
        }
    }
    public void TryAccomplishAllGuideBook()
    {
        if(RoleManager.Instance._CurGameInfo.AllGuideBookData == null)
        return;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            //process表示已暴露
            if (data.accomplishStatus == (int)AccomplishStatus.Processing)
            {
                TryAccomplishGuideBook((TaskType)setting.Type.ToInt32());
            }
        }
    }

    /// <summary>
    /// 拿奖每日任务
    /// </summary>
    /// <param name="singleGuideBookTaskData"></param>
    public void OnGetDailyTaskAward(SingleDailyTaskData taskData)
    {
        if (taskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
        {
            TaskSetting taskSetting = DataTable.FindTaskSetting(taskData.settingId);
            int huoYueAdd = taskSetting.HuoYueDuAdd.ToInt32();
            int lingShiNum= huoYueAdd * 100;
            ItemManager.Instance.GetItemWithTongZhiPanel((int)ItemIdType.LingShi, (ulong)lingShiNum);
            PanelManager.Instance.AddTongZhi(TongZhiType.Common, "活跃度+"+taskSetting.HuoYueDuAdd);
            taskData.accomplishStatus = (int)AccomplishStatus.GetAward;

            //活跃度
            RoleManager.Instance._CurGameInfo.AllDailyTaskData.curActiveNum += huoYueAdd;
            for(int i = 0; i < 5; i++)
            {
                int activeNeed = 20 * (i+1);
                if (RoleManager.Instance._CurGameInfo.AllDailyTaskData.curActiveNum >= activeNeed
                    &&RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[i]==(int)AccomplishStatus.Processing)
                {
                    RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[i] =(int) AccomplishStatus.Accomplished;
                    RefreshDailyProcessRedPointShow(i);
                }
            }
            RefreshDailyRedPointShow(taskData);

            EventCenter.Broadcast(TheEventType.OnGetDailyTaskAward);

        }
    }

    /// <summary>
    /// 日常进程奖励领取
    /// </summary>
    public void OnGetDailyTaskProcessAward(int index)
    {
        int needActiveNum = index * 20;
        if (RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index]==(int)AccomplishStatus.Accomplished)
        {
            List<List<List<int>>> awardList = CommonUtil.SplitThreeCfg(ConstantVal.dailyTaskProcessAward);
            List<List<int>> theAward = awardList[index];
            for(int i = 0; i < theAward.Count; i++)
            {
                List<int> singleAward = theAward[i];
                int id = singleAward[0];
                int num = singleAward[1];

                ItemManager.Instance.GetItemWithTongZhiPanel(id, (ulong)num);
            }
            RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index] = (int)AccomplishStatus.GetAward;
            EventCenter.Broadcast(TheEventType.OnGetDailyTaskProcessAward);
            RefreshDailyProcessRedPointShow(index);

        }
    }

    /// <summary>
    /// 完成冒险手札
    /// </summary>
    public void TryAccomplishGuideBook(TaskType taskType)
    {
        if (RoleManager.Instance._CurGameInfo.AllGuideBookData == null)
            return;
        List<SingleGuideBookTaskData> validList = new List<SingleGuideBookTaskData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            //process表示已暴露
            if (data.accomplishStatus == (int)AccomplishStatus.Processing
                &&setting.Type.ToInt32()==(int)taskType)
            {
                switch (taskType)
                {
                    //杀怪拿东西
                    case TaskType.KillMonsterGetItem:
                        List<int> itemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        ItemData item = ItemManager.Instance.FindItemBySettingId(itemParam[0]);
                        if (item != null)
                        {
                            if (item.count >= (ulong)itemParam[1])
                            {
                                AccomplishGuideBook(data);
                            }
                        }
                        
                        break;
                    //收集物品
                    case TaskType.ReceiveItem:

                        List<int> itemParam2 = CommonUtil.SplitCfgOneDepth(setting.Param);
                        ItemData item2 = ItemManager.Instance.FindItemBySettingId(itemParam2[0]);
                        if (item2!=null&&
                            item2.count >= (ulong)itemParam2[1])
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                     //炼丹
                    case TaskType.LianDan:
                        List<int> liandanParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int itemType = liandanParam[0];
                        int num = liandanParam[1];
                        if (ItemManager.Instance.FindItemListByType((ItemType)itemType).Count >= num)
                        {
                            //完成任务
                            AccomplishGuideBook(data);
                        }
                        break;
                    //打怪
                    case TaskType.KillEnemy:
                        List<int> enemyParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int enemyId = enemyParam[0];
                        int needKillEnemyNum = enemyParam[1];
                        int enemyNum = FindAchievement(AchievementType.KillEnmey, enemyId.ToString()).ToInt32();
                        if (enemyNum >= needKillEnemyNum)
                        {
                            //完成任务
                            AccomplishGuideBook(data);
                        }
                        break;
 
                    //引导建个丹炉
                    case TaskType.DanFarmNum:
                        List<int> farmParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int needFarmId = farmParam[0];
                        int needFarmNum = farmParam[1];
                        if (RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count > 0)
                        {
                            int validNumFarm = 0;
                            for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; j++)
                            {
                                SingleDanFarmData singleDanFarmData = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j];
                                if (singleDanFarmData.IsEmpty)
                                    continue;
                                if (singleDanFarmData.Status == (int)DanFarmStatusType.Building)
                                    continue;
                                if (needFarmId == (int)DanFarmIdType.Any)
                                {

                                    validNumFarm++;

                                }
                                else
                                {
                                    if (needFarmId == singleDanFarmData.SettingId)
                                    {
                                        validNumFarm++;
                                    }
                                }
                            }
                            if (validNumFarm >= needFarmNum)
                            {
                                AccomplishGuideBook(data);

                            }

                        }
                        break;
                    //引导招募
                    case TaskType.ZhaoMuDiZi:
                        int recruitStudentNum = setting.Param.ToInt32();
                        if (RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count >= recruitStudentNum)
                        {
                            //完成任务
                            AccomplishGuideBook(data);
                        }
                        break;
     
                    //炼制法器数量
                    case TaskType.EquipNum:
                        List<int> equipParam = CommonUtil.SplitCfgOneDepth(setting.Param);

                        ItemIdType idType = (ItemIdType)equipParam[0];
                        int needequipNum = equipParam[1];
                        //curTaskSetting.needNum==1
                        int haveNum = ItemManager.Instance.FindAllEquipmentList().Count;

                        if (haveNum >= needequipNum)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;

                    //炼制某等级法器数量
                    case TaskType.RarityEquipNum:
                        List<int> RarityequipParam = CommonUtil.SplitCfgOneDepth(setting.Param);

                        int rarity = RarityequipParam[0];
                        int needRarityEquipNum = RarityequipParam[1];
                        //curTaskSetting.needNum==1
                        int haveRarityNum = 0;
                        for (int j = 0; j < RoleManager.Instance._CurGameInfo.ItemModel.itemDataList.Count; j++)
                        {
                            int settingId = RoleManager.Instance._CurGameInfo.ItemModel.itemDataList[j].settingId;
                            ItemSetting itemSetting = DataTable.FindItemSetting(settingId);
                            if (itemSetting!=null  && itemSetting.Rarity.ToInt32() == rarity&&itemSetting.ItemType.ToInt32()==(int)ItemType.Equip)
                            {
                                haveRarityNum++;

                            }
                        }
                        List<PeopleData> RarityequipParampList = StudentManager.Instance.FindAllMyTalentStudentList(StudentTalent.LianGong);
                        for (int j = 0; j < RarityequipParampList.Count; j++)
                        {
                            PeopleData p = RarityequipParampList[j];
                            for(int k = 0; k < p.curEquipItemList.Count; k++)
                            {
                                ItemData equip = p.curEquipItemList[k];
                                if (equip != null
                         && equip.equipProtoData != null)
                                {
                                    int settingId = equip.settingId;
                                    ItemSetting itemSetting = DataTable.FindItemSetting(settingId);

                                    if (itemSetting!=null && itemSetting.Rarity.ToInt32() == rarity)
                                    {
                                        haveRarityNum++;

                                    }
                                }
                            }
                     
                        }
                        if (haveRarityNum >= needRarityEquipNum)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //完成地图事件次数
                    case TaskType.AccomplishMapEvent:
                        List<int> mapEventParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int theMapEventType = mapEventParam[0];
                        int needNum = mapEventParam[1];
                        int mapEventNum = FindAchievement(AchievementType.AccomplishedMapEvent, theMapEventType.ToString()).ToInt32();
                        if (mapEventNum >= needNum)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                        //玩家修为
                    case TaskType.PlayerLevel:
                        int level = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex + 1;
                        if (level >= setting.Param.ToInt32())
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //装备技能
                    case TaskType.EquipSkill:
                        int skillId = setting.Param.ToInt32();
                      
                        if (RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.equippedSkillIdList.Count >= 2)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //学习技能
                    case TaskType.StudySkill:
                        int StudySkillId = setting.Param.ToInt32();
                        if (StudySkillId == -1)
                        {
                            if(RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count>=2)
                                AccomplishGuideBook(data);

                        }
                        else
                        {
                            for (int j = 0; j < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; j++)
                            {
                                SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[j];
                                if (singleSkillData.skillId == StudySkillId)
                                    AccomplishGuideBook(data);
                            }
                        }
                     

                        break;
                    //升级技能
                    case TaskType.UpgradeSkill:
                        List<int> upgradeSkillParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int UpgradeSkillId = upgradeSkillParamList[0];
                        int UpgradeSkillLevel = upgradeSkillParamList[1];
                        for (int j = 1; j < RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList.Count; j++)
                        {
                            SingleSkillData singleSkillData = RoleManager.Instance._CurGameInfo.playerPeople.allSkillData.skillList[j];
                            if ((singleSkillData.skillId == UpgradeSkillId
                                ||UpgradeSkillId==0)
                                && singleSkillData.skillLevel >= UpgradeSkillLevel)
                                AccomplishGuideBook(data);
                        }
                        break;
                        //升级宗门
                    case TaskType.UpgradeZongMen:
                        int zongmenLevelNeed = setting.Param.ToInt32();
                        if (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel >= zongmenLevelNeed)
                        {
                            AccomplishGuideBook(data);
                        }

                        break;
                    case TaskType.QuanLiNum:
                        int quanLiNumNeed = setting.Param.ToInt32();
                        if (FindAchievement(AchievementType.QuanLiTime, "").ToInt32() >= quanLiNumNeed)
                        {
                            AccomplishGuideBook(data);
                        }

                        break;
                    //驻守建筑 建筑id|弟子数量|如果有 则是弟子天赋
                    case TaskType.StudentZuoZhen:
                        List<int> StudentZuoZhenParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int StudentZuoZhenfarmId = StudentZuoZhenParamList[0];
                        int StudentZuoZhenNum = StudentZuoZhenParamList[1];
                        bool haveTalentCondition = false;//有天赋需求
                        StudentTalent needTalent = StudentTalent.None;
                        if (StudentZuoZhenParamList.Count >= 3)
                        {
                            haveTalentCondition = true;
                            needTalent = (StudentTalent)StudentZuoZhenParamList[2];
                        }
                        int StudentZuoZhenvalidNum = 0;
                        for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                            if (p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                                || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                                || p.studentStatusType == (int)StudentStatusType.DanFarmWork)
                            {
                                SingleDanFarmData danFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(p.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[p.zuoZhenDanFarmIndex];
                                if (danFarmData.SettingId == StudentZuoZhenfarmId)
                                {
                                    if (haveTalentCondition)
                                    {
                                        if (p.talent == (int)needTalent)
                                        {
                                            StudentZuoZhenvalidNum++;
                                        }
                                    }
                                    else
                                    {
                                        StudentZuoZhenvalidNum++;
                                    }
                                }

                            }
                        }
                        if (StudentZuoZhenvalidNum >= StudentZuoZhenNum)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //拥有a个b级c建筑 
                    case TaskType.HaveABLevelCFarm:
                        List<int> HaveABLevelCFarmParamList = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int farmId = HaveABLevelCFarmParamList[2];
                        int needLevel = HaveABLevelCFarmParamList[1];
                        int needHaveABLevelCFarmParamNum = HaveABLevelCFarmParamList[0];
                        int validHaveABLevelCFarmParamNum = 0;
                        for (int j = 0; j < RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList.Count; j++)
                        {
                            SingleDanFarmData danFarm = RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[j];
                            if (danFarm.SettingId == farmId
                                && danFarm.CurLevel >= needLevel)
                            {
                                validHaveABLevelCFarmParamNum++;
                            }

                        }
                        if (validHaveABLevelCFarmParamNum >= needHaveABLevelCFarmParamNum)
                        {
                            AccomplishGuideBook(data);
                        }

                        break;
                    //天赋觉醒次数
                    case TaskType.TianFuJueXingNum:
                        int TianFuJueXingNumNeed = setting.Param.ToInt32();
                        if (FindAchievement(AchievementType.TianFuJueXingNum, "").ToInt32() >= TianFuJueXingNumNeed)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //弟子突破次数
                    case TaskType.StudentUpgradeCount:
                        int StudentUpgradeCountNeed = setting.Param.ToInt32();
                        if (FindAchievement(AchievementType.StudentUpgradeCount, "").ToInt32() >= StudentUpgradeCountNeed)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //升级法器等级
                    case TaskType.UpgradeEquip:
                        int UpgradeEquipNeed = setting.Param.ToInt32();
                        List<ItemData> equipItemList = ItemManager.Instance.FindAllEquipmentList();
                        for (int j = 0; j < equipItemList.Count; j++)
                        {
                            ItemData itemData = equipItemList[j];
                            if (itemData.equipProtoData.curLevel >= UpgradeEquipNeed)
                            {
                                AccomplishGuideBook(data);
                                break;
                            }
                        }
                        break;
                    //升级某级别法器等级
                    case TaskType.UpgradeRarityEquip:
                        List<int> UpgradeRarityEquipNeed = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int needUpgradeRarityEquipNeedRarity = UpgradeRarityEquipNeed[0];
                        int needUpgradeRarityEquipNeedLevel = UpgradeRarityEquipNeed[1];
                        List<ItemData> UpgradeRarityEquipNeedequipItemList = ItemManager.Instance.FindAllEquipmentList();
                        for (int j = 0; j < UpgradeRarityEquipNeedequipItemList.Count; j++)
                        {
                            ItemData itemData = UpgradeRarityEquipNeedequipItemList[j];
                            if (itemData.quality==needUpgradeRarityEquipNeedRarity
                                && itemData.equipProtoData!=null && itemData.equipProtoData.curLevel >= needUpgradeRarityEquipNeedLevel)
                            {
                                AccomplishGuideBook(data);
                                break;
                            }
                        }
                        break;
                    //解锁空地数量
                    case TaskType.UnlockFarmPosNum:
                        int UnlockFarmPosNumNeed = setting.Param.ToInt32();
                        if (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum >= UnlockFarmPosNumNeed)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //通过哪一关
                    case TaskType.PassLevel:
                        string needpassLevel = setting.Param;
                        string myPassedMaxLevel = FindAchievement(AchievementType.PassedMaxLevel);
                        int needLogicLevel = ConstantVal.GetLevelByLevelStr(needpassLevel);
                        int myLogicLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLevel);
                        if (myLogicLevel >= needLogicLevel)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //通过哪一关裂隙
                    case TaskType.PassedMaxLieXiLevel:
                        string needpassLieXiLevel = setting.Param;
                        string myPassedMaxLieXiLevel = FindAchievement(AchievementType.PassedMaxLieXiLevel);
                        int needLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(needpassLieXiLevel);
                        int myLogicLieXiLevel = ConstantVal.GetLevelByLevelStr(myPassedMaxLieXiLevel);
                        if (myLogicLieXiLevel >= needLogicLieXiLevel)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //弟子上阵
                    case TaskType.ShangZhen:
                        int needShangZhenNum = setting.Param.ToInt32();
                        if (TeamManager.Instance.FindMyTeam1PNum() >= needShangZhenNum)
                        {
                            AccomplishGuideBook(data);
                        }

                        break;
                    //装备法器
                    case TaskType.EquipEquip:
                        for(int j=0;j< RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList.Count; j++)
                        {
                            if (RoleManager.Instance._CurGameInfo.playerPeople.curEquipItemList[j] != null)
                            {
                                AccomplishGuideBook(data);
                                break;
                            }
                  
                        }
                     
                        break;
                    //炼制宝石
                    case TaskType.MakeGem:
                        List<ItemData> gemItemList = ItemManager.Instance.FindItemListByType(ItemType.Gem);
                        if (gemItemList.Count >= setting.Param.ToInt32())
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //镶嵌某级别宝石
                    case TaskType.InlayRarityGem:
                        List<int> inlayGemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int validGemNum = 0;
                        List<ItemData> equipList = ItemManager.Instance.FindAllEquipmentList();
                        for (int j = 0; j < equipList.Count; j++)
                        {
                            ItemData itemData = equipList[j];
                            if (itemData.equipProtoData != null)
                            {
                                for (int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                                {
                                    ItemData gem = itemData.equipProtoData.gemList[k];
                                    if (gem != null&&gem.onlyId>0)
                                    {
                                        if (gem.quality == inlayGemParam[0])
                                            validGemNum++;
                                    }
                                }
                            }
                        }
                        //for (int j = 0; j < RoleManager.Instance._CurGameInfo.itemModel.itemDataList.Count; j++)
                        //{
                        //    ItemData itemData = RoleManager.Instance._CurGameInfo.itemModel.itemDataList[j];
                        //    if (itemData.equipProtoData != null)
                        //    {
                        //        for(int k = 0; k < itemData.equipProtoData.gemList.Count; k++)
                        //        {
                        //            ItemData gem = itemData.equipProtoData.gemList[k];
                        //            if (gem != null)
                        //            {
                        //                if (gem.quality == inlayGemParam[0])
                        //                    validGemNum++;
                        //            }
                        //        }
                        //    }
                        //}
                        //List<PeopleData> studentList = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                        //for(int j = 0; j < studentList.Count; j++)
                        //{
                        //    PeopleData p = studentList[j];
                        //    for(int k = 0; k < p.curEquipItemList.Count; k++)
                        //    {
                        //        ItemData equip = p.curEquipItemList[k];
                        //        if (equip != null
                        //     && equip.equipProtoData != null)
                        //        {
                        //            for (int m = 0; m < equip.equipProtoData.gemList.Count; m++)
                        //            {
                        //                ItemData gem = equip.equipProtoData.gemList[m];
                        //                if (gem != null)
                        //                {
                        //                    if (gem.quality == inlayGemParam[0])
                        //                        validGemNum++;
                        //                }
                        //            }
                        //        }
                        //    }
                         
                        //}
                        if (validGemNum >= inlayGemParam[1])
                        {
                            AccomplishGuideBook(data);
                        }
                        break;

                    case TaskType.StudentHaveLevelGem:
                        List<int> studentHaveGemParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int validStudentGemNum = 0;
                        List<PeopleData> studentList2 = StudentManager.Instance.GetTypeStudent(StudentTalent.LianGong);
                        for (int j = 0; j < studentList2.Count; j++)
                        {
                            PeopleData p = studentList2[j];
                            for(int k = 0; k < p.curEquipItemList.Count; k++)
                            {
                                ItemData equip = p.curEquipItemList[k];
                                if (equip != null
                             && equip.equipProtoData != null)
                                {
                                    for (int m = 0; m < equip.equipProtoData.gemList.Count; m++)
                                    {
                                        ItemData gem = equip.equipProtoData.gemList[m];

                                        if (gem != null&&gem.onlyId>0)
                                        {
                                            GemSetting gemSetting = DataTable.FindGemSetting(gem.settingId);
                                            if (gemSetting.Level.ToInt32() == studentHaveGemParam[0]
                                                || studentHaveGemParam[0] == 0)
                                                validStudentGemNum++;
                                        }
                                    }
                                }
                            }
                         
                        }
                        if (validStudentGemNum >= studentHaveGemParam[1])
                        {
                            AccomplishGuideBook(data);
                        }

                        break;
                    //拥有a名b天赋弟子
                    case TaskType.HaveATalentBStudent:
                        List<int> HaveATalentBStudentParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int needTalentNum = HaveATalentBStudentParam[0];
                        int needTalentTalent = HaveATalentBStudentParam[1];
                        int myNum = 0;
                         for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                            if(p.talent== needTalentTalent
                                ||needTalentTalent==0)
                            {
                                myNum++;
                            }
                        }
                        if (myNum >= needTalentNum)
                        {
                            AccomplishGuideBook(data);
                        }
                        break;
                    //讨伐
                    case TaskType.TaoFa:
                        List<int> TaoFaParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int taoFaType = TaoFaParam[0];
                        int taoFaNum = TaoFaParam[1];
                        if (FindAchievement(AchievementType.TaoFa, taoFaType.ToString()).ToInt32() >= taoFaNum)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //炼丹
                    case TaskType.LianDan2:
                        List<int> lianDan2Param = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int LianDan2Id = lianDan2Param[0];
                        int LianDan2Num = lianDan2Param[1];
                        if (FindAchievement(AchievementType.LianDan2, LianDan2Id.ToString()).ToInt32() >= LianDan2Num)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //坐镇总数
                    case TaskType.StudentZuoZhenTotalNum:
                        int needZuoZhenNum = setting.Param.ToInt32();
                        int totalZuoZhenNum = 0;
                        for (int j = 0; j < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; j++)
                        {
                            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[j];
                            if (p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
                                || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
                                || p.studentStatusType == (int)StudentStatusType.DanFarmWork)
                            {
                                totalZuoZhenNum++;

                            }
                        }
                        if (totalZuoZhenNum >= needZuoZhenNum)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //秘境次数
                    case TaskType.Explore:
                        List<int> ExploreParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int needExploreNum = ExploreParam[1];
                        int needExploreId = ExploreParam[0];
                        int validExploreNum = FindAchievement(AchievementType.Explore, needExploreId.ToString()).ToInt32();
                        if (validExploreNum >= needExploreNum)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
                    //最大讨伐
                    case TaskType.MaxTaoFa:
                        List<int> MaxTaoFaParam = CommonUtil.SplitCfgOneDepth(setting.Param);
                        int MaxTaoFaType = MaxTaoFaParam[0];
                        int needMaxTaoFaLevel = MaxTaoFaParam[1];
                        int myMaxTaoFaLevel = TaskManager.Instance.FindAchievement(AchievementType.MaxTaoFa, MaxTaoFaType.ToString()).ToInt32();
                        if (myMaxTaoFaLevel >= needMaxTaoFaLevel)
                        {
                            AccomplishGuideBook(data);

                        }
                        break;
            
                }

            }
        }
    }
    public void AccomplishGuideBook(SingleGuideBookTaskData singleGuideBookTaskData)
    {
        if (singleGuideBookTaskData.accomplishStatus == (int)AccomplishStatus.Processing)
        {
            singleGuideBookTaskData.accomplishStatus = (int)AccomplishStatus.Accomplished;

            GuideBookSetting setting = DataTable.FindGuideBookSetting(singleGuideBookTaskData.settingId);
            int chapter= setting.Chapter.ToInt32();
            TryRevealNextChapterGuideBook(chapter);
            RefreshRedPointShow(singleGuideBookTaskData);
        }
    }

    /// <summary>
    /// 试图解锁下一章
    /// </summary>
    void TryRevealNextChapterGuideBook(int curchapter)
    {
        List<SingleGuideBookTaskData> chapterList = FindChapterGuideBookList(curchapter);
        int total = chapterList.Count;
        int awardedNum = 0;
        for (int i = 0; i < chapterList.Count; i++)
        {
            SingleGuideBookTaskData data = chapterList[i];
            if (data.accomplishStatus == (int)AccomplishStatus.GetAward)
            {
                awardedNum++;
            }
        }
        float rate = awardedNum / (float)total;
        SingleChapterGuideBookData singleChapterGuideBookData = FindChapterGuideBookData(curchapter);

        //定位到已领过的
        for (int i = 0; i < singleChapterGuideBookData.processAccomplishStatus.Count; i++)
        {
            if (rate * 10 >= i + 1)
            {
                if (singleChapterGuideBookData.processAccomplishStatus[i] == (int)AccomplishStatus.UnAccomplished)
                    singleChapterGuideBookData.processAccomplishStatus[i] = (int)AccomplishStatus.Accomplished;
            }
        }
        if (rate >= 1)
        {
            //该解锁下一章了
            int maxChapter = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count;
            if (curchapter < maxChapter)
            {
                RevealChapterGuideBookTask(curchapter + 1);
                TryAccomplishAllGuideBook();
            }
        }
    }
    /// <summary>
    /// 解锁章节的任务
    /// </summary>
    public void RevealChapterGuideBookTask(int chapter)
    {
        SingleChapterGuideBookData chapterData = FindChapterGuideBookData(chapter);
        if (!chapterData.reveal)
        {
            chapterData.reveal = true;
            List<SingleGuideBookTaskData> taskList = FindChapterGuideBookList(chapter);
            for (int i = 0; i < taskList.Count; i++)
            {
                SingleGuideBookTaskData data = taskList[i];
                GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
                if (string.IsNullOrWhiteSpace(setting.ForeTask) && data.accomplishStatus == (int)AccomplishStatus.Locked)
                {
                    data.accomplishStatus = (int)AccomplishStatus.Processing;
                }
            }

        }

    }
    /// <summary>
    /// 拿奖
    /// </summary>
    /// <param name="singleGuideBookTaskData"></param>
    public void OnGetGuideBookAward(SingleGuideBookTaskData singleGuideBookTaskData,GuideBookSetting guideBookSetting)
    {
        if (singleGuideBookTaskData.accomplishStatus ==(int)AccomplishStatus.Accomplished)
        {
            List<List<int>> award = CommonUtil.SplitCfg(guideBookSetting.Award);
            for(int i = 0; i < award.Count; i++)
            {
                List<int> single = award[i];
                ItemManager.Instance.GetItemWithTongZhiPanel(single[0], (ulong)single[1]);
            }
            singleGuideBookTaskData.accomplishStatus = (int)AccomplishStatus.GetAward;
            RevealNextGuideBook(guideBookSetting);
            TryRevealNextChapterGuideBook(guideBookSetting.Chapter.ToInt32());
            InitRedPoint();
            EventCenter.Broadcast(TheEventType.OnGetGuideBookAward);
        }
    }
    /// <summary>
    /// 拿过程奖励
    /// </summary>
    /// <param name="singleGuideBookTaskData"></param>
    public void OnGetGuideBookChapterProcessAward(SingleChapterGuideBookData singleChapterGuideBookData, int index)
    {
        if (singleChapterGuideBookData.processAccomplishStatus[index] == (int)AccomplishStatus.Accomplished)
        {
            GuideBookSetting setting= DataTable.FindAnyGuideBookSettingByChapter(singleChapterGuideBookData.chapter);

            if (!string.IsNullOrWhiteSpace(setting.ChapterAward))
            {
                List<List<List<int>>> award = CommonUtil.SplitThreeCfg(setting.ChapterAward);
                List<int> settingIdList = new List<int>();
                List<ulong> countList = new List<ulong>();
                for (int i = 0; i < award.Count; i++)
                {
                    if (i == index)
                    {
                        List<List<int>> singleProcessAward = award[i];
                        for (int j = 0; j < singleProcessAward.Count; j++)
                        {
                            List<int> single = singleProcessAward[j];
                            if (single.Count == 2)
                            {
                                settingIdList.Add( single[0]);
                               countList.Add((ulong)single[1]);
                            }
                        }

                    }

                }
                ItemManager.Instance.GetItemWithAwardPanel(settingIdList, countList);

            }
            singleChapterGuideBookData.processAccomplishStatus[index] = (int)AccomplishStatus.GetAward;
            RefreshRedPointShow(singleChapterGuideBookData);
            EventCenter.Broadcast(TheEventType.OnGetGuideBookAward);

        }

 
    }
    /// <summary>
    /// 刷新所有手札完成情况
    /// </summary>
    public void RefreshAllGuideBookStatus()
    {
        List<SingleGuideBookTaskData> validList = new List<SingleGuideBookTaskData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            if (data.accomplishStatus == (int)AccomplishStatus.GetAward)
            {
                if (!string.IsNullOrWhiteSpace(setting.NextTask))
                {
                    RevealNextGuideBook(setting);
                }
            }
        }
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count; i++)
        {
            SingleChapterGuideBookData chapterData = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList[i];
            TryRevealNextChapterGuideBook(chapterData.chapter);
        }
   
    }
    /// <summary>
    /// 揭露下一个任务
    /// </summary>
    public void RevealNextGuideBook(GuideBookSetting setting)
    {
        if (!string.IsNullOrWhiteSpace(setting.NextTask))
        {
            SingleGuideBookTaskData data = FindGuideBookTaskDataById(setting.NextTask.ToInt32());
            GuideBookSetting nextSetting = DataTable.FindGuideBookSetting(data.settingId);
            if (data != null)
            {
                if (data.accomplishStatus ==(int)AccomplishStatus.GetAward)
                {
                    if (!string.IsNullOrWhiteSpace(nextSetting.NextTask))
                    {
                        RevealNextGuideBook(nextSetting);
                    }
                }
                else if (data.accomplishStatus == (int)AccomplishStatus.Locked)
                {
                    data.accomplishStatus = (int)AccomplishStatus.Processing;
                    TryAccomplishGuideBook((TaskType)setting.Type.ToInt32());
                }

            }
        }
        
    }

    /// <summary>
    /// 通过id找手札任务data
    /// </summary>
    /// <returns></returns>
    public SingleGuideBookTaskData FindGuideBookTaskDataById(int id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            if (data.settingId == id)
                return data;
        }
        return null;
    }

    /// <summary>
    /// 找一章的任务
    /// </summary>
    /// <param name="chapter"></param>
    /// <returns></returns>
    public List<SingleGuideBookTaskData> FindChapterGuideBookList(int chapter)
    {
        List<SingleGuideBookTaskData> res = new List<SingleGuideBookTaskData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(data.settingId);
            if (setting.Chapter.ToInt32() == chapter)
                res.Add(data);
        }
        return res;
    }
    /// <summary>
    /// 通过任务找到单章手札
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleChapterGuideBookData FindSingleChapterBookDataByTaskId(int id)
    {
        GuideBookSetting setting = DataTable.FindGuideBookSetting(id);
        int chapter = setting.Chapter.ToInt32();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count; i++)
        {
            SingleChapterGuideBookData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList[i];
            if (data.chapter == chapter)
            {
                return data;
            }
    
        }
        return null;
    }
    /// <summary>
    /// 找一章的任务进度数据
    /// </summary>
    /// <param name="chapter"></param>
    /// <returns></returns>
    public SingleChapterGuideBookData FindChapterGuideBookData(int chapter)
    {
         for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count; i++)
        {
            SingleChapterGuideBookData data = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList[i];
            if (data.chapter == chapter)
                return data;
        }
        return null;
    }
    public void InitRedPoint()
    {
        ClearRedPoint();
        RedPoint main = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task, 0);


        #region 冒险手札
        RedPoint MainPanel_Btn_GuideBookTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_GuideBookTask, 0);
        RedPointManager.Instance.BindRedPoint(main, MainPanel_Btn_GuideBookTask);
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count; i++)
        {
            SingleGuideBookTaskData singleGuideBookTaskData = RoleManager.Instance._CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = DataTable.FindGuideBookSetting(singleGuideBookTaskData.settingId);
            int chapter = setting.Chapter.ToInt32();
            RedPoint MainPanel_Btn_SingleChapterTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterTask, chapter);

            for(int j = 0; j < 10; j++)
            {
                RedPoint MainPanel_Btn_SingleChapterProcessTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterProcessTask, 100 * chapter + j);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_SingleChapterTask, MainPanel_Btn_SingleChapterProcessTask);

            }

            if (singleGuideBookTaskData.accomplishStatus == (int)AccomplishStatus.Processing
                || singleGuideBookTaskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
            { 
                RedPoint MainPanel_Btn_SingleGuideBookTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleGuideBookTask, singleGuideBookTaskData.settingId);
                RedPointManager.Instance.BindRedPoint(MainPanel_Btn_SingleChapterTask, MainPanel_Btn_SingleGuideBookTask);

            } 
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_GuideBookTask, MainPanel_Btn_SingleChapterTask);

            RefreshRedPointShow(singleGuideBookTaskData);
        }
        for(int i=0;i < RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList.Count; i++)
        {
            RefreshRedPointShow(RoleManager.Instance._CurGameInfo.AllGuideBookData.singleChapterList[i]);
        }
        #endregion

        #region 每日任务
        RedPoint MainPanel_Btn_DailyTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_DailyTask, 0);
        RedPointManager.Instance.BindRedPoint(main, MainPanel_Btn_DailyTask);
        for(int i = 0; i < 5; i++)
        {
            RedPoint MainPanel_Btn_Task_DailyTask_Process = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_DailyTask_Process, i);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_DailyTask, MainPanel_Btn_Task_DailyTask_Process);
        }
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList.Count; i++)
        {
            SingleDailyTaskData data= RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList[i];
            RedPoint MainPanel_Btn_Task_DailyTask_SingleDailyTask = RedPointManager.Instance.GetRedPointFromDic(RedPointType.MainPanel_Btn_Task_DailyTask_SingleDailyTask, data.settingId);
            RedPointManager.Instance.BindRedPoint(MainPanel_Btn_DailyTask, MainPanel_Btn_Task_DailyTask_SingleDailyTask);
        }
        RedPointManager.Instance.BindRedPoint(main, MainPanel_Btn_DailyTask);

        #endregion
    }
    public void RefreshRedPointShow(SingleGuideBookTaskData guideBookTaskData)
    {
        bool show = false;
        if (guideBookTaskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
        {
            show = true;
        }
        RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleGuideBookTask, guideBookTaskData.settingId, show);
        EventCenter.Broadcast(TheEventType.RefreshGuideBookRedPoint);
    }
    public void RefreshRedPointShow(SingleChapterGuideBookData chapterProcessData)
    {
        for(int i = 0; i < chapterProcessData.processAccomplishStatus.Count; i++)
        {
            bool show = false;
            if (chapterProcessData.processAccomplishStatus[i] ==(int)AccomplishStatus.Accomplished)
            {
                show = true;
            }
            else
            {
                show = false;
            }
            RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterProcessTask, chapterProcessData.chapter * 100 + i, show);

        }
      
         EventCenter.Broadcast(TheEventType.RefreshGuideBookRedPoint);
    }

    public void RefreshDailyRedPointShow(SingleDailyTaskData singleDailyTaskData)
    {
        bool show = false;
        if (singleDailyTaskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
            show = true;
        RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Task_DailyTask_SingleDailyTask, singleDailyTaskData.settingId, show);
        EventCenter.Broadcast(TheEventType.RefreshDailyTaskRedPoint);

    }
    /// <summary>
    /// 日常进程奖励
    /// </summary>
    /// <param name="singleDailyTaskData"></param>
    public void RefreshDailyProcessRedPointShow(int index)
    {
        bool show = false;
        if (RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[index]==(int)AccomplishStatus.Accomplished)
            show = true;
        RedPointManager.Instance.ChangeRedPointStatus(RedPointType.MainPanel_Btn_Task_DailyTask_Process, index, show);
        EventCenter.Broadcast(TheEventType.RefreshDailyTaskRedPoint);

    }
    /// <summary>
    /// 清掉所有红点
    /// </summary>
    public void ClearRedPoint()
    {
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_GuideBookTask);

        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterTask);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleChapterProcessTask);

        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_GuideBookTask_SingleGuideBookTask);

        #region 每日任务
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_DailyTask);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_DailyTask_Process);
        RedPointManager.Instance.ClearTypeRedPoint(RedPointType.MainPanel_Btn_Task_DailyTask_SingleDailyTask);

        #endregion


    }

    /// <summary>
    /// 获得成就
    /// </summary>
    public void GetAchievement(AchievementType achievementType,string param)
    {
        switch (achievementType)
        {
            case AchievementType.KillEnmey:
                int enemyId = param.ToInt32();
                List<List<int>> enemyRec =CommonUtil.SplitCfg(RoleManager.Instance._CurGameInfo.AllAchievementData.KilledEnemy);
                bool haveSameEnemyRec = false;
                for(int i = enemyRec.Count-1; i >=0 ; i--)
                {
                    List<int> theEnemyRec = enemyRec[i];
                    if (theEnemyRec.Count == 1)
                        enemyRec.RemoveAt(i);
                    else
                    {
                        int theId = theEnemyRec[0];
                        if (theId == enemyId)
                        {
                            haveSameEnemyRec = true;
                            theEnemyRec[1]++;
                            break;
                        }
                    }

                }
                //如果没记录
                if (!haveSameEnemyRec)
                {
                    List<int> singleEnemyRec = new List<int>();
                    singleEnemyRec.Add(enemyId);
                    singleEnemyRec.Add(1);
                    enemyRec.Add(singleEnemyRec);
                }
                string res = "";
                for(int i = 0; i < enemyRec.Count; i++)
                {
                    List<int> singleEnemyRec = enemyRec[i];
                   res+=singleEnemyRec[0] + "|" + singleEnemyRec[1]+"$";
                }
                res.Remove(res.Length-1);
                RoleManager.Instance._CurGameInfo.AllAchievementData.KilledEnemy = res;
                for(int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
                {
                    ulong npcOnlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];
                    SingleNPCData curNPCData1 = FindNPCByOnlyId(npcOnlyId);
                    NPC npcSetting1 = DataTable.FindNPCArrById(curNPCData1.Id);
                    SingleTask taskSetting1 = FindTaskSettingById(npcSetting1, curNPCData1.CurTaskId);
                    //任务需要数量++
                    if (taskSetting1 != null
                        && taskSetting1.isNeedKillEnemy
                        && taskSetting1.needKillEnemy == (EnemyIdType)enemyId)
                    {
                        SingleTaskProtoData curTaskData = FindTaskById(curNPCData1, curNPCData1.CurTaskId);

                        curTaskData.CurNum++;
                        //检查任务是否完成
                        TryAccomplishTask(curNPCData1);
                    }
                }

       
                break;
            case AchievementType.QieCuo:
                ulong onlyId = param.ToUInt64();
                SingleNPCData curNPCData = FindNPCByOnlyId(onlyId);
                NPC npcSetting = DataTable.FindNPCArrById(curNPCData.Id);
                SingleTask taskSetting = FindTaskSettingById(npcSetting, curNPCData.CurTaskId);
                //任务需要数量++
                if (curNPCData!=null
                    && taskSetting != null
                    && taskSetting.taskType==TaskType.QieCuo
                    && curNPCData.PeopleData.onlyId == onlyId)
                {
                    SingleTaskProtoData curTaskData = FindTaskById(curNPCData, curNPCData.CurTaskId);
                    curTaskData.CurNum++;
                }

                break;
                //通过的最大地图
            case AchievementType.PassedMaxMap:
                int mapLevel = param.ToInt32();
                if (mapLevel >= RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxMapLevel)
                    RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxMapLevel = mapLevel;
                break;
                //完成了事件（类型）次数
            case AchievementType.AccomplishedMapEvent:
                int theMapEvent = param.ToInt32();
                List<int> theMapEventIdList = new List<int>();
                List<int> theMapEventNumList = new List<int>();

                for (int i=0;i< RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent.Count; i++)
                {
                    string single = RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent[i];
                    List<int> singleList = CommonUtil.SplitCfgOneDepth(single);
                    theMapEventIdList.Add(singleList[0]);
                    theMapEventNumList.Add(singleList[1]);
                }
                if (!theMapEventIdList.Contains(theMapEvent))
                {
                    theMapEventIdList.Add(theMapEvent);
                    theMapEventNumList.Add(0);
                }
                int index = theMapEventIdList.IndexOf(theMapEvent);
                theMapEventNumList[index]++;
                RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent.Clear();
                //List<string>
                //再重新保存
                for (int i = 0; i < theMapEventIdList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent.Add( theMapEventIdList[i] +"|"+ theMapEventNumList[i]);
                }

                break;
            case AchievementType.OnceGuide:
                int guideId = param.ToInt32();
                if (!RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedOnceGuideList.Contains(guideId))
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedOnceGuideList.Add(guideId);
                }
                break;
            case AchievementType.QuanLiTime:
                //if (!RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedOnceGuideList.Contains(guideId))
                //{
                //    RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedOnceGuideList.Add(guideId);
                //}
                RoleManager.Instance._CurGameInfo.AllAchievementData.QuanLiNum++;

                break;
                //使用特殊物品次数
            case AchievementType.UseSpecialTypeItemCount:
                break;
            case AchievementType.TianFuJueXingNum:
                RoleManager.Instance._CurGameInfo.AllAchievementData.TianFuJueXingNum++;
                break;
            case AchievementType.StudentUpgradeCount:
                RoleManager.Instance._CurGameInfo.AllAchievementData.StudentUpgradeCount++;
                break;
            case AchievementType.PassedMaxLevel:
                //List<int> levelList = CommonUtil.SplitCfgOneDepth(param);
                int logicLevel = ConstantVal.GetLevelByLevelStr(param);
                int logicRecordLevel = 0;
                if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLevel))
                {
                    string[] recordlevelList = RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLevel.Split('-');
                     logicRecordLevel = recordlevelList[0].ToInt32() * 1000 + recordlevelList[1].ToInt32();

                    
                }
                if (logicLevel >= logicRecordLevel)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLevel = param;
                }
                break;
            case AchievementType.PassedMaxLieXiLevel:
                //List<int> levelList = CommonUtil.SplitCfgOneDepth(param);
                int logicLieXiLevel = ConstantVal.GetLevelByLevelStr(param);
                int logicLieXiRecordLevel = 0;
                if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLieXiLevel))
                {
                    string[] recordlevelList = RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLieXiLevel.Split('-');
                    logicLieXiRecordLevel = recordlevelList[0].ToInt32() * 1000 + recordlevelList[1].ToInt32();
                }
                if (logicLieXiLevel >= logicLieXiRecordLevel)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLieXiLevel = param;
                }
                break;
            case AchievementType.TaoFa:
                int taoFaId = param.ToInt32();
                List<int> taoFaIdList = new List<int>();
                List<int> taoFaNumList = new List<int>();
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum.Count; i++)
                {
                    string singleStr = RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum[i];
                    List<int> single = CommonUtil.SplitCfgOneDepth(singleStr);
                    taoFaIdList.Add(single[0]);
                    taoFaNumList.Add(single[1]);
                }
                if (!taoFaIdList.Contains(taoFaId))
                {
                    taoFaIdList.Add(taoFaId);
                    taoFaNumList.Add(0);
                }
                int theIndex = taoFaIdList.IndexOf(taoFaId);
                taoFaNumList[theIndex]++;
                RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum.Clear();
                for (int i = 0; i < taoFaIdList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum.Add(taoFaIdList[i] + "|" + taoFaNumList[i]);
                }
                break;
            case AchievementType.LianDan2:
                int LianDan2Id = param.ToInt32();
                List<int> LianDan2IdList = new List<int>();
                List<int> LianDan2NumList = new List<int>();
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num.Count; i++)
                {
                    string singleStr = RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num[i];
                    List<int> single = CommonUtil.SplitCfgOneDepth(singleStr);
                    LianDan2IdList.Add(single[0]);
                    LianDan2NumList.Add(single[1]);
                }
                if (!LianDan2IdList.Contains(LianDan2Id))
                {
                    LianDan2IdList.Add(LianDan2Id);
                    LianDan2NumList.Add(0);
                }
                int theLianDan2IdIndex = LianDan2IdList.IndexOf(LianDan2Id);
                LianDan2NumList[theLianDan2IdIndex]++;
                RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num.Clear();
                for (int i = 0; i < LianDan2IdList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num.Add(LianDan2IdList[i] + "|" + LianDan2NumList[i]);
                }
                break;
                //秘境次数
            case AchievementType.Explore:
                int ExploreId = param.ToInt32();
                List<int> ExploreIdList = new List<int>();
                List<int> ExploreNumList = new List<int>();
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum.Count; i++)
                {
                    string singleStr = RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum[i];
                    List<int> single = CommonUtil.SplitCfgOneDepth(singleStr);
                    ExploreIdList.Add(single[0]);
                    ExploreNumList.Add(single[1]);
                }
                if (!ExploreIdList.Contains(ExploreId))
                {
                    ExploreIdList.Add(ExploreId);
                    ExploreNumList.Add(0);
                }
                int ExploreIdIndex = ExploreIdList.IndexOf(ExploreId);
                ExploreNumList[ExploreIdIndex]++;
                RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum.Clear();
                for (int i = 0; i < ExploreIdList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum.Add(ExploreIdList[i] + "|" + ExploreNumList[i]);
                }
                break;
            //最大讨伐等级
            case AchievementType.MaxTaoFa:
                List<int> maxTaoFaParam = CommonUtil.SplitCfgOneDepth(param);
                int taoFaType = maxTaoFaParam[0];
                int level = maxTaoFaParam[1];
                
                List<int> maxTaoFaTypeList = new List<int>();
                List<int> maxTaoFaLevelList = new List<int>();
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa.Count; i++)
                {
                    string singleStr = RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa[i];
                    List<int> single = CommonUtil.SplitCfgOneDepth(singleStr);
                    maxTaoFaTypeList.Add(single[0]);
                    maxTaoFaLevelList.Add(single[1]);
                }
                if (!maxTaoFaTypeList.Contains(taoFaType))
                {
                    maxTaoFaTypeList.Add(taoFaType);
                    maxTaoFaLevelList.Add(0);
                }
                int maxTaoFaTypeIndex = maxTaoFaTypeList.IndexOf(taoFaType);
                if (maxTaoFaLevelList[maxTaoFaTypeIndex] < level)
                    maxTaoFaLevelList[maxTaoFaTypeIndex] = level;
                 RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa.Clear();
                for (int i = 0; i < maxTaoFaTypeList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa.Add(maxTaoFaTypeList[i] + "|" + maxTaoFaLevelList[i]);
                }
                break;
            case AchievementType.LiLian:
                RoleManager.Instance._CurGameInfo.AllAchievementData.LiLianNum++;
                break;
            case AchievementType.MakeGem:
                RoleManager.Instance._CurGameInfo.AllAchievementData.MakeGemNum++;
                break;
        }
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
        {
            SingleNPCData singleNPCData = FindNPCByOnlyId(RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i]);
            TryAccomplishTask(singleNPCData);

        }
    }

    /// <summary>
    /// 记录通过的关卡id
    /// </summary>
    public void TryRecordPassedTaskTagId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;
        if (!RoleManager.Instance._CurGameInfo.AllAchievementData.PassedTaskTagIdList.Contains(id))
        {
            RoleManager.Instance._CurGameInfo.AllAchievementData.PassedTaskTagIdList.Add(id);
        }
    }

    /// <summary>
    /// 找成就
    /// </summary>
    public string FindAchievement(AchievementType achievementType,string param="")
    {
        switch (achievementType)
        {
            case AchievementType.KillEnmey:
                int enemyId = param.ToInt32();
                List<List<int>> enemyRec = CommonUtil.SplitCfg(RoleManager.Instance._CurGameInfo.AllAchievementData.KilledEnemy);
                for (int i = 0; i < enemyRec.Count; i++)
                {
                    List<int> theEnemyRec = enemyRec[i];
                    int theId = theEnemyRec[0];
                    if (theId == enemyId)
                    {
                        return theEnemyRec[1].ToString();
                    }
                }
                return "0";
                
            case AchievementType.PassedMaxMap:
                return RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxMapLevel.ToString();
            case AchievementType.AccomplishedMapEvent:
                string mapEventType = param;
                for(int i=0;i < RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent.Count; i++)
                {
                    string[] theArr = RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedMapEvent[i].Split('|');
                    string theType = theArr[0];
                    if (theType == mapEventType)
                    {
                       return theArr[1];
                    }
                }
                return "0";
            case AchievementType.OnceGuide:
                int onceGuideId = param.ToInt32();
                if (RoleManager.Instance._CurGameInfo.AllAchievementData.AccomplishedOnceGuideList.Contains(onceGuideId))
                    return "1";
                return "0";
            case AchievementType.QuanLiTime:
      
                    return RoleManager.Instance._CurGameInfo.AllAchievementData.QuanLiNum.ToString();
            case AchievementType.TianFuJueXingNum:
                return RoleManager.Instance._CurGameInfo.AllAchievementData.TianFuJueXingNum.ToString();

            case AchievementType.StudentUpgradeCount:

                return RoleManager.Instance._CurGameInfo.AllAchievementData.StudentUpgradeCount.ToString();

            case AchievementType.PassedMaxLevel:

                return RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLevel;
            case AchievementType.PassedMaxLieXiLevel:

                return RoleManager.Instance._CurGameInfo.AllAchievementData.PassedMaxLieXiLevel;
            case AchievementType.TaoFa:
                string taoFaId = param;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum.Count; i++)
                {
                    string[] theArr = RoleManager.Instance._CurGameInfo.AllAchievementData.TaoFaNum[i].Split('|');
                    string theId = theArr[0];
                    if (theId == taoFaId)
                    {
                        return theArr[1];
                    }
                }
                return "0";
            case AchievementType.LianDan2:
                string LianDan2Id = param;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num.Count; i++)
                {
                    string[] theArr = RoleManager.Instance._CurGameInfo.AllAchievementData.LianDan2Num[i].Split('|');
                    string theId = theArr[0];
                    if (theId == LianDan2Id)
                    {
                        return theArr[1];
                    }
                }
                return "0";
                //秘境次数
            case AchievementType.Explore:
                string ExploreId = param;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum.Count; i++)
                {
                    string[] theArr = RoleManager.Instance._CurGameInfo.AllAchievementData.ExploreNum[i].Split('|');
                    string theId = theArr[0];
                    if (theId == ExploreId)
                    {
                        return theArr[1];
                    }
                }
                return "0";
            //讨伐最大关卡
            case AchievementType.MaxTaoFa:
                string MaxTaoFaType = param;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa.Count; i++)
                {
                    string[] theArr = RoleManager.Instance._CurGameInfo.AllAchievementData.MaxTaoFa[i].Split('|');
                    string theType = theArr[0];
                    if (theType == MaxTaoFaType)
                    {
                        return theArr[1];
                    }
                }
                return "0";
            case AchievementType.LiLian:
                return RoleManager.Instance._CurGameInfo.AllAchievementData.LiLianNum.ToString();
            case AchievementType.MakeGem:
                return RoleManager.Instance._CurGameInfo.AllAchievementData.MakeGemNum.ToString();

        }
        return "";
    }
   
    /// <summary>
    /// 关闭指引
    /// </summary>
    public void CloseTaskGuide(TaskType taskType)
    {
        SingleNPCData singleNPCData = FindNPCByOnlyId(curGuideNPCOnlyId);
        if (singleNPCData == null)
            return;
        NPC npcSetting = DataTable.FindNPCArrById(singleNPCData.Id);
        SingleTask curTaskSetting = FindTaskSettingById(npcSetting, singleNPCData.CurTaskId);
        if ( curTaskSetting == null)
            return;
        if (curTaskSetting.taskType == taskType)
            PanelManager.Instance.CloseTaskGuidePanel();
    }

    /// <summary>
    /// 隐藏掉npc
    /// </summary>
    public void HideNPC(ulong onlyId)
    {
        EventCenter.Broadcast(TheEventType.HideNPC, onlyId);
    }

    /// <summary>
    /// 通过唯一id找npc
    /// </summary>
    /// <param name="onlyId"></param>
    public SingleNPCData FindNPCByOnlyId(ulong onlyId)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData data = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            if (data.OnlyId == onlyId)
            {
                return data;
            }
        }
        return null;
    }
    /// <summary>
    /// 通过id找npc
    /// </summary>
    /// <param name="onlyId"></param>
    public SingleNPCData FindNPCById(int settingId)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData data = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            if (data.Id == settingId)
            {
                return data;
            }
        }
        return null;
    }
    /// <summary>
    /// 移除npc
    /// </summary>
    /// <param name="onlyId"></param>
    public void RemoveNPC(ulong onlyId)
    {
        if (curGuideNPCOnlyId == onlyId)
            curGuideNPCOnlyId = 0;
        EventCenter.Broadcast(TheEventType.RemoveNPC, onlyId);
        if (RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(onlyId))
            RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Remove(onlyId);

        SingleNPCData data = FindNPCByOnlyId(onlyId);
        int settingId = data.Id;
        if (!RoleManager.Instance._CurGameInfo.allNPCData.RemovedNPCSettingIdList.Contains(settingId))
        {
            RoleManager.Instance._CurGameInfo.allNPCData.RemovedNPCSettingIdList.Add(settingId);
        }

        for (int i = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count-1; i >=0 ; i--)
        {
 
            SingleNPCData singleNPCData = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            if (singleNPCData.OnlyId == onlyId)
            {
                RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Remove(singleNPCData);

            }
         
        }
    }


    /// <summary>
    /// 通过postype得到scenetype
    /// </summary>
    public SceneType GetSceneByPosType(NPCAppearPosType nPCAppearPosType)
    {
        switch (nPCAppearPosType)
        {
            case NPCAppearPosType.ShanMen:
                return SceneType.Mountain;
            case NPCAppearPosType.ShanXia:
                return SceneType.OutsideMap;
        }
        return SceneType.None;
    }

    /// <summary>
    /// 显示第一个引导
    /// </summary>
    public void ShowFirstGuide()
    {
        CheckIfNPCAppear();
    }
}

public enum AchievementType
{
    None=0,
    KillEnmey,
    QieCuo,//切磋
    PassedMaxMap,//通过的最大地图
    PassedMaxLevel,//通过的最大关卡 几杠几
    PassedMaxLieXiLevel,//通过的最大裂隙
    AccomplishedMapEvent,//完成了地图事件
    OnceGuide,//一次性引导完成情况
    QuanLiTime,//加速生产次数
    UseSpecialTypeItemCount,//使用特殊类型物品次数
    TianFuJueXingNum,//天赋觉醒次数
    StudentUpgradeCount,//弟子升级次数
    TaoFa,//讨伐次数
    LianDan2,//炼丹 通过成就读取
    Explore,//秘境探险
MaxTaoFa,//最大讨伐等级
LiLian,//历练次数
MakeGem,//炼宝石
}

/// <summary>
/// 一次性引导的类型
/// </summary>
public enum OnceGuideIdType
{
    None=0,
    ZanQi=1,//攒气
    Battle_DaZhaoClickMai=2,//大招点脉
    Battle_UseDaZhao=3,//释放大招
    Battle_Escape=4,//引导逃跑
    Guide_Equip=5,//引导装备
    Guide_YuanSuReaction=6,//元素反应
    LiLian=7,//历练
    QiangHuaXueMai=8,//强化血脉
    ChangeYuanSu=9,//改变元素
}