using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using DG.Tweening;
using System;
using System.Threading;
using Spine.Unity;

public class MainPanel : PanelBase
{
     public Button btn_save;//保存


    public Transform trans_buffGrid;
    public GameObject obj_btnBg;//按钮bg
    public Button btn_knapsack;//背包
    public GameObject obj_knapsackRedPoint;//背包红点
    public Button btn_mountain;//后山
    public GameObject obj_mountainRedPoint;//后山红点
    public Button btn_house;//回家
    public Button btn_bigMap;//大地图

    public Button btn_outside;//外出

    //public Button btn_tree;

    public Transform trans_tongZhiGridParent;//通知格子

    public Image img_homeBtnLock;//家锁
    public Image img_matchBtnLock;//参赛锁

    public Transform trans_exploreGrid;//探险队
    //public Image img_matchBtnLock;//
    //public Transform trans_fullStudentExp;//有弟子经验值满了
    //public Transform trans_fullExpStudentGrid;//经验值满了的弟子
    public Button btn_tianFuTest;//天赋觉醒
    public Button btn_tuPo;//地址突破
                        
    public Transform trans_TopRecordGrid;//顶部记录

    public Button btn_team;//队伍
    public Button btn_student;//学生
    public Button btn_activity;//评选活动
    public Transform trans_activityGrid;//评选活动倒计时

    //public Button btn_offlineIncomePanel;

    public Button btn_economy;

    public Button btn_task;//任务
    public SkeletonGraphic ske_task;

    public Button btn_guideBook;//手札
    public GameObject obj_guideBookRedPoint;
 
    public Button btn_dabi;//大比

    public Transform trans_chatContent;//聊天框内容
    public Transform grid_chat;//聊天
     public Button btn_chat;
    public Button btn_shop;//坊市 
    public Button btn_qianDao;//签到
    public GameObject obj_sevenDayQianDaoRedPoint;//七天签到红点
    public Button btn_mail;//邮件
    public GameObject obj_mailRedPoint;//邮件红点

    public Button btn_building;//建造

    public Button btn_leiChong;//累充
    public Button btn_shouChong;//首充
    public Button btn_huoDong;//活动

    public Transform trans_rightBtns;//右边按钮
    public Transform trans_topBtns;//上面的按钮
    public Transform trans_leftBtns;//左边的按钮

 
    public override void Init(params object[] args)
    {
        base.Init(args);

      

      
        EventCenter.Register(TheEventType.CloseMainPanel, ClosedThePanel);
        //buff
         RegisterEvent(TheEventType.XianMenOpen, OnXianMenOpen);
        RegisterEvent(TheEventType.AddTopRecord , OnXianMenOpen);
 

        addBtnListener(btn_save, () =>
        {
            ArchiveManager.Instance.SaveArchive();
            PanelManager.Instance.OpenOnlyOkHint("保存成功！", null);
        });

        //addBtnListener(btn_tree, () =>
        //{
        //    PanelManager.Instance.ClosePanel(this);
        //    PanelManager.Instance.OpenPanel<ReceiveLeafPanel>(PanelManager.Instance.trans_sceneLayer);
        //});
        addBtnListener(btn_knapsack, () =>
        {
            PanelManager.Instance.OpenPanel<PlayerPeoplePanel>(PanelManager.Instance.trans_layer2,1);
        });

        addBtnListener(btn_mountain, () =>
        {
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.Mountain)
            {
                GameSceneManager.Instance.GoToScene(SceneType.Mountain);
                EventCenter.Broadcast(TheEventType.AccomplishNewGuide, NewGuideIdType.GoToMountain);
            }


        });

        ////回家
        //addBtnListener(btn_house, () =>
        //{
        //    if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.House)
        //    {
        //        GameSceneManager.Instance.GoToScene(SceneType.House);

        //    }
        //});

        ////大地图
        //addBtnListener(btn_bigMap, () =>
        //{
        //    if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.WorldMap)
        //    {
        //        GameSceneManager.Instance.GoToScene(SceneType.WorldMap);

        //    }
        //});

