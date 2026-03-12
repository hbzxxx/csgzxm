using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Google.Protobuf.Collections;
using Framework.Data;
using Spine.Unity;

public class MatchPanel : PanelBase
{

    public Button btn_startPiPei;

    #region 自我属性
    public Text txt_todayRemain;
    public Image img_todayRemainBar;

    public Text txt_todayWinNum;
    public Image img_todayWinRemainBar;

    public List<Button> todayWinAwardBtnList;
    public List<Button> todayParticipateAwardBtnList;

    public Text txt_jjbNum;//多少币

    public Button btn_shop;//商店

    public Button btn_getDailyJieSuan;//领每日结算
    public Text txt_dailyAward;
    public Transform trans_getJieSuanAwarded;//每日结算已领取
    #endregion

    #region 排行榜

    public Button btn_duanWeiRank;//段位排行
    public Button btn_totalRank;//总排行

    public Transform grid_rank;
    public Transform grid_testrank;

    public Transform trans_selfRank;
    #endregion
    #region 匹配面板
    public Transform trans_piPeiPanel;
    public Text txt_timeCount;//匹配时间
    public Vector2Int singleTimeRange = new Vector2Int(0, 40);
    public Transform trans_leftGrid;
    public Transform trans_rightGrid;
    bool startPiPei = false;
    public float piPeiTimer = 0;
    public float piPeiTime = 0;
    public float piPeiTotalTimer = 0;
    public int curGeneratedPiPeiIndex = 0;
    public List<SingleOtherZongMenData> otherZongMenList = new List<SingleOtherZongMenData>();
    public List<SingleMatchZongMenView> matchZongMenViewList = new List<SingleMatchZongMenView>();
    public int preparedNum = 0;//准备ok的人

    bool startEnterBattleReady = false;//开始进入战斗准备
    public float enterBattleReadyTime = 1;
    public float enterBattleReadyTimer = 0;

    public Button btn_ready;//准备
    public Button btn_cancelReady;//取消准备
    #endregion

