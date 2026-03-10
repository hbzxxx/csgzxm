using DG.Tweening;
using Framework.Data;
using Google.Protobuf.Collections;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPreparePanel : PanelBase
{
    public Image img_line_leftPhase1_1;
    public Image img_line_leftPhase1_2;
    public Image img_line_leftPhase1_3;
    public Image img_line_leftPhase1_4;

    public Image img_line_leftPhase1_1Win;
    public Image img_line_leftPhase1_2Win;
    public Image img_line_leftPhase1_3Win;
    public Image img_line_leftPhase1_4Win;


    public Image img_line_leftPhase2_1;
    public Image img_line_leftPhase2_2;

    public Image img_line_leftPhase2_1Win;
    public Image img_line_leftPhase2_2Win;
    public Image img_line_leftPhase3;

    public Image img_line_rightPhase1_1;
    public Image img_line_rightPhase1_2;
    public Image img_line_rightPhase1_3;
    public Image img_line_rightPhase1_4;

    public Image img_line_rightPhase1_1Win;
    public Image img_line_rightPhase1_2Win;
    public Image img_line_rightPhase1_3Win;
    public Image img_line_rightPhase1_4Win;


    public Image img_line_rightPhase2_1;
    public Image img_line_rightPhase2_2;

    public Image img_line_rightPhase2_1Win;
    public Image img_line_rightPhase2_2Win;
    public Image img_line_rightPhase3;

    //public List<PeopleData> leftPeopleList = new List<PeopleData>();
    //public List<PeopleData> rightPeopleList = new List<PeopleData>();

    //phase1
    public List<SingleMatchPeopleView> left1Match;//左1 左2
    public List<SingleMatchPeopleView> left2Match;//左3 左4


    public List<SingleMatchPeopleView> right1Match;//右1 右2
    public List<SingleMatchPeopleView> right2Match;//右3 右4

    //phase2
    public List<SingleMatchPeopleView> phase2LeftMatch;
    public List<SingleMatchPeopleView> phase2RightMatch;

    public float processMoveTime = 1f;
    public float processMoveTimer = 0;
    public bool startProcessMove;

    //public int curPhase;//当前第几阶段
    public Transform trans_end;
    public Text txt_end;//战斗结束
    public Transform trans_awardGrid;
    public Transform trans_equipExp;//装备经验
    public Text txt_equipExp;
    public Text txt_equipLv;//装备等级
    public Image img_equipExpBar;
    public float totalExpMoveTime = 1;//一秒动完
    public Text txt_scoreChange;//分数变化

    public Image img_beforeRank;
    public Transform beforeRankStarGrid;

    public Image img_afterRank;
    public Transform afterRankStarGrid;


    //#region 排位变化动画 

    //#endregion

    public Button btn_back;//退出
    bool refresh = false;//刷新显示
    bool win;//赢了
    public Text txt_needUpgradeZongMen;//需升级以提升更高段位

    public override void Init(params object[] args)
    {
        base.Init(args);
        refresh = (bool)args[0];
        addBtnListener(btn_back, () =>
        {
            BattleManager.Instance.MatchResult(win);

            GameSceneManager.Instance.GoToScene(SceneType.Mountain);

            //自动存档 并且时间要过一天
            GameTimeManager.Instance.DayPlus();
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        if (refresh)
        {
            left1Match[0].Init(BattleManager.Instance.leftMatch[0][0]);
            left1Match[1].Init(BattleManager.Instance.leftMatch[0][1]);

            left2Match[0].Init(BattleManager.Instance.leftMatch[1][0]);
            left2Match[1].Init(BattleManager.Instance.leftMatch[1][1]);

            right1Match[0].Init(BattleManager.Instance.rightMatch[0][0]);
            right1Match[1].Init(BattleManager.Instance.rightMatch[0][1]);

            right2Match[0].Init(BattleManager.Instance.rightMatch[1][0]);
            right2Match[1].Init(BattleManager.Instance.rightMatch[1][1]);
            OnInitPrepare();

        }

        EventCenter.Register(TheEventType.NextPhase, OnNextPhase);

        trans_end.gameObject.SetActive(false);
        startProcessMove = true;
        processMoveTimer = 0;
    }

    public void OnInitPrepare()
    {
        img_line_leftPhase1_1.fillAmount = 0;
        img_line_leftPhase1_2.fillAmount = 0;
        img_line_leftPhase1_3.fillAmount = 0;
        img_line_leftPhase1_4.fillAmount = 0;

        img_line_leftPhase1_1Win.fillAmount = 0;
        img_line_leftPhase1_2Win.fillAmount = 0;
        img_line_leftPhase1_3Win.fillAmount = 0;
        img_line_leftPhase1_4Win.fillAmount = 0;


        img_line_leftPhase2_1.fillAmount = 0;
        img_line_leftPhase2_2.fillAmount = 0;

        img_line_leftPhase2_1Win.fillAmount = 0;
        img_line_leftPhase2_2Win.fillAmount = 0;
        img_line_leftPhase3.fillAmount = 0;

        img_line_rightPhase1_1.fillAmount = 0;
        img_line_rightPhase1_2.fillAmount = 0;
        img_line_rightPhase1_3.fillAmount = 0;
        img_line_rightPhase1_4.fillAmount = 0;

        img_line_rightPhase1_1Win.fillAmount = 0;
        img_line_rightPhase1_2Win.fillAmount = 0;
        img_line_rightPhase1_3Win.fillAmount = 0;
        img_line_rightPhase1_4Win.fillAmount = 0;


        img_line_rightPhase2_1.fillAmount = 0;
        img_line_rightPhase2_2.fillAmount = 0;

        img_line_rightPhase2_1Win.fillAmount = 0;
        img_line_rightPhase2_2Win.fillAmount = 0;
        img_line_rightPhase3.fillAmount = 0;

        img_line_leftPhase1_1.DOFillAmount(1, 1);
        img_line_leftPhase1_2.DOFillAmount(1, 1);
        img_line_leftPhase1_3.DOFillAmount(1, 1);
        img_line_leftPhase1_4.DOFillAmount(1, 1);
        img_line_rightPhase1_1.DOFillAmount(1, 1);
        img_line_rightPhase1_2.DOFillAmount(1, 1);
        img_line_rightPhase1_3.DOFillAmount(1, 1);
        img_line_rightPhase1_4.DOFillAmount(1, 1);

        //curPhase = 0;
      
    }

    private void Update()
    {
        if (startProcessMove)
        {
            processMoveTimer += Time.deltaTime;
            if(processMoveTimer >= processMoveTime)
            {
                //curPhase++;
                startProcessMove = false;
                //战斗准备界面
                if (BattleManager.Instance.curPhase < 4)
                {
                    if (BattleManager.Instance.leftMatch[0][0].isPlayerZongMen)
                    {

                        SingleOtherZongMenData singleOtherZongMenData = null;
                        List<PeopleData> pList = new List<PeopleData>();
                        if (BattleManager.Instance.curPhase < 3)
                        {
                            singleOtherZongMenData = BattleManager.Instance.leftMatch[0][1];
                            pList = singleOtherZongMenData.pList;
                        }
                        else
                        {
                            singleOtherZongMenData = BattleManager.Instance.rightMatch[0][0];

                            pList = singleOtherZongMenData.pList;
                        }
                        string str = MatchManager.Instance.EnemyBeforeMatchMatchDialog(singleOtherZongMenData);
                        DialogData dialogData = new DialogData(pList[0], str);
                        DialogManager.Instance.CreateDialog(new List<DialogData> { dialogData }, () =>
                        {
                            gameObject.SetActive(false);

                            List<PeopleData> enemyList = new List<PeopleData>();
                            for (int i = 0; i < pList.Count; i++)
                            {
                                enemyList.Add(pList[i]);
                            }
                            PanelManager.Instance.OpenPanel<BattlePreparePanel>(PanelManager.Instance.trans_layer2, BattleType.Match, RoleManager.Instance.FindMyBattleTeamList(false, 0), enemyList);
                        });

                        
                    }
                    else
                    {
                        StartBattle();

                    }
                }
                
                else
                {
                    StartBattle();

                }

            }
        }

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    LevelInfo levelInfo = EquipmentManager.Instance.GetLevelInfo(RoleManager.Instance._CurGameInfo.playerPeople.CurEquip,500);

        //    int upgradeLevelNum = levelInfo.canReachLevel - levelInfo.beforeLevel;

        //    float curExpMoveTime = totalExpMoveTime / (float)(upgradeLevelNum + 1);

        //    SingleMove(curExpMoveTime, levelInfo.beforeLevel, levelInfo);

        //    RoleManager.Instance._CurGameInfo.playerPeople.CurEquip.CurLevel = levelInfo.canReachLevel;
        //    RoleManager.Instance._CurGameInfo.playerPeople.CurEquip.CurExp = levelInfo.ExpAfterUpgrade;
        //}
    }
    
    /// <summary>
    /// 下一阶段
    /// </summary>
    void OnNextPhase()
    {
        for(int i = 0; i < BattleManager.Instance.nextPrepareAnimDataList.Count; i++)
        {
            PrepareAnimData prepareAnimData = BattleManager.Instance.nextPrepareAnimDataList[i];
            OnWin(prepareAnimData.left, prepareAnimData.phase, prepareAnimData.index);
        }
        startProcessMove = true;
        processMoveTimer = 0;
        //phase2LeftMatch.Clear();
        //phase2LeftMatch.Add(BattleManager.Instance.leftMatch[0][0]);

    }



    /// <summary>
    /// 是否左边移动 第几场 第几个人赢了
    /// </summary>
    /// <param name="left"></param>
    /// <param name="phase"></param>
    /// <param name="index"></param>
    public void OnWin(bool left,int phase, int index)
    {
        switch (phase)
        {
            //第一场
            case 1:
                if (left)
                {
                    switch (index)
                    {
                        case 1:
                            img_line_leftPhase1_1Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_leftPhase2_1.DOFillAmount(1, 1);
                            });
                            break;
                        case 2:
                            img_line_leftPhase1_2Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_leftPhase2_1.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            });
                            break;
                        case 3:
                            img_line_leftPhase1_3Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_leftPhase2_2.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            }); break;
                        case 4:
                            img_line_leftPhase1_4Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_leftPhase2_2.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            }); 
                            break; 
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 1:
                            img_line_rightPhase1_1Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_rightPhase2_1.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            });
                            break;
                        case 2:
                            img_line_rightPhase1_2Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_rightPhase2_1.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            });
                            break;
                        case 3:
                            img_line_rightPhase1_3Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_rightPhase2_2.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            }); break;
                        case 4:
                            img_line_rightPhase1_4Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                                img_line_rightPhase2_2.DOFillAmount(1, 1).OnComplete(() =>
                                {
                                    // "开始下一场战斗";
                                });
                            });
                            break;
                    }
                }
                break;
                //第二场
            case 2:
                if (left)
                {
                    switch (index)
                    {
                        case 1:
                            img_line_leftPhase2_1Win.DOFillAmount(1, 1).OnComplete(() =>
                            {
                               
                                    // "开始下一场战斗";
                               
                            });
                            break;
                        case 2:
                            img_line_leftPhase2_2Win.DOFillAmount(1, 1).OnComplete(() =>
                            {

                                // "开始下一场战斗";

                            });
                            break;
                    }
                }
                else
                {
                    switch (index)
                    {
                        case 1:
                            img_line_rightPhase2_1Win.DOFillAmount(1, 1).OnComplete(() =>
                            {

                                // "开始下一场战斗";

                            });
                            break;
                        case 2:
                            img_line_rightPhase2_2Win.DOFillAmount(1, 1).OnComplete(() =>
                            {

                                // "开始下一场战斗";

                            });
                            break;
                    }
                }
                break;
            case 3:
                if (left)
                {
                    img_line_leftPhase3.DOFillAmount(1, 1).OnComplete(() =>
                    {
                        //夺冠面板打开
                    });
                }
                else
                {
                    img_line_rightPhase3.DOFillAmount(1, 1).OnComplete(() =>
                    {
                        //夺冠面板打开
                    });
                }
                break;
        }
    }




    public void StartBattle()
    {
        if (BattleManager.Instance.curPhase >= 4)
        {
            ShowMatchResult();
        }
        else
        {
            BattleManager.Instance.StartBattle();

            //如果有玩家在比赛，那么隐藏该准备面板，露出后面的战斗面板
            if (BattleManager.Instance.leftMatch[0][0].isPlayerZongMen)
            {
                gameObject.SetActive(false);
            }
        }

    }
    /// <summary>
    /// 显示结算界面
    /// </summary>
    void ShowMatchResult()
    {
        trans_end.gameObject.SetActive(true);
        win = false;
        string endTxtStr = "";
        switch (BattleManager.Instance.roleRank)
        {
            case 1:
                endTxtStr = "铩羽而归！";
                break;
            case 2:
                endTxtStr = "进入四强！";
                break;
            case 3:
                endTxtStr = "进入前两名！";
                break;
            case 4:
                endTxtStr = "获得冠军！";
                win = true;
                break;
        }
        txt_end.SetText(endTxtStr);
        ClearCertainParentAllSingle<WithCountItemView>(trans_awardGrid);
        for(int i=0;i< BattleManager.Instance.roleAward.Count; i++)
        {
            AddSingle<WithCountItemView>(trans_awardGrid, BattleManager.Instance.roleAward[i]);
        }
        RoleManager.Instance._CurGameInfo.allZongMenData.TheR = MatchManager.Instance.playerZongMenData.theR;

        RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel = MatchManager.Instance.playerZongMenData.curRankLevel;
        RoleManager.Instance._CurGameInfo.allZongMenData.CurStar = MatchManager.Instance.playerZongMenData.curStar;

        int scoreChange = (RoleManager.Instance._CurGameInfo.allZongMenData.TheR - BattleManager.Instance.beforeRankScore);
        string addScoreStr = "";
        if (scoreChange > 0)
            addScoreStr = "+";
        txt_scoreChange.SetText(RoleManager.Instance._CurGameInfo.allZongMenData.TheR+"("+( addScoreStr+scoreChange)+")");

        List<int> before = MatchManager.Instance.RankLevelByScore(BattleManager.Instance.beforeRankScore);
        List<int> after = MatchManager.Instance.RankLevelByScore(RoleManager.Instance._CurGameInfo.allZongMenData.TheR);

        img_beforeRank.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_paiwei" + before[0]);
        img_beforeRank.SetNativeSize();
        for (int i = 0; i < 5; i++)
        {
            if (i < before[1])
                beforeRankStarGrid.GetChild(i).gameObject.SetActive(true);
            else
                beforeRankStarGrid.GetChild(i).gameObject.SetActive(false);
        }

        img_afterRank.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_paiwei" + after[0]);
        img_afterRank.SetNativeSize();
        for (int i = 0; i < 5; i++)
        {
            if (i < after[1])
                afterRankStarGrid.GetChild(i).gameObject.SetActive(true);
            else
                afterRankStarGrid.GetChild(i).gameObject.SetActive(false);
        }
        if (RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel >= MatchManager.Instance.PlayerMaxRankLevel())
        {
            txt_needUpgradeZongMen.SetText("提高段位需要" + LanguageUtil.GetLanguageText((int)LanguageIdType.宗门)+"等级" + MatchManager.Instance.PlayerRankLevelNeedZongMenLevel(MatchManager.Instance.PlayerMaxRankLevel() + 1));
        }
        else
        {
            txt_needUpgradeZongMen.SetText("");
        }
    }

     


    public override void Clear()
    {
        base.Clear();
        EventCenter.Remove(TheEventType.NextPhase, OnNextPhase);
        PanelManager.Instance.CloseAllSingle(trans_awardGrid);
        trans_end.gameObject.SetActive(false);
        //trans_equipExp.gameObject.SetActive(false);
        //left1Match.Clear();
        //right1Match.Clear();
        //left2Match.Clear();
        //right2Match.Clear();
        //phase2LeftMatch.Clear();
        //phase2RightMatch.Clear();

    }

    public override void OnClose()
    {
        base.OnClose();
    }
}

public class PrepareAnimData
{
   public bool left;//是否左边的动
    public int phase;//第几轮
    public int index;//第几个人
    public PrepareAnimData(bool left,int phase,int index)
    {
        this.left = left;
        this.phase = phase;
        this.index = index;
    }
}