        //外出
        addBtnListener(btn_outside, () =>
        {
            if (RoleManager.Instance._CurGameInfo.SceneData.CurSceneType != (int)SceneType.OutsideMap)
            {
                //GameSceneManager.Instance.GoToScene(SceneType.OutsideMap);
                PanelManager.Instance.OpenPanel<OutCityMapView>(PanelManager.Instance.trans_layer2);
            }
        });

  
        addBtnListener(btn_tianFuTest, () =>
         {
             StudentHandlePanel panel= PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
             panel.AutoClickCanTalentTestStudent();
         });
        addBtnListener(btn_tuPo, () =>
        {
            StudentHandlePanel panel = PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
            panel.AutoClickCanTuPoStudent();
        });
        if (btn_team != null)
        {
            addBtnListener(btn_team, () =>
            {
                PanelManager.Instance.OpenPanel<TeamPanel>(PanelManager.Instance.trans_layer2);
            });

        }
        //弟子
        addBtnListener(btn_student, () =>
        {

            PanelManager.Instance.OpenPanel<StudentHandlePanel>(PanelManager.Instance.trans_layer2);
        });

        //addBtnListener(btn_taoFa, () =>
        // {
        //     GameSceneManager.Instance.GoToScene(SceneType.TaoFa);
        // });
        //addBtnListener(btn_activity, () =>
        //{
        //    PanelManager.Instance.OpenPanel<ActivityBaoMingPanel>(PanelManager.Instance.trans_layer2);
        //});
        //addBtnListener(btn_offlineIncomePanel, () =>
        //{
        //    PanelManager.Instance.OpenOfflinePanel();
        //});

        addBtnListener(btn_economy, () =>
        {
            PanelManager.Instance.OpenPanel<EconomyPanel>(PanelManager.Instance.trans_layer2);

        });

        addBtnListener(btn_task, () =>
        {
            if (TaskManager.Instance.curMainlineNPCData == null)
            {
                //for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Count; i++)
                //{
                //    ulong onlyId = RoleManager.Instance._CurGameInfo.allNPCData.CurShowNPCOnlyIdList[i];

                //    SingleNPCData data = TaskManager.Instance.FindNPCByOnlyId(onlyId);
                //    NPC setting = DataTable.FindNPCArrById(data.Id);
                //    if (setting.npcType==NPCType.None)
                //    {
                //        npcProtoData = data;
                //        break;
                //    }
                //}

            }

            if (TaskManager.Instance.curMainlineNPCData != null)
            {
                TaskManager.Instance.OnClickedNPC(TaskManager.Instance.curMainlineNPCData);
            }
        });

        addBtnListener(btn_guideBook, () =>
        {
            PanelManager.Instance.OpenPanel<GuideBookPanel>(PanelManager.Instance.trans_layer2);
        });

        addBtnListener(btn_dabi, () =>
         {
       
             if (MatchManager.Instance.generateAllZongMenThread != null && MatchManager.Instance.generateAllZongMenThread.ThreadState != ThreadState.Stopped)
                 return;
             PanelManager.Instance.OpenPanel<MatchPanel>(PanelManager.Instance.trans_layer2);
         });
  