    #region 商店
    public Transform trans_shop;
    public Text txt_brushCD;
    public Button btn_closeShop;
    public Transform grid_shopItem;
    #endregion
    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_startPiPei, () =>
         {
             //弟子数量少了不建议匹配
             if (TeamManager.Instance.FindMyTeam1PNum() < 4)
             {
                 PanelManager.Instance.OpenFloatWindow("队伍人数不满4人");
                 return;
             }

             //int adNum = 0;
             //if (RoleManager.Instance._CurGameInfo.MatchData.WatchedADAddParticipateTime)
             //{
             //    adNum = ConstantVal.adAddParticipateMatchNum;
             //}
             int limit = ConstantVal.maxMatchParticipatePerDay + ConstantVal.matchTianJingBrushAddNum * RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum;
             if (RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum < limit)
             {
                 MatchManager.Instance.PiPei();

             }
             else
             {
                 PanelManager.Instance.OpenPanel<MatchParticipateTimeAddADPanel>(PanelManager.Instance.trans_layer2);
             }
         });
        addBtnListener(btn_ready, () =>
        {
            btn_ready.gameObject.SetActive(false);
            btn_cancelReady.gameObject.SetActive(true);
            matchZongMenViewList[0].Prepare(true);
        });
        addBtnListener(btn_cancelReady, () =>
        {
            btn_cancelReady.gameObject.SetActive(false);
            btn_ready.gameObject.SetActive(true);

            matchZongMenViewList[0].Prepare(false);
        });

        for(int i = 0; i < todayWinAwardBtnList.Count; i++)
        {
            int index = i;
            Button btn = todayWinAwardBtnList[i];
            addBtnListener(btn, () =>
            {
                if (RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[index] == (int)AccomplishStatus.Accomplished)
                {
                    SkeletonGraphic ske = btn.GetComponentInChildren<SkeletonGraphic>();
                    ske.AnimationState.SetAnimation(0, "baoxiang_dakai", false).Complete += delegate
                    {
                        MatchManager.Instance.OnGetDailiWinMatchAward(index);

                    };
                }
       
            });
        }

        addBtnListener(btn_duanWeiRank, () =>
        {
            ShowRank(true);
        });

        addBtnListener(btn_totalRank, () =>
        {
            ShowRank(false);
        });

        addBtnListener(btn_shop, () =>
        {
            //ShowShop();
            ShopPanel shopPanel= PanelManager.Instance.OpenPanel<ShopPanel>(PanelManager.Instance.trans_layer2,ShopTag.FangShi);
            shopPanel.btn_MatchShopTag.onClick.Invoke();
        });

        addBtnListener(btn_closeShop, () =>
        {
            trans_shop.gameObject.SetActive(false);
        });
        addBtnListener(btn_getDailyJieSuan, () =>
        {
            MatchManager.Instance.GetJieSuanAward();
        });
        RegisterEvent(TheEventType.StartPiPei, StartPiPei);
        RegisterEvent(TheEventType.OnAddMatchParticipateNum, ShowMyStatus);
        RegisterEvent(TheEventType.OnGetDailyWinAward, ShowMyStatus);
        RegisterEvent(TheEventType.GetMatchDailyJieSuanAward, ShowMyStatus);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        btn_duanWeiRank.onClick.Invoke();
        ShowMyStatus();

    }

    /// <summary>
    /// 显示我的状态
    /// </summary>
    void ShowMyStatus()
    {
        //int adNum = 0;
        //if (RoleManager.Instance._CurGameInfo.MatchData.WatchedADAddParticipateTime)
        //{
        //    adNum = ConstantVal.adAddParticipateMatchNum;
        //}

        txt_todayRemain.SetText("今日参赛次数："+RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum + "/" + (ConstantVal.maxMatchParticipatePerDay + ConstantVal.matchTianJingBrushAddNum* RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum));
        img_todayRemainBar.fillAmount = RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum / (float)ConstantVal.maxMatchParticipatePerDay;
        int winNum = RoleManager.Instance._CurGameInfo.MatchData.TodayWinNum;
        if (winNum >= ConstantVal.maxMatchWinNumPerDay)
            winNum = ConstantVal.maxMatchWinNumPerDay;
        txt_todayWinNum.SetText(winNum + "/" + ConstantVal.maxMatchWinNumPerDay);
        img_todayWinRemainBar.fillAmount = winNum / (float)ConstantVal.maxMatchWinNumPerDay;

        for(int i = 0; i < todayWinAwardBtnList.Count; i++)
        {
            if (i >= RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Count)
                continue;
            SkeletonGraphic ske = todayWinAwardBtnList[i].GetComponentInChildren<SkeletonGraphic>();
            if (RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[i] == (int)AccomplishStatus.Accomplished)
            {
                ske.AnimationState.SetAnimation(0, "baoxiang_huangdong", true);
            }else if (RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[i] == (int)AccomplishStatus.GetAward)
            {
                ske.AnimationState.SetAnimation(0, "baoxiang_2", true);

            }
            else
            {
                ske.AnimationState.SetAnimation(0, "baoxiang_1", true);

            }

        }


        txt_dailyAward.SetText(MatchManager.Instance.JieSuanAwardNum(RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel, RoleManager.Instance._CurGameInfo.allZongMenData.CurStar).ToString());
        if (RoleManager.Instance._CurGameInfo.MatchData.GetJieSuanAward)
        {
            btn_getDailyJieSuan.gameObject.SetActive(false);
            trans_getJieSuanAwarded.gameObject.SetActive(true);
        }
        else
        {
            btn_getDailyJieSuan.gameObject.SetActive(true);
            trans_getJieSuanAwarded.gameObject.SetActive(false);

        }
    }

    /// <summary>
    /// 显示排名
    /// </summary>
    /// <param name="curDuanWei"></param>
    void ShowRank(bool curDuanWei)
    {
        ClearCertainParentAllSingle<SingleRankView>(trans_selfRank);

        ClearCertainParentAllSingle<SingleRankView>(grid_rank);
        //排序
        quick_sort(RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList,0, RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count-1);

        int startRank = 0;
        if (curDuanWei)
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count; i++)
            {
                SingleOtherZongMenData data = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[i];
                if (data.curRankLevel == RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel)
                {
                    startRank = i;
                    break;
                }
            }
        }
    

        //排前20
        List<SingleOtherZongMenData> rankList = new List<SingleOtherZongMenData>();
        List<SingleRankView> rankViewList = new List<SingleRankView>();
        SingleOtherZongMenData pData = new SingleOtherZongMenData();
        pData.zongMenName = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName;
        pData.curRankLevel = RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel;
        pData.curStar= RoleManager.Instance._CurGameInfo.allZongMenData.CurStar;
        pData.theR = RoleManager.Instance._CurGameInfo.allZongMenData.TheR;

        int selfRank = 99;
        bool selfadded = false;
        int theI = 0;
        //TODO这里改成99
        for (int i = startRank; i < 99+startRank; i++)
        {
            if (i > RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count - 1)
                break;
            SingleOtherZongMenData otherData = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[i];
            if(pData.theR>= otherData.theR&&!selfadded)
            {
                rankList.Add(pData);
                selfRank = theI + 1;
                selfadded = true;
            }
            else
            {
                rankList.Add(otherData);
            }
            theI++;
        }
 
        for (int i = 0; i < 10; i++)
        {
            if (i > rankList.Count - 1)
                break;
            AddSingle<SingleRankView>(grid_rank, rankList[i], i + 1,false);
        }
        //自己名次
        AddSingle<SingleRankView>(trans_selfRank, pData, selfRank,true);
    }
    

    // Update is called once per frame
    void Update()
    {
        if (startPiPei)
        {
            piPeiTotalTimer += Time.deltaTime;
            piPeiTimer += Time.deltaTime;
            txt_timeCount.SetText(((int)piPeiTotalTimer).ToString());
            if (piPeiTimer >= piPeiTime)
            {
                piPeiTimer = 0;
                piPeiTime = RandomManager.Next(singleTimeRange[0], singleTimeRange[1]) * 0.01f;

                Transform theParent = null;
                if (curGeneratedPiPeiIndex < 4)
                {
                    theParent = trans_leftGrid;
                }else
                {
                    theParent = trans_rightGrid;
                }
                //生成一个
                matchZongMenViewList.Add(AddSingle<SingleMatchZongMenView>(theParent, otherZongMenList[curGeneratedPiPeiIndex],this));
                if (curGeneratedPiPeiIndex < otherZongMenList.Count - 1)
                {
                    curGeneratedPiPeiIndex++;
                }
                //匹配结束
                else
                {
                    startPiPei = false;
                }
            }
        }

        if (startEnterBattleReady)
        {
            enterBattleReadyTimer += Time.deltaTime;
            if (enterBattleReadyTimer > enterBattleReadyTime)
            {
                 startEnterBattleReady = false;
                 BattleManager.Instance.StartMatch(otherZongMenList);
            }
        }
        if (trans_shop.gameObject.activeInHierarchy)
        {
            BrushCDShow();
        }
    }


    #region 显示商店

    void ShowShop()
    {
        trans_shop.gameObject.SetActive(true);
        BrushCDShow();
    }

    void BrushCDShow()
    {
        long guardNextTimeDistance = CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime);
        long theNextTimeStamp = guardNextTimeDistance + RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime;
        long nowToNextTimeDistance = theNextTimeStamp - CGameTime.Instance.GetTimeStamp();
        long hour = nowToNextTimeDistance / 3600;
        long min = (nowToNextTimeDistance - hour * 3600) / 60;
        long sec = nowToNextTimeDistance - hour * 3600 - min * 60;
        txt_brushCD.SetText("下次刷新：" + hour + "时" + min + "分");
    }

    #endregion


    /// <summary>
    /// 开始匹配
    /// </summary>
    /// <param name="args"></param>
    public void StartPiPei(object[] args)
    {
        List<SingleOtherZongMenData> zongMenProtoList= args[0] as List<SingleOtherZongMenData>;
        otherZongMenList.Clear();
        //把people注册进去
        for (int i = 0; i < zongMenProtoList.Count; i++)
        {
            SingleOtherZongMenData data = zongMenProtoList[i];
            otherZongMenList.Add(data);
        }
  
        trans_piPeiPanel.gameObject.SetActive(true);
        btn_ready.gameObject.SetActive(true);
        btn_cancelReady.gameObject.SetActive(false);
        startPiPei = false;
        preparedNum = 0;
        matchZongMenViewList.Clear();
        ClearCertainParentAllSingle<SingleMatchZongMenView>(trans_leftGrid);
        ClearCertainParentAllSingle<SingleMatchZongMenView>(trans_rightGrid);


        matchZongMenViewList.Add(AddSingle<SingleMatchZongMenView>(trans_leftGrid, otherZongMenList[0],this));

        curGeneratedPiPeiIndex = 1;
        piPeiTimer = 0;
        piPeiTotalTimer = 0;
        piPeiTime = RandomManager.Next(singleTimeRange[0], singleTimeRange[1]) * 0.01f;
        startPiPei = true;

    }

    /// <summary>
    /// 已准备
    /// </summary>
    public void Prepared(bool prepared)
    {
        if (prepared)
        {
            preparedNum++;
        }
        else
        {
            preparedNum--;
        }

        if (preparedNum >= 8)
        {
            enterBattleReadyTimer = 0;

            startEnterBattleReady = true;
        }
        else
        {
            startEnterBattleReady = false;
        }
    }

    // 快速排序函数
    static void quick_sort(List<SingleOtherZongMenData> arr, int start, int length)
    {
        // 如果子数组长度大于1
        if (length > 1)
        {
            // 随机选择一个元素作为基准元素，并将其与第一个元素交换
            //Random random = new Random();

            int pivotIndex = RandomManager.Next(start, start + length);
            var temp = arr[start];
            arr[start] = arr[pivotIndex];
            arr[pivotIndex] = temp;

            // 获取基准元素的值
            var b = arr[start];

            // 定义两个指针i和j，分别从左右两端向中间扫描数组
            var i = start;
            var j = start + length - 1;

            // 当两个指针没有相遇时，循环执行
            while (i < j)
            {
                // 从右向左扫描,找到第一个大于基准的元素，或者i和j相遇时，停止扫描
                while (i < j && arr[j].theR <= b.theR)
                {
                    j--;
                }

                // 将找到的元素放到i指向的位置
                arr[i] = arr[j];

                // 从左向右扫描，找到第一个小于基准的元素，或者i和j相遇时，停止扫描
                while (i < j && arr[i].theR >= b.theR)
                {
                    i++;
                }

                // 将找到的元素放到j指向的位置
                arr[j] = arr[i];
            }

            // 当i和j相遇时，说明划分完成，将基准元素与i指向的元素交换位置
            arr[i] = b;

            // 对左右两个子数组进行递归调用，传入起始索引和长度
            quick_sort(arr, start, i - start);
            quick_sort(arr, i + 1, start + length - i - 1);
        }
    }



    public override void Clear()
    {
        base.Clear();
        startPiPei = false;
        trans_piPeiPanel.gameObject.SetActive(false);
        trans_shop.gameObject.SetActive(false);
    }
}
