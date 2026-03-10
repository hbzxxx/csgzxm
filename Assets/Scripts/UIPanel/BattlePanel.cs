 using Coffee.UIEffects;
using DG.Tweening;
using Framework.Data;
using cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using TooSimpleFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class BattlePanel : PanelBase
{
    public float prepareTime = 3f;
    public float prepareTimer = 0;
    public bool startPrepare = false;

    public float attackWaitTime =1f;
    public float attackWaitTimer = 0;
    public bool startAttackWaitTime = false;

    public PeopleData p1;
    public PeopleData p2;

    public Transform trans_p1Parent;
    public Transform trans_p2Parent;

    // public BattlePeopleView p1View;
    // public BattlePeopleView p2View;

    //public BattlePeopleView nextMovePeople;//下一个行动的人

    #region 游戏结束
    public Transform trans_gameEnd;
    public Button btn_leaveGame;
    public Image img_winLabel;//胜利图标
    public Image img_loseLabel;//失败图标
    public Transform trans_awardGrid;//奖励

    public Transform trans_win;
    public Transform trans_lose;
    #endregion
    //public bool logicTimePause;//逻辑时间暂停（cd计算等暂停

    public Portrait portrait1;
    public Image icon1;
    public Text txt_p1;
    public Image img_p1HpBar;//p1血
    public Image img_p1AttackCD;
    public Image img_p1EnergyBar;//p1能量
    public Text txt_hp1;

    public Transform trans_huZhao1;
    public Image img_huZhao1;//护罩
    public Text txt_huZhao1;

    public Transform trans_loseHpParent1;
    public Transform trans_buffTxtParent1;

    public Transform trans_buffFunctionTxtParent1;//buff作用
    public Transform trans_buffFunctionTxtParent2;


    public Portrait portrait2;
    public Image icon2;
    public Text txt_p2;
    public Image img_p2HpBar;//p2血
    public Image img_p2AttackCD;
    public Image img_p2EnergyBar;//p2能量
    public Text txt_hp2;
    public Image img_yuanSu2;//2元素
    public Transform trans_huZhao2;
    public Image img_huZhao2;//护罩
    public Text txt_huZhao2;

    //public Transform trans_skillGrid2;//敌人技能
    public Transform trans_loseHpParent2;
    public Transform trans_buffTxtParent2;

    public List<SingleMai> maiList;
    public int curMaiIndex;
    public int choosedMainFunctionIndex;//选择哪一条效果
    public Text txt_maiFunctionName;//经脉效果名
    //public Text txt_maiFunctionDes;//经脉效果描述
    List<List<JingMaiIDType>> possibleList;//可能的经脉线路
    public List<JingMaiIDType> curJingMaiIDList;//当前筋脉
    public List<Image> baGuaMaiList;//八卦脉
    public List<SingleMai> curMaiList;//当前选择的筋脉
    public Transform trans_effectParent;//效果父物体
    public Image img_maiBlackMask;//筋脉黑色遮罩
    public Ease qteTxtEase;//qte文字变大的风格

    public Transform trans_mp;//积蓄蓝的地方
    public Transform trans_fangDa;//放大能量条和按钮
    //public Transform trans_mpLock;//蓝解锁
    public Transform trans_mai;//操作经脉的地方
    public Image img_mp;//蓝池子
    public Text txt_mp;//蓝池子具体蓝
    public Button btn_big;//大招按钮

    public bool startQTE = false;
    float qteTimer = 0;
    float qteTime = 0.1f;
    int curChoosedBigIndex = 0;//放哪个大
    public List<string> testMaiFunctionName = new List<string> { "太阴肺经", "少阳三焦经", "厥阴肝经", "少阴心经", "太阴脾经" };
    public List<string> testMaiFunctionDes = new List<string> { "伤害增加20%", "驱散对方30%气", "暴击率增加50%", "为对方施加“虚弱”buff，持续3回合", "为自己施加“灵元盾”buff" };

    //ui切人
    public Transform trans_waitQieRen;//等待切人

    [Header("我切人的格子")]
    public Transform grid_meQieRen;//我切人的格子
    public List<BattleTeamMemberView> qieRenStudentViewList = new List<BattleTeamMemberView>();//切人

    //敌人们
    public Transform grid_enemyQieRen;//敌人切人的格子
    public List<BattleTeamMemberView> enemyStudentViewList = new List<BattleTeamMemberView>();//切人的敌人

    #region 新版队伍成员头像显示
    //// 玩家方队伍成员头像
    //public Transform trans_p1TeamGrid;  // 玩家方队伍头像容器
    //public List<BattleTeamMemberView> p1TeamMemberViewList = new List<BattleTeamMemberView>();

    // 敌人方队伍成员头像
    public Transform trans_p2TeamGrid;  // 敌人方队伍头像容器
    public List<BattleTeamMemberView> p2TeamMemberViewList = new List<BattleTeamMemberView>();

    // 协战提示UI父物体
    public Transform trans_xieZhanLaiLoParent;
    #endregion

    public Transform trans_allEffectParent;//所有特效父物体
    public bool energyHuXi = false;

    public Button btn_openEscape;//逃跑
    public Transform trans_escape;//逃跑
    public Button btn_confirmEscape;//确认逃跑
    public Button btn_closeEscape;//关闭逃跑面板

    public Button btn_cancelMai;//取消经脉，直接普攻

    public List<SkillBtnView> skillBtnViewList = new List<SkillBtnView>();//技能按钮
    public List<Transform> trans_skillBtnViewParentList;//技能按钮父物体 3个
    #region 元素显示
    //p1的
    //public Transform trans_yuanSuShow1;
    public Transform trans_yuanSuStartPos1;//起始点
    public List<YuanSuType> yuanSuToShowPool1=new List<YuanSuType>();//元素等待出现的池子
    public List<SingleYuanSuPointView> curShowYuanSuList1 = new List<SingleYuanSuPointView>();//当前出现的元素
    public Transform trans_yuanSuShowParent1;//父物体
    public bool yuanSuMoving1;//元素在移动
    public Transform YuanSuReactionNumShowParent1;//元素反应数字显示父物体
    public Image img_kangXingBar1;//抗性

    //p2的
    //public Transform trans_yuanSuShow2;
    public Transform trans_yuanSuStartPos2;//起始点
    public List<YuanSuType> yuanSuToShowPool2 = new List<YuanSuType>();//元素等待出现的池子
    public List<SingleYuanSuPointView> curShowYuanSuList2 = new List<SingleYuanSuPointView>();//当前出现的元素
    public Transform trans_yuanSuShowParent2;//父物体
    public bool yuanSuMoving2;//元素在移动
    public Transform YuanSuReactionNumShowParent2;//元素反应数字显示父物体
    public Image img_kangXingBar2;//抗性

    public int yuanSuViewOffset = 54;//差值
    #endregion

    #region 战斗结束
    public float battleEndAppearAwardTime;
    public float battleEndAppearAwardTimer;
    public bool startBattleEndAppearAward;//开始计时战斗结束
    List<ItemData> awardItemList;//奖励
    List<GetAwardItemView> awardItemViewList = new List<GetAwardItemView>();
    public int curItemIndex = 0;//物品
    Action closeResultAction = null;//关闭结算
    bool win = false;

    public Transform trans_downBtns;//底部按钮

    public Button btn_goXiuLian;//去修炼
    public Button btn_goEquip;//去炼器房
    public Button btn_goStudent;//去仓库
    public Button btn_goSkill;//去升级技能
    public Button btn_gem;//去炼制

    #endregion


    public List<Transform> trans_people1skill;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p1 = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
        p2 = BattleManager.Instance.p2List[BattleManager.Instance.p2Index];
        EventCenter.Register(TheEventType.BattleNextRound, NextRound);
        EventCenter.Register(TheEventType.BattleDeHpShow, OnLoseHp);
        EventCenter.Register(TheEventType.AttackCDShow, OnAttackCDShow);
        EventCenter.Register(TheEventType.BattleDeHpShow, DeHpTxtShow);
        RegisterEvent(TheEventType.BattleEnd, ShowGameEnd);
        RegisterEvent(TheEventType.AddBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.DeBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.BattlePeopleQieRen, OnQieRen);
        RegisterEvent(TheEventType.BattlePeopleSuccessfulChangePhase, OnChangePhase);
        RegisterEvent(TheEventType.AddBattleBuff, OnAddBattleBuff);
        RegisterEvent(TheEventType.RemoveBattleBuff, OnRemoveBattleBuff);
        RegisterEvent(TheEventType.RefreshBatlteBuff, OnRemoveBattleBuff);

        RegisterEvent(TheEventType.AddBattleHp, ShowAddHp);
        RegisterEvent(TheEventType.ShowBuffFunction, OnBuffFunctionShow);
        RegisterEvent(TheEventType.OnAddQieRenP, OnAddQieRenP);
        RegisterEvent(TheEventType.ClearYuanSu, OnClearYuanSu);


        RegisterEvent(TheEventType.YuanSuReaction, OnYuanSuReaction);
        RegisterEvent(TheEventType.ShowYuanSuReactionNum, ShowReactionNum);
        RegisterEvent(TheEventType.RefreshKangXingNumShow, RefreshKangXingNumShow);
        RegisterEvent(TheEventType.BattleInfoTxtShow, OnBattleInfoTxtShow);


        RegisterEvent(TheEventType.DisappearQTE, OnDisappearQTE);
        RegisterEvent(TheEventType.StartQTE, QTE);

        // 注册死亡事件，用于更新队伍成员头像状态
        //RegisterEvent(TheEventType.BattlePeopleDead, OnBattlePeopleDeadUpdateUI);

        //退出战斗
        addBtnListener(btn_leaveGame, () =>
        {
            if (startBattleEndAppearAward)
                return;
            PanelManager.Instance.ClosePanel(this);
            EventCenter.Broadcast(TheEventType.CloseBattleScene);
            switch (BattleManager.Instance.curBattleType)
            {
                case BattleType.Match:
                    BattleManager.Instance.NextLayerPhase();
                    //closeResultAction = null;
                    break;
                case BattleType.ZhaoChaBattle:
                    if(BattleManager.Instance.winner.isMyTeam)        
                        ZhaoChaPeopleManager.Instance.WinZhaoCha();
                    else
                        ZhaoChaPeopleManager.Instance.LoseZhaoCha();
                    //closeResultAction = null;

                    break;
                case BattleType.LevelBattle:
                    //得奖面板
                    closeResultAction();
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;
                    //Action enterMapAction = BackLastScene;
                    //GetAwardPanel panel = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //   BattleManager.Instance.roleAward, enterMapAction);

                    // GameSceneManager.Instance.GoToScene(SceneType.SingleMap,false);
                    //前往最近关卡点
                    EventCenter.Broadcast(TheEventType.NevigateToLevel, RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId, MapManager.Instance.curChoosedLevelId, MapSceneType.XianMen);
                    if (win)
                    {
                        EventCenter.Broadcast(TheEventType.ShowAccomplishLevelAnim, RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId, MapManager.Instance.curChoosedLevelId, MapSceneType.XianMen);
                       
                    }
                    //如果最后一关 则结算
                    if (DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId).IsEndLevel == "1"
                    &&win)
                    {
                        MapManager.Instance.MapResult();

                    }

#if !UNITY_EDITOR
                    ArchiveManager.Instance.SaveArchive();
#endif
                    break;
                case BattleType.FixedLevelBattle:
                    //得奖面板
                    closeResultAction();

                    //结束对话
                    if (win)
                    {
                        LevelSetting levelSetting = DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId);
                   
                        EventCenter.Broadcast(TheEventType.ShowAccomplishLevelAnim, RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId, MapManager.Instance.curChoosedLevelId, MapSceneType.Fixed);

                    }
                    //前往最近关卡点
                    EventCenter.Broadcast(TheEventType.NevigateToLevel, RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId, MapManager.Instance.curChoosedLevelId, MapSceneType.Fixed);

                    break;
                case BattleType.OutsideBattle:
                    //得奖面板
                    closeResultAction();

                    //closeResultAction = BackLastScene;
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;

                    //Action enterOutSideMapAction = EnterOutsideMap;
                    //GetAwardPanel panel2 = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //   BattleManager.Instance.roleAward, enterOutSideMapAction);

                    // GameSceneManager.Instance.GoToScene(SceneType.SingleMap,false);
                    break;
                case BattleType.QieCuoBattle:
                    //得奖面板
                    //Action enterOutSideMapAction = EnterOutsideMap;
                    //GetAwardPanel panel2 = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //    new List<ItemData> { BattleManager.Instance.roleAward }, enterOutSideMapAction);

                    //GameSceneManager.Instance.BackLastScene();
                    closeResultAction();

                    break;
                case BattleType.MiJingGuardBattle:
                    //得奖面板
                    //closeResultAction = BackLastScene;
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;
                    //Action MiJingGuard_BackAction = BackLastScene;
                    //GetAwardPanel panel3 = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //  BattleManager.Instance.roleAward, MiJingGuard_BackAction);
                    closeResultAction();

                    // GameSceneManager.Instance.GoToScene(SceneType.SingleMap,false);
                    break;
                    //杀死npc
                case BattleType.NPCKillBattle:
                    //得奖面板
                    //closeResultAction = BackLastScene;
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;
                    //Action NPCKill_BackAction = BackLastScene;
                    //GetAwardPanel panel4= PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //  BattleManager.Instance.roleAward, NPCKill_BackAction);
                    closeResultAction();

                    // GameSceneManager.Instance.GoToScene(SceneType.SingleMap,false);
                    break;
                case BattleType.TouZiFirstBattle:
                    // 已废弃：头子战斗逻辑已删除
                    GameSceneManager.Instance.BackLastScene();
                    break;
                    //狸猫掌门战斗
                case BattleType.LiMaoZhangMenBattle:
                    closeResultAction();

                    //closeResultAction = BackLastScene;
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;
                    //Action LiMaoZhangMen_BackAction = BackLastScene;
                    //GetAwardPanel panel5 = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //  BattleManager.Instance.roleAward, LiMaoZhangMen_BackAction);
                    break;
                    //帝姝战斗
                case BattleType.DiShuFirstBattle:
                    //if (BattleManager.Instance.winner.OnlyId == RoleManager.Instance._CurGameInfo.playerPeople.OnlyId)
                    GameSceneManager.Instance.BackLastScene();
                    break;
                case BattleType.MapEventBattle:
                    closeResultAction();

                    //得奖面板
                    //closeResultAction = BackLastScene;
                    //startAttackWaitTime = true;
                    //attackWaitTimer = 0;
                    //awardItemList = BattleManager.Instance.roleAward;
                    //Action MapEventBattle_BackAction = BackLastScene;
                    //GetAwardPanel panel6 = PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2,
                    //  BattleManager.Instance.roleAward, MapEventBattle_BackAction);
                    break;
           
            }
        });

     



        addBtnListener(btn_openEscape, () =>
        {
            trans_escape.gameObject.SetActive(true);
            BattleManager.Instance.AddLogicPause();
        });
        addBtnListener(btn_confirmEscape, () =>
        {
            trans_escape.gameObject.SetActive(false);
            BattleManager.Instance.RemoveLogicPause();

            BattleManager.Instance.Escape(p1.onlyId);

        });
        addBtnListener(btn_closeEscape, () =>
        {
            trans_escape.gameObject.SetActive(false);
            BattleManager.Instance.RemoveLogicPause();
        });

        //取消放大
        addBtnListener(btn_cancelMai, () =>
        {
            PanelManager.Instance.CloseTaskGuidePanel();
            //EventCenter.Broadcast(TheEventType.SkillAttack, p1.OnlyId, 0);
            OnDisappearQTE();
            EventCenter.Broadcast(TheEventType.CancelMai, p1.onlyId);
            btn_big.gameObject.SetActive(true);

        });

        //修炼
        addBtnListener(btn_goXiuLian, () =>
        {
            btn_leaveGame.onClick.Invoke();
            JumpPageManager.Instance.JumpToXiuLian();
        });
        //法器
        addBtnListener(btn_goEquip, () =>
        {
            btn_leaveGame.onClick.Invoke();
            JumpPageManager.Instance.JumpToEquipBuilding();
        });
        //弟子
        addBtnListener(btn_goStudent, () =>
        {
            btn_leaveGame.onClick.Invoke();
            JumpPageManager.Instance.JumpToXiuLian();
        });
        //技能
        addBtnListener(btn_goSkill, () =>
        {
            btn_leaveGame.onClick.Invoke();
            JumpPageManager.Instance.JumpToXiuLian();
        });
        //宝石
        addBtnListener(btn_gem, () =>
        {
            btn_leaveGame.onClick.Invoke();
            JumpPageManager.Instance.JumpToGemBuilding();
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        trans_gameEnd.gameObject.SetActive(false);
        trans_huZhao1.gameObject.SetActive(false);
        trans_huZhao2.gameObject.SetActive(false);

        RefreshShow();
        QieRenHandleShow();

        // 显示新版队伍成员头像
        //ShowTeamMemberViews();

        // 显示我方当前上阵角色的技能
        ShowCurrentPlayerSkills();

        InitMai();
        StartPrepare();
        if(qieRenStudentViewList.Count>0)
        qieRenStudentViewList[BattleManager.Instance.p1Index].btn.onClick.Invoke();
    }
    
    /// <summary>
    /// 显示我方当前上阵角色的技能
    /// </summary>
    void ShowCurrentPlayerSkills()
    {
        // 清空现有技能视图
        if (trans_people1skill != null)
        {
            for (int i = 0; i < trans_people1skill.Count; i++)
            {
                if (trans_people1skill[i] != null)
                {
                    PanelManager.Instance.CloseAllSingle(trans_people1skill[i]);
                }
            }
        }

        // 获取当前上阵角色
        PeopleData currentPlayer = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
        if (currentPlayer == null || currentPlayer.allSkillData == null || currentPlayer.allSkillData.equippedSkillIdList == null)
            return;

        // 显示技能，从第一个携带的技能开始（跳过普攻索引0），最多显示3个技能
        int skillIndex = 1; // 从索引1开始，跳过普攻
        int maxSkills = Mathf.Min(3, trans_people1skill.Count); // 最多3个技能或根据trans_people1skill长度

        for (int i = 0; i < maxSkills && skillIndex < currentPlayer.allSkillData.equippedSkillIdList.Count; i++)
        {
            if (trans_people1skill[i] != null)
            {
                int skillId = currentPlayer.allSkillData.equippedSkillIdList[skillIndex];
                SkillSetting skillSetting = DataTable.FindSkillSetting(skillId);
                
                if (skillSetting != null)
                {
                    // 使用 BattleSkillView 显示技能
                    BattleSkillView skillView = PanelManager.Instance.OpenSingle<BattleSkillView>(
                        trans_people1skill[i], currentPlayer, skillSetting, SkillViewType.Show, false);
                }
                
                skillIndex++; // 移动到下一个技能
            }
        }
    }
  
    #region 战斗结束面板
    public void ShowAward()
    {
        //startBattleEndAppearAward = true;

    }
#endregion

    /// <summary>
    /// 初始化筋脉
    /// </summary>
    void InitMai()
    {
        //初始化筋脉
        for (int i = 0; i < maiList.Count; i++)
        {
            maiList[i].Init(this);
        }
    }
    /// <summary>
    /// 刷新显示
    /// </summary>
    void RefreshShow()
    {
        //if (p1.portraitType == (int)PortraitType.ChangeFace
        //    && p1.portraitIndexList.Count > 0)
        //{
        //    portrait1.gameObject.SetActive(true);
        //    icon1.gameObject.SetActive(false);
        //    portrait1.Refresh(p1);

        //}
        //else
        //{
        //    portrait1.gameObject.SetActive(false);
        //    icon1.gameObject.SetActive(true);
        //    //如果有enemysetting 则用enemysetting 的icon 如果没有 则用自己的icon 如果都没有 则用默认icon
        //    EnemySetting enemySetting = DataTable.FindEnemySetting(p1.enemySettingId);
        //    if (enemySetting != null)
        //    {
        //        icon1.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + enemySetting.SpecialPortrait);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(p1.specialPortraitName))
        //    {
        //        icon1.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + p1.specialPortraitName);

        //    }
        //    else
        //    {
        //        icon1.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + ConstantVal.defaultPortraitName);

        //    }
        //}
        portrait1.gameObject.SetActive(false);
        StudentManager.Instance.SetTouxiang(icon1, p1);
        //if (p2.portraitType == (int)PortraitType.ChangeFace && p2.portraitIndexList.Count > 0)
        //{
        //    portrait2.gameObject.SetActive(true);
        //    icon2.gameObject.SetActive(false);
        //    portrait2.Refresh(p2);

        //}
        //else
        //{
        //    portrait2.gameObject.SetActive(false);
        //    icon2.gameObject.SetActive(true);

        //    //如果有enemysetting 则用enemysetting 的icon 如果没有 则用自己的icon 如果都没有 则用默认icon
        //    EnemySetting enemySetting = DataTable.FindEnemySetting(p2.enemySettingId);
        //    if (enemySetting != null)
        //    {
        //        icon2.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + enemySetting.SpecialPortrait);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(p1.specialPortraitName))
        //    {
        //        icon2.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + p2.specialPortraitName);
        //    }
        //    else
        //    {
        //        icon2.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + ConstantVal.defaultPortraitName);
        //    }
        //}

        portrait2.gameObject.SetActive(false);
        StudentManager.Instance.SetTouxiang(icon2, p2);
        img_yuanSu2.sprite = ConstantVal.YuanSuIcon((YuanSuType)p2.yuanSu);
        txt_p1.SetText(p1.name);
        string phaseStr = "";
        if (p2.totalPhase > 1)
        {
            phaseStr="("+p2.curPhase + "/" + p2.totalPhase+")";
        }
        txt_p2.SetText(p2.name+ phaseStr);
        SinglePropertyData p1hpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p1);
        img_p1HpBar.DOKill();
        img_p1HpBar.fillAmount = p1hpPro.num / (float)p1hpPro.limit;
        img_p1AttackCD.fillAmount = 0;
        txt_hp1.DOKill();
        txt_hp1.SetText(p1hpPro.num + "/" + p1hpPro.limit);
        SinglePropertyData p2hpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p2);
        img_p2HpBar.DOKill();
        img_p2HpBar.fillAmount = p2hpPro.num / (float)p2hpPro.limit;
        txt_hp2.SetText(p2hpPro.num + "/" + p2hpPro.limit);



        img_p2AttackCD.DOKill();
        img_p2AttackCD.fillAmount = 0;
        SinglePropertyData p1MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p1);

        img_p1EnergyBar.DOKill();
        img_p1EnergyBar.fillAmount = p1MPPro.num / (float)p1MPPro.limit;
        SinglePropertyData p2MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p2);

        img_p2EnergyBar.DOKill();
        img_p2EnergyBar.fillAmount = p2MPPro.num / (float)p2MPPro.limit;

        img_maiBlackMask.color = new Color(0, 0, 0, 0);

        trans_mai.gameObject.SetActive(false);
        //技能
        //trans_mp.gameObject.SetActive(true);
        if (p1.allSkillData.equippedSkillIdList.Count == 1)
        {
            trans_fangDa.gameObject.SetActive(false);
        }
        else
        {
            trans_fangDa.gameObject.SetActive(true);
        }


        ShowBigButton();

        img_mp.DOKill();
        txt_mp.DOKill();
        img_mp.fillAmount = p1MPPro.num / (float)p1MPPro.limit;
        txt_mp.SetText(p1MPPro.num + "%");
        //无逃跑
        if (trans_escape.gameObject.activeInHierarchy)
        {
            trans_escape.gameObject.SetActive(false);
            BattleManager.Instance.RemoveLogicPause();
        }

        skillBtnViewList.Clear();
        //技能
        for(int i = 0; i < trans_skillBtnViewParentList.Count; i++)
        {
            ClearCertainParentAllSingle<SkillBtnView>(trans_skillBtnViewParentList[i]);
            if (p1.allSkillData.equippedSkillIdList.Count > i+1)
            {
                SingleSkillData SingleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p1.allSkillData.equippedSkillIdList[i+1], p1.allSkillData);
                SkillBtnView view= AddSingle<SkillBtnView>(trans_skillBtnViewParentList[i], SingleSkillData, this);
                skillBtnViewList.Add(view);
            }
        }
 
        MyQieRenHandleShow();
        ShowKangXingBar(true, BattleManager.Instance.kangXing1, false);
        ShowKangXingBar(false, BattleManager.Instance.kangXing2, false);

    }

    /// <summary>
    /// 显示切人操作台
    /// </summary>
    void QieRenHandleShow()
    {
        MyQieRenHandleShow();
        EnemyQieRenHandleShow();
    }

    /// <summary>
    /// 我切人
    /// </summary>
    void MyQieRenHandleShow()
    {
        qieRenStudentViewList.Clear();
        ClearCertainParentAllSingle<BattleTeamMemberView>(grid_meQieRen);
        for (int i = 0; i < BattleManager.Instance.p1List.Count; i++)
        {
            // PeopleData theP = BattleManager.Instance.p1List[i];
            //if (p1.onlyId!= theP.onlyId)
            //{
               BattleTeamMemberView view = AddSingle<BattleTeamMemberView>(grid_meQieRen, BattleManager.Instance.p1List[i], this, true);
               qieRenStudentViewList.Add(view);
            //}
           
        }
    }

    /// <summary>
    /// 敌人显示切人
    /// </summary>
    void EnemyQieRenHandleShow()
    {
        enemyStudentViewList.Clear();
        ClearCertainParentAllSingle<BattleTeamMemberView>(grid_enemyQieRen);
        for (int i = 0; i < BattleManager.Instance.p2List.Count; i++)
        {
            BattleTeamMemberView view = AddSingle<BattleTeamMemberView>(grid_enemyQieRen, BattleManager.Instance.p2List[i], this, false);
            enemyStudentViewList.Add(view);
        }
    }

    /// <summary>
    /// 增加一个切人的p
    /// </summary>
    void OnAddQieRenP(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        BattleTeamMemberView view = AddSingle<BattleTeamMemberView>(grid_meQieRen, theP, this, true);
        qieRenStudentViewList.Add(view);

    }
    /// <summary>
    /// 切人
    /// </summary>
    void OnQieRen()
    {
        //p1切了
        //if (p1.onlyId != BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId)
        //{
        //    p1 = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
        //    qieRenStudentViewList[BattleManager.Instance.p1Index].btn.onClick.Invoke();
        //}
        //if (p2.onlyId != BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId)
        //{
        //    p2 = BattleManager.Instance.p2List[BattleManager.Instance.p2Index];
        //}
        p1 = BattleManager.Instance.p1List[BattleManager.Instance.p1Index];
        p2 = BattleManager.Instance.p2List[BattleManager.Instance.p2Index];
        RefreshShow();
        QieRenHandleShow();
        //// 更新队伍成员高亮状态
        //UpdateTeamMemberHighlight();
        
        // 切换角色后更新技能显示
        ShowCurrentPlayerSkills();
        
        InitMai();
    }
    /// <summary>
    /// 改变阶段
    /// </summary>
    void OnChangePhase()
    {
        RefreshShow();
        InitMai();
    }

    /// <summary>
    /// 切人
    /// </summary>
    public void OnReadyToQieRen(BattleTeamMemberView view)
    {
     
        for (int i = 0; i < qieRenStudentViewList.Count; i++)
        {

            BattleTeamMemberView theView = qieRenStudentViewList[i];
            if (theView.peopleData.onlyId == view.peopleData.onlyId)
            {
                theView.SetAsCurrent(true);
                if (p1.onlyId != view.peopleData.onlyId)
                {
                    int theIndex = BattleManager.Instance.p1List.IndexOf(view.peopleData);
                    EventCenter.Broadcast(TheEventType.ReadyToQieRen, p1.onlyId, theIndex);
                }
                else
                {
                    EventCenter.Broadcast(TheEventType.CancelReadyToQieRen, p1.onlyId);
                }
            }
            else
            {
                theView.SetAsCurrent(false);
            }
        }


    }
    /// <summary>
    /// 前往上个场景
    /// </summary>
    public void BackLastScene()
    {
        GameSceneManager.Instance.BackLastScene();

    }

    /// <summary>
    /// 关闭结算面板进入外部地图
    /// </summary>
    public void EnterOutsideMap()
    {
        GameSceneManager.Instance.GoToScene(SceneType.OutsideMap, false);

    }
    /// <summary>
    /// 关闭结算面板进入地图
    /// </summary>
    public void EnterMap()
    {
        GameSceneManager.Instance.GoToScene(SceneType.SingleMap, false);

    }

    //开始准备
    void StartPrepare()
    {
        //BattleManager.Instance.logicPause = true;
        BattleManager.Instance.AddLogicPause();

        startPrepare = true;
        prepareTimer = 0;
        //p1View.EnterState(IState.Idle);
        //p2View.EnterState(IState.Idle);
    }

    /// <summary>
    /// 下回合
    /// </summary>
    void StartNextRound()
    {
        startAttackWaitTime = true;
        attackWaitTimer = 0;
        //p1View.EnterState(IState.Idle);
        //p2View.EnterState(IState.Idle);
    }

    private void Update()
    {
        if (startPrepare)
        {
            prepareTimer += Time.deltaTime;
            if (prepareTimer >= prepareTime)
            {
                //Debug.Log("准备阶段结束，关闭logicpause 双方开始计时");
                BattleManager.Instance.RemoveLogicPause();

                //nextMovePeople.EnterState(IState.Attack);
                //SetNextMovePeople();
                startPrepare = false;
            }

        }
        if (startBattleEndAppearAward)
        {
            battleEndAppearAwardTimer += Time.deltaTime;
            if (battleEndAppearAwardTimer >= battleEndAppearAwardTime)
            {
                if (awardItemList == null)
                {
                    startBattleEndAppearAward = false;
                    return;
                }
                if (curItemIndex >= awardItemList.Count)
                {
                    startBattleEndAppearAward = false;
                    return;
                }
                GetAwardItemView view =awardItemViewList[curItemIndex];
                view.StartAnim();
                curItemIndex++;
                //有下一个
                if (curItemIndex < awardItemList.Count)
                {
                    battleEndAppearAwardTimer = 0;
                }
                //无下一个 显示弟子加经验
                else
                {
                    startBattleEndAppearAward = false;
                }
            }
        }

  
        //有元素要塞进来
        if (yuanSuToShowPool1.Count > 0&&!yuanSuMoving1)
        {
            OnVisibleAddYuanSu(yuanSuToShowPool1[0],true);
        }   
        //有元素要塞进来
        if (yuanSuToShowPool2.Count > 0 && !yuanSuMoving2)
        {
            OnVisibleAddYuanSu(yuanSuToShowPool2[0], false);
        }

        if (startQTE)
        {
            qteTimer += Time.deltaTime;
            if (qteTimer >= qteTime)
            {
                qteTimer = 0;
                startQTE = false;
                OnShowBaGua();
            }
        }
    }
    /// <summary>
    /// 抵抗显示
    /// </summary>
    /// <param name="param"></param>
    public void OnBattleInfoTxtShow(object[] param)
    {
        PeopleData peopleData = param[0] as PeopleData;
        string str = (string)param[1];
        Transform YuanSuReactionNumShowParent = null;

        if (peopleData.onlyId == p1.onlyId)
        {
            YuanSuReactionNumShowParent = YuanSuReactionNumShowParent1;
        }
        else
        {
            YuanSuReactionNumShowParent = YuanSuReactionNumShowParent2;
        }

        AddSingle<YuanSuReactionTxtView>(YuanSuReactionNumShowParent, str, false, YuanSuType.None);
    }

    /// <summary>
    /// 刷新抗性数值显示
    /// </summary>
    public void RefreshKangXingNumShow(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        int num = 0;
        bool left = false;
        if (BattleManager.Instance.CheckIfLeftP(p))
        {
            left = true;
            num = BattleManager.Instance.kangXing1;
        }
        else
        {
            left = false;
            num = BattleManager.Instance.kangXing2;

        }
        ShowKangXingBar(left, num, true);
    }

    void ShowKangXingBar(bool left, int num,bool tween)
    {

        if (left)
        {
            img_kangXingBar1.DOKill();

            if (tween)
                img_kangXingBar1.DOFillAmount((float)num / 100, .5f);
            else
                img_kangXingBar1.fillAmount = (float)num / 100;
        }
        else
        {
            img_kangXingBar2.DOKill();
            if (tween)
                img_kangXingBar2.DOFillAmount((float)num / 100, .5f);
            else
                img_kangXingBar2.fillAmount = (float)num / 100;
        }
    }

    #region 元素球显示

    /// <summary>
    /// 清空元素
    /// </summary>
    void OnClearYuanSu(object[] args)
    {
        bool left = (bool)args[0];
        if (left)
        {
            for(int i = curShowYuanSuList1.Count - 1; i >= 0; i--)
            {
                RemovedShowYuanSu(i, left);
            }
            yuanSuToShowPool1.Clear();

            curShowYuanSuList1.Clear();
        }
        else
        {

            for (int i = curShowYuanSuList2.Count - 1; i >= 0; i--)
            {
                RemovedShowYuanSu(i, left);
            }
            yuanSuToShowPool2.Clear();

            curShowYuanSuList2.Clear();
        }
    }
    /// <summary>
    /// 元素反应
    /// </summary>
    public void OnYuanSuReaction(object[] param)
    {
        //return;
        AttackResData res = param[0] as AttackResData;
        if (res == null)
            return;
        if (res.skill.yuanSuType == YuanSuType.None)
            return;
        Transform YuanSuReactionNumShowParent = null;
        PeopleData peopleData = res.deHpPeople;
        //如果有元素盾
        List<BattleBuff> huZhaoBuffList =BattleManager.Instance.FindTypeBuffList(peopleData, BattleBuffType.HuZhao);
        if (huZhaoBuffList != null)
        {
            for (int i = 0; i < huZhaoBuffList.Count; i++)
            {
                BattleBuff b = huZhaoBuffList[i];
                if (!string.IsNullOrWhiteSpace(b.buffSetting.Param4))
                {
                     return;
                }
            }
        }
        if (peopleData.onlyId == p1.onlyId)
        {
            OnAddYuanSu(res.skill.yuanSuType, true);
            YuanSuReactionNumShowParent = YuanSuReactionNumShowParent1;
        }
        else
        {
            OnAddYuanSu(res.skill.yuanSuType, false);
            YuanSuReactionNumShowParent = YuanSuReactionNumShowParent2;
        }
        if (res.reactionType != ReactionType.None)
        {
            //反应
            //AddSingle<YuanSuReactionTxtView>(YuanSuReactionNumShowParent, ConstantVal.YuanSuReactionName(res.reactionType), false, res.skill.yuanSuType);
        }
 
    }
    /// <summary>
    /// 显示反应数字
    /// </summary>
    /// <param name="args"></param>
    public void ShowReactionNum(object[] args)
    {
        AttackResData res = args[0] as AttackResData;

        Transform YuanSuReactionNumShowParent = null;

        PeopleData peopleData = res.deHpPeople;
        if (peopleData.onlyId == p1.onlyId)
        {
             YuanSuReactionNumShowParent = YuanSuReactionNumShowParent1;
        }
        else
        {
             YuanSuReactionNumShowParent = YuanSuReactionNumShowParent2;
        }
      
            //反应
        AddSingle<YuanSuReactionTxtView>(YuanSuReactionNumShowParent, res.deHp.ToString(), true, YuanSuType.None);
        
    }

    public void OnAddYuanSu(YuanSuType yuanSu,bool left)
    {
 
        List<YuanSuType> yuanSuToShowPool = null;

        if (left)
        {
            yuanSuToShowPool = yuanSuToShowPool1;
        }
        else
        {
            yuanSuToShowPool = yuanSuToShowPool2;
        }

        yuanSuToShowPool.Add(yuanSu);

    }

    /// <summary>
    /// 视觉上增加元素
    /// </summary>
    public void OnVisibleAddYuanSu(YuanSuType yuanSu,bool left)
    {
        List<YuanSuType> yuanSuToShowPool = null;
        List<SingleYuanSuPointView> curShowYuanSuList = null;
        if (left)
        {
            yuanSuToShowPool = yuanSuToShowPool1;
            curShowYuanSuList = curShowYuanSuList1;
        }
        else
        {
            yuanSuToShowPool = yuanSuToShowPool2;
            curShowYuanSuList = curShowYuanSuList2;
        }

        yuanSuToShowPool.RemoveAt(0);
        AddYuanSuView(yuanSu, curShowYuanSuList.Count,left);


        //首先看能否与当前视觉上的元素反应
        List<YuanSuType> candidateList = new List<YuanSuType>();
        for (int i = 0; i < curShowYuanSuList.Count; i++)
        {
            candidateList.Add(curShowYuanSuList[i].yuanSuType);
        }
        //candidateList.Add(yuanSu);
        YuanSuReactionRes res = YuanSuReactionManager.Instance.CheckYuanSuReaction(candidateList);
        //如果有能反应的 则生成在该元素上并一起消失
        if (res.reactionType != ReactionType.None)
        {
            
            for (int i = curShowYuanSuList.Count-1; i >=0; i--)
            {
                YuanSuType theType = curShowYuanSuList[i].yuanSuType;
                //如果是4个 则移除掉的一定有个新来的（未显示）
                if (res.removedYuanSuList.Contains(theType))
                {
                    RemovedShowYuanSu(i,left);
                    res.removedYuanSuList.Remove(theType);
                }
            }
        }
        //没有能反应的 且大于4 则挤掉第一个元素
        else
        {
            if (curShowYuanSuList.Count >= 4)
            {
                RemovedShowYuanSu(0,left);
            }
        }
        //剩下的要归位
        for(int i = 0; i < curShowYuanSuList.Count; i++)
        {
            SingleYuanSuPointView singleView = curShowYuanSuList[i];
            if (singleView.posIndex != i)
            {
                RotateYuanSuView(singleView, i,left);
            }
        }
    }
    public void RotateYuanSuView(SingleYuanSuPointView singleView,int targetPosIndex,bool left)
    {
        Transform trans_yuanSuStartPos = null;
        if (left)
        {
            yuanSuMoving1 = true;

            trans_yuanSuStartPos = trans_yuanSuStartPos1;
        }
        else
        {
            trans_yuanSuStartPos = trans_yuanSuStartPos2;
            yuanSuMoving2 = true;

        }

        float posX = trans_yuanSuStartPos.localPosition.x + yuanSuViewOffset * targetPosIndex;
        singleView.transform.DOLocalMoveX(posX, .6f).OnComplete(() =>
        {
            if (left)
            {
                yuanSuMoving1 = false;
            }
            else
            {
                yuanSuMoving2 = false;
            }
            singleView.posIndex = targetPosIndex;
        });
    }
    public void AddYuanSuView(YuanSuType yuansu,int posIndex,bool left)
    {
        Transform trans_yuanSuShowParent = null;
        Transform trans_yuanSuStartPos = null;
        List<SingleYuanSuPointView> curShowYuanSuList = null;
        if (left)
        {
            trans_yuanSuShowParent = trans_yuanSuShowParent1;
            trans_yuanSuStartPos = trans_yuanSuStartPos1;
            curShowYuanSuList = curShowYuanSuList1;
        }
        else
        {
            trans_yuanSuShowParent = trans_yuanSuShowParent2;
            trans_yuanSuStartPos = trans_yuanSuStartPos2;
            curShowYuanSuList = curShowYuanSuList2;
        }
        SingleYuanSuPointView view = AddSingle<SingleYuanSuPointView>(trans_yuanSuShowParent, yuansu, posIndex,new Vector2(trans_yuanSuStartPos.localPosition.x+yuanSuViewOffset* posIndex, trans_yuanSuStartPos.localPosition.y));
        curShowYuanSuList.Add(view);
    }

    /// <summary>
    /// 移除掉一个显示的元素
    /// </summary>
    /// <param name="index"></param>
    public void RemovedShowYuanSu(int index,bool left)
    {
        List<SingleYuanSuPointView> curShowYuanSuList = null;
        if (left)
        {
            curShowYuanSuList = curShowYuanSuList1;
        }
        else
        {
            curShowYuanSuList = curShowYuanSuList2;
        }
        curShowYuanSuList[index].OnRemove();
        //PanelManager.Instance.CloseSingle(curShowYuanSuList[index]);
        curShowYuanSuList.RemoveAt(index);
    }

 
 
    #endregion


    /// <summary>
    /// 下一个回合
    /// </summary>
    public void NextRound()
    {
        StartNextRound();
    }

    /// <summary>
    /// 显示大招按钮
    /// </summary>
   void ShowBigButton()
    {
        SinglePropertyData p1MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p1);

        if (p1MPPro.num >= (float)p1MPPro.limit)
        {
            energyHuXi = true;

            EffectToBig();
            btn_big.gameObject.SetActive(true);

            //DOTween.To(() => uieffect_energy.colorFactor, x => uieffect_energy.colorFactor = x, 0.15f, 1f).OnComplete(() =>
            //{
            //    EffectToLittle();
            //});
        }
        else
        {
            energyHuXi = false;
            for(int i = 0; i < skillBtnViewList.Count; i++)
            {
                UIEffect uieffect_energy = skillBtnViewList[i].uieffect_energy;

                uieffect_energy.DOKill();
                uieffect_energy.colorFactor = 0;
                skillBtnViewList[i].RefreshShow();
            }
         
            btn_big.gameObject.SetActive(false);
        }
    }
    void EffectToBig()
    {
        for (int i = 0; i < skillBtnViewList.Count; i++)
        {
            UIEffect uieffect_energy = skillBtnViewList[i].uieffect_energy;
            skillBtnViewList[i].RefreshShow();
            uieffect_energy.colorFactor = 0.4f;

        }
    }

        //}
        //void EffectToLittle()
        //{
        //    for (int i = 0; i < skillBtnViewList.Count; i++)
        //    {
        //        UIEffect uieffect_energy = skillBtnViewList[i].uieffect_energy;

        //        DOTween.To(() => uieffect_energy.colorFactor, x => uieffect_energy.colorFactor = x, 0, 1f).OnComplete(() =>
        //        {
        //            if (energyHuXi)
        //                EffectToBig();
        //        });
        //    }


        //}
        /// <summary>
        /// 能量条改变
        /// </summary>
        /// <param name="param"></param>
        void OnBattleProChange(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        PropertyIdType idType = (PropertyIdType)param[1];

        if (p.onlyId == p1.onlyId)
        {
            p1 = p;
            if (idType == PropertyIdType.MpNum)
            {
                SinglePropertyData p1MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p1);

                float beforeFill = img_mp.fillAmount;
                int val = txt_mp.text.IndexOf("%");
                int beforeMp = txt_mp.text.Substring(0,txt_mp.text.Length-1).ToInt32();
                img_p1EnergyBar.DOFillAmount(p1MPPro.num / (float)p1MPPro.limit, .5f);
                img_mp.DOFillAmount(p1MPPro.num / (float)p1MPPro.limit, .5f).OnComplete(() =>
                {     
                    //cd如果满了 显示特效
                    if (beforeFill<1
                   && p1MPPro.num >= (float)p1MPPro.limit
                    && btn_big.gameObject.activeInHierarchy)
                    {
                        for(int i = 0; i < skillBtnViewList.Count; i++)
                        {
                            PanelManager.Instance.OpenSingle<zhandouCD>(trans_allEffectParent, trans_skillBtnViewParentList[i].transform.position);
                        }
                    }
                 
                });
                //txt_mp.DOText(p1MPPro.Num + "%", 0.5f);
                
                DOTween.To(() => beforeMp, (x) =>

                {
                    beforeMp = x;
                    txt_mp.SetText(x + "%");
                }, p1MPPro.num, .5f);
                ShowBigButton();
           
            }
        }

        else if(p.onlyId==p2.onlyId)
        {
            p2 = p;
            if (idType == PropertyIdType.MpNum)
            {
                img_p2EnergyBar.DOFillAmount(RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p).num / (float)100, .5f);

            }
        }


    }
    /// <summary>
    /// 显示加血
    /// </summary>
    void ShowAddHp(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        int num =(int)args[1];
        SinglePropertyData hpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p);

        if (p.onlyId == p1.onlyId)
        {

            img_p1HpBar.DOKill();
            txt_hp1.SetText(hpPro.num + "/" + hpPro.limit);
            img_p1HpBar.DOFillAmount(hpPro.num / (float)hpPro.limit,.3f);
        }
        else if(p.onlyId==p2.onlyId)
        {
            img_p2HpBar.DOKill();
            txt_hp2.SetText(hpPro.num + "/" + hpPro.limit);
            img_p2HpBar.DOFillAmount(hpPro.num / (float)hpPro.limit, .3f);
        }
        if(p.onlyId==p1.onlyId
            || p.onlyId == p2.onlyId)
        {
            AttackResData attackResData = new AttackResData(num, false, false, p, null, 0);
            attackResData.showDeHP = num;
            AttackResData[] theParam = { attackResData };
            DeHpTxtShow(theParam);
        }
     
    }

    /// <summary>
    /// 掉血
    /// </summary>
    void OnLoseHp(object[] param)
    {
        AttackResData res = param[0] as AttackResData;
        if (res == null)
            return;
        PeopleData peopleData = res.deHpPeople;

        Image hpBar = null;
        Text txt = null;
        if (peopleData.onlyId == p1.onlyId)
        {
            if (trans_huZhao1.gameObject.activeInHierarchy)
            {
                BattleBuff battleBuff= BattleManager.Instance.FindBuffTypeBuff(p1, BattleBuffType.HuZhao);
                img_huZhao1.fillAmount = battleBuff.remainHuZhaoNum / (float)battleBuff.totalHuZhaoNum;
                txt_huZhao1.SetText(battleBuff.remainHuZhaoNum + "/" + battleBuff.totalHuZhaoNum);
            }
            hpBar = img_p1HpBar;
            txt = txt_hp1;

        }
        else
        {
            hpBar = img_p2HpBar;
            txt = txt_hp2;
            if (trans_huZhao2.gameObject.activeInHierarchy)
            {
                BattleBuff battleBuff = BattleManager.Instance.FindBuffTypeBuff(p2, BattleBuffType.HuZhao);
                img_huZhao2.fillAmount = battleBuff.remainHuZhaoNum / (float)battleBuff.totalHuZhaoNum;
                txt_huZhao2.SetText(battleBuff.remainHuZhaoNum + "/" + battleBuff.totalHuZhaoNum);
            }
        }
        SinglePropertyData hpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
        txt.SetText(hpPro.num + "/" + hpPro.limit);


        hpBar.fillAmount = hpPro.num / (float)hpPro.limit;


    }

    /// <summary>
    /// 攻击cd
    /// </summary>
    void OnAttackCDShow(object[] param)
    {
        ulong onlyId = (ulong)param[0];
        float cdTimer = (float)param[1];
        float cdTime = (float)param[2];

        if (onlyId == p1.onlyId)
        {
            img_p1AttackCD.fillAmount = cdTimer / cdTime;
        }
        else
        {
            img_p2AttackCD.fillAmount = cdTimer / cdTime;

        }
    }

    /// <summary>
    /// 开始qte TODO具体读配置
    /// </summary>
    public void QTE(object[] args)
    {
        //trans_mp.gameObject.SetActive(false);
        //trans_mai.gameObject.SetActive(true);
        //txt_maiFunctionName.gameObject.SetActive(false);
        //img_maiBlackMask.DOFade(.8f, 1);
        curJingMaiIDList.Clear();
        curMaiList.Clear();

        possibleList = new List<List<JingMaiIDType>>();

        curChoosedBigIndex = (int)args[0];
       // List<JingMaiIDType> idList = new List<JingMaiIDType>();

        SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p1.allSkillData.equippedSkillIdList[curChoosedBigIndex], p1.allSkillData);
        SkillSetting setting = DataTable.FindSkillSetting(singleSkillData.skillId);
        List<int> mai = CommonUtil.SplitCfgOneDepth(setting.JingMai);
        List<JingMaiIDType> theList = new List<JingMaiIDType>();

        int bigLevel = (singleSkillData.skillLevel - 1) / 10 + 1;

        for (int j = 0; j < bigLevel + 1; j++)
        {
            curJingMaiIDList.Add((JingMaiIDType)mai[j]);
        }

        ////只出现已携带技能的qte
        //for (int i = 1; i < p1.allSkillData.equippedSkillIdList.Count; i++)
        //{
        //    SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p1.allSkillData.equippedSkillIdList[i], p1.allSkillData) ;
        //    SkillSetting setting = DataTable.FindSkillSetting(singleSkillData.skillId);
        //    List<int> mai = CommonUtil.SplitCfgOneDepth(setting.jingMai);
        //    List<JingMaiIDType> theList = new List<JingMaiIDType>();

        //    int bigLevel = (singleSkillData.skillLevel - 1) / 10 + 1;

        //    for (int j = 0; j < bigLevel+1; j++)
        //    {
        //        theList.Add((JingMaiIDType)mai[j]);
        //    }
        //    possibleList.Add(theList);
        //}


        //for (int i = 0; i < possibleList.Count; i++)
        //{
        //    JingMaiIDType theIdType = possibleList[i][0];
        //    for (int j = 0; j < maiList.Count; j++)
        //    {
        //        SingleMai mai = maiList[j];
        //        if (mai.jingMaiIDType == theIdType)
        //        {
        //            mai.Init(this);
        //            mai.OnLight(true);
        //            SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p1.allSkillData.equippedSkillIdList[i + 1],
        //                p1.allSkillData);
        //            mai.ShowCD(singleSkillData);
        //            //引导
        //            //第一次打长老 引导放大
        //            if (RoleManager.Instance._CurGameInfo.SceneData!=null)
        //            {
        //                if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Battle_DaZhaoClickMai).ToString()).ToInt32() == 0)
        //                {
        //                    PanelManager.Instance.ShowWithTxtTaskGuidePanel(mai.gameObject, "气集满了，调动经脉，释放绝学！");
        //                    TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Battle_DaZhaoClickMai).ToString());
        //                }
        //            }

        //        }
        //    }

        //}


        curMaiIndex = 0;
        //curMaiList[curMaiIndex].Init(this);
        //curMaiList[curMaiIndex].OnLight(true);
        //一个个亮起来
        OnShowBaGua();

    }

    /// <summary>
    /// 显示八卦
    /// </summary>
    void OnShowBaGua()
    {    //播放点脉声
        AuditionManager.Instance.PlayVoice(Camera.main.transform,
            ResourceManager.Instance.GetObj<AudioClip>(ConstantVal.audioFolderPath + ConstantVal.audio_clickMai));
        JingMaiIDType type = curJingMaiIDList[curMaiIndex];
        baGuaMaiList[(int)type].gameObject.SetActive(true);
        baGuaMaiList[(int)type].GetComponent<OutlineEx>().enabled = false;
        curMaiIndex++;
        if (curMaiIndex < curJingMaiIDList.Count)
        {
            qteTimer = 0;
            startQTE = true;
        }
        else
        {
            //全亮
            for(int i = 0; i < baGuaMaiList.Count; i++)
            {
                Image img = baGuaMaiList[i];
                if (img.gameObject.activeInHierarchy)
                {
                    img.GetComponent<OutlineEx>().enabled = true;
                }
             }
            //发招
            EventCenter.Broadcast(TheEventType.SkillAttack, p1.onlyId, curChoosedBigIndex);

        }
    }

    
    /// <summary>
    /// 点到正确的
    /// </summary>
    public void OnClickedMai(SingleMai mai)
    {
        //播放点脉声
        AuditionManager.Instance.PlayVoice(Camera.main.transform,
            ResourceManager.Instance.GetObj<AudioClip>(ConstantVal.audioFolderPath+ ConstantVal.audio_clickMai));
        //特效
        //如果还未选过脉路
        if (curJingMaiIDList.Count == 0)
        {
            for(int i = 0; i < possibleList.Count; i++)
            {
                if (mai.jingMaiIDType == possibleList[i][0])
                {
                    curJingMaiIDList = possibleList[i];
                    choosedMainFunctionIndex = i;
                }
                else
                {
                    for (int j = 0; j < maiList.Count; j++)
                    {
                        SingleMai theMai = maiList[j];
                        if (theMai.jingMaiIDType != mai.jingMaiIDType
                            && theMai.GetComponent<Light>())
                        {
                            theMai.Disappear();
                        }
                    }
                }
            }
            //其它的都熄灭

            curMaiList.Add(mai);
            for (int i = 1; i < curJingMaiIDList.Count; i++)
            {
                JingMaiIDType theIdType = curJingMaiIDList[i];
                for (int j = 0; j < maiList.Count; j++)
                {
                    SingleMai theMai = maiList[j];
                    if (theMai.jingMaiIDType == theIdType)
                    {
                        curMaiList.Add(theMai);
                    }
                }
            }
        }
        if (curMaiIndex >= 1)
        {
 
            AuditionManager.Instance.PlayVoice(Camera.main.transform,
                ResourceManager.Instance.GetObj<AudioClip>(ConstantVal.audioFolderPath + ConstantVal.audio_electronic),0.5f);
            //闪电链
            Transform startPos = curMaiList[curMaiIndex - 1].transform;
            Transform endPos = curMaiList[curMaiIndex].transform;
            PanelManager.Instance.OpenSingle<MaiLineView>(trans_effectParent, startPos, endPos);
        }
        PanelManager.Instance.OpenSingle<ClickMai>(trans_effectParent, curMaiList[curMaiIndex].transform);

        if (curMaiIndex< curJingMaiIDList.Count - 1)
        {   
           

            curMaiIndex++;
            curMaiList[curMaiIndex].Init(this);

            curMaiList[curMaiIndex].OnLight(false);
            //引导第二次释放绝学 消失的时候要去掉引导
            //第一次打长老 引导放大
            if (RoleManager.Instance._CurGameInfo.SceneData != null)
            {
                if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Battle_UseDaZhao).ToString()).ToInt32() == 0)
                {
                     TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Battle_UseDaZhao).ToString());
                }
            }

        }
        else
        {
            txt_maiFunctionName.DOKill();
            txt_maiFunctionName.transform.DOKill();

            txt_maiFunctionName.gameObject.SetActive(true);
            txt_maiFunctionName.transform.localScale = Vector3.zero;
            txt_maiFunctionName.color = new Color(txt_maiFunctionName.color.r, txt_maiFunctionName.color.g, txt_maiFunctionName.color.b, 0);
            txt_maiFunctionName.DOFade(1, 0.5f).OnComplete(() =>
            {
                
            });

            txt_maiFunctionName.transform.DOScale(1, 0.5f).OnComplete(() =>
            {
                if (txt_maiFunctionName.gameObject.activeInHierarchy)
                {
                    txt_maiFunctionName.transform.DOScale(1, 0.5f).OnComplete(() =>
                    {
                        if (txt_maiFunctionName.gameObject.activeInHierarchy)
                        {
                            txt_maiFunctionName.transform.DOScale(20, 0.5f);
                            txt_maiFunctionName.DOFade(0, 0.5f);
                        }

                    });
                }
            
            });

            //txt_maiFunctionDes.SetText(testMaiFunctionDes[choosedMainFunctionIndex]);
            SingleSkillData skillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p1.allSkillData.equippedSkillIdList[choosedMainFunctionIndex + 1], p1.allSkillData);

            SkillSetting skillSetting = BattleManager.Instance.FindSkillSetting(skillData.skillId);
            txt_maiFunctionName.SetText(skillSetting.Name);

            //点完了 发招 TODO 这个不一定是1 可能是别的技能
            PanelManager.Instance.CloseTaskGuidePanel();
            EventCenter.Broadcast(TheEventType.SkillAttack, p1.onlyId,choosedMainFunctionIndex+1);
        }
    }
    /// <summary>
    /// 点击失败，全灭
    /// </summary>
    public void OnLoseClickMai()
    {
        PanelManager.Instance.CloseTaskGuidePanel();
        //PanelManager.Instance.OpenFloatWindow("调息失败，经脉紊乱，流失真气");
        RoleManager.Instance.DeBattleProperty(PropertyIdType.MpNum, (int)-100, p1);
        EventCenter.Broadcast(TheEventType.SkillAttack, p1.onlyId, 0);
        OnDisappearQTE();
    }

    /// <summary>
    /// 筋脉消失
    /// </summary>
    void OnDisappearQTE()
    {
        img_maiBlackMask.DOFade(0, 1);

        PanelManager.Instance.CloseAllSingle(trans_effectParent);
        for(int i = 0; i < curMaiList.Count; i++)
        {
            curMaiList[i].Disappear();
        }
        trans_mai.gameObject.SetActive(false);
        //trans_mp.gameObject.SetActive(true);

        //全亮
        for (int i = 0; i < baGuaMaiList.Count; i++)
        {
            baGuaMaiList[i].gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 掉血
    /// </summary>
    public void DeHpTxtShow(object[] param)
    {
        AttackResData attackResData = param[0] as AttackResData;
        if (attackResData == null)
            return;
        Transform theParent = null;
        //我掉血
        if (attackResData.deHpPeople.onlyId == p1.onlyId)
        {
            theParent = trans_loseHpParent1;
        }
        else if(attackResData.deHpPeople.onlyId == p2.onlyId)
        {
            theParent = trans_loseHpParent2;
        }
        if (theParent != null)
        {

            int groupCount = theParent.childCount;
            if (groupCount == 0)
            {
                //创建新group
                PanelManager.Instance.OpenSingle<DeHpGroupView>(theParent);
            }
            DeHpGroupView choosedGroup = null;
            for (int i = groupCount - 1; i >= 0; i--)
            {
                DeHpGroupView theGroup = theParent.GetChild(i).GetComponent<DeHpGroupView>();
                if (!theGroup.cannotContainNewTxt)
                {
                    choosedGroup = theGroup;
                    break;
                }
            }
            if (choosedGroup == null)
                choosedGroup = PanelManager.Instance.OpenSingle<DeHpGroupView>(theParent);

            choosedGroup.AddDeHpTxt(attackResData);

        }


    }

    /// <summary>
    /// buff作用显示
    /// </summary>
    /// <param name="param"></param>
    public void OnBuffFunctionShow(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        string str = (string)param[1];
        Transform theParent = null;
        if (p.onlyId == p1.onlyId)
            theParent = trans_buffFunctionTxtParent1;
        else if (p.onlyId == p2.onlyId)
            theParent = trans_buffFunctionTxtParent2;

        AddSingle<BuffFunctionTxtView>(theParent, str);
    }

    public override void Clear()
    {
        base.Clear();
        EventCenter.Remove(TheEventType.BattleNextRound, NextRound);
        EventCenter.Remove(TheEventType.BattleDeHpShow, OnLoseHp);
        EventCenter.Remove(TheEventType.AttackCDShow, OnAttackCDShow);
        EventCenter.Remove(TheEventType.BattleDeHpShow, DeHpTxtShow);

        EventCenter.Remove(TheEventType.DisappearQTE, OnDisappearQTE);
        EventCenter.Remove(TheEventType.StartQTE, QTE);

        PanelManager.Instance.CloseAllSingle(trans_effectParent);
        PanelManager.Instance.CloseAllSingle(trans_allEffectParent);
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent1);
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent2);
        ClearCertainParentAllSingle<GetAwardItemView>(trans_awardGrid);
        awardItemViewList.Clear();
        ClearCertainParentAllSingle<FinishKillEffect>(trans_buffFunctionTxtParent1);
        ClearCertainParentAllSingle<FinishKillEffect>(trans_buffFunctionTxtParent2);
        ClearCertainParentAllSingle<SingleYuanSuPointView>(trans_yuanSuShowParent1);
        ClearCertainParentAllSingle<SingleYuanSuPointView>(trans_yuanSuShowParent2);

        for (int i = 0; i < baGuaMaiList.Count; i++)
        {
            baGuaMaiList[i].gameObject.SetActive(false);
        }

        startQTE = false;
        energyHuXi = false;
        skillBtnViewList.Clear();
        qieRenStudentViewList.Clear();
        enemyStudentViewList.Clear();
        yuanSuToShowPool1.Clear();
        curShowYuanSuList1.Clear();
        yuanSuMoving1 = false;
        yuanSuToShowPool2.Clear();
        curShowYuanSuList2.Clear();
        yuanSuMoving2 = false;

        // 清理队伍成员头像
        //ClearTeamMemberViews();
    }


    public void ShowGameEnd(object[] param)
    {
        trans_gameEnd.gameObject.SetActive(true);
        trans_win.gameObject.SetActive(false);
        trans_lose.gameObject.SetActive(false);
        trans_downBtns.gameObject.SetActive(false);

        PeopleData p = param[0] as PeopleData;
        ulong onlyId = p.onlyId;
        //玩家赢
        if (onlyId == p1.onlyId)
        {
            if (BattleManager.Instance.curBattleType == BattleType.Match)
            {
   
 
                     ShowWin();
               
            }
            else
            {
                ShowWin();

            }

        }
        else
        {
                     ShowLose();
        }
        PanelManager.Instance.CloseTaskGuidePanel();
    }

    /// <summary>
    /// 显示赢
    /// </summary>
    void ShowWin()
    {
        trans_win.gameObject.SetActive(true);
        trans_lose.gameObject.SetActive(false);

        img_winLabel.DOKill();

        img_winLabel.transform.localScale = Vector3.zero;
        img_winLabel.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), .2f).OnComplete(() =>
        {
            img_winLabel.transform.DOScale(new Vector3(1, 1, 1), .2f);
        });

        //直接显示奖励
        win = true;
        closeResultAction = BackLastScene;
        startBattleEndAppearAward = true;
        awardItemViewList.Clear();
        
        ClearCertainParentAllSingle<GetAwardItemView>(trans_awardGrid);
   
        battleEndAppearAwardTimer = 0;
        curItemIndex = 0;
        if (BattleManager.Instance.curBattleType == BattleType.Match)
        {
            ItemData item = new ItemData();
            item.settingId = (int)ItemIdType.RongYuBi;
            item.count = (ulong)ConstantVal.winMatchRongYuBi;
            awardItemList = new List<ItemData> { item };
        }
        else
        {
            awardItemList = BattleManager.Instance.roleAward;

        }
        if (awardItemList != null && awardItemList.Count > 0)
        {
            for (int i = 0; i < awardItemList.Count; i++)
            {
                GetAwardItemView view = AddSingle<GetAwardItemView>(trans_awardGrid, awardItemList[i]);
                awardItemViewList.Add(view);
            }
        }
   
    }


    /// <summary>
    /// 显示输
    /// </summary>
    void ShowLose()
    {
        closeResultAction = BackLastScene;

        trans_win.gameObject.SetActive(false);
        trans_lose.gameObject.SetActive(true);

        img_loseLabel.DOKill();
        img_loseLabel.color = new Color(img_loseLabel.color.r, img_loseLabel.color.g, img_loseLabel.color.b, 0);

        img_loseLabel.DOFade(1, .5f);
        win = false;

        if (BattleManager.Instance.curBattleType == BattleType.LevelBattle
            || BattleManager.Instance.curBattleType == BattleType.DiShuFirstBattle
            || BattleManager.Instance.curBattleType == BattleType.LiMaoZhangMenBattle
            || BattleManager.Instance.curBattleType == BattleType.Match)
        {
            trans_downBtns.gameObject.SetActive(false);
        }
        else
        {
            trans_downBtns.gameObject.SetActive(true);
        }

    }

    /// <summary>
    /// 增加战斗buff
    /// </summary>
    void OnAddBattleBuff(object[] args)
    {
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent1);
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent2);

        for (int i = 0; i < BattleManager.Instance.buffList1.Count; i++)
        {
            BattleBuff battleBuff = BattleManager.Instance.buffList1[i];
            AddSingle<BuffTxtView>(trans_buffTxtParent1, battleBuff);
        }
        for (int i = 0; i < BattleManager.Instance.buffList2.Count; i++)
        {
            BattleBuff battleBuff = BattleManager.Instance.buffList2[i];
            AddSingle<BuffTxtView>(trans_buffTxtParent2, battleBuff);
        }
        PeopleData p = args[0] as PeopleData;
        BattleBuff buff = args[1] as BattleBuff;

        if (p.onlyId == p1.onlyId)
        {
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao)
            {
                trans_huZhao1.gameObject.SetActive(true);
                img_huZhao1.fillAmount = buff.remainHuZhaoNum / (float)buff.totalHuZhaoNum;
                txt_huZhao1.SetText(buff.remainHuZhaoNum + "/" + buff.totalHuZhaoNum);
            }
        }
        else if (p.onlyId == p2.onlyId)
        {
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao)
            {
                trans_huZhao2.gameObject.SetActive(true);
                img_huZhao2.fillAmount = buff.remainHuZhaoNum / (float)buff.totalHuZhaoNum;
                txt_huZhao2.SetText(buff.remainHuZhaoNum + "/" + buff.totalHuZhaoNum);
            }
        }
    }


    /// <summary>
    /// 移除战斗buf
    /// </summary>
    /// <param name="ars"></param>
    void OnRemoveBattleBuff(object[] args)
    {
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent1);
        ClearCertainParentAllSingle<BuffTxtView>(trans_buffTxtParent2);

        for (int i = 0; i < BattleManager.Instance.buffList1.Count; i++)
        {
            BattleBuff battleBuff = BattleManager.Instance.buffList1[i];
            AddSingle<BuffTxtView>(trans_buffTxtParent1, battleBuff);
        }
        for (int i = 0; i < BattleManager.Instance.buffList2.Count; i++)
        {
            BattleBuff battleBuff = BattleManager.Instance.buffList2[i];
            AddSingle<BuffTxtView>(trans_buffTxtParent2, battleBuff);
        }
        PeopleData p = args[0] as PeopleData;
        if (args.Length == 2)
        {
            BattleBuff buff = args[1] as BattleBuff;

            if (p.onlyId == p1.onlyId)
            {

                if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao)
                {
                    trans_huZhao1.gameObject.SetActive(false);

                }
            }
            else if (p.onlyId == p2.onlyId)
            {
                if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao)
                {
                    trans_huZhao2.gameObject.SetActive(false);

                }
            }
        }
       
    }

    /// <summary>
    /// 放大
    /// </summary>
    public void OnFangDa(int skillId)
    {
        int index = p1.allSkillData.equippedSkillIdList.IndexOf(skillId);
        BattleManager.Instance.OnBigAttack(index);

    }

    #region 新版队伍成员头像显示

    /// <summary>
    /// 显示队伍成员头像（玩家方和敌人方）
    /// </summary>
    void ShowTeamMemberViews()
    {
        //ShowP1TeamMembers();
        ShowP2TeamMembers();
    }

    /// <summary>
    /// 显示玩家方队伍成员头像
    /// </summary>
    //void ShowP1TeamMembers()
    //{
    //    p1TeamMemberViewList.Clear();
    //    if (trans_p1TeamGrid != null)
    //    {
    //        ClearCertainParentAllSingle<BattleTeamMemberView>(trans_p1TeamGrid);

    //        for (int i = 0; i < BattleManager.Instance.p1List.Count; i++)
    //        {
    //            PeopleData memberData = BattleManager.Instance.p1List[i];
    //            BattleTeamMemberView view = AddSingle<BattleTeamMemberView>(trans_p1TeamGrid, memberData, this, true, i);
    //            p1TeamMemberViewList.Add(view);

    //            // 设置当前战斗角色高亮
    //            if (i == BattleManager.Instance.p1Index)
    //            {
    //                view.SetAsCurrent(true);
    //            }

    //            // 检查是否死亡
    //            SinglePropertyData hpData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, memberData);
    //            if (hpData.num <= 0)
    //            {
    //                view.SetDead(true);
    //            }
    //        }
    //    }
    //}

    /// <summary>
    /// 显示敌人方队伍成员头像
    /// </summary>
    void ShowP2TeamMembers()
    {
        p2TeamMemberViewList.Clear();
        if (trans_p2TeamGrid != null)
        {
            ClearCertainParentAllSingle<BattleTeamMemberView>(trans_p2TeamGrid);

            for (int i = 0; i < BattleManager.Instance.p2List.Count; i++)
            {
                PeopleData memberData = BattleManager.Instance.p2List[i];
                BattleTeamMemberView view = AddSingle<BattleTeamMemberView>(trans_p2TeamGrid, memberData, this, false, i);
                p2TeamMemberViewList.Add(view);

                // 设置当前战斗角色高亮
                if (i == BattleManager.Instance.p2Index)
                {
                    view.SetAsCurrent(true);
                }

                // 检查是否死亡
                SinglePropertyData hpData = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, memberData);
                if (hpData.num <= 0)
                {
                    view.SetDead(true);
                }
            }
        }
    }

    /// <summary>
    /// 更新队伍成员高亮状态（切人时调用）
    /// </summary>
    //void UpdateTeamMemberHighlight()
    //{
    //    // 更新玩家方高亮
    //    for (int i = 0; i < p1TeamMemberViewList.Count; i++)
    //    {
    //        p1TeamMemberViewList[i].SetAsCurrent(i == BattleManager.Instance.p1Index);
    //    }

    //    // 更新敌人方高亮
    //    for (int i = 0; i < p2TeamMemberViewList.Count; i++)
    //    {
    //        p2TeamMemberViewList[i].SetAsCurrent(i == BattleManager.Instance.p2Index);
    //    }
    //}

    /// <summary>
    /// 战斗角色死亡时更新UI
    /// </summary>
    //void OnBattlePeopleDeadUpdateUI(object[] args)
    //{
    //    ulong deadOnlyId = (ulong)args[0];

    //    // 检查玩家方
    //    for (int i = 0; i < p1TeamMemberViewList.Count; i++)
    //    {
    //        if (p1TeamMemberViewList[i].peopleData.onlyId == deadOnlyId)
    //        {
    //            p1TeamMemberViewList[i].SetDead(true);
    //            break;
    //        }
    //    }

    //    // 检查敌人方
    //    for (int i = 0; i < p2TeamMemberViewList.Count; i++)
    //    {
    //        if (p2TeamMemberViewList[i].peopleData.onlyId == deadOnlyId)
    //        {
    //            p2TeamMemberViewList[i].SetDead(true);
    //            break;
    //        }
    //    }
    //}

    /// <summary>
    /// 显示协战来咯提示
    /// </summary>
    public void ShowXieZhanLaiLo()
    {
        if (trans_xieZhanLaiLoParent != null)
        {
            AddSingle<XieZhanLaiLoView>(trans_xieZhanLaiLoParent);
        }
    }

    /// <summary>
    /// 清理队伍成员头像
    /// </summary>
    //void ClearTeamMemberViews()
    //{
    //    if (trans_p1TeamGrid != null)
    //    {
    //        ClearCertainParentAllSingle<BattleTeamMemberView>(trans_p1TeamGrid);
    //    }
    //    if (trans_p2TeamGrid != null)
    //    {
    //        ClearCertainParentAllSingle<BattleTeamMemberView>(trans_p2TeamGrid);
    //    }
    //    p1TeamMemberViewList.Clear();
    //    p2TeamMemberViewList.Clear();
    //}

    #endregion
}