        addBtnListener(btn_shop, () =>
        {
            PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.FangShi);
            //MailManager.Instance.SearchMyMail();
        });
        addBtnListener(btn_qianDao, () =>
        {
            PanelManager.Instance.OpenPanel<SevenDayQianDaoPanel>(PanelManager.Instance.trans_layer2);
        });

        addBtnListener(btn_mail, () =>
        {
            PanelManager.Instance.OpenPanel<MailPanel>(PanelManager.Instance.trans_layer2);
        });

        addBtnListener(btn_building, () =>
        {
            PanelManager.Instance.OpenPanel<NewDanFarmBuildPanel>(PanelManager.Instance.trans_layer2);
        });

        addBtnListener(btn_leiChong, () =>
        {
            PanelManager.Instance.OpenPanel<LeiChongPanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_shouChong, () =>
        {
            PanelManager.Instance.OpenPanel<ShouChongPanel>(PanelManager.Instance.trans_layer2);
        });
        addBtnListener(btn_huoDong, () =>
        {
            PanelManager.Instance.OpenPanel<HuoDongPanel>(PanelManager.Instance.trans_layer2);
        });
      
        ShowYieldShow();


        InitShow();

        RegisterEvent(TheEventType.RareStudentExpFull, OnRareStudentExpFull);
        RegisterEvent(TheEventType.EndowTalent, OnEndowTalent);
        RegisterEvent(TheEventType.TalentStudentExpFull, ShowCanBreakThroughTalentStudent);
        RegisterEvent(TheEventType.StudentBreakthroughSuccess, OnStudentBreakThrough);
        RegisterEvent(TheEventType.FailStudentBreakThrough, OnStudentBreakThrough);

         RegisterEvent(TheEventType.FinishCloudMove, ShowBtns);
        //RegisterEvent(TheEventType.AddTongZhi, AddTongZhi);
        RegisterEvent(TheEventType.ChangeTaskStatus, RefreshTaskStatusShow);
        RegisterEvent(TheEventType.ShowNPC, OnShowNPC);
        RegisterEvent(TheEventType.RefreshGuideBookRedPoint, RefreshGuideBookRedPointShow);
        RegisterEvent(TheEventType.RefreshDailyTaskRedPoint, RefreshGuideBookRedPointShow);

        RegisterEvent(TheEventType.RefreshSkillRedPoint, RefreshKnapsackRedPointShow);
        RegisterEvent(TheEventType.OnAddShowChat, OnAddShowChat);
        RegisterEvent(TheEventType.OnRemoveShowChat, OnRemoveShowChat);
        RegisterEvent(TheEventType.SuccessBreakThrough, OnBreakThrough);
        RegisterEvent(TheEventType.EnterBuildingMode, EnterBuildingMode);
        RegisterEvent(TheEventType.EnterOnlyMoveBuildingMode, EnterBuildingMode);
        RegisterEvent(TheEventType.GetShouChongAward, OnShouChongAwardGet);
        RegisterEvent(TheEventType.RefreshRedPointShow, RefreshRedPointShow);
        RegisterEvent(TheEventType.RefreshMainPanelBtnShow, InitShow);

        RegisterEvent(TheEventType.OnSearchedMyAllMail, RefreshMailRedPointShow);

    }
    void TestLeft()
    {
       // GameObject.Find("MountainPanel").GetComponent<MountainPanel>().scrollViewNevigation.GetComponent<MultipleTouchScrollRect>().
    }

    /// <summary>
    /// 初始化
    /// </summary>
    void InitShow()
    {
 
        if ((SceneType)RoleManager.Instance._CurGameInfo.SceneData.CurSceneType==SceneType.MiJingExplore)
        {
            trans_leftBtns.gameObject.SetActive(false);
            trans_topBtns.gameObject.SetActive(false);
            trans_rightBtns.gameObject.SetActive(false);
        }
        else
        {
            trans_leftBtns.gameObject.SetActive(true);
            trans_topBtns.gameObject.SetActive(true);
            trans_rightBtns.gameObject.SetActive(true);
        }

        InitExplore();
         ShowFullExpRareStudents();
        ShowCanBreakThroughTalentStudent();

        // 直接显示所有按钮
        ShowBtns();
        RefreshTaskStatusShow();
        ShowChat();
        RefreshRedPointShow();

 
    }

    void EnterBuildingMode()
    {
        PanelManager.Instance.ClosePanel(this);
    }

    void OnBreakThrough(object[] args)
    {
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex >= 20)
            btn_dabi.gameObject.SetActive(true);
    }
 
    /// <summary>
    /// 显示聊天框
    /// </summary>
    void ShowChat()
    {
        trans_chatContent.gameObject.SetActive(false);
        //if (GameSceneManager.Instance.toGoSceneType == SceneType.MiJingExplore)
        //{
        //    trans_chatContent.gameObject.SetActive(false);
        //}
        //else
        //{
        //    trans_chatContent.gameObject.SetActive(true);

        //}
         //ClearCertainParentAllSingle<SingleChatView>(grid_chat);
        //for(int i = 0; i < ChatManager.Instance.showChatQueue.Count; i++)
        //{
        //    SingleChatView chatView = AddSingle<SingleChatView>(grid_chat, ChatManager.Instance.showChatQueue[i]);
        //    chatViewList.Add(chatView);
        //}


    }
    void OnAddShowChat(object[] args)
    {
        //ChatData chatData = args[0] as ChatData;
        //SingleChatView chatView = AddSingle<SingleChatView>(grid_chat, chatData);
        //chatViewList.Add(chatView);

    }

    void OnRemoveShowChat(object[] args)
    {
        //ChatData chatData = args[0] as ChatData;
        //for(int i = chatViewList.Count - 1; i >= 0; i--)
        //{
        //    SingleChatView theView = chatViewList[i];
        //    if (theView.chatData.onlyId == chatData.onlyId)
        //    {
        //        PanelManager.Instance.CloseSingle(theView);
        //        chatViewList.RemoveAt(i);
        //    }
        //}
    }

    void RefreshRedPointShow()
    {
        RefreshGuideBookRedPointShow();
        RefreshKnapsackRedPointShow();
        RefreshMailRedPointShow();
        RefreshSevenQianDaoRedPoint();
    }

    void RefreshSevenQianDaoRedPoint()
    {
        bool show = false;
        if(RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex<7
            &&RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex + 1 == RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex)
        {
            show = true;
        }
        obj_sevenDayQianDaoRedPoint.SetActive(show);
    }

    /// <summary>
    /// 背包红点
    /// </summary>
    void RefreshKnapsackRedPointShow()
    {
        //RedPointManager.Instance.SetRedPointUI(obj_knapsackRedPoint, RedPointType.MainPanel_Btn_Knapsack, 0);
    }

    void RefreshGuideBookRedPointShow()
    {
        RedPointManager.Instance.SetRedPointUI(obj_guideBookRedPoint, RedPointType.MainPanel_Btn_Task, 0);

    }
    /// <summary>
    /// 邮件红点
    /// </summary>
    void RefreshMailRedPointShow()
    {
        bool show = false;
  
        obj_mailRedPoint.SetActive(show);
       
    }
    /// <summary>
    /// npc显示
    /// </summary>
    /// <param name="args"></param>
    void OnShowNPC(object[] args)
    {
        ShowNewComerTaskBtn();
        SingleNPCData nPCData = args[0] as SingleNPCData;
        NPC npcSetting = DataTable.FindNPCArrById(nPCData.Id);
        if (npcSetting.npcType == (int)NPCIDType.None)
        {
            //npcProtoData = nPCData;
            ShowTaskBtn();
        }
    }

    /// <summary>
    /// 显示任务按钮
    /// </summary>
    void ShowTaskBtn()
    {
        if (TaskManager.Instance.curMainlineNPCData == null)
            return;
        NPC npcSetting = DataTable.FindNPCArrById(TaskManager.Instance.curMainlineNPCData.Id);
        if (npcSetting == null)
            return;
        if (npcSetting.npcType != NPCType.None)
            return;
        if (npcSetting.id == (int)NPCIDType.PangBai)
            return;

        SingleTaskProtoData taskProtoData = TaskManager.Instance.FindTaskById(TaskManager.Instance.curMainlineNPCData, TaskManager.Instance.curMainlineNPCData.CurTaskId);
        if (taskProtoData != null)
        {
            btn_task.gameObject.SetActive(true);

            switch ((AccomplishStatus)taskProtoData.AccomplishStatus)
            {
                case AccomplishStatus.UnAccomplished:
                    ske_task.AnimationState.SetAnimation(0, "renwu_xinrenwu", true);
         

                    break;
                case AccomplishStatus.Processing:
                    ske_task.AnimationState.SetAnimation(0, "renwu_jingxingzhong", true);


                    break;
                case AccomplishStatus.Accomplished:
                    ske_task.AnimationState.SetAnimation(0, "renwu_wancheng", true);


                    break;
                default:
                    ske_task.AnimationState.SetAnimation(0, "renwu_wancheng", true);

                    break;
            }
        }
        else
        {
            btn_task.gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 刷新主线任务状态显示
    /// </summary>
    public void RefreshTaskStatusShow()
    {
        try
        {
            if (TaskManager.Instance.curMainlineNPCData ==null)
            {
                return;
            }
            ShowTaskBtn();

        }
        catch (Exception e)
        {
            Debug.LogError("NPCView刷新任务状态报错" + e);
        }

    }
 
    /// <summary>
    /// 普通弟子经验值满了
    /// </summary>
    public void OnRareStudentExpFull()
    {
        ShowFullExpRareStudents();
    }
    /// <summary>
    /// 赋予天赋
    /// </summary>
    public void OnEndowTalent(object[] args)
    {
        ShowFullExpRareStudents();
    }
    /// <summary>
    /// 显示感悟值满了的所有弟子
    /// </summary>
    void ShowFullExpRareStudents()
    {
        bool haveFullExpStudent = false;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.None
                && p.studentCurExp >= 120)
            {
                haveFullExpStudent = false;//不显示觉醒按钮
                break;
            }
        }
        //觉醒按钮
        if (haveFullExpStudent)
        {
            btn_tianFuTest.gameObject.SetActive(true);
        }
        else
        {
            btn_tianFuTest.gameObject.SetActive(false);

        }
        //if (haveFullExpStudent)
        //{
        //    trans_fullStudentExp.gameObject.SetActive(true);
        //}
        //else
        //{
        //    trans_fullStudentExp.gameObject.SetActive(false);

        //}
    }

    /// <summary>
    /// 显示可以突破的天赋弟子
    /// </summary>
    void ShowCanBreakThroughTalentStudent()
    {
        bool valid = false;
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent != (int)StudentTalent.None)
            {
                if (p.talent != (int)StudentTalent.LianGong)
                {
                    int maxLevel = DataTable._studentUpgradeList.Count;
                    if (p.studentLevel > 0 && p.studentLevel < maxLevel)
                    {
                        int limit = DataTable._studentUpgradeList[p.studentLevel - 1].NeedExp.ToInt32();
                        if (p.studentCurExp >= limit)
                        {
                            valid = true;
                            break;
                        }
                    }

                }
                else if (p.talent == (int)(StudentTalent.LianGong))
                {
                    TrainSetting curTrainSetting = DataTable._trainList[p.trainIndex];
                    ulong xiuweiNeed = curTrainSetting.XiuWeiNeed.ToUInt64();

                    if (p.curXiuwei >= xiuweiNeed)
                    {
                        valid = true;
                        break;
                    }
                }
            }
        }
        if (valid)
        {
            btn_tuPo.gameObject.SetActive(true);
        }
        else
        {
            btn_tuPo.gameObject.SetActive(false);

        }
    }
    /// <summary>
    /// 弟子突破
    /// </summary>
    /// <param name="args"></param>
    void OnStudentBreakThrough(object[] args)
    {
        ShowCanBreakThroughTalentStudent();
    }
    /// <summary>
    /// 弟子经验满了
    /// </summary>
    void OnStudentFullExp()
    {
        ShowFullExpRareStudents();
    }
    /// <summary>
    /// 仙门大开
    /// </summary>
    void OnXianMenOpen()
    {
      
    }

 

    void ShowBtns()
    {
        //btn_task.gameObject.SetActive(true);
        btn_save.gameObject.SetActive(true);
        btn_knapsack.gameObject.SetActive(true);
        btn_student.gameObject.SetActive(true);
        btn_mountain.gameObject.SetActive(true);
        btn_outside.gameObject.SetActive(true);
        btn_team.gameObject.SetActive(true);
        btn_economy.gameObject.SetActive(true);
        btn_guideBook.gameObject.SetActive(true);
        btn_shop.gameObject.SetActive(true);
        btn_qianDao.gameObject.SetActive(true);
        btn_chat.gameObject.SetActive(true);
        obj_btnBg.gameObject.SetActive(true);
        btn_building.gameObject.SetActive(true);

        // banHaoMode不显示累充
        if (Game.Instance.banHaoMode)
        {
            btn_leiChong.gameObject.SetActive(false);
        }
        else
        {
            btn_leiChong.gameObject.SetActive(true);
        }
        
        if(RoleManager.Instance._CurGameInfo.allShopData.shouChongAwardGet)
        btn_shouChong.gameObject.SetActive(false);
        else
            btn_shouChong.gameObject.SetActive(true);
        btn_huoDong.gameObject.SetActive(true);
        btn_mail.gameObject.SetActive(true);
         ShowNewComerTaskBtn();
        //if (MapManager.Instance.FindMapById(10000).LieXiMapStatus==(int)AccomplishStatus.Accomplished)
        //    btn_taoFa.gameObject.SetActive(true);
        //else
        //{
        //    btn_taoFa.gameObject.SetActive(false);
        //}
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex < 20)
        {
            btn_dabi.gameObject.SetActive(false);
        }
        else
        {
            btn_dabi.gameObject.SetActive(true);
        }
    }
    void ShowNewComerTaskBtn()
    {
        bool newComerTask = false;
        //新手
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData npc = RoleManager.Instance._CurGameInfo.allNPCData.AllNPCList[i];
            if (npc.Id == (int)NPCIDType.ShenMiShaoNv
                || npc.Id == (int)NPCIDType.SuMengLan)
            {
                 newComerTask = true;
                break;
            }
        }
         
        if (newComerTask)
        {
            RefreshTaskStatusShow();
        }
        //    btn_task.gameObject.SetActive(true);
        //else
        //    btn_task.gameObject.SetActive(false);
    }
    /// <summary>
    /// 探险
    /// </summary>
    void InitExplore()
    {
        ClearCertainParentAllSingle<SingleViewBase>(trans_exploreGrid);
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
        {
            ExploreTeamData teamData = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i].ExploreTeamData;
            if (teamData != null)
            {
                AddSingle<ExploreTeamStatusView>(trans_exploreGrid, RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i]);
            }
        }
    }


    /// <summary>
    /// 显示跳到这个界面时应该显示的东西（demo给测试员做的简单指引 TODO后续做到新手引导里面，废弃该功能）
    /// </summary>
    void ShowYieldShow()
    {
        switch (PanelManager.Instance.curYieldShowInMainPanelType)
        {
            case YieldShowInMainPanelType.UnlockNewMatch:
                PanelManager.Instance.OpenOnlyOkHint("解锁了新的赛事，快去看看吧！",null);
                PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.None;

                break;
            default:
                break;
        }
    }
 
    /// <summary>
    /// 关闭面板
    /// </summary>
    public void ClosedThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }



    #region 红点
 
    #endregion






    ///// <summary>
    ///// 刷新比赛预告
    ///// </summary>
    //void RefreshMatchForecast()
    //{
    //    int weekNum = RoleManager.Instance.GetWeekBeforeNextMatch();

    //}

 
    
    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    /// <summary>
    /// 顶部记录
    /// </summary>
    public void AddTopRecord(object[] args)
    {
        string content = (string)args[0];

        TopRecordTxtView view = AddSingle<TopRecordTxtView>(trans_TopRecordGrid, content);
   
        if (trans_TopRecordGrid.childCount >= 10)
        {
            TopRecordTxtView theView = trans_TopRecordGrid.GetChild(0).GetComponent<TopRecordTxtView>();
            mySingleList.Remove(theView);
            PanelManager.Instance.CloseSingle(theView);
        }
    }

    void OnShouChongAwardGet()
    {
        btn_shouChong.gameObject.SetActive(false);
    }

    public override void Clear()
    {
        base.Clear();

        //PanelManager.Instance.CloseAllSingle(trans_makingEquipProGrid);
        //PanelManager.Instance.CloseAllSingle(trans_addProTxtAnimViewParent);
        //PanelManager.Instance.CloseAllSingle(trans_equipProCompareGrid);





        EventCenter.Remove(TheEventType.CloseMainPanel, ClosedThePanel);
 
        PanelManager.Instance.CloseAllSingle(trans_tongZhiGridParent);
        ClearCertainParentAllSingle<SingleViewBase>(trans_exploreGrid);
    }

}
